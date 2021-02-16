using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Core.Project.Patterns
{
	/// <summary>
	///     A pattern function.
	/// </summary>
	// Token: 0x020000A1 RID: 161
	public abstract class PatternFunction : PatternExpression
	{
		/// <summary>
		///     Gets the name of function.
		/// </summary>
		/// <value>The name.</value>
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060003A6 RID: 934
		public abstract string Name { get; }

		/// <summary>
		///     Gets the number of arguments of the function.
		/// </summary>
		/// <value>The number of arguments.</value>
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060003A7 RID: 935
		public abstract int ArgumentCount { get; }

		/// <summary>
		///     Gets or sets the arguments of function.
		/// </summary>
		/// <value>The arguments.</value>
		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00003886 File Offset: 0x00001A86
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x0000388E File Offset: 0x00001A8E
		public IList<PatternExpression> Arguments
		{
			[CompilerGenerated]
			get
			{
				return this.<Arguments>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Arguments>k__BackingField = value;
			}
		}

		/// <inheritdoc />
		// Token: 0x060003AA RID: 938 RVA: 0x00015CDC File Offset: 0x00013EDC
		public override void Serialize(IList<PatternToken> tokens)
		{
			tokens.Add(new PatternToken(TokenType.Identifier, this.Name));
			tokens.Add(new PatternToken(TokenType.LParens));
			for (int i = 0; i < this.Arguments.Count; i++)
			{
				bool flag = i != 0;
				if (flag)
				{
					tokens.Add(new PatternToken(TokenType.Comma));
				}
				this.Arguments[i].Serialize(tokens);
			}
			tokens.Add(new PatternToken(TokenType.RParens));
		}

		// Token: 0x060003AB RID: 939 RVA: 0x00003897 File Offset: 0x00001A97
		protected PatternFunction()
		{
		}

		// Token: 0x0400026D RID: 621
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<PatternExpression> <Arguments>k__BackingField;
	}
}
