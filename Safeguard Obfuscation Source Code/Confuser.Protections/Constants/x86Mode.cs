using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Constants
{
	// Token: 0x0200008D RID: 141
	internal class x86Mode : IEncodeMode
	{
		// Token: 0x0600022D RID: 557 RVA: 0x00012B30 File Offset: 0x00010D30
		public IEnumerable<Instruction> EmitDecrypt(MethodDef init, CEContext ctx, Local block, Local key)
		{
			StatementBlock encrypt;
			StatementBlock decrypt;
			ctx.DynCipher.GenerateCipherPair(ctx.Random, out encrypt, out decrypt);
			List<Instruction> ret = new List<Instruction>();
			x86Mode.CipherCodeGen codeGen = new x86Mode.CipherCodeGen(block, key, init, ret);
			codeGen.GenerateCIL(decrypt);
			codeGen.Commit(init.Body);
			DMCodeGen dmCodeGen = new DMCodeGen(typeof(void), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{BUFFER}", typeof(uint[])),
				Tuple.Create<string, Type>("{KEY}", typeof(uint[]))
			});
			dmCodeGen.GenerateCIL(encrypt);
			this.encryptFunc = dmCodeGen.Compile<Action<uint[], uint[]>>();
			return ret;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00012BDC File Offset: 0x00010DDC
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] ret = new uint[key.Length];
			Buffer.BlockCopy(data, offset * 4, ret, 0, key.Length * 4);
			this.encryptFunc(ret, key);
			return ret;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00012C18 File Offset: 0x00010E18
		public object CreateDecoder(MethodDef decoder, CEContext ctx)
		{
			x86Mode.x86Encoding encoding = new x86Mode.x86Encoding();
			encoding.Compile(ctx);
			MutationHelper.ReplacePlaceholder(decoder, delegate(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Call, encoding.native));
				return repl.ToArray();
			});
			return encoding;
		}

		// Token: 0x06000230 RID: 560 RVA: 0x00012C64 File Offset: 0x00010E64
		public uint Encode(object data, CEContext ctx, uint id)
		{
			x86Mode.x86Encoding encoding = (x86Mode.x86Encoding)data;
			return (uint)encoding.expCompiled((int)id);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00004A68 File Offset: 0x00002C68
		public x86Mode()
		{
		}

		// Token: 0x0400017E RID: 382
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x0200008E RID: 142
		private class CipherCodeGen : CILCodeGen
		{
			// Token: 0x06000232 RID: 562 RVA: 0x00005456 File Offset: 0x00003656
			public CipherCodeGen(Local block, Local key, MethodDef init, IList<Instruction> instrs) : base(init, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x06000233 RID: 563 RVA: 0x00012C8C File Offset: 0x00010E8C
			protected override Local Var(Variable var)
			{
				bool flag = var.Name == "{BUFFER}";
				Local result;
				if (flag)
				{
					result = this.block;
				}
				else
				{
					bool flag2 = var.Name == "{KEY}";
					if (flag2)
					{
						result = this.key;
					}
					else
					{
						result = base.Var(var);
					}
				}
				return result;
			}

			// Token: 0x0400017F RID: 383
			private readonly Local block;

			// Token: 0x04000180 RID: 384
			private readonly Local key;
		}

		// Token: 0x0200008F RID: 143
		private class x86Encoding
		{
			// Token: 0x06000234 RID: 564 RVA: 0x00012CE0 File Offset: 0x00010EE0
			public void Compile(CEContext ctx)
			{
				Variable var = new Variable("{VAR}");
				Variable result = new Variable("{RESULT}");
				CorLibTypeSig int32 = ctx.Module.CorLibTypes.Int32;
				this.native = new MethodDefUser("", MethodSig.CreateStatic(int32, int32), MethodAttributes.Static | MethodAttributes.PinvokeImpl);
				this.native.ImplAttributes = (MethodImplAttributes.Native | MethodImplAttributes.ManagedMask | MethodImplAttributes.PreserveSig);
				ctx.Module.GlobalType.Methods.Add(this.native);
				ctx.Name.MarkHelper(this.native, ctx.Marker, ctx.Protection);
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
					}, 4, out this.expression, out this.inverse);
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

			// Token: 0x06000235 RID: 565 RVA: 0x00012E74 File Offset: 0x00011074
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

			// Token: 0x06000236 RID: 566 RVA: 0x00004A68 File Offset: 0x00002C68
			public x86Encoding()
			{
			}

			// Token: 0x04000181 RID: 385
			private byte[] code;

			// Token: 0x04000182 RID: 386
			private dnlib.DotNet.Writer.MethodBody codeChunk;

			// Token: 0x04000183 RID: 387
			public Func<int, int> expCompiled;

			// Token: 0x04000184 RID: 388
			private Expression expression;

			// Token: 0x04000185 RID: 389
			private Expression inverse;

			// Token: 0x04000186 RID: 390
			public MethodDef native;

			// Token: 0x02000090 RID: 144
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x06000237 RID: 567 RVA: 0x00005471 File Offset: 0x00003671
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x06000238 RID: 568 RVA: 0x00004A68 File Offset: 0x00002C68
				public <>c()
				{
				}

				// Token: 0x06000239 RID: 569 RVA: 0x00012F00 File Offset: 0x00011100
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

				// Token: 0x04000187 RID: 391
				public static readonly x86Mode.x86Encoding.<>c <>9 = new x86Mode.x86Encoding.<>c();

				// Token: 0x04000188 RID: 392
				public static Func<Variable, x86Register, IEnumerable<x86Instruction>> <>9__6_0;
			}
		}

		// Token: 0x02000091 RID: 145
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_0
		{
			// Token: 0x0600023A RID: 570 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass3_0()
			{
			}

			// Token: 0x0600023B RID: 571 RVA: 0x00012F30 File Offset: 0x00011130
			internal Instruction[] <CreateDecoder>b__0(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Call, this.encoding.native));
				return repl.ToArray();
			}

			// Token: 0x04000189 RID: 393
			public x86Mode.x86Encoding encoding;
		}
	}
}
