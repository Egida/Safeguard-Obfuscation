using System;

namespace Confuser.Core
{
	// Token: 0x02000043 RID: 67
	public abstract class ConfuserComponent
	{
		// Token: 0x0600016D RID: 365
		protected internal abstract void Initialize(ConfuserContext context);

		// Token: 0x0600016E RID: 366
		protected internal abstract void PopulatePipeline(ProtectionPipeline pipeline);

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600016F RID: 367
		public abstract string Description { get; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000170 RID: 368
		public abstract string FullId { get; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000171 RID: 369
		public abstract string Id { get; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000172 RID: 370
		public abstract string Name { get; }

		// Token: 0x06000173 RID: 371 RVA: 0x00002194 File Offset: 0x00000394
		protected ConfuserComponent()
		{
		}
	}
}
