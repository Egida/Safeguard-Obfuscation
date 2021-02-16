using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources
{
	// Token: 0x0200003D RID: 61
	internal class DynamicMode : IEncodeMode
	{
		// Token: 0x06000136 RID: 310 RVA: 0x0000AAE8 File Offset: 0x00008CE8
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

		// Token: 0x06000137 RID: 311 RVA: 0x0000AB94 File Offset: 0x00008D94
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] ret = new uint[key.Length];
			Buffer.BlockCopy(data, offset * 4, ret, 0, key.Length * 4);
			this.encryptFunc(ret, key);
			return ret;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00004A68 File Offset: 0x00002C68
		public DynamicMode()
		{
		}

		// Token: 0x0400006D RID: 109
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x0200003E RID: 62
		private class CodeGen : CILCodeGen
		{
			// Token: 0x06000139 RID: 313 RVA: 0x00004FC7 File Offset: 0x000031C7
			public CodeGen(Local block, Local key, MethodDef init, IList<Instruction> instrs) : base(init, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x0600013A RID: 314 RVA: 0x0000ABD0 File Offset: 0x00008DD0
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

			// Token: 0x0400006E RID: 110
			private readonly Local block;

			// Token: 0x0400006F RID: 111
			private readonly Local key;
		}
	}
}
