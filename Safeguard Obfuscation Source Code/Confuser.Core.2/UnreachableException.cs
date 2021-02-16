using System;

namespace Confuser.Core
{
	/// <summary>
	///     The exception that is thrown when supposedly unreachable code is executed.
	/// </summary>
	// Token: 0x02000070 RID: 112
	public class UnreachableException : SystemException
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.UnreachableException" /> class.
		/// </summary>
		// Token: 0x060002A1 RID: 673 RVA: 0x000032A7 File Offset: 0x000014A7
		public UnreachableException() : base("Unreachable code reached.")
		{
		}
	}
}
