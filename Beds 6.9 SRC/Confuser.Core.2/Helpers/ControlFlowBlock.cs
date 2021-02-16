using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers
{
	/// <summary>
	///     A block in Control Flow Graph (CFG).
	/// </summary>
	// Token: 0x020000A5 RID: 165
	public class ControlFlowBlock
	{
		// Token: 0x060003B4 RID: 948 RVA: 0x000038C2 File Offset: 0x00001AC2
		internal ControlFlowBlock(int id, ControlFlowBlockType type, Instruction header, Instruction footer)
		{
			this.Id = id;
			this.Type = type;
			this.Header = header;
			this.Footer = footer;
			this.Sources = new List<ControlFlowBlock>();
			this.Targets = new List<ControlFlowBlock>();
		}

		/// <summary>
		///     Returns a <see cref="T:System.String" /> that represents this block.
		/// </summary>
		/// <returns>A <see cref="T:System.String" /> that represents this block.</returns>
		// Token: 0x060003B5 RID: 949 RVA: 0x00015DC8 File Offset: 0x00013FC8
		public override string ToString()
		{
			return string.Format("Block {0} => {1} {2}", this.Id, this.Type, string.Join(", ", (from block in this.Targets
			select block.Id.ToString()).ToArray<string>()));
		}

		/// <summary>
		///     Gets the source blocks of this control flow block.
		/// </summary>
		/// <value>The source blocks.</value>
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x00003901 File Offset: 0x00001B01
		// (set) Token: 0x060003B7 RID: 951 RVA: 0x00003909 File Offset: 0x00001B09
		public IList<ControlFlowBlock> Sources
		{
			[CompilerGenerated]
			get
			{
				return this.<Sources>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Sources>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the target blocks of this control flow block.
		/// </summary>
		/// <value>The target blocks.</value>
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x00003912 File Offset: 0x00001B12
		// (set) Token: 0x060003B9 RID: 953 RVA: 0x0000391A File Offset: 0x00001B1A
		public IList<ControlFlowBlock> Targets
		{
			[CompilerGenerated]
			get
			{
				return this.<Targets>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Targets>k__BackingField = value;
			}
		}

		// Token: 0x04000276 RID: 630
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<ControlFlowBlock> <Sources>k__BackingField;

		// Token: 0x04000277 RID: 631
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<ControlFlowBlock> <Targets>k__BackingField;

		/// <summary>
		///     The footer instruction
		/// </summary>
		// Token: 0x04000278 RID: 632
		public readonly Instruction Footer;

		/// <summary>
		///     The header instruction
		/// </summary>
		// Token: 0x04000279 RID: 633
		public readonly Instruction Header;

		/// <summary>
		///     The identifier of this block
		/// </summary>
		// Token: 0x0400027A RID: 634
		public readonly int Id;

		/// <summary>
		///     The type of this block
		/// </summary>
		// Token: 0x0400027B RID: 635
		public readonly ControlFlowBlockType Type;

		// Token: 0x020000A6 RID: 166
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060003BA RID: 954 RVA: 0x00003923 File Offset: 0x00001B23
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060003BB RID: 955 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x060003BC RID: 956 RVA: 0x00015E34 File Offset: 0x00014034
			internal string <ToString>b__1_0(ControlFlowBlock block)
			{
				return block.Id.ToString();
			}

			// Token: 0x0400027C RID: 636
			public static readonly ControlFlowBlock.<>c <>9 = new ControlFlowBlock.<>c();

			// Token: 0x0400027D RID: 637
			public static Func<ControlFlowBlock, string> <>9__1_0;
		}
	}
}
