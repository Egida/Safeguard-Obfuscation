using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000016 RID: 22
	public class x86ImmediateOperand : Ix86Operand
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00004C86 File Offset: 0x00002E86
		public x86ImmediateOperand(int imm)
		{
			this.Immediate = imm;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00004C98 File Offset: 0x00002E98
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00004CA0 File Offset: 0x00002EA0
		public int Immediate
		{
			[CompilerGenerated]
			get
			{
				return this.<Immediate>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Immediate>k__BackingField = value;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004CAC File Offset: 0x00002EAC
		public override string ToString()
		{
			return this.Immediate.ToString("X") + "h";
		}

		// Token: 0x0400002F RID: 47
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int <Immediate>k__BackingField;
	}
}
