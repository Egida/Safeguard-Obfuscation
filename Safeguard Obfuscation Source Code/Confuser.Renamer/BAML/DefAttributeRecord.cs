using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000041 RID: 65
	internal class DefAttributeRecord : SizedBamlRecord
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00009A48 File Offset: 0x00007C48
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DefAttribute;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00002A20 File Offset: 0x00000C20
		// (set) Token: 0x0600017E RID: 382 RVA: 0x00002A28 File Offset: 0x00000C28
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00002A31 File Offset: 0x00000C31
		// (set) Token: 0x06000180 RID: 384 RVA: 0x00002A39 File Offset: 0x00000C39
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

		// Token: 0x06000181 RID: 385 RVA: 0x00002A42 File Offset: 0x00000C42
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Value = reader.ReadString();
			this.NameId = reader.ReadUInt16();
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00002A5F File Offset: 0x00000C5F
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
			writer.Write(this.NameId);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000027D4 File Offset: 0x000009D4
		public DefAttributeRecord()
		{
		}

		// Token: 0x040000E8 RID: 232
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Value>k__BackingField;

		// Token: 0x040000E9 RID: 233
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <NameId>k__BackingField;
	}
}
