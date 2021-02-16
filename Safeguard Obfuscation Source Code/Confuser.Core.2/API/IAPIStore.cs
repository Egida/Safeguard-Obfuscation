using System;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	/// <summary>
	///     Provides storage for API interfaces
	/// </summary>
	// Token: 0x020000B0 RID: 176
	public interface IAPIStore
	{
		/// <summary>
		///     Adds the specified data store into this store.
		/// </summary>
		/// <param name="dataStore">The data store.</param>
		// Token: 0x060003E9 RID: 1001
		void AddStore(IDataStore dataStore);

		/// <summary>
		///     Finds a suitable data store for the specified method, with the
		///     specified number of keys.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The suitable data store if found, or <c>null</c> if not found.</returns>
		/// <remarks>
		///     It should never returns null --- ConfuserEx has internal data store.
		/// </remarks>
		// Token: 0x060003EA RID: 1002
		IDataStore GetStore(MethodDef method);

		/// <summary>
		///     Adds the specified opaque predicate into this store.
		/// </summary>
		/// <param name="predicate">The opaque predicate.</param>
		// Token: 0x060003EB RID: 1003
		void AddPredicate(IOpaquePredicateDescriptor predicate);

		/// <summary>
		///     Finds a suitable opaque predicate for the specified method, with the
		///     specified properties.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="type">The required type of predicate, or <c>null</c> if it does not matter.</param>
		/// <param name="argCount">The required numbers of arguments, or <c>null</c> if it does not matter.</param>
		/// <returns>The suitable opaque predicate if found, or <c>null</c> if not found.</returns>
		// Token: 0x060003EC RID: 1004
		IOpaquePredicateDescriptor GetPredicate(MethodDef method, OpaquePredicateType? type, params int[] argCount);
	}
}
