﻿using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x0200000E RID: 14
	internal struct BitDecoder
	{
		// Token: 0x0600002D RID: 45 RVA: 0x0000412C File Offset: 0x0000232C
		public void UpdateModel(int numMoveBits, uint symbol)
		{
			bool flag = symbol == 0U;
			if (flag)
			{
				this.Prob += 2048U - this.Prob >> numMoveBits;
			}
			else
			{
				this.Prob -= this.Prob >> numMoveBits;
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000021F1 File Offset: 0x000003F1
		public void Init()
		{
			this.Prob = 1024U;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000417C File Offset: 0x0000237C
		public uint Decode(Decoder rangeDecoder)
		{
			uint newBound = (rangeDecoder.Range >> 11) * this.Prob;
			bool flag = rangeDecoder.Code < newBound;
			uint result;
			if (flag)
			{
				rangeDecoder.Range = newBound;
				this.Prob += 2048U - this.Prob >> 5;
				bool flag2 = rangeDecoder.Range < 16777216U;
				if (flag2)
				{
					rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
					rangeDecoder.Range <<= 8;
				}
				result = 0U;
			}
			else
			{
				rangeDecoder.Range -= newBound;
				rangeDecoder.Code -= newBound;
				this.Prob -= this.Prob >> 5;
				bool flag3 = rangeDecoder.Range < 16777216U;
				if (flag3)
				{
					rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
					rangeDecoder.Range <<= 8;
				}
				result = 1U;
			}
			return result;
		}

		// Token: 0x04000025 RID: 37
		public const int kNumBitModelTotalBits = 11;

		// Token: 0x04000026 RID: 38
		public const uint kBitModelTotal = 2048U;

		// Token: 0x04000027 RID: 39
		private const int kNumMoveBits = 5;

		// Token: 0x04000028 RID: 40
		private uint Prob;
	}
}
