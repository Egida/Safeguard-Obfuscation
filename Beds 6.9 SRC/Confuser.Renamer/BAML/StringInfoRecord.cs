using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000046 RID: 70
	internal class StringInfoRecord : SizedBamlRecord
	{
		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00009CA0 File Offset: 0x00007EA0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.StringInfo;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x00002C8A File Offset: 0x00000E8A
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x00002C92 File Offset: 0x00000E92
		public ushort StringId
		{
			[CompilerGenerated]
			get
			{
				return this.<StringId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<StringId>k__BackingField = value;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x00002C9B File Offset: 0x00000E9B
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x00002CA3 File Offset: 0x00000EA3
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

		// Token: 0x060001B4 RID: 436 RVA: 0x00002CAC File Offset: 0x00000EAC
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.StringId = reader.ReadUInt16();
			this.Value = reader.ReadString();
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00002CC9 File Offset: 0x00000EC9
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.StringId);
			writer.Write(this.Value);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x000027D4 File Offset: 0x000009D4
		public StringInfoRecord()
		{
		}

		// Token: 0x040000F7 RID: 247
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <StringId>k__BackingField;

		// Token: 0x040000F8 RID: 248
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <Value>k__BackingField;
	}
}
