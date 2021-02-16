using System;
using System.Collections.Generic;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x0200005C RID: 92
	internal class NormalEncoding : IRPEncoding
	{
		// Token: 0x06000192 RID: 402 RVA: 0x0000D1C8 File Offset: 0x0000B3C8
		public Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg)
		{
			Tuple<int, int> key = this.GetKey(ctx.Random, init);
			List<Instruction> ret = new List<Instruction>();
			bool flag = ctx.Random.NextBoolean();
			if (flag)
			{
				ret.Add(Instruction.Create(OpCodes.Ldc_I4, key.Item1));
				ret.AddRange(arg);
			}
			else
			{
				ret.AddRange(arg);
				ret.Add(Instruction.Create(OpCodes.Ldc_I4, key.Item1));
			}
			ret.Add(Instruction.Create(OpCodes.Mul));
			return ret.ToArray();
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000D258 File Offset: 0x0000B458
		public int Encode(MethodDef init, RPContext ctx, int value)
		{
			Tuple<int, int> key = this.GetKey(ctx.Random, init);
			return value * key.Item2;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000D280 File Offset: 0x0000B480
		private Tuple<int, int> GetKey(RandomGenerator random, MethodDef init)
		{
			Tuple<int, int> ret;
			bool flag = !this.keys.TryGetValue(init, out ret);
			if (flag)
			{
				int key = random.NextInt32() | 1;
				ret = (this.keys[init] = Tuple.Create<int, int>(key, (int)MathsUtils.modInv((uint)key)));
			}
			return ret;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00005228 File Offset: 0x00003428
		public NormalEncoding()
		{
		}

		// Token: 0x040000C7 RID: 199
		private readonly Dictionary<MethodDef, Tuple<int, int>> keys = new Dictionary<MethodDef, Tuple<int, int>>();
	}
}
