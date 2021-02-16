using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004E RID: 78
	internal class ElementStartRecord : BamlRecord
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060001EB RID: 491 RVA: 0x00009D40 File Offset: 0x00007F40
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ElementStart;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060001EC RID: 492 RVA: 0x00002F11 File Offset: 0x00001111
		// (set) Token: 0x060001ED RID: 493 RVA: 0x00002F19 File Offset: 0x00001119
		public ushort TypeId
		{
			[CompilerGenerated]
			get
			{
				return this.<TypeId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<TypeId>k__BackingField = value;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060001EE RID: 494 RVA: 0x00002F22 File Offset: 0x00001122
		// (set) Token: 0x060001EF RID: 495 RVA: 0x00002F2A File Offset: 0x0000112A
		public byte Flags
		{
			[CompilerGenerated]
			get
			{
				return this.<Flags>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Flags>k__BackingField = value;
			}
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00002F33 File Offset: 0x00001133
		public override void Read(BamlBinaryReader reader)
		{
			this.TypeId = reader.ReadUInt16();
			this.Flags = reader.ReadByte();
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00002F50 File Offset: 0x00001150
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.TypeId);
			writer.Write(this.Flags);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00002798 File Offset: 0x00000998
		public ElementStartRecord()
		{
		}

		// Token: 0x04000105 RID: 261
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <TypeId>k__BackingField;

		// Token: 0x04000106 RID: 262
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private byte <Flags>k__BackingField;
	}
}
