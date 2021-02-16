using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x0200002C RID: 44
	internal class ResourceReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000E7 RID: 231 RVA: 0x00002604 File Offset: 0x00000804
		public ResourceReference(Resource resource, TypeDef typeDef, string format)
		{
			this.resource = resource;
			this.typeDef = typeDef;
			this.format = format;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000074E4 File Offset: 0x000056E4
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.resource.Name = string.Format(this.format, this.typeDef.ReflectionFullName);
			return true;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x0400006F RID: 111
		private readonly string format;

		// Token: 0x04000070 RID: 112
		private readonly Resource resource;

		// Token: 0x04000071 RID: 113
		private readonly TypeDef typeDef;
	}
}
