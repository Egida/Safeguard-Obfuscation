using System;

namespace Confuser.Core.Helpers
{
	/// <summary>
	///     The type of Control Flow Block
	/// </summary>
	// Token: 0x020000A7 RID: 167
	[Flags]
	public enum ControlFlowBlockType
	{
		/// <summary>
		///     The block is a normal block
		/// </summary>
		// Token: 0x0400027F RID: 639
		Normal = 0,
		/// <summary>
		///     There are unknown edges to this block. Usually used at exception handlers / method entry.
		/// </summary>
		// Token: 0x04000280 RID: 640
		Entry = 1,
		/// <summary>
		///     There are unknown edges from this block. Usually used at filter blocks / throw / method exit.
		/// </summary>
		// Token: 0x04000281 RID: 641
		Exit = 2
	}
}
