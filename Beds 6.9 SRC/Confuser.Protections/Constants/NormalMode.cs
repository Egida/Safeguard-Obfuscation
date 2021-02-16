using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Confuser.Core.Helpers;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000A1 RID: 161
	internal class NormalMode : IEncodeMode
	{
		// Token: 0x0600026B RID: 619 RVA: 0x000054D9 File Offset: 0x000036D9
		public IEnumerable<Instruction> EmitDecrypt(MethodDef init, CEContext ctx, Local block, Local key)
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

		// Token: 0x0600026C RID: 620 RVA: 0x00014FC0 File Offset: 0x000131C0
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] ret = new uint[key.Length];
			for (int i = 0; i < key.Length; i++)
			{
				ret[i] = (data[i + offset] ^ key[i]);
			}
			return ret;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00014FFC File Offset: 0x000131FC
		public object CreateDecoder(MethodDef decoder, CEContext ctx)
		{
			uint k1 = ctx.Random.NextUInt32() | 1U;
			uint k2 = ctx.Random.NextUInt32();
			MutationHelper.ReplacePlaceholder(decoder, delegate(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Ldc_I4, (int)MathsUtils.modInv(k1)));
				repl.Add(Instruction.Create(OpCodes.Mul));
				repl.Add(Instruction.Create(OpCodes.Ldc_I4, (int)k2));
				repl.Add(Instruction.Create(OpCodes.Xor));
				return repl.ToArray();
			});
			return Tuple.Create<uint, uint>(k1, k2);
		}

		// Token: 0x0600026E RID: 622 RVA: 0x000130BC File Offset: 0x000112BC
		public uint Encode(object data, CEContext ctx, uint id)
		{
			Tuple<uint, uint> key = (Tuple<uint, uint>)data;
			uint ret = (id ^ key.Item2) * key.Item1;
			Debug.Assert((ret * MathsUtils.modInv(key.Item1) ^ key.Item2) == id);
			return ret;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00004A68 File Offset: 0x00002C68
		public NormalMode()
		{
		}

		// Token: 0x020000A2 RID: 162
		[CompilerGenerated]
		private sealed class <EmitDecrypt>d__0 : IEnumerable<Instruction>, IEnumerator<Instruction>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x06000270 RID: 624 RVA: 0x00005506 File Offset: 0x00003706
			[DebuggerHidden]
			public <EmitDecrypt>d__0(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x06000271 RID: 625 RVA: 0x000050A6 File Offset: 0x000032A6
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x06000272 RID: 626 RVA: 0x0001505C File Offset: 0x0001325C
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

			// Token: 0x17000099 RID: 153
			// (get) Token: 0x06000273 RID: 627 RVA: 0x00005526 File Offset: 0x00003726
			Instruction IEnumerator<Instruction>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000274 RID: 628 RVA: 0x000050B0 File Offset: 0x000032B0
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x1700009A RID: 154
			// (get) Token: 0x06000275 RID: 629 RVA: 0x00005526 File Offset: 0x00003726
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000276 RID: 630 RVA: 0x00015278 File Offset: 0x00013478
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

			// Token: 0x06000277 RID: 631 RVA: 0x0000552E File Offset: 0x0000372E
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<dnlib.DotNet.Emit.Instruction>.GetEnumerator();
			}

			// Token: 0x040001A9 RID: 425
			private int <>1__state;

			// Token: 0x040001AA RID: 426
			private Instruction <>2__current;

			// Token: 0x040001AB RID: 427
			private int <>l__initialThreadId;

			// Token: 0x040001AC RID: 428
			private MethodDef init;

			// Token: 0x040001AD RID: 429
			public MethodDef <>3__init;

			// Token: 0x040001AE RID: 430
			private CEContext ctx;

			// Token: 0x040001AF RID: 431
			public CEContext <>3__ctx;

			// Token: 0x040001B0 RID: 432
			private Local block;

			// Token: 0x040001B1 RID: 433
			public Local <>3__block;

			// Token: 0x040001B2 RID: 434
			private Local key;

			// Token: 0x040001B3 RID: 435
			public Local <>3__key;

			// Token: 0x040001B4 RID: 436
			public NormalMode <>4__this;

			// Token: 0x040001B5 RID: 437
			private int <i>5__1;
		}

		// Token: 0x020000A3 RID: 163
		[CompilerGenerated]
		private sealed class <>c__DisplayClass2_0
		{
			// Token: 0x06000278 RID: 632 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass2_0()
			{
			}

			// Token: 0x06000279 RID: 633 RVA: 0x000152F0 File Offset: 0x000134F0
			internal Instruction[] <CreateDecoder>b__0(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Ldc_I4, (int)MathsUtils.modInv(this.k1)));
				repl.Add(Instruction.Create(OpCodes.Mul));
				repl.Add(Instruction.Create(OpCodes.Ldc_I4, (int)this.k2));
				repl.Add(Instruction.Create(OpCodes.Xor));
				return repl.ToArray();
			}

			// Token: 0x040001B6 RID: 438
			public uint k1;

			// Token: 0x040001B7 RID: 439
			public uint k2;
		}
	}
}
