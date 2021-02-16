using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x0200001F RID: 31
	internal class BAMLEnumReference : INameReference<FieldDef>, INameReference
	{
		// Token: 0x060000BD RID: 189 RVA: 0x00002467 File Offset: 0x00000667
		public BAMLEnumReference(FieldDef enumField, PropertyRecord rec)
		{
			this.enumField = enumField;
			this.rec = rec;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00006D68 File Offset: 0x00004F68
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.rec.Value = this.enumField.Name;
			return true;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x0400004F RID: 79
		private readonly FieldDef enumField;

		// Token: 0x04000050 RID: 80
		private readonly PropertyRecord rec;
	}
}
