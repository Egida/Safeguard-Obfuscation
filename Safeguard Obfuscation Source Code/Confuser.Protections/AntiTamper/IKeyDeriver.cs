using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000A7 RID: 167
	internal interface IKeyDeriver
	{
		// Token: 0x06000280 RID: 640
		void Init(ConfuserContext ctx, RandomGenerator random);

		// Token: 0x06000281 RID: 641
		uint[] DeriveKey(uint[] a, uint[] b);

		// Token: 0x06000282 RID: 642
		IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src);
	}
}
