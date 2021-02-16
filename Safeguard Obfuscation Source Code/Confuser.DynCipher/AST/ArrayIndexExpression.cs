using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000021 RID: 33
	public class ArrayIndexExpression : Expression
	{
		// Token: 0x060000AC RID: 172 RVA: 0x00006734 File Offset: 0x00004934
		public override string ToString()
		{
			return string.Format("{0}[{1}]", this.Array, this.Index);
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00006761 File Offset: 0x00004961
		// (set) Token: 0x060000AE RID: 174 RVA: 0x00006769 File Offset: 0x00004969
		public Expression Array
		{
			[CompilerGenerated]
			get
			{
				return this.<Array>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Array>k__BackingField = value;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00006772 File Offset: 0x00004972
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x0000677A File Offset: 0x0000497A
		public int Index
		{
			[CompilerGenerated]
			get
			{
				return this.<Index>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Index>k__BackingField = value;
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00006783 File Offset: 0x00004983
		public ArrayIndexExpression()
		{
		}

		// Token: 0x04000048 RID: 72
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Expression <Array>k__BackingField;

		// Token: 0x04000049 RID: 73
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int <Index>k__BackingField;
	}
}
