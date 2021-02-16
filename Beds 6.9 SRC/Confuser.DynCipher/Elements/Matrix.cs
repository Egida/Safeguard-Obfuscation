using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x0200001C RID: 28
	internal class Matrix : CryptoElement
	{
		// Token: 0x06000083 RID: 131 RVA: 0x000057DD File Offset: 0x000039DD
		public Matrix() : base(4)
		{
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000057E8 File Offset: 0x000039E8
		private static uint cofactor4(uint[,] mat, int i, int j)
		{
			uint[,] sub = new uint[3, 3];
			int ci = 0;
			int si = 0;
			while (ci < 4)
			{
				bool flag = ci == i;
				if (flag)
				{
					si--;
				}
				else
				{
					int cj = 0;
					int sj = 0;
					while (cj < 4)
					{
						bool flag2 = cj == j;
						if (flag2)
						{
							sj--;
						}
						else
						{
							sub[si, sj] = mat[ci, cj];
						}
						cj++;
						sj++;
					}
				}
				ci++;
				si++;
			}
			uint ret = Matrix.det3(sub);
			bool flag3 = (i + j) % 2 == 0;
			uint result;
			if (flag3)
			{
				result = ret;
			}
			else
			{
				result = (uint)(-(uint)((ulong)((uint)((ulong)ret))));
			}
			return result;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000058A0 File Offset: 0x00003AA0
		private static uint det3(uint[,] mat)
		{
			return mat[0, 0] * mat[1, 1] * mat[2, 2] + mat[0, 1] * mat[1, 2] * mat[2, 0] + mat[0, 2] * mat[1, 0] * mat[2, 1] - mat[0, 2] * mat[1, 1] * mat[2, 0] - mat[0, 1] * mat[1, 0] * mat[2, 2] - mat[0, 0] * mat[1, 2] * mat[2, 1];
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00005953 File Offset: 0x00003B53
		public override void Emit(CipherGenContext context)
		{
			this.EmitCore(context, this.Key);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005964 File Offset: 0x00003B64
		private void EmitCore(CipherGenContext context, uint[,] k)
		{
			Expression a = context.GetDataExpression(base.DataIndexes[0]);
			Expression b = context.GetDataExpression(base.DataIndexes[1]);
			Expression c = context.GetDataExpression(base.DataIndexes[2]);
			Expression d = context.GetDataExpression(base.DataIndexes[3]);
			Func<uint, LiteralExpression> i = (uint v) => v;
			VariableExpression ta;
			using (context.AcquireTempVar(out ta))
			{
				VariableExpression tb;
				using (context.AcquireTempVar(out tb))
				{
					VariableExpression tc;
					using (context.AcquireTempVar(out tc))
					{
						VariableExpression td;
						using (context.AcquireTempVar(out td))
						{
							context.Emit(new AssignmentStatement
							{
								Value = a * i(k[0, 0]) + b * i(k[0, 1]) + c * i(k[0, 2]) + d * i(k[0, 3]),
								Target = ta
							}).Emit(new AssignmentStatement
							{
								Value = a * i(k[1, 0]) + b * i(k[1, 1]) + c * i(k[1, 2]) + d * i(k[1, 3]),
								Target = tb
							}).Emit(new AssignmentStatement
							{
								Value = a * i(k[2, 0]) + b * i(k[2, 1]) + c * i(k[2, 2]) + d * i(k[2, 3]),
								Target = tc
							}).Emit(new AssignmentStatement
							{
								Value = a * i(k[3, 0]) + b * i(k[3, 1]) + c * i(k[3, 2]) + d * i(k[3, 3]),
								Target = td
							}).Emit(new AssignmentStatement
							{
								Value = ta,
								Target = a
							}).Emit(new AssignmentStatement
							{
								Value = tb,
								Target = b
							}).Emit(new AssignmentStatement
							{
								Value = tc,
								Target = c
							}).Emit(new AssignmentStatement
							{
								Value = td,
								Target = d
							});
						}
					}
				}
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00005D04 File Offset: 0x00003F04
		public override void EmitInverse(CipherGenContext context)
		{
			this.EmitCore(context, this.InverseKey);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005D18 File Offset: 0x00003F18
		private static uint[,] GenerateUnimodularMatrix(RandomGenerator random)
		{
			Func<uint> next = () => (uint)random.NextInt32(4);
			uint[,] array = new uint[,]
			{
				{
					1U,
					0U,
					0U,
					0U
				},
				{
					0U,
					1U,
					0U,
					0U
				},
				{
					0U,
					0U,
					1U,
					0U
				},
				{
					0U,
					0U,
					0U,
					1U
				}
			};
			array[1, 0] = next();
			array[2, 0] = next();
			array[2, 1] = next();
			array[3, 0] = next();
			array[3, 1] = next();
			array[3, 2] = next();
			uint[,] i = array;
			uint[,] array2 = new uint[,]
			{
				{
					1U,
					0U,
					0U,
					0U
				},
				{
					0U,
					1U,
					0U,
					0U
				},
				{
					0U,
					0U,
					1U,
					0U
				},
				{
					0U,
					0U,
					0U,
					1U
				}
			};
			array2[0, 1] = next();
			array2[0, 2] = next();
			array2[0, 3] = next();
			array2[1, 2] = next();
			array2[1, 3] = next();
			array2[2, 3] = next();
			uint[,] u = array2;
			return Matrix.mul(i, u);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005E2C File Offset: 0x0000402C
		public override void Initialize(RandomGenerator random)
		{
			this.InverseKey = Matrix.mul(Matrix.transpose4(Matrix.GenerateUnimodularMatrix(random)), Matrix.GenerateUnimodularMatrix(random));
			uint[,] cof = new uint[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					cof[i, j] = Matrix.cofactor4(this.InverseKey, i, j);
				}
			}
			this.Key = Matrix.transpose4(cof);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005EA8 File Offset: 0x000040A8
		private static uint[,] mul(uint[,] a, uint[,] b)
		{
			int i = a.GetLength(0);
			int p = b.GetLength(1);
			int j = a.GetLength(1);
			bool flag = b.GetLength(0) != j;
			uint[,] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				uint[,] ret = new uint[i, p];
				for (int k = 0; k < i; k++)
				{
					for (int l = 0; l < p; l++)
					{
						ret[k, l] = 0U;
						for (int m = 0; m < j; m++)
						{
							ret[k, l] += a[k, m] * b[m, l];
						}
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005F70 File Offset: 0x00004170
		private static uint[,] transpose4(uint[,] mat)
		{
			uint[,] ret = new uint[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					ret[j, i] = mat[i, j];
				}
			}
			return ret;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00005FC3 File Offset: 0x000041C3
		// (set) Token: 0x0600008E RID: 142 RVA: 0x00005FCB File Offset: 0x000041CB
		public uint[,] InverseKey
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00005FD4 File Offset: 0x000041D4
		// (set) Token: 0x06000090 RID: 144 RVA: 0x00005FDC File Offset: 0x000041DC
		public uint[,] Key
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

		// Token: 0x0400003A RID: 58
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint[,] <InverseKey>k__BackingField;

		// Token: 0x0400003B RID: 59
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint[,] <Key>k__BackingField;

		// Token: 0x02000039 RID: 57
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000125 RID: 293 RVA: 0x00007620 File Offset: 0x00005820
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000126 RID: 294 RVA: 0x000020FC File Offset: 0x000002FC
			public <>c()
			{
			}

			// Token: 0x06000127 RID: 295 RVA: 0x0000762C File Offset: 0x0000582C
			internal LiteralExpression <EmitCore>b__4_0(uint v)
			{
				return v;
			}

			// Token: 0x0400009D RID: 157
			public static readonly Matrix.<>c <>9 = new Matrix.<>c();

			// Token: 0x0400009E RID: 158
			public static Func<uint, LiteralExpression> <>9__4_0;
		}

		// Token: 0x0200003A RID: 58
		[CompilerGenerated]
		private sealed class <>c__DisplayClass6_0
		{
			// Token: 0x06000128 RID: 296 RVA: 0x000020FC File Offset: 0x000002FC
			public <>c__DisplayClass6_0()
			{
			}

			// Token: 0x06000129 RID: 297 RVA: 0x00007634 File Offset: 0x00005834
			internal uint <GenerateUnimodularMatrix>b__0()
			{
				return (uint)this.random.NextInt32(4);
			}

			// Token: 0x0400009F RID: 159
			public RandomGenerator random;
		}
	}
}
