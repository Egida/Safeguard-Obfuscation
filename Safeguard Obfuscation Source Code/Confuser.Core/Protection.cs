using System;

namespace Confuser.Core
{
	// Token: 0x02000062 RID: 98
	public abstract class Protection : ConfuserComponent
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000244 RID: 580
		public abstract ProtectionPreset Preset { get; }

		// Token: 0x06000245 RID: 581 RVA: 0x00002EA7 File Offset: 0x000010A7
		protected Protection()
		{
		}
	}
}
