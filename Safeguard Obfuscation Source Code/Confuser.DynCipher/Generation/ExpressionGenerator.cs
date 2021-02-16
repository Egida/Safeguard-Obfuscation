using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000010 RID: 16
	internal class ExpressionGenerator
	{
		// Token: 0x0600004D RID: 77 RVA: 0x000040D8 File Offset: 0x000022D8
		private static Expression GenerateExpression(RandomGenerator random, Expression current, int currentDepth, int targetDepth)
		{
			bool flag = currentDepth == targetDepth || (currentDepth > targetDepth / 3 && random.NextInt32(100) > 85);
			Expression result;
			if (flag)
			{
				result = current;
			}
			else
			{
				switch (random.NextInt32(6))
				{
				case 0:
					result = ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth) + ExpressionGenerator.GenerateExpression(random, random.NextUInt32(), currentDepth + 1, targetDepth);
					break;
				case 1:
					result = ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth) - ExpressionGenerator.GenerateExpression(random, random.NextUInt32(), currentDepth + 1, targetDepth);
					break;
				case 2:
					result = ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth) * (random.NextUInt32() | 1U);
					break;
				case 3:
					result = (ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth) ^ ExpressionGenerator.GenerateExpression(random, random.NextUInt32(), currentDepth + 1, targetDepth));
					break;
				case 4:
					result = ~ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth);
					break;
				case 5:
					result = -ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth);
					break;
				default:
					throw new UnreachableException();
				}
			}
			return result;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004200 File Offset: 0x00002400
		private static void SwapOperands(RandomGenerator random, Expression exp)
		{
			bool flag = exp is BinOpExpression;
			if (flag)
			{
				BinOpExpression binExp = (BinOpExpression)exp;
				bool flag2 = random.NextBoolean();
				if (flag2)
				{
					Expression tmp = binExp.Left;
					binExp.Left = binExp.Right;
					binExp.Right = tmp;
				}
				ExpressionGenerator.SwapOperands(random, binExp.Left);
				ExpressionGenerator.SwapOperands(random, binExp.Right);
			}
			else
			{
				bool flag3 = exp is UnaryOpExpression;
				if (flag3)
				{
					ExpressionGenerator.SwapOperands(random, ((UnaryOpExpression)exp).Value);
				}
				else
				{
					bool flag4 = exp is LiteralExpression || exp is VariableExpression;
					if (!flag4)
					{
						throw new UnreachableException();
					}
				}
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000042B0 File Offset: 0x000024B0
		private static bool HasVariable(Expression exp, Dictionary<Expression, bool> hasVar)
		{
			bool ret;
			bool flag = !hasVar.TryGetValue(exp, out ret);
			if (flag)
			{
				bool flag2 = exp is VariableExpression;
				if (flag2)
				{
					ret = true;
				}
				else
				{
					bool flag3 = exp is LiteralExpression;
					if (flag3)
					{
						ret = false;
					}
					else
					{
						bool flag4 = exp is BinOpExpression;
						if (flag4)
						{
							BinOpExpression binExp = (BinOpExpression)exp;
							ret = (ExpressionGenerator.HasVariable(binExp.Left, hasVar) || ExpressionGenerator.HasVariable(binExp.Right, hasVar));
						}
						else
						{
							bool flag5 = exp is UnaryOpExpression;
							if (!flag5)
							{
								throw new UnreachableException();
							}
							ret = ExpressionGenerator.HasVariable(((UnaryOpExpression)exp).Value, hasVar);
						}
					}
				}
				hasVar[exp] = ret;
			}
			return ret;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000436C File Offset: 0x0000256C
		private static Expression GenerateInverse(Expression exp, Expression var, Dictionary<Expression, bool> hasVar)
		{
			Expression result = var;
			while (!(exp is VariableExpression))
			{
				Debug.Assert(hasVar[exp]);
				bool flag = exp is UnaryOpExpression;
				if (flag)
				{
					UnaryOpExpression unaryOp = (UnaryOpExpression)exp;
					result = new UnaryOpExpression
					{
						Operation = unaryOp.Operation,
						Value = result
					};
					exp = unaryOp.Value;
				}
				else
				{
					bool flag2 = exp is BinOpExpression;
					if (flag2)
					{
						BinOpExpression binOp = (BinOpExpression)exp;
						bool leftHasVar = hasVar[binOp.Left];
						Expression varExp = leftHasVar ? binOp.Left : binOp.Right;
						Expression constExp = leftHasVar ? binOp.Right : binOp.Left;
						bool flag3 = binOp.Operation == BinOps.Add;
						if (flag3)
						{
							result = new BinOpExpression
							{
								Operation = BinOps.Sub,
								Left = result,
								Right = constExp
							};
						}
						else
						{
							bool flag4 = binOp.Operation == BinOps.Sub;
							if (flag4)
							{
								bool flag5 = leftHasVar;
								if (flag5)
								{
									result = new BinOpExpression
									{
										Operation = BinOps.Add,
										Left = result,
										Right = constExp
									};
								}
								else
								{
									result = new BinOpExpression
									{
										Operation = BinOps.Sub,
										Left = constExp,
										Right = result
									};
								}
							}
							else
							{
								bool flag6 = binOp.Operation == BinOps.Mul;
								if (flag6)
								{
									Debug.Assert(constExp is LiteralExpression);
									uint val = ((LiteralExpression)constExp).Value;
									val = MathsUtils.modInv(val);
									result = new BinOpExpression
									{
										Operation = BinOps.Mul,
										Left = result,
										Right = val
									};
								}
								else
								{
									bool flag7 = binOp.Operation == BinOps.Xor;
									if (flag7)
									{
										result = new BinOpExpression
										{
											Operation = BinOps.Xor,
											Left = result,
											Right = constExp
										};
									}
								}
							}
						}
						exp = varExp;
					}
				}
			}
			return result;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004568 File Offset: 0x00002768
		public static void GeneratePair(RandomGenerator random, Expression var, Expression result, int depth, out Expression expression, out Expression inverse)
		{
			expression = ExpressionGenerator.GenerateExpression(random, var, 0, depth);
			ExpressionGenerator.SwapOperands(random, expression);
			Dictionary<Expression, bool> hasVar = new Dictionary<Expression, bool>();
			ExpressionGenerator.HasVariable(expression, hasVar);
			inverse = ExpressionGenerator.GenerateInverse(expression, result, hasVar);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000020FC File Offset: 0x000002FC
		public ExpressionGenerator()
		{
		}

		// Token: 0x02000037 RID: 55
		private enum ExpressionOps
		{
			// Token: 0x04000095 RID: 149
			Add,
			// Token: 0x04000096 RID: 150
			Sub,
			// Token: 0x04000097 RID: 151
			Mul,
			// Token: 0x04000098 RID: 152
			Xor,
			// Token: 0x04000099 RID: 153
			Not,
			// Token: 0x0400009A RID: 154
			Neg
		}
	}
}
