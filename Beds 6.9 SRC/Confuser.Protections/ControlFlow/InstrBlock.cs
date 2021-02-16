using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000E4 RID: 228
	internal class InstrBlock : BlockBase
	{
		// Token: 0x06000384 RID: 900 RVA: 0x00005C1E File Offset: 0x00003E1E
		public InstrBlock() : base(BlockType.Normal)
		{
			this.Instructions = new List<Instruction>();
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000385 RID: 901 RVA: 0x00005C35 File Offset: 0x00003E35
		// (set) Token: 0x06000386 RID: 902 RVA: 0x00005C3D File Offset: 0x00003E3D
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

		// Token: 0x06000387 RID: 903 RVA: 0x0001C774 File Offset: 0x0001A974
		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			foreach (Instruction instr in this.Instructions)
			{
				ret.AppendLine(instr.ToString());
			}
			return ret.ToString();
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0001C7E0 File Offset: 0x0001A9E0
		public override void ToBody(CilBody body)
		{
			foreach (Instruction instr in this.Instructions)
			{
				body.Instructions.Add(instr);
			}
		}

		// Token: 0x04000297 RID: 663
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<Instruction> <Instructions>k__BackingField;
	}
}
