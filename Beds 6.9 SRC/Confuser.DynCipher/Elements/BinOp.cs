using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x0200001A RID: 26
	internal class BinOp : CryptoElement
	{
		// Token: 0x06000075 RID: 117 RVA: 0x000055FD File Offset: 0x000037FD
		public BinOp() : base(2)
		{
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00005608 File Offset: 0x00003808
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00005610 File Offset: 0x00003810
		public CryptoBinOps Operation
		{
			[CompilerGenerated]
			get
			{
				return this.<Operation>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Operation>k__BackingField = value;
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00005619 File Offset: 0x00003819
		public override void Initialize(RandomGenerator random)
		{
			this.Operation = (CryptoBinOps)random.NextInt32(3);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x0000562C File Offset: 0x0000382C
		public override void Emit(CipherGenContext context)
		{
			Expression a = context.GetDataExpression(base.DataIndexes[0]);
			Expression b = context.GetDataExpression(base.DataIndexes[1]);
			switch (this.Operation)
			{
			case CryptoBinOps.Add:
				context.Emit(new AssignmentStatement
				{
					Value = a + b,
					Target = a
				});
				break;
			case CryptoBinOps.Xor:
				context.Emit(new AssignmentStatement
				{
					Value = (a ^ b),
					Target = a
				});
				break;
			case CryptoBinOps.Xnor:
				context.Emit(new AssignmentStatement
				{
					Value = ~(a ^ b),
					Target = a
				});
				break;
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000056E4 File Offset: 0x000038E4
		public override void EmitInverse(CipherGenContext context)
		{
			Expression a = context.GetDataExpression(base.DataIndexes[0]);
			Expression b = context.GetDataExpression(base.DataIndexes[1]);
			switch (this.Operation)
			{
			case CryptoBinOps.Add:
				context.Emit(new AssignmentStatement
				{
					Value = a - b,
					Target = a
				});
				break;
			case CryptoBinOps.Xor:
				context.Emit(new AssignmentStatement
				{
					Value = (a ^ b),
					Target = a
				});
				break;
			case CryptoBinOps.Xnor:
				context.Emit(new AssignmentStatement
				{
					Value = (a ^ ~b),
					Target = a
				});
				break;
			}
		}

		// Token: 0x04000037 RID: 55
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CryptoBinOps <Operation>k__BackingField;
	}
}
