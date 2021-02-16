using System;

namespace Confuser.Core.Helpers
{
	/// <summary>
	///     The type of block in the key sequence
	/// </summary>
	// Token: 0x020000A4 RID: 164
	public enum BlockKeyType
	{
		/// <summary>
		///     The state key should be explicitly set in the block
		/// </summary>
		// Token: 0x04000274 RID: 628
		Explicit,
		/// <summary>
		///     The state key could be assumed to be same as <see cref="F:Confuser.Core.Helpers.BlockKey.EntryState" /> at the beginning of block.
		/// </summary>
		// Token: 0x04000275 RID: 629
		Incremental
	}
}
