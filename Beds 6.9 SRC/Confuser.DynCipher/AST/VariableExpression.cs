using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.AST
{
	// Token: 0x0200002D RID: 45
	public class VariableExpression : Expression
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00006CF6 File Offset: 0x00004EF6
		// (set) Token: 0x060000EE RID: 238 RVA: 0x00006CFE File Offset: 0x00004EFE
		public Variable Variable
		{
			[CompilerGenerated]
			get
			{
				return this.<Variable>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Variable>k__BackingField = value;
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00006D08 File Offset: 0x00004F08
		public override string ToString()
		{
			return this.Variable.Name;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006783 File Offset: 0x00004983
		public VariableExpression()
		{
		}

		// Token: 0x04000066 RID: 102
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Variable <Variable>k__BackingField;
	}
}
