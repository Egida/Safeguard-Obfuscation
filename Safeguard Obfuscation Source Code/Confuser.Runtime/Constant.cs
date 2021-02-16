using System;
using System.Text;

namespace Confuser.Runtime
{
	// Token: 0x02000011 RID: 17
	internal static class Constant
	{
		// Token: 0x0600005A RID: 90 RVA: 0x000048F8 File Offset: 0x00002AF8
		private static void Initialize()
		{
			uint lol = (uint)Mutation.KeyI0;
			uint[] qqq = Mutation.Placeholder<uint[]>(new uint[Mutation.KeyI0]);
			uint[] kkk = new uint[16];
			uint i = (uint)Mutation.KeyI1;
			for (int j = 0; j < 16; j++)
			{
				i ^= i >> 12;
				i ^= i << 25;
				i ^= i >> 27;
				kkk[j] = i;
			}
			int sos = 0;
			int d = 0;
			uint[] waw = new uint[16];
			byte[] o = new byte[lol * 4U];
			while ((long)sos < (long)((ulong)lol))
			{
				for (int jjj = 0; jjj < 16; jjj++)
				{
					waw[jjj] = qqq[sos + jjj];
				}
				Mutation.Crypt(waw, kkk);
				for (int jjj2 = 0; jjj2 < 16; jjj2++)
				{
					uint e = waw[jjj2];
					o[d++] = (byte)e;
					o[d++] = (byte)(e >> 8);
					o[d++] = (byte)(e >> 16);
					o[d++] = (byte)(e >> 24);
					kkk[jjj2] ^= e;
				}
				sos += 16;
			}
			Constant.bbb = Lzma.Decompress(o);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004A18 File Offset: 0x00002C18
		private static T Get<T>(uint id)
		{
			id = (uint)Mutation.Placeholder<int>((int)id);
			uint t = id >> 30;
			T ret = default(T);
			id &= 1073741823U;
			id <<= 2;
			if ((ulong)t == (ulong)((long)Mutation.KeyI0))
			{
				int lol = (int)Constant.bbb[(int)id++] | (int)Constant.bbb[(int)id++] << 8 | (int)Constant.bbb[(int)id++] << 16 | (int)Constant.bbb[(int)id++] << 24;
				ret = (T)((object)string.Intern(Encoding.UTF8.GetString(Constant.bbb, (int)id, lol)));
			}
			else if ((ulong)t == (ulong)((long)Mutation.KeyI1))
			{
				T[] v = new T[1];
				Buffer.BlockCopy(Constant.bbb, (int)id, v, 0, Mutation.Value<int>());
				ret = v[0];
			}
			else if ((ulong)t == (ulong)((long)Mutation.KeyI2))
			{
				int sos = (int)Constant.bbb[(int)id++] | (int)Constant.bbb[(int)id++] << 8 | (int)Constant.bbb[(int)id++] << 16 | (int)Constant.bbb[(int)id++] << 24;
				int lol2 = (int)Constant.bbb[(int)id++] | (int)Constant.bbb[(int)id++] << 8 | (int)Constant.bbb[(int)id++] << 16 | (int)Constant.bbb[(int)id++] << 24;
				Array v2 = Array.CreateInstance(typeof(T).GetElementType(), lol2);
				Buffer.BlockCopy(Constant.bbb, (int)id, v2, 0, sos - 4);
				ret = (T)((object)v2);
			}
			return ret;
		}

		// Token: 0x04000033 RID: 51
		private static byte[] bbb;
	}
}
