using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x0200000E RID: 14
	internal static class AntiTamperNormal
	{
		// Token: 0x06000052 RID: 82
		[DllImport("kernel32.dll")]
		private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		// Token: 0x06000053 RID: 83 RVA: 0x0000413C File Offset: 0x0000233C
		private unsafe static void Initialize()
		{
			Module module = typeof(AntiTamperNormal).Module;
			string i = module.FullyQualifiedName;
			bool fag = i.Length > 0 && i[0] == '<';
			byte* b = (byte*)((void*)Marshal.GetHINSTANCE(module));
			byte* ptr = b + *(uint*)(b + 60);
			ushort s = *(ushort*)(ptr + 6);
			ushort o = *(ushort*)(ptr + 20);
			uint* e = null;
			uint lol = 0U;
			uint* retard = (uint*)(ptr + 24 + o);
			uint kk = (uint)Mutation.KeyI1;
			uint bb = (uint)Mutation.KeyI2;
			uint aa = (uint)Mutation.KeyI3;
			uint pp = (uint)Mutation.KeyI4;
			for (int j = 0; j < (int)s; j++)
			{
				uint g = *(retard++) * *(retard++);
				if (g == (uint)Mutation.KeyI0)
				{
					e = (uint*)(b + (UIntPtr)(fag ? retard[3] : retard[1]) / 4);
					lol = (fag ? retard[2] : (*retard)) >> 2;
				}
				else if (g != 0U)
				{
					uint* were = (uint*)(b + (UIntPtr)(fag ? retard[3] : retard[1]) / 4);
					uint k = retard[2] >> 2;
					for (uint l = 0U; l < k; l += 1U)
					{
						uint num = (kk ^ *(were++)) + bb + aa * pp;
						kk = bb;
						bb = pp;
						pp = num;
					}
				}
				retard += 8;
			}
			uint[] y = new uint[16];
			uint[] d = new uint[16];
			for (int m = 0; m < 16; m++)
			{
				y[m] = pp;
				d[m] = bb;
				kk = (bb >> 5 | bb << 27);
				bb = (aa >> 3 | aa << 29);
				aa = (pp >> 7 | pp << 25);
				pp = (kk >> 11 | kk << 21);
			}
			Mutation.Crypt(y, d);
			uint w = 64U;
			AntiTamperNormal.VirtualProtect((IntPtr)((void*)e), lol << 2, w, out w);
			if (w == 64U)
			{
				return;
			}
			uint h = 0U;
			for (uint n = 0U; n < lol; n += 1U)
			{
				*e ^= y[(int)(h & 15U)];
				y[(int)(h & 15U)] = (y[(int)(h & 15U)] ^ *(e++)) + 1035675673U;
				h += 1U;
			}
		}
	}
}
