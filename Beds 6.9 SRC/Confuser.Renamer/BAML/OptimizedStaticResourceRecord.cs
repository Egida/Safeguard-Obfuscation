using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000068 RID: 104
	internal class OptimizedStaticResourceRecord : BamlRecord
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000270 RID: 624 RVA: 0x0000A0EC File Offset: 0x000082EC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.OptimizedStaticResource;
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000271 RID: 625 RVA: 0x00003331 File Offset: 0x00001531
		// (set) Token: 0x06000272 RID: 626 RVA: 0x00003339 File Offset: 0x00001539
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

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000273 RID: 627 RVA: 0x00003342 File Offset: 0x00001542
		// (set) Token: 0x06000274 RID: 628 RVA: 0x0000334A File Offset: 0x0000154A
		public ushort ValueId
		{
			[CompilerGenerated]
			get
			{
				return this.<ValueId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<ValueId>k__BackingField = value;
			}
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00003353 File Offset: 0x00001553
		public override void Read(BamlBinaryReader reader)
		{
			this.Flags = reader.ReadByte();
			this.ValueId = reader.ReadUInt16();
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00003370 File Offset: 0x00001570
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.Flags);
			writer.Write(this.ValueId);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00002798 File Offset: 0x00000998
		public OptimizedStaticResourceRecord()
		{
		}

		// Token: 0x04000119 RID: 281
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private byte <Flags>k__BackingField;

		// Token: 0x0400011A RID: 282
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <ValueId>k__BackingField;
	}
}
