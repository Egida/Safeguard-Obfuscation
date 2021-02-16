using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources
{
	// Token: 0x0200003F RID: 63
	internal interface IEncodeMode
	{
		// Token: 0x0600013B RID: 315
		IEnumerable<Instruction> EmitDecrypt(MethodDef init, REContext ctx, Local block, Local key);

		// Token: 0x0600013C RID: 316
		uint[] Encrypt(uint[] data, int offset, uint[] key);
	}
}
