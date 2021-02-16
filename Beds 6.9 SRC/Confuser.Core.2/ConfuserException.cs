using System;

namespace Confuser.Core
{
	/// <summary>
	///     The exception that is thrown when a handled error occurred during the protection process.
	/// </summary>
	// Token: 0x02000038 RID: 56
	public class ConfuserException : Exception
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ConfuserException" /> class.
		/// </summary>
		/// <param name="innerException">The inner exception, or null if no exception is associated with the error.</param>
		// Token: 0x06000134 RID: 308 RVA: 0x0000281D File Offset: 0x00000A1D
		public ConfuserException(Exception innerException) : base("Exception occurred during the protection process.", innerException)
		{
		}
	}
}
