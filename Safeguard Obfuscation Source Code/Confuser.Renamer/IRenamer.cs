using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x0200000A RID: 10
	public interface IRenamer
	{
		// Token: 0x06000052 RID: 82
		void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def);

		// Token: 0x06000053 RID: 83
		void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def);

		// Token: 0x06000054 RID: 84
		void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def);
	}
}
