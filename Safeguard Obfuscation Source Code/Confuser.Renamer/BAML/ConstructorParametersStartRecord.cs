using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000061 RID: 97
	internal class ConstructorParametersStartRecord : BamlRecord
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600024E RID: 590 RVA: 0x0000A060 File Offset: 0x00008260
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ConstructorParametersStart;
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00002798 File Offset: 0x00000998
		public ConstructorParametersStartRecord()
		{
		}
	}
}
