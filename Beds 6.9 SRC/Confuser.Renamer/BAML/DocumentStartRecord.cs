using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004C RID: 76
	internal class DocumentStartRecord : BamlRecord
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001DD RID: 477 RVA: 0x00009D18 File Offset: 0x00007F18
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DocumentStart;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001DE RID: 478 RVA: 0x00002E87 File Offset: 0x00001087
		// (set) Token: 0x060001DF RID: 479 RVA: 0x00002E8F File Offset: 0x0000108F
		public bool LoadAsync
		{
			[CompilerGenerated]
			get
			{
				return this.<LoadAsync>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<LoadAsync>k__BackingField = value;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x00002E98 File Offset: 0x00001098
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x00002EA0 File Offset: 0x000010A0
		public uint MaxAsyncRecords
		{
			[CompilerGenerated]
			get
			{
				return this.<MaxAsyncRecords>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<MaxAsyncRecords>k__BackingField = value;
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x00002EA9 File Offset: 0x000010A9
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x00002EB1 File Offset: 0x000010B1
		public bool DebugBaml
		{
			[CompilerGenerated]
			get
			{
				return this.<DebugBaml>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<DebugBaml>k__BackingField = value;
			}
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00002EBA File Offset: 0x000010BA
		public override void Read(BamlBinaryReader reader)
		{
			this.LoadAsync = reader.ReadBoolean();
			this.MaxAsyncRecords = reader.ReadUInt32();
			this.DebugBaml = reader.ReadBoolean();
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00002EE4 File Offset: 0x000010E4
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.LoadAsync);
			writer.Write(this.MaxAsyncRecords);
			writer.Write(this.DebugBaml);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00002798 File Offset: 0x00000998
		public DocumentStartRecord()
		{
		}

		// Token: 0x04000102 RID: 258
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool <LoadAsync>k__BackingField;

		// Token: 0x04000103 RID: 259
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint <MaxAsyncRecords>k__BackingField;

		// Token: 0x04000104 RID: 260
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool <DebugBaml>k__BackingField;
	}
}
