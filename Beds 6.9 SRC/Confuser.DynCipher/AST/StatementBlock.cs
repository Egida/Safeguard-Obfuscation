using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000029 RID: 41
	public class StatementBlock : Statement
	{
		// Token: 0x060000DD RID: 221 RVA: 0x00006B98 File Offset: 0x00004D98
		public StatementBlock()
		{
			this.Statements = new List<Statement>();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00006BB0 File Offset: 0x00004DB0
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("{");
			foreach (Statement i in this.Statements)
			{
				sb.AppendLine(i.ToString());
			}
			sb.AppendLine("}");
			return sb.ToString();
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00006C30 File Offset: 0x00004E30
		// (set) Token: 0x060000E0 RID: 224 RVA: 0x00006C38 File Offset: 0x00004E38
		public IList<Statement> Statements
		{
			[CompilerGenerated]
			get
			{
				return this.<Statements>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Statements>k__BackingField = value;
			}
		}

		// Token: 0x0400005E RID: 94
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<Statement> <Statements>k__BackingField;
	}
}
