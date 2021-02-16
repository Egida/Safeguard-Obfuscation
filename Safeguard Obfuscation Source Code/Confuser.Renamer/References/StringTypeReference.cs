using System;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.References
{
	// Token: 0x02000026 RID: 38
	public class StringTypeReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x00002590 File Offset: 0x00000790
		public StringTypeReference(Instruction reference, TypeDef typeDef)
		{
			this.reference = reference;
			this.typeDef = typeDef;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00007044 File Offset: 0x00005244
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.reference.Operand = this.typeDef.ReflectionFullName;
			return true;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000064 RID: 100
		private readonly Instruction reference;

		// Token: 0x04000065 RID: 101
		private readonly TypeDef typeDef;
	}
}
