using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	/// <summary>
	///     The OR operator.
	/// </summary>
	// Token: 0x0200009D RID: 157
	public class OrOperator : PatternOperator
	{
		/// <inheritdoc />
		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000396 RID: 918 RVA: 0x00015BAC File Offset: 0x00013DAC
		public override string Name
		{
			get
			{
				return "or";
			}
		}

		/// <inheritdoc />
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000397 RID: 919 RVA: 0x0000DECC File Offset: 0x0000C0CC
		public override bool IsUnary
		{
			get
			{
				return false;
			}
		}

		/// <inheritdoc />
		// Token: 0x06000398 RID: 920 RVA: 0x00015BC4 File Offset: 0x00013DC4
		public override object Evaluate(IDnlibDef definition)
		{
			bool a = (bool)base.OperandA.Evaluate(definition);
			bool flag = a;
			object result;
			if (flag)
			{
				result = true;
			}
			else
			{
				result = (bool)base.OperandB.Evaluate(definition);
			}
			return result;
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00003851 File Offset: 0x00001A51
		public OrOperator()
		{
		}

		// Token: 0x0400026A RID: 618
		internal const string OpName = "or";
	}
}
