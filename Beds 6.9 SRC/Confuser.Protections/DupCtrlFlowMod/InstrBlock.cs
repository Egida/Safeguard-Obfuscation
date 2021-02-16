using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000D3 RID: 211
	internal class InstrBlock : BlockBase
	{
		// Token: 0x0600033D RID: 829 RVA: 0x00005A11 File Offset: 0x00003C11
		public InstrBlock() : base(BlockType.Normal)
		{
			this.Instructions = new List<Instruction>();
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0001AC00 File Offset: 0x00018E00
		public override void ToBody(CilBody body)
		{
			foreach (Instruction instruction in this.Instructions)
			{
				body.Instructions.Add(instruction);
			}
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0001AC60 File Offset: 0x00018E60
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			foreach (Instruction instruction in this.Instructions)
			{
				builder.AppendLine(instruction.ToString());
			}
			return builder.ToString();
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000340 RID: 832 RVA: 0x00005A28 File Offset: 0x00003C28
		// (set) Token: 0x06000341 RID: 833 RVA: 0x00005A30 File Offset: 0x00003C30
		public List<Instruction> Instructions
		{
			[CompilerGenerated]
			get
			{
				return this.<Instructions>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Instructions>k__BackingField = value;
			}
		}

		// Token: 0x0400026B RID: 619
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<Instruction> <Instructions>k__BackingField;
	}
}
