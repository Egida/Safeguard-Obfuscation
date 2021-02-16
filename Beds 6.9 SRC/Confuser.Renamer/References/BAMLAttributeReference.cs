using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000023 RID: 35
	internal class BAMLAttributeReference : INameReference<IDnlibDef>, INameReference
	{
		// Token: 0x060000CA RID: 202 RVA: 0x000024FB File Offset: 0x000006FB
		public BAMLAttributeReference(IDnlibDef member, AttributeInfoRecord rec)
		{
			this.member = member;
			this.attrRec = rec;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00002513 File Offset: 0x00000713
		public BAMLAttributeReference(IDnlibDef member, PropertyRecord rec)
		{
			this.member = member;
			this.propRec = rec;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00006EBC File Offset: 0x000050BC
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			bool flag = this.attrRec != null;
			if (flag)
			{
				this.attrRec.Name = this.member.Name;
			}
			else
			{
				this.propRec.Value = this.member.Name;
			}
			return true;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000059 RID: 89
		private readonly AttributeInfoRecord attrRec;

		// Token: 0x0400005A RID: 90
		private readonly IDnlibDef member;

		// Token: 0x0400005B RID: 91
		private readonly PropertyRecord propRec;
	}
}
