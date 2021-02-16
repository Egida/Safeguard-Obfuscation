using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005A RID: 90
	internal class PropertyListEndRecord : BamlRecord
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000234 RID: 564 RVA: 0x00009FD4 File Offset: 0x000081D4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyListEnd;
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00002798 File Offset: 0x00000998
		public PropertyListEndRecord()
		{
		}
	}
}
