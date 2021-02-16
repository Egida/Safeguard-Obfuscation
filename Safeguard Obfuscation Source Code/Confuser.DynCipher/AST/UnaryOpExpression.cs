using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.AST
{
	// Token: 0x0200002B RID: 43
	public class UnaryOpExpression : Expression
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00006C41 File Offset: 0x00004E41
		// (set) Token: 0x060000E2 RID: 226 RVA: 0x00006C49 File Offset: 0x00004E49
		public Expression Value
		{
			[CompilerGenerated]
			get
			{
				return this.<Value>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Value>k__BackingField = value;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00006C52 File Offset: 0x00004E52
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x00006C5A File Offset: 0x00004E5A
		public UnaryOps Operation
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

		// Token: 0x060000E5 RID: 229 RVA: 0x00006C64 File Offset: 0x00004E64
		public override string ToString()
		{
			UnaryOps operation = this.Operation;
			string op;
			if (operation != UnaryOps.Not)
			{
				if (operation != UnaryOps.Negate)
				{
					throw new Exception();
				}
				op = "-";
			}
			else
			{
				op = "~";
			}
			return op + this.Value;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006783 File Offset: 0x00004983
		public UnaryOpExpression()
		{
		}

		// Token: 0x04000062 RID: 98
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Expression <Value>k__BackingField;

		// Token: 0x04000063 RID: 99
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private UnaryOps <Operation>k__BackingField;
	}
}
