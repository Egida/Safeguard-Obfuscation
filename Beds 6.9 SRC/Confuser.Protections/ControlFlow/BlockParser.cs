using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000E0 RID: 224
	internal static class BlockParser
	{
		// Token: 0x06000374 RID: 884 RVA: 0x0001C0E8 File Offset: 0x0001A2E8
		public static ScopeBlock ParseBody(CilBody body)
		{
			Dictionary<ExceptionHandler, Tuple<ScopeBlock, ScopeBlock, ScopeBlock>> ehScopes = new Dictionary<ExceptionHandler, Tuple<ScopeBlock, ScopeBlock, ScopeBlock>>();
			foreach (ExceptionHandler eh in body.ExceptionHandlers)
			{
				ScopeBlock tryBlock = new ScopeBlock(BlockType.Try, eh);
				BlockType handlerType = BlockType.Handler;
				bool flag = eh.HandlerType == ExceptionHandlerType.Finally;
				if (flag)
				{
					handlerType = BlockType.Finally;
				}
				else
				{
					bool flag2 = eh.HandlerType == ExceptionHandlerType.Fault;
					if (flag2)
					{
						handlerType = BlockType.Fault;
					}
				}
				ScopeBlock handlerBlock = new ScopeBlock(handlerType, eh);
				bool flag3 = eh.FilterStart != null;
				if (flag3)
				{
					ScopeBlock filterBlock = new ScopeBlock(BlockType.Filter, eh);
					ehScopes[eh] = Tuple.Create<ScopeBlock, ScopeBlock, ScopeBlock>(tryBlock, handlerBlock, filterBlock);
				}
				else
				{
					ehScopes[eh] = Tuple.Create<ScopeBlock, ScopeBlock, ScopeBlock>(tryBlock, handlerBlock, null);
				}
			}
			ScopeBlock root = new ScopeBlock(BlockType.Normal, null);
			Stack<ScopeBlock> scopeStack = new Stack<ScopeBlock>();
			scopeStack.Push(root);
			foreach (Instruction instr in body.Instructions)
			{
				foreach (ExceptionHandler eh2 in body.ExceptionHandlers)
				{
					Tuple<ScopeBlock, ScopeBlock, ScopeBlock> ehScope = ehScopes[eh2];
					bool flag4 = instr == eh2.TryEnd;
					if (flag4)
					{
						scopeStack.Pop();
					}
					bool flag5 = instr == eh2.HandlerEnd;
					if (flag5)
					{
						scopeStack.Pop();
					}
					bool flag6 = eh2.FilterStart != null && instr == eh2.HandlerStart;
					if (flag6)
					{
						Debug.Assert(scopeStack.Peek().Type == BlockType.Filter);
						scopeStack.Pop();
					}
				}
				foreach (ExceptionHandler eh3 in body.ExceptionHandlers.Reverse<ExceptionHandler>())
				{
					Tuple<ScopeBlock, ScopeBlock, ScopeBlock> ehScope2 = ehScopes[eh3];
					ScopeBlock parent = (scopeStack.Count > 0) ? scopeStack.Peek() : null;
					bool flag7 = instr == eh3.TryStart;
					if (flag7)
					{
						bool flag8 = parent != null;
						if (flag8)
						{
							parent.Children.Add(ehScope2.Item1);
						}
						scopeStack.Push(ehScope2.Item1);
					}
					bool flag9 = instr == eh3.HandlerStart;
					if (flag9)
					{
						bool flag10 = parent != null;
						if (flag10)
						{
							parent.Children.Add(ehScope2.Item2);
						}
						scopeStack.Push(ehScope2.Item2);
					}
					bool flag11 = instr == eh3.FilterStart;
					if (flag11)
					{
						bool flag12 = parent != null;
						if (flag12)
						{
							parent.Children.Add(ehScope2.Item3);
						}
						scopeStack.Push(ehScope2.Item3);
					}
				}
				ScopeBlock scope = scopeStack.Peek();
				InstrBlock block = scope.Children.LastOrDefault<BlockBase>() as InstrBlock;
				bool flag13 = block == null;
				if (flag13)
				{
					scope.Children.Add(block = new InstrBlock());
				}
				block.Instructions.Add(instr);
			}
			foreach (ExceptionHandler eh4 in body.ExceptionHandlers)
			{
				bool flag14 = eh4.TryEnd == null;
				if (flag14)
				{
					scopeStack.Pop();
				}
				bool flag15 = eh4.HandlerEnd == null;
				if (flag15)
				{
					scopeStack.Pop();
				}
			}
			Debug.Assert(scopeStack.Count == 1);
			return root;
		}
	}
}
