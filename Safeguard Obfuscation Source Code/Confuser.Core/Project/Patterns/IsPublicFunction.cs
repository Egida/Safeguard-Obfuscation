using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000093 RID: 147
	public class IsPublicFunction : PatternFunction
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600036E RID: 878 RVA: 0x00015480 File Offset: 0x00013680
		public override string Name
		{
			get
			{
				return "is-public";
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600036F RID: 879 RVA: 0x00015498 File Offset: 0x00013698
		public override int ArgumentCount
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06000370 RID: 880 RVA: 0x000154AC File Offset: 0x000136AC
		public override object Evaluate(IDnlibDef definition)
		{
			IMemberDef member = definition as IMemberDef;
			bool flag = member == null;
			object result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (TypeDef declType = ((IMemberDef)definition).DeclaringType; declType != null; declType = declType.DeclaringType)
				{
					bool flag2 = !declType.IsPublic;
					if (flag2)
					{
						return false;
					}
				}
				bool flag3 = member is MethodDef;
				if (flag3)
				{
					result = ((MethodDef)member).IsPublic;
				}
				else
				{
					bool flag4 = member is FieldDef;
					if (flag4)
					{
						result = ((FieldDef)member).IsPublic;
					}
					else
					{
						bool flag5 = member is PropertyDef;
						if (flag5)
						{
							result = ((PropertyDef)member).IsPublic();
						}
						else
						{
							bool flag6 = member is EventDef;
							if (flag6)
							{
								result = ((EventDef)member).IsPublic();
							}
							else
							{
								bool flag7 = member is TypeDef;
								if (!flag7)
								{
									throw new NotSupportedException();
								}
								result = (((TypeDef)member).IsPublic || ((TypeDef)member).IsNestedPublic);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0000385A File Offset: 0x00001A5A
		public IsPublicFunction()
		{
		}

		// Token: 0x04000260 RID: 608
		internal const string FnName = "is-public";
	}
}
