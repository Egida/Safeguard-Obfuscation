using System;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000099 RID: 153
	public class MatchNameFunction : PatternFunction
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000386 RID: 902 RVA: 0x000158F4 File Offset: 0x00013AF4
		public override string Name
		{
			get
			{
				return "match-name";
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000387 RID: 903 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0001590C File Offset: 0x00013B0C
		public override object Evaluate(IDnlibDef definition)
		{
			string regex = base.Arguments[0].Evaluate(definition).ToString();
			return Regex.IsMatch(definition.Name, regex);
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0000385A File Offset: 0x00001A5A
		public MatchNameFunction()
		{
		}

		// Token: 0x04000266 RID: 614
		internal const string FnName = "match-name";
	}
}
