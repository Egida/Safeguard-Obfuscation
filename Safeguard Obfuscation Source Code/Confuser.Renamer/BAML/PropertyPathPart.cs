using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000079 RID: 121
	internal class PropertyPathPart
	{
		// Token: 0x060002B7 RID: 695 RVA: 0x000034E2 File Offset: 0x000016E2
		public PropertyPathPart(bool isIndexer, bool? isHiera, string name)
		{
			this.IsIndexer = isIndexer;
			this.IsHierarchical = isHiera;
			this.Name = name;
			this.IndexerArguments = null;
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x0000350C File Offset: 0x0000170C
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x00003514 File Offset: 0x00001714
		public bool IsIndexer
		{
			[CompilerGenerated]
			get
			{
				return this.<IsIndexer>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<IsIndexer>k__BackingField = value;
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002BA RID: 698 RVA: 0x0000351D File Offset: 0x0000171D
		// (set) Token: 0x060002BB RID: 699 RVA: 0x00003525 File Offset: 0x00001725
		public bool? IsHierarchical
		{
			[CompilerGenerated]
			get
			{
				return this.<IsHierarchical>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<IsHierarchical>k__BackingField = value;
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060002BC RID: 700 RVA: 0x0000352E File Offset: 0x0000172E
		// (set) Token: 0x060002BD RID: 701 RVA: 0x00003536 File Offset: 0x00001736
		public string Name
		{
			[CompilerGenerated]
			get
			{
				return this.<Name>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Name>k__BackingField = value;
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060002BE RID: 702 RVA: 0x0000353F File Offset: 0x0000173F
		// (set) Token: 0x060002BF RID: 703 RVA: 0x00003547 File Offset: 0x00001747
		public PropertyPathIndexer[] IndexerArguments
		{
			[CompilerGenerated]
			get
			{
				return this.<IndexerArguments>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<IndexerArguments>k__BackingField = value;
			}
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00020798 File Offset: 0x0001E998
		public bool IsAttachedDP()
		{
			return !this.IsIndexer && this.Name.Length >= 2 && this.Name[0] == '(' && this.Name[this.Name.Length - 1] == ')';
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x000207F0 File Offset: 0x0001E9F0
		public void ExtractAttachedDP(out string type, out string property)
		{
			string name = this.Name.Substring(1, this.Name.Length - 2);
			bool flag = !name.Contains('.');
			if (flag)
			{
				type = null;
				property = name.Trim();
			}
			else
			{
				int dot = name.LastIndexOf('.');
				type = name.Substring(0, dot).Trim();
				property = name.Substring(dot + 1).Trim();
			}
		}

		// Token: 0x04000533 RID: 1331
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool <IsIndexer>k__BackingField;

		// Token: 0x04000534 RID: 1332
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool? <IsHierarchical>k__BackingField;

		// Token: 0x04000535 RID: 1333
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <Name>k__BackingField;

		// Token: 0x04000536 RID: 1334
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private PropertyPathIndexer[] <IndexerArguments>k__BackingField;
	}
}
