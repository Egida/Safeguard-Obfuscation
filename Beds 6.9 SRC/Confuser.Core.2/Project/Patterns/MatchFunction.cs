using System;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	/// <summary>
	///     A function that match the full name of the definition with specified RegEx.
	/// </summary>
	// Token: 0x02000098 RID: 152
	public class MatchFunction : PatternFunction
	{
		/// <inheritdoc />
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000382 RID: 898 RVA: 0x000158A0 File Offset: 0x00013AA0
		public override string Name
		{
			get
			{
				return "match";
			}
		}

		/// <inheritdoc />
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000383 RID: 899 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		/// <inheritdoc />
		// Token: 0x06000384 RID: 900 RVA: 0x000158B8 File Offset: 0x00013AB8
		public override object Evaluate(IDnlibDef definition)
		{
			string regex = base.Arguments[0].Evaluate(definition).ToString();
			return Regex.IsMatch(definition.FullName, regex);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0000385A File Offset: 0x00001A5A
		public MatchFunction()
		{
		}

		// Token: 0x04000265 RID: 613
		internal const string FnName = "match";
	}
}
