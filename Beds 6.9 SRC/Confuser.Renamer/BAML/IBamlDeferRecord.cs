using System;
using System.IO;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000039 RID: 57
	internal interface IBamlDeferRecord
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600013C RID: 316
		// (set) Token: 0x0600013D RID: 317
		BamlRecord Record { get; set; }

		// Token: 0x0600013E RID: 318
		void ReadDefer(BamlDocument doc, int index, Func<long, BamlRecord> resolve);

		// Token: 0x0600013F RID: 319
		void WriteDefer(BamlDocument doc, int index, BinaryWriter wtr);
	}
}
