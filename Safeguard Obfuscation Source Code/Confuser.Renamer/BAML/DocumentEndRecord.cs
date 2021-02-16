using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004D RID: 77
	internal class DocumentEndRecord : BamlRecord
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060001E7 RID: 487 RVA: 0x00009D2C File Offset: 0x00007F2C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DocumentEnd;
			}
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00002798 File Offset: 0x00000998
		public DocumentEndRecord()
		{
		}
	}
}
