using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Confuser.Runtime
{
	// Token: 0x02000010 RID: 16
	internal static class Compressor
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00004358 File Offset: 0x00002558
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

		// Token: 0x06000058 RID: 88 RVA: 0x000046C4 File Offset: 0x000028C4
		[STAThread]
		private static int Main(string[] args)
		{
			int keyI = Mutation.KeyI0;
			uint[] array = Mutation.Placeholder<uint[]>(new uint[Mutation.KeyI0]);
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Module manifestModule = executingAssembly.ManifestModule;
			GCHandle gchandle = Compressor.Decrypt(array, (uint)Mutation.KeyI1);
			byte[] array2 = (byte[])gchandle.Target;
			Module module = executingAssembly.LoadModule("SafeGuard", array2);
			Array.Clear(array2, 0, array2.Length);
			gchandle.Free();
			Array.Clear(array, 0, array.Length);
			Compressor.key = manifestModule.ResolveSignature(Mutation.KeyI2);
			AppDomain.CurrentDomain.AssemblyResolve += Compressor.Resolve;
			module.GetTypes();
			MethodBase methodBase = module.ResolveMethod((int)Compressor.key[0] | (int)Compressor.key[1] << 8 | (int)Compressor.key[2] << 16 | (int)Compressor.key[3] << 24);
			object[] array3 = new object[methodBase.GetParameters().Length];
			if (array3.Length != 0)
			{
				array3[0] = args;
			}
			object obj = methodBase.Invoke(null, array3);
			if (obj is int)
			{
				return (int)obj;
			}
			return 0;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000047C0 File Offset: 0x000029C0
		private static Assembly Resolve(object sender, ResolveEventArgs e)
		{
			byte[] b = Encoding.UTF8.GetBytes(new AssemblyName(e.Name).FullName.ToUpperInvariant());
			Stream i = null;
			if (b.Length + 4 <= Compressor.key.Length)
			{
				for (int j = 0; j < b.Length; j++)
				{
					byte[] array = b;
					int num = j;
					array[num] *= Compressor.key[j + 4];
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
				GCHandle h = Compressor.Decrypt(d, s);
				byte[] f = (byte[])h.Target;
				Assembly result = Assembly.Load(f);
				Array.Clear(f, 0, f.Length);
				h.Free();
				Array.Clear(d, 0, d.Length);
				return result;
			}
			return null;
		}

		// Token: 0x04000032 RID: 50
		private static byte[] key;
	}
}
