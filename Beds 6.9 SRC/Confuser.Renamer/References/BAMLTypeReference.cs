using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000021 RID: 33
	internal class BAMLTypeReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000C4 RID: 196 RVA: 0x000024CB File Offset: 0x000006CB
		public BAMLTypeReference(TypeSig sig, TypeInfoRecord rec)
		{
			this.sig = sig;
			this.rec = rec;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006E64 File Offset: 0x00005064
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.rec.TypeFullName = this.sig.ReflectionFullName;
			return true;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000055 RID: 85
		private readonly TypeInfoRecord rec;

		// Token: 0x04000056 RID: 86
		private readonly TypeSig sig;
	}
}
