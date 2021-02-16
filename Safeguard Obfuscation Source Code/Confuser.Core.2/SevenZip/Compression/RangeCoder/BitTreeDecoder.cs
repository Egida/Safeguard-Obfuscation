using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x02000010 RID: 16
	internal struct BitTreeDecoder
	{
		// Token: 0x06000038 RID: 56 RVA: 0x0000221A File Offset: 0x0000041A
		public BitTreeDecoder(int numBitLevels)
		{
			this.NumBitLevels = numBitLevels;
			this.Models = new BitDecoder[1 << numBitLevels];
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00004498 File Offset: 0x00002698
		public void Init()
		{
			uint i = 1U;
			while ((ulong)i < (ulong)(1L << (this.NumBitLevels & 31)))
			{
				this.Models[(int)i].Init();
				i += 1U;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000044D4 File Offset: 0x000026D4
		public uint Decode(Decoder rangeDecoder)
		{
			uint i = 1U;
			for (int bitIndex = this.NumBitLevels; bitIndex > 0; bitIndex--)
			{
				i = (i << 1) + this.Models[(int)i].Decode(rangeDecoder);
			}
			return i - (1U << this.NumBitLevels);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004524 File Offset: 0x00002724
		public uint ReverseDecode(Decoder rangeDecoder)
		{
			uint i = 1U;
			uint symbol = 0U;
			for (int bitIndex = 0; bitIndex < this.NumBitLevels; bitIndex++)
			{
				uint bit = this.Models[(int)i].Decode(rangeDecoder);
				i <<= 1;
				i += bit;
				symbol |= bit << bitIndex;
			}
			return symbol;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000457C File Offset: 0x0000277C
		public static uint ReverseDecode(BitDecoder[] Models, uint startIndex, Decoder rangeDecoder, int NumBitLevels)
		{
			uint i = 1U;
			uint symbol = 0U;
			for (int bitIndex = 0; bitIndex < NumBitLevels; bitIndex++)
			{
				uint bit = Models[(int)(startIndex + i)].Decode(rangeDecoder);
				i <<= 1;
				i += bit;
				symbol |= bit << bitIndex;
			}
			return symbol;
		}

		// Token: 0x0400002B RID: 43
		private readonly BitDecoder[] Models;

		// Token: 0x0400002C RID: 44
		private readonly int NumBitLevels;
	}
}
