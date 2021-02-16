using System;

namespace Confuser.DynCipher
{
	// Token: 0x02000005 RID: 5
	public static class MathsUtils
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002108 File Offset: 0x00000308
		public static ulong modInv(ulong num, ulong mod)
		{
			ulong a = mod;
			ulong b = num % mod;
			ulong p0 = 0UL;
			ulong p = 1UL;
			while (b > 0UL)
			{
				bool flag = b == 1UL;
				ulong result;
				if (flag)
				{
					result = p;
				}
				else
				{
					p0 += a / b * p;
					a %= b;
					bool flag2 = a == 0UL;
					if (flag2)
					{
						break;
					}
					bool flag3 = a == 1UL;
					if (!flag3)
					{
						p += b / a * p0;
						b %= a;
						continue;
					}
					result = mod - p0;
				}
				return result;
			}
			return 0UL;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002180 File Offset: 0x00000380
		public static uint modInv(uint num)
		{
			return (uint)MathsUtils.modInv((ulong)num, 4294967296UL);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021A4 File Offset: 0x000003A4
		public static byte modInv(byte num)
		{
			return (byte)MathsUtils.modInv((ulong)num, 256UL);
		}

		// Token: 0x04000002 RID: 2
		private const ulong MODULO32 = 4294967296UL;
	}
}
