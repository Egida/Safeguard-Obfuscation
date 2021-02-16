using System;
using dnlib.DotNet.Emit;

namespace Confuser.Core.API
{
	/// <summary>
	///     An accessor of data store.
	/// </summary>
	// Token: 0x020000B2 RID: 178
	public interface IDataStoreAccessor
	{
		/// <summary>
		///     Emits the runtime instruction sequence for this accessor.
		/// </summary>
		/// <returns>An instruction sequence that returns the stored data.</returns>
		// Token: 0x060003F1 RID: 1009
		Instruction[] Emit();
	}
}
