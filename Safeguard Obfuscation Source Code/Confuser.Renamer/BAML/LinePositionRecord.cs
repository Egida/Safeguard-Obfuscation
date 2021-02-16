using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200006A RID: 106
	internal class LinePositionRecord : BamlRecord
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000280 RID: 640 RVA: 0x0000A114 File Offset: 0x00008314
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.LinePosition;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000281 RID: 641 RVA: 0x000033E9 File Offset: 0x000015E9
		// (set) Token: 0x06000282 RID: 642 RVA: 0x000033F1 File Offset: 0x000015F1
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

		// Token: 0x06000283 RID: 643 RVA: 0x000033FA File Offset: 0x000015FA
		public override void Read(BamlBinaryReader reader)
		{
			this.LinePosition = reader.ReadUInt32();
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000340A File Offset: 0x0000160A
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.LinePosition);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00002798 File Offset: 0x00000998
		public LinePositionRecord()
		{
		}

		// Token: 0x0400011D RID: 285
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private uint <LinePosition>k__BackingField;
	}
}
