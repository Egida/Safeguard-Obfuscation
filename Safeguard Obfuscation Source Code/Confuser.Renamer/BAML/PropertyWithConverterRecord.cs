using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003F RID: 63
	internal class PropertyWithConverterRecord : PropertyRecord
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600016C RID: 364 RVA: 0x000099CC File Offset: 0x00007BCC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyWithConverter;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00002978 File Offset: 0x00000B78
		// (set) Token: 0x0600016E RID: 366 RVA: 0x00002980 File Offset: 0x00000B80
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

		// Token: 0x0600016F RID: 367 RVA: 0x00002989 File Offset: 0x00000B89
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			base.ReadData(reader, size);
			this.ConverterTypeId = reader.ReadUInt16();
		}

		// Token: 0x06000170 RID: 368 RVA: 0x000029A2 File Offset: 0x00000BA2
		protected override void WriteData(BamlBinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.ConverterTypeId);
		}

		// Token: 0x06000171 RID: 369 RVA: 0x000029BA File Offset: 0x00000BBA
		public PropertyWithConverterRecord()
		{
		}

		// Token: 0x040000E4 RID: 228
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <ConverterTypeId>k__BackingField;
	}
}
