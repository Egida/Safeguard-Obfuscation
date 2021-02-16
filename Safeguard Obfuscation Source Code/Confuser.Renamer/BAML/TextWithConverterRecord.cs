using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000048 RID: 72
	internal class TextWithConverterRecord : TextRecord
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001BD RID: 445 RVA: 0x00009CC8 File Offset: 0x00007EC8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.TextWithConverter;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00002D17 File Offset: 0x00000F17
		// (set) Token: 0x060001BF RID: 447 RVA: 0x00002D1F File Offset: 0x00000F1F
		public ushort ConverterTypeId
		{
			[CompilerGenerated]
			get
			{
				return this.<ConverterTypeId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<ConverterTypeId>k__BackingField = value;
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00002D28 File Offset: 0x00000F28
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			base.ReadData(reader, size);
			this.ConverterTypeId = reader.ReadUInt16();
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00002D41 File Offset: 0x00000F41
		protected override void WriteData(BamlBinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.ConverterTypeId);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00002D59 File Offset: 0x00000F59
		public TextWithConverterRecord()
		{
		}

		// Token: 0x040000FA RID: 250
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <ConverterTypeId>k__BackingField;
	}
}
