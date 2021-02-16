using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	// Token: 0x02000066 RID: 102
	public class ProtectionSettings : Dictionary<ConfuserComponent, Dictionary<string, string>>
	{
		// Token: 0x06000253 RID: 595 RVA: 0x0000302E File Offset: 0x0000122E
		public ProtectionSettings()
		{
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00011408 File Offset: 0x0000F608
		public ProtectionSettings(ProtectionSettings settings)
		{
			foreach (KeyValuePair<ConfuserComponent, Dictionary<string, string>> i in settings)
			{
				base.Add(i.Key, new Dictionary<string, string>(i.Value));
			}
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00011478 File Offset: 0x0000F678
		public bool IsEmpty()
		{
			return base.Count == 0;
		}
	}
}
