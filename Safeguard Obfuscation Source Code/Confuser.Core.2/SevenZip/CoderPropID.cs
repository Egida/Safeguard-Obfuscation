using System;

namespace SevenZip
{
	/// <summary>
	///     Provides the fields that represent properties idenitifiers for compressing.
	/// </summary>
	// Token: 0x02000007 RID: 7
	internal enum CoderPropID
	{
		/// <summary>
		///     Specifies default property.
		/// </summary>
		// Token: 0x04000004 RID: 4
		DefaultProp,
		/// <summary>
		///     Specifies size of dictionary.
		/// </summary>
		// Token: 0x04000005 RID: 5
		DictionarySize,
		/// <summary>
		///     Specifies size of memory for PPM*.
		/// </summary>
		// Token: 0x04000006 RID: 6
		UsedMemorySize,
		/// <summary>
		///     Specifies order for PPM methods.
		/// </summary>
		// Token: 0x04000007 RID: 7
		Order,
		/// <summary>
		///     Specifies Block Size.
		/// </summary>
		// Token: 0x04000008 RID: 8
		BlockSize,
		/// <summary>
		///     Specifies number of postion state bits for LZMA (0 &lt;= x &lt;= 4).
		/// </summary>
		// Token: 0x04000009 RID: 9
		PosStateBits,
		/// <summary>
		///     Specifies number of literal context bits for LZMA (0 &lt;= x &lt;= 8).
		/// </summary>
		// Token: 0x0400000A RID: 10
		LitContextBits,
		/// <summary>
		///     Specifies number of literal position bits for LZMA (0 &lt;= x &lt;= 4).
		/// </summary>
		// Token: 0x0400000B RID: 11
		LitPosBits,
		/// <summary>
		///     Specifies number of fast bytes for LZ*.
		/// </summary>
		// Token: 0x0400000C RID: 12
		NumFastBytes,
		/// <summary>
		///     Specifies match finder. LZMA: "BT2", "BT4" or "BT4B".
		/// </summary>
		// Token: 0x0400000D RID: 13
		MatchFinder,
		/// <summary>
		///     Specifies the number of match finder cyckes.
		/// </summary>
		// Token: 0x0400000E RID: 14
		MatchFinderCycles,
		/// <summary>
		///     Specifies number of passes.
		/// </summary>
		// Token: 0x0400000F RID: 15
		NumPasses,
		/// <summary>
		///     Specifies number of algorithm.
		/// </summary>
		// Token: 0x04000010 RID: 16
		Algorithm,
		/// <summary>
		///     Specifies the number of threads.
		/// </summary>
		// Token: 0x04000011 RID: 17
		NumThreads,
		/// <summary>
		///     Specifies mode with end marker.
		/// </summary>
		// Token: 0x04000012 RID: 18
		EndMarker
	}
}
