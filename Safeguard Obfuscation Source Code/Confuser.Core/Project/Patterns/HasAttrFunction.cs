using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000090 RID: 144
	public class HasAttrFunction : PatternFunction
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000362 RID: 866 RVA: 0x00015230 File Offset: 0x00013430
		public override string Name
		{
			get
			{
				return "has-attr";
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000363 RID: 867 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0001525C File Offset: 0x0001345C
		public override object Evaluate(IDnlibDef definition)
		{
			string attrName = base.Arguments[0].Evaluate(definition).ToString();
			return definition.CustomAttributes.IsDefined(attrName);
		}

		// Token: 0x06000365 RID: 869 RVA: 0x0000385A File Offset: 0x00001A5A
		public HasAttrFunction()
		{
		}

		// Token: 0x0400025D RID: 605
		internal const string FnName = "has-attr";
	}
}
