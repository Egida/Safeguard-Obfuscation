using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x0200005E RID: 94
	internal interface IRPEncoding
	{
		// Token: 0x06000199 RID: 409
		Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg);

		// Token: 0x0600019A RID: 410
		int Encode(MethodDef init, RPContext ctx, int value);
	}
}
