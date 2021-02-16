using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000044 RID: 68
	internal class TypeSerializerInfoRecord : TypeInfoRecord
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00009C78 File Offset: 0x00007E78
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.TypeSerializerInfo;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600019E RID: 414 RVA: 0x00002B8D File Offset: 0x00000D8D
		// (set) Token: 0x0600019F RID: 415 RVA: 0x00002B95 File Offset: 0x00000D95
		public ushort SerializerTypeId
		{
			[CompilerGenerated]
			get
			{
				return this.<SerializerTypeId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<SerializerTypeId>k__BackingField = value;
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00002B9E File Offset: 0x00000D9E
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			base.ReadData(reader, size);
			this.SerializerTypeId = reader.ReadUInt16();
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00002BB7 File Offset: 0x00000DB7
		protected override void WriteData(BamlBinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.SerializerTypeId);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00002BCF File Offset: 0x00000DCF
		public TypeSerializerInfoRecord()
		{
		}

		// Token: 0x040000F2 RID: 242
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <SerializerTypeId>k__BackingField;
	}
}
