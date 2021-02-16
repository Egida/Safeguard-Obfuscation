using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000DD RID: 221
	internal class x86Predicate : IPredicate
	{
		// Token: 0x06000369 RID: 873 RVA: 0x00005B4E File Offset: 0x00003D4E
		public x86Predicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00005B5F File Offset: 0x00003D5F
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Call, this.encoding.native));
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00005B7E File Offset: 0x00003D7E
		public int GetSwitchKey(int key)
		{
			return this.encoding.expCompiled(key);
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0001BDC4 File Offset: 0x00019FC4
		public void Init(CilBody body)
		{
			bool flag = !this.inited;
			if (flag)
			{
				this.encoding = this.ctx.Context.Annotations.Get<x86Predicate.x86Encoding>(this.ctx.Method.DeclaringType, x86Predicate.Encoding, null);
				bool flag2 = this.encoding == null;
				if (flag2)
				{
					this.encoding = new x86Predicate.x86Encoding();
					this.encoding.Compile(this.ctx);
					this.ctx.Context.Annotations.Set<x86Predicate.x86Encoding>(this.ctx.Method.DeclaringType, x86Predicate.Encoding, this.encoding);
				}
				this.inited = true;
			}
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00005B91 File Offset: 0x00003D91
		// Note: this type is marked as 'beforefieldinit'.
		static x86Predicate()
		{
		}

		// Token: 0x04000280 RID: 640
		private readonly CFContext ctx;

		// Token: 0x04000281 RID: 641
		private x86Predicate.x86Encoding encoding;

		// Token: 0x04000282 RID: 642
		private static readonly object Encoding = new object();

		// Token: 0x04000283 RID: 643
		private bool inited;

		// Token: 0x020000DE RID: 222
		private class x86Encoding
		{
			// Token: 0x0600036E RID: 878 RVA: 0x0001BE7C File Offset: 0x0001A07C
			public void Compile(CFContext ctx)
			{
				Variable variable = new Variable("{VAR}");
				Variable variable2 = new Variable("{RESULT}");
				CorLibTypeSig retType = ctx.Method.Module.CorLibTypes.Int32;
				this.native = new MethodDefUser(ctx.Context.Registry.GetService<INameService>().RandomName(), MethodSig.CreateStatic(retType, retType), MethodAttributes.Static | MethodAttributes.PinvokeImpl);
				this.native.ImplAttributes = (MethodImplAttributes.Native | MethodImplAttributes.ManagedMask | MethodImplAttributes.PreserveSig);
				ctx.Method.Module.GlobalType.Methods.Add(this.native);
				ctx.Context.Registry.GetService<IMarkerService>().Mark(this.native, ctx.Protection);
				ctx.Context.Registry.GetService<INameService>().SetCanRename(this.native, false);
				x86CodeGen codeGen = new x86CodeGen();
				x86Register? nullable;
				do
				{
					VariableExpression var = new VariableExpression
					{
						Variable = variable
					};
					VariableExpression result = new VariableExpression
					{
						Variable = variable2
					};
					ctx.DynCipher.GenerateExpressionPair(ctx.Random, var, result, ctx.Depth, out this.expression, out this.inverse);
					nullable = codeGen.GenerateX86(this.inverse, (Variable v, x86Register r) => new x86Instruction[]
					{
						x86Instruction.Create(x86OpCode.POP, new Ix86Operand[]
						{
							new x86RegisterOperand(r)
						})
					});
				}
				while (nullable == null);
				this.code = CodeGenUtils.AssembleCode(codeGen, nullable.Value);
				this.expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
				{
					Tuple.Create<string, Type>("{VAR}", typeof(int))
				}).GenerateCIL(this.expression).Compile<Func<int, int>>();
				ctx.Context.CurrentModuleWriterListener.OnWriterEvent += this.InjectNativeCode;
			}

			// Token: 0x0600036F RID: 879 RVA: 0x0001C05C File Offset: 0x0001A25C
			private void InjectNativeCode(object sender, ModuleWriterListenerEventArgs e)
			{
				ModuleWriterBase base2 = (ModuleWriterBase)sender;
				bool flag = e.WriterEvent == ModuleWriterEvent.MDEndWriteMethodBodies;
				if (flag)
				{
					this.codeChunk = base2.MethodBodies.Add(new dnlib.DotNet.Writer.MethodBody(this.code));
				}
				else
				{
					bool flag2 = e.WriterEvent == ModuleWriterEvent.EndCalculateRvasAndFileOffsets;
					if (flag2)
					{
						uint rid = base2.MetaData.GetRid(this.native);
						base2.MetaData.TablesHeap.MethodTable[rid].RVA = (uint)this.codeChunk.RVA;
					}
				}
			}

			// Token: 0x06000370 RID: 880 RVA: 0x00004A68 File Offset: 0x00002C68
			public x86Encoding()
			{
			}

			// Token: 0x04000284 RID: 644
			private byte[] code;

			// Token: 0x04000285 RID: 645
			private dnlib.DotNet.Writer.MethodBody codeChunk;

			// Token: 0x04000286 RID: 646
			public Func<int, int> expCompiled;

			// Token: 0x04000287 RID: 647
			private Expression expression;

			// Token: 0x04000288 RID: 648
			private Expression inverse;

			// Token: 0x04000289 RID: 649
			public MethodDef native;

			// Token: 0x020000DF RID: 223
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x06000371 RID: 881 RVA: 0x00005B9D File Offset: 0x00003D9D
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x06000372 RID: 882 RVA: 0x00004A68 File Offset: 0x00002C68
				public <>c()
				{
				}

				// Token: 0x06000373 RID: 883 RVA: 0x000051E1 File Offset: 0x000033E1
				internal IEnumerable<x86Instruction> <Compile>b__6_0(Variable v, x86Register r)
				{
					return new x86Instruction[]
					{
						x86Instruction.Create(x86OpCode.POP, new Ix86Operand[]
						{
							new x86RegisterOperand(r)
						})
					};
				}

				// Token: 0x0400028A RID: 650
				public static readonly x86Predicate.x86Encoding.<>c <>9 = new x86Predicate.x86Encoding.<>c();

				// Token: 0x0400028B RID: 651
				public static Func<Variable, x86Register, IEnumerable<x86Instruction>> <>9__6_0;
			}
		}
	}
}
