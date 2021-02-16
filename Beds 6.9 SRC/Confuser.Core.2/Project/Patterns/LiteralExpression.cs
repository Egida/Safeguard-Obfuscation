using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	/// <summary>
	///     A literal expression.
	/// </summary>
	// Token: 0x0200009E RID: 158
	public class LiteralExpression : PatternExpression
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.Project.Patterns.LiteralExpression" /> class.
		/// </summary>
		/// <param name="literal">The literal.</param>
		// Token: 0x0600039A RID: 922 RVA: 0x00003863 File Offset: 0x00001A63
		public LiteralExpression(object literal)
		{
			this.Literal = literal;
		}

		/// <summary>
		///     Gets the value of literal.
		/// </summary>
		/// <value>The value of literal.</value>
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600039B RID: 923 RVA: 0x00003875 File Offset: 0x00001A75
		// (set) Token: 0x0600039C RID: 924 RVA: 0x0000387D File Offset: 0x00001A7D
		public object Literal
		{
			[CompilerGenerated]
			get
			{
				return this.<Literal>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Literal>k__BackingField = value;
			}
		}

		/// <inheritdoc />
		// Token: 0x0600039D RID: 925 RVA: 0x00015C0C File Offset: 0x00013E0C
		public override object Evaluate(IDnlibDef definition)
		{
			return this.Literal;
		}

		/// <inheritdoc />
		// Token: 0x0600039E RID: 926 RVA: 0x00015C24 File Offset: 0x00013E24
		public override void Serialize(IList<PatternToken> tokens)
		{
			bool flag = this.Literal is bool;
			if (flag)
			{
				tokens.Add(new PatternToken(TokenType.Identifier, ((bool)this.Literal).ToString().ToLowerInvariant()));
			}
			else
			{
				tokens.Add(new PatternToken(TokenType.Literal, this.Literal.ToString()));
			}
		}

		// Token: 0x0400026B RID: 619
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private object <Literal>k__BackingField;
	}
}
