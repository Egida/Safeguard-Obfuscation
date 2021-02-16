using System;

namespace Confuser.Core
{
	// Token: 0x02000067 RID: 103
	[Flags]
	public enum ProtectionTargets
	{
		// Token: 0x040001CD RID: 461
		Types = 1,
		// Token: 0x040001CE RID: 462
		Methods = 2,
		// Token: 0x040001CF RID: 463
		Fields = 4,
		// Token: 0x040001D0 RID: 464
		Events = 8,
		// Token: 0x040001D1 RID: 465
		Properties = 16,
		// Token: 0x040001D2 RID: 466
		AllMembers = 31,
		// Token: 0x040001D3 RID: 467
		Modules = 32,
		// Token: 0x040001D4 RID: 468
		AllDefinitions = 63
	}
}
