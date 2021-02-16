using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200009F RID: 159
	public class NameFunction : PatternFunction
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600039F RID: 927 RVA: 0x00015C88 File Offset: 0x00013E88
		public override string Name
		{
			get
			{
				return "name";
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00015CA0 File Offset: 0x00013EA0
		public override object Evaluate(IDnlibDef definition)
		{
			object name = base.Arguments[0].Evaluate(definition);
			return definition.Name == name.ToString();
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0000385A File Offset: 0x00001A5A
		public NameFunction()
		{
		}

		// Token: 0x0400026C RID: 620
		internal const string FnName = "name";
	}
}
