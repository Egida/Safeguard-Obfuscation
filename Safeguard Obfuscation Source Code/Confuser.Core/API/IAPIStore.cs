using System;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	// Token: 0x020000B0 RID: 176
	public interface IAPIStore
	{
		// Token: 0x060003E9 RID: 1001
		void AddStore(IDataStore dataStore);

		// Token: 0x060003EA RID: 1002
		IDataStore GetStore(MethodDef method);

		// Token: 0x060003EB RID: 1003
		void AddPredicate(IOpaquePredicateDescriptor predicate);

		// Token: 0x060003EC RID: 1004
		IOpaquePredicateDescriptor GetPredicate(MethodDef method, OpaquePredicateType? type, params int[] argCount);
	}
}
