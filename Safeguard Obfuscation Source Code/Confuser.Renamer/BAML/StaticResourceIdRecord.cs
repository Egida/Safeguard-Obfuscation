using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000067 RID: 103
	internal class StaticResourceIdRecord : BamlRecord
	{
		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600026A RID: 618 RVA: 0x0000A0D8 File Offset: 0x000082D8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.StaticResourceId;
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600026B RID: 619 RVA: 0x00003300 File Offset: 0x00001500
		// (set) Token: 0x0600026C RID: 620 RVA: 0x00003308 File Offset: 0x00001508
		public ushort StaticResourceId
		{
			[CompilerGenerated]
			get
			{
				return this.<StaticResourceId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<StaticResourceId>k__BackingField = value;
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00003311 File Offset: 0x00001511
		public override void Read(BamlBinaryReader reader)
		{
			this.StaticResourceId = reader.ReadUInt16();
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00003321 File Offset: 0x00001521
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.StaticResourceId);
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00002798 File Offset: 0x00000998
		public StaticResourceIdRecord()
		{
		}

		// Token: 0x04000118 RID: 280
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <StaticResourceId>k__BackingField;
	}
}
