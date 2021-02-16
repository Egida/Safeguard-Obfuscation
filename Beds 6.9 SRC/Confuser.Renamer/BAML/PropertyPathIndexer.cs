using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000078 RID: 120
	internal class PropertyPathIndexer
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x000034C0 File Offset: 0x000016C0
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x000034C8 File Offset: 0x000016C8
		public string Type
		{
			[CompilerGenerated]
			get
			{
				return this.<Type>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Type>k__BackingField = value;
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x000034D1 File Offset: 0x000016D1
		// (set) Token: 0x060002B5 RID: 693 RVA: 0x000034D9 File Offset: 0x000016D9
		public string Value
		{
			[CompilerGenerated]
			get
			{
				return this.<Value>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Value>k__BackingField = value;
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00002184 File Offset: 0x00000384
		public PropertyPathIndexer()
		{
		}

		// Token: 0x04000531 RID: 1329
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <Type>k__BackingField;

		// Token: 0x04000532 RID: 1330
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <Value>k__BackingField;
	}
}
