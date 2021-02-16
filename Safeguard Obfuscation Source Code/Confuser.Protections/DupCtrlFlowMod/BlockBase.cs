using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000CB RID: 203
	internal abstract class BlockBase
	{
		// Token: 0x06000323 RID: 803 RVA: 0x00005990 File Offset: 0x00003B90
		public BlockBase(BlockType type)
		{
			this.Type = type;
		}

		// Token: 0x06000324 RID: 804
		public abstract void ToBody(CilBody body);

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000325 RID: 805 RVA: 0x000059A2 File Offset: 0x00003BA2
		// (set) Token: 0x06000326 RID: 806 RVA: 0x000059AA File Offset: 0x00003BAA
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

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000327 RID: 807 RVA: 0x000059B3 File Offset: 0x00003BB3
		// (set) Token: 0x06000328 RID: 808 RVA: 0x000059BB File Offset: 0x00003BBB
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

		// Token: 0x0400024B RID: 587
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ScopeBlock <Parent>k__BackingField;

		// Token: 0x0400024C RID: 588
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BlockType <Type>k__BackingField;
	}
}
