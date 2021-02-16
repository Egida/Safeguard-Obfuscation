using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000015 RID: 21
	public interface DupCtrlFlowModService
	{
		// Token: 0x06000064 RID: 100
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
