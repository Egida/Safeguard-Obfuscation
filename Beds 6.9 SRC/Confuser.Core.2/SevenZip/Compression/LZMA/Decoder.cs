using System;
using System.IO;
using SevenZip.Compression.LZ;
using SevenZip.Compression.RangeCoder;

namespace SevenZip.Compression.LZMA
{
	// Token: 0x02000018 RID: 24
	internal class Decoder : ICoder, ISetDecoderProperties
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00005720 File Offset: 0x00003920
		public Decoder()
		{
			this.m_DictionarySize = uint.MaxValue;
			int i = 0;
			while ((long)i < 4L)
			{
				this.m_PosSlotDecoder[i] = new BitTreeDecoder(6);
				i++;
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005810 File Offset: 0x00003A10
		public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
		{
			this.Init(inStream, outStream);
			Base.State state = default(Base.State);
			state.Init();
			uint rep0 = 0U;
			uint rep = 0U;
			uint rep2 = 0U;
			uint rep3 = 0U;
			ulong nowPos64 = 0UL;
			bool flag = nowPos64 < (ulong)outSize;
			if (flag)
			{
				bool flag2 = this.m_IsMatchDecoders[(int)((int)state.Index << 4)].Decode(this.m_RangeDecoder) > 0U;
				if (flag2)
				{
					throw new DataErrorException();
				}
				state.UpdateChar();
				byte b = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, 0U, 0);
				this.m_OutWindow.PutByte(b);
				nowPos64 += 1UL;
			}
			while (nowPos64 < (ulong)outSize)
			{
				uint posState = (uint)nowPos64 & this.m_PosStateMask;
				bool flag3 = this.m_IsMatchDecoders[(int)((state.Index << 4) + posState)].Decode(this.m_RangeDecoder) == 0U;
				if (flag3)
				{
					byte prevByte = this.m_OutWindow.GetByte(0U);
					bool flag4 = !state.IsCharState();
					byte b2;
					if (flag4)
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
					bool flag5 = this.m_IsRepDecoders[(int)state.Index].Decode(this.m_RangeDecoder) == 1U;
					uint len;
					if (flag5)
					{
						bool flag6 = this.m_IsRepG0Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U;
						if (flag6)
						{
							bool flag7 = this.m_IsRep0LongDecoders[(int)((state.Index << 4) + posState)].Decode(this.m_RangeDecoder) == 0U;
							if (flag7)
							{
								state.UpdateShortRep();
								this.m_OutWindow.PutByte(this.m_OutWindow.GetByte(rep0));
								nowPos64 += 1UL;
								continue;
							}
						}
						else
						{
							bool flag8 = this.m_IsRepG1Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U;
							uint distance;
							if (flag8)
							{
								distance = rep;
							}
							else
							{
								bool flag9 = this.m_IsRepG2Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U;
								if (flag9)
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
						uint posSlot = this.m_PosSlotDecoder[(int)Base.GetLenToPosState(len)].Decode(this.m_RangeDecoder);
						bool flag10 = posSlot >= 4U;
						if (flag10)
						{
							int numDirectBits = (int)((posSlot >> 1) - 1U);
							rep0 = (2U | (posSlot & 1U)) << numDirectBits;
							bool flag11 = posSlot < 14U;
							if (flag11)
							{
								rep0 += BitTreeDecoder.ReverseDecode(this.m_PosDecoders, rep0 - posSlot - 1U, this.m_RangeDecoder, numDirectBits);
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
					bool flag12 = (ulong)rep0 >= (ulong)this.m_OutWindow.TrainSize + nowPos64 || rep0 >= this.m_DictionarySizeCheck;
					if (flag12)
					{
						bool flag13 = rep0 == uint.MaxValue;
						if (flag13)
						{
							break;
						}
						throw new DataErrorException();
					}
					else
					{
						this.m_OutWindow.CopyBlock(rep0, len);
						nowPos64 += (ulong)len;
					}
				}
			}
			this.m_OutWindow.Flush();
			this.m_OutWindow.ReleaseStream();
			this.m_RangeDecoder.ReleaseStream();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005BFC File Offset: 0x00003DFC
		public void SetDecoderProperties(byte[] properties)
		{
			bool flag = properties.Length < 5;
			if (flag)
			{
				throw new InvalidParamException();
			}
			int lc = (int)(properties[0] % 9);
			int remainder = (int)(properties[0] / 9);
			int lp = remainder % 5;
			int pb = remainder / 5;
			bool flag2 = pb > 4;
			if (flag2)
			{
				throw new InvalidParamException();
			}
			uint dictionarySize = 0U;
			for (int i = 0; i < 4; i++)
			{
				dictionarySize += (uint)((uint)properties[1 + i] << i * 8);
			}
			this.SetDictionarySize(dictionarySize);
			this.SetLiteralProperties(lp, lc);
			this.SetPosBitsProperties(pb);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00005C88 File Offset: 0x00003E88
		private void SetDictionarySize(uint dictionarySize)
		{
			bool flag = this.m_DictionarySize != dictionarySize;
			if (flag)
			{
				this.m_DictionarySize = dictionarySize;
				this.m_DictionarySizeCheck = Math.Max(this.m_DictionarySize, 1U);
				uint blockSize = Math.Max(this.m_DictionarySizeCheck, 4096U);
				this.m_OutWindow.Create(blockSize);
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005CE0 File Offset: 0x00003EE0
		private void SetLiteralProperties(int lp, int lc)
		{
			bool flag = lp > 8;
			if (flag)
			{
				throw new InvalidParamException();
			}
			bool flag2 = lc > 8;
			if (flag2)
			{
				throw new InvalidParamException();
			}
			this.m_LiteralDecoder.Create(lp, lc);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00005D18 File Offset: 0x00003F18
		private void SetPosBitsProperties(int pb)
		{
			bool flag = pb > 4;
			if (flag)
			{
				throw new InvalidParamException();
			}
			uint numPosStates = 1U << pb;
			this.m_LenDecoder.Create(numPosStates);
			this.m_RepLenDecoder.Create(numPosStates);
			this.m_PosStateMask = numPosStates - 1U;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005D60 File Offset: 0x00003F60
		private void Init(Stream inStream, Stream outStream)
		{
			this.m_RangeDecoder.Init(inStream);
			this.m_OutWindow.Init(outStream, this._solid);
			for (uint i = 0U; i < 12U; i += 1U)
			{
				for (uint j = 0U; j <= this.m_PosStateMask; j += 1U)
				{
					uint index = (i << 4) + j;
					this.m_IsMatchDecoders[(int)index].Init();
					this.m_IsRep0LongDecoders[(int)index].Init();
				}
				this.m_IsRepDecoders[(int)i].Init();
				this.m_IsRepG0Decoders[(int)i].Init();
				this.m_IsRepG1Decoders[(int)i].Init();
				this.m_IsRepG2Decoders[(int)i].Init();
			}
			this.m_LiteralDecoder.Init();
			for (uint i = 0U; i < 4U; i += 1U)
			{
				this.m_PosSlotDecoder[(int)i].Init();
			}
			for (uint i = 0U; i < 114U; i += 1U)
			{
				this.m_PosDecoders[(int)i].Init();
			}
			this.m_LenDecoder.Init();
			this.m_RepLenDecoder.Init();
			this.m_PosAlignDecoder.Init();
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005EB0 File Offset: 0x000040B0
		public bool Train(Stream stream)
		{
			this._solid = true;
			return this.m_OutWindow.Train(stream);
		}

		// Token: 0x0400006D RID: 109
		private readonly BitDecoder[] m_IsMatchDecoders = new BitDecoder[192];

		// Token: 0x0400006E RID: 110
		private readonly BitDecoder[] m_IsRep0LongDecoders = new BitDecoder[192];

		// Token: 0x0400006F RID: 111
		private readonly BitDecoder[] m_IsRepDecoders = new BitDecoder[12];

		// Token: 0x04000070 RID: 112
		private readonly BitDecoder[] m_IsRepG0Decoders = new BitDecoder[12];

		// Token: 0x04000071 RID: 113
		private readonly BitDecoder[] m_IsRepG1Decoders = new BitDecoder[12];

		// Token: 0x04000072 RID: 114
		private readonly BitDecoder[] m_IsRepG2Decoders = new BitDecoder[12];

		// Token: 0x04000073 RID: 115
		private readonly Decoder.LenDecoder m_LenDecoder = new Decoder.LenDecoder();

		// Token: 0x04000074 RID: 116
		private readonly Decoder.LiteralDecoder m_LiteralDecoder = new Decoder.LiteralDecoder();

		// Token: 0x04000075 RID: 117
		private readonly OutWindow m_OutWindow = new OutWindow();

		// Token: 0x04000076 RID: 118
		private readonly BitDecoder[] m_PosDecoders = new BitDecoder[114];

		// Token: 0x04000077 RID: 119
		private readonly BitTreeDecoder[] m_PosSlotDecoder = new BitTreeDecoder[4];

		// Token: 0x04000078 RID: 120
		private readonly Decoder m_RangeDecoder = new Decoder();

		// Token: 0x04000079 RID: 121
		private readonly Decoder.LenDecoder m_RepLenDecoder = new Decoder.LenDecoder();

		// Token: 0x0400007A RID: 122
		private bool _solid;

		// Token: 0x0400007B RID: 123
		private uint m_DictionarySize;

		// Token: 0x0400007C RID: 124
		private uint m_DictionarySizeCheck;

		// Token: 0x0400007D RID: 125
		private BitTreeDecoder m_PosAlignDecoder = new BitTreeDecoder(4);

		// Token: 0x0400007E RID: 126
		private uint m_PosStateMask;

		// Token: 0x02000019 RID: 25
		private class LenDecoder
		{
			// Token: 0x0600007B RID: 123 RVA: 0x00005ED8 File Offset: 0x000040D8
			public void Create(uint numPosStates)
			{
				for (uint posState = this.m_NumPosStates; posState < numPosStates; posState += 1U)
				{
					this.m_LowCoder[(int)posState] = new BitTreeDecoder(3);
					this.m_MidCoder[(int)posState] = new BitTreeDecoder(3);
				}
				this.m_NumPosStates = numPosStates;
			}

			// Token: 0x0600007C RID: 124 RVA: 0x00005F28 File Offset: 0x00004128
			public void Init()
			{
				this.m_Choice.Init();
				for (uint posState = 0U; posState < this.m_NumPosStates; posState += 1U)
				{
					this.m_LowCoder[(int)posState].Init();
					this.m_MidCoder[(int)posState].Init();
				}
				this.m_Choice2.Init();
				this.m_HighCoder.Init();
			}

			// Token: 0x0600007D RID: 125 RVA: 0x00005F98 File Offset: 0x00004198
			public uint Decode(Decoder rangeDecoder, uint posState)
			{
				bool flag = this.m_Choice.Decode(rangeDecoder) == 0U;
				uint result;
				if (flag)
				{
					result = this.m_LowCoder[(int)posState].Decode(rangeDecoder);
				}
				else
				{
					uint symbol = 8U;
					bool flag2 = this.m_Choice2.Decode(rangeDecoder) == 0U;
					if (flag2)
					{
						symbol += this.m_MidCoder[(int)posState].Decode(rangeDecoder);
					}
					else
					{
						symbol += 8U;
						symbol += this.m_HighCoder.Decode(rangeDecoder);
					}
					result = symbol;
				}
				return result;
			}

			// Token: 0x0600007E RID: 126 RVA: 0x00006014 File Offset: 0x00004214
			public LenDecoder()
			{
			}

			// Token: 0x0400007F RID: 127
			private readonly BitTreeDecoder[] m_LowCoder = new BitTreeDecoder[16];

			// Token: 0x04000080 RID: 128
			private readonly BitTreeDecoder[] m_MidCoder = new BitTreeDecoder[16];

			// Token: 0x04000081 RID: 129
			private BitDecoder m_Choice = default(BitDecoder);

			// Token: 0x04000082 RID: 130
			private BitDecoder m_Choice2 = default(BitDecoder);

			// Token: 0x04000083 RID: 131
			private BitTreeDecoder m_HighCoder = new BitTreeDecoder(8);

			// Token: 0x04000084 RID: 132
			private uint m_NumPosStates;
		}

		// Token: 0x0200001A RID: 26
		private class LiteralDecoder
		{
			// Token: 0x0600007F RID: 127 RVA: 0x00006068 File Offset: 0x00004268
			public void Create(int numPosBits, int numPrevBits)
			{
				bool flag = this.m_Coders != null && this.m_NumPrevBits == numPrevBits && this.m_NumPosBits == numPosBits;
				if (!flag)
				{
					this.m_NumPosBits = numPosBits;
					this.m_PosMask = (1U << numPosBits) - 1U;
					this.m_NumPrevBits = numPrevBits;
					uint numStates = 1U << this.m_NumPrevBits + this.m_NumPosBits;
					this.m_Coders = new Decoder.LiteralDecoder.Decoder2[numStates];
					for (uint i = 0U; i < numStates; i += 1U)
					{
						this.m_Coders[(int)i].Create();
					}
				}
			}

			// Token: 0x06000080 RID: 128 RVA: 0x000060F8 File Offset: 0x000042F8
			public void Init()
			{
				uint numStates = 1U << this.m_NumPrevBits + this.m_NumPosBits;
				for (uint i = 0U; i < numStates; i += 1U)
				{
					this.m_Coders[(int)i].Init();
				}
			}

			// Token: 0x06000081 RID: 129 RVA: 0x0000613C File Offset: 0x0000433C
			private uint GetState(uint pos, byte prevByte)
			{
				return ((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> 8 - this.m_NumPrevBits);
			}

			// Token: 0x06000082 RID: 130 RVA: 0x00006170 File Offset: 0x00004370
			public byte DecodeNormal(Decoder rangeDecoder, uint pos, byte prevByte)
			{
				return this.m_Coders[(int)this.GetState(pos, prevByte)].DecodeNormal(rangeDecoder);
			}

			// Token: 0x06000083 RID: 131 RVA: 0x0000619C File Offset: 0x0000439C
			public byte DecodeWithMatchByte(Decoder rangeDecoder, uint pos, byte prevByte, byte matchByte)
			{
				return this.m_Coders[(int)this.GetState(pos, prevByte)].DecodeWithMatchByte(rangeDecoder, matchByte);
			}

			// Token: 0x06000084 RID: 132 RVA: 0x00002194 File Offset: 0x00000394
			public LiteralDecoder()
			{
			}

			// Token: 0x04000085 RID: 133
			private Decoder.LiteralDecoder.Decoder2[] m_Coders;

			// Token: 0x04000086 RID: 134
			private int m_NumPosBits;

			// Token: 0x04000087 RID: 135
			private int m_NumPrevBits;

			// Token: 0x04000088 RID: 136
			private uint m_PosMask;

			// Token: 0x0200001B RID: 27
			private struct Decoder2
			{
				// Token: 0x06000085 RID: 133 RVA: 0x00002381 File Offset: 0x00000581
				public void Create()
				{
					this.m_Decoders = new BitDecoder[768];
				}

				// Token: 0x06000086 RID: 134 RVA: 0x000061CC File Offset: 0x000043CC
				public void Init()
				{
					for (int i = 0; i < 768; i++)
					{
						this.m_Decoders[i].Init();
					}
				}

				// Token: 0x06000087 RID: 135 RVA: 0x00006200 File Offset: 0x00004400
				public byte DecodeNormal(Decoder rangeDecoder)
				{
					uint symbol = 1U;
					do
					{
						symbol = (symbol << 1 | this.m_Decoders[(int)symbol].Decode(rangeDecoder));
					}
					while (symbol < 256U);
					return (byte)symbol;
				}

				// Token: 0x06000088 RID: 136 RVA: 0x0000623C File Offset: 0x0000443C
				public byte DecodeWithMatchByte(Decoder rangeDecoder, byte matchByte)
				{
					uint symbol = 1U;
					for (;;)
					{
						uint matchBit = (uint)(matchByte >> 7 & 1);
						matchByte = (byte)(matchByte << 1);
						uint bit = this.m_Decoders[(int)((1U + matchBit << 8) + symbol)].Decode(rangeDecoder);
						symbol = (symbol << 1 | bit);
						bool flag = matchBit != bit;
						if (flag)
						{
							break;
						}
						if (symbol >= 256U)
						{
							goto IL_73;
						}
					}
					while (symbol < 256U)
					{
						symbol = (symbol << 1 | this.m_Decoders[(int)symbol].Decode(rangeDecoder));
					}
					IL_73:
					return (byte)symbol;
				}

				// Token: 0x04000089 RID: 137
				private BitDecoder[] m_Decoders;
			}
		}
	}
}
