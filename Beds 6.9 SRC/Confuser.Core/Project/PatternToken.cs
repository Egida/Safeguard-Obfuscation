using System;

namespace Confuser.Core.Project
{
	// Token: 0x02000088 RID: 136
	public struct PatternToken
	{
		// Token: 0x0600032B RID: 811 RVA: 0x000036A1 File Offset: 0x000018A1
		public PatternToken(int pos, TokenType type)
		{
			this.Position = new int?(pos);
			this.Type = type;
			this.Value = null;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x000036BE File Offset: 0x000018BE
		public PatternToken(int pos, TokenType type, string value)
		{
			this.Position = new int?(pos);
			this.Type = type;
			this.Value = value;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x000036DB File Offset: 0x000018DB
		public PatternToken(TokenType type)
		{
			this.Position = null;
			this.Type = type;
			this.Value = null;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x000036F8 File Offset: 0x000018F8
		public PatternToken(TokenType type, string value)
		{
			this.Position = null;
			this.Type = type;
			this.Value = value;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x000142D4 File Offset: 0x000124D4
		public override string ToString()
		{
			bool flag = this.Position != null;
			string result;
			if (flag)
			{
				bool flag2 = this.Value != null;
				if (flag2)
				{
					result = string.Format("[{0}] {1} @ {2}", this.Type, this.Value, this.Position);
				}
				else
				{
					result = string.Format("[{0}] @ {1}", this.Type, this.Position);
				}
			}
			else
			{
				bool flag3 = this.Value != null;
				if (flag3)
				{
					result = string.Format("[{0}] {1}", this.Type, this.Value);
				}
				else
				{
					result = string.Format("[{0}]", this.Type);
				}
			}
			return result;
		}

		// Token: 0x04000249 RID: 585
		public readonly int? Position;

		// Token: 0x0400024A RID: 586
		public readonly TokenType Type;

		// Token: 0x0400024B RID: 587
		public readonly string Value;
	}
}
