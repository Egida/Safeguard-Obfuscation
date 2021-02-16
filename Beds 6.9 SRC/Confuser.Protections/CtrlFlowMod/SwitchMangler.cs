using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000C7 RID: 199
	internal class SwitchMangler : ManglerBase
	{
		// Token: 0x06000314 RID: 788 RVA: 0x00019370 File Offset: 0x00017570
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

		// Token: 0x06000315 RID: 789 RVA: 0x00019428 File Offset: 0x00017628
		public override void Mangle(CilBody body, ScopeBlock root, CFContext ctx)
		{
			MethodTrace trace = ctx.Context.Registry.GetService<ITraceService>().Trace(ctx.Method);
			body.MaxStack += 2;
			IPredicate predicate = null;
			bool flag2 = ctx.Predicate == PredicateType.Normal;
			if (flag2)
			{
				predicate = new NormalPredicate(ctx);
			}
			else
			{
				bool flag3 = ctx.Predicate == PredicateType.Expression;
				if (flag3)
				{
					predicate = new ExpressionPredicate(ctx);
				}
				else
				{
					bool flag4 = ctx.Predicate == PredicateType.x86;
					if (flag4)
					{
						predicate = new x86Predicate(ctx);
					}
				}
			}
			foreach (InstrBlock block in ManglerBase.GetAllBlocks(root))
			{
				LinkedList<Instruction[]> source = this.SpiltStatements(block, trace, ctx);
				bool isInstanceConstructor = ctx.Method.IsInstanceConstructor;
				if (isInstanceConstructor)
				{
					List<Instruction> list2 = new List<Instruction>();
					while (source.First != null)
					{
						list2.AddRange(source.First.Value);
						Instruction instruction = source.First.Value.Last<Instruction>();
						source.RemoveFirst();
						bool flag5 = instruction.OpCode == OpCodes.Call && ((IMethod)instruction.Operand).Name == ".ctor";
						if (flag5)
						{
							break;
						}
					}
					source.AddFirst(list2.ToArray());
				}
				bool flag6 = source.Count >= 3;
				if (flag6)
				{
					int[] list3 = Enumerable.Range(0, source.Count).ToArray<int>();
					ctx.Random.Shuffle<int>(list3);
					Dictionary<Instruction, int> dictionary = new Dictionary<Instruction, int>();
					LinkedListNode<Instruction[]> first = source.First;
					int index = 0;
					while (first != null)
					{
						bool flag7 = index != 0;
						if (flag7)
						{
							dictionary[first.Value[0]] = list3[index];
						}
						index++;
						first = first.Next;
					}
					Instruction item = new Instruction(OpCodes.Switch);
					List<Instruction> instrs = new List<Instruction>();
					bool flag8 = predicate != null;
					if (flag8)
					{
						predicate.Init(body);
						instrs.Add(Instruction.CreateLdcI4(predicate.GetSwitchKey(list3[1])));
						predicate.EmitSwitchLoad(instrs);
					}
					else
					{
						instrs.Add(Instruction.CreateLdcI4(list3[1]));
					}
					instrs.Add(item);
					ctx.AddJump(instrs, source.Last.Value[0]);
					ctx.AddJunk(instrs);
					Instruction[] instructionArray = new Instruction[source.Count];
					first = source.First;
					index = 0;
					while (first.Next != null)
					{
						List<Instruction> list4 = new List<Instruction>(first.Value);
						bool flag9 = index != 0;
						if (flag9)
						{
							bool flag = false;
							bool flag10 = list4.Last<Instruction>().IsBr();
							if (flag10)
							{
								Instruction operand = (Instruction)list4.Last<Instruction>().Operand;
								int num2;
								bool flag11 = !trace.IsBranchTarget(trace.OffsetToIndexMap(list4.Last<Instruction>().Offset)) && dictionary.TryGetValue(operand, out num2);
								if (flag11)
								{
									list4.RemoveAt(list4.Count - 1);
									list4.Add(Instruction.CreateLdcI4((predicate != null) ? predicate.GetSwitchKey(num2) : num2));
									ctx.AddJump(list4, instrs[1]);
									ctx.AddJunk(list4);
									instructionArray[list3[index]] = list4[0];
									flag = true;
								}
							}
							else
							{
								bool flag12 = list4.Last<Instruction>().IsConditionalBranch();
								if (flag12)
								{
									Instruction key = (Instruction)list4.Last<Instruction>().Operand;
									int num3;
									bool flag13 = !trace.IsBranchTarget(trace.OffsetToIndexMap(list4.Last<Instruction>().Offset)) && dictionary.TryGetValue(key, out num3);
									if (flag13)
									{
										int num4 = list3[index + 1];
										OpCode opCode = list4.Last<Instruction>().OpCode;
										list4.RemoveAt(list4.Count - 1);
										bool flag14 = ctx.Random.NextBoolean();
										if (flag14)
										{
											opCode = SwitchMangler.InverseBranch(opCode);
											int num5 = num3;
											num3 = num4;
											num4 = num5;
										}
										Instruction target = Instruction.CreateLdcI4((predicate != null) ? predicate.GetSwitchKey(num3) : num3);
										Instruction instruction2 = Instruction.CreateLdcI4((predicate != null) ? predicate.GetSwitchKey(num4) : num4);
										Instruction instruction3 = Instruction.Create(OpCodes.Pop);
										list4.Add(Instruction.Create(opCode, target));
										list4.Add(instruction2);
										list4.Add(Instruction.Create(OpCodes.Dup));
										list4.Add(Instruction.Create(OpCodes.Br, instruction3));
										list4.Add(target);
										list4.Add(Instruction.Create(OpCodes.Dup));
										list4.Add(instruction3);
										ctx.AddJump(list4, instrs[1]);
										ctx.AddJunk(list4);
										instructionArray[list3[index]] = list4[0];
										flag = true;
									}
								}
							}
							bool flag15 = !flag;
							if (flag15)
							{
								list4.Add(Instruction.CreateLdcI4((predicate != null) ? predicate.GetSwitchKey(list3[index + 1]) : list3[index + 1]));
								ctx.AddJump(list4, instrs[1]);
								ctx.AddJunk(list4);
								instructionArray[list3[index]] = list4[0];
							}
						}
						else
						{
							instructionArray[list3[index]] = instrs[0];
						}
						first.Value = list4.ToArray();
						first = first.Next;
						index++;
					}
					instructionArray[list3[index]] = first.Value[0];
					item.Operand = instructionArray;
					Instruction[] collection = source.First.Value;
					source.RemoveFirst();
					Instruction[] instructionArray2 = source.Last.Value;
					source.RemoveLast();
					List<Instruction[]> list5 = source.ToList<Instruction[]>();
					ctx.Random.Shuffle<Instruction[]>(list5);
					block.Instructions.Clear();
					block.Instructions.AddRange(collection);
					block.Instructions.AddRange(instrs);
					foreach (Instruction[] instructionArray3 in list5)
					{
						block.Instructions.AddRange(instructionArray3);
					}
					block.Instructions.AddRange(instructionArray2);
				}
			}
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00019AD4 File Offset: 0x00017CD4
		private LinkedList<Instruction[]> SpiltStatements(InstrBlock block, MethodTrace trace, CFContext ctx)
		{
			LinkedList<Instruction[]> list = new LinkedList<Instruction[]>();
			List<Instruction> list2 = new List<Instruction>();
			for (int i = 0; i < block.Instructions.Count; i++)
			{
				Instruction item = block.Instructions[i];
				list2.Add(item);
				bool flag = i + 1 < block.Instructions.Count && trace.IsBranchTarget(trace.OffsetToIndexMap(block.Instructions[i + 1].Offset));
				bool flag2 = item.OpCode.OpCodeType != OpCodeType.Prefix && trace.AfterStackDepths[trace.OffsetToIndexMap(item.Offset)] == 0 && (flag || ctx.Intensity > ctx.Random.NextDouble());
				if (flag2)
				{
					list.AddLast(list2.ToArray());
					list2.Clear();
				}
			}
			bool flag3 = list2.Count > 0;
			if (flag3)
			{
				list.AddLast(list2.ToArray());
			}
			return list;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00005820 File Offset: 0x00003A20
		public SwitchMangler()
		{
		}
	}
}
