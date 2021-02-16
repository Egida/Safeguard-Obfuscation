using System;

namespace Confuser.Core
{
	// Token: 0x02000038 RID: 56
	public class ConfuserException : Exception
	{
		// Token: 0x06000134 RID: 308 RVA: 0x0000281D File Offset: 0x00000A1D
		public ConfuserException(Exception innerException) : base("Exception occurred during the protection process.", innerException)
		{
		}
	}
}
