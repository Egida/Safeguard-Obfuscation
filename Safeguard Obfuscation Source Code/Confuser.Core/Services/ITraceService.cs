using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x02000080 RID: 128
	public interface ITraceService
	{
		// Token: 0x060002E5 RID: 741
		MethodTrace Trace(MethodDef method);
	}
}
