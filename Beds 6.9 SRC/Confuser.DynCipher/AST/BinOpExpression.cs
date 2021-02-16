using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000023 RID: 35
	public class BinOpExpression : Expression
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x000067E0 File Offset: 0x000049E0
		public override string ToString()
		{
			string op;
			switch (this.Operation)
			{
			case BinOps.Add:
				op = "+";
				break;
			case BinOps.Sub:
				op = "-";
				break;
			case BinOps.Div:
				op = "/";
				break;
			case BinOps.Mul:
				op = "*";
				break;
			case BinOps.Or:
				op = "|";
				break;
			case BinOps.And:
				op = "&";
				break;
			case BinOps.Xor:
				op = "^";
				break;
			case BinOps.Lsh:
				op = "<<";
				break;
			case BinOps.Rsh:
				op = ">>";
				break;
			default:
				throw new Exception();
			}
			return string.Format("({0} {1} {2})", this.Left, op, this.Right);
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x0000688A File Offset: 0x00004A8A
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00006892 File Offset: 0x00004A92
		public Expression Left
		{
			[CompilerGenerated]
			get
			{
				return this.<Left>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Left>k__BackingField = value;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000BB RID: 187 RVA: 0x0000689B File Offset: 0x00004A9B
		// (set) Token: 0x060000BC RID: 188 RVA: 0x000068A3 File Offset: 0x00004AA3
		public BinOps Operation
		{
			[CompilerGenerated]
			get
			{
				return this.<Operation>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Operation>k__BackingField = value;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000BD RID: 189 RVA: 0x000068AC File Offset: 0x00004AAC
		// (set) Token: 0x060000BE RID: 190 RVA: 0x000068B4 File Offset: 0x00004AB4
		public Expression Right
		{
			[CompilerGenerated]
			get
			{
				return this.<Right>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Right>k__BackingField = value;
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00006783 File Offset: 0x00004983
		public BinOpExpression()
		{
		}

		// Token: 0x0400004C RID: 76
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Expression <Left>k__BackingField;

		// Token: 0x0400004D RID: 77
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BinOps <Operation>k__BackingField;

		// Token: 0x0400004E RID: 78
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Expression <Right>k__BackingField;
	}
}
