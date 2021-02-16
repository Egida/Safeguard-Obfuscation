using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers
{
	/// <summary>
	///     Computes a key sequence that is valid according to the execution of the CFG.
	/// </summary>
	/// <remarks>
	///     The caller can utilize the information provided by this classes to instruments state machines.
	///     For example:
	///     <code>
	/// int state = 4;
	/// for (int i = 0 ; i &lt; 10; i++) {
	///     state = 6;
	///     if (i % 2 == 0) {
	///         state = 3;
	///     else {
	///         // The state varaible is guaranteed to be 6 in here.
	///     }
	/// }
	///     </code>
	/// </remarks>
	// Token: 0x020000A9 RID: 169
	public static class KeySequence
	{
		/// <summary>
		///     Computes a key sequence of the given CFG.
		/// </summary>
		/// <param name="graph">The CFG.</param>
		/// <param name="random">The random source, or <c>null</c> if key id is needed.</param>
		/// <returns>The generated key sequence of the CFG.</returns>
		// Token: 0x060003C9 RID: 969 RVA: 0x00016664 File Offset: 0x00014864
		public static BlockKey[] ComputeKeys(ControlFlowGraph graph, RandomGenerator random)
		{
			BlockKey[] keys = new BlockKey[graph.Count];
			foreach (ControlFlowBlock block in ((IEnumerable<ControlFlowBlock>)graph))
			{
				BlockKey key = default(BlockKey);
				bool flag = (block.Type & ControlFlowBlockType.Entry) > ControlFlowBlockType.Normal;
				if (flag)
				{
					key.Type = BlockKeyType.Explicit;
				}
				else
				{
					key.Type = BlockKeyType.Incremental;
				}
				keys[block.Id] = key;
			}
			KeySequence.ProcessBlocks(keys, graph, random);
			return keys;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00016704 File Offset: 0x00014904
		private static void ProcessBlocks(BlockKey[] keys, ControlFlowGraph graph, RandomGenerator random)
		{
			uint id = 0U;
			for (int i = 0; i < keys.Length; i++)
			{
				keys[i].EntryState = id++;
				keys[i].ExitState = id++;
			}
			Dictionary<ExceptionHandler, uint> finallyIds = new Dictionary<ExceptionHandler, uint>();
			Dictionary<ControlFlowBlock, List<ExceptionHandler>> ehMap = new Dictionary<ControlFlowBlock, List<ExceptionHandler>>();
			Func<ControlFlowBlock, uint> <>9__0;
			Func<ControlFlowBlock, uint> <>9__1;
			bool updated;
			do
			{
				updated = false;
				foreach (ControlFlowBlock block in ((IEnumerable<ControlFlowBlock>)graph))
				{
					BlockKey key = keys[block.Id];
					bool flag = block.Sources.Count > 0;
					if (flag)
					{
						IEnumerable<ControlFlowBlock> sources = block.Sources;
						Func<ControlFlowBlock, uint> selector;
						if ((selector = <>9__0) == null)
						{
							selector = (<>9__0 = ((ControlFlowBlock b) => keys[b.Id].ExitState));
						}
						uint newEntry = sources.Select(selector).Max<uint>();
						bool flag2 = key.EntryState != newEntry;
						if (flag2)
						{
							key.EntryState = newEntry;
							updated = true;
						}
					}
					bool flag3 = block.Targets.Count > 0;
					if (flag3)
					{
						IEnumerable<ControlFlowBlock> targets = block.Targets;
						Func<ControlFlowBlock, uint> selector2;
						if ((selector2 = <>9__1) == null)
						{
							selector2 = (<>9__1 = ((ControlFlowBlock b) => keys[b.Id].EntryState));
						}
						uint newExit = targets.Select(selector2).Max<uint>();
						bool flag4 = key.ExitState != newExit;
						if (flag4)
						{
							key.ExitState = newExit;
							updated = true;
						}
					}
					bool flag5 = block.Footer.OpCode.Code == Code.Endfilter || block.Footer.OpCode.Code == Code.Endfinally;
					if (flag5)
					{
						List<ExceptionHandler> ehs;
						bool flag6 = !ehMap.TryGetValue(block, out ehs);
						if (flag6)
						{
							ehs = new List<ExceptionHandler>();
							int footerIndex = graph.IndexOf(block.Footer);
							foreach (ExceptionHandler eh in graph.Body.ExceptionHandlers)
							{
								bool flag7 = eh.FilterStart != null && block.Footer.OpCode.Code == Code.Endfilter;
								if (flag7)
								{
									bool flag8 = footerIndex >= graph.IndexOf(eh.FilterStart) && footerIndex < graph.IndexOf(eh.HandlerStart);
									if (flag8)
									{
										ehs.Add(eh);
									}
								}
								else
								{
									bool flag9 = (eh.HandlerType == ExceptionHandlerType.Finally || eh.HandlerType == ExceptionHandlerType.Fault) && footerIndex >= graph.IndexOf(eh.HandlerStart) && (eh.HandlerEnd == null || footerIndex < graph.IndexOf(eh.HandlerEnd));
									if (flag9)
									{
										ehs.Add(eh);
									}
								}
							}
							ehMap[block] = ehs;
						}
						using (List<ExceptionHandler>.Enumerator enumerator3 = ehs.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								ExceptionHandler eh2 = enumerator3.Current;
								uint ehVal;
								bool flag10 = finallyIds.TryGetValue(eh2, out ehVal);
								if (flag10)
								{
									bool flag11 = key.ExitState > ehVal;
									if (flag11)
									{
										finallyIds[eh2] = key.ExitState;
										updated = true;
									}
									else
									{
										bool flag12 = key.ExitState < ehVal;
										if (flag12)
										{
											key.ExitState = ehVal;
											updated = true;
										}
									}
								}
								else
								{
									finallyIds[eh2] = key.ExitState;
									updated = true;
								}
							}
							goto IL_38D;
						}
						goto IL_38B;
					}
					goto IL_38B;
					IL_38D:
					keys[block.Id] = key;
					continue;
					IL_38B:
					bool flag13 = block.Footer.OpCode.Code != Code.Leave && block.Footer.OpCode.Code != Code.Leave_S;
					if (flag13)
					{
						goto IL_38D;
					}
					List<ExceptionHandler> ehs2;
					bool flag14 = !ehMap.TryGetValue(block, out ehs2);
					if (flag14)
					{
						ehs2 = new List<ExceptionHandler>();
						int footerIndex2 = graph.IndexOf(block.Footer);
						foreach (ExceptionHandler eh3 in graph.Body.ExceptionHandlers)
						{
							bool flag15 = footerIndex2 >= graph.IndexOf(eh3.TryStart) && (eh3.TryEnd == null || footerIndex2 < graph.IndexOf(eh3.TryEnd));
							if (flag15)
							{
								ehs2.Add(eh3);
							}
						}
						ehMap[block] = ehs2;
					}
					uint? maxVal = null;
					foreach (ExceptionHandler eh4 in ehs2)
					{
						uint ehVal2;
						bool flag16 = finallyIds.TryGetValue(eh4, out ehVal2) && (maxVal == null || ehVal2 > maxVal);
						if (flag16)
						{
							bool flag17 = maxVal != null;
							if (flag17)
							{
								updated = true;
							}
							maxVal = new uint?(ehVal2);
						}
					}
					bool flag18 = maxVal != null;
					if (flag18)
					{
						bool flag19 = key.ExitState > maxVal.Value;
						if (flag19)
						{
							maxVal = new uint?(key.ExitState);
							updated = true;
						}
						else
						{
							bool flag20 = key.ExitState < maxVal.Value;
							if (flag20)
							{
								key.ExitState = maxVal.Value;
								updated = true;
							}
						}
						foreach (ExceptionHandler eh5 in ehs2)
						{
							finallyIds[eh5] = maxVal.Value;
						}
						goto IL_38D;
					}
					goto IL_38D;
				}
			}
			while (updated);
			bool flag21 = random != null;
			if (flag21)
			{
				Dictionary<uint, uint> idMap = new Dictionary<uint, uint>();
				for (int j = 0; j < keys.Length; j++)
				{
					BlockKey key2 = keys[j];
					uint entryId = key2.EntryState;
					bool flag22 = !idMap.TryGetValue(entryId, out key2.EntryState);
					if (flag22)
					{
						key2.EntryState = (idMap[entryId] = random.NextUInt32());
					}
					uint exitId = key2.ExitState;
					bool flag23 = !idMap.TryGetValue(exitId, out key2.ExitState);
					if (flag23)
					{
						key2.ExitState = (idMap[exitId] = random.NextUInt32());
					}
					keys[j] = key2;
				}
			}
		}

		// Token: 0x020000AA RID: 170
		[CompilerGenerated]
		private sealed class <>c__DisplayClass1_0
		{
			// Token: 0x060003CB RID: 971 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass1_0()
			{
			}

			// Token: 0x060003CC RID: 972 RVA: 0x0000392F File Offset: 0x00001B2F
			internal uint <ProcessBlocks>b__0(ControlFlowBlock b)
			{
				return this.keys[b.Id].ExitState;
			}

			// Token: 0x060003CD RID: 973 RVA: 0x00003947 File Offset: 0x00001B47
			internal uint <ProcessBlocks>b__1(ControlFlowBlock b)
			{
				return this.keys[b.Id].EntryState;
			}

			// Token: 0x04000286 RID: 646
			public BlockKey[] keys;

			// Token: 0x04000287 RID: 647
			public Func<ControlFlowBlock, uint> <>9__0;

			// Token: 0x04000288 RID: 648
			public Func<ControlFlowBlock, uint> <>9__1;
		}
	}
}
