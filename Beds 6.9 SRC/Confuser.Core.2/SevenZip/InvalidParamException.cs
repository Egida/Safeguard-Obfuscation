using System;

namespace SevenZip
{
	/// <summary>
	///     The exception that is thrown when the value of an argument is outside the allowable range.
	/// </summary>
	// Token: 0x02000004 RID: 4
	internal class InvalidParamException : ApplicationException
	{
		// Token: 0x0600000A RID: 10 RVA: 0x00002122 File Offset: 0x00000322
		public InvalidParamException() : base("Invalid Parameter")
		{
		}
	}
}
