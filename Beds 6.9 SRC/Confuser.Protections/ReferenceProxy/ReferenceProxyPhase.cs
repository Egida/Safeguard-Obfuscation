using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x02000069 RID: 105
	internal class ReferenceProxyPhase : ProtectionPhase
	{
		// Token: 0x060001BE RID: 446 RVA: 0x00004A51 File Offset: 0x00002C51
		public ReferenceProxyPhase(ReferenceProxyProtection parent) : base(parent)
		{
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000EC60 File Offset: 0x0000CE60
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			RandomGenerator random = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.RefProxy");
			ReferenceProxyPhase.RPStore store = new ReferenceProxyPhase.RPStore
			{
				random = random
			};
			foreach (MethodDef method in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				bool flag = method.HasBody && method.Body.Instructions.Count > 0;
				if (flag)
				{
					this.ProcessMethod(this.ParseParameters(method, context, parameters, store));
					context.CheckCancellation();
				}
			}
			RPContext ctx = ReferenceProxyPhase.ParseParameters(context.CurrentModule, context, parameters, store);
			bool flag2 = store.mild != null;
			if (flag2)
			{
				store.mild.Finalize(ctx);
			}
			bool flag3 = store.strong != null;
			if (flag3)
			{
				store.strong.Finalize(ctx);
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000ED6C File Offset: 0x0000CF6C
		private RPContext ParseParameters(MethodDef method, ConfuserContext context, ProtectionParameters parameters, ReferenceProxyPhase.RPStore store)
		{
			RPContext ret = new RPContext();
			ret.Mode = parameters.GetParameter<Mode>(context, method, "mode", Mode.Strong);
			ret.Encoding = parameters.GetParameter<EncodingType>(context, method, "encoding", EncodingType.x86);
			ret.InternalAlso = parameters.GetParameter<bool>(context, method, "internal", true);
			ret.TypeErasure = parameters.GetParameter<bool>(context, method, "typeErasure", true);
			ret.Depth = parameters.GetParameter<int>(context, method, "depth", 20);
			ret.Module = method.Module;
			ret.Method = method;
			ret.Body = method.Body;
			ret.BranchTargets = new HashSet<Instruction>(from target in (from instr in method.Body.Instructions
			select instr.Operand as Instruction).Concat((from instr in method.Body.Instructions
			where instr.Operand is Instruction[]
			select instr).SelectMany((Instruction instr) => (Instruction[])instr.Operand))
			where target != null
			select target);
			ret.Protection = (ReferenceProxyProtection)base.Parent;
			ret.Random = store.random;
			ret.Context = context;
			ret.Marker = context.Registry.GetService<IMarkerService>();
			ret.DynCipher = context.Registry.GetService<IDynCipherService>();
			ret.Name = context.Registry.GetService<INameService>();
			ret.Delegates = store.delegates;
			Mode mode = ret.Mode;
			if (mode != Mode.Mild)
			{
				if (mode != Mode.Strong)
				{
					throw new UnreachableException();
				}
				RPContext arg_1F1_0 = ret;
				StrongMode arg_1F1_;
				bool flag = (arg_1F1_ = store.strong) == null;
				if (flag)
				{
					arg_1F1_ = (store.strong = new StrongMode());
				}
				arg_1F1_0.ModeHandler = arg_1F1_;
			}
			else
			{
				RPContext arg_1CF_0 = ret;
				MildMode arg_1CF_;
				bool flag2 = (arg_1CF_ = store.mild) == null;
				if (flag2)
				{
					arg_1CF_ = (store.mild = new MildMode());
				}
				arg_1CF_0.ModeHandler = arg_1CF_;
			}
			switch (ret.Encoding)
			{
			case EncodingType.Normal:
			{
				RPContext arg_23B_0 = ret;
				NormalEncoding arg_23B_;
				bool flag3 = (arg_23B_ = store.normal) == null;
				if (flag3)
				{
					arg_23B_ = (store.normal = new NormalEncoding());
				}
				arg_23B_0.EncodingHandler = arg_23B_;
				break;
			}
			case EncodingType.Expression:
			{
				RPContext arg_262_0 = ret;
				ExpressionEncoding arg_262_;
				bool flag4 = (arg_262_ = store.expression) == null;
				if (flag4)
				{
					arg_262_ = (store.expression = new ExpressionEncoding());
				}
				arg_262_0.EncodingHandler = arg_262_;
				break;
			}
			case EncodingType.x86:
			{
				RPContext arg_286_0 = ret;
				x86Encoding arg_286_;
				bool flag5 = (arg_286_ = store.x86) == null;
				if (flag5)
				{
					arg_286_ = (store.x86 = new x86Encoding());
				}
				arg_286_0.EncodingHandler = arg_286_;
				bool flag6 = (context.CurrentModule.Cor20HeaderFlags & ComImageFlags.ILOnly) > (ComImageFlags)0U;
				if (flag6)
				{
					context.CurrentModuleWriterOptions.Cor20HeaderOptions.Flags &= ~ComImageFlags.ILOnly;
				}
				break;
			}
			default:
				throw new UnreachableException();
			}
			return ret;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000F0C8 File Offset: 0x0000D2C8
		private static RPContext ParseParameters(ModuleDef module, ConfuserContext context, ProtectionParameters parameters, ReferenceProxyPhase.RPStore store)
		{
			return new RPContext
			{
				Depth = parameters.GetParameter<int>(context, module, "depth", 20),
				InitCount = parameters.GetParameter<int>(context, module, "initCount", 32),
				Random = store.random,
				Module = module,
				Context = context,
				Marker = context.Registry.GetService<IMarkerService>(),
				DynCipher = context.Registry.GetService<IDynCipherService>(),
				Name = context.Registry.GetService<INameService>(),
				Delegates = store.delegates
			};
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000F164 File Offset: 0x0000D364
		private void ProcessMethod(RPContext ctx)
		{
			for (int i = 0; i < ctx.Body.Instructions.Count; i++)
			{
				Instruction instr = ctx.Body.Instructions[i];
				bool flag = instr.OpCode.Code == Code.Call || instr.OpCode.Code == Code.Callvirt || instr.OpCode.Code == Code.Newobj;
				if (flag)
				{
					IMethod operand = (IMethod)instr.Operand;
					bool flag2 = (instr.OpCode.Code == Code.Newobj || !(operand.Name == ".ctor")) && (!(operand is MethodDef) || ctx.InternalAlso) && !(operand is MethodSpec) && !(operand.DeclaringType is TypeSpec) && (operand.MethodSig.ParamsAfterSentinel == null || operand.MethodSig.ParamsAfterSentinel.Count <= 0);
					if (flag2)
					{
						TypeDef declType = operand.DeclaringType.ResolveTypeDefThrow();
						bool flag3 = !declType.IsDelegate();
						if (flag3)
						{
							bool flag4 = declType.IsValueType && operand.MethodSig.HasThis;
							if (flag4)
							{
								break;
							}
							bool flag5 = i - 1 < 0 || ctx.Body.Instructions[i - 1].OpCode.OpCodeType != OpCodeType.Prefix;
							if (flag5)
							{
								ctx.ModeHandler.ProcessCall(ctx, i);
							}
						}
					}
				}
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x0000F2F0 File Offset: 0x0000D4F0
		public override string Name
		{
			get
			{
				return "Encoding reference proxies";
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x00009294 File Offset: 0x00007494
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x0200006A RID: 106
		private class RPStore
		{
			// Token: 0x060001C5 RID: 453 RVA: 0x000052B4 File Offset: 0x000034B4
			public RPStore()
			{
			}

			// Token: 0x040000E7 RID: 231
			public readonly Dictionary<MethodSig, TypeDef> delegates = new Dictionary<MethodSig, TypeDef>(new ReferenceProxyPhase.RPStore.MethodSigComparer());

			// Token: 0x040000E8 RID: 232
			public ExpressionEncoding expression;

			// Token: 0x040000E9 RID: 233
			public MildMode mild;

			// Token: 0x040000EA RID: 234
			public NormalEncoding normal;

			// Token: 0x040000EB RID: 235
			public RandomGenerator random;

			// Token: 0x040000EC RID: 236
			public StrongMode strong;

			// Token: 0x040000ED RID: 237
			public x86Encoding x86;

			// Token: 0x0200006B RID: 107
			private class MethodSigComparer : IEqualityComparer<MethodSig>
			{
				// Token: 0x060001C6 RID: 454 RVA: 0x0000F308 File Offset: 0x0000D508
				public bool Equals(MethodSig x, MethodSig y)
				{
					return default(SigComparer).Equals(x, y);
				}

				// Token: 0x060001C7 RID: 455 RVA: 0x0000F330 File Offset: 0x0000D530
				public int GetHashCode(MethodSig obj)
				{
					return default(SigComparer).GetHashCode(obj);
				}

				// Token: 0x060001C8 RID: 456 RVA: 0x00004A68 File Offset: 0x00002C68
				public MethodSigComparer()
				{
				}
			}
		}

		// Token: 0x0200006C RID: 108
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060001C9 RID: 457 RVA: 0x000052CD File Offset: 0x000034CD
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060001CA RID: 458 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x060001CB RID: 459 RVA: 0x000052D9 File Offset: 0x000034D9
			internal Instruction <ParseParameters>b__2_0(Instruction instr)
			{
				return instr.Operand as Instruction;
			}

			// Token: 0x060001CC RID: 460 RVA: 0x000052E6 File Offset: 0x000034E6
			internal bool <ParseParameters>b__2_1(Instruction instr)
			{
				return instr.Operand is Instruction[];
			}

			// Token: 0x060001CD RID: 461 RVA: 0x000052F6 File Offset: 0x000034F6
			internal IEnumerable<Instruction> <ParseParameters>b__2_2(Instruction instr)
			{
				return (Instruction[])instr.Operand;
			}

			// Token: 0x060001CE RID: 462 RVA: 0x0000529E File Offset: 0x0000349E
			internal bool <ParseParameters>b__2_3(Instruction target)
			{
				return target != null;
			}

			// Token: 0x040000EE RID: 238
			public static readonly ReferenceProxyPhase.<>c <>9 = new ReferenceProxyPhase.<>c();

			// Token: 0x040000EF RID: 239
			public static Func<Instruction, Instruction> <>9__2_0;

			// Token: 0x040000F0 RID: 240
			public static Func<Instruction, bool> <>9__2_1;

			// Token: 0x040000F1 RID: 241
			public static Func<Instruction, IEnumerable<Instruction>> <>9__2_2;

			// Token: 0x040000F2 RID: 242
			public static Func<Instruction, bool> <>9__2_3;
		}
	}
}
