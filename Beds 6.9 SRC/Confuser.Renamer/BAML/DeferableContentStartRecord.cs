using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000064 RID: 100
	internal class DeferableContentStartRecord : BamlRecord, IBamlDeferRecord
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600025C RID: 604 RVA: 0x0000A09C File Offset: 0x0000829C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DeferableContentStart;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600025D RID: 605 RVA: 0x00003249 File Offset: 0x00001449
		// (set) Token: 0x0600025E RID: 606 RVA: 0x00003251 File Offset: 0x00001451
		public BamlRecord Record
		{
			[CompilerGenerated]
			get
			{
				return this.<Record>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Record>k__BackingField = value;
			}
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000325A File Offset: 0x0000145A
		public void ReadDefer(BamlDocument doc, int index, Func<long, BamlRecord> resolve)
		{
			this.Record = resolve(this.pos + (long)((ulong)this.size));
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00003278 File Offset: 0x00001478
		public void WriteDefer(BamlDocument doc, int index, BinaryWriter wtr)
		{
			wtr.BaseStream.Seek(this.pos, SeekOrigin.Begin);
			wtr.Write((uint)(this.Record.Position - (this.pos + 4L)));
		}

		// Token: 0x06000261 RID: 609 RVA: 0x000032AB File Offset: 0x000014AB
		public override void Read(BamlBinaryReader reader)
		{
			this.size = reader.ReadUInt32();
			this.pos = reader.BaseStream.Position;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x000032CB File Offset: 0x000014CB
		public override void Write(BamlBinaryWriter writer)
		{
			this.pos = writer.BaseStream.Position;
			writer.Write(0U);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x000032E7 File Offset: 0x000014E7
		public DeferableContentStartRecord()
		{
		}

		// Token: 0x04000115 RID: 277
		private long pos;

		// Token: 0x04000116 RID: 278
		internal uint size = uint.MaxValue;

		// Token: 0x04000117 RID: 279
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BamlRecord <Record>k__BackingField;
	}
}
