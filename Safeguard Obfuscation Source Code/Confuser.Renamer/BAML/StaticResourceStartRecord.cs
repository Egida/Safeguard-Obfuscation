using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000065 RID: 101
	internal class StaticResourceStartRecord : ElementStartRecord
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000264 RID: 612 RVA: 0x0000A0B0 File Offset: 0x000082B0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.StaticResourceStart;
			}
		}

		// Token: 0x06000265 RID: 613 RVA: 0x000032F7 File Offset: 0x000014F7
		public StaticResourceStartRecord()
		{
		}
	}
}
