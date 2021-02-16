using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000062 RID: 98
	internal class ConstructorParametersEndRecord : BamlRecord
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000252 RID: 594 RVA: 0x0000A074 File Offset: 0x00008274
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ConstructorParametersEnd;
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00002F0E File Offset: 0x0000110E
		public override void Write(BamlBinaryWriter writer)
		{
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00002798 File Offset: 0x00000998
		public ConstructorParametersEndRecord()
		{
		}
	}
}
