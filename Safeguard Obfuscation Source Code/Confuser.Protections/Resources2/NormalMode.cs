using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources2
{
	// Token: 0x02000054 RID: 84
	internal class NormalMode : IEncodeMode
	{
		// Token: 0x06000174 RID: 372 RVA: 0x00005146 File Offset: 0x00003346
		public IEnumerable<Instruction> EmitDecrypt(MethodDef init, REContext ctx, Local block, Local key)
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				yield return Instruction.Create(OpCodes.Ldloc, block);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldloc, block);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldelem_U4);
				yield return Instruction.Create(OpCodes.Ldloc, key);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldelem_U4);
				yield return Instruction.Create(OpCodes.Xor);
				yield return Instruction.Create(OpCodes.Stelem_I4);
				num = i;
			}
			yield break;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000B8C0 File Offset: 0x00009AC0
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] ret = new uint[key.Length];
			for (int i = 0; i < key.Length; i++)
			{
				ret[i] = (data[i + offset] ^ key[i]);
			}
			return ret;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00004A68 File Offset: 0x00002C68
		public NormalMode()
		{
		}

		// Token: 0x02000055 RID: 85
		[CompilerGenerated]
		private sealed class <EmitDecrypt>d__0 : IEnumerable<Instruction>, IEnumerator<Instruction>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x06000177 RID: 375 RVA: 0x00005173 File Offset: 0x00003373
			[DebuggerHidden]
			public <EmitDecrypt>d__0(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x06000178 RID: 376 RVA: 0x000050A6 File Offset: 0x000032A6
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x06000179 RID: 377 RVA: 0x0000C950 File Offset: 0x0000AB50
			bool IEnumerator.MoveNext()
			{
				switch (this.<>1__state)
				{
				case 0:
					this.<>1__state = -1;
					i = 0;
					break;
				case 1:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Ldc_I4, i);
					this.<>1__state = 2;
					return true;
				case 2:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Ldloc, block);
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
					this.<>2__current = Instruction.Create(OpCodes.Ldloc, key);
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
					this.<>2__current = Instruction.Create(OpCodes.Xor);
					this.<>1__state = 9;
					return true;
				case 9:
					this.<>1__state = -1;
					this.<>2__current = Instruction.Create(OpCodes.Stelem_I4);
					this.<>1__state = 10;
					return true;
				case 10:
				{
					this.<>1__state = -1;
					int num = i;
					i = num + 1;
					break;
				}
				default:
					return false;
				}
				if (i >= 16)
				{
					return false;
				}
				this.<>2__current = Instruction.Create(OpCodes.Ldloc, block);
				this.<>1__state = 1;
				return true;
			}

			// Token: 0x1700008F RID: 143
			// (get) Token: 0x0600017A RID: 378 RVA: 0x00005193 File Offset: 0x00003393
			Instruction IEnumerator<Instruction>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0600017B RID: 379 RVA: 0x000050B0 File Offset: 0x000032B0
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000090 RID: 144
			// (get) Token: 0x0600017C RID: 380 RVA: 0x00005193 File Offset: 0x00003393
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0600017D RID: 381 RVA: 0x0000CB6C File Offset: 0x0000AD6C
			[DebuggerHidden]
			IEnumerator<Instruction> IEnumerable<Instruction>.GetEnumerator()
			{
				NormalMode.<EmitDecrypt>d__0 <EmitDecrypt>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<EmitDecrypt>d__ = this;
				}
				else
				{
					<EmitDecrypt>d__ = new NormalMode.<EmitDecrypt>d__0(0);
					<EmitDecrypt>d__.<>4__this = this;
				}
				<EmitDecrypt>d__.init = init;
				<EmitDecrypt>d__.ctx = ctx;
				<EmitDecrypt>d__.block = block;
				<EmitDecrypt>d__.key = key;
				return <EmitDecrypt>d__;
			}

			// Token: 0x0600017E RID: 382 RVA: 0x0000519B File Offset: 0x0000339B
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<dnlib.DotNet.Emit.Instruction>.GetEnumerator();
			}

			// Token: 0x040000A6 RID: 166
			private int <>1__state;

			// Token: 0x040000A7 RID: 167
			private Instruction <>2__current;

			// Token: 0x040000A8 RID: 168
			private int <>l__initialThreadId;

			// Token: 0x040000A9 RID: 169
			private MethodDef init;

			// Token: 0x040000AA RID: 170
			public MethodDef <>3__init;

			// Token: 0x040000AB RID: 171
			private REContext ctx;

			// Token: 0x040000AC RID: 172
			public REContext <>3__ctx;

			// Token: 0x040000AD RID: 173
			private Local block;

			// Token: 0x040000AE RID: 174
			public Local <>3__block;

			// Token: 0x040000AF RID: 175
			private Local key;

			// Token: 0x040000B0 RID: 176
			public Local <>3__key;

			// Token: 0x040000B1 RID: 177
			public NormalMode <>4__this;

			// Token: 0x040000B2 RID: 178
			private int <i>5__1;
		}
	}
}
