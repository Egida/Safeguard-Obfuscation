using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000060 RID: 96
	internal class PropertyComplexEndRecord : BamlRecord
	{
		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600024A RID: 586 RVA: 0x0000A04C File Offset: 0x0000824C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyComplexEnd;
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00002798 File Offset: 0x00000998
		public PropertyComplexEndRecord()
		{
		}
	}
}
