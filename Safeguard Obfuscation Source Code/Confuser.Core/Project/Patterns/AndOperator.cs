using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200008F RID: 143
	public class AndOperator : PatternOperator
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600035E RID: 862 RVA: 0x000151CC File Offset: 0x000133CC
		public override string Name
		{
			get
			{
				return "and";
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600035F RID: 863 RVA: 0x0000DECC File Offset: 0x0000C0CC
		public override bool IsUnary
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000360 RID: 864 RVA: 0x000151E4 File Offset: 0x000133E4
		public override object Evaluate(IDnlibDef definition)
		{
			bool a = (bool)base.OperandA.Evaluate(definition);
			bool flag = !a;
			object result;
			if (flag)
			{
				result = false;
			}
			else
			{
				result = (bool)base.OperandB.Evaluate(definition);
			}
			return result;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x00003851 File Offset: 0x00001A51
		public AndOperator()
		{
		}

		// Token: 0x0400025C RID: 604
		internal const string OpName = "and";
	}
}
