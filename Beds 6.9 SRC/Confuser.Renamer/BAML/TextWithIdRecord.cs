using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000049 RID: 73
	internal class TextWithIdRecord : TextRecord
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x00009CDC File Offset: 0x00007EDC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.TextWithId;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x00002D62 File Offset: 0x00000F62
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x00002D6A File Offset: 0x00000F6A
		public ushort ValueId
		{
			[CompilerGenerated]
			get
			{
				return this.<ValueId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<ValueId>k__BackingField = value;
			}
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00002D73 File Offset: 0x00000F73
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.ValueId = reader.ReadUInt16();
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00002D83 File Offset: 0x00000F83
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.ValueId);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00002D59 File Offset: 0x00000F59
		public TextWithIdRecord()
		{
		}

		// Token: 0x040000FB RID: 251
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <ValueId>k__BackingField;
	}
}
