using System;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200009C RID: 156
	public class NamespaceFunction : PatternFunction
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000392 RID: 914 RVA: 0x00015AC4 File Offset: 0x00013CC4
		public override string Name
		{
			get
			{
				return "namespace";
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000393 RID: 915 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00015ADC File Offset: 0x00013CDC
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = !(definition is TypeDef) && !(definition is IMemberDef);
			object result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string ns = "^" + base.Arguments[0].Evaluate(definition).ToString() + "$";
				TypeDef type = definition as TypeDef;
				bool flag2 = type == null;
				if (flag2)
				{
					type = ((IMemberDef)definition).DeclaringType;
				}
				bool flag3 = type == null;
				if (flag3)
				{
					result = false;
				}
				else
				{
					while (type.IsNested)
					{
						type = type.DeclaringType;
					}
					result = (type != null && Regex.IsMatch(type.Namespace ?? "", ns));
				}
			}
			return result;
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0000385A File Offset: 0x00001A5A
		public NamespaceFunction()
		{
		}

		// Token: 0x04000269 RID: 617
		internal const string FnName = "namespace";
	}
}
