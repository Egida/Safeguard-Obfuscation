using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Sort modules according dependencies.
	/// </summary>
	// Token: 0x02000032 RID: 50
	internal class ModuleSorter
	{
		// Token: 0x0600011A RID: 282 RVA: 0x0000271D File Offset: 0x0000091D
		public ModuleSorter(IEnumerable<ModuleDefMD> modules)
		{
			this.modules = modules.ToList<ModuleDefMD>();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000C218 File Offset: 0x0000A418
		public IList<ModuleDefMD> Sort()
		{
			List<ModuleSorter.DependencyGraphEdge> edges = new List<ModuleSorter.DependencyGraphEdge>();
			HashSet<ModuleDefMD> roots = new HashSet<ModuleDefMD>(this.modules);
			Dictionary<IAssembly, List<ModuleDefMD>> asmMap = this.modules.GroupBy((ModuleDefMD module) => module.Assembly.ToAssemblyRef(), AssemblyNameComparer.CompareAll).ToDictionary((IGrouping<IAssembly, ModuleDefMD> gp) => gp.Key, (IGrouping<IAssembly, ModuleDefMD> gp) => gp.ToList<ModuleDefMD>(), AssemblyNameComparer.CompareAll);
			foreach (ModuleDefMD i in this.modules)
			{
				foreach (AssemblyRef nameRef in i.GetAssemblyRefs())
				{
					bool flag = !asmMap.ContainsKey(nameRef);
					if (!flag)
					{
						foreach (ModuleDefMD asmModule in asmMap[nameRef])
						{
							edges.Add(new ModuleSorter.DependencyGraphEdge(asmModule, i));
						}
						roots.Remove(i);
					}
				}
			}
			List<ModuleDefMD> sorted = this.SortGraph(roots, edges).ToList<ModuleDefMD>();
			Debug.Assert(sorted.Count == this.modules.Count);
			return sorted;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00002733 File Offset: 0x00000933
		private IEnumerable<ModuleDefMD> SortGraph(IEnumerable<ModuleDefMD> roots, IList<ModuleSorter.DependencyGraphEdge> edges)
		{
			HashSet<ModuleDefMD> visited = new HashSet<ModuleDefMD>();
			Queue<ModuleDefMD> queue = new Queue<ModuleDefMD>(roots);
			do
			{
				while (queue.Count > 0)
				{
					ModuleSorter.<>c__DisplayClass3_0 CS$<>8__locals1 = new ModuleSorter.<>c__DisplayClass3_0();
					CS$<>8__locals1.node = queue.Dequeue();
					visited.Add(CS$<>8__locals1.node);
					Debug.Assert(!(from edge in edges
					where edge.To == CS$<>8__locals1.node
					select edge).Any<ModuleSorter.DependencyGraphEdge>());
					yield return CS$<>8__locals1.node;
					Func<ModuleSorter.DependencyGraphEdge, bool> predicate;
					if ((predicate = CS$<>8__locals1.<>9__1) == null)
					{
						predicate = (CS$<>8__locals1.<>9__1 = ((ModuleSorter.DependencyGraphEdge edge) => edge.From == CS$<>8__locals1.node));
					}
					using (List<ModuleSorter.DependencyGraphEdge>.Enumerator enumerator = edges.Where(predicate).ToList<ModuleSorter.DependencyGraphEdge>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ModuleSorter.<>c__DisplayClass3_1 CS$<>8__locals2 = new ModuleSorter.<>c__DisplayClass3_1();
							CS$<>8__locals2.edge = enumerator.Current;
							edges.Remove(CS$<>8__locals2.edge);
							bool flag = !edges.Any((ModuleSorter.DependencyGraphEdge e) => e.To == CS$<>8__locals2.edge.To);
							if (flag)
							{
								queue.Enqueue(CS$<>8__locals2.edge.To);
							}
							CS$<>8__locals2 = null;
						}
					}
					List<ModuleSorter.DependencyGraphEdge>.Enumerator enumerator = default(List<ModuleSorter.DependencyGraphEdge>.Enumerator);
					CS$<>8__locals1 = null;
				}
				bool flag2 = edges.Count > 0;
				if (flag2)
				{
					foreach (ModuleSorter.DependencyGraphEdge edge2 in edges)
					{
						bool flag3 = !visited.Contains(edge2.From);
						if (flag3)
						{
							queue.Enqueue(edge2.From);
							break;
						}
						edge2 = null;
					}
					IEnumerator<ModuleSorter.DependencyGraphEdge> enumerator2 = null;
				}
			}
			while (edges.Count > 0);
			yield break;
		}

		// Token: 0x0400011B RID: 283
		private readonly List<ModuleDefMD> modules;

		// Token: 0x02000033 RID: 51
		private class DependencyGraphEdge
		{
			// Token: 0x0600011D RID: 285 RVA: 0x00002751 File Offset: 0x00000951
			public DependencyGraphEdge(ModuleDefMD from, ModuleDefMD to)
			{
				this.From = from;
				this.To = to;
			}

			// Token: 0x17000004 RID: 4
			// (get) Token: 0x0600011E RID: 286 RVA: 0x0000276B File Offset: 0x0000096B
			// (set) Token: 0x0600011F RID: 287 RVA: 0x00002773 File Offset: 0x00000973
			public ModuleDefMD From
			{
				[CompilerGenerated]
				get
				{
					return this.<From>k__BackingField;
				}
				[CompilerGenerated]
				private set
				{
					this.<From>k__BackingField = value;
				}
			}

			// Token: 0x17000005 RID: 5
			// (get) Token: 0x06000120 RID: 288 RVA: 0x0000277C File Offset: 0x0000097C
			// (set) Token: 0x06000121 RID: 289 RVA: 0x00002784 File Offset: 0x00000984
			public ModuleDefMD To
			{
				[CompilerGenerated]
				get
				{
					return this.<To>k__BackingField;
				}
				[CompilerGenerated]
				private set
				{
					this.<To>k__BackingField = value;
				}
			}

			// Token: 0x0400011C RID: 284
			[CompilerGenerated]
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private ModuleDefMD <From>k__BackingField;

			// Token: 0x0400011D RID: 285
			[CompilerGenerated]
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private ModuleDefMD <To>k__BackingField;
		}

		// Token: 0x02000034 RID: 52
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000122 RID: 290 RVA: 0x0000278D File Offset: 0x0000098D
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000123 RID: 291 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x06000124 RID: 292 RVA: 0x00002799 File Offset: 0x00000999
			internal IAssembly <Sort>b__2_0(ModuleDefMD module)
			{
				return module.Assembly.ToAssemblyRef();
			}

			// Token: 0x06000125 RID: 293 RVA: 0x000027A6 File Offset: 0x000009A6
			internal IAssembly <Sort>b__2_1(IGrouping<IAssembly, ModuleDefMD> gp)
			{
				return gp.Key;
			}

			// Token: 0x06000126 RID: 294 RVA: 0x000027AE File Offset: 0x000009AE
			internal List<ModuleDefMD> <Sort>b__2_2(IGrouping<IAssembly, ModuleDefMD> gp)
			{
				return gp.ToList<ModuleDefMD>();
			}

			// Token: 0x0400011E RID: 286
			public static readonly ModuleSorter.<>c <>9 = new ModuleSorter.<>c();

			// Token: 0x0400011F RID: 287
			public static Func<ModuleDefMD, IAssembly> <>9__2_0;

			// Token: 0x04000120 RID: 288
			public static Func<IGrouping<IAssembly, ModuleDefMD>, IAssembly> <>9__2_1;

			// Token: 0x04000121 RID: 289
			public static Func<IGrouping<IAssembly, ModuleDefMD>, List<ModuleDefMD>> <>9__2_2;
		}

		// Token: 0x02000035 RID: 53
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_0
		{
			// Token: 0x06000127 RID: 295 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass3_0()
			{
			}

			// Token: 0x06000128 RID: 296 RVA: 0x000027B6 File Offset: 0x000009B6
			internal bool <SortGraph>b__0(ModuleSorter.DependencyGraphEdge edge)
			{
				return edge.To == this.node;
			}

			// Token: 0x06000129 RID: 297 RVA: 0x000027C6 File Offset: 0x000009C6
			internal bool <SortGraph>b__1(ModuleSorter.DependencyGraphEdge edge)
			{
				return edge.From == this.node;
			}

			// Token: 0x04000122 RID: 290
			public ModuleDefMD node;

			// Token: 0x04000123 RID: 291
			public Func<ModuleSorter.DependencyGraphEdge, bool> <>9__1;
		}

		// Token: 0x02000036 RID: 54
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_1
		{
			// Token: 0x0600012A RID: 298 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass3_1()
			{
			}

			// Token: 0x0600012B RID: 299 RVA: 0x000027D6 File Offset: 0x000009D6
			internal bool <SortGraph>b__2(ModuleSorter.DependencyGraphEdge e)
			{
				return e.To == this.edge.To;
			}

			// Token: 0x04000124 RID: 292
			public ModuleSorter.DependencyGraphEdge edge;
		}

		// Token: 0x02000037 RID: 55
		[CompilerGenerated]
		private sealed class <SortGraph>d__3 : IEnumerable<ModuleDefMD>, IEnumerator<ModuleDefMD>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x0600012C RID: 300 RVA: 0x000027EB File Offset: 0x000009EB
			[DebuggerHidden]
			public <SortGraph>d__3(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x0600012D RID: 301 RVA: 0x0000280B File Offset: 0x00000A0B
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x0600012E RID: 302 RVA: 0x0000C3E0 File Offset: 0x0000A5E0
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
					IEnumerable<ModuleSorter.DependencyGraphEdge> source = edges;
					Func<ModuleSorter.DependencyGraphEdge, bool> predicate;
					if ((predicate = CS$<>8__locals1.<>9__1) == null)
					{
						predicate = (CS$<>8__locals1.<>9__1 = new Func<ModuleSorter.DependencyGraphEdge, bool>(CS$<>8__locals1.<SortGraph>b__1));
					}
					enumerator = source.Where(predicate).ToList<ModuleSorter.DependencyGraphEdge>().GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							CS$<>8__locals2 = new ModuleSorter.<>c__DisplayClass3_1();
							CS$<>8__locals2.edge = enumerator.Current;
							edges.Remove(CS$<>8__locals2.edge);
							bool flag = !edges.Any(new Func<ModuleSorter.DependencyGraphEdge, bool>(CS$<>8__locals2.<SortGraph>b__2));
							if (flag)
							{
								queue.Enqueue(CS$<>8__locals2.edge.To);
							}
							CS$<>8__locals2 = null;
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					enumerator = default(List<ModuleSorter.DependencyGraphEdge>.Enumerator);
					CS$<>8__locals1 = null;
				}
				else
				{
					this.<>1__state = -1;
					visited = new HashSet<ModuleDefMD>();
					queue = new Queue<ModuleDefMD>(roots);
				}
				IL_3F:
				if (queue.Count > 0)
				{
					CS$<>8__locals1 = new ModuleSorter.<>c__DisplayClass3_0();
					CS$<>8__locals1.node = queue.Dequeue();
					visited.Add(CS$<>8__locals1.node);
					Debug.Assert(!edges.Where(new Func<ModuleSorter.DependencyGraphEdge, bool>(CS$<>8__locals1.<SortGraph>b__0)).Any<ModuleSorter.DependencyGraphEdge>());
					this.<>2__current = CS$<>8__locals1.node;
					this.<>1__state = 1;
					return true;
				}
				bool flag2 = edges.Count > 0;
				if (flag2)
				{
					enumerator2 = edges.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							edge = enumerator2.Current;
							bool flag3 = !visited.Contains(edge.From);
							if (flag3)
							{
								queue.Enqueue(edge.From);
								break;
							}
							edge = null;
						}
					}
					finally
					{
						if (enumerator2 != null)
						{
							enumerator2.Dispose();
						}
					}
					enumerator2 = null;
				}
				if (edges.Count <= 0)
				{
					return false;
				}
				goto IL_3F;
			}

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x0600012F RID: 303 RVA: 0x0000280D File Offset: 0x00000A0D
			ModuleDefMD IEnumerator<ModuleDefMD>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000130 RID: 304 RVA: 0x0000268B File Offset: 0x0000088B
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000131 RID: 305 RVA: 0x0000280D File Offset: 0x00000A0D
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000132 RID: 306 RVA: 0x0000C6B4 File Offset: 0x0000A8B4
			[DebuggerHidden]
			IEnumerator<ModuleDefMD> IEnumerable<ModuleDefMD>.GetEnumerator()
			{
				ModuleSorter.<SortGraph>d__3 <SortGraph>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<SortGraph>d__ = this;
				}
				else
				{
					<SortGraph>d__ = new ModuleSorter.<SortGraph>d__3(0);
					<SortGraph>d__.<>4__this = this;
				}
				<SortGraph>d__.roots = roots;
				<SortGraph>d__.edges = edges;
				return <SortGraph>d__;
			}

			// Token: 0x06000133 RID: 307 RVA: 0x00002815 File Offset: 0x00000A15
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<dnlib.DotNet.ModuleDefMD>.GetEnumerator();
			}

			// Token: 0x04000125 RID: 293
			private int <>1__state;

			// Token: 0x04000126 RID: 294
			private ModuleDefMD <>2__current;

			// Token: 0x04000127 RID: 295
			private int <>l__initialThreadId;

			// Token: 0x04000128 RID: 296
			private IEnumerable<ModuleDefMD> roots;

			// Token: 0x04000129 RID: 297
			public IEnumerable<ModuleDefMD> <>3__roots;

			// Token: 0x0400012A RID: 298
			private IList<ModuleSorter.DependencyGraphEdge> edges;

			// Token: 0x0400012B RID: 299
			public IList<ModuleSorter.DependencyGraphEdge> <>3__edges;

			// Token: 0x0400012C RID: 300
			public ModuleSorter <>4__this;

			// Token: 0x0400012D RID: 301
			private HashSet<ModuleDefMD> <visited>5__1;

			// Token: 0x0400012E RID: 302
			private Queue<ModuleDefMD> <queue>5__2;

			// Token: 0x0400012F RID: 303
			private ModuleSorter.<>c__DisplayClass3_0 <>8__3;

			// Token: 0x04000130 RID: 304
			private List<ModuleSorter.DependencyGraphEdge>.Enumerator <>s__4;

			// Token: 0x04000131 RID: 305
			private ModuleSorter.<>c__DisplayClass3_1 <>8__5;

			// Token: 0x04000132 RID: 306
			private IEnumerator<ModuleSorter.DependencyGraphEdge> <>s__6;

			// Token: 0x04000133 RID: 307
			private ModuleSorter.DependencyGraphEdge <edge>5__7;
		}
	}
}
