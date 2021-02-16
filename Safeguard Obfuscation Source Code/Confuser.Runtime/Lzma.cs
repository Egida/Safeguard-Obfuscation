using System;
using System.IO;

namespace Confuser.Runtime
{
	// Token: 0x02000027 RID: 39
	internal static class Lzma
	{
		// Token: 0x06000083 RID: 131 RVA: 0x00005838 File Offset: 0x00003A38
		public static byte[] Decompress(byte[] data)
		{
			MemoryStream star = new MemoryStream(data);
			Lzma.LzmaDecoder decoder = new Lzma.LzmaDecoder();
			byte[] prop = new byte[5];
			star.Read(prop, 0, 5);
			decoder.SetDecoderProperties(prop);
			long outSize = 0L;
			for (int i = 0; i < 8; i++)
			{
				int v = star.ReadByte();
				outSize |= (long)((long)((ulong)((byte)v)) << 8 * i);
			}
			byte[] array = new byte[(int)outSize];
			MemoryStream zzzzz = new MemoryStream(array, true);
			long compressedSize = star.Length - 13L;
			decoder.Code(star, zzzzz, compressedSize, outSize);
			return array;
		}

		// Token: 0x0400007B RID: 123
		private const uint kAlignTableSize = 16U;

		// Token: 0x0400007C RID: 124
		private const uint kEndPosModelIndex = 14U;

		// Token: 0x0400007D RID: 125
		private const uint kMatchMinLen = 2U;

		// Token: 0x0400007E RID: 126
		private const int kNumAlignBits = 4;

		// Token: 0x0400007F RID: 127
		private const uint kNumFullDistances = 128U;

		// Token: 0x04000080 RID: 128
		private const int kNumHighLenBits = 8;

		// Token: 0x04000081 RID: 129
		private const uint kNumLenToPosStates = 4U;

		// Token: 0x04000082 RID: 130
		private const int kNumLowLenBits = 3;

		// Token: 0x04000083 RID: 131
		private const uint kNumLowLenSymbols = 8U;

		// Token: 0x04000084 RID: 132
		private const int kNumMidLenBits = 3;

		// Token: 0x04000085 RID: 133
		private const uint kNumMidLenSymbols = 8U;

		// Token: 0x04000086 RID: 134
		private const int kNumPosSlotBits = 6;

		// Token: 0x04000087 RID: 135
		private const int kNumPosStatesBitsMax = 4;

		// Token: 0x04000088 RID: 136
		private const uint kNumPosStatesMax = 16U;

		// Token: 0x04000089 RID: 137
		private const uint kNumStates = 12U;

		// Token: 0x0400008A RID: 138
		private const uint kStartPosModelIndex = 4U;

		// Token: 0x02000028 RID: 40
		private struct BitDecoder
		{
			// Token: 0x06000084 RID: 132 RVA: 0x000058BC File Offset: 0x00003ABC
			public uint Decode(Lzma.Decoder rangeDecoder)
			{
				uint newBound = (rangeDecoder.Range >> 11) * this.Prob;
				if (rangeDecoder.Code < newBound)
				{
					rangeDecoder.Range = newBound;
					this.Prob += 2048U - this.Prob >> 5;
					if (rangeDecoder.Range < 16777216U)
					{
						rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
						rangeDecoder.Range <<= 8;
					}
					return 0U;
				}
				rangeDecoder.Range -= newBound;
				rangeDecoder.Code -= newBound;
				this.Prob -= this.Prob >> 5;
				if (rangeDecoder.Range < 16777216U)
				{
					rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
					rangeDecoder.Range <<= 8;
				}
				return 1U;
			}

			// Token: 0x06000085 RID: 133 RVA: 0x0000247A File Offset: 0x0000067A
			public void Init()
			{
				this.Prob = 1024U;
			}

			// Token: 0x0400008B RID: 139
			public const int kNumBitModelTotalBits = 11;

			// Token: 0x0400008C RID: 140
			public const uint kBitModelTotal = 2048U;

			// Token: 0x0400008D RID: 141
			private const int kNumMoveBits = 5;

			// Token: 0x0400008E RID: 142
			private uint Prob;
		}

		// Token: 0x02000029 RID: 41
		private struct BitTreeDecoder
		{
			// Token: 0x06000086 RID: 134 RVA: 0x00002487 File Offset: 0x00000687
			public BitTreeDecoder(int numBitLevels)
			{
				this.NumBitLevels = numBitLevels;
				this.Models = new Lzma.BitDecoder[1 << numBitLevels];
			}

			// Token: 0x06000087 RID: 135 RVA: 0x000059A8 File Offset: 0x00003BA8
			public uint Decode(Lzma.Decoder rangeDecoder)
			{
				uint i = 1U;
				for (int bitIndex = this.NumBitLevels; bitIndex > 0; bitIndex--)
				{
					i = (i << 1) + this.Models[(int)((uint)((UIntPtr)i))].Decode(rangeDecoder);
				}
				return i - (1U << this.NumBitLevels);
			}

			// Token: 0x06000088 RID: 136 RVA: 0x000059F8 File Offset: 0x00003BF8
			public void Init()
			{
				uint i = 1U;
				while ((ulong)i < 1UL << (this.NumBitLevels & 31))
				{
					this.Models[(int)((uint)((UIntPtr)i))].Init();
					i += 1U;
				}
			}

			// Token: 0x06000089 RID: 137 RVA: 0x00005A3C File Offset: 0x00003C3C
			public uint ReverseDecode(Lzma.Decoder rangeDecoder)
			{
				uint i = 1U;
				uint symbol = 0U;
				for (int bitIndex = 0; bitIndex < this.NumBitLevels; bitIndex++)
				{
					uint bit = this.Models[(int)((uint)((UIntPtr)i))].Decode(rangeDecoder);
					i <<= 1;
					i += bit;
					symbol |= bit << bitIndex;
				}
				return symbol;
			}

			// Token: 0x0600008A RID: 138 RVA: 0x00005A90 File Offset: 0x00003C90
			public static uint ReverseDecode(Lzma.BitDecoder[] Models, uint startIndex, Lzma.Decoder rangeDecoder, int NumBitLevels)
			{
				uint i = 1U;
				uint symbol = 0U;
				for (int bitIndex = 0; bitIndex < NumBitLevels; bitIndex++)
				{
					uint bit = Models[(int)((uint)((UIntPtr)(startIndex + i)))].Decode(rangeDecoder);
					i <<= 1;
					i += bit;
					symbol |= bit << bitIndex;
				}
				return symbol;
			}

			// Token: 0x0400008F RID: 143
			private readonly Lzma.BitDecoder[] Models;

			// Token: 0x04000090 RID: 144
			private readonly int NumBitLevels;
		}

		// Token: 0x0200002A RID: 42
		private class Decoder
		{
			// Token: 0x0600008B RID: 139 RVA: 0x00005ADC File Offset: 0x00003CDC
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
					if (range < 16777216U)
					{
						code = (code << 8 | (uint)((byte)this.Stream.ReadByte()));
						range <<= 8;
					}
				}
				this.Range = range;
				this.Code = code;
				return result;
			}

			// Token: 0x0600008C RID: 140 RVA: 0x00005B50 File Offset: 0x00003D50
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

			// Token: 0x0600008D RID: 141 RVA: 0x000024A1 File Offset: 0x000006A1
			public void Normalize()
			{
				while (this.Range < 16777216U)
				{
					this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
					this.Range <<= 8;
				}
			}

			// Token: 0x0600008E RID: 142 RVA: 0x000024DB File Offset: 0x000006DB
			public void ReleaseStream()
			{
				this.Stream = null;
			}

			// Token: 0x0600008F RID: 143 RVA: 0x000020D5 File Offset: 0x000002D5
			public Decoder()
			{
			}

			// Token: 0x04000091 RID: 145
			public uint Code;

			// Token: 0x04000092 RID: 146
			public const uint kTopValue = 16777216U;

			// Token: 0x04000093 RID: 147
			public uint Range;

			// Token: 0x04000094 RID: 148
			public Stream Stream;
		}

		// Token: 0x0200002B RID: 43
		private class LzmaDecoder
		{
			// Token: 0x06000090 RID: 144 RVA: 0x00005B9C File Offset: 0x00003D9C
			public LzmaDecoder()
			{
				this.m_DictionarySize = uint.MaxValue;
				int i = 0;
				while ((long)i < 4L)
				{
					this.m_PosSlotDecoder[i] = new Lzma.BitTreeDecoder(6);
					i++;
				}
			}

			// Token: 0x06000091 RID: 145 RVA: 0x00005C88 File Offset: 0x00003E88
			public void Code(Stream inStream, Stream outStream, long inSize, long outSize)
			{
				this.Init(inStream, outStream);
				Lzma.State state = default(Lzma.State);
				state.Init();
				uint rep0 = 0U;
				uint rep = 0U;
				uint rep2 = 0U;
				uint rep3 = 0U;
				ulong nowPos64 = 0UL;
				if (nowPos64 < (ulong)outSize)
				{
					this.m_IsMatchDecoders[(int)((uint)((UIntPtr)(state.Index << 4)))].Decode(this.m_RangeDecoder);
					state.UpdateChar();
					byte babe = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, 0U, 0);
					this.m_OutWindow.PutByte(babe);
					nowPos64 += 1UL;
				}
				while (nowPos64 < (ulong)outSize)
				{
					uint posState = (uint)nowPos64 & this.m_PosStateMask;
					if (this.m_IsMatchDecoders[(int)((uint)((UIntPtr)((state.Index << 4) + posState)))].Decode(this.m_RangeDecoder) == 0U)
					{
						byte prevByte = this.m_OutWindow.GetByte(0U);
						byte b2;
						if (!state.IsCharState())
						{
							b2 = this.m_LiteralDecoder.DecodeWithMatchByte(this.m_RangeDecoder, (uint)nowPos64, prevByte, this.m_OutWindow.GetByte(rep0));
						}
						else
						{
							b2 = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, (uint)nowPos64, prevByte);
						}
						this.m_OutWindow.PutByte(b2);
						state.UpdateChar();
						nowPos64 += 1UL;
					}
					else
					{
						uint len;
						if (this.m_IsRepDecoders[(int)((uint)((UIntPtr)state.Index))].Decode(this.m_RangeDecoder) == 1U)
						{
							if (this.m_IsRepG0Decoders[(int)((uint)((UIntPtr)state.Index))].Decode(this.m_RangeDecoder) == 0U)
							{
								if (this.m_IsRep0LongDecoders[(int)((uint)((UIntPtr)((state.Index << 4) + posState)))].Decode(this.m_RangeDecoder) == 0U)
								{
									state.UpdateShortRep();
									this.m_OutWindow.PutByte(this.m_OutWindow.GetByte(rep0));
									nowPos64 += 1UL;
									continue;
								}
							}
							else
							{
								uint distance;
								if (this.m_IsRepG1Decoders[(int)((uint)((UIntPtr)state.Index))].Decode(this.m_RangeDecoder) == 0U)
								{
									distance = rep;
								}
								else
								{
									if (this.m_IsRepG2Decoders[(int)((uint)((UIntPtr)state.Index))].Decode(this.m_RangeDecoder) == 0U)
									{
										distance = rep2;
									}
									else
									{
										distance = rep3;
										rep3 = rep2;
									}
									rep2 = rep;
								}
								rep = rep0;
								rep0 = distance;
							}
							len = this.m_RepLenDecoder.Decode(this.m_RangeDecoder, posState) + 2U;
							state.UpdateRep();
						}
						else
						{
							rep3 = rep2;
							rep2 = rep;
							rep = rep0;
							len = 2U + this.m_LenDecoder.Decode(this.m_RangeDecoder, posState);
							state.UpdateMatch();
							uint posSlot = this.m_PosSlotDecoder[(int)((uint)((UIntPtr)Lzma.LzmaDecoder.GetLenToPosState(len)))].Decode(this.m_RangeDecoder);
							if (posSlot >= 4U)
							{
								int numDirectBits = (int)((posSlot >> 1) - 1U);
								rep0 = (2U | (posSlot & 1U)) << numDirectBits;
								if (posSlot < 14U)
								{
									rep0 += Lzma.BitTreeDecoder.ReverseDecode(this.m_PosDecoders, rep0 - posSlot - 1U, this.m_RangeDecoder, numDirectBits);
								}
								else
								{
									rep0 += this.m_RangeDecoder.DecodeDirectBits(numDirectBits - 4) << 4;
									rep0 += this.m_PosAlignDecoder.ReverseDecode(this.m_RangeDecoder);
								}
							}
							else
							{
								rep0 = posSlot;
							}
						}
						if (((ulong)rep0 >= nowPos64 || rep0 >= this.m_DictionarySizeCheck) && rep0 == 4294967295U)
						{
							break;
						}
						this.m_OutWindow.CopyBlock(rep0, len);
						nowPos64 += (ulong)len;
					}
				}
				this.m_OutWindow.Flush();
				this.m_OutWindow.ReleaseStream();
				this.m_RangeDecoder.ReleaseStream();
			}

			// Token: 0x06000092 RID: 146 RVA: 0x000024E4 File Offset: 0x000006E4
			private static uint GetLenToPosState(uint len)
			{
				len -= 2U;
				if (len < 4U)
				{
					return len;
				}
				return 3U;
			}

			// Token: 0x06000093 RID: 147 RVA: 0x00006014 File Offset: 0x00004214
			private void Init(Stream inStream, Stream outStream)
			{
				this.m_RangeDecoder.Init(inStream);
				this.m_OutWindow.Init(outStream, this._solid);
				for (uint i = 0U; i < 12U; i += 1U)
				{
					for (uint j = 0U; j <= this.m_PosStateMask; j += 1U)
					{
						uint index = (i << 4) + j;
						this.m_IsMatchDecoders[(int)((uint)((UIntPtr)index))].Init();
						this.m_IsRep0LongDecoders[(int)((uint)((UIntPtr)index))].Init();
					}
					this.m_IsRepDecoders[(int)((uint)((UIntPtr)i))].Init();
					this.m_IsRepG0Decoders[(int)((uint)((UIntPtr)i))].Init();
					this.m_IsRepG1Decoders[(int)((uint)((UIntPtr)i))].Init();
					this.m_IsRepG2Decoders[(int)((uint)((UIntPtr)i))].Init();
				}
				this.m_LiteralDecoder.Init();
				for (uint k = 0U; k < 4U; k += 1U)
				{
					this.m_PosSlotDecoder[(int)((uint)((UIntPtr)k))].Init();
				}
				for (uint l = 0U; l < 114U; l += 1U)
				{
					this.m_PosDecoders[(int)((uint)((UIntPtr)l))].Init();
				}
				this.m_LenDecoder.Init();
				this.m_RepLenDecoder.Init();
				this.m_PosAlignDecoder.Init();
			}

			// Token: 0x06000094 RID: 148 RVA: 0x0000618C File Offset: 0x0000438C
			public void SetDecoderProperties(byte[] properties)
			{
				int lc = (int)(properties[0] % 9);
				byte b = properties[0] / 9;
				int lp = (int)(b % 5);
				int pb = (int)(b / 5);
				uint dictionarySize = 0U;
				for (int i = 0; i < 4; i++)
				{
					dictionarySize += (uint)((uint)properties[1 + i] << i * 8);
				}
				this.SetDictionarySize(dictionarySize);
				this.SetLiteralProperties(lp, lc);
				this.SetPosBitsProperties(pb);
			}

			// Token: 0x06000095 RID: 149 RVA: 0x000061E8 File Offset: 0x000043E8
			private void SetDictionarySize(uint dictionarySize)
			{
				if (this.m_DictionarySize != dictionarySize)
				{
					this.m_DictionarySize = dictionarySize;
					this.m_DictionarySizeCheck = Math.Max(this.m_DictionarySize, 1U);
					uint blockSize = Math.Max(this.m_DictionarySizeCheck, 4096U);
					this.m_OutWindow.Create(blockSize);
				}
			}

			// Token: 0x06000096 RID: 150 RVA: 0x000024F2 File Offset: 0x000006F2
			private void SetLiteralProperties(int lp, int lc)
			{
				this.m_LiteralDecoder.Create(lp, lc);
			}

			// Token: 0x06000097 RID: 151 RVA: 0x00006234 File Offset: 0x00004434
			private void SetPosBitsProperties(int pb)
			{
				uint numPosStates = 1U << pb;
				this.m_LenDecoder.Create(numPosStates);
				this.m_RepLenDecoder.Create(numPosStates);
				this.m_PosStateMask = numPosStates - 1U;
			}

			// Token: 0x04000095 RID: 149
			private uint m_DictionarySize;

			// Token: 0x04000096 RID: 150
			private uint m_DictionarySizeCheck;

			// Token: 0x04000097 RID: 151
			private readonly Lzma.BitDecoder[] m_IsMatchDecoders = new Lzma.BitDecoder[192];

			// Token: 0x04000098 RID: 152
			private readonly Lzma.BitDecoder[] m_IsRep0LongDecoders = new Lzma.BitDecoder[192];

			// Token: 0x04000099 RID: 153
			private readonly Lzma.BitDecoder[] m_IsRepDecoders = new Lzma.BitDecoder[12];

			// Token: 0x0400009A RID: 154
			private readonly Lzma.BitDecoder[] m_IsRepG0Decoders = new Lzma.BitDecoder[12];

			// Token: 0x0400009B RID: 155
			private readonly Lzma.BitDecoder[] m_IsRepG1Decoders = new Lzma.BitDecoder[12];

			// Token: 0x0400009C RID: 156
			private readonly Lzma.BitDecoder[] m_IsRepG2Decoders = new Lzma.BitDecoder[12];

			// Token: 0x0400009D RID: 157
			private readonly Lzma.LzmaDecoder.LenDecoder m_LenDecoder = new Lzma.LzmaDecoder.LenDecoder();

			// Token: 0x0400009E RID: 158
			private readonly Lzma.LzmaDecoder.LiteralDecoder m_LiteralDecoder = new Lzma.LzmaDecoder.LiteralDecoder();

			// Token: 0x0400009F RID: 159
			private readonly Lzma.OutWindow m_OutWindow = new Lzma.OutWindow();

			// Token: 0x040000A0 RID: 160
			private Lzma.BitTreeDecoder m_PosAlignDecoder = new Lzma.BitTreeDecoder(4);

			// Token: 0x040000A1 RID: 161
			private readonly Lzma.BitDecoder[] m_PosDecoders = new Lzma.BitDecoder[114];

			// Token: 0x040000A2 RID: 162
			private readonly Lzma.BitTreeDecoder[] m_PosSlotDecoder = new Lzma.BitTreeDecoder[4];

			// Token: 0x040000A3 RID: 163
			private uint m_PosStateMask;

			// Token: 0x040000A4 RID: 164
			private readonly Lzma.Decoder m_RangeDecoder = new Lzma.Decoder();

			// Token: 0x040000A5 RID: 165
			private readonly Lzma.LzmaDecoder.LenDecoder m_RepLenDecoder = new Lzma.LzmaDecoder.LenDecoder();

			// Token: 0x040000A6 RID: 166
			private bool _solid;

			// Token: 0x0200002C RID: 44
			private class LenDecoder
			{
				// Token: 0x06000098 RID: 152 RVA: 0x0000626C File Offset: 0x0000446C
				public void Create(uint numPosStates)
				{
					for (uint posState = this.m_NumPosStates; posState < numPosStates; posState += 1U)
					{
						this.m_LowCoder[(int)((uint)((UIntPtr)posState))] = new Lzma.BitTreeDecoder(3);
						this.m_MidCoder[(int)((uint)((UIntPtr)posState))] = new Lzma.BitTreeDecoder(3);
					}
					this.m_NumPosStates = numPosStates;
				}

				// Token: 0x06000099 RID: 153 RVA: 0x000062CC File Offset: 0x000044CC
				public uint Decode(Lzma.Decoder rangeDecoder, uint posState)
				{
					if (this.m_Choice.Decode(rangeDecoder) == 0U)
					{
						return this.m_LowCoder[(int)((uint)((UIntPtr)posState))].Decode(rangeDecoder);
					}
					uint symbol = 8U;
					if (this.m_Choice2.Decode(rangeDecoder) == 0U)
					{
						symbol += this.m_MidCoder[(int)((uint)((UIntPtr)posState))].Decode(rangeDecoder);
					}
					else
					{
						symbol += 8U;
						symbol += this.m_HighCoder.Decode(rangeDecoder);
					}
					return symbol;
				}

				// Token: 0x0600009A RID: 154 RVA: 0x0000634C File Offset: 0x0000454C
				public void Init()
				{
					this.m_Choice.Init();
					for (uint posState = 0U; posState < this.m_NumPosStates; posState += 1U)
					{
						this.m_LowCoder[(int)((uint)((UIntPtr)posState))].Init();
						this.m_MidCoder[(int)((uint)((UIntPtr)posState))].Init();
					}
					this.m_Choice2.Init();
					this.m_HighCoder.Init();
				}

				// Token: 0x0600009B RID: 155 RVA: 0x00002501 File Offset: 0x00000701
				public LenDecoder()
				{
				}

				// Token: 0x040000A7 RID: 167
				private Lzma.BitDecoder m_Choice;

				// Token: 0x040000A8 RID: 168
				private Lzma.BitDecoder m_Choice2;

				// Token: 0x040000A9 RID: 169
				private Lzma.BitTreeDecoder m_HighCoder = new Lzma.BitTreeDecoder(8);

				// Token: 0x040000AA RID: 170
				private readonly Lzma.BitTreeDecoder[] m_LowCoder = new Lzma.BitTreeDecoder[16];

				// Token: 0x040000AB RID: 171
				private readonly Lzma.BitTreeDecoder[] m_MidCoder = new Lzma.BitTreeDecoder[16];

				// Token: 0x040000AC RID: 172
				private uint m_NumPosStates;
			}

			// Token: 0x0200002D RID: 45
			private class LiteralDecoder
			{
				// Token: 0x0600009C RID: 156 RVA: 0x000063C4 File Offset: 0x000045C4
				public void Create(int numPosBits, int numPrevBits)
				{
					if (this.m_Coders != null && this.m_NumPrevBits == numPrevBits && this.m_NumPosBits == numPosBits)
					{
						return;
					}
					this.m_NumPosBits = numPosBits;
					this.m_PosMask = (1U << numPosBits) - 1U;
					this.m_NumPrevBits = numPrevBits;
					uint numStates = 1U << this.m_NumPrevBits + this.m_NumPosBits;
					this.m_Coders = new Lzma.LzmaDecoder.LiteralDecoder.Decoder2[numStates];
					for (uint i = 0U; i < numStates; i += 1U)
					{
						this.m_Coders[(int)((uint)((UIntPtr)i))].Create();
					}
				}

				// Token: 0x0600009D RID: 157 RVA: 0x0000252F File Offset: 0x0000072F
				public byte DecodeNormal(Lzma.Decoder rangeDecoder, uint pos, byte prevByte)
				{
					return this.m_Coders[(int)((uint)((UIntPtr)this.GetState(pos, prevByte)))].DecodeNormal(rangeDecoder);
				}

				// Token: 0x0600009E RID: 158 RVA: 0x00002554 File Offset: 0x00000754
				public byte DecodeWithMatchByte(Lzma.Decoder rangeDecoder, uint pos, byte prevByte, byte matchByte)
				{
					return this.m_Coders[(int)((uint)((UIntPtr)this.GetState(pos, prevByte)))].DecodeWithMatchByte(rangeDecoder, matchByte);
				}

				// Token: 0x0600009F RID: 159 RVA: 0x0000257B File Offset: 0x0000077B
				private uint GetState(uint pos, byte prevByte)
				{
					return ((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> 8 - this.m_NumPrevBits);
				}

				// Token: 0x060000A0 RID: 160 RVA: 0x00006450 File Offset: 0x00004650
				public void Init()
				{
					uint numStates = 1U << this.m_NumPrevBits + this.m_NumPosBits;
					for (uint i = 0U; i < numStates; i += 1U)
					{
						this.m_Coders[(int)((uint)((UIntPtr)i))].Init();
					}
				}

				// Token: 0x060000A1 RID: 161 RVA: 0x000020D5 File Offset: 0x000002D5
				public LiteralDecoder()
				{
				}

				// Token: 0x040000AD RID: 173
				private Lzma.LzmaDecoder.LiteralDecoder.Decoder2[] m_Coders;

				// Token: 0x040000AE RID: 174
				private int m_NumPosBits;

				// Token: 0x040000AF RID: 175
				private int m_NumPrevBits;

				// Token: 0x040000B0 RID: 176
				private uint m_PosMask;

				// Token: 0x0200002E RID: 46
				private struct Decoder2
				{
					// Token: 0x060000A2 RID: 162 RVA: 0x0000259D File Offset: 0x0000079D
					public void Create()
					{
						this.m_Decoders = new Lzma.BitDecoder[768];
					}

					// Token: 0x060000A3 RID: 163 RVA: 0x00006498 File Offset: 0x00004698
					public byte DecodeNormal(Lzma.Decoder rangeDecoder)
					{
						uint symbol = 1U;
						do
						{
							symbol = (symbol << 1 | this.m_Decoders[(int)((uint)((UIntPtr)symbol))].Decode(rangeDecoder));
						}
						while (symbol < 256U);
						return (byte)symbol;
					}

					// Token: 0x060000A4 RID: 164 RVA: 0x000064D4 File Offset: 0x000046D4
					public byte DecodeWithMatchByte(Lzma.Decoder rangeDecoder, byte matchByte)
					{
						uint symbol = 1U;
						for (;;)
						{
							uint matchBit = (uint)(matchByte >> 7 & 1);
							matchByte = (byte)(matchByte << 1);
							uint bit = this.m_Decoders[(int)((uint)((UIntPtr)((1U + matchBit << 8) + symbol)))].Decode(rangeDecoder);
							symbol = (symbol << 1 | bit);
							if (matchBit != bit)
							{
								break;
							}
							if (symbol >= 256U)
							{
								goto Block_2;
							}
						}
						while (symbol < 256U)
						{
							symbol = (symbol << 1 | this.m_Decoders[(int)((uint)((UIntPtr)symbol))].Decode(rangeDecoder));
						}
						Block_2:
						return (byte)symbol;
					}

					// Token: 0x060000A5 RID: 165 RVA: 0x00006554 File Offset: 0x00004754
					public void Init()
					{
						for (int i = 0; i < 768; i++)
						{
							this.m_Decoders[i].Init();
						}
					}

					// Token: 0x040000B1 RID: 177
					private Lzma.BitDecoder[] m_Decoders;
				}
			}
		}

		// Token: 0x0200002F RID: 47
		private class OutWindow
		{
			// Token: 0x060000A6 RID: 166 RVA: 0x00006584 File Offset: 0x00004784
			public void CopyBlock(uint distance, uint len)
			{
				uint pos = this._pos - distance - 1U;
				if (pos >= this._windowSize)
				{
					pos += this._windowSize;
				}
				while (len > 0U)
				{
					if (pos >= this._windowSize)
					{
						pos = 0U;
					}
					byte[] buffer = this._buffer;
					uint pos2 = this._pos;
					this._pos = pos2 + 1U;
					buffer[(int)((uint)((UIntPtr)pos2))] = this._buffer[(int)((uint)((UIntPtr)(pos++)))];
					if (this._pos >= this._windowSize)
					{
						this.Flush();
					}
					len -= 1U;
				}
			}

			// Token: 0x060000A7 RID: 167 RVA: 0x000025AF File Offset: 0x000007AF
			public void Create(uint windowSize)
			{
				if (this._windowSize != windowSize)
				{
					this._buffer = new byte[windowSize];
				}
				this._windowSize = windowSize;
				this._pos = 0U;
				this._streamPos = 0U;
			}

			// Token: 0x060000A8 RID: 168 RVA: 0x00006610 File Offset: 0x00004810
			public void Flush()
			{
				uint size = this._pos - this._streamPos;
				if (size == 0U)
				{
					return;
				}
				this._stream.Write(this._buffer, (int)this._streamPos, (int)size);
				if (this._pos >= this._windowSize)
				{
					this._pos = 0U;
				}
				this._streamPos = this._pos;
			}

			// Token: 0x060000A9 RID: 169 RVA: 0x00006668 File Offset: 0x00004868
			public byte GetByte(uint distance)
			{
				uint pos = this._pos - distance - 1U;
				if (pos >= this._windowSize)
				{
					pos += this._windowSize;
				}
				return this._buffer[(int)((uint)((UIntPtr)pos))];
			}

			// Token: 0x060000AA RID: 170 RVA: 0x000025DB File Offset: 0x000007DB
			public void Init(Stream stream, bool solid)
			{
				this.ReleaseStream();
				this._stream = stream;
				if (!solid)
				{
					this._streamPos = 0U;
					this._pos = 0U;
				}
			}

			// Token: 0x060000AB RID: 171 RVA: 0x000066A4 File Offset: 0x000048A4
			public void PutByte(byte babe)
			{
				byte[] buffer = this._buffer;
				uint pos = this._pos;
				this._pos = pos + 1U;
				buffer[(int)((uint)((UIntPtr)pos))] = babe;
				if (this._pos >= this._windowSize)
				{
					this.Flush();
				}
			}

			// Token: 0x060000AC RID: 172 RVA: 0x000025FB File Offset: 0x000007FB
			public void ReleaseStream()
			{
				this.Flush();
				this._stream = null;
				Buffer.BlockCopy(new byte[this._buffer.Length], 0, this._buffer, 0, this._buffer.Length);
			}

			// Token: 0x060000AD RID: 173 RVA: 0x000020D5 File Offset: 0x000002D5
			public OutWindow()
			{
			}

			// Token: 0x040000B2 RID: 178
			private byte[] _buffer;

			// Token: 0x040000B3 RID: 179
			private uint _pos;

			// Token: 0x040000B4 RID: 180
			private Stream _stream;

			// Token: 0x040000B5 RID: 181
			private uint _streamPos;

			// Token: 0x040000B6 RID: 182
			private uint _windowSize;
		}

		// Token: 0x02000030 RID: 48
		private struct State
		{
			// Token: 0x060000AE RID: 174 RVA: 0x0000262C File Offset: 0x0000082C
			public void Init()
			{
				this.Index = 0U;
			}

			// Token: 0x060000AF RID: 175 RVA: 0x00002635 File Offset: 0x00000835
			public bool IsCharState()
			{
				return this.Index < 7U;
			}

			// Token: 0x060000B0 RID: 176 RVA: 0x00002640 File Offset: 0x00000840
			public void UpdateChar()
			{
				if (this.Index < 4U)
				{
					this.Index = 0U;
					return;
				}
				if (this.Index < 10U)
				{
					this.Index -= 3U;
					return;
				}
				this.Index -= 6U;
			}

			// Token: 0x060000B1 RID: 177 RVA: 0x0000267A File Offset: 0x0000087A
			public void UpdateMatch()
			{
				this.Index = ((this.Index < 7U) ? 7U : 10U);
			}

			// Token: 0x060000B2 RID: 178 RVA: 0x00002690 File Offset: 0x00000890
			public void UpdateRep()
			{
				this.Index = ((this.Index < 7U) ? 8U : 11U);
			}

			// Token: 0x060000B3 RID: 179 RVA: 0x000026A6 File Offset: 0x000008A6
			public void UpdateShortRep()
			{
				this.Index = ((this.Index < 7U) ? 9U : 11U);
			}

			// Token: 0x040000B7 RID: 183
			public uint Index;
		}
	}
}
