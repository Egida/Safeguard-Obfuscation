using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000D0 RID: 208
	internal class ControlFlowPhase : ProtectionPhase
	{
		// Token: 0x0600032D RID: 813 RVA: 0x00004A51 File Offset: 0x00002C51
		public ControlFlowPhase(DupCtrlFlowModProtection parent) : base(parent)
		{
		}

		// Token: 0x0600032E RID: 814 RVA: 0x000182F8 File Offset: 0x000164F8
		private static bool DisabledOptimization(ModuleDef module)
		{
			bool flag = false;
			CustomAttribute attribute = module.Assembly.CustomAttributes.Find("System.Diagnostics.DebuggableAttribute");
			bool flag2 = attribute != null;
			if (flag2)
			{
				bool flag3 = attribute.ConstructorArguments.Count == 1;
				if (flag3)
				{
					flag |= (((int)attribute.ConstructorArguments[0].Value & 256) != 0);
				}
				else
				{
					flag |= (bool)attribute.ConstructorArguments[1].Value;
				}
			}
			attribute = module.CustomAttributes.Find("System.Diagnostics.DebuggableAttribute");
			bool flag4 = attribute == null;
			bool result;
			if (flag4)
			{
				result = flag;
			}
			else
			{
				bool flag5 = attribute.ConstructorArguments.Count == 1;
				if (flag5)
				{
					result = (flag | ((int)attribute.ConstructorArguments[0].Value & 256) != 0);
				}
				else
				{
					result = (flag | (bool)attribute.ConstructorArguments[1].Value);
				}
			}
			return result;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0001A698 File Offset: 0x00018898
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool disableOpti = ControlFlowPhase.DisabledOptimization(context.CurrentModule);
			RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.CtrlFlowMod");
			foreach (MethodDef def in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				bool flag = def.HasBody && def.Body.Instructions.Count > 0;
				if (flag)
				{
					this.ProcessMethod(def.Body, this.ParseParameters(def, context, parameters, randomGenerator, disableOpti));
					context.CheckCancellation();
				}
			}
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0001A75C File Offset: 0x0001895C
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

		// Token: 0x06000331 RID: 817 RVA: 0x0001A784 File Offset: 0x00018984
		private CFContext ParseParameters(MethodDef method, ConfuserContext context, ProtectionParameters parameters, RandomGenerator random, bool disableOpti)
		{
			CFContext ret = new CFContext();
			ret.Type = parameters.GetParameter<CFType>(context, method, "type", CFType.Switch);
			ret.Predicate = parameters.GetParameter<PredicateType>(context, method, "predicate", PredicateType.x86);
			int rawIntensity = parameters.GetParameter<int>(context, method, "intensity", 99);
			ret.Intensity = (double)rawIntensity / 100.0;
			ret.Depth = parameters.GetParameter<int>(context, method, "depth", 7);
			ret.JunkCode = (parameters.GetParameter<bool>(context, method, "junk", true) && !disableOpti);
			ret.Random = random;
			ret.Method = method;
			ret.Context = context;
			ret.DynCipher = context.Registry.GetService<IDynCipherService>();
			bool flag = ret.Predicate == PredicateType.x86 && (context.CurrentModule.Cor20HeaderFlags & ComImageFlags.ILOnly) > (ComImageFlags)0U;
			if (flag)
			{
				context.CurrentModuleWriterOptions.Cor20HeaderOptions.Flags &= ~ComImageFlags.ILOnly;
			}
			return ret;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0001A8A0 File Offset: 0x00018AA0
		private void ProcessMethod(CilBody body, CFContext ctx)
		{
			uint num;
			bool flag = !MaxStackCalculator.GetMaxStack(body.Instructions, body.ExceptionHandlers, out num);
			if (flag)
			{
				ctx.Context.Logger.Error("Failed to calcuate maxstack.");
				throw new ConfuserException(null);
			}
			body.MaxStack = (ushort)num;
			ScopeBlock root = BlockParser.ParseBody(body);
			ControlFlowPhase.GetMangler(ctx.Type).Mangle(body, root, ctx);
			body.Instructions.Clear();
			root.ToBody(body);
			foreach (ExceptionHandler handler in body.ExceptionHandlers)
			{
				int num2 = body.Instructions.IndexOf(handler.TryEnd) + 1;
				handler.TryEnd = ((num2 < body.Instructions.Count) ? body.Instructions[num2] : null);
				num2 = body.Instructions.IndexOf(handler.HandlerEnd) + 1;
				handler.HandlerEnd = ((num2 < body.Instructions.Count) ? body.Instructions[num2] : null);
			}
			body.KeepOldMaxStack = true;
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000333 RID: 819 RVA: 0x000057A4 File Offset: 0x000039A4
		public override string Name
		{
			get
			{
				return "Control flow mangling";
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000334 RID: 820 RVA: 0x00004BE0 File Offset: 0x00002DE0
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x06000335 RID: 821 RVA: 0x000059C4 File Offset: 0x00003BC4
		// Note: this type is marked as 'beforefieldinit'.
		static ControlFlowPhase()
		{
		}

		// Token: 0x04000261 RID: 609
		private static readonly JumpMangler Jump = new JumpMangler();

		// Token: 0x04000262 RID: 610
		private static readonly SwitchMangler Switch = new SwitchMangler();
	}
}
