using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	/// <summary>
	///     The NOT operator.
	/// </summary>
	// Token: 0x02000095 RID: 149
	public class NotOperator : PatternOperator
	{
		/// <inheritdoc />
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000376 RID: 886 RVA: 0x0001562C File Offset: 0x0001382C
		public override string Name
		{
			get
			{
				return "not";
			}
		}

		/// <inheritdoc />
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000377 RID: 887 RVA: 0x0000DEB8 File Offset: 0x0000C0B8
		public override bool IsUnary
		{
			get
			{
				return true;
			}
		}

		/// <inheritdoc />
		// Token: 0x06000378 RID: 888 RVA: 0x00015644 File Offset: 0x00013844
		public override object Evaluate(IDnlibDef definition)
		{
			return !(bool)base.OperandA.Evaluate(definition);
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00003851 File Offset: 0x00001A51
		public NotOperator()
		{
		}

		// Token: 0x04000262 RID: 610
		internal const string OpName = "not";
	}
}
