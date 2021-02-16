using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Compress
{
	// Token: 0x02000102 RID: 258
	internal class NormalDeriver : IKeyDeriver
	{
		// Token: 0x060003E9 RID: 1001 RVA: 0x00005E0A File Offset: 0x0000400A
		public void Init(ConfuserContext ctx, RandomGenerator random)
		{
			this.k1 = (random.NextUInt32() | 1U);
			this.k2 = (random.NextUInt32() | 1U);
			this.k3 = (random.NextUInt32() | 1U);
			this.seed = random.NextUInt32();
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0001FC7C File Offset: 0x0001DE7C
		public uint[] DeriveKey(uint[] a, uint[] b)
		{
			uint[] ret = new uint[16];
			uint state = this.seed;
			for (int i = 0; i < 16; i++)
			{
				switch (state % 3U)
				{
				case 0U:
					ret[i] = (a[i] ^ b[i]);
					break;
				case 1U:
					ret[i] = a[i] * b[i];
					break;
				case 2U:
					ret[i] = a[i] + b[i];
					break;
				}
				state = state * state % 772287797U;
				switch (state % 3U)
				{
				case 0U:
					ret[i] += this.k1;
					break;
				case 1U:
					ret[i] ^= this.k2;
					break;
				case 2U:
					ret[i] *= this.k3;
					break;
				}
				state = state * state % 772287797U;
			}
			return ret;
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00005E43 File Offset: 0x00004043
		public IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src)
		{
			uint state = this.seed;
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				yield return Instruction.Create(OpCodes.Ldloc, dst);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldloc, dst);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldelem_U4);
				yield return Instruction.Create(OpCodes.Ldloc, src);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldelem_U4);
				switch (state % 3U)
				{
				case 0U:
					yield return Instruction.Create(OpCodes.Xor);
					break;
				case 1U:
					yield return Instruction.Create(OpCodes.Mul);
					break;
				case 2U:
					yield return Instruction.Create(OpCodes.Add);
					break;
				}
				state = state * state % 772287797U;
				switch (state % 3U)
				{
				case 0U:
					yield return Instruction.Create(OpCodes.Ldc_I4, (int)this.k1);
					yield return Instruction.Create(OpCodes.Add);
					break;
				case 1U:
					yield return Instruction.Create(OpCodes.Ldc_I4, (int)this.k2);
					yield return Instruction.Create(OpCodes.Xor);
					break;
				case 2U:
					yield return Instruction.Create(OpCodes.Ldc_I4, (int)this.k3);
					yield return Instruction.Create(OpCodes.Mul);
					break;
				}
				state = state * state % 772287797U;
				yield return Instruction.Create(OpCodes.Stelem_I4);
				num = i;
			}
			yield break;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00004A68 File Offset: 0x00002C68
		public NormalDeriver()
		{
		}

		// Token: 0x040002F7 RID: 759
		private uint k1;

		// Token: 0x040002F8 RID: 760
		private uint k2;

		// Token: 0x040002F9 RID: 761
		private uint k3;

		// Token: 0x040002FA RID: 762
		private uint seed;

		// Token: 0x02000103 RID: 259
		[CompilerGenerated]
		private sealed class <EmitDerivation>d__6 : IEnumerable<Instruction>, IEnumerator<Instruction>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x060003ED RID: 1005 RVA: 0x00005E70 File Offset: 0x00004070
			[DebuggerHidden]
			public <EmitDerivation>d__6(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x060003EE RID: 1006 RVA: 0x000050A6 File Offset: 0x000032A6
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x060003EF RID: 1007 RVA: 0x0001FD60 File Offset: 0x0001DF60
			bool IEnumerator.MoveNext()
			{
				switch (this.<>1__state)
				{
				case 0:
					this.<>1__state = -1;
					state = this.seed;
					i = 0;
					goto IL_3FD;
				case 1:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Ldc_I4, i);
					this.<>1__state = 2;
					return true;
				case 2:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Ldloc, dst);
					this.<>1__state = 3;
					return true;
				case 3:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Ldc_I4, i);
					this.<>1__state = 4;
					return true;
				case 4:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Ldelem_U4);
					this.<>1__state = 5;
					return true;
				case 5:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Ldloc, src);
					this.<>1__state = 6;
					return true;
				case 6:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Ldc_I4, i);
					this.<>1__state = 7;
					return true;
				case 7:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Ldelem_U4);
					this.<>1__state = 8;
					return true;
				case 8:
					this.<>1__state = -1;
					switch (state % 3U)
					{
					case 0U:
						this.<>2__current = Instruction.Create(OpCodes.Xor);
						this.<>1__state = 9;
						return true;
					case 1U:
						this.<>2__current = Instruction.Create(OpCodes.Mul);
						this.<>1__state = 10;
						return true;
					case 2U:
						this.<>2__current = Instruction.Create(OpCodes.Add);
						this.<>1__state = 11;
						return true;
					}
					break;
				case 9:
					this.<>1__state = -1;
					break;
				case 10:
					this.<>1__state = -1;
					break;
				case 11:
					this.<>1__state = -1;
					break;
				case 12:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Add);
					this.<>1__state = 13;
					return true;
				case 13:
					this.<>1__state = -1;
					goto IL_3B2;
				case 14:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Xor);
					this.<>1__state = 15;
					return true;
				case 15:
					this.<>1__state = -1;
					goto IL_3B2;
				case 16:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Mul);
					this.<>1__state = 17;
					return true;
				case 17:
					this.<>1__state = -1;
					goto IL_3B2;
				case 18:
				{
					this.<>1__state = -1;
					int num = i;
					i = num + 1;
					goto IL_3FD;
				}
				default:
					return false;
				}
				state = state * state % 772287797U;
				switch (state % 3U)
				{
				case 0U:
					this.<>2__current = Instruction.Create(OpCodes.Ldc_I4, (int)this.k1);
					this.<>1__state = 12;
					return true;
				case 1U:
					this.<>2__current = Instruction.Create(OpCodes.Ldc_I4, (int)this.k2);
					this.<>1__state = 14;
					return true;
				case 2U:
					this.<>2__current = Instruction.Create(OpCodes.Ldc_I4, (int)this.k3);
					this.<>1__state = 16;
					return true;
				}
				IL_3B2:
				state = state * state % 772287797U;
				this.<>2__current = Instruction.Create(OpCodes.Stelem_I4);
				this.<>1__state = 18;
				return true;
				IL_3FD:
				if (i >= 16)
				{
					return false;
				}
				this.<>2__current = Instruction.Create(OpCodes.Ldloc, dst);
				this.<>1__state = 1;
				return true;
			}

			// Token: 0x170000BE RID: 190
			// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00005E90 File Offset: 0x00004090
			Instruction IEnumerator<Instruction>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060003F1 RID: 1009 RVA: 0x000050B0 File Offset: 0x000032B0
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x170000BF RID: 191
			// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00005E90 File Offset: 0x00004090
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060003F3 RID: 1011 RVA: 0x00020180 File Offset: 0x0001E380
			[DebuggerHidden]
			IEnumerator<Instruction> IEnumerable<Instruction>.GetEnumerator()
			{
				NormalDeriver.<EmitDerivation>d__6 <EmitDerivation>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<EmitDerivation>d__ = this;
				}
				else
				{
					<EmitDerivation>d__ = new NormalDeriver.<EmitDerivation>d__6(0);
					<EmitDerivation>d__.<>4__this = this;
				}
				<EmitDerivation>d__.method = method;
				<EmitDerivation>d__.ctx = ctx;
				<EmitDerivation>d__.dst = dst;
				<EmitDerivation>d__.src = src;
				return <EmitDerivation>d__;
			}

			// Token: 0x060003F4 RID: 1012 RVA: 0x00005E98 File Offset: 0x00004098
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<dnlib.DotNet.Emit.Instruction>.GetEnumerator();
			}

			// Token: 0x040002FB RID: 763
			private int <>1__state;

			// Token: 0x040002FC RID: 764
			private Instruction <>2__current;

			// Token: 0x040002FD RID: 765
			private int <>l__initialThreadId;

			// Token: 0x040002FE RID: 766
			private MethodDef method;

			// Token: 0x040002FF RID: 767
			public MethodDef <>3__method;

			// Token: 0x04000300 RID: 768
			private ConfuserContext ctx;

			// Token: 0x04000301 RID: 769
			public ConfuserContext <>3__ctx;

			// Token: 0x04000302 RID: 770
			private Local dst;

			// Token: 0x04000303 RID: 771
			public Local <>3__dst;

			// Token: 0x04000304 RID: 772
			private Local src;

			// Token: 0x04000305 RID: 773
			public Local <>3__src;

			// Token: 0x04000306 RID: 774
			public NormalDeriver <>4__this;

			// Token: 0x04000307 RID: 775
			private uint <state>5__1;

			// Token: 0x04000308 RID: 776
			private int <i>5__2;
		}
	}
}
