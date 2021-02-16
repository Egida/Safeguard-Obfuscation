using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000D9 RID: 217
	internal class NormalPredicate : IPredicate
	{
		// Token: 0x06000358 RID: 856 RVA: 0x00005AC7 File Offset: 0x00003CC7
		public NormalPredicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00005AD8 File Offset: 0x00003CD8
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.xorKey));
			instrs.Add(Instruction.Create(OpCodes.Xor));
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00005B03 File Offset: 0x00003D03
		public int GetSwitchKey(int key)
		{
			return key ^ this.xorKey;
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0001B350 File Offset: 0x00019550
		public void Init(CilBody body)
		{
			bool flag = !this.inited;
			if (flag)
			{
				this.xorKey = this.ctx.Random.NextInt32();
				this.inited = true;
			}
		}

		// Token: 0x04000277 RID: 631
		private readonly CFContext ctx;

		// Token: 0x04000278 RID: 632
		private bool inited;

		// Token: 0x04000279 RID: 633
		private int xorKey;
	}
}
