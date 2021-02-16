using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000BF RID: 191
	internal interface IPredicate
	{
		// Token: 0x060002F1 RID: 753
		void EmitSwitchLoad(IList<Instruction> instrs);

		// Token: 0x060002F2 RID: 754
		int GetSwitchKey(int key);

		// Token: 0x060002F3 RID: 755
		void Init(CilBody body);
	}
}
