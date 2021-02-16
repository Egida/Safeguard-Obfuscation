using System;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	// Token: 0x020000B1 RID: 177
	public interface IDataStore
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060003ED RID: 1005
		int Priority { get; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060003EE RID: 1006
		int KeyCount { get; }

		// Token: 0x060003EF RID: 1007
		bool IsUsable(MethodDef method);

		// Token: 0x060003F0 RID: 1008
		IDataStoreAccessor CreateAccessor(MethodDef method, uint[] keys, byte[] data);
	}
}
