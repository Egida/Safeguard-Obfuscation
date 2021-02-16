using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000B6 RID: 182
	internal abstract class BlockBase
	{
		// Token: 0x060002D2 RID: 722 RVA: 0x00005770 File Offset: 0x00003970
		public BlockBase(BlockType type)
		{
			this.Type = type;
		}

		// Token: 0x060002D3 RID: 723
		public abstract void ToBody(CilBody body);

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x00005782 File Offset: 0x00003982
		// (set) Token: 0x060002D5 RID: 725 RVA: 0x0000578A File Offset: 0x0000398A
		public ScopeBlock Parent
		{
			[CompilerGenerated]
			get
			{
				return this.<Parent>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Parent>k__BackingField = value;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x00005793 File Offset: 0x00003993
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x0000579B File Offset: 0x0000399B
		public BlockType Type
		{
			[CompilerGenerated]
			get
			{
				return this.<Type>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Type>k__BackingField = value;
			}
		}

		// Token: 0x0400020A RID: 522
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ScopeBlock <Parent>k__BackingField;

		// Token: 0x0400020B RID: 523
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BlockType <Type>k__BackingField;
	}
}
