using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000CC RID: 204
	internal static class BlockParser
	{
		// Token: 0x06000329 RID: 809 RVA: 0x00019F0C File Offset: 0x0001810C
		public static ScopeBlock ParseBody(CilBody body)
		{
			Dictionary<ExceptionHandler, Tuple<ScopeBlock, ScopeBlock, ScopeBlock>> dictionary = new Dictionary<ExceptionHandler, Tuple<ScopeBlock, ScopeBlock, ScopeBlock>>();
			foreach (ExceptionHandler handler in body.ExceptionHandlers)
			{
				ScopeBlock block = new ScopeBlock(BlockType.Try, handler);
				BlockType @finally = BlockType.Handler;
				bool flag = handler.HandlerType == ExceptionHandlerType.Finally;
				if (flag)
				{
					@finally = BlockType.Finally;
				}
				else
				{
					bool flag2 = handler.HandlerType == ExceptionHandlerType.Fault;
					if (flag2)
					{
						@finally = BlockType.Fault;
					}
				}
				ScopeBlock block2 = new ScopeBlock(@finally, handler);
				bool flag3 = handler.FilterStart != null;
				if (flag3)
				{
					ScopeBlock block3 = new ScopeBlock(BlockType.Filter, handler);
					dictionary[handler] = Tuple.Create<ScopeBlock, ScopeBlock, ScopeBlock>(block, block2, block3);
				}
				else
				{
					dictionary[handler] = Tuple.Create<ScopeBlock, ScopeBlock, ScopeBlock>(block, block2, null);
				}
			}
			ScopeBlock item = new ScopeBlock(BlockType.Normal, null);
			Stack<ScopeBlock> stack = new Stack<ScopeBlock>();
			stack.Push(item);
			foreach (Instruction instruction in body.Instructions)
			{
				foreach (ExceptionHandler handler2 in body.ExceptionHandlers)
				{
					Tuple<ScopeBlock, ScopeBlock, ScopeBlock> local = dictionary[handler2];
					bool flag4 = instruction == handler2.TryEnd;
					if (flag4)
					{
						stack.Pop();
					}
					bool flag5 = instruction == handler2.HandlerEnd;
					if (flag5)
					{
						stack.Pop();
					}
					bool flag6 = handler2.FilterStart != null && instruction == handler2.HandlerStart;
					if (flag6)
					{
						stack.Pop();
					}
				}
				foreach (ExceptionHandler handler3 in body.ExceptionHandlers.Reverse<ExceptionHandler>())
				{
					Tuple<ScopeBlock, ScopeBlock, ScopeBlock> tuple = dictionary[handler3];
					ScopeBlock block4 = (stack.Count > 0) ? stack.Peek() : null;
					bool flag7 = instruction == handler3.TryStart;
					if (flag7)
					{
						bool flag8 = block4 != null;
						if (flag8)
						{
							block4.Children.Add(tuple.Item1);
						}
						stack.Push(tuple.Item1);
					}
					bool flag9 = instruction == handler3.HandlerStart;
					if (flag9)
					{
						bool flag10 = block4 != null;
						if (flag10)
						{
							block4.Children.Add(tuple.Item2);
						}
						stack.Push(tuple.Item2);
					}
					bool flag11 = instruction == handler3.FilterStart;
					if (flag11)
					{
						bool flag12 = block4 != null;
						if (flag12)
						{
							block4.Children.Add(tuple.Item3);
						}
						stack.Push(tuple.Item3);
					}
				}
				ScopeBlock block5 = stack.Peek();
				InstrBlock block6 = block5.Children.LastOrDefault<BlockBase>() as InstrBlock;
				bool flag13 = block6 == null;
				if (flag13)
				{
					block5.Children.Add(block6 = new InstrBlock());
				}
				block6.Instructions.Add(instruction);
			}
			foreach (ExceptionHandler handler4 in body.ExceptionHandlers)
			{
				bool flag14 = handler4.TryEnd == null;
				if (flag14)
				{
					stack.Pop();
				}
				bool flag15 = handler4.HandlerEnd == null;
				if (flag15)
				{
					stack.Pop();
				}
			}
			return item;
		}
	}
}
