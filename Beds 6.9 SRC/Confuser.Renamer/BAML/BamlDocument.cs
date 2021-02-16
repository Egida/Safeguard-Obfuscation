using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000032 RID: 50
	internal class BamlDocument : List<BamlRecord>
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600010F RID: 271 RVA: 0x000026A1 File Offset: 0x000008A1
		// (set) Token: 0x06000110 RID: 272 RVA: 0x000026A9 File Offset: 0x000008A9
		public string DocumentName
		{
			[CompilerGenerated]
			get
			{
				return this.<DocumentName>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<DocumentName>k__BackingField = value;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000111 RID: 273 RVA: 0x000026B2 File Offset: 0x000008B2
		// (set) Token: 0x06000112 RID: 274 RVA: 0x000026BA File Offset: 0x000008BA
		public string Signature
		{
			[CompilerGenerated]
			get
			{
				return this.<Signature>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Signature>k__BackingField = value;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000113 RID: 275 RVA: 0x000026C3 File Offset: 0x000008C3
		// (set) Token: 0x06000114 RID: 276 RVA: 0x000026CB File Offset: 0x000008CB
		public BamlDocument.BamlVersion ReaderVersion
		{
			[CompilerGenerated]
			get
			{
				return this.<ReaderVersion>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<ReaderVersion>k__BackingField = value;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000115 RID: 277 RVA: 0x000026D4 File Offset: 0x000008D4
		// (set) Token: 0x06000116 RID: 278 RVA: 0x000026DC File Offset: 0x000008DC
		public BamlDocument.BamlVersion UpdaterVersion
		{
			[CompilerGenerated]
			get
			{
				return this.<UpdaterVersion>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<UpdaterVersion>k__BackingField = value;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000117 RID: 279 RVA: 0x000026E5 File Offset: 0x000008E5
		// (set) Token: 0x06000118 RID: 280 RVA: 0x000026ED File Offset: 0x000008ED
		public BamlDocument.BamlVersion WriterVersion
		{
			[CompilerGenerated]
			get
			{
				return this.<WriterVersion>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<WriterVersion>k__BackingField = value;
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000026F6 File Offset: 0x000008F6
		public BamlDocument()
		{
		}

		// Token: 0x0400008E RID: 142
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <DocumentName>k__BackingField;

		// Token: 0x0400008F RID: 143
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Signature>k__BackingField;

		// Token: 0x04000090 RID: 144
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private BamlDocument.BamlVersion <ReaderVersion>k__BackingField;

		// Token: 0x04000091 RID: 145
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private BamlDocument.BamlVersion <UpdaterVersion>k__BackingField;

		// Token: 0x04000092 RID: 146
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private BamlDocument.BamlVersion <WriterVersion>k__BackingField;

		// Token: 0x02000033 RID: 51
		public struct BamlVersion
		{
			// Token: 0x04000093 RID: 147
			public ushort Major;

			// Token: 0x04000094 RID: 148
			public ushort Minor;
		}
	}
}
