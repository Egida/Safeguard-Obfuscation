using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000014 RID: 20
	public interface DupCtrlFlowService
	{
		// Token: 0x06000063 RID: 99
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
