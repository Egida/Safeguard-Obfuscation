using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000037 RID: 55
	public interface IReferenceProxyService
	{
		// Token: 0x0600010B RID: 267
		void ExcludeMethod(ConfuserContext context, MethodDef method);

		// Token: 0x0600010C RID: 268
		void ExcludeTarget(ConfuserContext context, MethodDef method);

		// Token: 0x0600010D RID: 269
		bool IsTargeted(ConfuserContext context, MethodDef method);
	}
}
