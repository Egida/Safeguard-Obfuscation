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

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000C8 RID: 200
	internal class x86Predicate : IPredicate
	{
		// Token: 0x06000318 RID: 792 RVA: 0x00005935 File Offset: 0x00003B35
		public x86Predicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00005946 File Offset: 0x00003B46
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Call, this.encoding.native));
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00005965 File Offset: 0x00003B65
		public int GetSwitchKey(int key)
		{
			return this.encoding.expCompiled(key);
		}

		// Token: 0x0600031B RID: 795 RVA: 0x00019BE8 File Offset: 0x00017DE8
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

		// Token: 0x0600031C RID: 796 RVA: 0x00005978 File Offset: 0x00003B78
		// Note: this type is marked as 'beforefieldinit'.
		static x86Predicate()
		{
		}

		// Token: 0x0400023F RID: 575
		private readonly CFContext ctx;

		// Token: 0x04000240 RID: 576
		private x86Predicate.x86Encoding encoding;

		// Token: 0x04000241 RID: 577
		private static readonly object Encoding = new object();

		// Token: 0x04000242 RID: 578
		private bool inited;

		// Token: 0x020000C9 RID: 201
		private class x86Encoding
		{
			// Token: 0x0600031D RID: 797 RVA: 0x00019CA0 File Offset: 0x00017EA0
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

			// Token: 0x0600031E RID: 798 RVA: 0x00019E80 File Offset: 0x00018080
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

			// Token: 0x0600031F RID: 799 RVA: 0x00004A68 File Offset: 0x00002C68
			public x86Encoding()
			{
			}

			// Token: 0x04000243 RID: 579
			private byte[] code;

			// Token: 0x04000244 RID: 580
			private dnlib.DotNet.Writer.MethodBody codeChunk;

			// Token: 0x04000245 RID: 581
			public Func<int, int> expCompiled;

			// Token: 0x04000246 RID: 582
			private Expression expression;

			// Token: 0x04000247 RID: 583
			private Expression inverse;

			// Token: 0x04000248 RID: 584
			public MethodDef native;

			// Token: 0x020000CA RID: 202
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x06000320 RID: 800 RVA: 0x00005984 File Offset: 0x00003B84
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x06000321 RID: 801 RVA: 0x00004A68 File Offset: 0x00002C68
				public <>c()
				{
				}

				// Token: 0x06000322 RID: 802 RVA: 0x000051E1 File Offset: 0x000033E1
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

				// Token: 0x04000249 RID: 585
				public static readonly x86Predicate.x86Encoding.<>c <>9 = new x86Predicate.x86Encoding.<>c();

				// Token: 0x0400024A RID: 586
				public static Func<Variable, x86Register, IEnumerable<x86Instruction>> <>9__6_0;
			}
		}
	}
}
