using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000016 RID: 22
	public interface ICtrlFlowModService
	{
		// Token: 0x06000065 RID: 101
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
