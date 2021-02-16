using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000015 RID: 21
	public class x86RegisterOperand : Ix86Operand
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00004C3B File Offset: 0x00002E3B
		public x86RegisterOperand(x86Register reg)
		{
			this.Register = reg;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00004C4D File Offset: 0x00002E4D
		// (set) Token: 0x06000060 RID: 96 RVA: 0x00004C55 File Offset: 0x00002E55
		public x86Register Register
		{
			[CompilerGenerated]
			get
			{
				return this.<Register>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Register>k__BackingField = value;
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004C60 File Offset: 0x00002E60
		public override string ToString()
		{
			return this.Register.ToString();
		}

		// Token: 0x0400002E RID: 46
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private x86Register <Register>k__BackingField;
	}
}
