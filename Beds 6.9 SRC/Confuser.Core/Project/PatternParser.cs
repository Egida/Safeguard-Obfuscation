using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Confuser.Core.Project.Patterns;

namespace Confuser.Core.Project
{
	// Token: 0x02000085 RID: 133
	public class PatternParser
	{
		// Token: 0x0600030C RID: 780 RVA: 0x00013C34 File Offset: 0x00011E34
		static PatternParser()
		{
			PatternParser.fns.Add("module", () => new ModuleFunction());
			PatternParser.fns.Add("decl-type", () => new DeclTypeFunction());
			PatternParser.fns.Add("namespace", () => new NamespaceFunction());
			PatternParser.fns.Add("name", () => new NameFunction());
			PatternParser.fns.Add("full-name", () => new FullNameFunction());
			PatternParser.fns.Add("match", () => new MatchFunction());
			PatternParser.fns.Add("match-name", () => new MatchNameFunction());
			PatternParser.fns.Add("match-type-name", () => new MatchTypeNameFunction());
			PatternParser.fns.Add("member-type", () => new MemberTypeFunction());
			PatternParser.fns.Add("is-public", () => new IsPublicFunction());
			PatternParser.fns.Add("inherits", () => new InheritsFunction());
			PatternParser.fns.Add("is-type", () => new IsTypeFunction());
			PatternParser.ops = new Dictionary<string, Func<PatternOperator>>(StringComparer.OrdinalIgnoreCase);
			PatternParser.ops.Add("and", () => new AndOperator());
			PatternParser.ops.Add("or", () => new OrOperator());
			PatternParser.ops.Add("not", () => new NotOperator());
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000355B File Offset: 0x0000175B
		private Exception BadArgCount(PatternToken token, int expected)
		{
			throw new InvalidPatternException(string.Format("Invalid argument count for '{0}' at position {1}. Expected {2}", token.Value, token.Position, expected));
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00013E40 File Offset: 0x00012040
		private static bool IsFunction(PatternToken token)
		{
			return token.Type == TokenType.Identifier && PatternParser.fns.ContainsKey(token.Value);
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00013E70 File Offset: 0x00012070
		private static bool IsOperator(PatternToken token)
		{
			return token.Type == TokenType.Identifier && PatternParser.ops.ContainsKey(token.Value);
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00003584 File Offset: 0x00001784
		private Exception MismatchParens(int position)
		{
			throw new InvalidPatternException(string.Format("Mismatched parentheses at position {0}.", position));
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00013EA0 File Offset: 0x000120A0
		public PatternExpression Parse(string pattern)
		{
			bool flag = pattern == null;
			if (flag)
			{
				throw new ArgumentNullException("pattern");
			}
			PatternExpression result;
			try
			{
				this.tokenizer.Initialize(pattern);
				this.lookAhead = this.tokenizer.NextToken();
				PatternExpression ret = this.ParseExpression(true);
				bool flag2 = this.PeekToken() != null;
				if (flag2)
				{
					throw new InvalidPatternException("Extra tokens beyond the end of pattern.");
				}
				result = ret;
			}
			catch (Exception ex)
			{
				bool flag3 = ex is InvalidPatternException;
				if (flag3)
				{
					throw;
				}
				throw new InvalidPatternException("Invalid pattern.", ex);
			}
			return result;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00013F48 File Offset: 0x00012148
		private PatternExpression ParseExpression(bool readBinOp = false)
		{
			PatternToken token = this.ReadToken();
			PatternExpression ret;
			switch (token.Type)
			{
			case TokenType.Identifier:
			{
				bool flag = PatternParser.IsOperator(token);
				if (flag)
				{
					PatternOperator op = PatternParser.ops[token.Value]();
					bool flag2 = !op.IsUnary;
					if (flag2)
					{
						throw this.UnexpectedToken(token);
					}
					op.OperandA = this.ParseExpression(false);
					ret = op;
				}
				else
				{
					bool flag3 = PatternParser.IsFunction(token);
					if (flag3)
					{
						PatternFunction fn = PatternParser.fns[token.Value]();
						PatternToken parens = this.ReadToken();
						bool flag4 = parens.Type != TokenType.LParens;
						if (flag4)
						{
							throw this.UnexpectedToken(parens, '(');
						}
						fn.Arguments = new List<PatternExpression>(fn.ArgumentCount);
						for (int i = 0; i < fn.ArgumentCount; i++)
						{
							bool flag5 = this.PeekToken() == null;
							if (flag5)
							{
								throw this.UnexpectedEnd();
							}
							bool flag6 = this.PeekToken().Value.Type == TokenType.RParens;
							if (flag6)
							{
								throw this.BadArgCount(token, fn.ArgumentCount);
							}
							bool flag7 = i != 0;
							if (flag7)
							{
								PatternToken comma = this.ReadToken();
								bool flag8 = comma.Type != TokenType.Comma;
								if (flag8)
								{
									throw this.UnexpectedToken(comma, ',');
								}
							}
							fn.Arguments.Add(this.ParseExpression(false));
						}
						parens = this.ReadToken();
						bool flag9 = parens.Type == TokenType.Comma;
						if (flag9)
						{
							throw this.BadArgCount(token, fn.ArgumentCount);
						}
						bool flag10 = parens.Type != TokenType.RParens;
						if (flag10)
						{
							throw this.MismatchParens(parens.Position.Value);
						}
						ret = fn;
					}
					else
					{
						bool boolValue;
						bool flag11 = !bool.TryParse(token.Value, out boolValue);
						if (flag11)
						{
							throw this.UnknownToken(token);
						}
						ret = new LiteralExpression(boolValue);
					}
				}
				break;
			}
			case TokenType.Literal:
				ret = new LiteralExpression(token.Value);
				break;
			case TokenType.LParens:
			{
				ret = this.ParseExpression(true);
				bool flag12 = this.ReadToken().Type != TokenType.RParens;
				if (flag12)
				{
					throw this.MismatchParens(token.Position.Value);
				}
				break;
			}
			default:
				throw this.UnexpectedToken(token);
			}
			bool flag13 = !readBinOp;
			PatternExpression result;
			if (flag13)
			{
				result = ret;
			}
			else
			{
				PatternToken? peek = this.PeekToken();
				while (peek != null && peek.Value.Type == TokenType.Identifier && PatternParser.IsOperator(peek.Value))
				{
					PatternToken binOpToken = this.ReadToken();
					PatternOperator binOp = PatternParser.ops[binOpToken.Value]();
					bool isUnary = binOp.IsUnary;
					if (isUnary)
					{
						throw this.UnexpectedToken(binOpToken);
					}
					binOp.OperandA = ret;
					binOp.OperandB = this.ParseExpression(false);
					ret = binOp;
					peek = this.PeekToken();
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00014270 File Offset: 0x00012470
		private PatternToken? PeekToken()
		{
			return this.lookAhead;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00014288 File Offset: 0x00012488
		private PatternToken ReadToken()
		{
			bool flag = this.lookAhead == null;
			if (flag)
			{
				throw this.UnexpectedEnd();
			}
			PatternToken ret = this.lookAhead.Value;
			this.lookAhead = this.tokenizer.NextToken();
			return ret;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0000359C File Offset: 0x0000179C
		private Exception UnexpectedEnd()
		{
			throw new InvalidPatternException("Unexpected end of pattern.");
		}

		// Token: 0x06000316 RID: 790 RVA: 0x000035A9 File Offset: 0x000017A9
		private Exception UnexpectedToken(PatternToken token)
		{
			throw new InvalidPatternException(string.Format("Unexpected token '{0}' at position {1}.", token.Value, token.Position));
		}

		// Token: 0x06000317 RID: 791 RVA: 0x000035CC File Offset: 0x000017CC
		private Exception UnexpectedToken(PatternToken token, char expect)
		{
			throw new InvalidPatternException(string.Format("Unexpected token '{0}' at position {1}. Expected '{2}'.", token.Value, token.Position, expect));
		}

		// Token: 0x06000318 RID: 792 RVA: 0x000035F5 File Offset: 0x000017F5
		private Exception UnknownToken(PatternToken token)
		{
			throw new InvalidPatternException(string.Format("Unknown token '{0}' at position {1}.", token.Value, token.Position));
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00003618 File Offset: 0x00001818
		public PatternParser()
		{
		}

		// Token: 0x0400023E RID: 574
		private static readonly Dictionary<string, Func<PatternFunction>> fns = new Dictionary<string, Func<PatternFunction>>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x0400023F RID: 575
		private PatternToken? lookAhead;

		// Token: 0x04000240 RID: 576
		private static readonly Dictionary<string, Func<PatternOperator>> ops;

		// Token: 0x04000241 RID: 577
		private readonly PatternTokenizer tokenizer = new PatternTokenizer();

		// Token: 0x02000086 RID: 134
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600031A RID: 794 RVA: 0x0000362C File Offset: 0x0000182C
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600031B RID: 795 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x0600031C RID: 796 RVA: 0x00003638 File Offset: 0x00001838
			internal PatternFunction <.cctor>b__0_0()
			{
				return new ModuleFunction();
			}

			// Token: 0x0600031D RID: 797 RVA: 0x0000363F File Offset: 0x0000183F
			internal PatternFunction <.cctor>b__0_1()
			{
				return new DeclTypeFunction();
			}

			// Token: 0x0600031E RID: 798 RVA: 0x00003646 File Offset: 0x00001846
			internal PatternFunction <.cctor>b__0_2()
			{
				return new NamespaceFunction();
			}

			// Token: 0x0600031F RID: 799 RVA: 0x0000364D File Offset: 0x0000184D
			internal PatternFunction <.cctor>b__0_3()
			{
				return new NameFunction();
			}

			// Token: 0x06000320 RID: 800 RVA: 0x00003654 File Offset: 0x00001854
			internal PatternFunction <.cctor>b__0_4()
			{
				return new FullNameFunction();
			}

			// Token: 0x06000321 RID: 801 RVA: 0x0000365B File Offset: 0x0000185B
			internal PatternFunction <.cctor>b__0_5()
			{
				return new MatchFunction();
			}

			// Token: 0x06000322 RID: 802 RVA: 0x00003662 File Offset: 0x00001862
			internal PatternFunction <.cctor>b__0_6()
			{
				return new MatchNameFunction();
			}

			// Token: 0x06000323 RID: 803 RVA: 0x00003669 File Offset: 0x00001869
			internal PatternFunction <.cctor>b__0_7()
			{
				return new MatchTypeNameFunction();
			}

			// Token: 0x06000324 RID: 804 RVA: 0x00003670 File Offset: 0x00001870
			internal PatternFunction <.cctor>b__0_8()
			{
				return new MemberTypeFunction();
			}

			// Token: 0x06000325 RID: 805 RVA: 0x00003677 File Offset: 0x00001877
			internal PatternFunction <.cctor>b__0_9()
			{
				return new IsPublicFunction();
			}

			// Token: 0x06000326 RID: 806 RVA: 0x0000367E File Offset: 0x0000187E
			internal PatternFunction <.cctor>b__0_10()
			{
				return new InheritsFunction();
			}

			// Token: 0x06000327 RID: 807 RVA: 0x00003685 File Offset: 0x00001885
			internal PatternFunction <.cctor>b__0_11()
			{
				return new IsTypeFunction();
			}

			// Token: 0x06000328 RID: 808 RVA: 0x0000368C File Offset: 0x0000188C
			internal PatternOperator <.cctor>b__0_12()
			{
				return new AndOperator();
			}

			// Token: 0x06000329 RID: 809 RVA: 0x00003693 File Offset: 0x00001893
			internal PatternOperator <.cctor>b__0_13()
			{
				return new OrOperator();
			}

			// Token: 0x0600032A RID: 810 RVA: 0x0000369A File Offset: 0x0000189A
			internal PatternOperator <.cctor>b__0_14()
			{
				return new NotOperator();
			}

			// Token: 0x04000242 RID: 578
			public static readonly PatternParser.<>c <>9 = new PatternParser.<>c();
		}
	}
}
