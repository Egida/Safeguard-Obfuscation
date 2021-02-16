using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000027 RID: 39
	public class TypeRefReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000D8 RID: 216 RVA: 0x000025A8 File Offset: 0x000007A8
		public TypeRefReference(TypeRef typeRef, TypeDef typeDef)
		{
			this.typeRef = typeRef;
			this.typeDef = typeDef;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00007070 File Offset: 0x00005270
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.typeRef.Namespace = this.typeDef.Namespace;
			this.typeRef.Name = this.typeDef.Name;
			return true;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000066 RID: 102
		private readonly TypeDef typeDef;

		// Token: 0x04000067 RID: 103
		private readonly TypeRef typeRef;
	}
}
