using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000C0 RID: 192
	internal class JumpMangler : ManglerBase
	{
		// Token: 0x060002F4 RID: 756 RVA: 0x00018A3C File Offset: 0x00016C3C
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

		// Token: 0x060002F5 RID: 757 RVA: 0x00018BA8 File Offset: 0x00016DA8
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

		// Token: 0x060002F6 RID: 758 RVA: 0x00005820 File Offset: 0x00003A20
		public JumpMangler()
		{
		}

		// Token: 0x020000C1 RID: 193
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060002F7 RID: 759 RVA: 0x00005829 File Offset: 0x00003A29
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060002F8 RID: 760 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x060002F9 RID: 761 RVA: 0x0000565F File Offset: 0x0000385F
			internal IEnumerable<Instruction> <Mangle>b__0_0(Instruction[] fragment)
			{
				return fragment;
			}

			// Token: 0x0400022B RID: 555
			public static readonly JumpMangler.<>c <>9 = new JumpMangler.<>c();

			// Token: 0x0400022C RID: 556
			public static Func<Instruction[], IEnumerable<Instruction>> <>9__0_0;
		}
	}
}
