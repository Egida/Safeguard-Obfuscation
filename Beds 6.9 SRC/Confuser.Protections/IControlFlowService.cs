using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200000C RID: 12
	public interface IControlFlowService
	{
		// Token: 0x0600002D RID: 45
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
