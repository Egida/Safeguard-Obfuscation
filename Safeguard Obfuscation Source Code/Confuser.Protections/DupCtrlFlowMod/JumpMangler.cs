using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000D5 RID: 213
	internal class JumpMangler : ManglerBase
	{
		// Token: 0x06000345 RID: 837 RVA: 0x0001ACD0 File Offset: 0x00018ED0
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

		// Token: 0x06000346 RID: 838 RVA: 0x0001AE3C File Offset: 0x0001903C
		private LinkedList<Instruction[]> SpiltFragments(InstrBlock block, CFContext ctx)
		{
			LinkedList<Instruction[]> list = new LinkedList<Instruction[]>();
			List<Instruction> list2 = new List<Instruction>();
			int num = -1;
			int i = 0;
			while (i < block.Instructions.Count)
			{
				bool flag = num != -1;
				if (!flag)
				{
					goto IL_63;
				}
				bool flag2 = num > 0;
				if (!flag2)
				{
					list.AddLast(list2.ToArray());
					list2.Clear();
					num = -1;
					goto IL_63;
				}
				list2.Add(block.Instructions[i]);
				num--;
				IL_293:
				i++;
				continue;
				IL_63:
				bool flag3 = block.Instructions[i].OpCode.OpCodeType == OpCodeType.Prefix;
				if (flag3)
				{
					num = 1;
					list2.Add(block.Instructions[i]);
				}
				bool flag4 = i + 2 < block.Instructions.Count && block.Instructions[i].OpCode.Code == Code.Dup && block.Instructions[i + 1].OpCode.Code == Code.Ldvirtftn && block.Instructions[i + 2].OpCode.Code == Code.Newobj;
				if (flag4)
				{
					num = 2;
					list2.Add(block.Instructions[i]);
				}
				bool flag5 = i + 4 < block.Instructions.Count && block.Instructions[i].OpCode.Code == Code.Ldc_I4 && block.Instructions[i + 1].OpCode.Code == Code.Newarr && block.Instructions[i + 2].OpCode.Code == Code.Dup && block.Instructions[i + 3].OpCode.Code == Code.Ldtoken && block.Instructions[i + 4].OpCode.Code == Code.Call;
				if (flag5)
				{
					num = 4;
					list2.Add(block.Instructions[i]);
				}
				bool flag6 = i + 1 < block.Instructions.Count && block.Instructions[i].OpCode.Code == Code.Ldftn && block.Instructions[i + 1].OpCode.Code == Code.Newobj;
				if (flag6)
				{
					num = 1;
					list2.Add(block.Instructions[i]);
				}
				list2.Add(block.Instructions[i]);
				bool flag7 = ctx.Intensity > ctx.Random.NextDouble();
				if (flag7)
				{
					list.AddLast(list2.ToArray());
					list2.Clear();
				}
				goto IL_293;
			}
			bool flag8 = list2.Count > 0;
			if (flag8)
			{
				list.AddLast(list2.ToArray());
			}
			return list;
		}

		// Token: 0x06000347 RID: 839 RVA: 0x00005A39 File Offset: 0x00003C39
		public JumpMangler()
		{
		}

		// Token: 0x020000D6 RID: 214
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000348 RID: 840 RVA: 0x00005A42 File Offset: 0x00003C42
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000349 RID: 841 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x0600034A RID: 842 RVA: 0x0000565F File Offset: 0x0000385F
			internal IEnumerable<Instruction> <Mangle>b__0_0(Instruction[] fragment)
			{
				return fragment;
			}

			// Token: 0x0400026C RID: 620
			public static readonly JumpMangler.<>c <>9 = new JumpMangler.<>c();

			// Token: 0x0400026D RID: 621
			public static Func<Instruction[], IEnumerable<Instruction>> <>9__0_0;
		}
	}
}
