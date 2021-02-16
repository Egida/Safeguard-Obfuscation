using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.MildReferenceProxy
{
	// Token: 0x02000073 RID: 115
	internal interface IRPEncoding
	{
		// Token: 0x060001D7 RID: 471
		Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg);

		// Token: 0x060001D8 RID: 472
		int Encode(MethodDef init, RPContext ctx, int value);
	}
}
