using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000C6 RID: 198
	internal class ScopeBlock : BlockBase
	{
		// Token: 0x0600030B RID: 779 RVA: 0x000058F4 File Offset: 0x00003AF4
		public ScopeBlock(BlockType type, ExceptionHandler handler) : base(type)
		{
			this.Handler = handler;
			this.Children = new List<BlockBase>();
		}

		// Token: 0x0600030C RID: 780 RVA: 0x000190F8 File Offset: 0x000172F8
		public Instruction GetFirstInstr()
		{
			BlockBase base2 = this.Children.First<BlockBase>();
			bool flag = base2 is ScopeBlock;
			Instruction result;
			if (flag)
			{
				result = ((ScopeBlock)base2).GetFirstInstr();
			}
			else
			{
				result = ((InstrBlock)base2).Instructions.First<Instruction>();
			}
			return result;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00019144 File Offset: 0x00017344
		public Instruction GetLastInstr()
		{
			BlockBase base2 = this.Children.Last<BlockBase>();
			bool flag = base2 is ScopeBlock;
			Instruction result;
			if (flag)
			{
				result = ((ScopeBlock)base2).GetLastInstr();
			}
			else
			{
				result = ((InstrBlock)base2).Instructions.Last<Instruction>();
			}
			return result;
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00019190 File Offset: 0x00017390
		public override void ToBody(CilBody body)
		{
			bool flag = base.Type > BlockType.Normal;
			if (flag)
			{
				bool flag2 = base.Type == BlockType.Try;
				if (flag2)
				{
					this.Handler.TryStart = this.GetFirstInstr();
					this.Handler.TryEnd = this.GetLastInstr();
				}
				else
				{
					bool flag3 = base.Type == BlockType.Filter;
					if (flag3)
					{
						this.Handler.FilterStart = this.GetFirstInstr();
					}
					else
					{
						this.Handler.HandlerStart = this.GetFirstInstr();
						this.Handler.HandlerEnd = this.GetLastInstr();
					}
				}
			}
			foreach (BlockBase base2 in this.Children)
			{
				base2.ToBody(body);
			}
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00019274 File Offset: 0x00017474
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			bool flag = base.Type == BlockType.Try;
			if (flag)
			{
				builder.Append("try ");
			}
			else
			{
				bool flag2 = base.Type == BlockType.Handler;
				if (flag2)
				{
					builder.Append("handler ");
				}
				else
				{
					bool flag3 = base.Type == BlockType.Finally;
					if (flag3)
					{
						builder.Append("finally ");
					}
					else
					{
						bool flag4 = base.Type == BlockType.Fault;
						if (flag4)
						{
							builder.Append("fault ");
						}
					}
				}
			}
			builder.AppendLine("{");
			foreach (BlockBase base2 in this.Children)
			{
				builder.Append(base2);
			}
			builder.AppendLine("}");
			return builder.ToString();
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000310 RID: 784 RVA: 0x00005913 File Offset: 0x00003B13
		// (set) Token: 0x06000311 RID: 785 RVA: 0x0000591B File Offset: 0x00003B1B
		public List<BlockBase> Children
		{
			[CompilerGenerated]
			get
			{
				return this.<Children>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Children>k__BackingField = value;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000312 RID: 786 RVA: 0x00005924 File Offset: 0x00003B24
		// (set) Token: 0x06000313 RID: 787 RVA: 0x0000592C File Offset: 0x00003B2C
		public ExceptionHandler Handler
		{
			[CompilerGenerated]
			get
			{
				return this.<Handler>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Handler>k__BackingField = value;
			}
		}

		// Token: 0x0400023D RID: 573
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<BlockBase> <Children>k__BackingField;

		// Token: 0x0400023E RID: 574
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ExceptionHandler <Handler>k__BackingField;
	}
}
