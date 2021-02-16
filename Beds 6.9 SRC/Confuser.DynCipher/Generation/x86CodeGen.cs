using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000011 RID: 17
	public class x86CodeGen
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000045AC File Offset: 0x000027AC
		public IList<x86Instruction> Instructions
		{
			get
			{
				return this.instrs;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000045C4 File Offset: 0x000027C4
		// (set) Token: 0x06000055 RID: 85 RVA: 0x000045CC File Offset: 0x000027CC
		public int MaxUsedRegister
		{
			[CompilerGenerated]
			get
			{
				return this.<MaxUsedRegister>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<MaxUsedRegister>k__BackingField = value;
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000045D8 File Offset: 0x000027D8
		public x86Register? GenerateX86(Expression expression, Func<Variable, x86Register, IEnumerable<x86Instruction>> loadArg)
		{
			this.instrs = new List<x86Instruction>();
			this.usedRegs = new bool[8];
			this.MaxUsedRegister = -1;
			this.usedRegs[5] = true;
			this.usedRegs[4] = true;
			x86Register? result;
			try
			{
				result = new x86Register?(((x86RegisterOperand)this.Emit(expression, loadArg)).Register);
			}
			catch (Exception ex)
			{
				bool flag = ex.Message == "Register overflowed.";
				if (!flag)
				{
					throw;
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004668 File Offset: 0x00002868
		private x86Register GetFreeRegister()
		{
			for (int i = 0; i < 8; i++)
			{
				bool flag = !this.usedRegs[i];
				if (flag)
				{
					return (x86Register)i;
				}
			}
			throw new Exception("Register overflowed.");
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000046A8 File Offset: 0x000028A8
		private void TakeRegister(x86Register reg)
		{
			this.usedRegs[(int)reg] = true;
			bool flag = reg > (x86Register)this.MaxUsedRegister;
			if (flag)
			{
				this.MaxUsedRegister = (int)reg;
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000046D4 File Offset: 0x000028D4
		private void ReleaseRegister(x86Register reg)
		{
			this.usedRegs[(int)reg] = false;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000046E0 File Offset: 0x000028E0
		private x86Register Normalize(x86Instruction instr)
		{
			bool flag = instr.Operands.Length == 2 && instr.Operands[0] is x86ImmediateOperand && instr.Operands[1] is x86ImmediateOperand;
			x86Register result;
			if (flag)
			{
				x86Register reg = this.GetFreeRegister();
				this.instrs.Add(x86Instruction.Create(x86OpCode.MOV, new Ix86Operand[]
				{
					new x86RegisterOperand(reg),
					instr.Operands[0]
				}));
				instr.Operands[0] = new x86RegisterOperand(reg);
				this.instrs.Add(instr);
				result = reg;
			}
			else
			{
				bool flag2 = instr.Operands.Length == 1 && instr.Operands[0] is x86ImmediateOperand;
				if (flag2)
				{
					x86Register reg2 = this.GetFreeRegister();
					this.instrs.Add(x86Instruction.Create(x86OpCode.MOV, new Ix86Operand[]
					{
						new x86RegisterOperand(reg2),
						instr.Operands[0]
					}));
					instr.Operands[0] = new x86RegisterOperand(reg2);
					this.instrs.Add(instr);
					result = reg2;
				}
				else
				{
					bool flag3 = instr.OpCode == x86OpCode.SUB && instr.Operands[0] is x86ImmediateOperand && instr.Operands[1] is x86RegisterOperand;
					if (flag3)
					{
						x86Register reg3 = ((x86RegisterOperand)instr.Operands[1]).Register;
						this.instrs.Add(x86Instruction.Create(x86OpCode.NEG, new Ix86Operand[]
						{
							new x86RegisterOperand(reg3)
						}));
						instr.OpCode = x86OpCode.ADD;
						instr.Operands[1] = instr.Operands[0];
						instr.Operands[0] = new x86RegisterOperand(reg3);
						this.instrs.Add(instr);
						result = reg3;
					}
					else
					{
						bool flag4 = instr.Operands.Length == 2 && instr.Operands[0] is x86ImmediateOperand && instr.Operands[1] is x86RegisterOperand;
						if (flag4)
						{
							x86Register reg4 = ((x86RegisterOperand)instr.Operands[1]).Register;
							instr.Operands[1] = instr.Operands[0];
							instr.Operands[0] = new x86RegisterOperand(reg4);
							this.instrs.Add(instr);
							result = reg4;
						}
						else
						{
							Debug.Assert(instr.Operands.Length != 0);
							Debug.Assert(instr.Operands[0] is x86RegisterOperand);
							bool flag5 = instr.Operands.Length == 2 && instr.Operands[1] is x86RegisterOperand;
							if (flag5)
							{
								this.ReleaseRegister(((x86RegisterOperand)instr.Operands[1]).Register);
							}
							this.instrs.Add(instr);
							result = ((x86RegisterOperand)instr.Operands[0]).Register;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004998 File Offset: 0x00002B98
		private Ix86Operand Emit(Expression exp, Func<Variable, x86Register, IEnumerable<x86Instruction>> loadArg)
		{
			bool flag = exp is BinOpExpression;
			Ix86Operand result;
			if (flag)
			{
				BinOpExpression binOp = (BinOpExpression)exp;
				x86Register reg;
				switch (binOp.Operation)
				{
				case BinOps.Add:
					reg = this.Normalize(x86Instruction.Create(x86OpCode.ADD, new Ix86Operand[]
					{
						this.Emit(binOp.Left, loadArg),
						this.Emit(binOp.Right, loadArg)
					}));
					goto IL_124;
				case BinOps.Sub:
					reg = this.Normalize(x86Instruction.Create(x86OpCode.SUB, new Ix86Operand[]
					{
						this.Emit(binOp.Left, loadArg),
						this.Emit(binOp.Right, loadArg)
					}));
					goto IL_124;
				case BinOps.Mul:
					reg = this.Normalize(x86Instruction.Create(x86OpCode.IMUL, new Ix86Operand[]
					{
						this.Emit(binOp.Left, loadArg),
						this.Emit(binOp.Right, loadArg)
					}));
					goto IL_124;
				case BinOps.Xor:
					reg = this.Normalize(x86Instruction.Create(x86OpCode.XOR, new Ix86Operand[]
					{
						this.Emit(binOp.Left, loadArg),
						this.Emit(binOp.Right, loadArg)
					}));
					goto IL_124;
				}
				throw new NotSupportedException();
				IL_124:
				this.TakeRegister(reg);
				result = new x86RegisterOperand(reg);
			}
			else
			{
				bool flag2 = exp is UnaryOpExpression;
				if (flag2)
				{
					UnaryOpExpression unaryOp = (UnaryOpExpression)exp;
					UnaryOps operation = unaryOp.Operation;
					x86Register reg2;
					if (operation != UnaryOps.Not)
					{
						if (operation != UnaryOps.Negate)
						{
							throw new NotSupportedException();
						}
						reg2 = this.Normalize(x86Instruction.Create(x86OpCode.NEG, new Ix86Operand[]
						{
							this.Emit(unaryOp.Value, loadArg)
						}));
					}
					else
					{
						reg2 = this.Normalize(x86Instruction.Create(x86OpCode.NOT, new Ix86Operand[]
						{
							this.Emit(unaryOp.Value, loadArg)
						}));
					}
					this.TakeRegister(reg2);
					result = new x86RegisterOperand(reg2);
				}
				else
				{
					bool flag3 = exp is LiteralExpression;
					if (flag3)
					{
						result = new x86ImmediateOperand((int)((LiteralExpression)exp).Value);
					}
					else
					{
						bool flag4 = exp is VariableExpression;
						if (!flag4)
						{
							throw new NotSupportedException();
						}
						x86Register reg3 = this.GetFreeRegister();
						this.TakeRegister(reg3);
						this.instrs.AddRange(loadArg(((VariableExpression)exp).Variable, reg3));
						result = new x86RegisterOperand(reg3);
					}
				}
			}
			return result;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004BF0 File Offset: 0x00002DF0
		public override string ToString()
		{
			return string.Join("\r\n", (from instr in this.instrs
			select instr.ToString()).ToArray<string>());
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000020FC File Offset: 0x000002FC
		public x86CodeGen()
		{
		}

		// Token: 0x04000018 RID: 24
		private List<x86Instruction> instrs;

		// Token: 0x04000019 RID: 25
		private bool[] usedRegs;

		// Token: 0x0400001A RID: 26
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int <MaxUsedRegister>k__BackingField;

		// Token: 0x02000038 RID: 56
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000122 RID: 290 RVA: 0x0000760C File Offset: 0x0000580C
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000123 RID: 291 RVA: 0x000020FC File Offset: 0x000002FC
			public <>c()
			{
			}

			// Token: 0x06000124 RID: 292 RVA: 0x00007618 File Offset: 0x00005818
			internal string <ToString>b__14_0(x86Instruction instr)
			{
				return instr.ToString();
			}

			// Token: 0x0400009B RID: 155
			public static readonly x86CodeGen.<>c <>9 = new x86CodeGen.<>c();

			// Token: 0x0400009C RID: 156
			public static Func<x86Instruction, string> <>9__14_0;
		}
	}
}
