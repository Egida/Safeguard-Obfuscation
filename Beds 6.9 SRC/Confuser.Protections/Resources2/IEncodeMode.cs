using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources2
{
	// Token: 0x0200004C RID: 76
	internal interface IEncodeMode
	{
		// Token: 0x06000160 RID: 352
		IEnumerable<Instruction> EmitDecrypt(MethodDef init, REContext ctx, Local block, Local key);

		// Token: 0x06000161 RID: 353
		uint[] Encrypt(uint[] data, int offset, uint[] key);
	}
}
