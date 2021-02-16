using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003B RID: 59
	internal class PresentationOptionsAttributeRecord : SizedBamlRecord
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600014A RID: 330 RVA: 0x0000997C File Offset: 0x00007B7C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PresentationOptionsAttribute;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600014B RID: 331 RVA: 0x000027DD File Offset: 0x000009DD
		// (set) Token: 0x0600014C RID: 332 RVA: 0x000027E5 File Offset: 0x000009E5
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

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600014D RID: 333 RVA: 0x000027EE File Offset: 0x000009EE
		// (set) Token: 0x0600014E RID: 334 RVA: 0x000027F6 File Offset: 0x000009F6
		public ushort NameId
		{
			[CompilerGenerated]
			get
			{
				return this.<NameId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<NameId>k__BackingField = value;
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x000027FF File Offset: 0x000009FF
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Value = reader.ReadString();
			this.NameId = reader.ReadUInt16();
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000281C File Offset: 0x00000A1C
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
			writer.Write(this.NameId);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x000027D4 File Offset: 0x000009D4
		public PresentationOptionsAttributeRecord()
		{
		}

		// Token: 0x040000DB RID: 219
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Value>k__BackingField;

		// Token: 0x040000DC RID: 220
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <NameId>k__BackingField;
	}
}
