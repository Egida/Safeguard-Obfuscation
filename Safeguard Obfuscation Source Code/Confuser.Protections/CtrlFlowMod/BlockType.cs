using System;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000B8 RID: 184
	internal enum BlockType
	{
		// Token: 0x0400020D RID: 525
		Normal,
		// Token: 0x0400020E RID: 526
		Try,
		// Token: 0x0400020F RID: 527
		Handler,
		// Token: 0x04000210 RID: 528
		Finally,
		// Token: 0x04000211 RID: 529
		Filter,
		// Token: 0x04000212 RID: 530
		Fault
	}
}
