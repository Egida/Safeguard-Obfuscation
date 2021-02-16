using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000027 RID: 39
	public class LoopStatement : StatementBlock
	{
		// Token: 0x060000D3 RID: 211 RVA: 0x00006B08 File Offset: 0x00004D08
		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			ret.AppendFormat("for (int i = {0}; i < {1}; i++)", this.Begin, this.Limit);
			ret.AppendLine();
			ret.Append(base.ToString());
			return ret.ToString();
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00006B5C File Offset: 0x00004D5C
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x00006B64 File Offset: 0x00004D64
		public int Begin
		{
			[CompilerGenerated]
			get
			{
				return this.<Begin>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Begin>k__BackingField = value;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00006B6D File Offset: 0x00004D6D
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00006B75 File Offset: 0x00004D75
		public int Limit
		{
			[CompilerGenerated]
			get
			{
				return this.<Limit>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Limit>k__BackingField = value;
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00006B7E File Offset: 0x00004D7E
		public LoopStatement()
		{
		}

		// Token: 0x0400005B RID: 91
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int <Begin>k__BackingField;

		// Token: 0x0400005C RID: 92
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int <Limit>k__BackingField;
	}
}
