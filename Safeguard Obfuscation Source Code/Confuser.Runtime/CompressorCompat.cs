using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Confuser.Runtime
{
	// Token: 0x0200000F RID: 15
	internal static class CompressorCompat
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00004358 File Offset: 0x00002558
		private static GCHandle Decrypt(uint[] data, uint seed)
		{
			uint[] w = new uint[16];
			uint[] i = new uint[16];
			ulong s = (ulong)seed;
			for (int j = 0; j < 16; j++)
			{
				s = s * s % 339722377UL;
				i[j] = (uint)s;
				w[j] = (uint)(s * s % 1145919227UL);
			}
			Mutation.Crypt(w, i);
			Array.Clear(i, 0, 16);
			byte[] b = new byte[data.Length << 2];
			uint h = 0U;
			for (int k = 0; k < data.Length; k++)
			{
				uint d = data[k] ^ w[k & 15];
				w[k & 15] = (w[k & 15] ^ d) + 1037772825U;
				b[(int)h] = (byte)d;
				b[(int)(h + 1U)] = (byte)(d >> 8);
				b[(int)(h + 2U)] = (byte)(d >> 16);
				b[(int)(h + 3U)] = (byte)(d >> 24);
				h += 4U;
			}
			Array.Clear(w, 0, 16);
			byte[] l = Lzma.Decompress(b);
			Array.Clear(b, 0, b.Length);
			GCHandle g = GCHandle.Alloc(l, GCHandleType.Pinned);
			ulong num = s % 9067703UL;
			for (int m = 0; m < l.Length; m++)
			{
				byte[] array = l;
				int num2 = m;
				array[num2] ^= (byte)s;
				if ((m & 255) == 0)
				{
					s = s * s % 9067703UL;
				}
			}
			return g;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004498 File Offset: 0x00002698
		[STAThread]
		private static int Main(string[] args)
		{
			int keyI = Mutation.KeyI0;
			uint[] q = Mutation.Placeholder<uint[]>(new uint[Mutation.KeyI0]);
			GCHandle h = CompressorCompat.Decrypt(q, (uint)Mutation.KeyI1);
			byte[] b = (byte[])h.Target;
			Assembly assembly = Assembly.Load(b);
			Array.Clear(b, 0, b.Length);
			h.Free();
			Array.Clear(q, 0, q.Length);
			CompressorCompat.key = typeof(CompressorCompat).Module.ResolveSignature(Mutation.KeyI2);
			AppDomain.CurrentDomain.AssemblyResolve += CompressorCompat.Resolve;
			MethodBase methodBase = assembly.ManifestModule.ResolveMethod((int)CompressorCompat.key[0] | (int)CompressorCompat.key[1] << 8 | (int)CompressorCompat.key[2] << 16 | (int)CompressorCompat.key[3] << 24);
			object[] g = new object[methodBase.GetParameters().Length];
			if (g.Length != 0)
			{
				g[0] = args;
			}
			object r = methodBase.Invoke(null, g);
			if (r is int)
			{
				return (int)r;
			}
			return 0;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000458C File Offset: 0x0000278C
		private static Assembly Resolve(object sender, ResolveEventArgs e)
		{
			byte[] b = Encoding.UTF8.GetBytes(new AssemblyName(e.Name).FullName.ToUpperInvariant());
			Stream i = null;
			if (b.Length + 4 <= CompressorCompat.key.Length)
			{
				for (int j = 0; j < b.Length; j++)
				{
					byte[] array = b;
					int num = j;
					array[num] *= CompressorCompat.key[j + 4];
				}
				string k = Convert.ToBase64String(b);
				i = Assembly.GetEntryAssembly().GetManifestResourceStream(k);
			}
			if (i != null)
			{
				uint[] d = new uint[i.Length >> 2];
				byte[] t = new byte[256];
				int o = 0;
				int r;
				while ((r = i.Read(t, 0, 256)) > 0)
				{
					Buffer.BlockCopy(t, 0, d, o, r);
					o += r;
				}
				uint s = 7339873U;
				foreach (byte c in b)
				{
					s = s * 6176543U + (uint)c;
				}
				GCHandle h = CompressorCompat.Decrypt(d, s);
				byte[] f = (byte[])h.Target;
				Assembly result = Assembly.Load(f);
				Array.Clear(f, 0, f.Length);
				h.Free();
				Array.Clear(d, 0, d.Length);
				return result;
			}
			return null;
		}

		// Token: 0x04000031 RID: 49
		private static byte[] key;
	}
}
