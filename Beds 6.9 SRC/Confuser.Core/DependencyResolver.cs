using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Confuser.Core
{
	// Token: 0x0200003B RID: 59
	internal class DependencyResolver
	{
		// Token: 0x0600014A RID: 330 RVA: 0x0000289A File Offset: 0x00000A9A
		public DependencyResolver(IEnumerable<Protection> protections)
		{
			this.protections = (from prot in protections
			orderby prot.FullId
			select prot).ToList<Protection>();
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000C8B0 File Offset: 0x0000AAB0
		public IList<Protection> SortDependency()
		{
			List<DependencyResolver.DependencyGraphEdge> edges = new List<DependencyResolver.DependencyGraphEdge>();
			HashSet<Protection> roots = new HashSet<Protection>(this.protections);
			Dictionary<string, Protection> id2prot = this.protections.ToDictionary((Protection prot) => prot.FullId, (Protection prot) => prot);
			Func<string, Protection> <>9__2;
			Func<string, Protection> <>9__3;
			foreach (Protection prot2 in this.protections)
			{
				Type protType = prot2.GetType();
				BeforeProtectionAttribute before = protType.GetCustomAttributes(typeof(BeforeProtectionAttribute), false).Cast<BeforeProtectionAttribute>().SingleOrDefault<BeforeProtectionAttribute>();
				bool flag = before != null;
				if (flag)
				{
					IEnumerable<string> ids = before.Ids;
					Func<string, Protection> selector;
					if ((selector = <>9__2) == null)
					{
						selector = (<>9__2 = ((string id) => id2prot[id]));
					}
					IEnumerable<Protection> targets = ids.Select(selector);
					foreach (Protection target in targets)
					{
						edges.Add(new DependencyResolver.DependencyGraphEdge(prot2, target));
						roots.Remove(target);
					}
				}
				AfterProtectionAttribute after = protType.GetCustomAttributes(typeof(AfterProtectionAttribute), false).Cast<AfterProtectionAttribute>().SingleOrDefault<AfterProtectionAttribute>();
				bool flag2 = after != null;
				if (flag2)
				{
					IEnumerable<string> ids2 = after.Ids;
					Func<string, Protection> selector2;
					if ((selector2 = <>9__3) == null)
					{
						selector2 = (<>9__3 = ((string id) => id2prot[id]));
					}
					IEnumerable<Protection> targets2 = ids2.Select(selector2);
					foreach (Protection target2 in targets2)
					{
						edges.Add(new DependencyResolver.DependencyGraphEdge(target2, prot2));
						roots.Remove(prot2);
					}
				}
			}
			IEnumerable<Protection> sorted = this.SortGraph(roots, edges);
			return sorted.ToList<Protection>();
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000028D4 File Offset: 0x00000AD4
		private IEnumerable<Protection> SortGraph(IEnumerable<Protection> roots, IList<DependencyResolver.DependencyGraphEdge> edges)
		{
			Queue<Protection> queue = new Queue<Protection>(from prot in roots
			orderby prot.FullId
			select prot);
			while (queue.Count > 0)
			{
				DependencyResolver.<>c__DisplayClass3_0 CS$<>8__locals1 = new DependencyResolver.<>c__DisplayClass3_0();
				CS$<>8__locals1.root = queue.Dequeue();
				Debug.Assert(!(from edge in edges
				where edge.To == CS$<>8__locals1.root
				select edge).Any<DependencyResolver.DependencyGraphEdge>());
				yield return CS$<>8__locals1.root;
				Func<DependencyResolver.DependencyGraphEdge, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__2) == null)
				{
					predicate = (CS$<>8__locals1.<>9__2 = ((DependencyResolver.DependencyGraphEdge edge) => edge.From == CS$<>8__locals1.root));
				}
				using (List<DependencyResolver.DependencyGraphEdge>.Enumerator enumerator = edges.Where(predicate).ToList<DependencyResolver.DependencyGraphEdge>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DependencyResolver.<>c__DisplayClass3_1 CS$<>8__locals2 = new DependencyResolver.<>c__DisplayClass3_1();
						CS$<>8__locals2.edge = enumerator.Current;
						edges.Remove(CS$<>8__locals2.edge);
						bool flag = !edges.Any((DependencyResolver.DependencyGraphEdge e) => e.To == CS$<>8__locals2.edge.To);
						if (flag)
						{
							queue.Enqueue(CS$<>8__locals2.edge.To);
						}
						CS$<>8__locals2 = null;
					}
				}
				List<DependencyResolver.DependencyGraphEdge>.Enumerator enumerator = default(List<DependencyResolver.DependencyGraphEdge>.Enumerator);
				CS$<>8__locals1 = null;
			}
			bool flag2 = edges.Count != 0;
			if (flag2)
			{
				throw new CircularDependencyException(edges[0].From, edges[0].To);
			}
			yield break;
		}

		// Token: 0x04000141 RID: 321
		private readonly List<Protection> protections;

		// Token: 0x0200003C RID: 60
		private class DependencyGraphEdge
		{
			// Token: 0x0600014D RID: 333 RVA: 0x000028F2 File Offset: 0x00000AF2
			public DependencyGraphEdge(Protection from, Protection to)
			{
				this.From = from;
				this.To = to;
			}

			// Token: 0x17000011 RID: 17
			// (get) Token: 0x0600014E RID: 334 RVA: 0x0000290C File Offset: 0x00000B0C
			// (set) Token: 0x0600014F RID: 335 RVA: 0x00002914 File Offset: 0x00000B14
			public Protection From
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

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x06000150 RID: 336 RVA: 0x0000291D File Offset: 0x00000B1D
			// (set) Token: 0x06000151 RID: 337 RVA: 0x00002925 File Offset: 0x00000B25
			public Protection To
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

			// Token: 0x04000142 RID: 322
			[CompilerGenerated]
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private Protection <From>k__BackingField;

			// Token: 0x04000143 RID: 323
			[CompilerGenerated]
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private Protection <To>k__BackingField;
		}

		// Token: 0x0200003D RID: 61
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000152 RID: 338 RVA: 0x0000292E File Offset: 0x00000B2E
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000153 RID: 339 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x06000154 RID: 340 RVA: 0x0000293A File Offset: 0x00000B3A
			internal string <.ctor>b__1_0(Protection prot)
			{
				return prot.FullId;
			}

			// Token: 0x06000155 RID: 341 RVA: 0x0000293A File Offset: 0x00000B3A
			internal string <SortDependency>b__2_0(Protection prot)
			{
				return prot.FullId;
			}

			// Token: 0x06000156 RID: 342 RVA: 0x00002942 File Offset: 0x00000B42
			internal Protection <SortDependency>b__2_1(Protection prot)
			{
				return prot;
			}

			// Token: 0x06000157 RID: 343 RVA: 0x0000293A File Offset: 0x00000B3A
			internal string <SortGraph>b__3_0(Protection prot)
			{
				return prot.FullId;
			}

			// Token: 0x04000144 RID: 324
			public static readonly DependencyResolver.<>c <>9 = new DependencyResolver.<>c();

			// Token: 0x04000145 RID: 325
			public static Func<Protection, string> <>9__1_0;

			// Token: 0x04000146 RID: 326
			public static Func<Protection, string> <>9__2_0;

			// Token: 0x04000147 RID: 327
			public static Func<Protection, Protection> <>9__2_1;

			// Token: 0x04000148 RID: 328
			public static Func<Protection, string> <>9__3_0;
		}

		// Token: 0x0200003E RID: 62
		[CompilerGenerated]
		private sealed class <>c__DisplayClass2_0
		{
			// Token: 0x06000158 RID: 344 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass2_0()
			{
			}

			// Token: 0x06000159 RID: 345 RVA: 0x00002945 File Offset: 0x00000B45
			internal Protection <SortDependency>b__2(string id)
			{
				return this.id2prot[id];
			}

			// Token: 0x0600015A RID: 346 RVA: 0x00002945 File Offset: 0x00000B45
			internal Protection <SortDependency>b__3(string id)
			{
				return this.id2prot[id];
			}

			// Token: 0x04000149 RID: 329
			public Dictionary<string, Protection> id2prot;

			// Token: 0x0400014A RID: 330
			public Func<string, Protection> <>9__2;

			// Token: 0x0400014B RID: 331
			public Func<string, Protection> <>9__3;
		}

		// Token: 0x0200003F RID: 63
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_0
		{
			// Token: 0x0600015B RID: 347 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass3_0()
			{
			}

			// Token: 0x0600015C RID: 348 RVA: 0x00002953 File Offset: 0x00000B53
			internal bool <SortGraph>b__1(DependencyResolver.DependencyGraphEdge edge)
			{
				return edge.To == this.root;
			}

			// Token: 0x0600015D RID: 349 RVA: 0x00002963 File Offset: 0x00000B63
			internal bool <SortGraph>b__2(DependencyResolver.DependencyGraphEdge edge)
			{
				return edge.From == this.root;
			}

			// Token: 0x0400014C RID: 332
			public Protection root;

			// Token: 0x0400014D RID: 333
			public Func<DependencyResolver.DependencyGraphEdge, bool> <>9__2;
		}

		// Token: 0x02000040 RID: 64
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_1
		{
			// Token: 0x0600015E RID: 350 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass3_1()
			{
			}

			// Token: 0x0600015F RID: 351 RVA: 0x00002973 File Offset: 0x00000B73
			internal bool <SortGraph>b__3(DependencyResolver.DependencyGraphEdge e)
			{
				return e.To == this.edge.To;
			}

			// Token: 0x0400014E RID: 334
			public DependencyResolver.DependencyGraphEdge edge;
		}

		// Token: 0x02000041 RID: 65
		[CompilerGenerated]
		private sealed class <SortGraph>d__3 : IEnumerable<Protection>, IEnumerator<Protection>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x06000160 RID: 352 RVA: 0x00002988 File Offset: 0x00000B88
			[DebuggerHidden]
			public <SortGraph>d__3(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x06000161 RID: 353 RVA: 0x0000280B File Offset: 0x00000A0B
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x06000162 RID: 354 RVA: 0x0000CB10 File Offset: 0x0000AD10
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
					IEnumerable<DependencyResolver.DependencyGraphEdge> source = edges;
					Func<DependencyResolver.DependencyGraphEdge, bool> predicate;
					if ((predicate = CS$<>8__locals1.<>9__2) == null)
					{
						predicate = (CS$<>8__locals1.<>9__2 = new Func<DependencyResolver.DependencyGraphEdge, bool>(CS$<>8__locals1.<SortGraph>b__2));
					}
					enumerator = source.Where(predicate).ToList<DependencyResolver.DependencyGraphEdge>().GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							CS$<>8__locals2 = new DependencyResolver.<>c__DisplayClass3_1();
							CS$<>8__locals2.edge = enumerator.Current;
							edges.Remove(CS$<>8__locals2.edge);
							bool flag = !edges.Any(new Func<DependencyResolver.DependencyGraphEdge, bool>(CS$<>8__locals2.<SortGraph>b__3));
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
					enumerator = default(List<DependencyResolver.DependencyGraphEdge>.Enumerator);
					CS$<>8__locals1 = null;
				}
				else
				{
					this.<>1__state = -1;
					queue = new Queue<Protection>(roots.OrderBy(new Func<Protection, string>(DependencyResolver.<>c.<>9.<SortGraph>b__3_0)));
				}
				if (queue.Count > 0)
				{
					CS$<>8__locals1 = new DependencyResolver.<>c__DisplayClass3_0();
					CS$<>8__locals1.root = queue.Dequeue();
					Debug.Assert(!edges.Where(new Func<DependencyResolver.DependencyGraphEdge, bool>(CS$<>8__locals1.<SortGraph>b__1)).Any<DependencyResolver.DependencyGraphEdge>());
					this.<>2__current = CS$<>8__locals1.root;
					this.<>1__state = 1;
					return true;
				}
				bool flag2 = edges.Count != 0;
				if (flag2)
				{
					throw new CircularDependencyException(edges[0].From, edges[0].To);
				}
				return false;
			}

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x06000163 RID: 355 RVA: 0x000029A8 File Offset: 0x00000BA8
			Protection IEnumerator<Protection>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000164 RID: 356 RVA: 0x0000268B File Offset: 0x0000088B
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x06000165 RID: 357 RVA: 0x000029A8 File Offset: 0x00000BA8
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000166 RID: 358 RVA: 0x0000CD54 File Offset: 0x0000AF54
			[DebuggerHidden]
			IEnumerator<Protection> IEnumerable<Protection>.GetEnumerator()
			{
				DependencyResolver.<SortGraph>d__3 <SortGraph>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<SortGraph>d__ = this;
				}
				else
				{
					<SortGraph>d__ = new DependencyResolver.<SortGraph>d__3(0);
					<SortGraph>d__.<>4__this = this;
				}
				<SortGraph>d__.roots = roots;
				<SortGraph>d__.edges = edges;
				return <SortGraph>d__;
			}

			// Token: 0x06000167 RID: 359 RVA: 0x000029B0 File Offset: 0x00000BB0
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Confuser.Core.Protection>.GetEnumerator();
			}

			// Token: 0x0400014F RID: 335
			private int <>1__state;

			// Token: 0x04000150 RID: 336
			private Protection <>2__current;

			// Token: 0x04000151 RID: 337
			private int <>l__initialThreadId;

			// Token: 0x04000152 RID: 338
			private IEnumerable<Protection> roots;

			// Token: 0x04000153 RID: 339
			public IEnumerable<Protection> <>3__roots;

			// Token: 0x04000154 RID: 340
			private IList<DependencyResolver.DependencyGraphEdge> edges;

			// Token: 0x04000155 RID: 341
			public IList<DependencyResolver.DependencyGraphEdge> <>3__edges;

			// Token: 0x04000156 RID: 342
			public DependencyResolver <>4__this;

			// Token: 0x04000157 RID: 343
			private Queue<Protection> <queue>5__1;

			// Token: 0x04000158 RID: 344
			private DependencyResolver.<>c__DisplayClass3_0 <>8__2;

			// Token: 0x04000159 RID: 345
			private List<DependencyResolver.DependencyGraphEdge>.Enumerator <>s__3;

			// Token: 0x0400015A RID: 346
			private DependencyResolver.<>c__DisplayClass3_1 <>8__4;
		}
	}
}
