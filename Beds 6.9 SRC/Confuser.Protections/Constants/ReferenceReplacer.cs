using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x0200008A RID: 138
	internal class ReferenceReplacer
	{
		// Token: 0x06000220 RID: 544 RVA: 0x00011760 File Offset: 0x0000F960
		public static void ReplaceReference(CEContext ctx, ProtectionParameters parameters)
		{
			foreach (KeyValuePair<MethodDef, List<Tuple<Instruction, uint, IMethod>>> entry in ctx.ReferenceRepl)
			{
				bool parameter = parameters.GetParameter<bool>(ctx.Context, entry.Key, "cfg", false);
				if (parameter)
				{
					ReferenceReplacer.ReplaceCFG(entry.Key, entry.Value, ctx);
				}
				else
				{
					ReferenceReplacer.ReplaceNormal(entry.Key, entry.Value);
				}
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x000117FC File Offset: 0x0000F9FC
		private static void ReplaceNormal(MethodDef method, List<Tuple<Instruction, uint, IMethod>> instrs)
		{
			foreach (Tuple<Instruction, uint, IMethod> instr in instrs)
			{
				int i = method.Body.Instructions.IndexOf(instr.Item1);
				instr.Item1.OpCode = OpCodes.Ldc_I4;
				instr.Item1.Operand = (int)instr.Item2;
				method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Call, instr.Item3));
			}
		}

		// Token: 0x06000222 RID: 546 RVA: 0x000118AC File Offset: 0x0000FAAC
		private static void InjectStateType(CEContext ctx)
		{
			bool flag = ctx.CfgCtxType == null;
			if (flag)
			{
				TypeDef type = ctx.Context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.CFGCtx");
				ctx.CfgCtxType = InjectHelper.Inject(type, ctx.Module);
				ctx.Module.Types.Add(ctx.CfgCtxType);
				ctx.CfgCtxCtor = ctx.CfgCtxType.FindMethod(".ctor");
				ctx.CfgCtxNext = ctx.CfgCtxType.FindMethod("Next");
				ctx.Name.MarkHelper(ctx.CfgCtxType, ctx.Marker, ctx.Protection);
				foreach (FieldDef def in ctx.CfgCtxType.Fields)
				{
					ctx.Name.MarkHelper(def, ctx.Marker, ctx.Protection);
				}
				foreach (MethodDef def2 in ctx.CfgCtxType.Methods)
				{
					ctx.Name.MarkHelper(def2, ctx.Marker, ctx.Protection);
				}
			}
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00011A1C File Offset: 0x0000FC1C
		private static void InsertEmptyStateUpdate(ReferenceReplacer.CFGContext ctx, ControlFlowBlock block)
		{
			CilBody body = ctx.Graph.Body;
			BlockKey key = ctx.Keys[block.Id];
			bool flag = key.EntryState == key.ExitState;
			if (!flag)
			{
				int targetIndex = body.Instructions.IndexOf(block.Header);
				ReferenceReplacer.CFGState entry;
				bool flag2 = !ctx.StatesMap.TryGetValue(key.EntryState, out entry);
				if (flag2)
				{
					key.Type = BlockKeyType.Explicit;
				}
				bool flag3 = key.Type == BlockKeyType.Incremental;
				Instruction first;
				if (flag3)
				{
					ReferenceReplacer.CFGState exit;
					bool flag4 = !ctx.StatesMap.TryGetValue(key.ExitState, out exit);
					if (flag4)
					{
						exit = entry;
						int updateId = ctx.Random.NextInt32(3);
						uint targetValue = ctx.Random.NextUInt32();
						exit.UpdateExplicit(updateId, targetValue);
						int getId = ctx.Random.NextInt32(3);
						byte fl = ReferenceReplacer.CFGState.EncodeFlag(true, updateId, getId);
						uint incr = entry.GetIncrementalUpdate(updateId, targetValue);
						body.Instructions.Insert(targetIndex++, first = Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
						body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)fl));
						body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Ldc_I4, (int)incr));
						body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
						body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Pop));
						ctx.StatesMap[key.ExitState] = exit;
					}
					else
					{
						int headerIndex = targetIndex;
						for (int stateId = 0; stateId < 4; stateId++)
						{
							bool flag5 = entry.Get(stateId) == exit.Get(stateId);
							if (!flag5)
							{
								uint targetValue2 = exit.Get(stateId);
								int getId2 = ctx.Random.NextInt32(3);
								byte fl2 = ReferenceReplacer.CFGState.EncodeFlag(true, stateId, getId2);
								uint incr2 = entry.GetIncrementalUpdate(stateId, targetValue2);
								body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
								body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)fl2));
								body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Ldc_I4, (int)incr2));
								body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
								body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Pop));
							}
						}
						first = body.Instructions[headerIndex];
					}
				}
				else
				{
					ReferenceReplacer.CFGState exit2;
					bool flag6 = !ctx.StatesMap.TryGetValue(key.ExitState, out exit2);
					if (flag6)
					{
						uint seed = ctx.Random.NextUInt32();
						exit2 = new ReferenceReplacer.CFGState(seed);
						body.Instructions.Insert(targetIndex++, first = Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
						body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Ldc_I4, (int)seed));
						body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxCtor));
						ctx.StatesMap[key.ExitState] = exit2;
					}
					else
					{
						int headerIndex2 = targetIndex;
						for (int stateId2 = 0; stateId2 < 4; stateId2++)
						{
							uint targetValue3 = exit2.Get(stateId2);
							int getId3 = ctx.Random.NextInt32(3);
							byte fl3 = ReferenceReplacer.CFGState.EncodeFlag(true, stateId2, getId3);
							body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
							body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)fl3));
							body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Ldc_I4, (int)targetValue3));
							body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
							body.Instructions.Insert(targetIndex++, Instruction.Create(OpCodes.Pop));
						}
						first = body.Instructions[headerIndex2];
					}
				}
				ctx.Graph.Body.ReplaceReference(block.Header, first);
			}
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00011EC4 File Offset: 0x000100C4
		private static uint InsertStateGetAndUpdate(ReferenceReplacer.CFGContext ctx, ref int index, BlockKeyType type, ref ReferenceReplacer.CFGState currentState, ReferenceReplacer.CFGState? targetState)
		{
			CilBody body = ctx.Graph.Body;
			bool flag = type == BlockKeyType.Incremental;
			uint result;
			if (flag)
			{
				bool flag2 = targetState == null;
				if (flag2)
				{
					int updateId = ctx.Random.NextInt32(3);
					uint targetValue = ctx.Random.NextUInt32();
					int getId = ctx.Random.NextInt32(3);
					byte fl = ReferenceReplacer.CFGState.EncodeFlag(true, updateId, getId);
					uint incr = currentState.GetIncrementalUpdate(updateId, targetValue);
					currentState.UpdateExplicit(updateId, targetValue);
					IList<Instruction> instructions = body.Instructions;
					int num = index;
					index = num + 1;
					instructions.Insert(num, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
					IList<Instruction> instructions2 = body.Instructions;
					num = index;
					index = num + 1;
					instructions2.Insert(num, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)fl));
					IList<Instruction> instructions3 = body.Instructions;
					num = index;
					index = num + 1;
					instructions3.Insert(num, Instruction.Create(OpCodes.Ldc_I4, (int)incr));
					IList<Instruction> instructions4 = body.Instructions;
					num = index;
					index = num + 1;
					instructions4.Insert(num, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
					result = currentState.Get(getId);
				}
				else
				{
					int[] stateIds = new int[]
					{
						0,
						1,
						2,
						3
					};
					ctx.Random.Shuffle<int>(stateIds);
					int i = 0;
					uint getValue = 0U;
					foreach (int stateId in stateIds)
					{
						bool flag3 = currentState.Get(stateId) == targetState.Value.Get(stateId) && i != stateIds.Length - 1;
						if (flag3)
						{
							i++;
						}
						else
						{
							uint targetValue2 = targetState.Value.Get(stateId);
							int getId2 = ctx.Random.NextInt32(3);
							byte fl2 = ReferenceReplacer.CFGState.EncodeFlag(true, stateId, getId2);
							uint incr2 = currentState.GetIncrementalUpdate(stateId, targetValue2);
							currentState.UpdateExplicit(stateId, targetValue2);
							IList<Instruction> instructions5 = body.Instructions;
							int num = index;
							index = num + 1;
							instructions5.Insert(num, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
							IList<Instruction> instructions6 = body.Instructions;
							num = index;
							index = num + 1;
							instructions6.Insert(num, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)fl2));
							IList<Instruction> instructions7 = body.Instructions;
							num = index;
							index = num + 1;
							instructions7.Insert(num, Instruction.Create(OpCodes.Ldc_I4, (int)incr2));
							IList<Instruction> instructions8 = body.Instructions;
							num = index;
							index = num + 1;
							instructions8.Insert(num, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
							i++;
							bool flag4 = i == stateIds.Length;
							if (flag4)
							{
								getValue = currentState.Get(getId2);
							}
							else
							{
								IList<Instruction> instructions9 = body.Instructions;
								num = index;
								index = num + 1;
								instructions9.Insert(num, Instruction.Create(OpCodes.Pop));
							}
						}
					}
					result = getValue;
				}
			}
			else
			{
				bool flag5 = targetState == null;
				if (flag5)
				{
					uint seed = ctx.Random.NextUInt32();
					currentState = new ReferenceReplacer.CFGState(seed);
					IList<Instruction> instructions10 = body.Instructions;
					int num = index;
					index = num + 1;
					instructions10.Insert(num, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
					IList<Instruction> instructions11 = body.Instructions;
					num = index;
					index = num + 1;
					instructions11.Insert(num, Instruction.Create(OpCodes.Dup));
					IList<Instruction> instructions12 = body.Instructions;
					num = index;
					index = num + 1;
					instructions12.Insert(num, Instruction.Create(OpCodes.Ldc_I4, (int)seed));
					IList<Instruction> instructions13 = body.Instructions;
					num = index;
					index = num + 1;
					instructions13.Insert(num, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxCtor));
					int updateId2 = ctx.Random.NextInt32(3);
					uint targetValue3 = ctx.Random.NextUInt32();
					int getId3 = ctx.Random.NextInt32(3);
					byte fl3 = ReferenceReplacer.CFGState.EncodeFlag(true, updateId2, getId3);
					uint incr3 = currentState.GetIncrementalUpdate(updateId2, targetValue3);
					currentState.UpdateExplicit(updateId2, targetValue3);
					IList<Instruction> instructions14 = body.Instructions;
					num = index;
					index = num + 1;
					instructions14.Insert(num, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)fl3));
					IList<Instruction> instructions15 = body.Instructions;
					num = index;
					index = num + 1;
					instructions15.Insert(num, Instruction.Create(OpCodes.Ldc_I4, (int)incr3));
					IList<Instruction> instructions16 = body.Instructions;
					num = index;
					index = num + 1;
					instructions16.Insert(num, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
					result = currentState.Get(getId3);
				}
				else
				{
					int[] stateIds2 = new int[]
					{
						0,
						1,
						2,
						3
					};
					ctx.Random.Shuffle<int>(stateIds2);
					int j = 0;
					uint getValue2 = 0U;
					foreach (int stateId2 in stateIds2)
					{
						uint targetValue4 = targetState.Value.Get(stateId2);
						int getId4 = ctx.Random.NextInt32(3);
						byte fl4 = ReferenceReplacer.CFGState.EncodeFlag(true, stateId2, getId4);
						currentState.UpdateExplicit(stateId2, targetValue4);
						IList<Instruction> instructions17 = body.Instructions;
						int num = index;
						index = num + 1;
						instructions17.Insert(num, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
						IList<Instruction> instructions18 = body.Instructions;
						num = index;
						index = num + 1;
						instructions18.Insert(num, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)fl4));
						IList<Instruction> instructions19 = body.Instructions;
						num = index;
						index = num + 1;
						instructions19.Insert(num, Instruction.Create(OpCodes.Ldc_I4, (int)targetValue4));
						IList<Instruction> instructions20 = body.Instructions;
						num = index;
						index = num + 1;
						instructions20.Insert(num, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
						j++;
						bool flag6 = j == stateIds2.Length;
						if (flag6)
						{
							getValue2 = targetState.Value.Get(getId4);
						}
						else
						{
							IList<Instruction> instructions21 = body.Instructions;
							num = index;
							index = num + 1;
							instructions21.Insert(num, Instruction.Create(OpCodes.Pop));
						}
					}
					result = getValue2;
				}
			}
			return result;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x000124BC File Offset: 0x000106BC
		private static void ReplaceCFG(MethodDef method, List<Tuple<Instruction, uint, IMethod>> instrs, CEContext ctx)
		{
			ReferenceReplacer.InjectStateType(ctx);
			ControlFlowGraph graph = ControlFlowGraph.Construct(method.Body);
			BlockKey[] sequence = KeySequence.ComputeKeys(graph, null);
			ReferenceReplacer.CFGContext cfgCtx = new ReferenceReplacer.CFGContext
			{
				Ctx = ctx,
				Graph = graph,
				Keys = sequence,
				StatesMap = new Dictionary<uint, ReferenceReplacer.CFGState>(),
				Random = ctx.Random
			};
			cfgCtx.StateVariable = new Local(ctx.CfgCtxType.ToTypeSig());
			method.Body.Variables.Add(cfgCtx.StateVariable);
			method.Body.InitLocals = true;
			Dictionary<int, SortedList<int, Tuple<Instruction, uint, IMethod>>> blockReferences = new Dictionary<int, SortedList<int, Tuple<Instruction, uint, IMethod>>>();
			foreach (Tuple<Instruction, uint, IMethod> instr in instrs)
			{
				int index = graph.IndexOf(instr.Item1);
				ControlFlowBlock block = graph.GetContainingBlock(index);
				SortedList<int, Tuple<Instruction, uint, IMethod>> list;
				bool flag = !blockReferences.TryGetValue(block.Id, out list);
				if (flag)
				{
					list = (blockReferences[block.Id] = new SortedList<int, Tuple<Instruction, uint, IMethod>>());
				}
				list.Add(index, instr);
			}
			for (int i = 0; i < graph.Count; i++)
			{
				ControlFlowBlock block2 = graph[i];
				bool flag2 = blockReferences.ContainsKey(block2.Id);
				if (!flag2)
				{
					ReferenceReplacer.InsertEmptyStateUpdate(cfgCtx, block2);
				}
			}
			foreach (KeyValuePair<int, SortedList<int, Tuple<Instruction, uint, IMethod>>> blockRef in blockReferences)
			{
				BlockKey key = sequence[blockRef.Key];
				ReferenceReplacer.CFGState currentState;
				bool flag3 = !cfgCtx.StatesMap.TryGetValue(key.EntryState, out currentState);
				if (flag3)
				{
					Debug.Assert((graph[blockRef.Key].Type & ControlFlowBlockType.Entry) > ControlFlowBlockType.Normal);
					Debug.Assert(key.Type == BlockKeyType.Explicit);
					uint blockSeed = ctx.Random.NextUInt32();
					currentState = new ReferenceReplacer.CFGState(blockSeed);
					cfgCtx.StatesMap[key.EntryState] = currentState;
					int index2 = graph.Body.Instructions.IndexOf(graph[blockRef.Key].Header);
					Instruction newHeader;
					method.Body.Instructions.Insert(index2++, newHeader = Instruction.Create(OpCodes.Ldloca, cfgCtx.StateVariable));
					method.Body.Instructions.Insert(index2++, Instruction.Create(OpCodes.Ldc_I4, (int)blockSeed));
					method.Body.Instructions.Insert(index2++, Instruction.Create(OpCodes.Call, ctx.CfgCtxCtor));
					method.Body.ReplaceReference(graph[blockRef.Key].Header, newHeader);
					key.Type = BlockKeyType.Incremental;
				}
				BlockKeyType type = key.Type;
				for (int j = 0; j < blockRef.Value.Count; j++)
				{
					Tuple<Instruction, uint, IMethod> refEntry = blockRef.Value.Values[j];
					ReferenceReplacer.CFGState? targetState = null;
					bool flag4 = j == blockRef.Value.Count - 1;
					if (flag4)
					{
						ReferenceReplacer.CFGState exitState;
						bool flag5 = cfgCtx.StatesMap.TryGetValue(key.ExitState, out exitState);
						if (flag5)
						{
							targetState = new ReferenceReplacer.CFGState?(exitState);
						}
					}
					int index3 = graph.Body.Instructions.IndexOf(refEntry.Item1) + 1;
					uint value = ReferenceReplacer.InsertStateGetAndUpdate(cfgCtx, ref index3, type, ref currentState, targetState);
					refEntry.Item1.OpCode = OpCodes.Ldc_I4;
					refEntry.Item1.Operand = (int)(refEntry.Item2 ^ value);
					method.Body.Instructions.Insert(index3++, Instruction.Create(OpCodes.Xor));
					method.Body.Instructions.Insert(index3, Instruction.Create(OpCodes.Call, refEntry.Item3));
					bool flag6 = j == blockRef.Value.Count - 1 && targetState == null;
					if (flag6)
					{
						cfgCtx.StatesMap[key.ExitState] = currentState;
					}
					type = BlockKeyType.Incremental;
				}
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00004A68 File Offset: 0x00002C68
		public ReferenceReplacer()
		{
		}

		// Token: 0x0200008B RID: 139
		private struct CFGContext
		{
			// Token: 0x04000174 RID: 372
			public CEContext Ctx;

			// Token: 0x04000175 RID: 373
			public ControlFlowGraph Graph;

			// Token: 0x04000176 RID: 374
			public BlockKey[] Keys;

			// Token: 0x04000177 RID: 375
			public RandomGenerator Random;

			// Token: 0x04000178 RID: 376
			public Dictionary<uint, ReferenceReplacer.CFGState> StatesMap;

			// Token: 0x04000179 RID: 377
			public Local StateVariable;
		}

		// Token: 0x0200008C RID: 140
		private struct CFGState
		{
			// Token: 0x06000227 RID: 551 RVA: 0x0001294C File Offset: 0x00010B4C
			public CFGState(uint seed)
			{
				seed = (this.A = seed * 557916961U);
				seed = (this.B = seed * 557916961U);
				seed = (this.C = seed * 557916961U);
				seed = (this.D = seed * 557916961U);
			}

			// Token: 0x06000228 RID: 552 RVA: 0x0001299C File Offset: 0x00010B9C
			public void UpdateExplicit(int id, uint value)
			{
				switch (id)
				{
				case 0:
					this.A = value;
					break;
				case 1:
					this.B = value;
					break;
				case 2:
					this.C = value;
					break;
				case 3:
					this.D = value;
					break;
				}
			}

			// Token: 0x06000229 RID: 553 RVA: 0x000129E8 File Offset: 0x00010BE8
			public void UpdateIncremental(int id, uint value)
			{
				switch (id)
				{
				case 0:
					this.A *= value;
					break;
				case 1:
					this.B += value;
					break;
				case 2:
					this.C ^= value;
					break;
				case 3:
					this.D -= value;
					break;
				}
			}

			// Token: 0x0600022A RID: 554 RVA: 0x00012A50 File Offset: 0x00010C50
			public uint GetIncrementalUpdate(int id, uint target)
			{
				uint result;
				switch (id)
				{
				case 0:
					result = (this.A ^ target);
					break;
				case 1:
					result = target - this.B;
					break;
				case 2:
					result = (this.C ^ target);
					break;
				case 3:
					result = this.D - target;
					break;
				default:
					throw new UnreachableException();
				}
				return result;
			}

			// Token: 0x0600022B RID: 555 RVA: 0x00012AAC File Offset: 0x00010CAC
			public uint Get(int id)
			{
				uint result;
				switch (id)
				{
				case 0:
					result = this.A;
					break;
				case 1:
					result = this.B;
					break;
				case 2:
					result = this.C;
					break;
				case 3:
					result = this.D;
					break;
				default:
					throw new UnreachableException();
				}
				return result;
			}

			// Token: 0x0600022C RID: 556 RVA: 0x00012B00 File Offset: 0x00010D00
			public static byte EncodeFlag(bool exp, int updateId, int getId)
			{
				byte fl = exp ? 128 : 0;
				fl |= (byte)updateId;
				return fl | (byte)(getId << 2);
			}

			// Token: 0x0400017A RID: 378
			public uint A;

			// Token: 0x0400017B RID: 379
			public uint B;

			// Token: 0x0400017C RID: 380
			public uint C;

			// Token: 0x0400017D RID: 381
			public uint D;
		}
	}
}
