using System;
using System.Text;

namespace Confuser.Core.Project
{
	// Token: 0x02000089 RID: 137
	internal class PatternTokenizer
	{
		// Token: 0x06000330 RID: 816 RVA: 0x00003715 File Offset: 0x00001915
		public void Initialize(string pattern)
		{
			this.rulePattern = pattern;
			this.index = 0;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00014394 File Offset: 0x00012594
		private char NextChar()
		{
			bool flag = this.index >= this.rulePattern.Length;
			if (flag)
			{
				throw new InvalidPatternException("Unexpected end of pattern.");
			}
			string text = this.rulePattern;
			int num = this.index;
			this.index = num + 1;
			return text[num];
		}

		// Token: 0x06000332 RID: 818 RVA: 0x000143E8 File Offset: 0x000125E8
		public PatternToken? NextToken()
		{
			bool flag = this.rulePattern == null;
			if (flag)
			{
				throw new InvalidOperationException("Tokenizer not initialized.");
			}
			this.SkipWhitespace();
			char? tokenBegin = this.PeekChar();
			char? c = tokenBegin;
			int? num = (c != null) ? new int?((int)c.GetValueOrDefault()) : null;
			bool flag2 = num == null;
			PatternToken? result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				int pos = this.index;
				char value = tokenBegin.Value;
				bool flag3 = value != '"';
				if (flag3)
				{
					switch (value)
					{
					case '\'':
						goto IL_163;
					case '(':
						this.index++;
						return new PatternToken?(new PatternToken(pos, TokenType.LParens));
					case ')':
						this.index++;
						return new PatternToken?(new PatternToken(pos, TokenType.RParens));
					case ',':
						this.index++;
						return new PatternToken?(new PatternToken(pos, TokenType.Comma));
					}
					bool flag4 = !char.IsLetter(tokenBegin.Value);
					if (flag4)
					{
						throw new InvalidPatternException(string.Format("Unknown token '{0}' at position {1}.", tokenBegin, pos));
					}
					return new PatternToken?(new PatternToken(pos, TokenType.Identifier, this.ReadIdentifier()));
				}
				IL_163:
				result = new PatternToken?(new PatternToken(pos, TokenType.Literal, this.ReadLiteral()));
			}
			return result;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00014574 File Offset: 0x00012774
		private char? PeekChar()
		{
			bool flag = this.index >= this.rulePattern.Length;
			char? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new char?(this.rulePattern[this.index]);
			}
			return result;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x000145C4 File Offset: 0x000127C4
		private string ReadIdentifier()
		{
			StringBuilder ret = new StringBuilder();
			char? chr = this.PeekChar();
			for (;;)
			{
				char? c = chr;
				int? num = (c != null) ? new int?((int)c.GetValueOrDefault()) : null;
				bool flag;
				if (num != null)
				{
					if (char.IsLetterOrDigit(chr.Value))
					{
						goto IL_DB;
					}
					char? c2 = chr;
					if (((c2 != null) ? new int?((int)c2.GetValueOrDefault()) : null) == 95)
					{
						goto IL_DB;
					}
					c2 = chr;
					flag = !(((c2 != null) ? new int?((int)c2.GetValueOrDefault()) : null) == 45);
					goto IL_DF;
					IL_DB:
					flag = false;
				}
				else
				{
					flag = true;
				}
				IL_DF:
				bool flag2 = flag;
				if (flag2)
				{
					break;
				}
				ret.Append(this.NextChar());
				chr = this.PeekChar();
			}
			return ret.ToString();
		}

		// Token: 0x06000335 RID: 821 RVA: 0x000146E0 File Offset: 0x000128E0
		private string ReadLiteral()
		{
			StringBuilder ret = new StringBuilder();
			char delim = this.NextChar();
			for (char chr = this.NextChar(); chr != delim; chr = this.NextChar())
			{
				bool flag = chr == '\\';
				if (flag)
				{
					ret.Append(this.NextChar());
				}
				else
				{
					ret.Append(chr);
				}
			}
			return ret.ToString();
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0001474C File Offset: 0x0001294C
		private void SkipWhitespace()
		{
			while (this.index < this.rulePattern.Length && char.IsWhiteSpace(this.rulePattern[this.index]))
			{
				this.index++;
			}
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00002194 File Offset: 0x00000394
		public PatternTokenizer()
		{
		}

		// Token: 0x0400024C RID: 588
		private int index;

		// Token: 0x0400024D RID: 589
		private string rulePattern;
	}
}
