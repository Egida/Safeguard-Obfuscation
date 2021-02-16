using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000EC RID: 236
	internal class JumpMangler : ManglerBase
	{
		// Token: 0x0600039F RID: 927 RVA: 0x0001DA54 File Offset: 0x0001BC54
		private LinkedList<Instruction[]> SpiltFragments(InstrBlock block, CFContext ctx)
		{
			LinkedList<Instruction[]> fragments = new LinkedList<Instruction[]>();
			List<Instruction> currentFragment = new List<Instruction>();
			int skipCount = -1;
			int i = 0;
			while (i < block.Instructions.Count)
			{
				bool flag = skipCount != -1;
				if (!flag)
				{
					goto IL_63;
				}
				bool flag2 = skipCount > 0;
				if (!flag2)
				{
					fragments.AddLast(currentFragment.ToArray());
					currentFragment.Clear();
					skipCount = -1;
					goto IL_63;
				}
				currentFragment.Add(block.Instructions[i]);
				skipCount--;
				IL_28D:
				i++;
				continue;
				IL_63:
				bool flag3 = block.Instructions[i].OpCode.OpCodeType == OpCodeType.Prefix;
				if (flag3)
				{
					skipCount = 1;
					currentFragment.Add(block.Instructions[i]);
				}
				bool flag4 = i + 2 < block.Instructions.Count && block.Instructions[i].OpCode.Code == Code.Dup && block.Instructions[i + 1].OpCode.Code == Code.Ldvirtftn && block.Instructions[i + 2].OpCode.Code == Code.Newobj;
				if (flag4)
				{
					skipCount = 2;
					currentFragment.Add(block.Instructions[i]);
				}
				bool flag5 = i + 4 < block.Instructions.Count && block.Instructions[i].OpCode.Code == Code.Ldc_I4 && block.Instructions[i + 1].OpCode.Code == Code.Newarr && block.Instructions[i + 2].OpCode.Code == Code.Dup && block.Instructions[i + 3].OpCode.Code == Code.Ldtoken && block.Instructions[i + 4].OpCode.Code == Code.Call;
				if (flag5)
				{
					skipCount = 4;
					currentFragment.Add(block.Instructions[i]);
				}
				bool flag6 = i + 1 < block.Instructions.Count && block.Instructions[i].OpCode.Code == Code.Ldftn && block.Instructions[i + 1].OpCode.Code == Code.Newobj;
				if (flag6)
				{
					skipCount = 1;
					currentFragment.Add(block.Instructions[i]);
				}
				currentFragment.Add(block.Instructions[i]);
				bool flag7 = ctx.Intensity > ctx.Random.NextDouble();
				if (flag7)
				{
					fragments.AddLast(currentFragment.ToArray());
					currentFragment.Clear();
				}
				goto IL_28D;
			}
			bool flag8 = currentFragment.Count > 0;
			if (flag8)
			{
				fragments.AddLast(currentFragment.ToArray());
			}
			return fragments;
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x0001DD2C File Offset: 0x0001BF2C
		public override void Mangle(CilBody body, ScopeBlock root, CFContext ctx)
		{
			ushort maxStack = body.MaxStack;
			body.MaxStack = maxStack + 1;
			foreach (InstrBlock block in ManglerBase.GetAllBlocks(root))
			{
				LinkedList<Instruction[]> fragments = this.SpiltFragments(block, ctx);
				bool flag = fragments.Count < 4;
				if (!flag)
				{
					LinkedListNode<Instruction[]> current = fragments.First;
					while (current.Next != null)
					{
						List<Instruction> newFragment = new List<Instruction>(current.Value);
						ctx.AddJump(newFragment, current.Next.Value[0]);
						ctx.AddJunk(newFragment);
						current.Value = newFragment.ToArray();
						current = current.Next;
					}
					Instruction[] first = fragments.First.Value;
					fragments.RemoveFirst();
					Instruction[] last = fragments.Last.Value;
					fragments.RemoveLast();
					List<Instruction[]> newFragments = fragments.ToList<Instruction[]>();
					ctx.Random.Shuffle<Instruction[]>(newFragments);
					block.Instructions = first.Concat(newFragments.SelectMany((Instruction[] fragment) => fragment)).Concat(last).ToList<Instruction>();
				}
			}
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00005C85 File Offset: 0x00003E85
		public JumpMangler()
		{
		}

		// Token: 0x020000ED RID: 237
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060003A2 RID: 930 RVA: 0x00005C8E File Offset: 0x00003E8E
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060003A3 RID: 931 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x060003A4 RID: 932 RVA: 0x0000565F File Offset: 0x0000385F
			internal IEnumerable<Instruction> <Mangle>b__1_0(Instruction[] fragment)
			{
				return fragment;
			}

			// Token: 0x040002B3 RID: 691
			public static readonly JumpMangler.<>c <>9 = new JumpMangler.<>c();

			// Token: 0x040002B4 RID: 692
			public static Func<Instruction[], IEnumerable<Instruction>> <>9__1_0;
		}
	}
}
