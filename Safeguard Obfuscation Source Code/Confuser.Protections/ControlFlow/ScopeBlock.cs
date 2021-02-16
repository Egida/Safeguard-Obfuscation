using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000E3 RID: 227
	internal class ScopeBlock : BlockBase
	{
		// Token: 0x0600037B RID: 891 RVA: 0x00005BDD File Offset: 0x00003DDD
		public ScopeBlock(BlockType type, ExceptionHandler handler) : base(type)
		{
			this.Handler = handler;
			this.Children = new List<BlockBase>();
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600037C RID: 892 RVA: 0x00005BFC File Offset: 0x00003DFC
		// (set) Token: 0x0600037D RID: 893 RVA: 0x00005C04 File Offset: 0x00003E04
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

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600037E RID: 894 RVA: 0x00005C0D File Offset: 0x00003E0D
		// (set) Token: 0x0600037F RID: 895 RVA: 0x00005C15 File Offset: 0x00003E15
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

		// Token: 0x06000380 RID: 896 RVA: 0x0001C508 File Offset: 0x0001A708
		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			bool flag = base.Type == BlockType.Try;
			if (flag)
			{
				ret.Append("try ");
			}
			else
			{
				bool flag2 = base.Type == BlockType.Handler;
				if (flag2)
				{
					ret.Append("handler ");
				}
				else
				{
					bool flag3 = base.Type == BlockType.Finally;
					if (flag3)
					{
						ret.Append("finally ");
					}
					else
					{
						bool flag4 = base.Type == BlockType.Fault;
						if (flag4)
						{
							ret.Append("fault ");
						}
					}
				}
			}
			ret.AppendLine("{");
			foreach (BlockBase child in this.Children)
			{
				ret.Append(child);
			}
			ret.AppendLine("}");
			return ret.ToString();
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0001C5F8 File Offset: 0x0001A7F8
		public Instruction GetFirstInstr()
		{
			BlockBase firstBlock = this.Children.First<BlockBase>();
			bool flag = firstBlock is ScopeBlock;
			Instruction result;
			if (flag)
			{
				result = ((ScopeBlock)firstBlock).GetFirstInstr();
			}
			else
			{
				result = ((InstrBlock)firstBlock).Instructions.First<Instruction>();
			}
			return result;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0001C644 File Offset: 0x0001A844
		public Instruction GetLastInstr()
		{
			BlockBase firstBlock = this.Children.Last<BlockBase>();
			bool flag = firstBlock is ScopeBlock;
			Instruction result;
			if (flag)
			{
				result = ((ScopeBlock)firstBlock).GetLastInstr();
			}
			else
			{
				result = ((InstrBlock)firstBlock).Instructions.Last<Instruction>();
			}
			return result;
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0001C690 File Offset: 0x0001A890
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
			foreach (BlockBase block in this.Children)
			{
				block.ToBody(body);
			}
		}

		// Token: 0x04000295 RID: 661
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ExceptionHandler <Handler>k__BackingField;

		// Token: 0x04000296 RID: 662
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<BlockBase> <Children>k__BackingField;
	}
}
