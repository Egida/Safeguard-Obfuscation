using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000A4 RID: 164
	internal class DynamicDeriver : IKeyDeriver
	{
		// Token: 0x0600027A RID: 634 RVA: 0x0001536C File Offset: 0x0001356C
		public void Init(ConfuserContext ctx, RandomGenerator random)
		{
			StatementBlock dummy;
			ctx.Registry.GetService<IDynCipherService>().GenerateCipherPair(random, out this.derivation, out dummy);
			DMCodeGen dmCodeGen = new DMCodeGen(typeof(void), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{BUFFER}", typeof(uint[])),
				Tuple.Create<string, Type>("{KEY}", typeof(uint[]))
			});
			dmCodeGen.GenerateCIL(this.derivation);
			this.encryptFunc = dmCodeGen.Compile<Action<uint[], uint[]>>();
		}

		// Token: 0x0600027B RID: 635 RVA: 0x000153F4 File Offset: 0x000135F4
		public uint[] DeriveKey(uint[] a, uint[] b)
		{
			uint[] ret = new uint[16];
			Buffer.BlockCopy(a, 0, ret, 0, a.Length * 4);
			this.encryptFunc(ret, b);
			return ret;
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0001542C File Offset: 0x0001362C
		public IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src)
		{
			List<Instruction> ret = new List<Instruction>();
			DynamicDeriver.CodeGen codeGen = new DynamicDeriver.CodeGen(dst, src, method, ret);
			codeGen.GenerateCIL(this.derivation);
			codeGen.Commit(method.Body);
			return ret;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00004A68 File Offset: 0x00002C68
		public DynamicDeriver()
		{
		}

		// Token: 0x040001B8 RID: 440
		private StatementBlock derivation;

		// Token: 0x040001B9 RID: 441
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x020000A5 RID: 165
		private class CodeGen : CILCodeGen
		{
			// Token: 0x0600027E RID: 638 RVA: 0x00005536 File Offset: 0x00003736
			public CodeGen(Local block, Local key, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x0600027F RID: 639 RVA: 0x0001546C File Offset: 0x0001366C
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

			// Token: 0x040001BA RID: 442
			private readonly Local block;

			// Token: 0x040001BB RID: 443
			private readonly Local key;
		}
	}
}
