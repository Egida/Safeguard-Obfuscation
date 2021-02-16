using System;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	// Token: 0x020000B3 RID: 179
	public interface IOpaquePredicateDescriptor
	{
		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060003F2 RID: 1010
		OpaquePredicateType Type { get; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060003F3 RID: 1011
		int ArgumentCount { get; }

		// Token: 0x060003F4 RID: 1012
		bool IsUsable(MethodDef method);

		// Token: 0x060003F5 RID: 1013
		IOpaquePredicate CreatePredicate(MethodDef method);
	}
}
