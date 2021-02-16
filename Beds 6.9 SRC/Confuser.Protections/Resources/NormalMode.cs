using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources
{
	// Token: 0x02000047 RID: 71
	internal class NormalMode : IEncodeMode
	{
		// Token: 0x0600014F RID: 335 RVA: 0x00005059 File Offset: 0x00003259
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

		// Token: 0x06000150 RID: 336 RVA: 0x0000B8C0 File Offset: 0x00009AC0
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] ret = new uint[key.Length];
			for (int i = 0; i < key.Length; i++)
			{
				ret[i] = (data[i + offset] ^ key[i]);
			}
			return ret;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00004A68 File Offset: 0x00002C68
		public NormalMode()
		{
		}

		// Token: 0x02000048 RID: 72
		[CompilerGenerated]
		private sealed class <EmitDecrypt>d__0 : IEnumerable<Instruction>, IEnumerator<Instruction>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x06000152 RID: 338 RVA: 0x00005086 File Offset: 0x00003286
			[DebuggerHidden]
			public <EmitDecrypt>d__0(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x06000153 RID: 339 RVA: 0x000050A6 File Offset: 0x000032A6
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x06000154 RID: 340 RVA: 0x0000B8FC File Offset: 0x00009AFC
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

			// Token: 0x1700008B RID: 139
			// (get) Token: 0x06000155 RID: 341 RVA: 0x000050A8 File Offset: 0x000032A8
			Instruction IEnumerator<Instruction>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000156 RID: 342 RVA: 0x000050B0 File Offset: 0x000032B0
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x1700008C RID: 140
			// (get) Token: 0x06000157 RID: 343 RVA: 0x000050A8 File Offset: 0x000032A8
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000158 RID: 344 RVA: 0x0000BB18 File Offset: 0x00009D18
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

			// Token: 0x06000159 RID: 345 RVA: 0x000050B7 File Offset: 0x000032B7
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<dnlib.DotNet.Emit.Instruction>.GetEnumerator();
			}

			// Token: 0x0400007D RID: 125
			private int <>1__state;

			// Token: 0x0400007E RID: 126
			private Instruction <>2__current;

			// Token: 0x0400007F RID: 127
			private int <>l__initialThreadId;

			// Token: 0x04000080 RID: 128
			private MethodDef init;

			// Token: 0x04000081 RID: 129
			public MethodDef <>3__init;

			// Token: 0x04000082 RID: 130
			private REContext ctx;

			// Token: 0x04000083 RID: 131
			public REContext <>3__ctx;

			// Token: 0x04000084 RID: 132
			private Local block;

			// Token: 0x04000085 RID: 133
			public Local <>3__block;

			// Token: 0x04000086 RID: 134
			private Local key;

			// Token: 0x04000087 RID: 135
			public Local <>3__key;

			// Token: 0x04000088 RID: 136
			public NormalMode <>4__this;

			// Token: 0x04000089 RID: 137
			private int <i>5__1;
		}
	}
}
