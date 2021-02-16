using System;

namespace SevenZip
{
	/// <summary>
	///     The exception that is thrown when an error in input stream occurs during decoding.
	/// </summary>
	// Token: 0x02000003 RID: 3
	internal class DataErrorException : ApplicationException
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002113 File Offset: 0x00000313
		public DataErrorException() : base("Data Error")
		{
		}
	}
}
