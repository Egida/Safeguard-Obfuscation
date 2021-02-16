using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	/// <summary>
	///     A function that indicate whether the type inherits from the specified type.
	/// </summary>
	// Token: 0x02000092 RID: 146
	public class InheritsFunction : PatternFunction
	{
		/// <inheritdoc />
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600036A RID: 874 RVA: 0x000153D8 File Offset: 0x000135D8
		public override string Name
		{
			get
			{
				return "inherits";
			}
		}

		/// <inheritdoc />
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600036B RID: 875 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		/// <inheritdoc />
		// Token: 0x0600036C RID: 876 RVA: 0x000153F0 File Offset: 0x000135F0
		public override object Evaluate(IDnlibDef definition)
		{
			string name = base.Arguments[0].Evaluate(definition).ToString();
			TypeDef type = definition as TypeDef;
			bool flag = type == null && definition is IMemberDef;
			if (flag)
			{
				type = ((IMemberDef)definition).DeclaringType;
			}
			bool flag2 = type == null;
			object result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = type.InheritsFrom(name) || type.Implements(name);
				if (flag3)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x0000385A File Offset: 0x00001A5A
		public InheritsFunction()
		{
		}

		// Token: 0x0400025F RID: 607
		internal const string FnName = "inherits";
	}
}
