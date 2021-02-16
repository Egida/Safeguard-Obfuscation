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

namespace Confuser.Protections.MildReferenceProxy
{
	// Token: 0x02000077 RID: 119
	internal class MildReferenceProxyPhase : ProtectionPhase
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x00004A51 File Offset: 0x00002C51
		public MildReferenceProxyPhase(MildReferenceProxyProtection parent) : base(parent)
		{
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x00009294 File Offset: 0x00007494
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000F2F0 File Offset: 0x0000D4F0
		public override string Name
		{
			get
			{
				return "Encoding reference proxies";
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000F8BC File Offset: 0x0000DABC
		private RPContext ParseParameters(MethodDef method, ConfuserContext context, ProtectionParameters parameters, MildReferenceProxyPhase.RPStore store)
		{
			RPContext ret = new RPContext();
			ret.Mode = parameters.GetParameter<Mode>(context, method, "mode", Mode.Mild);
			ret.Encoding = parameters.GetParameter<EncodingType>(context, method, "encoding", EncodingType.Expression);
			ret.InternalAlso = parameters.GetParameter<bool>(context, method, "internal", true);
			ret.TypeErasure = parameters.GetParameter<bool>(context, method, "typeErasure", true);
			ret.Depth = parameters.GetParameter<int>(context, method, "depth", 15);
			ret.Module = method.Module;
			ret.Method = method;
			ret.Body = method.Body;
			ret.BranchTargets = new HashSet<Instruction>(from target in (from instr in method.Body.Instructions
			select instr.Operand as Instruction).Concat((from instr in method.Body.Instructions
			where instr.Operand is Instruction[]
			select instr).SelectMany((Instruction instr) => (Instruction[])instr.Operand))
			where target != null
			select target);
			ret.Protection = (MildReferenceProxyProtection)base.Parent;
			ret.Random = store.random;
			ret.Context = context;
			ret.Marker = context.Registry.GetService<IMarkerService>();
			ret.DynCipher = context.Registry.GetService<IDynCipherService>();
			ret.Name = context.Registry.GetService<INameService>();
			ret.Delegates = store.delegates;
			Mode mode = ret.Mode;
			if (mode != Mode.Strong)
			{
				if (mode != Mode.Mild)
				{
					throw new UnreachableException();
				}
				RPContext rpcontext = ret;
				MildMode modeHandler;
				if ((modeHandler = store.mild) == null)
				{
					modeHandler = (store.mild = new MildMode());
				}
				rpcontext.ModeHandler = modeHandler;
			}
			else
			{
				RPContext rpcontext2 = ret;
				StrongMode modeHandler2;
				if ((modeHandler2 = store.strong) == null)
				{
					modeHandler2 = (store.strong = new StrongMode());
				}
				rpcontext2.ModeHandler = modeHandler2;
			}
			switch (ret.Encoding)
			{
			case EncodingType.x86:
			{
				RPContext rpcontext3 = ret;
				x86Encoding encodingHandler;
				if ((encodingHandler = store.x86) == null)
				{
					encodingHandler = (store.x86 = new x86Encoding());
				}
				rpcontext3.EncodingHandler = encodingHandler;
				bool flag = (context.CurrentModule.Cor20HeaderFlags & ComImageFlags.ILOnly) > (ComImageFlags)0U;
				if (flag)
				{
					context.CurrentModuleWriterOptions.Cor20HeaderOptions.Flags &= ~ComImageFlags.ILOnly;
				}
				break;
			}
			case EncodingType.Expression:
			{
				RPContext rpcontext4 = ret;
				ExpressionEncoding encodingHandler2;
				if ((encodingHandler2 = store.expression) == null)
				{
					encodingHandler2 = (store.expression = new ExpressionEncoding());
				}
				rpcontext4.EncodingHandler = encodingHandler2;
				break;
			}
			case EncodingType.Normal:
			{
				RPContext rpcontext5 = ret;
				NormalEncoding encodingHandler3;
				if ((encodingHandler3 = store.normal) == null)
				{
					encodingHandler3 = (store.normal = new NormalEncoding());
				}
				rpcontext5.EncodingHandler = encodingHandler3;
				break;
			}
			default:
				throw new UnreachableException();
			}
			return ret;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000FBBC File Offset: 0x0000DDBC
		private static RPContext ParseParameters(ModuleDef module, ConfuserContext context, ProtectionParameters parameters, MildReferenceProxyPhase.RPStore store)
		{
			return new RPContext
			{
				Depth = parameters.GetParameter<int>(context, module, "depth", 15),
				InitCount = parameters.GetParameter<int>(context, module, "initCount", 25),
				Random = store.random,
				Module = module,
				Context = context,
				Marker = context.Registry.GetService<IMarkerService>(),
				DynCipher = context.Registry.GetService<IDynCipherService>(),
				Name = context.Registry.GetService<INameService>(),
				Delegates = store.delegates
			};
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000FC58 File Offset: 0x0000DE58
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			RandomGenerator random = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.RefProxy");
			MildReferenceProxyPhase.RPStore store = new MildReferenceProxyPhase.RPStore
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
			RPContext ctx = MildReferenceProxyPhase.ParseParameters(context.CurrentModule, context, parameters, store);
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

		// Token: 0x060001E6 RID: 486 RVA: 0x0000FD60 File Offset: 0x0000DF60
		private void ProcessMethod(RPContext ctx)
		{
			int i = 0;
			while (i < ctx.Body.Instructions.Count)
			{
				Instruction instr = ctx.Body.Instructions[i];
				bool flag = instr.OpCode.Code == Code.Call || instr.OpCode.Code == Code.Callvirt || instr.OpCode.Code == Code.Newobj;
				if (flag)
				{
					IMethod operand = (IMethod)instr.Operand;
					MethodDef def = operand.ResolveMethodDef();
					bool flag2 = def != null && ctx.Context.Annotations.Get<object>(def, ReferenceProxyProtection.TargetExcluded, null) != null;
					if (flag2)
					{
						break;
					}
					bool flag3 = instr.OpCode.Code != Code.Newobj && operand.Name == ".ctor";
					if (!flag3)
					{
						bool flag4 = operand is MethodDef && !ctx.InternalAlso;
						if (!flag4)
						{
							bool flag5 = operand is MethodSpec;
							if (!flag5)
							{
								bool flag6 = operand.DeclaringType is TypeSpec;
								if (!flag6)
								{
									bool flag7 = operand.MethodSig.ParamsAfterSentinel != null && operand.MethodSig.ParamsAfterSentinel.Count > 0;
									if (!flag7)
									{
										TypeDef declType = operand.DeclaringType.ResolveTypeDefThrow();
										bool flag8 = declType.IsDelegate();
										if (!flag8)
										{
											bool flag9 = declType.IsValueType && operand.MethodSig.HasThis;
											if (flag9)
											{
												break;
											}
											bool flag10 = i - 1 >= 0 && ctx.Body.Instructions[i - 1].OpCode.OpCodeType == OpCodeType.Prefix;
											if (!flag10)
											{
												ctx.ModeHandler.ProcessCall(ctx, i);
											}
										}
									}
								}
							}
						}
					}
				}
				IL_1B4:
				i++;
				continue;
				goto IL_1B4;
			}
		}

		// Token: 0x02000078 RID: 120
		private class RPStore
		{
			// Token: 0x060001E7 RID: 487 RVA: 0x00005367 File Offset: 0x00003567
			public RPStore()
			{
			}

			// Token: 0x0400011A RID: 282
			public readonly Dictionary<MethodSig, TypeDef> delegates = new Dictionary<MethodSig, TypeDef>(new MildReferenceProxyPhase.RPStore.MethodSigComparer());

			// Token: 0x0400011B RID: 283
			public ExpressionEncoding expression;

			// Token: 0x0400011C RID: 284
			public MildMode mild;

			// Token: 0x0400011D RID: 285
			public NormalEncoding normal;

			// Token: 0x0400011E RID: 286
			public RandomGenerator random;

			// Token: 0x0400011F RID: 287
			public StrongMode strong;

			// Token: 0x04000120 RID: 288
			public x86Encoding x86;

			// Token: 0x02000079 RID: 121
			private class MethodSigComparer : IEqualityComparer<MethodSig>
			{
				// Token: 0x060001E8 RID: 488 RVA: 0x0000F308 File Offset: 0x0000D508
				public bool Equals(MethodSig x, MethodSig y)
				{
					return default(SigComparer).Equals(x, y);
				}

				// Token: 0x060001E9 RID: 489 RVA: 0x0000F330 File Offset: 0x0000D530
				public int GetHashCode(MethodSig obj)
				{
					return default(SigComparer).GetHashCode(obj);
				}

				// Token: 0x060001EA RID: 490 RVA: 0x00004A68 File Offset: 0x00002C68
				public MethodSigComparer()
				{
				}
			}
		}

		// Token: 0x0200007A RID: 122
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060001EB RID: 491 RVA: 0x00005380 File Offset: 0x00003580
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060001EC RID: 492 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x060001ED RID: 493 RVA: 0x000052D9 File Offset: 0x000034D9
			internal Instruction <ParseParameters>b__5_0(Instruction instr)
			{
				return instr.Operand as Instruction;
			}

			// Token: 0x060001EE RID: 494 RVA: 0x000052E6 File Offset: 0x000034E6
			internal bool <ParseParameters>b__5_1(Instruction instr)
			{
				return instr.Operand is Instruction[];
			}

			// Token: 0x060001EF RID: 495 RVA: 0x000052F6 File Offset: 0x000034F6
			internal IEnumerable<Instruction> <ParseParameters>b__5_2(Instruction instr)
			{
				return (Instruction[])instr.Operand;
			}

			// Token: 0x060001F0 RID: 496 RVA: 0x0000529E File Offset: 0x0000349E
			internal bool <ParseParameters>b__5_3(Instruction target)
			{
				return target != null;
			}

			// Token: 0x04000121 RID: 289
			public static readonly MildReferenceProxyPhase.<>c <>9 = new MildReferenceProxyPhase.<>c();

			// Token: 0x04000122 RID: 290
			public static Func<Instruction, Instruction> <>9__5_0;

			// Token: 0x04000123 RID: 291
			public static Func<Instruction, bool> <>9__5_1;

			// Token: 0x04000124 RID: 292
			public static Func<Instruction, IEnumerable<Instruction>> <>9__5_2;

			// Token: 0x04000125 RID: 293
			public static Func<Instruction, bool> <>9__5_3;
		}
	}
}
