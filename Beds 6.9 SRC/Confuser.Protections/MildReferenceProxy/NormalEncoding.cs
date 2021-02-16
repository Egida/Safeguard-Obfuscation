using System;
using System.Collections.Generic;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.MildReferenceProxy
{
	// Token: 0x02000076 RID: 118
	internal class NormalEncoding : IRPEncoding
	{
		// Token: 0x060001DC RID: 476 RVA: 0x0000F7B4 File Offset: 0x0000D9B4
		public Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg)
		{
			Tuple<int, int> key = this.GetKey(ctx.Random, init);
			List<Instruction> list = new List<Instruction>();
			bool flag = ctx.Random.NextBoolean();
			if (flag)
			{
				list.Add(Instruction.Create(OpCodes.Ldc_I4, key.Item1));
				list.AddRange(arg);
			}
			else
			{
				list.AddRange(arg);
				list.Add(Instruction.Create(OpCodes.Ldc_I4, key.Item1));
			}
			list.Add(Instruction.Create(OpCodes.Mul));
			return list.ToArray();
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000F844 File Offset: 0x0000DA44
		public int Encode(MethodDef init, RPContext ctx, int value)
		{
			Tuple<int, int> key = this.GetKey(ctx.Random, init);
			return value * key.Item2;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000F86C File Offset: 0x0000DA6C
		private Tuple<int, int> GetKey(RandomGenerator random, MethodDef init)
		{
			Tuple<int, int> tuple;
			bool flag = !this.keys.TryGetValue(init, out tuple);
			if (flag)
			{
				int num = random.NextInt32() | 1;
				tuple = (this.keys[init] = Tuple.Create<int, int>(num, (int)MathsUtils.modInv((uint)num)));
			}
			return tuple;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00005353 File Offset: 0x00003553
		public NormalEncoding()
		{
		}

		// Token: 0x04000119 RID: 281
		private readonly Dictionary<MethodDef, Tuple<int, int>> keys = new Dictionary<MethodDef, Tuple<int, int>>();
	}
}
