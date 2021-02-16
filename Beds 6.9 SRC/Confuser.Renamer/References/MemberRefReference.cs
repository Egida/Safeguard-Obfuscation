using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x0200002B RID: 43
	public class MemberRefReference : INameReference<IDnlibDef>, INameReference
	{
		// Token: 0x060000E4 RID: 228 RVA: 0x000025EC File Offset: 0x000007EC
		public MemberRefReference(MemberRef memberRef, IDnlibDef memberDef)
		{
			this.memberRef = memberRef;
			this.memberDef = memberDef;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000074B8 File Offset: 0x000056B8
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.memberRef.Name = this.memberDef.Name;
			return true;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x0400006D RID: 109
		private readonly IDnlibDef memberDef;

		// Token: 0x0400006E RID: 110
		private readonly MemberRef memberRef;
	}
}
