using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x0200000F RID: 15
	internal struct BitTreeEncoder
	{
		// Token: 0x06000030 RID: 48 RVA: 0x000021FF File Offset: 0x000003FF
		public BitTreeEncoder(int numBitLevels)
		{
			this.NumBitLevels = numBitLevels;
			this.Models = new BitEncoder[1 << numBitLevels];
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00004280 File Offset: 0x00002480
		public void Init()
		{
			uint i = 1U;
			while ((ulong)i < (ulong)(1L << (this.NumBitLevels & 31)))
			{
				this.Models[(int)i].Init();
				i += 1U;
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000042BC File Offset: 0x000024BC
		public void Encode(Encoder rangeEncoder, uint symbol)
		{
			uint i = 1U;
			int bitIndex = this.NumBitLevels;
			while (bitIndex > 0)
			{
				bitIndex--;
				uint bit = symbol >> bitIndex & 1U;
				this.Models[(int)i].Encode(rangeEncoder, bit);
				i = (i << 1 | bit);
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004308 File Offset: 0x00002508
		public void ReverseEncode(Encoder rangeEncoder, uint symbol)
		{
			uint i = 1U;
			uint j = 0U;
			while ((ulong)j < (ulong)((long)this.NumBitLevels))
			{
				uint bit = symbol & 1U;
				this.Models[(int)i].Encode(rangeEncoder, bit);
				i = (i << 1 | bit);
				symbol >>= 1;
				j += 1U;
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004354 File Offset: 0x00002554
		public uint GetPrice(uint symbol)
		{
			uint price = 0U;
			uint i = 1U;
			int bitIndex = this.NumBitLevels;
			while (bitIndex > 0)
			{
				bitIndex--;
				uint bit = symbol >> bitIndex & 1U;
				price += this.Models[(int)i].GetPrice(bit);
				i = (i << 1) + bit;
			}
			return price;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000043AC File Offset: 0x000025AC
		public uint ReverseGetPrice(uint symbol)
		{
			uint price = 0U;
			uint i = 1U;
			for (int j = this.NumBitLevels; j > 0; j--)
			{
				uint bit = symbol & 1U;
				symbol >>= 1;
				price += this.Models[(int)i].GetPrice(bit);
				i = (i << 1 | bit);
			}
			return price;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004404 File Offset: 0x00002604
		public static uint ReverseGetPrice(BitEncoder[] Models, uint startIndex, int NumBitLevels, uint symbol)
		{
			uint price = 0U;
			uint i = 1U;
			for (int j = NumBitLevels; j > 0; j--)
			{
				uint bit = symbol & 1U;
				symbol >>= 1;
				price += Models[(int)(startIndex + i)].GetPrice(bit);
				i = (i << 1 | bit);
			}
			return price;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004454 File Offset: 0x00002654
		public static void ReverseEncode(BitEncoder[] Models, uint startIndex, Encoder rangeEncoder, int NumBitLevels, uint symbol)
		{
			uint i = 1U;
			for (int j = 0; j < NumBitLevels; j++)
			{
				uint bit = symbol & 1U;
				Models[(int)(startIndex + i)].Encode(rangeEncoder, bit);
				i = (i << 1 | bit);
				symbol >>= 1;
			}
		}

		// Token: 0x04000029 RID: 41
		private readonly BitEncoder[] Models;

		// Token: 0x0400002A RID: 42
		private readonly int NumBitLevels;
	}
}
