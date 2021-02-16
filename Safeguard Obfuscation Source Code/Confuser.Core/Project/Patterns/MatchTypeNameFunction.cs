using System;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200009A RID: 154
	public class MatchTypeNameFunction : PatternFunction
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600038A RID: 906 RVA: 0x0001594C File Offset: 0x00013B4C
		public override string Name
		{
			get
			{
				return "match-type-name";
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600038B RID: 907 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00015964 File Offset: 0x00013B64
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = definition is TypeDef;
			object result;
			if (flag)
			{
				string regex = base.Arguments[0].Evaluate(definition).ToString();
				result = Regex.IsMatch(definition.Name, regex);
			}
			else
			{
				bool flag2 = definition is IMemberDef && ((IMemberDef)definition).DeclaringType != null;
				if (flag2)
				{
					string regex2 = base.Arguments[0].Evaluate(definition).ToString();
					result = Regex.IsMatch(((IMemberDef)definition).DeclaringType.Name, regex2);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0000385A File Offset: 0x00001A5A
		public MatchTypeNameFunction()
		{
		}

		// Token: 0x04000267 RID: 615
		internal const string FnName = "match-type-name";
	}
}
