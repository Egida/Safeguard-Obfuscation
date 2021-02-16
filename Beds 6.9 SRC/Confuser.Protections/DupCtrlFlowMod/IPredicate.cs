using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000D4 RID: 212
	internal interface IPredicate
	{
		// Token: 0x06000342 RID: 834
		void EmitSwitchLoad(IList<Instruction> instrs);

		// Token: 0x06000343 RID: 835
		int GetSwitchKey(int key);

		// Token: 0x06000344 RID: 836
		void Init(CilBody body);
	}
}
