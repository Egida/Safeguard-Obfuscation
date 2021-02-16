using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000F0 RID: 240
	internal class NormalPredicate : IPredicate
	{
		// Token: 0x060003B2 RID: 946 RVA: 0x00005D13 File Offset: 0x00003F13
		public NormalPredicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0001E0C8 File Offset: 0x0001C2C8
		public void Init(CilBody body)
		{
			bool flag = this.inited;
			if (!flag)
			{
				this.xorKey = this.ctx.Random.NextInt32();
				this.inited = true;
			}
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x00005D24 File Offset: 0x00003F24
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.xorKey));
			instrs.Add(Instruction.Create(OpCodes.Xor));
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0001E100 File Offset: 0x0001C300
		public int GetSwitchKey(int key)
		{
			return key ^ this.xorKey;
		}

		// Token: 0x040002BE RID: 702
		private readonly CFContext ctx;

		// Token: 0x040002BF RID: 703
		private bool inited;

		// Token: 0x040002C0 RID: 704
		private int xorKey;
	}
}
