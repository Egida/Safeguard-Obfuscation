using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003E RID: 62
	internal class PropertyRecord : SizedBamlRecord
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000164 RID: 356 RVA: 0x000099B8 File Offset: 0x00007BB8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.Property;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000165 RID: 357 RVA: 0x0000291C File Offset: 0x00000B1C
		// (set) Token: 0x06000166 RID: 358 RVA: 0x00002924 File Offset: 0x00000B24
		public ushort AttributeId
		{
			[CompilerGenerated]
			get
			{
				return this.<AttributeId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<AttributeId>k__BackingField = value;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000167 RID: 359 RVA: 0x0000292D File Offset: 0x00000B2D
		// (set) Token: 0x06000168 RID: 360 RVA: 0x00002935 File Offset: 0x00000B35
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

		// Token: 0x06000169 RID: 361 RVA: 0x0000293E File Offset: 0x00000B3E
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.AttributeId = reader.ReadUInt16();
			this.Value = reader.ReadString();
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000295B File Offset: 0x00000B5B
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			writer.Write(this.Value);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x000027D4 File Offset: 0x000009D4
		public PropertyRecord()
		{
		}

		// Token: 0x040000E2 RID: 226
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <AttributeId>k__BackingField;

		// Token: 0x040000E3 RID: 227
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <Value>k__BackingField;
	}
}
