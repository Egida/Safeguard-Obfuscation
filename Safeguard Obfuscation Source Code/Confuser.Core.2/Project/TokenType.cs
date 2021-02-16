using System;

namespace Confuser.Core.Project
{
	/// <summary>
	///     The type of pattern tokens
	/// </summary>
	// Token: 0x02000087 RID: 135
	public enum TokenType
	{
		/// <summary>
		///     An identifier, could be functions/operators.
		/// </summary>
		// Token: 0x04000244 RID: 580
		Identifier,
		/// <summary>
		///     A string literal.
		/// </summary>
		// Token: 0x04000245 RID: 581
		Literal,
		/// <summary>
		///     A left parenthesis.
		/// </summary>
		// Token: 0x04000246 RID: 582
		LParens,
		/// <summary>
		///     A right parenthesis.
		/// </summary>
		// Token: 0x04000247 RID: 583
		RParens,
		/// <summary>
		///     A comma.
		/// </summary>
		// Token: 0x04000248 RID: 584
		Comma
	}
}
