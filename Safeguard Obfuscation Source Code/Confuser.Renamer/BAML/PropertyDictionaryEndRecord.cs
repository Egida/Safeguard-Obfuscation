using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005C RID: 92
	internal class PropertyDictionaryEndRecord : BamlRecord
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600023A RID: 570 RVA: 0x00009FFC File Offset: 0x000081FC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyDictionaryEnd;
			}
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00002798 File Offset: 0x00000998
		public PropertyDictionaryEndRecord()
		{
		}
	}
}
