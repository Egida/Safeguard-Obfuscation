using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200002E RID: 46
	public interface IDupControlFlowService2
	{
		// Token: 0x060000D6 RID: 214
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
