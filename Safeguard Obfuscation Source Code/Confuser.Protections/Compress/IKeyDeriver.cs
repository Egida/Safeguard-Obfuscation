using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Compress
{
	// Token: 0x02000101 RID: 257
	internal interface IKeyDeriver
	{
		// Token: 0x060003E6 RID: 998
		void Init(ConfuserContext ctx, RandomGenerator random);

		// Token: 0x060003E7 RID: 999
		uint[] DeriveKey(uint[] a, uint[] b);

		// Token: 0x060003E8 RID: 1000
		IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src);
	}
}
