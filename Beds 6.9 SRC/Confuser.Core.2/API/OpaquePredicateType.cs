using System;

namespace Confuser.Core.API
{
	/// <summary>
	///     The type of opaque predicate.
	/// </summary>
	// Token: 0x020000B5 RID: 181
	public enum OpaquePredicateType
	{
		/// <summary>
		///     A function, in a mathematics sense, with one input and one output.
		/// </summary>
		// Token: 0x04000296 RID: 662
		Function,
		/// <summary>
		///     A constant function, always returning the same value.
		/// </summary>
		// Token: 0x04000297 RID: 663
		Invariant
	}
}
