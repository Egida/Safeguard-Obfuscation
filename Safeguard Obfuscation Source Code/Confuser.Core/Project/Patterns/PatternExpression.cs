using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x020000A0 RID: 160
	public abstract class PatternExpression
	{
		// Token: 0x060003A3 RID: 931
		public abstract object Evaluate(IDnlibDef definition);

		// Token: 0x060003A4 RID: 932
		public abstract void Serialize(IList<PatternToken> tokens);

		// Token: 0x060003A5 RID: 933 RVA: 0x00002194 File Offset: 0x00000394
		protected PatternExpression()
		{
		}
	}
}
