using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	/// <summary>
	///     Provides methods to trace stack of method body.
	/// </summary>
	// Token: 0x02000080 RID: 128
	public interface ITraceService
	{
		/// <summary>
		///     Trace the stack of the specified method.
		/// </summary>
		/// <param name="method">The method to trace.</param>
		/// <exception cref="T:dnlib.DotNet.Emit.InvalidMethodException"><paramref name="method" /> has invalid body.</exception>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="method" /> is <c>null</c>.</exception>
		// Token: 0x060002E5 RID: 741
		MethodTrace Trace(MethodDef method);
	}
}
