using System;

namespace Confuser.Core.Project
{
	// Token: 0x02000084 RID: 132
	public class InvalidPatternException : Exception
	{
		// Token: 0x0600030A RID: 778 RVA: 0x00003544 File Offset: 0x00001744
		public InvalidPatternException(string message) : base(message)
		{
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0000354F File Offset: 0x0000174F
		public InvalidPatternException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
