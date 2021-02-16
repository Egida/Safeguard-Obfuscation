using System;
using System.IO;
using SevenZip.Compression.LZ;
using SevenZip.Compression.RangeCoder;

namespace SevenZip.Compression.LZMA
{
	// Token: 0x0200001C RID: 28
	internal class Encoder : ICoder, ISetCoderProperties, IWriteCoderProperties
	{
		// Token: 0x06000089 RID: 137 RVA: 0x000062C4 File Offset: 0x000044C4
		static Encoder()
		{
			int c = 2;
			Encoder.g_FastPos[0] = 0;
			Encoder.g_FastPos[1] = 1;
			for (byte slotFast = 2; slotFast < 22; slotFast += 1)
			{
				uint i = 1U << (slotFast >> 1) - 1;
				uint j = 0U;
				while (j < i)
				{
					Encoder.g_FastPos[c] = slotFast;
					j += 1U;
					c++;
				}
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00006350 File Offset: 0x00004550
		public Encoder()
		{
			int i = 0;
			while ((long)i < 4096L)
			{
				this._optimum[i] = new Encoder.Optimal();
				i++;
			}
			int j = 0;
			while ((long)j < 4L)
			{
				this._posSlotEncoder[j] = new BitTreeEncoder(6);
				j++;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00006530 File Offset: 0x00004730
		public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
		{
			this._needReleaseMFStream = false;
			try
			{
				this.SetStreams(inStream, outStream, inSize, outSize);
				for (;;)
				{
					long processedInSize;
					long processedOutSize;
					bool finished;
					this.CodeOneBlock(out processedInSize, out processedOutSize, out finished);
					bool flag = finished;
					if (flag)
					{
						break;
					}
					bool flag2 = progress != null;
					if (flag2)
					{
						progress.SetProgress(processedInSize, processedOutSize);
					}
				}
			}
			finally
			{
				this.ReleaseStreams();
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000065A0 File Offset: 0x000047A0
		public void SetCoderProperties(CoderPropID[] propIDs, object[] properties)
		{
			uint i = 0U;
			while ((ulong)i < (ulong)((long)properties.Length))
			{
				object prop = properties[(int)i];
				switch (propIDs[(int)i])
				{
				case CoderPropID.DictionarySize:
				{
					bool flag = !(prop is int);
					if (flag)
					{
						throw new InvalidParamException();
					}
					int dictionarySize = (int)prop;
					bool flag2 = (long)dictionarySize < 1L || (long)dictionarySize > 1073741824L;
					if (flag2)
					{
						throw new InvalidParamException();
					}
					this._dictionarySize = (uint)dictionarySize;
					int dicLogSize = 0;
					while ((long)dicLogSize < 30L)
					{
						bool flag3 = (long)dictionarySize <= (long)(1UL << (dicLogSize & 31));
						if (flag3)
						{
							break;
						}
						dicLogSize++;
					}
					this._distTableSize = (uint)(dicLogSize * 2);
					break;
				}
				case CoderPropID.UsedMemorySize:
				case CoderPropID.Order:
				case CoderPropID.BlockSize:
				case CoderPropID.MatchFinderCycles:
				case CoderPropID.NumPasses:
				case CoderPropID.NumThreads:
					goto IL_2C0;
				case CoderPropID.PosStateBits:
				{
					bool flag4 = !(prop is int);
					if (flag4)
					{
						throw new InvalidParamException();
					}
					int v = (int)prop;
					bool flag5 = v < 0 || (long)v > 4L;
					if (flag5)
					{
						throw new InvalidParamException();
					}
					this._posStateBits = v;
					this._posStateMask = (1U << this._posStateBits) - 1U;
					break;
				}
				case CoderPropID.LitContextBits:
				{
					bool flag6 = !(prop is int);
					if (flag6)
					{
						throw new InvalidParamException();
					}
					int v2 = (int)prop;
					bool flag7 = v2 < 0 || (long)v2 > 8L;
					if (flag7)
					{
						throw new InvalidParamException();
					}
					this._numLiteralContextBits = v2;
					break;
				}
				case CoderPropID.LitPosBits:
				{
					bool flag8 = !(prop is int);
					if (flag8)
					{
						throw new InvalidParamException();
					}
					int v3 = (int)prop;
					bool flag9 = v3 < 0 || (long)v3 > 4L;
					if (flag9)
					{
						throw new InvalidParamException();
					}
					this._numLiteralPosStateBits = v3;
					break;
				}
				case CoderPropID.NumFastBytes:
				{
					bool flag10 = !(prop is int);
					if (flag10)
					{
						throw new InvalidParamException();
					}
					int numFastBytes = (int)prop;
					bool flag11 = numFastBytes < 5 || (long)numFastBytes > 273L;
					if (flag11)
					{
						throw new InvalidParamException();
					}
					this._numFastBytes = (uint)numFastBytes;
					break;
				}
				case CoderPropID.MatchFinder:
				{
					bool flag12 = !(prop is string);
					if (flag12)
					{
						throw new InvalidParamException();
					}
					Encoder.EMatchFinderType matchFinderIndexPrev = this._matchFinderType;
					int j = Encoder.FindMatchFinder(((string)prop).ToUpper());
					bool flag13 = j < 0;
					if (flag13)
					{
						throw new InvalidParamException();
					}
					this._matchFinderType = (Encoder.EMatchFinderType)j;
					bool flag14 = this._matchFinder != null && matchFinderIndexPrev != this._matchFinderType;
					if (flag14)
					{
						this._dictionarySizePrev = uint.MaxValue;
						this._matchFinder = null;
					}
					break;
				}
				case CoderPropID.Algorithm:
					break;
				case CoderPropID.EndMarker:
				{
					bool flag15 = !(prop is bool);
					if (flag15)
					{
						throw new InvalidParamException();
					}
					this.SetWriteEndMarkerMode((bool)prop);
					break;
				}
				default:
					goto IL_2C0;
				}
				i += 1U;
				continue;
				IL_2C0:
				throw new InvalidParamException();
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000688C File Offset: 0x00004A8C
		public void WriteCoderProperties(Stream outStream)
		{
			this.properties[0] = (byte)((this._posStateBits * 5 + this._numLiteralPosStateBits) * 9 + this._numLiteralContextBits);
			for (int i = 0; i < 4; i++)
			{
				this.properties[1 + i] = (byte)(this._dictionarySize >> 8 * i & 255U);
			}
			outStream.Write(this.properties, 0, 5);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000068FC File Offset: 0x00004AFC
		private static uint GetPosSlot(uint pos)
		{
			bool flag = pos < 2048U;
			uint result;
			if (flag)
			{
				result = (uint)Encoder.g_FastPos[(int)pos];
			}
			else
			{
				bool flag2 = pos < 2097152U;
				if (flag2)
				{
					result = (uint)(Encoder.g_FastPos[(int)(pos >> 10)] + 20);
				}
				else
				{
					result = (uint)(Encoder.g_FastPos[(int)(pos >> 20)] + 40);
				}
			}
			return result;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00006950 File Offset: 0x00004B50
		private static uint GetPosSlot2(uint pos)
		{
			bool flag = pos < 131072U;
			uint result;
			if (flag)
			{
				result = (uint)(Encoder.g_FastPos[(int)(pos >> 6)] + 12);
			}
			else
			{
				bool flag2 = pos < 134217728U;
				if (flag2)
				{
					result = (uint)(Encoder.g_FastPos[(int)(pos >> 16)] + 32);
				}
				else
				{
					result = (uint)(Encoder.g_FastPos[(int)(pos >> 26)] + 52);
				}
			}
			return result;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000069A8 File Offset: 0x00004BA8
		private void BaseInit()
		{
			this._state.Init();
			this._previousByte = 0;
			for (uint i = 0U; i < 4U; i += 1U)
			{
				this._repDistances[(int)i] = 0U;
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000069E4 File Offset: 0x00004BE4
		private void Create()
		{
			bool flag = this._matchFinder == null;
			if (flag)
			{
				BinTree bt = new BinTree();
				int numHashBytes = 4;
				bool flag2 = this._matchFinderType == Encoder.EMatchFinderType.BT2;
				if (flag2)
				{
					numHashBytes = 2;
				}
				bt.SetType(numHashBytes);
				this._matchFinder = bt;
			}
			this._literalEncoder.Create(this._numLiteralPosStateBits, this._numLiteralContextBits);
			bool flag3 = this._dictionarySize == this._dictionarySizePrev && this._numFastBytesPrev == this._numFastBytes;
			if (!flag3)
			{
				this._matchFinder.Create(this._dictionarySize, 4096U, this._numFastBytes, 274U);
				this._dictionarySizePrev = this._dictionarySize;
				this._numFastBytesPrev = this._numFastBytes;
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00002394 File Offset: 0x00000594
		private void SetWriteEndMarkerMode(bool writeEndMarker)
		{
			this._writeEndMark = writeEndMarker;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00006AA0 File Offset: 0x00004CA0
		private void Init()
		{
			this.BaseInit();
			this._rangeEncoder.Init();
			for (uint i = 0U; i < 12U; i += 1U)
			{
				for (uint j = 0U; j <= this._posStateMask; j += 1U)
				{
					uint complexState = (i << 4) + j;
					this._isMatch[(int)complexState].Init();
					this._isRep0Long[(int)complexState].Init();
				}
				this._isRep[(int)i].Init();
				this._isRepG0[(int)i].Init();
				this._isRepG1[(int)i].Init();
				this._isRepG2[(int)i].Init();
			}
			this._literalEncoder.Init();
			for (uint i = 0U; i < 4U; i += 1U)
			{
				this._posSlotEncoder[(int)i].Init();
			}
			for (uint i = 0U; i < 114U; i += 1U)
			{
				this._posEncoders[(int)i].Init();
			}
			this._lenEncoder.Init(1U << this._posStateBits);
			this._repMatchLenEncoder.Init(1U << this._posStateBits);
			this._posAlignEncoder.Init();
			this._longestMatchWasFound = false;
			this._optimumEndIndex = 0U;
			this._optimumCurrentIndex = 0U;
			this._additionalOffset = 0U;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00006C14 File Offset: 0x00004E14
		private void ReadMatchDistances(out uint lenRes, out uint numDistancePairs)
		{
			lenRes = 0U;
			numDistancePairs = this._matchFinder.GetMatches(this._matchDistances);
			bool flag = numDistancePairs > 0U;
			if (flag)
			{
				lenRes = this._matchDistances[(int)(numDistancePairs - 2U)];
				bool flag2 = lenRes == this._numFastBytes;
				if (flag2)
				{
					lenRes += this._matchFinder.GetMatchLen((int)(lenRes - 1U), this._matchDistances[(int)(numDistancePairs - 1U)], 273U - lenRes);
				}
			}
			this._additionalOffset += 1U;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00006C94 File Offset: 0x00004E94
		private void MovePos(uint num)
		{
			bool flag = num > 0U;
			if (flag)
			{
				this._matchFinder.Skip(num);
				this._additionalOffset += num;
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00006CC8 File Offset: 0x00004EC8
		private uint GetRepLen1Price(Base.State state, uint posState)
		{
			return this._isRepG0[(int)state.Index].GetPrice0() + this._isRep0Long[(int)((state.Index << 4) + posState)].GetPrice0();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00006D0C File Offset: 0x00004F0C
		private uint GetPureRepPrice(uint repIndex, Base.State state, uint posState)
		{
			bool flag = repIndex == 0U;
			uint price;
			if (flag)
			{
				price = this._isRepG0[(int)state.Index].GetPrice0();
				price += this._isRep0Long[(int)((state.Index << 4) + posState)].GetPrice1();
			}
			else
			{
				price = this._isRepG0[(int)state.Index].GetPrice1();
				bool flag2 = repIndex == 1U;
				if (flag2)
				{
					price += this._isRepG1[(int)state.Index].GetPrice0();
				}
				else
				{
					price += this._isRepG1[(int)state.Index].GetPrice1();
					price += this._isRepG2[(int)state.Index].GetPrice(repIndex - 2U);
				}
			}
			return price;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00006DD4 File Offset: 0x00004FD4
		private uint GetRepPrice(uint repIndex, uint len, Base.State state, uint posState)
		{
			uint price = this._repMatchLenEncoder.GetPrice(len - 2U, posState);
			return price + this.GetPureRepPrice(repIndex, state, posState);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006E04 File Offset: 0x00005004
		private uint GetPosLenPrice(uint pos, uint len, uint posState)
		{
			uint lenToPosState = Base.GetLenToPosState(len);
			bool flag = pos < 128U;
			uint price;
			if (flag)
			{
				price = this._distancesPrices[(int)(lenToPosState * 128U + pos)];
			}
			else
			{
				price = this._posSlotPrices[(int)((lenToPosState << 6) + Encoder.GetPosSlot2(pos))] + this._alignPrices[(int)(pos & 15U)];
			}
			return price + this._lenEncoder.GetPrice(len - 2U, posState);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006E6C File Offset: 0x0000506C
		private uint Backward(out uint backRes, uint cur)
		{
			this._optimumEndIndex = cur;
			uint posMem = this._optimum[(int)cur].PosPrev;
			uint backMem = this._optimum[(int)cur].BackPrev;
			do
			{
				bool prev1IsChar = this._optimum[(int)cur].Prev1IsChar;
				if (prev1IsChar)
				{
					this._optimum[(int)posMem].MakeAsChar();
					this._optimum[(int)posMem].PosPrev = posMem - 1U;
					bool prev = this._optimum[(int)cur].Prev2;
					if (prev)
					{
						this._optimum[(int)(posMem - 1U)].Prev1IsChar = false;
						this._optimum[(int)(posMem - 1U)].PosPrev = this._optimum[(int)cur].PosPrev2;
						this._optimum[(int)(posMem - 1U)].BackPrev = this._optimum[(int)cur].BackPrev2;
					}
				}
				uint posPrev = posMem;
				uint backCur = backMem;
				backMem = this._optimum[(int)posPrev].BackPrev;
				posMem = this._optimum[(int)posPrev].PosPrev;
				this._optimum[(int)posPrev].BackPrev = backCur;
				this._optimum[(int)posPrev].PosPrev = cur;
				cur = posPrev;
			}
			while (cur > 0U);
			backRes = this._optimum[0].BackPrev;
			this._optimumCurrentIndex = this._optimum[0].PosPrev;
			return this._optimumCurrentIndex;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006FAC File Offset: 0x000051AC
		private uint GetOptimum(uint position, out uint backRes)
		{
			bool flag = this._optimumEndIndex != this._optimumCurrentIndex;
			uint result;
			if (flag)
			{
				uint lenRes = this._optimum[(int)this._optimumCurrentIndex].PosPrev - this._optimumCurrentIndex;
				backRes = this._optimum[(int)this._optimumCurrentIndex].BackPrev;
				this._optimumCurrentIndex = this._optimum[(int)this._optimumCurrentIndex].PosPrev;
				result = lenRes;
			}
			else
			{
				this._optimumCurrentIndex = (this._optimumEndIndex = 0U);
				bool flag2 = !this._longestMatchWasFound;
				uint lenMain;
				uint numDistancePairs;
				if (flag2)
				{
					this.ReadMatchDistances(out lenMain, out numDistancePairs);
				}
				else
				{
					lenMain = this._longestMatchLength;
					numDistancePairs = this._numDistancePairs;
					this._longestMatchWasFound = false;
				}
				uint numAvailableBytes = this._matchFinder.GetNumAvailableBytes() + 1U;
				bool flag3 = numAvailableBytes < 2U;
				if (flag3)
				{
					backRes = uint.MaxValue;
					result = 1U;
				}
				else
				{
					bool flag4 = numAvailableBytes > 273U;
					if (flag4)
					{
					}
					uint repMaxIndex = 0U;
					for (uint i = 0U; i < 4U; i += 1U)
					{
						this.reps[(int)i] = this._repDistances[(int)i];
						this.repLens[(int)i] = this._matchFinder.GetMatchLen(-1, this.reps[(int)i], 273U);
						bool flag5 = this.repLens[(int)i] > this.repLens[(int)repMaxIndex];
						if (flag5)
						{
							repMaxIndex = i;
						}
					}
					bool flag6 = this.repLens[(int)repMaxIndex] >= this._numFastBytes;
					if (flag6)
					{
						backRes = repMaxIndex;
						uint lenRes2 = this.repLens[(int)repMaxIndex];
						this.MovePos(lenRes2 - 1U);
						result = lenRes2;
					}
					else
					{
						bool flag7 = lenMain >= this._numFastBytes;
						if (flag7)
						{
							backRes = this._matchDistances[(int)(numDistancePairs - 1U)] + 4U;
							this.MovePos(lenMain - 1U);
							result = lenMain;
						}
						else
						{
							byte currentByte = this._matchFinder.GetIndexByte(-1);
							byte matchByte = this._matchFinder.GetIndexByte((int)(0U - this._repDistances[0] - 1U - 1U));
							bool flag8 = lenMain < 2U && currentByte != matchByte && this.repLens[(int)repMaxIndex] < 2U;
							if (flag8)
							{
								backRes = uint.MaxValue;
								result = 1U;
							}
							else
							{
								this._optimum[0].State = this._state;
								uint posState = position & this._posStateMask;
								this._optimum[1].Price = this._isMatch[(int)((this._state.Index << 4) + posState)].GetPrice0() + this._literalEncoder.GetSubCoder(position, this._previousByte).GetPrice(!this._state.IsCharState(), matchByte, currentByte);
								this._optimum[1].MakeAsChar();
								uint matchPrice = this._isMatch[(int)((this._state.Index << 4) + posState)].GetPrice1();
								uint repMatchPrice = matchPrice + this._isRep[(int)this._state.Index].GetPrice1();
								bool flag9 = matchByte == currentByte;
								if (flag9)
								{
									uint shortRepPrice = repMatchPrice + this.GetRepLen1Price(this._state, posState);
									bool flag10 = shortRepPrice < this._optimum[1].Price;
									if (flag10)
									{
										this._optimum[1].Price = shortRepPrice;
										this._optimum[1].MakeAsShortRep();
									}
								}
								uint lenEnd = (lenMain >= this.repLens[(int)repMaxIndex]) ? lenMain : this.repLens[(int)repMaxIndex];
								bool flag11 = lenEnd < 2U;
								if (flag11)
								{
									backRes = this._optimum[1].BackPrev;
									result = 1U;
								}
								else
								{
									this._optimum[1].PosPrev = 0U;
									this._optimum[0].Backs0 = this.reps[0];
									this._optimum[0].Backs1 = this.reps[1];
									this._optimum[0].Backs2 = this.reps[2];
									this._optimum[0].Backs3 = this.reps[3];
									uint len = lenEnd;
									do
									{
										this._optimum[(int)len--].Price = 268435455U;
									}
									while (len >= 2U);
									for (uint i = 0U; i < 4U; i += 1U)
									{
										uint repLen = this.repLens[(int)i];
										bool flag12 = repLen < 2U;
										if (!flag12)
										{
											uint price = repMatchPrice + this.GetPureRepPrice(i, this._state, posState);
											do
											{
												uint curAndLenPrice = price + this._repMatchLenEncoder.GetPrice(repLen - 2U, posState);
												Encoder.Optimal optimum = this._optimum[(int)repLen];
												bool flag13 = curAndLenPrice < optimum.Price;
												if (flag13)
												{
													optimum.Price = curAndLenPrice;
													optimum.PosPrev = 0U;
													optimum.BackPrev = i;
													optimum.Prev1IsChar = false;
												}
											}
											while ((repLen -= 1U) >= 2U);
										}
									}
									uint normalMatchPrice = matchPrice + this._isRep[(int)this._state.Index].GetPrice0();
									len = ((this.repLens[0] >= 2U) ? (this.repLens[0] + 1U) : 2U);
									bool flag14 = len <= lenMain;
									if (flag14)
									{
										uint offs = 0U;
										while (len > this._matchDistances[(int)offs])
										{
											offs += 2U;
										}
										for (;;)
										{
											uint distance = this._matchDistances[(int)(offs + 1U)];
											uint curAndLenPrice2 = normalMatchPrice + this.GetPosLenPrice(distance, len, posState);
											Encoder.Optimal optimum2 = this._optimum[(int)len];
											bool flag15 = curAndLenPrice2 < optimum2.Price;
											if (flag15)
											{
												optimum2.Price = curAndLenPrice2;
												optimum2.PosPrev = 0U;
												optimum2.BackPrev = distance + 4U;
												optimum2.Prev1IsChar = false;
											}
											bool flag16 = len == this._matchDistances[(int)offs];
											if (flag16)
											{
												offs += 2U;
												bool flag17 = offs == numDistancePairs;
												if (flag17)
												{
													break;
												}
											}
											len += 1U;
										}
									}
									uint cur = 0U;
									uint newLen;
									for (;;)
									{
										cur += 1U;
										bool flag18 = cur == lenEnd;
										if (flag18)
										{
											break;
										}
										this.ReadMatchDistances(out newLen, out numDistancePairs);
										bool flag19 = newLen >= this._numFastBytes;
										if (flag19)
										{
											goto Block_26;
										}
										position += 1U;
										uint posPrev = this._optimum[(int)cur].PosPrev;
										bool prev1IsChar = this._optimum[(int)cur].Prev1IsChar;
										Base.State state;
										if (prev1IsChar)
										{
											posPrev -= 1U;
											bool prev = this._optimum[(int)cur].Prev2;
											if (prev)
											{
												state = this._optimum[(int)this._optimum[(int)cur].PosPrev2].State;
												bool flag20 = this._optimum[(int)cur].BackPrev2 < 4U;
												if (flag20)
												{
													state.UpdateRep();
												}
												else
												{
													state.UpdateMatch();
												}
											}
											else
											{
												state = this._optimum[(int)posPrev].State;
											}
											state.UpdateChar();
										}
										else
										{
											state = this._optimum[(int)posPrev].State;
										}
										bool flag21 = posPrev == cur - 1U;
										if (flag21)
										{
											bool flag22 = this._optimum[(int)cur].IsShortRep();
											if (flag22)
											{
												state.UpdateShortRep();
											}
											else
											{
												state.UpdateChar();
											}
										}
										else
										{
											bool flag23 = this._optimum[(int)cur].Prev1IsChar && this._optimum[(int)cur].Prev2;
											uint pos;
											if (flag23)
											{
												posPrev = this._optimum[(int)cur].PosPrev2;
												pos = this._optimum[(int)cur].BackPrev2;
												state.UpdateRep();
											}
											else
											{
												pos = this._optimum[(int)cur].BackPrev;
												bool flag24 = pos < 4U;
												if (flag24)
												{
													state.UpdateRep();
												}
												else
												{
													state.UpdateMatch();
												}
											}
											Encoder.Optimal opt = this._optimum[(int)posPrev];
											bool flag25 = pos < 4U;
											if (flag25)
											{
												bool flag26 = pos == 0U;
												if (flag26)
												{
													this.reps[0] = opt.Backs0;
													this.reps[1] = opt.Backs1;
													this.reps[2] = opt.Backs2;
													this.reps[3] = opt.Backs3;
												}
												else
												{
													bool flag27 = pos == 1U;
													if (flag27)
													{
														this.reps[0] = opt.Backs1;
														this.reps[1] = opt.Backs0;
														this.reps[2] = opt.Backs2;
														this.reps[3] = opt.Backs3;
													}
													else
													{
														bool flag28 = pos == 2U;
														if (flag28)
														{
															this.reps[0] = opt.Backs2;
															this.reps[1] = opt.Backs0;
															this.reps[2] = opt.Backs1;
															this.reps[3] = opt.Backs3;
														}
														else
														{
															this.reps[0] = opt.Backs3;
															this.reps[1] = opt.Backs0;
															this.reps[2] = opt.Backs1;
															this.reps[3] = opt.Backs2;
														}
													}
												}
											}
											else
											{
												this.reps[0] = pos - 4U;
												this.reps[1] = opt.Backs0;
												this.reps[2] = opt.Backs1;
												this.reps[3] = opt.Backs2;
											}
										}
										this._optimum[(int)cur].State = state;
										this._optimum[(int)cur].Backs0 = this.reps[0];
										this._optimum[(int)cur].Backs1 = this.reps[1];
										this._optimum[(int)cur].Backs2 = this.reps[2];
										this._optimum[(int)cur].Backs3 = this.reps[3];
										uint curPrice = this._optimum[(int)cur].Price;
										currentByte = this._matchFinder.GetIndexByte(-1);
										matchByte = this._matchFinder.GetIndexByte((int)(0U - this.reps[0] - 1U - 1U));
										posState = (position & this._posStateMask);
										uint curAnd1Price = curPrice + this._isMatch[(int)((state.Index << 4) + posState)].GetPrice0() + this._literalEncoder.GetSubCoder(position, this._matchFinder.GetIndexByte(-2)).GetPrice(!state.IsCharState(), matchByte, currentByte);
										Encoder.Optimal nextOptimum = this._optimum[(int)(cur + 1U)];
										bool nextIsChar = false;
										bool flag29 = curAnd1Price < nextOptimum.Price;
										if (flag29)
										{
											nextOptimum.Price = curAnd1Price;
											nextOptimum.PosPrev = cur;
											nextOptimum.MakeAsChar();
											nextIsChar = true;
										}
										matchPrice = curPrice + this._isMatch[(int)((state.Index << 4) + posState)].GetPrice1();
										repMatchPrice = matchPrice + this._isRep[(int)state.Index].GetPrice1();
										bool flag30 = matchByte == currentByte && (nextOptimum.PosPrev >= cur || nextOptimum.BackPrev > 0U);
										if (flag30)
										{
											uint shortRepPrice2 = repMatchPrice + this.GetRepLen1Price(state, posState);
											bool flag31 = shortRepPrice2 <= nextOptimum.Price;
											if (flag31)
											{
												nextOptimum.Price = shortRepPrice2;
												nextOptimum.PosPrev = cur;
												nextOptimum.MakeAsShortRep();
												nextIsChar = true;
											}
										}
										uint numAvailableBytesFull = this._matchFinder.GetNumAvailableBytes() + 1U;
										numAvailableBytesFull = Math.Min(4095U - cur, numAvailableBytesFull);
										numAvailableBytes = numAvailableBytesFull;
										bool flag32 = numAvailableBytes < 2U;
										if (!flag32)
										{
											bool flag33 = numAvailableBytes > this._numFastBytes;
											if (flag33)
											{
												numAvailableBytes = this._numFastBytes;
											}
											bool flag34 = !nextIsChar && matchByte != currentByte;
											if (flag34)
											{
												uint t = Math.Min(numAvailableBytesFull - 1U, this._numFastBytes);
												uint lenTest2 = this._matchFinder.GetMatchLen(0, this.reps[0], t);
												bool flag35 = lenTest2 >= 2U;
												if (flag35)
												{
													Base.State state2 = state;
													state2.UpdateChar();
													uint posStateNext = position + 1U & this._posStateMask;
													uint nextRepMatchPrice = curAnd1Price + this._isMatch[(int)((state2.Index << 4) + posStateNext)].GetPrice1() + this._isRep[(int)state2.Index].GetPrice1();
													uint offset = cur + 1U + lenTest2;
													while (lenEnd < offset)
													{
														this._optimum[(int)(lenEnd += 1U)].Price = 268435455U;
													}
													uint curAndLenPrice3 = nextRepMatchPrice + this.GetRepPrice(0U, lenTest2, state2, posStateNext);
													Encoder.Optimal optimum3 = this._optimum[(int)offset];
													bool flag36 = curAndLenPrice3 < optimum3.Price;
													if (flag36)
													{
														optimum3.Price = curAndLenPrice3;
														optimum3.PosPrev = cur + 1U;
														optimum3.BackPrev = 0U;
														optimum3.Prev1IsChar = true;
														optimum3.Prev2 = false;
													}
												}
											}
											uint startLen = 2U;
											for (uint repIndex = 0U; repIndex < 4U; repIndex += 1U)
											{
												uint lenTest3 = this._matchFinder.GetMatchLen(-1, this.reps[(int)repIndex], numAvailableBytes);
												bool flag37 = lenTest3 < 2U;
												if (!flag37)
												{
													uint lenTestTemp = lenTest3;
													do
													{
														while (lenEnd < cur + lenTest3)
														{
															this._optimum[(int)(lenEnd += 1U)].Price = 268435455U;
														}
														uint curAndLenPrice4 = repMatchPrice + this.GetRepPrice(repIndex, lenTest3, state, posState);
														Encoder.Optimal optimum4 = this._optimum[(int)(cur + lenTest3)];
														bool flag38 = curAndLenPrice4 < optimum4.Price;
														if (flag38)
														{
															optimum4.Price = curAndLenPrice4;
															optimum4.PosPrev = cur;
															optimum4.BackPrev = repIndex;
															optimum4.Prev1IsChar = false;
														}
													}
													while ((lenTest3 -= 1U) >= 2U);
													lenTest3 = lenTestTemp;
													bool flag39 = repIndex == 0U;
													if (flag39)
													{
														startLen = lenTest3 + 1U;
													}
													bool flag40 = lenTest3 < numAvailableBytesFull;
													if (flag40)
													{
														uint t2 = Math.Min(numAvailableBytesFull - 1U - lenTest3, this._numFastBytes);
														uint lenTest4 = this._matchFinder.GetMatchLen((int)lenTest3, this.reps[(int)repIndex], t2);
														bool flag41 = lenTest4 >= 2U;
														if (flag41)
														{
															Base.State state3 = state;
															state3.UpdateRep();
															uint posStateNext2 = position + lenTest3 & this._posStateMask;
															uint curAndLenCharPrice = repMatchPrice + this.GetRepPrice(repIndex, lenTest3, state, posState) + this._isMatch[(int)((state3.Index << 4) + posStateNext2)].GetPrice0() + this._literalEncoder.GetSubCoder(position + lenTest3, this._matchFinder.GetIndexByte((int)(lenTest3 - 1U - 1U))).GetPrice(true, this._matchFinder.GetIndexByte((int)(lenTest3 - 1U - (this.reps[(int)repIndex] + 1U))), this._matchFinder.GetIndexByte((int)(lenTest3 - 1U)));
															state3.UpdateChar();
															posStateNext2 = (position + lenTest3 + 1U & this._posStateMask);
															uint nextMatchPrice = curAndLenCharPrice + this._isMatch[(int)((state3.Index << 4) + posStateNext2)].GetPrice1();
															uint nextRepMatchPrice2 = nextMatchPrice + this._isRep[(int)state3.Index].GetPrice1();
															uint offset2 = lenTest3 + 1U + lenTest4;
															while (lenEnd < cur + offset2)
															{
																this._optimum[(int)(lenEnd += 1U)].Price = 268435455U;
															}
															uint curAndLenPrice5 = nextRepMatchPrice2 + this.GetRepPrice(0U, lenTest4, state3, posStateNext2);
															Encoder.Optimal optimum5 = this._optimum[(int)(cur + offset2)];
															bool flag42 = curAndLenPrice5 < optimum5.Price;
															if (flag42)
															{
																optimum5.Price = curAndLenPrice5;
																optimum5.PosPrev = cur + lenTest3 + 1U;
																optimum5.BackPrev = 0U;
																optimum5.Prev1IsChar = true;
																optimum5.Prev2 = true;
																optimum5.PosPrev2 = cur;
																optimum5.BackPrev2 = repIndex;
															}
														}
													}
												}
											}
											bool flag43 = newLen > numAvailableBytes;
											if (flag43)
											{
												newLen = numAvailableBytes;
												numDistancePairs = 0U;
												while (newLen > this._matchDistances[(int)numDistancePairs])
												{
													numDistancePairs += 2U;
												}
												this._matchDistances[(int)numDistancePairs] = newLen;
												numDistancePairs += 2U;
											}
											bool flag44 = newLen >= startLen;
											if (flag44)
											{
												normalMatchPrice = matchPrice + this._isRep[(int)state.Index].GetPrice0();
												while (lenEnd < cur + newLen)
												{
													this._optimum[(int)(lenEnd += 1U)].Price = 268435455U;
												}
												uint offs2 = 0U;
												while (startLen > this._matchDistances[(int)offs2])
												{
													offs2 += 2U;
												}
												uint lenTest5 = startLen;
												for (;;)
												{
													uint curBack = this._matchDistances[(int)(offs2 + 1U)];
													uint curAndLenPrice6 = normalMatchPrice + this.GetPosLenPrice(curBack, lenTest5, posState);
													Encoder.Optimal optimum6 = this._optimum[(int)(cur + lenTest5)];
													bool flag45 = curAndLenPrice6 < optimum6.Price;
													if (flag45)
													{
														optimum6.Price = curAndLenPrice6;
														optimum6.PosPrev = cur;
														optimum6.BackPrev = curBack + 4U;
														optimum6.Prev1IsChar = false;
													}
													bool flag46 = lenTest5 == this._matchDistances[(int)offs2];
													if (flag46)
													{
														bool flag47 = lenTest5 < numAvailableBytesFull;
														if (flag47)
														{
															uint t3 = Math.Min(numAvailableBytesFull - 1U - lenTest5, this._numFastBytes);
															uint lenTest6 = this._matchFinder.GetMatchLen((int)lenTest5, curBack, t3);
															bool flag48 = lenTest6 >= 2U;
															if (flag48)
															{
																Base.State state4 = state;
																state4.UpdateMatch();
																uint posStateNext3 = position + lenTest5 & this._posStateMask;
																uint curAndLenCharPrice2 = curAndLenPrice6 + this._isMatch[(int)((state4.Index << 4) + posStateNext3)].GetPrice0() + this._literalEncoder.GetSubCoder(position + lenTest5, this._matchFinder.GetIndexByte((int)(lenTest5 - 1U - 1U))).GetPrice(true, this._matchFinder.GetIndexByte((int)(lenTest5 - (curBack + 1U) - 1U)), this._matchFinder.GetIndexByte((int)(lenTest5 - 1U)));
																state4.UpdateChar();
																posStateNext3 = (position + lenTest5 + 1U & this._posStateMask);
																uint nextMatchPrice2 = curAndLenCharPrice2 + this._isMatch[(int)((state4.Index << 4) + posStateNext3)].GetPrice1();
																uint nextRepMatchPrice3 = nextMatchPrice2 + this._isRep[(int)state4.Index].GetPrice1();
																uint offset3 = lenTest5 + 1U + lenTest6;
																while (lenEnd < cur + offset3)
																{
																	this._optimum[(int)(lenEnd += 1U)].Price = 268435455U;
																}
																curAndLenPrice6 = nextRepMatchPrice3 + this.GetRepPrice(0U, lenTest6, state4, posStateNext3);
																optimum6 = this._optimum[(int)(cur + offset3)];
																bool flag49 = curAndLenPrice6 < optimum6.Price;
																if (flag49)
																{
																	optimum6.Price = curAndLenPrice6;
																	optimum6.PosPrev = cur + lenTest5 + 1U;
																	optimum6.BackPrev = 0U;
																	optimum6.Prev1IsChar = true;
																	optimum6.Prev2 = true;
																	optimum6.PosPrev2 = cur;
																	optimum6.BackPrev2 = curBack + 4U;
																}
															}
														}
														offs2 += 2U;
														bool flag50 = offs2 == numDistancePairs;
														if (flag50)
														{
															break;
														}
													}
													lenTest5 += 1U;
												}
											}
										}
									}
									return this.Backward(out backRes, cur);
									Block_26:
									this._numDistancePairs = numDistancePairs;
									this._longestMatchLength = newLen;
									this._longestMatchWasFound = true;
									result = this.Backward(out backRes, cur);
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000824C File Offset: 0x0000644C
		private bool ChangePair(uint smallDist, uint bigDist)
		{
			return smallDist < 33554432U && bigDist >= smallDist << 7;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00008274 File Offset: 0x00006474
		private void WriteEndMarker(uint posState)
		{
			bool flag = !this._writeEndMark;
			if (!flag)
			{
				this._isMatch[(int)((this._state.Index << 4) + posState)].Encode(this._rangeEncoder, 1U);
				this._isRep[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
				this._state.UpdateMatch();
				uint len = 2U;
				this._lenEncoder.Encode(this._rangeEncoder, len - 2U, posState);
				uint posSlot = 63U;
				uint lenToPosState = Base.GetLenToPosState(len);
				this._posSlotEncoder[(int)lenToPosState].Encode(this._rangeEncoder, posSlot);
				int footerBits = 30;
				uint posReduced = (1U << footerBits) - 1U;
				this._rangeEncoder.EncodeDirectBits(posReduced >> 4, footerBits - 4);
				this._posAlignEncoder.ReverseEncode(this._rangeEncoder, posReduced & 15U);
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000239E File Offset: 0x0000059E
		private void Flush(uint nowPos)
		{
			this.ReleaseMFStream();
			this.WriteEndMarker(nowPos & this._posStateMask);
			this._rangeEncoder.FlushData();
			this._rangeEncoder.FlushStream();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00008360 File Offset: 0x00006560
		public void CodeOneBlock(out long inSize, out long outSize, out bool finished)
		{
			inSize = 0L;
			outSize = 0L;
			finished = true;
			bool flag = this._inStream != null;
			if (flag)
			{
				this._matchFinder.SetStream(this._inStream);
				this._matchFinder.Init();
				this._needReleaseMFStream = true;
				this._inStream = null;
				bool flag2 = this._trainSize > 0U;
				if (flag2)
				{
					this._matchFinder.Skip(this._trainSize);
				}
			}
			bool finished2 = this._finished;
			if (!finished2)
			{
				this._finished = true;
				long progressPosValuePrev = this.nowPos64;
				bool flag3 = this.nowPos64 == 0L;
				if (flag3)
				{
					bool flag4 = this._matchFinder.GetNumAvailableBytes() == 0U;
					if (flag4)
					{
						this.Flush((uint)this.nowPos64);
						return;
					}
					uint len;
					uint numDistancePairs;
					this.ReadMatchDistances(out len, out numDistancePairs);
					uint posState = (uint)this.nowPos64 & this._posStateMask;
					this._isMatch[(int)((this._state.Index << 4) + posState)].Encode(this._rangeEncoder, 0U);
					this._state.UpdateChar();
					byte curByte = this._matchFinder.GetIndexByte((int)(0U - this._additionalOffset));
					this._literalEncoder.GetSubCoder((uint)this.nowPos64, this._previousByte).Encode(this._rangeEncoder, curByte);
					this._previousByte = curByte;
					this._additionalOffset -= 1U;
					this.nowPos64 += 1L;
				}
				bool flag5 = this._matchFinder.GetNumAvailableBytes() == 0U;
				if (flag5)
				{
					this.Flush((uint)this.nowPos64);
				}
				else
				{
					for (;;)
					{
						uint pos;
						uint len2 = this.GetOptimum((uint)this.nowPos64, out pos);
						uint posState2 = (uint)this.nowPos64 & this._posStateMask;
						uint complexState = (this._state.Index << 4) + posState2;
						bool flag6 = len2 == 1U && pos == uint.MaxValue;
						if (flag6)
						{
							this._isMatch[(int)complexState].Encode(this._rangeEncoder, 0U);
							byte curByte2 = this._matchFinder.GetIndexByte((int)(0U - this._additionalOffset));
							Encoder.LiteralEncoder.Encoder2 subCoder = this._literalEncoder.GetSubCoder((uint)this.nowPos64, this._previousByte);
							bool flag7 = !this._state.IsCharState();
							if (flag7)
							{
								byte matchByte = this._matchFinder.GetIndexByte((int)(0U - this._repDistances[0] - 1U - this._additionalOffset));
								subCoder.EncodeMatched(this._rangeEncoder, matchByte, curByte2);
							}
							else
							{
								subCoder.Encode(this._rangeEncoder, curByte2);
							}
							this._previousByte = curByte2;
							this._state.UpdateChar();
						}
						else
						{
							this._isMatch[(int)complexState].Encode(this._rangeEncoder, 1U);
							bool flag8 = pos < 4U;
							if (flag8)
							{
								this._isRep[(int)this._state.Index].Encode(this._rangeEncoder, 1U);
								bool flag9 = pos == 0U;
								if (flag9)
								{
									this._isRepG0[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
									bool flag10 = len2 == 1U;
									if (flag10)
									{
										this._isRep0Long[(int)complexState].Encode(this._rangeEncoder, 0U);
									}
									else
									{
										this._isRep0Long[(int)complexState].Encode(this._rangeEncoder, 1U);
									}
								}
								else
								{
									this._isRepG0[(int)this._state.Index].Encode(this._rangeEncoder, 1U);
									bool flag11 = pos == 1U;
									if (flag11)
									{
										this._isRepG1[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
									}
									else
									{
										this._isRepG1[(int)this._state.Index].Encode(this._rangeEncoder, 1U);
										this._isRepG2[(int)this._state.Index].Encode(this._rangeEncoder, pos - 2U);
									}
								}
								bool flag12 = len2 == 1U;
								if (flag12)
								{
									this._state.UpdateShortRep();
								}
								else
								{
									this._repMatchLenEncoder.Encode(this._rangeEncoder, len2 - 2U, posState2);
									this._state.UpdateRep();
								}
								uint distance = this._repDistances[(int)pos];
								bool flag13 = pos > 0U;
								if (flag13)
								{
									for (uint i = pos; i >= 1U; i -= 1U)
									{
										this._repDistances[(int)i] = this._repDistances[(int)(i - 1U)];
									}
									this._repDistances[0] = distance;
								}
							}
							else
							{
								this._isRep[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
								this._state.UpdateMatch();
								this._lenEncoder.Encode(this._rangeEncoder, len2 - 2U, posState2);
								pos -= 4U;
								uint posSlot = Encoder.GetPosSlot(pos);
								uint lenToPosState = Base.GetLenToPosState(len2);
								this._posSlotEncoder[(int)lenToPosState].Encode(this._rangeEncoder, posSlot);
								bool flag14 = posSlot >= 4U;
								if (flag14)
								{
									int footerBits = (int)((posSlot >> 1) - 1U);
									uint baseVal = (2U | (posSlot & 1U)) << footerBits;
									uint posReduced = pos - baseVal;
									bool flag15 = posSlot < 14U;
									if (flag15)
									{
										BitTreeEncoder.ReverseEncode(this._posEncoders, baseVal - posSlot - 1U, this._rangeEncoder, footerBits, posReduced);
									}
									else
									{
										this._rangeEncoder.EncodeDirectBits(posReduced >> 4, footerBits - 4);
										this._posAlignEncoder.ReverseEncode(this._rangeEncoder, posReduced & 15U);
										this._alignPriceCount += 1U;
									}
								}
								uint distance2 = pos;
								for (uint j = 3U; j >= 1U; j -= 1U)
								{
									this._repDistances[(int)j] = this._repDistances[(int)(j - 1U)];
								}
								this._repDistances[0] = distance2;
								this._matchPriceCount += 1U;
							}
							this._previousByte = this._matchFinder.GetIndexByte((int)(len2 - 1U - this._additionalOffset));
						}
						this._additionalOffset -= len2;
						this.nowPos64 += (long)((ulong)len2);
						bool flag16 = this._additionalOffset == 0U;
						if (flag16)
						{
							bool flag17 = this._matchPriceCount >= 128U;
							if (flag17)
							{
								this.FillDistancesPrices();
							}
							bool flag18 = this._alignPriceCount >= 16U;
							if (flag18)
							{
								this.FillAlignPrices();
							}
							inSize = this.nowPos64;
							outSize = this._rangeEncoder.GetProcessedSizeAdd();
							bool flag19 = this._matchFinder.GetNumAvailableBytes() == 0U;
							if (flag19)
							{
								break;
							}
							bool flag20 = this.nowPos64 - progressPosValuePrev >= 4096L;
							if (flag20)
							{
								goto Block_24;
							}
						}
					}
					this.Flush((uint)this.nowPos64);
					return;
					Block_24:
					this._finished = false;
					finished = false;
				}
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00008A58 File Offset: 0x00006C58
		private void ReleaseMFStream()
		{
			bool flag = this._matchFinder != null && this._needReleaseMFStream;
			if (flag)
			{
				this._matchFinder.ReleaseStream();
				this._needReleaseMFStream = false;
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000023CF File Offset: 0x000005CF
		private void SetOutStream(Stream outStream)
		{
			this._rangeEncoder.SetStream(outStream);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000023DF File Offset: 0x000005DF
		private void ReleaseOutStream()
		{
			this._rangeEncoder.ReleaseStream();
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000023EE File Offset: 0x000005EE
		private void ReleaseStreams()
		{
			this.ReleaseMFStream();
			this.ReleaseOutStream();
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00008A90 File Offset: 0x00006C90
		private void SetStreams(Stream inStream, Stream outStream, long inSize, long outSize)
		{
			this._inStream = inStream;
			this._finished = false;
			this.Create();
			this.SetOutStream(outStream);
			this.Init();
			this.FillDistancesPrices();
			this.FillAlignPrices();
			this._lenEncoder.SetTableSize(this._numFastBytes + 1U - 2U);
			this._lenEncoder.UpdateTables(1U << this._posStateBits);
			this._repMatchLenEncoder.SetTableSize(this._numFastBytes + 1U - 2U);
			this._repMatchLenEncoder.UpdateTables(1U << this._posStateBits);
			this.nowPos64 = 0L;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00008B34 File Offset: 0x00006D34
		private void FillDistancesPrices()
		{
			for (uint i = 4U; i < 128U; i += 1U)
			{
				uint posSlot = Encoder.GetPosSlot(i);
				int footerBits = (int)((posSlot >> 1) - 1U);
				uint baseVal = (2U | (posSlot & 1U)) << footerBits;
				this.tempPrices[(int)i] = BitTreeEncoder.ReverseGetPrice(this._posEncoders, baseVal - posSlot - 1U, footerBits, i - baseVal);
			}
			for (uint lenToPosState = 0U; lenToPosState < 4U; lenToPosState += 1U)
			{
				BitTreeEncoder encoder = this._posSlotEncoder[(int)lenToPosState];
				uint st = lenToPosState << 6;
				for (uint posSlot2 = 0U; posSlot2 < this._distTableSize; posSlot2 += 1U)
				{
					this._posSlotPrices[(int)(st + posSlot2)] = encoder.GetPrice(posSlot2);
				}
				for (uint posSlot2 = 14U; posSlot2 < this._distTableSize; posSlot2 += 1U)
				{
					this._posSlotPrices[(int)(st + posSlot2)] += (posSlot2 >> 1) - 1U - 4U << 6;
				}
				uint st2 = lenToPosState * 128U;
				uint j;
				for (j = 0U; j < 4U; j += 1U)
				{
					this._distancesPrices[(int)(st2 + j)] = this._posSlotPrices[(int)(st + j)];
				}
				while (j < 128U)
				{
					this._distancesPrices[(int)(st2 + j)] = this._posSlotPrices[(int)(st + Encoder.GetPosSlot(j))] + this.tempPrices[(int)j];
					j += 1U;
				}
			}
			this._matchPriceCount = 0U;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00008CA8 File Offset: 0x00006EA8
		private void FillAlignPrices()
		{
			for (uint i = 0U; i < 16U; i += 1U)
			{
				this._alignPrices[(int)i] = this._posAlignEncoder.ReverseGetPrice(i);
			}
			this._alignPriceCount = 0U;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00008CE4 File Offset: 0x00006EE4
		private static int FindMatchFinder(string s)
		{
			for (int i = 0; i < Encoder.kMatchFinderIDs.Length; i++)
			{
				bool flag = s == Encoder.kMatchFinderIDs[i];
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000023FF File Offset: 0x000005FF
		public void SetTrainSize(uint trainSize)
		{
			this._trainSize = trainSize;
		}

		// Token: 0x0400008A RID: 138
		private const uint kIfinityPrice = 268435455U;

		// Token: 0x0400008B RID: 139
		private const int kDefaultDictionaryLogSize = 22;

		// Token: 0x0400008C RID: 140
		private const uint kNumFastBytesDefault = 32U;

		// Token: 0x0400008D RID: 141
		private const uint kNumLenSpecSymbols = 16U;

		// Token: 0x0400008E RID: 142
		private const uint kNumOpts = 4096U;

		// Token: 0x0400008F RID: 143
		private const int kPropSize = 5;

		// Token: 0x04000090 RID: 144
		private static readonly byte[] g_FastPos = new byte[2048];

		// Token: 0x04000091 RID: 145
		private static readonly string[] kMatchFinderIDs = new string[]
		{
			"BT2",
			"BT4"
		};

		// Token: 0x04000092 RID: 146
		private readonly uint[] _alignPrices = new uint[16];

		// Token: 0x04000093 RID: 147
		private readonly uint[] _distancesPrices = new uint[512];

		// Token: 0x04000094 RID: 148
		private readonly BitEncoder[] _isMatch = new BitEncoder[192];

		// Token: 0x04000095 RID: 149
		private readonly BitEncoder[] _isRep = new BitEncoder[12];

		// Token: 0x04000096 RID: 150
		private readonly BitEncoder[] _isRep0Long = new BitEncoder[192];

		// Token: 0x04000097 RID: 151
		private readonly BitEncoder[] _isRepG0 = new BitEncoder[12];

		// Token: 0x04000098 RID: 152
		private readonly BitEncoder[] _isRepG1 = new BitEncoder[12];

		// Token: 0x04000099 RID: 153
		private readonly BitEncoder[] _isRepG2 = new BitEncoder[12];

		// Token: 0x0400009A RID: 154
		private readonly Encoder.LenPriceTableEncoder _lenEncoder = new Encoder.LenPriceTableEncoder();

		// Token: 0x0400009B RID: 155
		private readonly Encoder.LiteralEncoder _literalEncoder = new Encoder.LiteralEncoder();

		// Token: 0x0400009C RID: 156
		private readonly uint[] _matchDistances = new uint[548];

		// Token: 0x0400009D RID: 157
		private readonly Encoder.Optimal[] _optimum = new Encoder.Optimal[4096];

		// Token: 0x0400009E RID: 158
		private readonly BitEncoder[] _posEncoders = new BitEncoder[114];

		// Token: 0x0400009F RID: 159
		private readonly BitTreeEncoder[] _posSlotEncoder = new BitTreeEncoder[4];

		// Token: 0x040000A0 RID: 160
		private readonly uint[] _posSlotPrices = new uint[256];

		// Token: 0x040000A1 RID: 161
		private readonly Encoder _rangeEncoder = new Encoder();

		// Token: 0x040000A2 RID: 162
		private readonly uint[] _repDistances = new uint[4];

		// Token: 0x040000A3 RID: 163
		private readonly Encoder.LenPriceTableEncoder _repMatchLenEncoder = new Encoder.LenPriceTableEncoder();

		// Token: 0x040000A4 RID: 164
		private readonly byte[] properties = new byte[5];

		// Token: 0x040000A5 RID: 165
		private readonly uint[] repLens = new uint[4];

		// Token: 0x040000A6 RID: 166
		private readonly uint[] reps = new uint[4];

		// Token: 0x040000A7 RID: 167
		private readonly uint[] tempPrices = new uint[128];

		// Token: 0x040000A8 RID: 168
		private uint _additionalOffset;

		// Token: 0x040000A9 RID: 169
		private uint _alignPriceCount;

		// Token: 0x040000AA RID: 170
		private uint _dictionarySize = 4194304U;

		// Token: 0x040000AB RID: 171
		private uint _dictionarySizePrev = uint.MaxValue;

		// Token: 0x040000AC RID: 172
		private uint _distTableSize = 44U;

		// Token: 0x040000AD RID: 173
		private bool _finished;

		// Token: 0x040000AE RID: 174
		private Stream _inStream;

		// Token: 0x040000AF RID: 175
		private uint _longestMatchLength;

		// Token: 0x040000B0 RID: 176
		private bool _longestMatchWasFound;

		// Token: 0x040000B1 RID: 177
		private IMatchFinder _matchFinder;

		// Token: 0x040000B2 RID: 178
		private Encoder.EMatchFinderType _matchFinderType = Encoder.EMatchFinderType.BT4;

		// Token: 0x040000B3 RID: 179
		private uint _matchPriceCount;

		// Token: 0x040000B4 RID: 180
		private bool _needReleaseMFStream;

		// Token: 0x040000B5 RID: 181
		private uint _numDistancePairs;

		// Token: 0x040000B6 RID: 182
		private uint _numFastBytes = 32U;

		// Token: 0x040000B7 RID: 183
		private uint _numFastBytesPrev = uint.MaxValue;

		// Token: 0x040000B8 RID: 184
		private int _numLiteralContextBits = 3;

		// Token: 0x040000B9 RID: 185
		private int _numLiteralPosStateBits;

		// Token: 0x040000BA RID: 186
		private uint _optimumCurrentIndex;

		// Token: 0x040000BB RID: 187
		private uint _optimumEndIndex;

		// Token: 0x040000BC RID: 188
		private BitTreeEncoder _posAlignEncoder = new BitTreeEncoder(4);

		// Token: 0x040000BD RID: 189
		private int _posStateBits = 2;

		// Token: 0x040000BE RID: 190
		private uint _posStateMask = 3U;

		// Token: 0x040000BF RID: 191
		private byte _previousByte;

		// Token: 0x040000C0 RID: 192
		private Base.State _state = default(Base.State);

		// Token: 0x040000C1 RID: 193
		private uint _trainSize;

		// Token: 0x040000C2 RID: 194
		private bool _writeEndMark;

		// Token: 0x040000C3 RID: 195
		private long nowPos64;

		// Token: 0x0200001D RID: 29
		private enum EMatchFinderType
		{
			// Token: 0x040000C5 RID: 197
			BT2,
			// Token: 0x040000C6 RID: 198
			BT4
		}

		// Token: 0x0200001E RID: 30
		private class LenEncoder
		{
			// Token: 0x060000A9 RID: 169 RVA: 0x00008D24 File Offset: 0x00006F24
			public LenEncoder()
			{
				for (uint posState = 0U; posState < 16U; posState += 1U)
				{
					this._lowCoder[(int)posState] = new BitTreeEncoder(3);
					this._midCoder[(int)posState] = new BitTreeEncoder(3);
				}
			}

			// Token: 0x060000AA RID: 170 RVA: 0x00008DB0 File Offset: 0x00006FB0
			public void Init(uint numPosStates)
			{
				this._choice.Init();
				this._choice2.Init();
				for (uint posState = 0U; posState < numPosStates; posState += 1U)
				{
					this._lowCoder[(int)posState].Init();
					this._midCoder[(int)posState].Init();
				}
				this._highCoder.Init();
			}

			// Token: 0x060000AB RID: 171 RVA: 0x00008E18 File Offset: 0x00007018
			public void Encode(Encoder rangeEncoder, uint symbol, uint posState)
			{
				bool flag = symbol < 8U;
				if (flag)
				{
					this._choice.Encode(rangeEncoder, 0U);
					this._lowCoder[(int)posState].Encode(rangeEncoder, symbol);
				}
				else
				{
					symbol -= 8U;
					this._choice.Encode(rangeEncoder, 1U);
					bool flag2 = symbol < 8U;
					if (flag2)
					{
						this._choice2.Encode(rangeEncoder, 0U);
						this._midCoder[(int)posState].Encode(rangeEncoder, symbol);
					}
					else
					{
						this._choice2.Encode(rangeEncoder, 1U);
						this._highCoder.Encode(rangeEncoder, symbol - 8U);
					}
				}
			}

			// Token: 0x060000AC RID: 172 RVA: 0x00008EB8 File Offset: 0x000070B8
			public void SetPrices(uint posState, uint numSymbols, uint[] prices, uint st)
			{
				uint a0 = this._choice.GetPrice0();
				uint a = this._choice.GetPrice1();
				uint b0 = a + this._choice2.GetPrice0();
				uint b = a + this._choice2.GetPrice1();
				uint i;
				for (i = 0U; i < 8U; i += 1U)
				{
					bool flag = i >= numSymbols;
					if (flag)
					{
						return;
					}
					prices[(int)(st + i)] = a0 + this._lowCoder[(int)posState].GetPrice(i);
				}
				while (i < 16U)
				{
					bool flag2 = i >= numSymbols;
					if (flag2)
					{
						return;
					}
					prices[(int)(st + i)] = b0 + this._midCoder[(int)posState].GetPrice(i - 8U);
					i += 1U;
				}
				while (i < numSymbols)
				{
					prices[(int)(st + i)] = b + this._highCoder.GetPrice(i - 8U - 8U);
					i += 1U;
				}
			}

			// Token: 0x040000C7 RID: 199
			private readonly BitTreeEncoder[] _lowCoder = new BitTreeEncoder[16];

			// Token: 0x040000C8 RID: 200
			private readonly BitTreeEncoder[] _midCoder = new BitTreeEncoder[16];

			// Token: 0x040000C9 RID: 201
			private BitEncoder _choice = default(BitEncoder);

			// Token: 0x040000CA RID: 202
			private BitEncoder _choice2 = default(BitEncoder);

			// Token: 0x040000CB RID: 203
			private BitTreeEncoder _highCoder = new BitTreeEncoder(8);
		}

		// Token: 0x0200001F RID: 31
		private class LenPriceTableEncoder : Encoder.LenEncoder
		{
			// Token: 0x060000AD RID: 173 RVA: 0x00002409 File Offset: 0x00000609
			public void SetTableSize(uint tableSize)
			{
				this._tableSize = tableSize;
			}

			// Token: 0x060000AE RID: 174 RVA: 0x00008FB8 File Offset: 0x000071B8
			public uint GetPrice(uint symbol, uint posState)
			{
				return this._prices[(int)(posState * 272U + symbol)];
			}

			// Token: 0x060000AF RID: 175 RVA: 0x00002413 File Offset: 0x00000613
			private void UpdateTable(uint posState)
			{
				base.SetPrices(posState, this._tableSize, this._prices, posState * 272U);
				this._counters[(int)posState] = this._tableSize;
			}

			// Token: 0x060000B0 RID: 176 RVA: 0x00008FDC File Offset: 0x000071DC
			public void UpdateTables(uint numPosStates)
			{
				for (uint posState = 0U; posState < numPosStates; posState += 1U)
				{
					this.UpdateTable(posState);
				}
			}

			// Token: 0x060000B1 RID: 177 RVA: 0x00009004 File Offset: 0x00007204
			public new void Encode(Encoder rangeEncoder, uint symbol, uint posState)
			{
				base.Encode(rangeEncoder, symbol, posState);
				uint[] counters = this._counters;
				uint num = counters[(int)posState] - 1U;
				counters[(int)posState] = num;
				bool flag = num == 0U;
				if (flag)
				{
					this.UpdateTable(posState);
				}
			}

			// Token: 0x060000B2 RID: 178 RVA: 0x0000243F File Offset: 0x0000063F
			public LenPriceTableEncoder()
			{
			}

			// Token: 0x040000CC RID: 204
			private readonly uint[] _counters = new uint[16];

			// Token: 0x040000CD RID: 205
			private readonly uint[] _prices = new uint[4352];

			// Token: 0x040000CE RID: 206
			private uint _tableSize;
		}

		// Token: 0x02000020 RID: 32
		private class LiteralEncoder
		{
			// Token: 0x060000B3 RID: 179 RVA: 0x00009040 File Offset: 0x00007240
			public void Create(int numPosBits, int numPrevBits)
			{
				bool flag = this.m_Coders != null && this.m_NumPrevBits == numPrevBits && this.m_NumPosBits == numPosBits;
				if (!flag)
				{
					this.m_NumPosBits = numPosBits;
					this.m_PosMask = (1U << numPosBits) - 1U;
					this.m_NumPrevBits = numPrevBits;
					uint numStates = 1U << this.m_NumPrevBits + this.m_NumPosBits;
					this.m_Coders = new Encoder.LiteralEncoder.Encoder2[numStates];
					for (uint i = 0U; i < numStates; i += 1U)
					{
						this.m_Coders[(int)i].Create();
					}
				}
			}

			// Token: 0x060000B4 RID: 180 RVA: 0x000090D0 File Offset: 0x000072D0
			public void Init()
			{
				uint numStates = 1U << this.m_NumPrevBits + this.m_NumPosBits;
				for (uint i = 0U; i < numStates; i += 1U)
				{
					this.m_Coders[(int)i].Init();
				}
			}

			// Token: 0x060000B5 RID: 181 RVA: 0x00009114 File Offset: 0x00007314
			public Encoder.LiteralEncoder.Encoder2 GetSubCoder(uint pos, byte prevByte)
			{
				return this.m_Coders[(int)(((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> 8 - this.m_NumPrevBits))];
			}

			// Token: 0x060000B6 RID: 182 RVA: 0x00002194 File Offset: 0x00000394
			public LiteralEncoder()
			{
			}

			// Token: 0x040000CF RID: 207
			private Encoder.LiteralEncoder.Encoder2[] m_Coders;

			// Token: 0x040000D0 RID: 208
			private int m_NumPosBits;

			// Token: 0x040000D1 RID: 209
			private int m_NumPrevBits;

			// Token: 0x040000D2 RID: 210
			private uint m_PosMask;

			// Token: 0x02000021 RID: 33
			public struct Encoder2
			{
				// Token: 0x060000B7 RID: 183 RVA: 0x00002465 File Offset: 0x00000665
				public void Create()
				{
					this.m_Encoders = new BitEncoder[768];
				}

				// Token: 0x060000B8 RID: 184 RVA: 0x00009154 File Offset: 0x00007354
				public void Init()
				{
					for (int i = 0; i < 768; i++)
					{
						this.m_Encoders[i].Init();
					}
				}

				// Token: 0x060000B9 RID: 185 RVA: 0x00009188 File Offset: 0x00007388
				public void Encode(Encoder rangeEncoder, byte symbol)
				{
					uint context = 1U;
					for (int i = 7; i >= 0; i--)
					{
						uint bit = (uint)(symbol >> i & 1);
						this.m_Encoders[(int)context].Encode(rangeEncoder, bit);
						context = (context << 1 | bit);
					}
				}

				// Token: 0x060000BA RID: 186 RVA: 0x000091D0 File Offset: 0x000073D0
				public void EncodeMatched(Encoder rangeEncoder, byte matchByte, byte symbol)
				{
					uint context = 1U;
					bool same = true;
					for (int i = 7; i >= 0; i--)
					{
						uint bit = (uint)(symbol >> i & 1);
						uint state = context;
						bool flag = same;
						if (flag)
						{
							uint matchBit = (uint)(matchByte >> i & 1);
							state += 1U + matchBit << 8;
							same = (matchBit == bit);
						}
						this.m_Encoders[(int)state].Encode(rangeEncoder, bit);
						context = (context << 1 | bit);
					}
				}

				// Token: 0x060000BB RID: 187 RVA: 0x00009244 File Offset: 0x00007444
				public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
				{
					uint price = 0U;
					uint context = 1U;
					int i = 7;
					if (matchMode)
					{
						while (i >= 0)
						{
							uint matchBit = (uint)(matchByte >> i & 1);
							uint bit = (uint)(symbol >> i & 1);
							price += this.m_Encoders[(int)((1U + matchBit << 8) + context)].GetPrice(bit);
							context = (context << 1 | bit);
							bool flag = matchBit != bit;
							if (flag)
							{
								i--;
								break;
							}
							i--;
						}
					}
					while (i >= 0)
					{
						uint bit2 = (uint)(symbol >> i & 1);
						price += this.m_Encoders[(int)context].GetPrice(bit2);
						context = (context << 1 | bit2);
						i--;
					}
					return price;
				}

				// Token: 0x040000D3 RID: 211
				private BitEncoder[] m_Encoders;
			}
		}

		// Token: 0x02000022 RID: 34
		private class Optimal
		{
			// Token: 0x060000BC RID: 188 RVA: 0x00002478 File Offset: 0x00000678
			public void MakeAsChar()
			{
				this.BackPrev = uint.MaxValue;
				this.Prev1IsChar = false;
			}

			// Token: 0x060000BD RID: 189 RVA: 0x00002489 File Offset: 0x00000689
			public void MakeAsShortRep()
			{
				this.BackPrev = 0U;
				this.Prev1IsChar = false;
			}

			// Token: 0x060000BE RID: 190 RVA: 0x00009308 File Offset: 0x00007508
			public bool IsShortRep()
			{
				return this.BackPrev == 0U;
			}

			// Token: 0x060000BF RID: 191 RVA: 0x00002194 File Offset: 0x00000394
			public Optimal()
			{
			}

			// Token: 0x040000D4 RID: 212
			public uint BackPrev;

			// Token: 0x040000D5 RID: 213
			public uint BackPrev2;

			// Token: 0x040000D6 RID: 214
			public uint Backs0;

			// Token: 0x040000D7 RID: 215
			public uint Backs1;

			// Token: 0x040000D8 RID: 216
			public uint Backs2;

			// Token: 0x040000D9 RID: 217
			public uint Backs3;

			// Token: 0x040000DA RID: 218
			public uint PosPrev;

			// Token: 0x040000DB RID: 219
			public uint PosPrev2;

			// Token: 0x040000DC RID: 220
			public bool Prev1IsChar;

			// Token: 0x040000DD RID: 221
			public bool Prev2;

			// Token: 0x040000DE RID: 222
			public uint Price;

			// Token: 0x040000DF RID: 223
			public Base.State State;
		}
	}
}
