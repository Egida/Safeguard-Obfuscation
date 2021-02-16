using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000026 RID: 38
	public class LiteralExpression : Expression
	{
		// Token: 0x060000CE RID: 206 RVA: 0x00006AA8 File Offset: 0x00004CA8
		public static implicit operator LiteralExpression(uint val)
		{
			return new LiteralExpression
			{
				Value = val
			};
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00006AC8 File Offset: 0x00004CC8
		public override string ToString()
		{
			return this.Value.ToString("x8") + "h";
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00006AF7 File Offset: 0x00004CF7
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x00006AFF File Offset: 0x00004CFF
		public uint Value
		{
			[CompilerGenerated]
			get
			{
				return this.<Value>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Value>k__BackingField = value;
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00006783 File Offset: 0x00004983
		public LiteralExpression()
		{
		}

		// Token: 0x0400005A RID: 90
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint <Value>k__BackingField;
	}
}
