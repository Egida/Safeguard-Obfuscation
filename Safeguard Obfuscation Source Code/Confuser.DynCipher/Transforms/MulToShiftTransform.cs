using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Transforms
{
	// Token: 0x02000009 RID: 9
	internal class MulToShiftTransform
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00002734 File Offset: 0x00000934
		private static uint NumberOfSetBits(uint i)
		{
			i -= (i >> 1 & 1431655765U);
			i = (i & 858993459U) + (i >> 2 & 858993459U);
			return (i + (i >> 4) & 252645135U) * 16843009U >> 24;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000277C File Offset: 0x0000097C
		private static Expression ProcessExpression(Expression exp)
		{
			bool flag = exp is BinOpExpression;
			if (flag)
			{
				BinOpExpression binOp = (BinOpExpression)exp;
				bool flag2 = binOp.Operation == BinOps.Mul && binOp.Right is LiteralExpression;
				if (flag2)
				{
					uint literal = ((LiteralExpression)binOp.Right).Value;
					bool flag3 = literal == 0U;
					if (flag3)
					{
						return 0U;
					}
					bool flag4 = literal == 1U;
					if (flag4)
					{
						return binOp.Left;
					}
					uint bits = MulToShiftTransform.NumberOfSetBits(literal);
					bool flag5 = bits <= 2U;
					if (flag5)
					{
						List<Expression> sum = new List<Expression>();
						int i = 0;
						while (literal > 0U)
						{
							bool flag6 = (literal & 1U) > 0U;
							if (flag6)
							{
								bool flag7 = i == 0;
								if (flag7)
								{
									sum.Add(binOp.Left);
								}
								else
								{
									sum.Add(binOp.Left << i);
								}
							}
							literal >>= 1;
							i++;
						}
						BinOpExpression x = sum.OfType<BinOpExpression>().First<BinOpExpression>();
						foreach (Expression j in sum.Except(new BinOpExpression[]
						{
							x
						}))
						{
							x += j;
						}
						return x;
					}
				}
				else
				{
					binOp.Left = MulToShiftTransform.ProcessExpression(binOp.Left);
					binOp.Right = MulToShiftTransform.ProcessExpression(binOp.Right);
				}
			}
			else
			{
				bool flag8 = exp is ArrayIndexExpression;
				if (flag8)
				{
					((ArrayIndexExpression)exp).Array = MulToShiftTransform.ProcessExpression(((ArrayIndexExpression)exp).Array);
				}
				else
				{
					bool flag9 = exp is UnaryOpExpression;
					if (flag9)
					{
						((UnaryOpExpression)exp).Value = MulToShiftTransform.ProcessExpression(((UnaryOpExpression)exp).Value);
					}
				}
			}
			return exp;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000297C File Offset: 0x00000B7C
		private static void ProcessStatement(Statement st)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				AssignmentStatement assign = (AssignmentStatement)st;
				assign.Target = MulToShiftTransform.ProcessExpression(assign.Target);
				assign.Value = MulToShiftTransform.ProcessExpression(assign.Value);
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000029C4 File Offset: 0x00000BC4
		public static void Run(StatementBlock block)
		{
			foreach (Statement st in block.Statements)
			{
				MulToShiftTransform.ProcessStatement(st);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000020FC File Offset: 0x000002FC
		public MulToShiftTransform()
		{
		}
	}
}
