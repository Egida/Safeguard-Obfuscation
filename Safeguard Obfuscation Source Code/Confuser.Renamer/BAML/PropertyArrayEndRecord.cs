using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005E RID: 94
	internal class PropertyArrayEndRecord : BamlRecord
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000240 RID: 576 RVA: 0x0000A024 File Offset: 0x00008224
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyArrayEnd;
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00002798 File Offset: 0x00000998
		public PropertyArrayEndRecord()
		{
		}
	}
}
