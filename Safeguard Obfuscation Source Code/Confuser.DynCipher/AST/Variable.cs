using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.AST
{
	// Token: 0x0200002C RID: 44
	public class Variable
	{
		// Token: 0x060000E7 RID: 231 RVA: 0x00006CAA File Offset: 0x00004EAA
		public Variable(string name)
		{
			this.Name = name;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00006CBC File Offset: 0x00004EBC
		public override string ToString()
		{
			return this.Name;
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00006CD4 File Offset: 0x00004ED4
		// (set) Token: 0x060000EA RID: 234 RVA: 0x00006CDC File Offset: 0x00004EDC
		public string Name
		{
			[CompilerGenerated]
			get
			{
				return this.<Name>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Name>k__BackingField = value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000EB RID: 235 RVA: 0x00006CE5 File Offset: 0x00004EE5
		// (set) Token: 0x060000EC RID: 236 RVA: 0x00006CED File Offset: 0x00004EED
		public object Tag
		{
			[CompilerGenerated]
			get
			{
				return this.<Tag>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Tag>k__BackingField = value;
			}
		}

		// Token: 0x04000064 RID: 100
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Name>k__BackingField;

		// Token: 0x04000065 RID: 101
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object <Tag>k__BackingField;
	}
}
