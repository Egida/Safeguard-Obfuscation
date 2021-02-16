using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x02000013 RID: 19
	internal class BinTree : InWindow, IInWindowStream, IMatchFinder
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00002235 File Offset: 0x00000435
		public new void SetStream(Stream stream)
		{
			base.SetStream(stream);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002240 File Offset: 0x00000440
		public new void ReleaseStream()
		{
			base.ReleaseStream();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000045CC File Offset: 0x000027CC
		public new void Init()
		{
			base.Init();
			for (uint i = 0U; i < this._hashSizeSum; i += 1U)
			{
				this._hash[(int)i] = 0U;
			}
			this._cyclicBufferPos = 0U;
			base.ReduceOffsets(-1);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004610 File Offset: 0x00002810
		public new byte GetIndexByte(int index)
		{
			return base.GetIndexByte(index);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000462C File Offset: 0x0000282C
		public new uint GetMatchLen(int index, uint distance, uint limit)
		{
			return base.GetMatchLen(index, distance, limit);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004648 File Offset: 0x00002848
		public new uint GetNumAvailableBytes()
		{
			return base.GetNumAvailableBytes();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004660 File Offset: 0x00002860
		public void Create(uint historySize, uint keepAddBufferBefore, uint matchMaxLen, uint keepAddBufferAfter)
		{
			bool flag = historySize > 2147483391U;
			if (flag)
			{
				throw new Exception();
			}
			this._cutValue = 16U + (matchMaxLen >> 1);
			uint windowReservSize = (historySize + keepAddBufferBefore + matchMaxLen + keepAddBufferAfter) / 2U + 256U;
			base.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, windowReservSize);
			this._matchMaxLen = matchMaxLen;
			uint cyclicBufferSize = historySize + 1U;
			bool flag2 = this._cyclicBufferSize != cyclicBufferSize;
			if (flag2)
			{
				this._son = new uint[(this._cyclicBufferSize = cyclicBufferSize) * 2U];
			}
			uint hs = 65536U;
			bool hash_ARRAY = this.HASH_ARRAY;
			if (hash_ARRAY)
			{
				hs = historySize - 1U;
				hs |= hs >> 1;
				hs |= hs >> 2;
				hs |= hs >> 4;
				hs |= hs >> 8;
				hs >>= 1;
				hs |= 65535U;
				bool flag3 = hs > 16777216U;
				if (flag3)
				{
					hs >>= 1;
				}
				this._hashMask = hs;
				hs += 1U;
				hs += this.kFixHashSize;
			}
			bool flag4 = hs != this._hashSizeSum;
			if (flag4)
			{
				this._hash = new uint[this._hashSizeSum = hs];
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004770 File Offset: 0x00002970
		public uint GetMatches(uint[] distances)
		{
			bool flag = this._pos + this._matchMaxLen <= this._streamPos;
			uint lenLimit;
			if (flag)
			{
				lenLimit = this._matchMaxLen;
			}
			else
			{
				lenLimit = this._streamPos - this._pos;
				bool flag2 = lenLimit < this.kMinMatchCheck;
				if (flag2)
				{
					this.MovePos();
					return 0U;
				}
			}
			uint offset = 0U;
			uint matchMinPos = (this._pos > this._cyclicBufferSize) ? (this._pos - this._cyclicBufferSize) : 0U;
			uint cur = this._bufferOffset + this._pos;
			uint maxLen = 1U;
			uint hash2Value = 0U;
			uint hash3Value = 0U;
			bool hash_ARRAY = this.HASH_ARRAY;
			uint hashValue;
			if (hash_ARRAY)
			{
				uint temp = CRC.Table[(int)this._bufferBase[(int)cur]] ^ (uint)this._bufferBase[(int)(cur + 1U)];
				hash2Value = (temp & 1023U);
				temp ^= (uint)((uint)this._bufferBase[(int)(cur + 2U)] << 8);
				hash3Value = (temp & 65535U);
				hashValue = ((temp ^ CRC.Table[(int)this._bufferBase[(int)(cur + 3U)]] << 5) & this._hashMask);
			}
			else
			{
				hashValue = (uint)((int)this._bufferBase[(int)cur] ^ (int)this._bufferBase[(int)(cur + 1U)] << 8);
			}
			uint curMatch = this._hash[(int)(this.kFixHashSize + hashValue)];
			bool hash_ARRAY2 = this.HASH_ARRAY;
			if (hash_ARRAY2)
			{
				uint curMatch2 = this._hash[(int)hash2Value];
				uint curMatch3 = this._hash[(int)(1024U + hash3Value)];
				this._hash[(int)hash2Value] = this._pos;
				this._hash[(int)(1024U + hash3Value)] = this._pos;
				bool flag3 = curMatch2 > matchMinPos;
				if (flag3)
				{
					bool flag4 = this._bufferBase[(int)(this._bufferOffset + curMatch2)] == this._bufferBase[(int)cur];
					if (flag4)
					{
						maxLen = (distances[(int)offset++] = 2U);
						distances[(int)offset++] = this._pos - curMatch2 - 1U;
					}
				}
				bool flag5 = curMatch3 > matchMinPos;
				if (flag5)
				{
					bool flag6 = this._bufferBase[(int)(this._bufferOffset + curMatch3)] == this._bufferBase[(int)cur];
					if (flag6)
					{
						bool flag7 = curMatch3 == curMatch2;
						if (flag7)
						{
							offset -= 2U;
						}
						maxLen = (distances[(int)offset++] = 3U);
						distances[(int)offset++] = this._pos - curMatch3 - 1U;
						curMatch2 = curMatch3;
					}
				}
				bool flag8 = offset != 0U && curMatch2 == curMatch;
				if (flag8)
				{
					offset -= 2U;
					maxLen = 1U;
				}
			}
			this._hash[(int)(this.kFixHashSize + hashValue)] = this._pos;
			uint ptr0 = (this._cyclicBufferPos << 1) + 1U;
			uint ptr = this._cyclicBufferPos << 1;
			uint len2;
			uint len = len2 = this.kNumHashDirectBytes;
			bool flag9 = this.kNumHashDirectBytes > 0U;
			if (flag9)
			{
				bool flag10 = curMatch > matchMinPos;
				if (flag10)
				{
					bool flag11 = this._bufferBase[(int)(this._bufferOffset + curMatch + this.kNumHashDirectBytes)] != this._bufferBase[(int)(cur + this.kNumHashDirectBytes)];
					if (flag11)
					{
						maxLen = (distances[(int)offset++] = this.kNumHashDirectBytes);
						distances[(int)offset++] = this._pos - curMatch - 1U;
					}
				}
			}
			uint count = this._cutValue;
			uint cyclicPos;
			for (;;)
			{
				bool flag12 = curMatch <= matchMinPos || count-- == 0U;
				if (flag12)
				{
					break;
				}
				uint delta = this._pos - curMatch;
				cyclicPos = ((delta <= this._cyclicBufferPos) ? (this._cyclicBufferPos - delta) : (this._cyclicBufferPos - delta + this._cyclicBufferSize)) << 1;
				uint pby = this._bufferOffset + curMatch;
				uint len3 = Math.Min(len2, len);
				bool flag13 = this._bufferBase[(int)(pby + len3)] == this._bufferBase[(int)(cur + len3)];
				if (flag13)
				{
					while ((len3 += 1U) != lenLimit)
					{
						bool flag14 = this._bufferBase[(int)(pby + len3)] != this._bufferBase[(int)(cur + len3)];
						if (flag14)
						{
							break;
						}
					}
					bool flag15 = maxLen < len3;
					if (flag15)
					{
						maxLen = (distances[(int)offset++] = len3);
						distances[(int)offset++] = delta - 1U;
						bool flag16 = len3 == lenLimit;
						if (flag16)
						{
							goto Block_22;
						}
					}
				}
				bool flag17 = this._bufferBase[(int)(pby + len3)] < this._bufferBase[(int)(cur + len3)];
				if (flag17)
				{
					this._son[(int)ptr] = curMatch;
					ptr = cyclicPos + 1U;
					curMatch = this._son[(int)ptr];
					len = len3;
				}
				else
				{
					this._son[(int)ptr0] = curMatch;
					ptr0 = cyclicPos;
					curMatch = this._son[(int)ptr0];
					len2 = len3;
				}
			}
			this._son[(int)ptr0] = (this._son[(int)ptr] = 0U);
			goto IL_494;
			Block_22:
			this._son[(int)ptr] = this._son[(int)cyclicPos];
			this._son[(int)ptr0] = this._son[(int)(cyclicPos + 1U)];
			IL_494:
			this.MovePos();
			return offset;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004C20 File Offset: 0x00002E20
		public void Skip(uint num)
		{
			for (;;)
			{
				bool flag = this._pos + this._matchMaxLen <= this._streamPos;
				uint lenLimit;
				if (flag)
				{
					lenLimit = this._matchMaxLen;
					goto IL_55;
				}
				lenLimit = this._streamPos - this._pos;
				bool flag2 = lenLimit < this.kMinMatchCheck;
				if (!flag2)
				{
					goto IL_55;
				}
				this.MovePos();
				IL_303:
				if ((num -= 1U) <= 0U)
				{
					break;
				}
				continue;
				IL_55:
				uint matchMinPos = (this._pos > this._cyclicBufferSize) ? (this._pos - this._cyclicBufferSize) : 0U;
				uint cur = this._bufferOffset + this._pos;
				bool hash_ARRAY = this.HASH_ARRAY;
				uint hashValue;
				if (hash_ARRAY)
				{
					uint temp = CRC.Table[(int)this._bufferBase[(int)cur]] ^ (uint)this._bufferBase[(int)(cur + 1U)];
					uint hash2Value = temp & 1023U;
					this._hash[(int)hash2Value] = this._pos;
					temp ^= (uint)((uint)this._bufferBase[(int)(cur + 2U)] << 8);
					uint hash3Value = temp & 65535U;
					this._hash[(int)(1024U + hash3Value)] = this._pos;
					hashValue = ((temp ^ CRC.Table[(int)this._bufferBase[(int)(cur + 3U)]] << 5) & this._hashMask);
				}
				else
				{
					hashValue = (uint)((int)this._bufferBase[(int)cur] ^ (int)this._bufferBase[(int)(cur + 1U)] << 8);
				}
				uint curMatch = this._hash[(int)(this.kFixHashSize + hashValue)];
				this._hash[(int)(this.kFixHashSize + hashValue)] = this._pos;
				uint ptr0 = (this._cyclicBufferPos << 1) + 1U;
				uint ptr = this._cyclicBufferPos << 1;
				uint len2;
				uint len = len2 = this.kNumHashDirectBytes;
				uint count = this._cutValue;
				uint cyclicPos;
				for (;;)
				{
					bool flag3 = curMatch <= matchMinPos || count-- == 0U;
					if (flag3)
					{
						goto Block_5;
					}
					uint delta = this._pos - curMatch;
					cyclicPos = ((delta <= this._cyclicBufferPos) ? (this._cyclicBufferPos - delta) : (this._cyclicBufferPos - delta + this._cyclicBufferSize)) << 1;
					uint pby = this._bufferOffset + curMatch;
					uint len3 = Math.Min(len2, len);
					bool flag4 = this._bufferBase[(int)(pby + len3)] == this._bufferBase[(int)(cur + len3)];
					if (flag4)
					{
						while ((len3 += 1U) != lenLimit)
						{
							bool flag5 = this._bufferBase[(int)(pby + len3)] != this._bufferBase[(int)(cur + len3)];
							if (flag5)
							{
								break;
							}
						}
						bool flag6 = len3 == lenLimit;
						if (flag6)
						{
							goto Block_9;
						}
					}
					bool flag7 = this._bufferBase[(int)(pby + len3)] < this._bufferBase[(int)(cur + len3)];
					if (flag7)
					{
						this._son[(int)ptr] = curMatch;
						ptr = cyclicPos + 1U;
						curMatch = this._son[(int)ptr];
						len = len3;
					}
					else
					{
						this._son[(int)ptr0] = curMatch;
						ptr0 = cyclicPos;
						curMatch = this._son[(int)ptr0];
						len2 = len3;
					}
				}
				IL_2FB:
				this.MovePos();
				goto IL_303;
				Block_9:
				this._son[(int)ptr] = this._son[(int)cyclicPos];
				this._son[(int)ptr0] = this._son[(int)(cyclicPos + 1U)];
				goto IL_2FB;
				Block_5:
				this._son[(int)ptr0] = (this._son[(int)ptr] = 0U);
				goto IL_2FB;
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004F44 File Offset: 0x00003144
		public void SetType(int numHashBytes)
		{
			this.HASH_ARRAY = (numHashBytes > 2);
			bool hash_ARRAY = this.HASH_ARRAY;
			if (hash_ARRAY)
			{
				this.kNumHashDirectBytes = 0U;
				this.kMinMatchCheck = 4U;
				this.kFixHashSize = 66560U;
			}
			else
			{
				this.kNumHashDirectBytes = 2U;
				this.kMinMatchCheck = 3U;
				this.kFixHashSize = 0U;
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004F9C File Offset: 0x0000319C
		public new void MovePos()
		{
			uint num = this._cyclicBufferPos + 1U;
			this._cyclicBufferPos = num;
			bool flag = num >= this._cyclicBufferSize;
			if (flag)
			{
				this._cyclicBufferPos = 0U;
			}
			base.MovePos();
			bool flag2 = this._pos == 2147483647U;
			if (flag2)
			{
				this.Normalize();
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004FF0 File Offset: 0x000031F0
		private void NormalizeLinks(uint[] items, uint numItems, uint subValue)
		{
			for (uint i = 0U; i < numItems; i += 1U)
			{
				uint value = items[(int)i];
				bool flag = value <= subValue;
				if (flag)
				{
					value = 0U;
				}
				else
				{
					value -= subValue;
				}
				items[(int)i] = value;
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000502C File Offset: 0x0000322C
		private void Normalize()
		{
			uint subValue = this._pos - this._cyclicBufferSize;
			this.NormalizeLinks(this._son, this._cyclicBufferSize * 2U, subValue);
			this.NormalizeLinks(this._hash, this._hashSizeSum, subValue);
			base.ReduceOffsets((int)subValue);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000224A File Offset: 0x0000044A
		public void SetCutValue(uint cutValue)
		{
			this._cutValue = cutValue;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002254 File Offset: 0x00000454
		public BinTree()
		{
		}

		// Token: 0x0400002D RID: 45
		private const uint kHash2Size = 1024U;

		// Token: 0x0400002E RID: 46
		private const uint kHash3Size = 65536U;

		// Token: 0x0400002F RID: 47
		private const uint kBT2HashSize = 65536U;

		// Token: 0x04000030 RID: 48
		private const uint kStartMaxLen = 1U;

		// Token: 0x04000031 RID: 49
		private const uint kHash3Offset = 1024U;

		// Token: 0x04000032 RID: 50
		private const uint kEmptyHashValue = 0U;

		// Token: 0x04000033 RID: 51
		private const uint kMaxValForNormalize = 2147483647U;

		// Token: 0x04000034 RID: 52
		private bool HASH_ARRAY = true;

		// Token: 0x04000035 RID: 53
		private uint _cutValue = 255U;

		// Token: 0x04000036 RID: 54
		private uint _cyclicBufferPos;

		// Token: 0x04000037 RID: 55
		private uint _cyclicBufferSize;

		// Token: 0x04000038 RID: 56
		private uint[] _hash;

		// Token: 0x04000039 RID: 57
		private uint _hashMask;

		// Token: 0x0400003A RID: 58
		private uint _hashSizeSum;

		// Token: 0x0400003B RID: 59
		private uint _matchMaxLen;

		// Token: 0x0400003C RID: 60
		private uint[] _son;

		// Token: 0x0400003D RID: 61
		private uint kFixHashSize = 66560U;

		// Token: 0x0400003E RID: 62
		private uint kMinMatchCheck = 4U;

		// Token: 0x0400003F RID: 63
		private uint kNumHashDirectBytes;
	}
}
