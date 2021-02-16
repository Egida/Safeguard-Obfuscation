using System;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	/// <summary>
	///     The descriptor of a type of opaque predicate.
	/// </summary>
	// Token: 0x020000B3 RID: 179
	public interface IOpaquePredicateDescriptor
	{
		/// <summary>
		///     Gets the type of the opaque predicate.
		/// </summary>
		/// <value>The type of the opaque predicate.</value>
		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060003F2 RID: 1010
		OpaquePredicateType Type { get; }

		/// <summary>
		///     Gets the number of arguments this predicate has.
		/// </summary>
		/// <remarks>
		///     When <see cref="P:Confuser.Core.API.IOpaquePredicateDescriptor.Type" /> is <see cref="F:Confuser.Core.API.OpaquePredicateType.Invariant" />,
		///     there can be 0 or more arguments.
		///     When <see cref="P:Confuser.Core.API.IOpaquePredicateDescriptor.Type" /> is <see cref="F:Confuser.Core.API.OpaquePredicateType.Function" />,
		///     there must be more than 0 arguments.
		/// </remarks>
		/// <value>The number of arguments this predicate has.</value>
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060003F3 RID: 1011
		int ArgumentCount { get; }

		/// <summary>
		///     Determines whether this predicate can be used with the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <value><c>true</c> if this predicate can be used with the specified method; otherwise, <c>false</c>.</value>
		// Token: 0x060003F4 RID: 1012
		bool IsUsable(MethodDef method);

		/// <summary>
		///     Creates a new opaque predicate for the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>A newly create opaque predicate.</returns>
		// Token: 0x060003F5 RID: 1013
		IOpaquePredicate CreatePredicate(MethodDef method);
	}
}
