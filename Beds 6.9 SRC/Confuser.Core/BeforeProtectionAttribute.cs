using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Core
{
	// Token: 0x02000063 RID: 99
	[AttributeUsage(AttributeTargets.Class)]
	public class BeforeProtectionAttribute : Attribute
	{
		// Token: 0x06000246 RID: 582 RVA: 0x00002FC5 File Offset: 0x000011C5
		public BeforeProtectionAttribute(params string[] ids)
		{
			this.Ids = ids;
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000247 RID: 583 RVA: 0x00002FD7 File Offset: 0x000011D7
		// (set) Token: 0x06000248 RID: 584 RVA: 0x00002FDF File Offset: 0x000011DF
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

		// Token: 0x040001C9 RID: 457
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string[] <Ids>k__BackingField;
	}
}
