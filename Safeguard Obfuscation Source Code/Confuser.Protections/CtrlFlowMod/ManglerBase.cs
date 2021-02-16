using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000C2 RID: 194
	internal abstract class ManglerBase
	{
		// Token: 0x060002FA RID: 762 RVA: 0x0000538C File Offset: 0x0000358C
		protected ManglerBase()
		{
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00005835 File Offset: 0x00003A35
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

		// Token: 0x060002FC RID: 764
		public abstract void Mangle(CilBody body, ScopeBlock root, CFContext ctx);

		// Token: 0x020000C3 RID: 195
		[CompilerGenerated]
		private sealed class <GetAllBlocks>d__1 : IEnumerable<InstrBlock>, IEnumerator<InstrBlock>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x060002FD RID: 765 RVA: 0x00005845 File Offset: 0x00003A45
			[DebuggerHidden]
			public <GetAllBlocks>d__1(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x060002FE RID: 766 RVA: 0x00018E88 File Offset: 0x00017088
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

			// Token: 0x060002FF RID: 767 RVA: 0x00018EF0 File Offset: 0x000170F0
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

			// Token: 0x06000300 RID: 768 RVA: 0x00005865 File Offset: 0x00003A65
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				((IDisposable)enumerator).Dispose();
			}

			// Token: 0x06000301 RID: 769 RVA: 0x00005880 File Offset: 0x00003A80
			private void <>m__Finally2()
			{
				this.<>1__state = -3;
				if (enumerator2 != null)
				{
					enumerator2.Dispose();
				}
			}

			// Token: 0x170000A6 RID: 166
			// (get) Token: 0x06000302 RID: 770 RVA: 0x0000589E File Offset: 0x00003A9E
			InstrBlock IEnumerator<InstrBlock>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000303 RID: 771 RVA: 0x000050B0 File Offset: 0x000032B0
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x170000A7 RID: 167
			// (get) Token: 0x06000304 RID: 772 RVA: 0x0000589E File Offset: 0x00003A9E
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000305 RID: 773 RVA: 0x00019074 File Offset: 0x00017274
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

			// Token: 0x06000306 RID: 774 RVA: 0x000058A6 File Offset: 0x00003AA6
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Confuser.Protections.CtrlFlowMod.InstrBlock>.GetEnumerator();
			}

			// Token: 0x0400022D RID: 557
			private int <>1__state;

			// Token: 0x0400022E RID: 558
			private InstrBlock <>2__current;

			// Token: 0x0400022F RID: 559
			private int <>l__initialThreadId;

			// Token: 0x04000230 RID: 560
			private ScopeBlock scope;

			// Token: 0x04000231 RID: 561
			public ScopeBlock <>3__scope;

			// Token: 0x04000232 RID: 562
			private List<BlockBase>.Enumerator <>s__1;

			// Token: 0x04000233 RID: 563
			private BlockBase <iteratorVariable0>5__2;

			// Token: 0x04000234 RID: 564
			private IEnumerator<InstrBlock> <>s__3;

			// Token: 0x04000235 RID: 565
			private InstrBlock <iteratorVariable1>5__4;
		}
	}
}
