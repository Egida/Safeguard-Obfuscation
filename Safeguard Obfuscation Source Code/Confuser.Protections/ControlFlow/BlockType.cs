using System;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000E2 RID: 226
	internal enum BlockType
	{
		// Token: 0x0400028F RID: 655
		Normal,
		// Token: 0x04000290 RID: 656
		Try,
		// Token: 0x04000291 RID: 657
		Handler,
		// Token: 0x04000292 RID: 658
		Finally,
		// Token: 0x04000293 RID: 659
		Filter,
		// Token: 0x04000294 RID: 660
		Fault
	}
}
