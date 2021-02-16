using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000BB RID: 187
	internal class ControlFlowPhase : ProtectionPhase
	{
		// Token: 0x060002DC RID: 732 RVA: 0x00004A51 File Offset: 0x00002C51
		public ControlFlowPhase(CtrlFlowModProtection parent) : base(parent)
		{
		}

		// Token: 0x060002DD RID: 733 RVA: 0x000182F8 File Offset: 0x000164F8
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

		// Token: 0x060002DE RID: 734 RVA: 0x00018408 File Offset: 0x00016608
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

		// Token: 0x060002DF RID: 735 RVA: 0x000184CC File Offset: 0x000166CC
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

		// Token: 0x060002E0 RID: 736 RVA: 0x000184F4 File Offset: 0x000166F4
		private CFContext ParseParameters(MethodDef method, ConfuserContext context, ProtectionParameters parameters, RandomGenerator random, bool disableOpti)
		{
			CFContext context2 = new CFContext
			{
				Type = parameters.GetParameter<CFType>(context, method, "type", CFType.Switch),
				Predicate = parameters.GetParameter<PredicateType>(context, method, "predicate", PredicateType.Normal)
			};
			int num = parameters.GetParameter<int>(context, method, "intensity", 75);
			context2.Intensity = (double)num / 20.0;
			context2.Depth = parameters.GetParameter<int>(context, method, "depth", 40);
			context2.JunkCode = (parameters.GetParameter<bool>(context, method, "junk", true) && !disableOpti);
			context2.Protection = (CtrlFlowModProtection)base.Parent;
			context2.Random = random;
			context2.Method = method;
			context2.Context = context;
			context2.DynCipher = context.Registry.GetService<IDynCipherService>();
			bool flag = context2.Predicate == PredicateType.x86 && (context.CurrentModule.Cor20HeaderFlags & ComImageFlags.ILOnly) > (ComImageFlags)0U;
			if (flag)
			{
				Cor20HeaderOptions options = context.CurrentModuleWriterOptions.Cor20HeaderOptions;
				options.Flags = new ComImageFlags?(options.Flags.Value & ~ComImageFlags.ILOnly);
			}
			return context2;
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0001860C File Offset: 0x0001680C
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

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x000057A4 File Offset: 0x000039A4
		public override string Name
		{
			get
			{
				return "Control flow mangling";
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x00004BE0 File Offset: 0x00002DE0
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x000057AB File Offset: 0x000039AB
		// Note: this type is marked as 'beforefieldinit'.
		static ControlFlowPhase()
		{
		}

		// Token: 0x04000220 RID: 544
		private static readonly JumpMangler Jump = new JumpMangler();

		// Token: 0x04000221 RID: 545
		private static readonly SwitchMangler Switch = new SwitchMangler();
	}
}
