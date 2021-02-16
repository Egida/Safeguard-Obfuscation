using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000F1 RID: 241
	internal class SwitchMangler : ManglerBase
	{
		// Token: 0x060003B6 RID: 950 RVA: 0x0001E11C File Offset: 0x0001C31C
		private LinkedList<Instruction[]> SpiltStatements(InstrBlock block, SwitchMangler.Trace trace, CFContext ctx)
		{
			LinkedList<Instruction[]> statements = new LinkedList<Instruction[]>();
			List<Instruction> currentStatement = new List<Instruction>();
			HashSet<Instruction> requiredInstr = new HashSet<Instruction>();
			for (int i = 0; i < block.Instructions.Count; i++)
			{
				Instruction instr = block.Instructions[i];
				currentStatement.Add(instr);
				bool shouldSpilt = i + 1 < block.Instructions.Count && trace.HasMultipleSources(block.Instructions[i + 1].Offset);
				FlowControl flowControl = instr.OpCode.FlowControl;
				if (flowControl == FlowControl.Branch || flowControl == FlowControl.Cond_Branch || flowControl - FlowControl.Return <= 1)
				{
					shouldSpilt = true;
					bool flag = trace.AfterStack[instr.Offset] != 0;
					if (flag)
					{
						bool flag2 = instr.Operand is Instruction;
						if (flag2)
						{
							requiredInstr.Add((Instruction)instr.Operand);
						}
						else
						{
							bool flag3 = instr.Operand is Instruction[];
							if (flag3)
							{
								foreach (Instruction target in (Instruction[])instr.Operand)
								{
									requiredInstr.Add(target);
								}
							}
						}
					}
				}
				requiredInstr.Remove(instr);
				bool flag4 = instr.OpCode.OpCodeType != OpCodeType.Prefix && trace.AfterStack[instr.Offset] == 0 && requiredInstr.Count == 0 && (shouldSpilt || ctx.Intensity > ctx.Random.NextDouble());
				if (flag4)
				{
					statements.AddLast(currentStatement.ToArray());
					currentStatement.Clear();
				}
			}
			bool flag5 = currentStatement.Count > 0;
			if (flag5)
			{
				statements.AddLast(currentStatement.ToArray());
			}
			return statements;
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x0001E2F4 File Offset: 0x0001C4F4
		private static OpCode InverseBranch(OpCode opCode)
		{
			OpCode result;
			switch (opCode.Code)
			{
			case Code.Brfalse:
				result = OpCodes.Brtrue;
				break;
			case Code.Brtrue:
				result = OpCodes.Brfalse;
				break;
			case Code.Beq:
				result = OpCodes.Bne_Un;
				break;
			case Code.Bge:
				result = OpCodes.Blt;
				break;
			case Code.Bgt:
				result = OpCodes.Ble;
				break;
			case Code.Ble:
				result = OpCodes.Bgt;
				break;
			case Code.Blt:
				result = OpCodes.Bge;
				break;
			case Code.Bne_Un:
				result = OpCodes.Beq;
				break;
			case Code.Bge_Un:
				result = OpCodes.Blt_Un;
				break;
			case Code.Bgt_Un:
				result = OpCodes.Ble_Un;
				break;
			case Code.Ble_Un:
				result = OpCodes.Bgt_Un;
				break;
			case Code.Blt_Un:
				result = OpCodes.Bge_Un;
				break;
			default:
				throw new NotSupportedException();
			}
			return result;
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0001E3AC File Offset: 0x0001C5AC
		public override void Mangle(CilBody body, ScopeBlock root, CFContext ctx)
		{
			SwitchMangler.<>c__DisplayClass3_0 CS$<>8__locals1 = new SwitchMangler.<>c__DisplayClass3_0();
			CS$<>8__locals1.trace = new SwitchMangler.Trace(body, ctx.Method.ReturnType.RemoveModifiers().ElementType != ElementType.Void);
			Local local = new Local(ctx.Method.Module.CorLibTypes.UInt32);
			body.Variables.Add(local);
			body.InitLocals = true;
			body.MaxStack += 2;
			IPredicate predicate = null;
			bool flag = ctx.Predicate == PredicateType.Normal;
			if (flag)
			{
				predicate = new NormalPredicate(ctx);
			}
			else
			{
				bool flag2 = ctx.Predicate == PredicateType.Expression;
				if (flag2)
				{
					predicate = new ExpressionPredicate(ctx);
				}
				else
				{
					bool flag3 = ctx.Predicate == PredicateType.x86;
					if (flag3)
					{
						predicate = new x86Predicate(ctx);
					}
				}
			}
			using (IEnumerator<InstrBlock> enumerator = ManglerBase.GetAllBlocks(root).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SwitchMangler.<>c__DisplayClass3_2 CS$<>8__locals2 = new SwitchMangler.<>c__DisplayClass3_2();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.block = enumerator.Current;
					LinkedList<Instruction[]> statements = this.SpiltStatements(CS$<>8__locals2.block, CS$<>8__locals2.CS$<>8__locals1.trace, ctx);
					bool isInstanceConstructor = ctx.Method.IsInstanceConstructor;
					if (isInstanceConstructor)
					{
						List<Instruction> newStatement = new List<Instruction>();
						while (statements.First != null)
						{
							newStatement.AddRange(statements.First.Value);
							Instruction lastInstr = statements.First.Value.Last<Instruction>();
							statements.RemoveFirst();
							bool flag4 = lastInstr.OpCode == OpCodes.Call && ((IMethod)lastInstr.Operand).Name == ".ctor";
							if (flag4)
							{
								break;
							}
						}
						statements.AddFirst(newStatement.ToArray());
					}
					bool flag5 = statements.Count < 3;
					if (!flag5)
					{
						int[] keyId = Enumerable.Range(0, statements.Count).ToArray<int>();
						ctx.Random.Shuffle<int>(keyId);
						int[] key = new int[keyId.Length];
						int i;
						for (i = 0; i < key.Length; i++)
						{
							int q = ctx.Random.NextInt32() & int.MaxValue;
							key[i] = q - q % statements.Count + keyId[i];
						}
						Dictionary<Instruction, int> statementKeys = new Dictionary<Instruction, int>();
						LinkedListNode<Instruction[]> current = statements.First;
						i = 0;
						while (current != null)
						{
							bool flag6 = i != 0;
							if (flag6)
							{
								statementKeys[current.Value[0]] = key[i];
							}
							i++;
							current = current.Next;
						}
						HashSet<Instruction> statementLast = new HashSet<Instruction>(from st in statements
						select st.Last<Instruction>());
						Func<Instruction, bool> <>9__4;
						Func<Instruction, bool> <>9__5;
						Func<Instruction, bool> <>9__2;
						Func<IList<Instruction>, bool> hasUnknownSource = delegate(IList<Instruction> instrs)
						{
							Func<Instruction, bool> predicate2;
							if ((predicate2 = <>9__2) == null)
							{
								predicate2 = (<>9__2 = delegate(Instruction instr)
								{
									bool flag19 = CS$<>8__locals2.CS$<>8__locals1.trace.HasMultipleSources(instr.Offset);
									bool result;
									if (flag19)
									{
										result = true;
									}
									else
									{
										List<Instruction> srcs;
										bool flag20 = CS$<>8__locals2.CS$<>8__locals1.trace.BrRefs.TryGetValue(instr.Offset, out srcs);
										if (flag20)
										{
											bool flag21 = srcs.Any((Instruction src) => src.Operand is Instruction[]);
											if (flag21)
											{
												return true;
											}
											IEnumerable<Instruction> source = srcs;
											Func<Instruction, bool> predicate3;
											if ((predicate3 = <>9__4) == null)
											{
												predicate3 = (<>9__4 = ((Instruction src) => src.Offset <= statements.First.Value.Last<Instruction>().Offset || src.Offset >= CS$<>8__locals2.block.Instructions.Last<Instruction>().Offset));
											}
											bool flag22 = source.Any(predicate3);
											if (flag22)
											{
												return true;
											}
											IEnumerable<Instruction> source2 = srcs;
											Func<Instruction, bool> predicate4;
											if ((predicate4 = <>9__5) == null)
											{
												predicate4 = (<>9__5 = ((Instruction src) => statementLast.Contains(src)));
											}
											bool flag23 = source2.Any(predicate4);
											if (flag23)
											{
												return true;
											}
										}
										result = false;
									}
									return result;
								});
							}
							return instrs.Any(predicate2);
						};
						Instruction switchInstr = new Instruction(OpCodes.Switch);
						List<Instruction> switchHdr = new List<Instruction>();
						bool flag7 = predicate != null;
						if (flag7)
						{
							predicate.Init(body);
							switchHdr.Add(Instruction.CreateLdcI4(predicate.GetSwitchKey(key[1])));
							predicate.EmitSwitchLoad(switchHdr);
						}
						else
						{
							switchHdr.Add(Instruction.CreateLdcI4(key[1]));
						}
						switchHdr.Add(Instruction.Create(OpCodes.Dup));
						switchHdr.Add(Instruction.Create(OpCodes.Stloc, local));
						switchHdr.Add(Instruction.Create(OpCodes.Ldc_I4, statements.Count));
						switchHdr.Add(Instruction.Create(OpCodes.Rem_Un));
						switchHdr.Add(switchInstr);
						ctx.AddJump(switchHdr, statements.Last.Value[0]);
						ctx.AddJunk(switchHdr);
						Instruction[] operands = new Instruction[statements.Count];
						current = statements.First;
						i = 0;
						while (current.Next != null)
						{
							List<Instruction> newStatement2 = new List<Instruction>(current.Value);
							bool flag8 = i != 0;
							if (flag8)
							{
								bool converted = false;
								bool flag9 = newStatement2.Last<Instruction>().IsBr();
								if (flag9)
								{
									Instruction target = (Instruction)newStatement2.Last<Instruction>().Operand;
									int brKey;
									bool flag10 = !CS$<>8__locals2.CS$<>8__locals1.trace.IsBranchTarget(newStatement2.Last<Instruction>().Offset) && statementKeys.TryGetValue(target, out brKey);
									if (flag10)
									{
										int targetKey = (predicate != null) ? predicate.GetSwitchKey(brKey) : brKey;
										bool unkSrc = hasUnknownSource(newStatement2);
										newStatement2.RemoveAt(newStatement2.Count - 1);
										bool flag11 = unkSrc;
										if (flag11)
										{
											newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, targetKey));
										}
										else
										{
											int thisKey = key[i];
											int r = ctx.Random.NextInt32();
											newStatement2.Add(Instruction.Create(OpCodes.Ldloc, local));
											newStatement2.Add(Instruction.CreateLdcI4(r));
											newStatement2.Add(Instruction.Create(OpCodes.Mul));
											newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, thisKey * r ^ targetKey));
											newStatement2.Add(Instruction.Create(OpCodes.Xor));
										}
										ctx.AddJump(newStatement2, switchHdr[1]);
										ctx.AddJunk(newStatement2);
										operands[keyId[i]] = newStatement2[0];
										converted = true;
									}
								}
								else
								{
									bool flag12 = newStatement2.Last<Instruction>().IsConditionalBranch();
									if (flag12)
									{
										Instruction target2 = (Instruction)newStatement2.Last<Instruction>().Operand;
										int brKey2;
										bool flag13 = !CS$<>8__locals2.CS$<>8__locals1.trace.IsBranchTarget(newStatement2.Last<Instruction>().Offset) && statementKeys.TryGetValue(target2, out brKey2);
										if (flag13)
										{
											bool unkSrc2 = hasUnknownSource(newStatement2);
											int nextKey = key[i + 1];
											OpCode condBr = newStatement2.Last<Instruction>().OpCode;
											newStatement2.RemoveAt(newStatement2.Count - 1);
											bool flag14 = ctx.Random.NextBoolean();
											if (flag14)
											{
												condBr = SwitchMangler.InverseBranch(condBr);
												int tmp = brKey2;
												brKey2 = nextKey;
												nextKey = tmp;
											}
											int thisKey2 = key[i];
											int r2 = 0;
											int xorKey = 0;
											bool flag15 = !unkSrc2;
											if (flag15)
											{
												r2 = ctx.Random.NextInt32();
												xorKey = thisKey2 * r2;
											}
											Instruction brKeyInstr = Instruction.CreateLdcI4(xorKey ^ ((predicate != null) ? predicate.GetSwitchKey(brKey2) : brKey2));
											Instruction nextKeyInstr = Instruction.CreateLdcI4(xorKey ^ ((predicate != null) ? predicate.GetSwitchKey(nextKey) : nextKey));
											Instruction pop = Instruction.Create(OpCodes.Pop);
											newStatement2.Add(Instruction.Create(condBr, brKeyInstr));
											newStatement2.Add(nextKeyInstr);
											newStatement2.Add(Instruction.Create(OpCodes.Dup));
											newStatement2.Add(Instruction.Create(OpCodes.Br, pop));
											newStatement2.Add(brKeyInstr);
											newStatement2.Add(Instruction.Create(OpCodes.Dup));
											newStatement2.Add(pop);
											bool flag16 = !unkSrc2;
											if (flag16)
											{
												newStatement2.Add(Instruction.Create(OpCodes.Ldloc, local));
												newStatement2.Add(Instruction.CreateLdcI4(r2));
												newStatement2.Add(Instruction.Create(OpCodes.Mul));
												newStatement2.Add(Instruction.Create(OpCodes.Xor));
											}
											ctx.AddJump(newStatement2, switchHdr[1]);
											ctx.AddJunk(newStatement2);
											operands[keyId[i]] = newStatement2[0];
											converted = true;
										}
									}
								}
								bool flag17 = !converted;
								if (flag17)
								{
									int targetKey2 = (predicate != null) ? predicate.GetSwitchKey(key[i + 1]) : key[i + 1];
									bool flag18 = !hasUnknownSource(newStatement2);
									if (flag18)
									{
										int thisKey3 = key[i];
										int r3 = ctx.Random.NextInt32();
										newStatement2.Add(Instruction.Create(OpCodes.Ldloc, local));
										newStatement2.Add(Instruction.CreateLdcI4(r3));
										newStatement2.Add(Instruction.Create(OpCodes.Mul));
										newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, thisKey3 * r3 ^ targetKey2));
										newStatement2.Add(Instruction.Create(OpCodes.Xor));
									}
									else
									{
										newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, targetKey2));
									}
									ctx.AddJump(newStatement2, switchHdr[1]);
									ctx.AddJunk(newStatement2);
									operands[keyId[i]] = newStatement2[0];
								}
							}
							else
							{
								operands[keyId[i]] = switchHdr[0];
							}
							current.Value = newStatement2.ToArray();
							current = current.Next;
							i++;
						}
						operands[keyId[i]] = current.Value[0];
						switchInstr.Operand = operands;
						Instruction[] first = statements.First.Value;
						statements.RemoveFirst();
						Instruction[] last = statements.Last.Value;
						statements.RemoveLast();
						List<Instruction[]> newStatements = statements.ToList<Instruction[]>();
						ctx.Random.Shuffle<Instruction[]>(newStatements);
						CS$<>8__locals2.block.Instructions.Clear();
						CS$<>8__locals2.block.Instructions.AddRange(first);
						CS$<>8__locals2.block.Instructions.AddRange(switchHdr);
						foreach (Instruction[] statement in newStatements)
						{
							CS$<>8__locals2.block.Instructions.AddRange(statement);
						}
						CS$<>8__locals2.block.Instructions.AddRange(last);
					}
				}
			}
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00005C85 File Offset: 0x00003E85
		public SwitchMangler()
		{
		}

		// Token: 0x020000F2 RID: 242
		private struct Trace
		{
			// Token: 0x060003BA RID: 954 RVA: 0x0001EE18 File Offset: 0x0001D018
			private static void Increment(Dictionary<uint, int> counts, uint key)
			{
				int value;
				bool flag = !counts.TryGetValue(key, out value);
				if (flag)
				{
					value = 0;
				}
				counts[key] = value + 1;
			}

			// Token: 0x060003BB RID: 955 RVA: 0x0001EE44 File Offset: 0x0001D044
			public Trace(CilBody body, bool hasReturnValue)
			{
				this.RefCount = new Dictionary<uint, int>();
				this.BrRefs = new Dictionary<uint, List<Instruction>>();
				this.BeforeStack = new Dictionary<uint, int>();
				this.AfterStack = new Dictionary<uint, int>();
				body.UpdateInstructionOffsets();
				foreach (ExceptionHandler eh in body.ExceptionHandlers)
				{
					this.BeforeStack[eh.TryStart.Offset] = 0;
					this.BeforeStack[eh.HandlerStart.Offset] = ((eh.HandlerType != ExceptionHandlerType.Finally) ? 1 : 0);
					bool flag = eh.FilterStart != null;
					if (flag)
					{
						this.BeforeStack[eh.FilterStart.Offset] = 1;
					}
				}
				int currentStack = 0;
				int i = 0;
				while (i < body.Instructions.Count)
				{
					Instruction instr = body.Instructions[i];
					bool flag2 = this.BeforeStack.ContainsKey(instr.Offset);
					if (flag2)
					{
						currentStack = this.BeforeStack[instr.Offset];
					}
					this.BeforeStack[instr.Offset] = currentStack;
					instr.UpdateStack(ref currentStack, hasReturnValue);
					this.AfterStack[instr.Offset] = currentStack;
					switch (instr.OpCode.FlowControl)
					{
					case FlowControl.Branch:
					{
						uint offset = ((Instruction)instr.Operand).Offset;
						bool flag3 = !this.BeforeStack.ContainsKey(offset);
						if (flag3)
						{
							this.BeforeStack[offset] = currentStack;
						}
						SwitchMangler.Trace.Increment(this.RefCount, offset);
						this.BrRefs.AddListEntry(offset, instr);
						currentStack = 0;
						break;
					}
					case FlowControl.Break:
					case FlowControl.Meta:
					case FlowControl.Next:
						goto IL_2F5;
					case FlowControl.Call:
					{
						bool flag4 = instr.OpCode.Code == Code.Jmp;
						if (flag4)
						{
							currentStack = 0;
						}
						goto IL_2F5;
					}
					case FlowControl.Cond_Branch:
					{
						bool flag5 = instr.OpCode.Code == Code.Switch;
						if (flag5)
						{
							foreach (Instruction target in (Instruction[])instr.Operand)
							{
								bool flag6 = !this.BeforeStack.ContainsKey(target.Offset);
								if (flag6)
								{
									this.BeforeStack[target.Offset] = currentStack;
								}
								SwitchMangler.Trace.Increment(this.RefCount, target.Offset);
								this.BrRefs.AddListEntry(target.Offset, instr);
							}
						}
						else
						{
							uint offset = ((Instruction)instr.Operand).Offset;
							bool flag7 = !this.BeforeStack.ContainsKey(offset);
							if (flag7)
							{
								this.BeforeStack[offset] = currentStack;
							}
							SwitchMangler.Trace.Increment(this.RefCount, offset);
							this.BrRefs.AddListEntry(offset, instr);
						}
						goto IL_2F5;
					}
					case FlowControl.Phi:
						goto IL_2EF;
					case FlowControl.Return:
					case FlowControl.Throw:
						break;
					default:
						goto IL_2EF;
					}
					IL_333:
					i++;
					continue;
					IL_2F5:
					bool flag8 = i + 1 < body.Instructions.Count;
					if (flag8)
					{
						uint offset = body.Instructions[i + 1].Offset;
						SwitchMangler.Trace.Increment(this.RefCount, offset);
					}
					goto IL_333;
					IL_2EF:
					throw new UnreachableException();
				}
			}

			// Token: 0x060003BC RID: 956 RVA: 0x0001F1B4 File Offset: 0x0001D3B4
			public bool IsBranchTarget(uint offset)
			{
				List<Instruction> src;
				bool flag = this.BrRefs.TryGetValue(offset, out src);
				return flag && src.Count > 0;
			}

			// Token: 0x060003BD RID: 957 RVA: 0x0001F1E8 File Offset: 0x0001D3E8
			public bool HasMultipleSources(uint offset)
			{
				int src;
				bool flag = this.RefCount.TryGetValue(offset, out src);
				return flag && src > 1;
			}

			// Token: 0x040002C1 RID: 705
			public Dictionary<uint, int> RefCount;

			// Token: 0x040002C2 RID: 706
			public Dictionary<uint, List<Instruction>> BrRefs;

			// Token: 0x040002C3 RID: 707
			public Dictionary<uint, int> BeforeStack;

			// Token: 0x040002C4 RID: 708
			public Dictionary<uint, int> AfterStack;
		}

		// Token: 0x020000F3 RID: 243
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_0
		{
			// Token: 0x060003BE RID: 958 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass3_0()
			{
			}

			// Token: 0x040002C5 RID: 709
			public SwitchMangler.Trace trace;
		}

		// Token: 0x020000F4 RID: 244
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_1
		{
			// Token: 0x060003BF RID: 959 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass3_1()
			{
			}

			// Token: 0x060003C0 RID: 960 RVA: 0x0001F214 File Offset: 0x0001D414
			internal bool <Mangle>b__1(IList<Instruction> instrs)
			{
				Func<Instruction, bool> predicate;
				if ((predicate = this.<>9__2) == null)
				{
					predicate = (this.<>9__2 = delegate(Instruction instr)
					{
						bool flag = this.CS$<>8__locals2.CS$<>8__locals1.trace.HasMultipleSources(instr.Offset);
						bool result;
						if (flag)
						{
							result = true;
						}
						else
						{
							List<Instruction> srcs;
							bool flag2 = this.CS$<>8__locals2.CS$<>8__locals1.trace.BrRefs.TryGetValue(instr.Offset, out srcs);
							if (flag2)
							{
								bool flag3 = srcs.Any(new Func<Instruction, bool>(SwitchMangler.<>c.<>9.<Mangle>b__3_3));
								if (flag3)
								{
									return true;
								}
								IEnumerable<Instruction> source = srcs;
								Func<Instruction, bool> predicate2;
								if ((predicate2 = this.<>9__4) == null)
								{
									predicate2 = (this.<>9__4 = ((Instruction src) => src.Offset <= this.statements.First.Value.Last<Instruction>().Offset || src.Offset >= this.CS$<>8__locals2.block.Instructions.Last<Instruction>().Offset));
								}
								bool flag4 = source.Any(predicate2);
								if (flag4)
								{
									return true;
								}
								IEnumerable<Instruction> source2 = srcs;
								Func<Instruction, bool> predicate3;
								if ((predicate3 = this.<>9__5) == null)
								{
									predicate3 = (this.<>9__5 = ((Instruction src) => this.statementLast.Contains(src)));
								}
								bool flag5 = source2.Any(predicate3);
								if (flag5)
								{
									return true;
								}
							}
							result = false;
						}
						return result;
					});
				}
				return instrs.Any(predicate);
			}

			// Token: 0x060003C1 RID: 961 RVA: 0x0001F248 File Offset: 0x0001D448
			internal bool <Mangle>b__2(Instruction instr)
			{
				bool flag = this.CS$<>8__locals2.CS$<>8__locals1.trace.HasMultipleSources(instr.Offset);
				bool result;
				if (flag)
				{
					result = true;
				}
				else
				{
					List<Instruction> srcs;
					bool flag2 = this.CS$<>8__locals2.CS$<>8__locals1.trace.BrRefs.TryGetValue(instr.Offset, out srcs);
					if (flag2)
					{
						bool flag3 = srcs.Any(new Func<Instruction, bool>(SwitchMangler.<>c.<>9.<Mangle>b__3_3));
						if (flag3)
						{
							return true;
						}
						IEnumerable<Instruction> source = srcs;
						Func<Instruction, bool> predicate;
						if ((predicate = this.<>9__4) == null)
						{
							predicate = (this.<>9__4 = ((Instruction src) => src.Offset <= this.statements.First.Value.Last<Instruction>().Offset || src.Offset >= this.CS$<>8__locals2.block.Instructions.Last<Instruction>().Offset));
						}
						bool flag4 = source.Any(predicate);
						if (flag4)
						{
							return true;
						}
						IEnumerable<Instruction> source2 = srcs;
						Func<Instruction, bool> predicate2;
						if ((predicate2 = this.<>9__5) == null)
						{
							predicate2 = (this.<>9__5 = ((Instruction src) => this.statementLast.Contains(src)));
						}
						bool flag5 = source2.Any(predicate2);
						if (flag5)
						{
							return true;
						}
					}
					result = false;
				}
				return result;
			}

			// Token: 0x060003C2 RID: 962 RVA: 0x0001F340 File Offset: 0x0001D540
			internal bool <Mangle>b__4(Instruction src)
			{
				return src.Offset <= this.statements.First.Value.Last<Instruction>().Offset || src.Offset >= this.CS$<>8__locals2.block.Instructions.Last<Instruction>().Offset;
			}

			// Token: 0x060003C3 RID: 963 RVA: 0x00005D4F File Offset: 0x00003F4F
			internal bool <Mangle>b__5(Instruction src)
			{
				return this.statementLast.Contains(src);
			}

			// Token: 0x040002C6 RID: 710
			public LinkedList<Instruction[]> statements;

			// Token: 0x040002C7 RID: 711
			public HashSet<Instruction> statementLast;

			// Token: 0x040002C8 RID: 712
			public SwitchMangler.<>c__DisplayClass3_2 CS$<>8__locals2;

			// Token: 0x040002C9 RID: 713
			public Func<Instruction, bool> <>9__4;

			// Token: 0x040002CA RID: 714
			public Func<Instruction, bool> <>9__5;

			// Token: 0x040002CB RID: 715
			public Func<Instruction, bool> <>9__2;
		}

		// Token: 0x020000F5 RID: 245
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_2
		{
			// Token: 0x060003C4 RID: 964 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass3_2()
			{
			}

			// Token: 0x040002CC RID: 716
			public InstrBlock block;

			// Token: 0x040002CD RID: 717
			public SwitchMangler.<>c__DisplayClass3_0 CS$<>8__locals1;
		}

		// Token: 0x020000F6 RID: 246
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060003C5 RID: 965 RVA: 0x00005D5D File Offset: 0x00003F5D
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060003C6 RID: 966 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x060003C7 RID: 967 RVA: 0x00005D69 File Offset: 0x00003F69
			internal Instruction <Mangle>b__3_0(Instruction[] st)
			{
				return st.Last<Instruction>();
			}

			// Token: 0x060003C8 RID: 968 RVA: 0x000052E6 File Offset: 0x000034E6
			internal bool <Mangle>b__3_3(Instruction src)
			{
				return src.Operand is Instruction[];
			}

			// Token: 0x040002CE RID: 718
			public static readonly SwitchMangler.<>c <>9 = new SwitchMangler.<>c();

			// Token: 0x040002CF RID: 719
			public static Func<Instruction[], Instruction> <>9__3_0;

			// Token: 0x040002D0 RID: 720
			public static Func<Instruction, bool> <>9__3_3;
		}
	}
}
