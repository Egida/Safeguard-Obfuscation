using System;

namespace Confuser.Core.Project
{
	/// <summary>
	///     The exception that is thrown when attempted to parse an invalid pattern.
	/// </summary>
	// Token: 0x02000084 RID: 132
	public class InvalidPatternException : Exception
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ConfuserException" /> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		// Token: 0x0600030A RID: 778 RVA: 0x00003544 File Offset: 0x00001744
		public InvalidPatternException(string message) : base(message)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ConfuserException" /> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">
		///     The exception that is the cause of the current exception, or a null reference (Nothing in
		///     Visual Basic) if no inner exception is specified.
		/// </param>
		// Token: 0x0600030B RID: 779 RVA: 0x0000354F File Offset: 0x0000174F
		public InvalidPatternException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
