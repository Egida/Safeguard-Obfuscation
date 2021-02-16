using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources2
{
	// Token: 0x0200004A RID: 74
	internal class DynamicMode : IEncodeMode
	{
		// Token: 0x0600015B RID: 347 RVA: 0x0000BB90 File Offset: 0x00009D90
		public IEnumerable<Instruction> EmitDecrypt(MethodDef init, REContext ctx, Local block, Local key)
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

		// Token: 0x0600015C RID: 348 RVA: 0x0000BC3C File Offset: 0x00009E3C
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] ret = new uint[key.Length];
			Buffer.BlockCopy(data, offset * 4, ret, 0, key.Length * 4);
			this.encryptFunc(ret, key);
			return ret;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00004A68 File Offset: 0x00002C68
		public DynamicMode()
		{
		}

		// Token: 0x04000096 RID: 150
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x0200004B RID: 75
		private class CodeGen : CILCodeGen
		{
			// Token: 0x0600015E RID: 350 RVA: 0x000050BF File Offset: 0x000032BF
			public CodeGen(Local block, Local key, MethodDef init, IList<Instruction> instrs) : base(init, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x0600015F RID: 351 RVA: 0x0000BC78 File Offset: 0x00009E78
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

			// Token: 0x04000097 RID: 151
			private readonly Local block;

			// Token: 0x04000098 RID: 152
			private readonly Local key;
		}
	}
}
