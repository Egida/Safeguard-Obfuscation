using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x02000020 RID: 32
	internal class Swap : CryptoElement
	{
		// Token: 0x060000A3 RID: 163 RVA: 0x000055FD File Offset: 0x000037FD
		public Swap() : base(2)
		{
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x000064FB File Offset: 0x000046FB
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x00006503 File Offset: 0x00004703
		public uint Mask
		{
			[CompilerGenerated]
			get
			{
				return this.<Mask>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Mask>k__BackingField = value;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x0000650C File Offset: 0x0000470C
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x00006514 File Offset: 0x00004714
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

		// Token: 0x060000A8 RID: 168 RVA: 0x00006520 File Offset: 0x00004720
		public override void Initialize(RandomGenerator random)
		{
			bool flag = random.NextInt32(3) == 0;
			if (flag)
			{
				this.Mask = uint.MaxValue;
			}
			else
			{
				this.Mask = random.NextUInt32();
			}
			this.Key = (random.NextUInt32() | 1U);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006564 File Offset: 0x00004764
		private void EmitCore(CipherGenContext context)
		{
			Expression a = context.GetDataExpression(base.DataIndexes[0]);
			Expression b = context.GetDataExpression(base.DataIndexes[1]);
			bool flag = this.Mask == uint.MaxValue;
			if (flag)
			{
				VariableExpression tmp;
				using (context.AcquireTempVar(out tmp))
				{
					context.Emit(new AssignmentStatement
					{
						Value = a * this.Key,
						Target = tmp
					}).Emit(new AssignmentStatement
					{
						Value = b,
						Target = a
					}).Emit(new AssignmentStatement
					{
						Value = tmp * MathsUtils.modInv(this.Key),
						Target = b
					});
				}
			}
			else
			{
				LiteralExpression mask = this.Mask;
				LiteralExpression notMask = ~this.Mask;
				VariableExpression tmp;
				using (context.AcquireTempVar(out tmp))
				{
					context.Emit(new AssignmentStatement
					{
						Value = (a & mask) * this.Key,
						Target = tmp
					}).Emit(new AssignmentStatement
					{
						Value = ((a & notMask) | (b & mask)),
						Target = a
					}).Emit(new AssignmentStatement
					{
						Value = ((b & notMask) | tmp * MathsUtils.modInv(this.Key)),
						Target = b
					});
				}
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006728 File Offset: 0x00004928
		public override void Emit(CipherGenContext context)
		{
			this.EmitCore(context);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006728 File Offset: 0x00004928
		public override void EmitInverse(CipherGenContext context)
		{
			this.EmitCore(context);
		}

		// Token: 0x04000046 RID: 70
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint <Mask>k__BackingField;

		// Token: 0x04000047 RID: 71
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint <Key>k__BackingField;
	}
}
