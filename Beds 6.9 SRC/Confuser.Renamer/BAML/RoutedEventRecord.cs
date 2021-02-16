using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004B RID: 75
	internal class RoutedEventRecord : SizedBamlRecord
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x00009D04 File Offset: 0x00007F04
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.RoutedEvent;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x00002E1A File Offset: 0x0000101A
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x00002E22 File Offset: 0x00001022
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

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x00002E2B File Offset: 0x0000102B
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x00002E33 File Offset: 0x00001033
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

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x00002E3C File Offset: 0x0000103C
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x00002E44 File Offset: 0x00001044
		public uint Reserved1
		{
			[CompilerGenerated]
			get
			{
				return this.<Reserved1>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Reserved1>k__BackingField = value;
			}
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00002E4D File Offset: 0x0000104D
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.AttributeId = reader.ReadUInt16();
			this.Value = reader.ReadString();
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00002E6A File Offset: 0x0000106A
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
			writer.Write(this.AttributeId);
		}

		// Token: 0x060001DC RID: 476 RVA: 0x000027D4 File Offset: 0x000009D4
		public RoutedEventRecord()
		{
		}

		// Token: 0x040000FF RID: 255
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Value>k__BackingField;

		// Token: 0x04000100 RID: 256
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <AttributeId>k__BackingField;

		// Token: 0x04000101 RID: 257
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private uint <Reserved1>k__BackingField;
	}
}
