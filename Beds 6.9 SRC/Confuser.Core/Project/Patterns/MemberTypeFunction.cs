using System;
using System.Text;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000096 RID: 150
	public class MemberTypeFunction : PatternFunction
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600037A RID: 890 RVA: 0x00015670 File Offset: 0x00013870
		public override string Name
		{
			get
			{
				return "member-type";
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600037B RID: 891 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00015688 File Offset: 0x00013888
		public override object Evaluate(IDnlibDef definition)
		{
			string typeRegex = base.Arguments[0].Evaluate(definition).ToString();
			StringBuilder memberType = new StringBuilder();
			bool flag = definition is TypeDef;
			if (flag)
			{
				memberType.Append("type ");
			}
			bool flag2 = definition is MethodDef;
			if (flag2)
			{
				memberType.Append("method ");
				MethodDef method = (MethodDef)definition;
				bool isGetter = method.IsGetter;
				if (isGetter)
				{
					memberType.Append("propertym getter ");
				}
				else
				{
					bool isSetter = method.IsSetter;
					if (isSetter)
					{
						memberType.Append("propertym setter ");
					}
					else
					{
						bool isAddOn = method.IsAddOn;
						if (isAddOn)
						{
							memberType.Append("eventm add ");
						}
						else
						{
							bool isRemoveOn = method.IsRemoveOn;
							if (isRemoveOn)
							{
								memberType.Append("eventm remove ");
							}
							else
							{
								bool isFire = method.IsFire;
								if (isFire)
								{
									memberType.Append("eventm fire ");
								}
								else
								{
									bool isOther = method.IsOther;
									if (isOther)
									{
										memberType.Append("other ");
									}
								}
							}
						}
					}
				}
			}
			bool flag3 = definition is FieldDef;
			if (flag3)
			{
				memberType.Append("field ");
			}
			bool flag4 = definition is PropertyDef;
			if (flag4)
			{
				memberType.Append("property ");
			}
			bool flag5 = definition is EventDef;
			if (flag5)
			{
				memberType.Append("event ");
			}
			bool flag6 = definition is ModuleDef;
			if (flag6)
			{
				memberType.Append("module ");
			}
			return Regex.IsMatch(memberType.ToString(), typeRegex);
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0000385A File Offset: 0x00001A5A
		public MemberTypeFunction()
		{
		}

		// Token: 0x04000263 RID: 611
		internal const string FnName = "member-type";
	}
}
