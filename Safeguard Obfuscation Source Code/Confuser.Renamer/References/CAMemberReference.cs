using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000022 RID: 34
	internal class CAMemberReference : INameReference<IDnlibDef>, INameReference
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x000024E3 File Offset: 0x000006E3
		public CAMemberReference(CANamedArgument namedArg, IDnlibDef definition)
		{
			this.namedArg = namedArg;
			this.definition = definition;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00006E90 File Offset: 0x00005090
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.namedArg.Name = this.definition.Name;
			return true;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000057 RID: 87
		private readonly IDnlibDef definition;

		// Token: 0x04000058 RID: 88
		private readonly CANamedArgument namedArg;
	}
}
