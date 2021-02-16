using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000066 RID: 102
	internal class StaticResourceEndRecord : BamlRecord
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000266 RID: 614 RVA: 0x0000A0C4 File Offset: 0x000082C4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.StaticResourceEnd;
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00002798 File Offset: 0x00000998
		public StaticResourceEndRecord()
		{
		}
	}
}
