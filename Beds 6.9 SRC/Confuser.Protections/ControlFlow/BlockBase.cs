using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000E1 RID: 225
	internal abstract class BlockBase
	{
		// Token: 0x06000375 RID: 885 RVA: 0x00005BA9 File Offset: 0x00003DA9
		public BlockBase(BlockType type)
		{
			this.Type = type;
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000376 RID: 886 RVA: 0x00005BBB File Offset: 0x00003DBB
		// (set) Token: 0x06000377 RID: 887 RVA: 0x00005BC3 File Offset: 0x00003DC3
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

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000378 RID: 888 RVA: 0x00005BCC File Offset: 0x00003DCC
		// (set) Token: 0x06000379 RID: 889 RVA: 0x00005BD4 File Offset: 0x00003DD4
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

		// Token: 0x0600037A RID: 890
		public abstract void ToBody(CilBody body);

		// Token: 0x0400028C RID: 652
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ScopeBlock <Parent>k__BackingField;

		// Token: 0x0400028D RID: 653
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BlockType <Type>k__BackingField;
	}
}
