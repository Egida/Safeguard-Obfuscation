using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x0200009C RID: 156
	internal interface IEncodeMode
	{
		// Token: 0x0600025E RID: 606
		IEnumerable<Instruction> EmitDecrypt(MethodDef init, CEContext ctx, Local block, Local key);

		// Token: 0x0600025F RID: 607
		uint[] Encrypt(uint[] data, int offset, uint[] key);

		// Token: 0x06000260 RID: 608
		object CreateDecoder(MethodDef decoder, CEContext ctx);

		// Token: 0x06000261 RID: 609
		uint Encode(object data, CEContext ctx, uint id);
	}
}
