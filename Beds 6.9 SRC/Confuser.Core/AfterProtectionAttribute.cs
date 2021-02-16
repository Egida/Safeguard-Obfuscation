using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Core
{
	// Token: 0x02000064 RID: 100
	[AttributeUsage(AttributeTargets.Class)]
	public class AfterProtectionAttribute : Attribute
	{
		// Token: 0x06000249 RID: 585 RVA: 0x00002FE8 File Offset: 0x000011E8
		public AfterProtectionAttribute(params string[] ids)
		{
			this.Ids = ids;
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600024A RID: 586 RVA: 0x00002FFA File Offset: 0x000011FA
		// (set) Token: 0x0600024B RID: 587 RVA: 0x00003002 File Offset: 0x00001202
		public string[] Ids
		{
			[CompilerGenerated]
			get
			{
				return this.<Ids>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Ids>k__BackingField = value;
			}
		}

		// Token: 0x040001CA RID: 458
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string[] <Ids>k__BackingField;
	}
}
