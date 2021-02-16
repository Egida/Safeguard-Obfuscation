using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000E8 RID: 232
	internal class ControlFlowPhase : ProtectionPhase
	{
		// Token: 0x0600038C RID: 908 RVA: 0x00004A51 File Offset: 0x00002C51
		public ControlFlowPhase(ControlFlowProtection parent) : base(parent)
		{
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600038D RID: 909 RVA: 0x00009294 File Offset: 0x00007494
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600038E RID: 910 RVA: 0x0001D3AC File Offset: 0x0001B5AC
		public override string Name
		{
			get
			{
				return "Control flow mangling";
			}
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0001D3C4 File Offset: 0x0001B5C4
		private CFContext ParseParameters(MethodDef method, ConfuserContext context, ProtectionParameters parameters, RandomGenerator random, bool disableOpti)
		{
			CFContext ret = new CFContext();
			ret.Type = parameters.GetParameter<CFType>(context, method, "type", CFType.Switch);
			ret.Predicate = parameters.GetParameter<PredicateType>(context, method, "predicate", PredicateType.Expression);
			int rawIntensity = parameters.GetParameter<int>(context, method, "intensity", 120);
			ret.Intensity = (double)rawIntensity / 120.0;
			ret.Depth = parameters.GetParameter<int>(context, method, "depth", 20);
			ret.JunkCode = (parameters.GetParameter<bool>(context, method, "junk", true) && !disableOpti);
			ret.Protection = (ControlFlowProtection)base.Parent;
			ret.Random = random;
			ret.Method = method;
			ret.Context = context;
			ret.DynCipher = context.Registry.GetService<IDynCipherService>();
			bool flag = ret.Predicate == PredicateType.x86;
			if (flag)
			{
				bool flag2 = (context.CurrentModule.Cor20HeaderFlags & ComImageFlags.ILOnly) > (ComImageFlags)0U;
				if (flag2)
				{
					context.CurrentModuleWriterOptions.Cor20HeaderOptions.Flags &= ~ComImageFlags.ILOnly;
				}
			}
			return ret;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0001D4F4 File Offset: 0x0001B6F4
		private static bool DisabledOptimization(ModuleDef module)
		{
			bool disableOpti = false;
			CustomAttribute debugAttr = module.Assembly.CustomAttributes.Find("System.Diagnostics.DebuggableAttribute");
			bool flag = debugAttr != null;
			if (flag)
			{
				bool flag2 = debugAttr.ConstructorArguments.Count == 1;
				if (flag2)
				{
					disableOpti |= (((int)debugAttr.ConstructorArguments[0].Value & 256) != 0);
				}
				else
				{
					disableOpti |= (bool)debugAttr.ConstructorArguments[1].Value;
				}
			}
			debugAttr = module.CustomAttributes.Find("System.Diagnostics.DebuggableAttribute");
			bool flag3 = debugAttr != null;
			if (flag3)
			{
				bool flag4 = debugAttr.ConstructorArguments.Count == 1;
				if (flag4)
				{
					disableOpti |= (((int)debugAttr.ConstructorArguments[0].Value & 256) != 0);
				}
				else
				{
					disableOpti |= (bool)debugAttr.ConstructorArguments[1].Value;
				}
			}
			return disableOpti;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0001D5F8 File Offset: 0x0001B7F8
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool disabledOpti = ControlFlowPhase.DisabledOptimization(context.CurrentModule);
			RandomGenerator random = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.ControlFlow");
			foreach (MethodDef method in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				bool flag = method.HasBody && method.Body.Instructions.Count > 0;
				if (flag)
				{
					this.ProcessMethod(method.Body, this.ParseParameters(method, context, parameters, random, disabledOpti));
					context.CheckCancellation();
				}
			}
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0001D6B8 File Offset: 0x0001B8B8
		private static ManglerBase GetMangler(CFType type)
		{
			bool flag = type == CFType.Switch;
			ManglerBase result;
			if (flag)
			{
				result = ControlFlowPhase.Switch;
			}
			else
			{
				result = ControlFlowPhase.Jump;
			}
			return result;
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0001D6E0 File Offset: 0x0001B8E0
		private void ProcessMethod(CilBody body, CFContext ctx)
		{
			uint maxStack;
			bool flag = !MaxStackCalculator.GetMaxStack(body.Instructions, body.ExceptionHandlers, out maxStack);
			if (flag)
			{
				ctx.Context.Logger.Error("Failed to calcuate maxstack.");
				throw new ConfuserException(null);
			}
			body.MaxStack = (ushort)maxStack;
			ScopeBlock root = BlockParser.ParseBody(body);
			ControlFlowPhase.GetMangler(ctx.Type).Mangle(body, root, ctx);
			body.Instructions.Clear();
			root.ToBody(body);
			foreach (ExceptionHandler eh in body.ExceptionHandlers)
			{
				int index = body.Instructions.IndexOf(eh.TryEnd) + 1;
				eh.TryEnd = ((index < body.Instructions.Count) ? body.Instructions[index] : null);
				index = body.Instructions.IndexOf(eh.HandlerEnd) + 1;
				eh.HandlerEnd = ((index < body.Instructions.Count) ? body.Instructions[index] : null);
			}
			body.KeepOldMaxStack = true;
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00005C46 File Offset: 0x00003E46
		// Note: this type is marked as 'beforefieldinit'.
		static ControlFlowPhase()
		{
		}

		// Token: 0x040002A9 RID: 681
		private static readonly JumpMangler Jump = new JumpMangler();

		// Token: 0x040002AA RID: 682
		private static readonly SwitchMangler Switch = new SwitchMangler();
	}
}
