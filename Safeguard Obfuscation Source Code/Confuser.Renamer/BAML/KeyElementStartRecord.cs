using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000050 RID: 80
	internal class KeyElementStartRecord : DefAttributeKeyTypeRecord
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x00009D68 File Offset: 0x00007F68
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.KeyElementStart;
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00002F6D File Offset: 0x0000116D
		public KeyElementStartRecord()
		{
		}
	}
}
