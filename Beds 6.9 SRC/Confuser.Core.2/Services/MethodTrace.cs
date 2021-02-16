using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Services
{
	/// <summary>
	///     The trace result of a method.
	/// </summary>
	// Token: 0x02000081 RID: 129
	public class MethodTrace
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.Services.MethodTrace" /> class.
		/// </summary>
		/// <param name="method">The method to trace.</param>
		// Token: 0x060002E6 RID: 742 RVA: 0x00003416 File Offset: 0x00001616
		internal MethodTrace(MethodDef method)
		{
			this.method = method;
		}

		/// <summary>
		///     Gets the method this trace belongs to.
		/// </summary>
		/// <value>The method.</value>
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x00012CBC File Offset: 0x00010EBC
		public MethodDef Method
		{
			get
			{
				return this.method;
			}
		}

		/// <summary>
		///     Gets the instructions this trace is performed on.
		/// </summary>
		/// <value>The instructions.</value>
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x00003427 File Offset: 0x00001627
		// (set) Token: 0x060002E9 RID: 745 RVA: 0x0000342F File Offset: 0x0000162F
		public Instruction[] Instructions
		{
			[CompilerGenerated]
			get
			{
				return this.<Instructions>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Instructions>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the map of offset to index.
		/// </summary>
		/// <value>The map.</value>
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060002EA RID: 746 RVA: 0x00012CD4 File Offset: 0x00010ED4
		public Func<uint, int> OffsetToIndexMap
		{
			get
			{
				return (uint offset) => this.offset2index[offset];
			}
		}

		/// <summary>
		///     Gets the stack depths of method body.
		/// </summary>
		/// <value>The stack depths.</value>
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060002EB RID: 747 RVA: 0x00003438 File Offset: 0x00001638
		// (set) Token: 0x060002EC RID: 748 RVA: 0x00003440 File Offset: 0x00001640
		public int[] BeforeStackDepths
		{
			[CompilerGenerated]
			get
			{
				return this.<BeforeStackDepths>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<BeforeStackDepths>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the stack depths of method body.
		/// </summary>
		/// <value>The stack depths.</value>
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060002ED RID: 749 RVA: 0x00003449 File Offset: 0x00001649
		// (set) Token: 0x060002EE RID: 750 RVA: 0x00003451 File Offset: 0x00001651
		public int[] AfterStackDepths
		{
			[CompilerGenerated]
			get
			{
				return this.<AfterStackDepths>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<AfterStackDepths>k__BackingField = value;
			}
		}

		/// <summary>
		///     Determines whether the specified instruction is the target of a branch instruction.
		/// </summary>
		/// <param name="instrIndex">The index of instruction.</param>
		/// <returns><c>true</c> if the specified instruction is a branch target; otherwise, <c>false</c>.</returns>
		// Token: 0x060002EF RID: 751 RVA: 0x00012CF4 File Offset: 0x00010EF4
		public bool IsBranchTarget(int instrIndex)
		{
			return this.fromInstrs.ContainsKey(instrIndex);
		}

		/// <summary>
		///     Perform the actual tracing.
		/// </summary>
		/// <returns>This instance.</returns>
		/// <exception cref="T:dnlib.DotNet.Emit.InvalidMethodException">Bad method body.</exception>
		// Token: 0x060002F0 RID: 752 RVA: 0x00012D14 File Offset: 0x00010F14
		internal MethodTrace Trace()
		{
			CilBody body = this.method.Body;
			this.method.Body.UpdateInstructionOffsets();
			this.Instructions = this.method.Body.Instructions.ToArray<Instruction>();
			this.offset2index = new Dictionary<uint, int>();
			int[] beforeDepths = new int[body.Instructions.Count];
			int[] afterDepths = new int[body.Instructions.Count];
			this.fromInstrs = new Dictionary<int, List<Instruction>>();
			IList<Instruction> instrs = body.Instructions;
			for (int i = 0; i < instrs.Count; i++)
			{
				this.offset2index.Add(instrs[i].Offset, i);
				beforeDepths[i] = int.MinValue;
			}
			foreach (ExceptionHandler eh in body.ExceptionHandlers)
			{
				beforeDepths[this.offset2index[eh.TryStart.Offset]] = 0;
				beforeDepths[this.offset2index[eh.HandlerStart.Offset]] = ((eh.HandlerType != ExceptionHandlerType.Finally) ? 1 : 0);
				bool flag = eh.FilterStart != null;
				if (flag)
				{
					beforeDepths[this.offset2index[eh.FilterStart.Offset]] = 1;
				}
			}
			int currentStack = 0;
			int j = 0;
			while (j < instrs.Count)
			{
				Instruction instr = instrs[j];
				bool flag2 = beforeDepths[j] != int.MinValue;
				if (flag2)
				{
					currentStack = beforeDepths[j];
				}
				beforeDepths[j] = currentStack;
				instr.UpdateStack(ref currentStack);
				afterDepths[j] = currentStack;
				switch (instr.OpCode.FlowControl)
				{
				case FlowControl.Branch:
				{
					int index = this.offset2index[((Instruction)instr.Operand).Offset];
					bool flag3 = beforeDepths[index] == int.MinValue;
					if (flag3)
					{
						beforeDepths[index] = currentStack;
					}
					this.fromInstrs.AddListEntry(this.offset2index[((Instruction)instr.Operand).Offset], instr);
					currentStack = 0;
					break;
				}
				case FlowControl.Break:
					break;
				case FlowControl.Call:
				{
					bool flag4 = instr.OpCode.Code == Code.Jmp;
					if (flag4)
					{
						currentStack = 0;
					}
					break;
				}
				case FlowControl.Cond_Branch:
				{
					bool flag5 = instr.OpCode.Code == Code.Switch;
					if (flag5)
					{
						foreach (Instruction target in (Instruction[])instr.Operand)
						{
							int targetIndex = this.offset2index[target.Offset];
							bool flag6 = beforeDepths[targetIndex] == int.MinValue;
							if (flag6)
							{
								beforeDepths[targetIndex] = currentStack;
							}
							this.fromInstrs.AddListEntry(this.offset2index[target.Offset], instr);
						}
					}
					else
					{
						int targetIndex2 = this.offset2index[((Instruction)instr.Operand).Offset];
						bool flag7 = beforeDepths[targetIndex2] == int.MinValue;
						if (flag7)
						{
							beforeDepths[targetIndex2] = currentStack;
						}
						this.fromInstrs.AddListEntry(this.offset2index[((Instruction)instr.Operand).Offset], instr);
					}
					break;
				}
				case FlowControl.Meta:
					break;
				case FlowControl.Next:
					break;
				case FlowControl.Phi:
					goto IL_361;
				case FlowControl.Return:
					break;
				case FlowControl.Throw:
					break;
				default:
					goto IL_361;
				}
				j++;
				continue;
				IL_361:
				throw new UnreachableException();
			}
			foreach (int stackDepth in beforeDepths)
			{
				bool flag8 = stackDepth == int.MinValue;
				if (flag8)
				{
					throw new InvalidMethodException("Bad method body.");
				}
			}
			foreach (int stackDepth2 in afterDepths)
			{
				bool flag9 = stackDepth2 == int.MinValue;
				if (flag9)
				{
					throw new InvalidMethodException("Bad method body.");
				}
			}
			this.BeforeStackDepths = beforeDepths;
			this.AfterStackDepths = afterDepths;
			return this;
		}

		/// <summary>
		///     Traces the arguments of the specified call instruction.
		/// </summary>
		/// <param name="instr">The call instruction.</param>
		/// <returns>The indexes of the begin instruction of arguments.</returns>
		/// <exception cref="T:System.ArgumentException">The specified call instruction is invalid.</exception>
		/// <exception cref="T:dnlib.DotNet.Emit.InvalidMethodException">The method body is invalid.</exception>
		// Token: 0x060002F1 RID: 753 RVA: 0x0001313C File Offset: 0x0001133C
		public int[] TraceArguments(Instruction instr)
		{
			bool flag = instr.OpCode.Code != Code.Call && instr.OpCode.Code != Code.Callvirt && instr.OpCode.Code != Code.Newobj;
			if (flag)
			{
				throw new ArgumentException("Invalid call instruction.", "instr");
			}
			int push;
			int pop;
			instr.CalculateStackUsage(out push, out pop);
			bool flag2 = pop == 0;
			int[] result;
			if (flag2)
			{
				result = new int[0];
			}
			else
			{
				int instrIndex = this.offset2index[instr.Offset];
				int argCount = pop;
				int targetStack = this.BeforeStackDepths[instrIndex] - argCount;
				int beginInstrIndex = -1;
				HashSet<uint> seen = new HashSet<uint>();
				Queue<int> working = new Queue<int>();
				working.Enqueue(this.offset2index[instr.Offset] - 1);
				while (working.Count > 0)
				{
					int index;
					for (index = working.Dequeue(); index >= 0; index--)
					{
						bool flag3 = this.BeforeStackDepths[index] == targetStack;
						if (flag3)
						{
							break;
						}
						bool flag4 = this.fromInstrs.ContainsKey(index);
						if (flag4)
						{
							foreach (Instruction fromInstr in this.fromInstrs[index])
							{
								bool flag5 = !seen.Contains(fromInstr.Offset);
								if (flag5)
								{
									seen.Add(fromInstr.Offset);
									working.Enqueue(this.offset2index[fromInstr.Offset]);
								}
							}
						}
					}
					bool flag6 = index < 0;
					if (flag6)
					{
						return null;
					}
					bool flag7 = beginInstrIndex == -1;
					if (flag7)
					{
						beginInstrIndex = index;
					}
					else
					{
						bool flag8 = beginInstrIndex != index;
						if (flag8)
						{
							return null;
						}
					}
				}
				seen.Clear();
				Queue<Tuple<int, Stack<int>>> working2 = new Queue<Tuple<int, Stack<int>>>();
				working2.Clear();
				working2.Enqueue(Tuple.Create<int, Stack<int>>(beginInstrIndex, new Stack<int>()));
				int[] ret = null;
				while (working2.Count > 0)
				{
					Tuple<int, Stack<int>> tuple = working2.Dequeue();
					int index2 = tuple.Item1;
					Stack<int> evalStack = tuple.Item2;
					while (index2 != instrIndex && index2 < this.method.Body.Instructions.Count)
					{
						Instruction currentInstr = this.Instructions[index2];
						currentInstr.CalculateStackUsage(out push, out pop);
						int stackUsage = pop - push;
						bool flag9 = stackUsage < 0;
						if (flag9)
						{
							Debug.Assert(stackUsage == -1);
							evalStack.Push(index2);
						}
						else
						{
							bool flag10 = evalStack.Count < stackUsage;
							if (flag10)
							{
								return null;
							}
							for (int i = 0; i < stackUsage; i++)
							{
								evalStack.Pop();
							}
						}
						object instrOperand = currentInstr.Operand;
						bool flag11 = currentInstr.Operand is Instruction;
						if (flag11)
						{
							int targetIndex = this.offset2index[((Instruction)currentInstr.Operand).Offset];
							bool flag12 = currentInstr.OpCode.FlowControl == FlowControl.Branch;
							if (flag12)
							{
								index2 = targetIndex;
							}
							else
							{
								working2.Enqueue(Tuple.Create<int, Stack<int>>(targetIndex, new Stack<int>(evalStack)));
								index2++;
							}
						}
						else
						{
							bool flag13 = currentInstr.Operand is Instruction[];
							if (flag13)
							{
								foreach (Instruction targetInstr in (Instruction[])currentInstr.Operand)
								{
									working2.Enqueue(Tuple.Create<int, Stack<int>>(this.offset2index[targetInstr.Offset], new Stack<int>(evalStack)));
								}
								index2++;
							}
							else
							{
								index2++;
							}
						}
					}
					bool flag14 = evalStack.Count != argCount;
					if (flag14)
					{
						return null;
					}
					bool flag15 = ret != null && !evalStack.SequenceEqual(ret);
					if (flag15)
					{
						return null;
					}
					ret = evalStack.ToArray();
				}
				bool flag16 = ret == null;
				if (flag16)
				{
					result = ret;
				}
				else
				{
					Array.Reverse(ret);
					result = ret;
				}
			}
			return result;
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0000345A File Offset: 0x0000165A
		[CompilerGenerated]
		private int <get_OffsetToIndexMap>b__11_0(uint offset)
		{
			return this.offset2index[offset];
		}

		// Token: 0x0400022D RID: 557
		private readonly MethodDef method;

		// Token: 0x0400022E RID: 558
		private Dictionary<int, List<Instruction>> fromInstrs;

		// Token: 0x0400022F RID: 559
		private Dictionary<uint, int> offset2index;

		// Token: 0x04000230 RID: 560
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private Instruction[] <Instructions>k__BackingField;

		// Token: 0x04000231 RID: 561
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int[] <BeforeStackDepths>k__BackingField;

		// Token: 0x04000232 RID: 562
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int[] <AfterStackDepths>k__BackingField;
	}
}
