using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000D7 RID: 215
	internal abstract class ManglerBase
	{
		// Token: 0x0600034B RID: 843 RVA: 0x0000538C File Offset: 0x0000358C
		protected ManglerBase()
		{
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00005A4E File Offset: 0x00003C4E
		protected static IEnumerable<InstrBlock> GetAllBlocks(ScopeBlock scope)
		{
			foreach (BlockBase iteratorVariable0 in scope.Children)
			{
				bool flag = iteratorVariable0 is InstrBlock;
				if (flag)
				{
					yield return (InstrBlock)iteratorVariable0;
				}
				else
				{
					foreach (InstrBlock iteratorVariable in ManglerBase.GetAllBlocks((ScopeBlock)iteratorVariable0))
					{
						yield return iteratorVariable;
						iteratorVariable = null;
					}
					IEnumerator<InstrBlock> enumerator2 = null;
				}
				iteratorVariable0 = null;
			}
			List<BlockBase>.Enumerator enumerator = default(List<BlockBase>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600034D RID: 845
		public abstract void Mangle(CilBody body, ScopeBlock root, CFContext ctx);

		// Token: 0x020000D8 RID: 216
		[CompilerGenerated]
		private sealed class <GetAllBlocks>d__1 : IEnumerable<InstrBlock>, IEnumerator<InstrBlock>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x0600034E RID: 846 RVA: 0x00005A5E File Offset: 0x00003C5E
			[DebuggerHidden]
			public <GetAllBlocks>d__1(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x0600034F RID: 847 RVA: 0x0001B11C File Offset: 0x0001931C
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				int num = this.<>1__state;
				if (num - -4 <= 1 || num - 1 <= 1)
				{
					try
					{
						if (num == -4 || num == 2)
						{
							try
							{
							}
							finally
							{
								this.<>m__Finally2();
							}
						}
					}
					finally
					{
						this.<>m__Finally1();
					}
				}
			}

			// Token: 0x06000350 RID: 848 RVA: 0x0001B184 File Offset: 0x00019384
			bool IEnumerator.MoveNext()
			{
				bool result;
				try
				{
					switch (this.<>1__state)
					{
					case 0:
						this.<>1__state = -1;
						enumerator = scope.Children.GetEnumerator();
						this.<>1__state = -3;
						goto IL_12A;
					case 1:
						this.<>1__state = -3;
						goto IL_122;
					case 2:
						this.<>1__state = -4;
						iteratorVariable = null;
						break;
					default:
						return false;
					}
					IL_106:
					if (enumerator2.MoveNext())
					{
						iteratorVariable = enumerator2.Current;
						this.<>2__current = iteratorVariable;
						this.<>1__state = 2;
						return true;
					}
					this.<>m__Finally2();
					enumerator2 = null;
					IL_122:
					iteratorVariable0 = null;
					IL_12A:
					if (!enumerator.MoveNext())
					{
						this.<>m__Finally1();
						enumerator = default(List<BlockBase>.Enumerator);
						result = false;
					}
					else
					{
						iteratorVariable0 = enumerator.Current;
						bool flag = iteratorVariable0 is InstrBlock;
						if (!flag)
						{
							enumerator2 = ManglerBase.GetAllBlocks((ScopeBlock)iteratorVariable0).GetEnumerator();
							this.<>1__state = -4;
							goto IL_106;
						}
						this.<>2__current = (InstrBlock)iteratorVariable0;
						this.<>1__state = 1;
						result = true;
					}
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return result;
			}

			// Token: 0x06000351 RID: 849 RVA: 0x00005A7E File Offset: 0x00003C7E
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				((IDisposable)enumerator).Dispose();
			}

			// Token: 0x06000352 RID: 850 RVA: 0x00005A99 File Offset: 0x00003C99
			private void <>m__Finally2()
			{
				this.<>1__state = -3;
				if (enumerator2 != null)
				{
					enumerator2.Dispose();
				}
			}

			// Token: 0x170000AF RID: 175
			// (get) Token: 0x06000353 RID: 851 RVA: 0x00005AB7 File Offset: 0x00003CB7
			InstrBlock IEnumerator<InstrBlock>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000354 RID: 852 RVA: 0x000050B0 File Offset: 0x000032B0
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x170000B0 RID: 176
			// (get) Token: 0x06000355 RID: 853 RVA: 0x00005AB7 File Offset: 0x00003CB7
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000356 RID: 854 RVA: 0x0001B308 File Offset: 0x00019508
			[DebuggerHidden]
			IEnumerator<InstrBlock> IEnumerable<InstrBlock>.GetEnumerator()
			{
				ManglerBase.<GetAllBlocks>d__1 <GetAllBlocks>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<GetAllBlocks>d__ = this;
				}
				else
				{
					<GetAllBlocks>d__ = new ManglerBase.<GetAllBlocks>d__1(0);
				}
				<GetAllBlocks>d__.scope = scope;
				return <GetAllBlocks>d__;
			}

			// Token: 0x06000357 RID: 855 RVA: 0x00005ABF File Offset: 0x00003CBF
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Confuser.Protections.DupCtrlFlowMod.InstrBlock>.GetEnumerator();
			}

			// Token: 0x0400026E RID: 622
			private int <>1__state;

			// Token: 0x0400026F RID: 623
			private InstrBlock <>2__current;

			// Token: 0x04000270 RID: 624
			private int <>l__initialThreadId;

			// Token: 0x04000271 RID: 625
			private ScopeBlock scope;

			// Token: 0x04000272 RID: 626
			public ScopeBlock <>3__scope;

			// Token: 0x04000273 RID: 627
			private List<BlockBase>.Enumerator <>s__1;

			// Token: 0x04000274 RID: 628
			private BlockBase <iteratorVariable0>5__2;

			// Token: 0x04000275 RID: 629
			private IEnumerator<InstrBlock> <>s__3;

			// Token: 0x04000276 RID: 630
			private InstrBlock <iteratorVariable1>5__4;
		}
	}
}
