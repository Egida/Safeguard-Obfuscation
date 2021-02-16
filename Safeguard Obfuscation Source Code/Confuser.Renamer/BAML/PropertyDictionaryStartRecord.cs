using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005B RID: 91
	internal class PropertyDictionaryStartRecord : PropertyComplexStartRecord
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000238 RID: 568 RVA: 0x00009FE8 File Offset: 0x000081E8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyDictionaryStart;
			}
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000306F File Offset: 0x0000126F
		public PropertyDictionaryStartRecord()
		{
		}
	}
}
