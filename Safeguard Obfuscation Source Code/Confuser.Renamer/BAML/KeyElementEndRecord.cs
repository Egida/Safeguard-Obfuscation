using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000051 RID: 81
	internal class KeyElementEndRecord : BamlRecord
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x00009D7C File Offset: 0x00007F7C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.KeyElementEnd;
			}
		}

		// Token: 0x060001FA RID: 506 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00002798 File Offset: 0x00000998
		public KeyElementEndRecord()
		{
		}
	}
}
