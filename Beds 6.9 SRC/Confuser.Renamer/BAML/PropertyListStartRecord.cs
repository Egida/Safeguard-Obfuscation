using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000059 RID: 89
	internal class PropertyListStartRecord : PropertyComplexStartRecord
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000232 RID: 562 RVA: 0x00009FC0 File Offset: 0x000081C0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyListStart;
			}
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000306F File Offset: 0x0000126F
		public PropertyListStartRecord()
		{
		}
	}
}
