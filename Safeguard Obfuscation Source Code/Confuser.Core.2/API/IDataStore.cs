using System;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	/// <summary>
	///     A data store.
	/// </summary>
	// Token: 0x020000B1 RID: 177
	public interface IDataStore
	{
		/// <summary>
		///     Gets the priority of this data store; higher priority means it
		///     would be tried earlier.
		/// </summary>
		/// <value>The priority of this data store.</value>
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060003ED RID: 1005
		int Priority { get; }

		/// <summary>
		///     Gets the number of keys this predicate has.
		/// </summary>
		/// <remarks>
		///     Keys are used by the data store to encrypt data/whatever purpose.
		/// </remarks>
		/// <value>The number of keys this data store has.</value>
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060003EE RID: 1006
		int KeyCount { get; }

		/// <summary>
		///     Determines whether this data store can be used in the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <value><c>true</c> if this data store can be used in the specified method; otherwise, <c>false</c>.</value>
		// Token: 0x060003EF RID: 1007
		bool IsUsable(MethodDef method);

		/// <summary>
		///     Creates an accessor of this data store for the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="data">The data to store.</param>
		/// <returns>A newly accessor of this data store.</returns>
		// Token: 0x060003F0 RID: 1008
		IDataStoreAccessor CreateAccessor(MethodDef method, uint[] keys, byte[] data);
	}
}
