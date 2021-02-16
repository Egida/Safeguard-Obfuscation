using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x0200001E RID: 30
	internal class NumOp : CryptoElement
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00005FE5 File Offset: 0x000041E5
		public NumOp() : base(1)
		{
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00005FF0 File Offset: 0x000041F0
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00005FF8 File Offset: 0x000041F8
		public uint Key
		{
			[CompilerGenerated]
			get
			{
				return this.<Key>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Key>k__BackingField = value;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00006001 File Offset: 0x00004201
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00006009 File Offset: 0x00004209
		public uint InverseKey
		{
			[CompilerGenerated]
			get
			{
				return this.<InverseKey>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<InverseKey>k__BackingField = value;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00006012 File Offset: 0x00004212
		// (set) Token: 0x06000097 RID: 151 RVA: 0x0000601A File Offset: 0x0000421A
		public CryptoNumOps Operation
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

		// Token: 0x06000098 RID: 152 RVA: 0x00006024 File Offset: 0x00004224
		public override void Initialize(RandomGenerator random)
		{
			this.Operation = (CryptoNumOps)random.NextInt32(4);
			switch (this.Operation)
			{
			case CryptoNumOps.Add:
			case CryptoNumOps.Xor:
				this.Key = (this.InverseKey = random.NextUInt32());
				break;
			case CryptoNumOps.Mul:
				this.Key = (random.NextUInt32() | 1U);
				this.InverseKey = MathsUtils.modInv(this.Key);
				break;
			case CryptoNumOps.Xnor:
				this.Key = random.NextUInt32();
				this.InverseKey = ~this.Key;
				break;
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000060B8 File Offset: 0x000042B8
		public override void Emit(CipherGenContext context)
		{
			Expression val = context.GetDataExpression(base.DataIndexes[0]);
			switch (this.Operation)
			{
			case CryptoNumOps.Add:
				context.Emit(new AssignmentStatement
				{
					Value = val + this.Key,
					Target = val
				});
				break;
			case CryptoNumOps.Mul:
				context.Emit(new AssignmentStatement
				{
					Value = val * this.Key,
					Target = val
				});
				break;
			case CryptoNumOps.Xor:
				context.Emit(new AssignmentStatement
				{
					Value = (val ^ this.Key),
					Target = val
				});
				break;
			case CryptoNumOps.Xnor:
				context.Emit(new AssignmentStatement
				{
					Value = ~(val ^ this.Key),
					Target = val
				});
				break;
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000061B8 File Offset: 0x000043B8
		public override void EmitInverse(CipherGenContext context)
		{
			Expression val = context.GetDataExpression(base.DataIndexes[0]);
			switch (this.Operation)
			{
			case CryptoNumOps.Add:
				context.Emit(new AssignmentStatement
				{
					Value = val - this.InverseKey,
					Target = val
				});
				break;
			case CryptoNumOps.Mul:
				context.Emit(new AssignmentStatement
				{
					Value = val * this.InverseKey,
					Target = val
				});
				break;
			case CryptoNumOps.Xor:
				context.Emit(new AssignmentStatement
				{
					Value = (val ^ this.InverseKey),
					Target = val
				});
				break;
			case CryptoNumOps.Xnor:
				context.Emit(new AssignmentStatement
				{
					Value = (val ^ this.InverseKey),
					Target = val
				});
				break;
			}
		}

		// Token: 0x04000041 RID: 65
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint <Key>k__BackingField;

		// Token: 0x04000042 RID: 66
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint <InverseKey>k__BackingField;

		// Token: 0x04000043 RID: 67
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CryptoNumOps <Operation>k__BackingField;
	}
}
