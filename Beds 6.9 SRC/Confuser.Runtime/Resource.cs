using System;
using System.Reflection;

namespace Confuser.Runtime
{
	// Token: 0x02000034 RID: 52
	internal static class Resource
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x000026D9 File Offset: 0x000008D9
		private static Assembly Handler(object sender, ResolveEventArgs args)
		{
			if (Resource.c.FullName == args.Name)
			{
				return Resource.c;
			}
			return null;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00006B58 File Offset: 0x00004D58
		private static void Initialize()
		{
			uint i = (uint)Mutation.KeyI0;
			uint[] q = Mutation.Placeholder<uint[]>(new uint[Mutation.KeyI0]);
			uint[] j = new uint[16];
			uint k = (uint)Mutation.KeyI1;
			for (int l = 0; l < 16; l++)
			{
				k ^= k >> 13;
				k ^= k << 25;
				k ^= k >> 27;
				j[l] = k;
			}
			int s = 0;
			int d = 0;
			uint[] w = new uint[16];
			byte[] o = new byte[i * 4U];
			while ((long)s < (long)((ulong)i))
			{
				for (int m = 0; m < 16; m++)
				{
					w[m] = q[s + m];
				}
				Mutation.Crypt(w, j);
				for (int n = 0; n < 16; n++)
				{
					uint e = w[n];
					o[d++] = (byte)e;
					o[d++] = (byte)(e >> 8);
					o[d++] = (byte)(e >> 16);
					o[d++] = (byte)(e >> 24);
					j[n] ^= e;
				}
				s += 16;
			}
			Resource.c = Assembly.Load(Lzma.Decompress(o));
			AppDomain.CurrentDomain.AssemblyResolve += Resource.Handler;
		}

		// Token: 0x040000B9 RID: 185
		private static Assembly c;
	}
}
