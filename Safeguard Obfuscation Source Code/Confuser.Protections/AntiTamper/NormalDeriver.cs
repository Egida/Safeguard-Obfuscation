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

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000B2 RID: 178
	internal class NormalDeriver : IKeyDeriver
	{
		// Token: 0x060002BB RID: 699 RVA: 0x00004A34 File Offset: 0x00002C34
		public void Init(ConfuserContext ctx, RandomGenerator random)
		{
		}

		// Token: 0x060002BC RID: 700 RVA: 0x00016C58 File Offset: 0x00014E58
		public uint[] DeriveKey(uint[] a, uint[] b)
		{
			uint[] ret = new uint[16];
			for (int i = 0; i < 16; i++)
			{
				switch (i % 3)
				{
				case 0:
					ret[i] = (a[i] ^ b[i]);
					break;
				case 1:
					ret[i] = a[i] * b[i];
					break;
				case 2:
					ret[i] = a[i] + b[i];
					break;
				}
			}
			return ret;
		}

		// Token: 0x060002BD RID: 701 RVA: 0x000056D6 File Offset: 0x000038D6
		public IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src)
		{
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
				switch (i % 3)
				{
				case 0:
					yield return Instruction.Create(OpCodes.Xor);
					break;
				case 1:
					yield return Instruction.Create(OpCodes.Mul);
					break;
				case 2:
					yield return Instruction.Create(OpCodes.Add);
					break;
				}
				yield return Instruction.Create(OpCodes.Stelem_I4);
				num = i;
			}
			yield break;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00004A68 File Offset: 0x00002C68
		public NormalDeriver()
		{
		}

		// Token: 0x020000B3 RID: 179
		[CompilerGenerated]
		private sealed class <EmitDerivation>d__2 : IEnumerable<Instruction>, IEnumerator<Instruction>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x060002BF RID: 703 RVA: 0x00005703 File Offset: 0x00003903
			[DebuggerHidden]
			public <EmitDerivation>d__2(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x060002C0 RID: 704 RVA: 0x000050A6 File Offset: 0x000032A6
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x060002C1 RID: 705 RVA: 0x00016CC4 File Offset: 0x00014EC4
			bool IEnumerator.MoveNext()
			{
				switch (this.<>1__state)
				{
				case 0:
					this.<>1__state = -1;
					i = 0;
					goto IL_271;
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
					switch (i % 3)
					{
					case 0:
						this.<>2__current = Instruction.Create(OpCodes.Xor);
						this.<>1__state = 9;
						return true;
					case 1:
						this.<>2__current = Instruction.Create(OpCodes.Mul);
						this.<>1__state = 10;
						return true;
					case 2:
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
				{
					this.<>1__state = -1;
					int num = i;
					i = num + 1;
					goto IL_271;
				}
				default:
					return false;
				}
				this.<>2__current = Instruction.Create(OpCodes.Stelem_I4);
				this.<>1__state = 12;
				return true;
				IL_271:
				if (i >= 16)
				{
					return false;
				}
				this.<>2__current = Instruction.Create(OpCodes.Ldloc, dst);
				this.<>1__state = 1;
				return true;
			}

			// Token: 0x1700009F RID: 159
			// (get) Token: 0x060002C2 RID: 706 RVA: 0x00005723 File Offset: 0x00003923
			Instruction IEnumerator<Instruction>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060002C3 RID: 707 RVA: 0x000050B0 File Offset: 0x000032B0
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x170000A0 RID: 160
			// (get) Token: 0x060002C4 RID: 708 RVA: 0x00005723 File Offset: 0x00003923
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060002C5 RID: 709 RVA: 0x00016F54 File Offset: 0x00015154
			[DebuggerHidden]
			IEnumerator<Instruction> IEnumerable<Instruction>.GetEnumerator()
			{
				NormalDeriver.<EmitDerivation>d__2 <EmitDerivation>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<EmitDerivation>d__ = this;
				}
				else
				{
					<EmitDerivation>d__ = new NormalDeriver.<EmitDerivation>d__2(0);
					<EmitDerivation>d__.<>4__this = this;
				}
				<EmitDerivation>d__.method = method;
				<EmitDerivation>d__.ctx = ctx;
				<EmitDerivation>d__.dst = dst;
				<EmitDerivation>d__.src = src;
				return <EmitDerivation>d__;
			}

			// Token: 0x060002C6 RID: 710 RVA: 0x0000572B File Offset: 0x0000392B
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<dnlib.DotNet.Emit.Instruction>.GetEnumerator();
			}

			// Token: 0x040001F2 RID: 498
			private int <>1__state;

			// Token: 0x040001F3 RID: 499
			private Instruction <>2__current;

			// Token: 0x040001F4 RID: 500
			private int <>l__initialThreadId;

			// Token: 0x040001F5 RID: 501
			private MethodDef method;

			// Token: 0x040001F6 RID: 502
			public MethodDef <>3__method;

			// Token: 0x040001F7 RID: 503
			private ConfuserContext ctx;

			// Token: 0x040001F8 RID: 504
			public ConfuserContext <>3__ctx;

			// Token: 0x040001F9 RID: 505
			private Local dst;

			// Token: 0x040001FA RID: 506
			public Local <>3__dst;

			// Token: 0x040001FB RID: 507
			private Local src;

			// Token: 0x040001FC RID: 508
			public Local <>3__src;

			// Token: 0x040001FD RID: 509
			public NormalDeriver <>4__this;

			// Token: 0x040001FE RID: 510
			private int <i>5__1;
		}
	}
}
