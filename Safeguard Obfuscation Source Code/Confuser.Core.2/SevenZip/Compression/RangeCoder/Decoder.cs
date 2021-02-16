using System;
using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x0200000C RID: 12
	internal class Decoder
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00003D44 File Offset: 0x00001F44
		public void Init(Stream stream)
		{
			this.Stream = stream;
			this.Code = 0U;
			this.Range = uint.MaxValue;
			for (int i = 0; i < 5; i++)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000219D File Offset: 0x0000039D
		public void ReleaseStream()
		{
			this.Stream = null;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000021A7 File Offset: 0x000003A7
		public void CloseStream()
		{
			this.Stream.Close();
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003D94 File Offset: 0x00001F94
		public void Normalize()
		{
			while (this.Range < 16777216U)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003DE0 File Offset: 0x00001FE0
		public void Normalize2()
		{
			bool flag = this.Range < 16777216U;
			if (flag)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003E2C File Offset: 0x0000202C
		public uint GetThreshold(uint total)
		{
			return this.Code / (this.Range /= total);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000021B6 File Offset: 0x000003B6
		public void Decode(uint start, uint size, uint total)
		{
			this.Code -= start * this.Range;
			this.Range *= size;
			this.Normalize();
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003E58 File Offset: 0x00002058
		public uint DecodeDirectBits(int numTotalBits)
		{
			uint range = this.Range;
			uint code = this.Code;
			uint result = 0U;
			for (int i = numTotalBits; i > 0; i--)
			{
				range >>= 1;
				uint t = code - range >> 31;
				code -= (range & t - 1U);
				result = (result << 1 | 1U - t);
				bool flag = range < 16777216U;
				if (flag)
				{
					code = (code << 8 | (uint)((byte)this.Stream.ReadByte()));
					range <<= 8;
				}
			}
			this.Range = range;
			this.Code = code;
			return result;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003EE4 File Offset: 0x000020E4
		public uint DecodeBit(uint size0, int numTotalBits)
		{
			uint newBound = (this.Range >> numTotalBits) * size0;
			bool flag = this.Code < newBound;
			uint symbol;
			if (flag)
			{
				symbol = 0U;
				this.Range = newBound;
			}
			else
			{
				symbol = 1U;
				this.Code -= newBound;
				this.Range -= newBound;
			}
			this.Normalize();
			return symbol;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002194 File Offset: 0x00000394
		public Decoder()
		{
		}

		// Token: 0x0400001A RID: 26
		public const uint kTopValue = 16777216U;

		// Token: 0x0400001B RID: 27
		public uint Code;

		// Token: 0x0400001C RID: 28
		public uint Range;

		// Token: 0x0400001D RID: 29
		public Stream Stream;
	}
}
