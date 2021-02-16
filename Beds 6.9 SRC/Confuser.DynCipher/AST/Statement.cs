using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000028 RID: 40
	public abstract class Statement
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00006B87 File Offset: 0x00004D87
		// (set) Token: 0x060000DA RID: 218 RVA: 0x00006B8F File Offset: 0x00004D8F
		public object Tag
		{
			[CompilerGenerated]
			get
			{
				return this.<Tag>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Tag>k__BackingField = value;
			}
		}

		// Token: 0x060000DB RID: 219
		public abstract override string ToString();

		// Token: 0x060000DC RID: 220 RVA: 0x000020FC File Offset: 0x000002FC
		protected Statement()
		{
		}

		// Token: 0x0400005D RID: 93
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object <Tag>k__BackingField;
	}
}
