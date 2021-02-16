using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200001E RID: 30
	public interface IAntiTamperService
	{
		// Token: 0x0600008E RID: 142
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
