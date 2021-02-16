using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005D RID: 93
	internal class PropertyArrayStartRecord : PropertyComplexStartRecord
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600023E RID: 574 RVA: 0x0000A010 File Offset: 0x00008210
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyArrayStart;
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000306F File Offset: 0x0000126F
		public PropertyArrayStartRecord()
		{
		}
	}
}
