using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000EE RID: 238
	internal abstract class ManglerBase
	{
		// Token: 0x060003A5 RID: 933 RVA: 0x00005C9A File Offset: 0x00003E9A
		protected static IEnumerable<InstrBlock> GetAllBlocks(ScopeBlock scope)
		{
			foreach (BlockBase child in scope.Children)
			{
				bool flag = child is InstrBlock;
				if (flag)
				{
					yield return (InstrBlock)child;
				}
				else
				{
					foreach (InstrBlock block in ManglerBase.GetAllBlocks((ScopeBlock)child))
					{
						yield return block;
						block = null;
					}
					IEnumerator<InstrBlock> enumerator2 = null;
				}
				child = null;
			}
			List<BlockBase>.Enumerator enumerator = default(List<BlockBase>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060003A6 RID: 934
		public abstract void Mangle(CilBody body, ScopeBlock root, CFContext ctx);

		// Token: 0x060003A7 RID: 935 RVA: 0x00004A68 File Offset: 0x00002C68
		protected ManglerBase()
		{
		}

		// Token: 0x020000EF RID: 239
		[CompilerGenerated]
		private sealed class <GetAllBlocks>d__0 : IEnumerable<InstrBlock>, IEnumerator<InstrBlock>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x060003A8 RID: 936 RVA: 0x00005CAA File Offset: 0x00003EAA
			[DebuggerHidden]
			public <GetAllBlocks>d__0(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x060003A9 RID: 937 RVA: 0x0001DE98 File Offset: 0x0001C098
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

			// Token: 0x060003AA RID: 938 RVA: 0x0001DF00 File Offset: 0x0001C100
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
						goto IL_126;
					case 1:
						this.<>1__state = -3;
						goto IL_11E;
					case 2:
						this.<>1__state = -4;
						block = null;
						break;
					default:
						return false;
					}
					IL_102:
					if (enumerator2.MoveNext())
					{
						block = enumerator2.Current;
						this.<>2__current = block;
						this.<>1__state = 2;
						return true;
					}
					this.<>m__Finally2();
					enumerator2 = null;
					IL_11E:
					child = null;
					IL_126:
					if (!enumerator.MoveNext())
					{
						this.<>m__Finally1();
						enumerator = default(List<BlockBase>.Enumerator);
						result = false;
					}
					else
					{
						child = enumerator.Current;
						bool flag = child is InstrBlock;
						if (!flag)
						{
							enumerator2 = ManglerBase.GetAllBlocks((ScopeBlock)child).GetEnumerator();
							this.<>1__state = -4;
							goto IL_102;
						}
						this.<>2__current = (InstrBlock)child;
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

			// Token: 0x060003AB RID: 939 RVA: 0x00005CCA File Offset: 0x00003ECA
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				((IDisposable)enumerator).Dispose();
			}

			// Token: 0x060003AC RID: 940 RVA: 0x00005CE5 File Offset: 0x00003EE5
			private void <>m__Finally2()
			{
				this.<>1__state = -3;
				if (enumerator2 != null)
				{
					enumerator2.Dispose();
				}
			}

			// Token: 0x170000BA RID: 186
			// (get) Token: 0x060003AD RID: 941 RVA: 0x00005D03 File Offset: 0x00003F03
			InstrBlock IEnumerator<InstrBlock>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060003AE RID: 942 RVA: 0x000050B0 File Offset: 0x000032B0
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x170000BB RID: 187
			// (get) Token: 0x060003AF RID: 943 RVA: 0x00005D03 File Offset: 0x00003F03
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060003B0 RID: 944 RVA: 0x0001E080 File Offset: 0x0001C280
			[DebuggerHidden]
			IEnumerator<InstrBlock> IEnumerable<InstrBlock>.GetEnumerator()
			{
				ManglerBase.<GetAllBlocks>d__0 <GetAllBlocks>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<GetAllBlocks>d__ = this;
				}
				else
				{
					<GetAllBlocks>d__ = new ManglerBase.<GetAllBlocks>d__0(0);
				}
				<GetAllBlocks>d__.scope = scope;
				return <GetAllBlocks>d__;
			}

			// Token: 0x060003B1 RID: 945 RVA: 0x00005D0B File Offset: 0x00003F0B
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Confuser.Protections.ControlFlow.InstrBlock>.GetEnumerator();
			}

			// Token: 0x040002B5 RID: 693
			private int <>1__state;

			// Token: 0x040002B6 RID: 694
			private InstrBlock <>2__current;

			// Token: 0x040002B7 RID: 695
			private int <>l__initialThreadId;

			// Token: 0x040002B8 RID: 696
			private ScopeBlock scope;

			// Token: 0x040002B9 RID: 697
			public ScopeBlock <>3__scope;

			// Token: 0x040002BA RID: 698
			private List<BlockBase>.Enumerator <>s__1;

			// Token: 0x040002BB RID: 699
			private BlockBase <child>5__2;

			// Token: 0x040002BC RID: 700
			private IEnumerator<InstrBlock> <>s__3;

			// Token: 0x040002BD RID: 701
			private InstrBlock <block>5__4;
		}
	}
}
