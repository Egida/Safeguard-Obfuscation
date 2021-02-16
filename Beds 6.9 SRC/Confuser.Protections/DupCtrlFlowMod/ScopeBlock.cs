using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000DB RID: 219
	internal class ScopeBlock : BlockBase
	{
		// Token: 0x0600035C RID: 860 RVA: 0x00005B0D File Offset: 0x00003D0D
		public ScopeBlock(BlockType type, ExceptionHandler handler) : base(type)
		{
			this.Handler = handler;
			this.Children = new List<BlockBase>();
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0001B38C File Offset: 0x0001958C
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

		// Token: 0x0600035E RID: 862 RVA: 0x0001B3D8 File Offset: 0x000195D8
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

		// Token: 0x0600035F RID: 863 RVA: 0x0001B424 File Offset: 0x00019624
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

		// Token: 0x06000360 RID: 864 RVA: 0x0001B508 File Offset: 0x00019708
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

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000361 RID: 865 RVA: 0x00005B2C File Offset: 0x00003D2C
		// (set) Token: 0x06000362 RID: 866 RVA: 0x00005B34 File Offset: 0x00003D34
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

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000363 RID: 867 RVA: 0x00005B3D File Offset: 0x00003D3D
		// (set) Token: 0x06000364 RID: 868 RVA: 0x00005B45 File Offset: 0x00003D45
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

		// Token: 0x0400027E RID: 638
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<BlockBase> <Children>k__BackingField;

		// Token: 0x0400027F RID: 639
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ExceptionHandler <Handler>k__BackingField;
	}
}
