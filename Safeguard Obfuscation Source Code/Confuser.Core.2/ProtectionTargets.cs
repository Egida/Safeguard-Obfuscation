using System;

namespace Confuser.Core
{
	/// <summary>
	///     Targets of protection.
	/// </summary>
	// Token: 0x02000067 RID: 103
	[Flags]
	public enum ProtectionTargets
	{
		/// <summary> Type definitions. </summary>
		// Token: 0x040001CD RID: 461
		Types = 1,
		/// <summary> Method definitions. </summary>
		// Token: 0x040001CE RID: 462
		Methods = 2,
		/// <summary> Field definitions. </summary>
		// Token: 0x040001CF RID: 463
		Fields = 4,
		/// <summary> Event definitions. </summary>
		// Token: 0x040001D0 RID: 464
		Events = 8,
		/// <summary> Property definitions. </summary>
		// Token: 0x040001D1 RID: 465
		Properties = 16,
		/// <summary> All member definitions (i.e. type, methods, fields, events and properties). </summary>
		// Token: 0x040001D2 RID: 466
		AllMembers = 31,
		/// <summary> Module definitions. </summary>
		// Token: 0x040001D3 RID: 467
		Modules = 32,
		/// <summary> All definitions (i.e. All member definitions and modules). </summary>
		// Token: 0x040001D4 RID: 468
		AllDefinitions = 63
	}
}
