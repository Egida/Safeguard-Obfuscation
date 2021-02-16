using System;

namespace Confuser.Core
{
	/// <summary>
	///     Various presets of protections.
	/// </summary>
	// Token: 0x0200006E RID: 110
	public enum ProtectionPreset
	{
		/// <summary> The protection does not belong to any preset. </summary>
		// Token: 0x04000208 RID: 520
		None,
		/// <summary> The protection provides basic security. </summary>
		// Token: 0x04000209 RID: 521
		Minimum,
		/// <summary> The protection provides normal security for public release. </summary>
		// Token: 0x0400020A RID: 522
		Normal,
		/// <summary> The protection provides better security with observable performance impact. </summary>
		// Token: 0x0400020B RID: 523
		Aggressive,
		/// <summary> The protection provides strongest security with possible incompatibility. </summary>
		// Token: 0x0400020C RID: 524
		Maximum
	}
}
