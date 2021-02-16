using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Helpers;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x02000092 RID: 146
	internal class DynamicMode : IEncodeMode
	{
		// Token: 0x0600023C RID: 572 RVA: 0x00012F74 File Offset: 0x00011174
		public IEnumerable<Instruction> EmitDecrypt(MethodDef init, CEContext ctx, Local block, Local key)
		{
			StatementBlock encrypt;
			StatementBlock decrypt;
			ctx.DynCipher.GenerateCipherPair(ctx.Random, out encrypt, out decrypt);
			List<Instruction> ret = new List<Instruction>();
			DynamicMode.CodeGen codeGen = new DynamicMode.CodeGen(block, key, init, ret);
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

		// Token: 0x0600023D RID: 573 RVA: 0x00013020 File Offset: 0x00011220
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] ret = new uint[key.Length];
			Buffer.BlockCopy(data, offset * 4, ret, 0, key.Length * 4);
			this.encryptFunc(ret, key);
			return ret;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0001305C File Offset: 0x0001125C
		public object CreateDecoder(MethodDef decoder, CEContext ctx)
		{
			uint k1 = ctx.Random.NextUInt32() | 1U;
			uint k2 = ctx.Random.NextUInt32();
			MutationHelper.ReplacePlaceholder(decoder, delegate(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Ldc_I4, (int)MathsUtils.modInv(k1)));
				repl.Add(Instruction.Create(OpCodes.Mul));
				repl.Add(Instruction.Create(OpCodes.Ldc_I4, (int)k2));
				repl.Add(Instruction.Create(OpCodes.Xor));
				return repl.ToArray();
			});
			return Tuple.Create<uint, uint>(k1, k2);
		}

		// Token: 0x0600023F RID: 575 RVA: 0x000130BC File Offset: 0x000112BC
		public uint Encode(object data, CEContext ctx, uint id)
		{
			Tuple<uint, uint> key = (Tuple<uint, uint>)data;
			uint ret = (id ^ key.Item2) * key.Item1;
			Debug.Assert((ret * MathsUtils.modInv(key.Item1) ^ key.Item2) == id);
			return ret;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00004A68 File Offset: 0x00002C68
		public DynamicMode()
		{
		}

		// Token: 0x0400018A RID: 394
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x02000093 RID: 147
		private class CodeGen : CILCodeGen
		{
			// Token: 0x06000241 RID: 577 RVA: 0x0000547D File Offset: 0x0000367D
			public CodeGen(Local block, Local key, MethodDef init, IList<Instruction> instrs) : base(init, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x06000242 RID: 578 RVA: 0x00013104 File Offset: 0x00011304
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

			// Token: 0x0400018B RID: 395
			private readonly Local block;

			// Token: 0x0400018C RID: 396
			private readonly Local key;
		}

		// Token: 0x02000094 RID: 148
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_0
		{
			// Token: 0x06000243 RID: 579 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass3_0()
			{
			}

			// Token: 0x06000244 RID: 580 RVA: 0x00013158 File Offset: 0x00011358
			internal Instruction[] <CreateDecoder>b__0(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Ldc_I4, (int)MathsUtils.modInv(this.k1)));
				repl.Add(Instruction.Create(OpCodes.Mul));
				repl.Add(Instruction.Create(OpCodes.Ldc_I4, (int)this.k2));
				repl.Add(Instruction.Create(OpCodes.Xor));
				return repl.ToArray();
			}

			// Token: 0x0400018D RID: 397
			public uint k1;

			// Token: 0x0400018E RID: 398
			public uint k2;
		}
	}
}
