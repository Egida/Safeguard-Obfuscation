using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Compress
{
	// Token: 0x020000FB RID: 251
	internal class DynamicDeriver : IKeyDeriver
	{
		// Token: 0x060003D6 RID: 982 RVA: 0x0001F860 File Offset: 0x0001DA60
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

		// Token: 0x060003D7 RID: 983 RVA: 0x0001F8E8 File Offset: 0x0001DAE8
		public uint[] DeriveKey(uint[] a, uint[] b)
		{
			uint[] ret = new uint[16];
			Buffer.BlockCopy(a, 0, ret, 0, a.Length * 4);
			this.encryptFunc(ret, b);
			return ret;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0001F920 File Offset: 0x0001DB20
		public IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src)
		{
			List<Instruction> ret = new List<Instruction>();
			DynamicDeriver.CodeGen codeGen = new DynamicDeriver.CodeGen(dst, src, method, ret);
			codeGen.GenerateCIL(this.derivation);
			codeGen.Commit(method.Body);
			return ret;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00004A68 File Offset: 0x00002C68
		public DynamicDeriver()
		{
		}

		// Token: 0x040002EB RID: 747
		private StatementBlock derivation;

		// Token: 0x040002EC RID: 748
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x020000FC RID: 252
		private class CodeGen : CILCodeGen
		{
			// Token: 0x060003DA RID: 986 RVA: 0x00005DB9 File Offset: 0x00003FB9
			public CodeGen(Local block, Local key, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x060003DB RID: 987 RVA: 0x0001F960 File Offset: 0x0001DB60
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

			// Token: 0x040002ED RID: 749
			private readonly Local block;

			// Token: 0x040002EE RID: 750
			private readonly Local key;
		}
	}
}
