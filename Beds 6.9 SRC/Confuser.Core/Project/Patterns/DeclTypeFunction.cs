using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000097 RID: 151
	public class DeclTypeFunction : PatternFunction
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600037E RID: 894 RVA: 0x0001581C File Offset: 0x00013A1C
		public override string Name
		{
			get
			{
				return "decl-type";
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600037F RID: 895 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00015834 File Offset: 0x00013A34
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = !(definition is IMemberDef) || ((IMemberDef)definition).DeclaringType == null;
			object result;
			if (flag)
			{
				result = false;
			}
			else
			{
				object fullName = base.Arguments[0].Evaluate(definition);
				result = (((IMemberDef)definition).DeclaringType.FullName == fullName.ToString());
			}
			return result;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0000385A File Offset: 0x00001A5A
		public DeclTypeFunction()
		{
		}

		// Token: 0x04000264 RID: 612
		internal const string FnName = "decl-type";
	}
}
