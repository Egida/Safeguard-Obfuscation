using System;
using dnlib.DotNet.Emit;

namespace Confuser.Core.API
{
	/// <summary>
	///     An instance of opaque predicate.
	/// </summary>
	// Token: 0x020000B4 RID: 180
	public interface IOpaquePredicate
	{
		/// <summary>
		///     Emits the runtime instruction sequence for this predicate.
		/// </summary>
		/// <param name="loadArg">
		///     A function that returns an instruction sequence that returns the input value,
		///     or <c>null</c> if <see cref="P:Confuser.Core.API.IOpaquePredicateDescriptor.ArgumentCount" /> is 0.
		/// </param>
		/// <returns>An instruction sequence that returns the value of this predicate.</returns>
		// Token: 0x060003F6 RID: 1014
		Instruction[] Emit(Func<Instruction[]> loadArg);

		/// <summary>
		///     Computes the value of this predicate with the specified argument.
		/// </summary>
		/// <param name="arg">The argument to this predicate.</param>
		/// <returns>The return value of this predicate.</returns>
		// Token: 0x060003F7 RID: 1015
		uint GetValue(uint[] arg);
	}
}
