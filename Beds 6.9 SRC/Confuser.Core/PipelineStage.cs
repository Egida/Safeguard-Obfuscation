using System;

namespace Confuser.Core
{
	// Token: 0x0200006A RID: 106
	public enum PipelineStage
	{
		// Token: 0x040001EF RID: 495
		Inspection,
		// Token: 0x040001F0 RID: 496
		BeginModule,
		// Token: 0x040001F1 RID: 497
		ProcessModule,
		// Token: 0x040001F2 RID: 498
		OptimizeMethods,
		// Token: 0x040001F3 RID: 499
		EndModule,
		// Token: 0x040001F4 RID: 500
		WriteModule,
		// Token: 0x040001F5 RID: 501
		Debug,
		// Token: 0x040001F6 RID: 502
		Pack,
		// Token: 0x040001F7 RID: 503
		SaveModules
	}
}
