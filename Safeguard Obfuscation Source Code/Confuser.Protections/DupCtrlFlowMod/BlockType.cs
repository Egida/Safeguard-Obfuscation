using System;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000CD RID: 205
	internal enum BlockType
	{
		// Token: 0x0400024E RID: 590
		Normal,
		// Token: 0x0400024F RID: 591
		Try,
		// Token: 0x04000250 RID: 592
		Handler,
		// Token: 0x04000251 RID: 593
		Finally,
		// Token: 0x04000252 RID: 594
		Filter,
		// Token: 0x04000253 RID: 595
		Fault
	}
}
