using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x0200000D RID: 13
	internal struct BitEncoder
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00003F48 File Offset: 0x00002148
		static BitEncoder()
		{
			for (int i = 8; i >= 0; i--)
			{
				uint start = 1U << 9 - i - 1;
				uint end = 1U << 9 - i;
				for (uint j = start; j < end; j += 1U)
				{
					BitEncoder.ProbPrices[(int)j] = (uint)((i << 6) + (int)(end - j << 6 >> 9 - i - 1));
				}
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000021E3 File Offset: 0x000003E3
		public void Init()
		{
			this.Prob = 1024U;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003FC0 File Offset: 0x000021C0
		public void UpdateModel(uint symbol)
		{
			bool flag = symbol == 0U;
			if (flag)
			{
				this.Prob += 2048U - this.Prob >> 5;
			}
			else
			{
				this.Prob -= this.Prob >> 5;
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00004008 File Offset: 0x00002208
		public void Encode(Encoder encoder, uint symbol)
		{
			uint newBound = (encoder.Range >> 11) * this.Prob;
			bool flag = symbol == 0U;
			if (flag)
			{
				encoder.Range = newBound;
				this.Prob += 2048U - this.Prob >> 5;
			}
			else
			{
				encoder.Low += (ulong)newBound;
				encoder.Range -= newBound;
				this.Prob -= this.Prob >> 5;
			}
			bool flag2 = encoder.Range < 16777216U;
			if (flag2)
			{
				encoder.Range <<= 8;
				encoder.ShiftLow();
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000040B4 File Offset: 0x000022B4
		public uint GetPrice(uint symbol)
		{
			return BitEncoder.ProbPrices[(int)(checked((IntPtr)((unchecked((ulong)(this.Prob - symbol) ^ (ulong)((long)(-(long)symbol))) & 2047UL) >> 2)))];
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000040E4 File Offset: 0x000022E4
		public uint GetPrice0()
		{
			return BitEncoder.ProbPrices[(int)(this.Prob >> 2)];
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00004104 File Offset: 0x00002304
		public uint GetPrice1()
		{
			return BitEncoder.ProbPrices[(int)(2048U - this.Prob >> 2)];
		}

		// Token: 0x0400001E RID: 30
		public const int kNumBitModelTotalBits = 11;

		// Token: 0x0400001F RID: 31
		public const uint kBitModelTotal = 2048U;

		// Token: 0x04000020 RID: 32
		private const int kNumMoveBits = 5;

		// Token: 0x04000021 RID: 33
		private const int kNumMoveReducingBits = 2;

		// Token: 0x04000022 RID: 34
		public const int kNumBitPriceShiftBits = 6;

		// Token: 0x04000023 RID: 35
		private static readonly uint[] ProbPrices = new uint[512];

		// Token: 0x04000024 RID: 36
		private uint Prob;
	}
}
