using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000022 RID: 34
	public class AssignmentStatement : Statement
	{
		// Token: 0x060000B2 RID: 178 RVA: 0x0000678C File Offset: 0x0000498C
		public override string ToString()
		{
			return string.Format("{0} = {1};", this.Target, this.Value);
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x000067B4 File Offset: 0x000049B4
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x000067BC File Offset: 0x000049BC
		public Expression Target
		{
			[CompilerGenerated]
			get
			{
				return this.<Target>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Target>k__BackingField = value;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000067C5 File Offset: 0x000049C5
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x000067CD File Offset: 0x000049CD
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

		// Token: 0x060000B7 RID: 183 RVA: 0x000067D6 File Offset: 0x000049D6
		public AssignmentStatement()
		{
		}

		// Token: 0x0400004A RID: 74
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Expression <Target>k__BackingField;

		// Token: 0x0400004B RID: 75
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Expression <Value>k__BackingField;
	}
}
