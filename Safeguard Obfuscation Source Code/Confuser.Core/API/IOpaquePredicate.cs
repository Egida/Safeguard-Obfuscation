using System;
using dnlib.DotNet.Emit;

namespace Confuser.Core.API
{
	// Token: 0x020000B4 RID: 180
	public interface IOpaquePredicate
	{
		// Token: 0x060003F6 RID: 1014
		Instruction[] Emit(Func<Instruction[]> loadArg);

		// Token: 0x060003F7 RID: 1015
		uint GetValue(uint[] arg);
	}
}
