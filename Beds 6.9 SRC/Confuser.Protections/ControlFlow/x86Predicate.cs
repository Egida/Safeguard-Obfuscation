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

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000F7 RID: 247
	internal class x86Predicate : IPredicate
	{
		// Token: 0x060003C9 RID: 969 RVA: 0x00005D71 File Offset: 0x00003F71
		public x86Predicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0001F398 File Offset: 0x0001D598
		public void Init(CilBody body)
		{
			bool flag = this.inited;
			if (!flag)
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

		// Token: 0x060003CB RID: 971 RVA: 0x00005D82 File Offset: 0x00003F82
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Call, this.encoding.native));
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0001F44C File Offset: 0x0001D64C
		public int GetSwitchKey(int key)
		{
			return this.encoding.expCompiled(key);
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00005DA1 File Offset: 0x00003FA1
		// Note: this type is marked as 'beforefieldinit'.
		static x86Predicate()
		{
		}

		// Token: 0x040002D1 RID: 721
		private static readonly object Encoding = new object();

		// Token: 0x040002D2 RID: 722
		private readonly CFContext ctx;

		// Token: 0x040002D3 RID: 723
		private x86Predicate.x86Encoding encoding;

		// Token: 0x040002D4 RID: 724
		private bool inited;

		// Token: 0x020000F8 RID: 248
		private class x86Encoding
		{
			// Token: 0x060003CE RID: 974 RVA: 0x0001F470 File Offset: 0x0001D670
			public void Compile(CFContext ctx)
			{
				Variable var = new Variable("{VAR}");
				Variable result = new Variable("{RESULT}");
				CorLibTypeSig int32 = ctx.Method.Module.CorLibTypes.Int32;
				this.native = new MethodDefUser(ctx.Context.Registry.GetService<INameService>().RandomName(), MethodSig.CreateStatic(int32, int32), MethodAttributes.Static | MethodAttributes.PinvokeImpl);
				this.native.ImplAttributes = (MethodImplAttributes.Native | MethodImplAttributes.ManagedMask | MethodImplAttributes.PreserveSig);
				ctx.Method.Module.GlobalType.Methods.Add(this.native);
				ctx.Context.Registry.GetService<IMarkerService>().Mark(this.native, ctx.Protection);
				ctx.Context.Registry.GetService<INameService>().SetCanRename(this.native, false);
				x86CodeGen codeGen = new x86CodeGen();
				x86Register? reg;
				do
				{
					ctx.DynCipher.GenerateExpressionPair(ctx.Random, new VariableExpression
					{
						Variable = var
					}, new VariableExpression
					{
						Variable = result
					}, ctx.Depth, out this.expression, out this.inverse);
					reg = codeGen.GenerateX86(this.inverse, (Variable v, x86Register r) => new x86Instruction[]
					{
						x86Instruction.Create(x86OpCode.POP, new Ix86Operand[]
						{
							new x86RegisterOperand(r)
						})
					});
				}
				while (reg == null);
				this.code = CodeGenUtils.AssembleCode(codeGen, reg.Value);
				this.expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
				{
					Tuple.Create<string, Type>("{VAR}", typeof(int))
				}).GenerateCIL(this.expression).Compile<Func<int, int>>();
				ctx.Context.CurrentModuleWriterListener.OnWriterEvent += this.InjectNativeCode;
			}

			// Token: 0x060003CF RID: 975 RVA: 0x0001F644 File Offset: 0x0001D844
			private void InjectNativeCode(object sender, ModuleWriterListenerEventArgs e)
			{
				ModuleWriterBase writer = (ModuleWriterBase)sender;
				bool flag = e.WriterEvent == ModuleWriterEvent.MDEndWriteMethodBodies;
				if (flag)
				{
					this.codeChunk = writer.MethodBodies.Add(new dnlib.DotNet.Writer.MethodBody(this.code));
				}
				else
				{
					bool flag2 = e.WriterEvent == ModuleWriterEvent.EndCalculateRvasAndFileOffsets;
					if (flag2)
					{
						uint rid = writer.MetaData.GetRid(this.native);
						writer.MetaData.TablesHeap.MethodTable[rid].RVA = (uint)this.codeChunk.RVA;
					}
				}
			}

			// Token: 0x060003D0 RID: 976 RVA: 0x00004A68 File Offset: 0x00002C68
			public x86Encoding()
			{
			}

			// Token: 0x040002D5 RID: 725
			private byte[] code;

			// Token: 0x040002D6 RID: 726
			private dnlib.DotNet.Writer.MethodBody codeChunk;

			// Token: 0x040002D7 RID: 727
			public Func<int, int> expCompiled;

			// Token: 0x040002D8 RID: 728
			private Expression expression;

			// Token: 0x040002D9 RID: 729
			private Expression inverse;

			// Token: 0x040002DA RID: 730
			public MethodDef native;

			// Token: 0x020000F9 RID: 249
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x060003D1 RID: 977 RVA: 0x00005DAD File Offset: 0x00003FAD
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x060003D2 RID: 978 RVA: 0x00004A68 File Offset: 0x00002C68
				public <>c()
				{
				}

				// Token: 0x060003D3 RID: 979 RVA: 0x00012F00 File Offset: 0x00011100
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

				// Token: 0x040002DB RID: 731
				public static readonly x86Predicate.x86Encoding.<>c <>9 = new x86Predicate.x86Encoding.<>c();

				// Token: 0x040002DC RID: 732
				public static Func<Variable, x86Register, IEnumerable<x86Instruction>> <>9__6_0;
			}
		}
	}
}
