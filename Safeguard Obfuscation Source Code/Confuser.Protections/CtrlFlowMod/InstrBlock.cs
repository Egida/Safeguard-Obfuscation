using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000BE RID: 190
	internal class InstrBlock : BlockBase
	{
		// Token: 0x060002EC RID: 748 RVA: 0x000057F8 File Offset: 0x000039F8
		public InstrBlock() : base(BlockType.Normal)
		{
			this.Instructions = new List<Instruction>();
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0001896C File Offset: 0x00016B6C
		public override void ToBody(CilBody body)
		{
			foreach (Instruction instruction in this.Instructions)
			{
				body.Instructions.Add(instruction);
			}
		}

		// Token: 0x060002EE RID: 750 RVA: 0x000189CC File Offset: 0x00016BCC
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			foreach (Instruction instruction in this.Instructions)
			{
				builder.AppendLine(instruction.ToString());
			}
			return builder.ToString();
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000580F File Offset: 0x00003A0F
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x00005817 File Offset: 0x00003A17
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

		// Token: 0x0400022A RID: 554
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<Instruction> <Instructions>k__BackingField;
	}
}
