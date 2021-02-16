using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000EB RID: 235
	internal interface IPredicate
	{
		// Token: 0x0600039C RID: 924
		void Init(CilBody body);

		// Token: 0x0600039D RID: 925
		void EmitSwitchLoad(IList<Instruction> instrs);

		// Token: 0x0600039E RID: 926
		int GetSwitchKey(int key);
	}
}
