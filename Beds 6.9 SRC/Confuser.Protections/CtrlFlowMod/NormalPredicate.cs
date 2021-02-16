using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000C4 RID: 196
	internal class NormalPredicate : IPredicate
	{
		// Token: 0x06000307 RID: 775 RVA: 0x000058AE File Offset: 0x00003AAE
		public NormalPredicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000308 RID: 776 RVA: 0x000058BF File Offset: 0x00003ABF
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.xorKey));
			instrs.Add(Instruction.Create(OpCodes.Xor));
		}

		// Token: 0x06000309 RID: 777 RVA: 0x000058EA File Offset: 0x00003AEA
		public int GetSwitchKey(int key)
		{
			return key ^ this.xorKey;
		}

		// Token: 0x0600030A RID: 778 RVA: 0x000190BC File Offset: 0x000172BC
		public void Init(CilBody body)
		{
			bool flag = !this.inited;
			if (flag)
			{
				this.xorKey = this.ctx.Random.NextInt32();
				this.inited = true;
			}
		}

		// Token: 0x04000236 RID: 566
		private readonly CFContext ctx;

		// Token: 0x04000237 RID: 567
		private bool inited;

		// Token: 0x04000238 RID: 568
		private int xorKey;
	}
}
