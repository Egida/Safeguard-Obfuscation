using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200002F RID: 47
	public interface IMildReferenceProxyService
	{
		// Token: 0x060000D7 RID: 215
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
