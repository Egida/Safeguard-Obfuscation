using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004F RID: 79
	internal class ElementEndRecord : BamlRecord
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x00009D54 File Offset: 0x00007F54
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ElementEnd;
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00002798 File Offset: 0x00000998
		public ElementEndRecord()
		{
		}
	}
}
