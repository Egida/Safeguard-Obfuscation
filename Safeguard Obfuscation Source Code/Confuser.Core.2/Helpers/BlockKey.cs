using System;

namespace Confuser.Core.Helpers
{
	/// <summary>
	///     The information of the block in the key sequence
	/// </summary>
	// Token: 0x020000A3 RID: 163
	public struct BlockKey
	{
		/// <summary>
		///     The state key at the beginning of the block
		/// </summary>
		// Token: 0x04000270 RID: 624
		public uint EntryState;

		/// <summary>
		///     The state key at the end of the block
		/// </summary>
		// Token: 0x04000271 RID: 625
		public uint ExitState;

		/// <summary>
		///     The type of block
		/// </summary>
		// Token: 0x04000272 RID: 626
		public BlockKeyType Type;
	}
}
