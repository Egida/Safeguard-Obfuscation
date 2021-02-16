using System;
using System.Collections.Generic;
using Confuser.Core.Services;
using dnlib.DotNet;

namespace Confuser.Protections.Compress
{
	// Token: 0x020000FA RID: 250
	internal class CompressorContext
	{
		// Token: 0x060003D4 RID: 980 RVA: 0x0001F6D0 File Offset: 0x0001D8D0
		public byte[] Encrypt(ICompressionService compress, byte[] data, uint seed, Action<double> progressFunc)
		{
			data = (byte[])data.Clone();
			uint[] dst = new uint[16];
			uint[] src = new uint[16];
			ulong state = (ulong)seed;
			for (int i = 0; i < 16; i++)
			{
				state = state * state % 339722377UL;
				src[i] = (uint)state;
				dst[i] = (uint)(state * state % 1145919227UL);
			}
			uint[] key = this.Deriver.DeriveKey(dst, src);
			uint z = (uint)(state % 9067703UL);
			for (int j = 0; j < data.Length; j++)
			{
				byte[] array = data;
				int num = j;
				array[num] ^= (byte)state;
				bool flag = (j & 255) == 0;
				if (flag)
				{
					state = state * state % 9067703UL;
				}
			}
			data = compress.Compress(data, progressFunc);
			Array.Resize<byte>(ref data, data.Length + 3 & -4);
			byte[] encryptedData = new byte[data.Length];
			int keyIndex = 0;
			for (int k = 0; k < data.Length; k += 4)
			{
				uint datum = (uint)((int)data[k] | (int)data[k + 1] << 8 | (int)data[k + 2] << 16 | (int)data[k + 3] << 24);
				uint encrypted = datum ^ key[keyIndex & 15];
				key[keyIndex & 15] = (key[keyIndex & 15] ^ datum) + 1037772825U;
				encryptedData[k] = (byte)encrypted;
				encryptedData[k + 1] = (byte)(encrypted >> 8);
				encryptedData[k + 2] = (byte)(encrypted >> 16);
				encryptedData[k + 3] = (byte)(encrypted >> 24);
				keyIndex++;
			}
			return encryptedData;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00004A68 File Offset: 0x00002C68
		public CompressorContext()
		{
		}

		// Token: 0x040002DD RID: 733
		public AssemblyDef Assembly;

		// Token: 0x040002DE RID: 734
		public IKeyDeriver Deriver;

		// Token: 0x040002DF RID: 735
		public byte[] EncryptedModule;

		// Token: 0x040002E0 RID: 736
		public MethodDef EntryPoint;

		// Token: 0x040002E1 RID: 737
		public uint EntryPointToken;

		// Token: 0x040002E2 RID: 738
		public byte[] KeySig;

		// Token: 0x040002E3 RID: 739
		public uint KeyToken;

		// Token: 0x040002E4 RID: 740
		public ModuleKind Kind;

		// Token: 0x040002E5 RID: 741
		public List<Tuple<uint, uint, string>> ManifestResources;

		// Token: 0x040002E6 RID: 742
		public int ModuleIndex;

		// Token: 0x040002E7 RID: 743
		public string ModuleName;

		// Token: 0x040002E8 RID: 744
		public byte[] OriginModule;

		// Token: 0x040002E9 RID: 745
		public ModuleDef OriginModuleDef;

		// Token: 0x040002EA RID: 746
		public bool CompatMode;
	}
}
