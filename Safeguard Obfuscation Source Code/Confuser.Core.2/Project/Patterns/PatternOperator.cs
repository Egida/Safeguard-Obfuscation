using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Core.Project.Patterns
{
	/// <summary>
	///     A pattern operator.
	/// </summary>
	// Token: 0x020000A2 RID: 162
	public abstract class PatternOperator : PatternExpression
	{
		/// <summary>
		///     Gets the name of operator.
		/// </summary>
		/// <value>The name.</value>
		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060003AC RID: 940
		public abstract string Name { get; }

		/// <summary>
		///     Gets a value indicating whether this is an unary operator.
		/// </summary>
		/// <value><c>true</c> if this is an unary operator; otherwise, <c>false</c>.</value>
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060003AD RID: 941
		public abstract bool IsUnary { get; }

		/// <summary>
		///     Gets or sets the first operand.
		/// </summary>
		/// <value>The first operand.</value>
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060003AE RID: 942 RVA: 0x000038A0 File Offset: 0x00001AA0
		// (set) Token: 0x060003AF RID: 943 RVA: 0x000038A8 File Offset: 0x00001AA8
		public PatternExpression OperandA
		{
			[CompilerGenerated]
			get
			{
				return this.<OperandA>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<OperandA>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets or sets the second operand.
		/// </summary>
		/// <value>The second operand.</value>
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060003B0 RID: 944 RVA: 0x000038B1 File Offset: 0x00001AB1
		// (set) Token: 0x060003B1 RID: 945 RVA: 0x000038B9 File Offset: 0x00001AB9
		public PatternExpression OperandB
		{
			[CompilerGenerated]
			get
			{
				return this.<OperandB>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<OperandB>k__BackingField = value;
			}
		}

		/// <inheritdoc />
		// Token: 0x060003B2 RID: 946 RVA: 0x00015D5C File Offset: 0x00013F5C
		public override void Serialize(IList<PatternToken> tokens)
		{
			bool isUnary = this.IsUnary;
			if (isUnary)
			{
				tokens.Add(new PatternToken(TokenType.Identifier, this.Name));
				this.OperandA.Serialize(tokens);
			}
			else
			{
				this.OperandA.Serialize(tokens);
				tokens.Add(new PatternToken(TokenType.Identifier, this.Name));
				this.OperandB.Serialize(tokens);
			}
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00003897 File Offset: 0x00001A97
		protected PatternOperator()
		{
		}

		// Token: 0x0400026E RID: 622
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PatternExpression <OperandA>k__BackingField;

		// Token: 0x0400026F RID: 623
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PatternExpression <OperandB>k__BackingField;
	}
}
