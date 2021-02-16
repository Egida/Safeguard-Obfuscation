using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000069 RID: 105
	internal class LineNumberAndPositionRecord : BamlRecord
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000278 RID: 632 RVA: 0x0000A100 File Offset: 0x00008300
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.LineNumberAndPosition;
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000279 RID: 633 RVA: 0x0000338D File Offset: 0x0000158D
		// (set) Token: 0x0600027A RID: 634 RVA: 0x00003395 File Offset: 0x00001595
		public uint LineNumber
		{
			[CompilerGenerated]
			get
			{
				return this.<LineNumber>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<LineNumber>k__BackingField = value;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x0600027B RID: 635 RVA: 0x0000339E File Offset: 0x0000159E
		// (set) Token: 0x0600027C RID: 636 RVA: 0x000033A6 File Offset: 0x000015A6
		public uint LinePosition
		{
			[CompilerGenerated]
			get
			{
				return this.<LinePosition>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<LinePosition>k__BackingField = value;
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000033AF File Offset: 0x000015AF
		public override void Read(BamlBinaryReader reader)
		{
			this.LineNumber = reader.ReadUInt32();
			this.LinePosition = reader.ReadUInt32();
		}

		// Token: 0x0600027E RID: 638 RVA: 0x000033CC File Offset: 0x000015CC
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.LineNumber);
			writer.Write(this.LinePosition);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00002798 File Offset: 0x00000998
		public LineNumberAndPositionRecord()
		{
		}

		// Token: 0x0400011B RID: 283
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint <LineNumber>k__BackingField;

		// Token: 0x0400011C RID: 284
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private uint <LinePosition>k__BackingField;
	}
}
