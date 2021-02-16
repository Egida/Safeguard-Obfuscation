using System;

namespace SevenZip
{
	// Token: 0x02000002 RID: 2
	internal class CRC
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000039D8 File Offset: 0x00001BD8
		static CRC()
		{
			for (uint i = 0U; i < 256U; i += 1U)
			{
				uint r = i;
				for (int j = 0; j < 8; j++)
				{
					bool flag = (r & 1U) > 0U;
					if (flag)
					{
						r = (r >> 1 ^ 3988292384U);
					}
					else
					{
						r >>= 1;
					}
				}
				CRC.Table[(int)i] = r;
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020D8 File Offset: 0x000002D8
		public void Init()
		{
			this._value = uint.MaxValue;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020E2 File Offset: 0x000002E2
		public void UpdateByte(byte b)
		{
			this._value = (CRC.Table[(int)((byte)this._value ^ b)] ^ this._value >> 8);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00003A44 File Offset: 0x00001C44
		public void Update(byte[] data, uint offset, uint size)
		{
			for (uint i = 0U; i < size; i += 1U)
			{
				this._value = (CRC.Table[(int)((byte)this._value ^ data[(int)(offset + i)])] ^ this._value >> 8);
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00003A84 File Offset: 0x00001C84
		public uint GetDigest()
		{
			return this._value ^ uint.MaxValue;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00003AA0 File Offset: 0x00001CA0
		private static uint CalculateDigest(byte[] data, uint offset, uint size)
		{
			CRC crc = new CRC();
			crc.Update(data, offset, size);
			return crc.GetDigest();
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00003AC8 File Offset: 0x00001CC8
		private static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
		{
			return CRC.CalculateDigest(data, offset, size) == digest;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002103 File Offset: 0x00000303
		public CRC()
		{
		}

		// Token: 0x04000001 RID: 1
		public static readonly uint[] Table = new uint[256];

		// Token: 0x04000002 RID: 2
		private uint _value = uint.MaxValue;
	}
}
