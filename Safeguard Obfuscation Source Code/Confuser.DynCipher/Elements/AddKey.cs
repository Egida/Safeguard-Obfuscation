using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x02000018 RID: 24
	internal class AddKey : CryptoElement
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00005584 File Offset: 0x00003784
		public AddKey(int index) : base(0)
		{
			this.Index = index;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00005597 File Offset: 0x00003797
		public override void Emit(CipherGenContext context)
		{
			this.EmitCore(context);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000055A4 File Offset: 0x000037A4
		private void EmitCore(CipherGenContext context)
		{
			Expression val = context.GetDataExpression(this.Index);
			context.Emit(new AssignmentStatement
			{
				Value = (val ^ context.GetKeyExpression(this.Index)),
				Target = val
			});
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00005597 File Offset: 0x00003797
		public override void EmitInverse(CipherGenContext context)
		{
			this.EmitCore(context);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000020D2 File Offset: 0x000002D2
		public override void Initialize(RandomGenerator random)
		{
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000073 RID: 115 RVA: 0x000055EC File Offset: 0x000037EC
		// (set) Token: 0x06000074 RID: 116 RVA: 0x000055F4 File Offset: 0x000037F4
		public int Index
		{
			[CompilerGenerated]
			get
			{
				return this.<Index>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Index>k__BackingField = value;
			}
		}

		// Token: 0x04000032 RID: 50
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int <Index>k__BackingField;
	}
}
