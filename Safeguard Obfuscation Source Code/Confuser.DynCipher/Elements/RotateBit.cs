using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x0200001F RID: 31
	internal class RotateBit : CryptoElement
	{
		// Token: 0x0600009B RID: 155 RVA: 0x00005FE5 File Offset: 0x000041E5
		public RotateBit() : base(1)
		{
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000062B4 File Offset: 0x000044B4
		public override void Emit(CipherGenContext context)
		{
			Expression val = context.GetDataExpression(base.DataIndexes[0]);
			VariableExpression tmp;
			using (context.AcquireTempVar(out tmp))
			{
				bool isAlternate = this.IsAlternate;
				if (isAlternate)
				{
					context.Emit(new AssignmentStatement
					{
						Value = val >> 32 - this.Bits,
						Target = tmp
					}).Emit(new AssignmentStatement
					{
						Value = (val << this.Bits | tmp),
						Target = val
					});
				}
				else
				{
					context.Emit(new AssignmentStatement
					{
						Value = val << 32 - this.Bits,
						Target = tmp
					}).Emit(new AssignmentStatement
					{
						Value = (val >> this.Bits | tmp),
						Target = val
					});
				}
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000063B4 File Offset: 0x000045B4
		public override void EmitInverse(CipherGenContext context)
		{
			Expression val = context.GetDataExpression(base.DataIndexes[0]);
			VariableExpression tmp;
			using (context.AcquireTempVar(out tmp))
			{
				bool isAlternate = this.IsAlternate;
				if (isAlternate)
				{
					context.Emit(new AssignmentStatement
					{
						Value = val << 32 - this.Bits,
						Target = tmp
					}).Emit(new AssignmentStatement
					{
						Value = (val >> this.Bits | tmp),
						Target = val
					});
				}
				else
				{
					context.Emit(new AssignmentStatement
					{
						Value = val >> 32 - this.Bits,
						Target = tmp
					}).Emit(new AssignmentStatement
					{
						Value = (val << this.Bits | tmp),
						Target = val
					});
				}
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000064B4 File Offset: 0x000046B4
		public override void Initialize(RandomGenerator random)
		{
			this.Bits = random.NextInt32(1, 32);
			this.IsAlternate = (random.NextInt32() % 2 == 0);
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600009F RID: 159 RVA: 0x000064D9 File Offset: 0x000046D9
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x000064E1 File Offset: 0x000046E1
		public int Bits
		{
			[CompilerGenerated]
			get
			{
				return this.<Bits>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Bits>k__BackingField = value;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x000064EA File Offset: 0x000046EA
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x000064F2 File Offset: 0x000046F2
		public bool IsAlternate
		{
			[CompilerGenerated]
			get
			{
				return this.<IsAlternate>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<IsAlternate>k__BackingField = value;
			}
		}

		// Token: 0x04000044 RID: 68
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int <Bits>k__BackingField;

		// Token: 0x04000045 RID: 69
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool <IsAlternate>k__BackingField;
	}
}
