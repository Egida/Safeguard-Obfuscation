using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000094 RID: 148
	public class FullNameFunction : PatternFunction
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000372 RID: 882 RVA: 0x000155D8 File Offset: 0x000137D8
		public override string Name
		{
			get
			{
				return "full-name";
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000373 RID: 883 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000374 RID: 884 RVA: 0x000155F0 File Offset: 0x000137F0
		public override object Evaluate(IDnlibDef definition)
		{
			object name = base.Arguments[0].Evaluate(definition);
			return definition.FullName == name.ToString();
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0000385A File Offset: 0x00001A5A
		public FullNameFunction()
		{
		}

		// Token: 0x04000261 RID: 609
		internal const string FnName = "full-name";
	}
}
