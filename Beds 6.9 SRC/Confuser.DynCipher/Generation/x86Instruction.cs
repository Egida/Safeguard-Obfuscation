using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000017 RID: 23
	public class x86Instruction
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00004CDB File Offset: 0x00002EDB
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00004CE3 File Offset: 0x00002EE3
		public x86OpCode OpCode
		{
			[CompilerGenerated]
			get
			{
				return this.<OpCode>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<OpCode>k__BackingField = value;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00004CEC File Offset: 0x00002EEC
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00004CF4 File Offset: 0x00002EF4
		public Ix86Operand[] Operands
		{
			[CompilerGenerated]
			get
			{
				return this.<Operands>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Operands>k__BackingField = value;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004D00 File Offset: 0x00002F00
		public static x86Instruction Create(x86OpCode opCode, params Ix86Operand[] operands)
		{
			return new x86Instruction
			{
				OpCode = opCode,
				Operands = operands
			};
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004D2C File Offset: 0x00002F2C
		public byte[] Assemble()
		{
			switch (this.OpCode)
			{
			case x86OpCode.MOV:
			{
				bool flag = this.Operands.Length != 2;
				if (flag)
				{
					throw new InvalidOperationException();
				}
				bool flag2 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag2)
				{
					byte[] ret = new byte[]
					{
						137,
						192
					};
					byte[] array = ret;
					int num = 1;
					array[num] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array2 = ret;
					int num2 = 1;
					array2[num2] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return ret;
				}
				bool flag3 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag3)
				{
					byte[] ret2 = new byte[5];
					ret2[0] = 184;
					byte[] array3 = ret2;
					int num3 = 0;
					array3[num3] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, ret2, 1, 4);
					return ret2;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.ADD:
			{
				bool flag4 = this.Operands.Length != 2;
				if (flag4)
				{
					throw new InvalidOperationException();
				}
				bool flag5 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag5)
				{
					byte[] ret3 = new byte[]
					{
						1,
						192
					};
					byte[] array4 = ret3;
					int num4 = 1;
					array4[num4] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array5 = ret3;
					int num5 = 1;
					array5[num5] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return ret3;
				}
				bool flag6 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag6)
				{
					byte[] ret4 = new byte[6];
					ret4[0] = 129;
					ret4[1] = 192;
					byte[] array6 = ret4;
					int num6 = 1;
					array6[num6] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, ret4, 2, 4);
					return ret4;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.SUB:
			{
				bool flag7 = this.Operands.Length != 2;
				if (flag7)
				{
					throw new InvalidOperationException();
				}
				bool flag8 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag8)
				{
					byte[] ret5 = new byte[]
					{
						41,
						192
					};
					byte[] array7 = ret5;
					int num7 = 1;
					array7[num7] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array8 = ret5;
					int num8 = 1;
					array8[num8] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return ret5;
				}
				bool flag9 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag9)
				{
					byte[] ret6 = new byte[6];
					ret6[0] = 129;
					ret6[1] = 232;
					byte[] array9 = ret6;
					int num9 = 1;
					array9[num9] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, ret6, 2, 4);
					return ret6;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.IMUL:
			{
				bool flag10 = this.Operands.Length != 2;
				if (flag10)
				{
					throw new InvalidOperationException();
				}
				bool flag11 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag11)
				{
					byte[] ret7 = new byte[3];
					ret7[0] = 15;
					ret7[1] = 175;
					ret7[1] = 192;
					byte[] array10 = ret7;
					int num10 = 1;
					array10[num10] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array11 = ret7;
					int num11 = 1;
					array11[num11] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return ret7;
				}
				bool flag12 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag12)
				{
					byte[] ret8 = new byte[6];
					ret8[0] = 105;
					ret8[1] = 192;
					byte[] array12 = ret8;
					int num12 = 1;
					array12[num12] |= (byte)((this.Operands[0] as x86RegisterOperand).Register << 3);
					byte[] array13 = ret8;
					int num13 = 1;
					array13[num13] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, ret8, 2, 4);
					return ret8;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.NEG:
			{
				bool flag13 = this.Operands.Length != 1;
				if (flag13)
				{
					throw new InvalidOperationException();
				}
				bool flag14 = this.Operands[0] is x86RegisterOperand;
				if (flag14)
				{
					byte[] ret9 = new byte[]
					{
						247,
						216
					};
					byte[] array14 = ret9;
					int num14 = 1;
					array14[num14] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return ret9;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.NOT:
			{
				bool flag15 = this.Operands.Length != 1;
				if (flag15)
				{
					throw new InvalidOperationException();
				}
				bool flag16 = this.Operands[0] is x86RegisterOperand;
				if (flag16)
				{
					byte[] ret10 = new byte[]
					{
						247,
						208
					};
					byte[] array15 = ret10;
					int num15 = 1;
					array15[num15] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return ret10;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.XOR:
			{
				bool flag17 = this.Operands.Length != 2;
				if (flag17)
				{
					throw new InvalidOperationException();
				}
				bool flag18 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag18)
				{
					byte[] ret11 = new byte[]
					{
						49,
						192
					};
					byte[] array16 = ret11;
					int num16 = 1;
					array16[num16] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array17 = ret11;
					int num17 = 1;
					array17[num17] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return ret11;
				}
				bool flag19 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag19)
				{
					byte[] ret12 = new byte[6];
					ret12[0] = 129;
					ret12[1] = 240;
					byte[] array18 = ret12;
					int num18 = 1;
					array18[num18] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, ret12, 2, 4);
					return ret12;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.POP:
			{
				bool flag20 = this.Operands.Length != 1;
				if (flag20)
				{
					throw new InvalidOperationException();
				}
				bool flag21 = this.Operands[0] is x86RegisterOperand;
				if (flag21)
				{
					byte[] ret13 = new byte[]
					{
						88
					};
					byte[] array19 = ret13;
					int num19 = 0;
					array19[num19] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return ret13;
				}
				throw new NotSupportedException();
			}
			}
			throw new NotSupportedException();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005518 File Offset: 0x00003718
		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			ret.Append(this.OpCode);
			for (int i = 0; i < this.Operands.Length; i++)
			{
				ret.AppendFormat("{0}{1}", (i == 0) ? " " : ", ", this.Operands[i]);
			}
			return ret.ToString();
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000020FC File Offset: 0x000002FC
		public x86Instruction()
		{
		}

		// Token: 0x04000030 RID: 48
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private x86OpCode <OpCode>k__BackingField;

		// Token: 0x04000031 RID: 49
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Ix86Operand[] <Operands>k__BackingField;
	}
}
