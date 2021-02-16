using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	/// <summary>
	///     Provides methods to obtain runtime library injection type.
	/// </summary>
	// Token: 0x0200007A RID: 122
	public interface IRuntimeService
	{
		/// <summary>
		///     Gets the specified runtime type for injection.
		/// </summary>
		/// <param name="fullName">The full name of the runtime type.</param>
		/// <returns>The requested runtime type.</returns>
		// Token: 0x060002CB RID: 715
		TypeDef GetRuntimeType(string fullName);
	}
}
