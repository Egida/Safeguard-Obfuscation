using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000023 RID: 35
	public interface IConstantService
	{
		// Token: 0x060000A1 RID: 161
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
