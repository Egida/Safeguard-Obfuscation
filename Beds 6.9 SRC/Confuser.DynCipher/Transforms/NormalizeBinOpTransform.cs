using System;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Transforms
{
	// Token: 0x0200000A RID: 10
	internal class NormalizeBinOpTransform
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002A14 File Offset: 0x00000C14
		private static Expression ProcessExpression(Expression exp)
		{
			bool flag = exp is BinOpExpression;
			if (flag)
			{
				BinOpExpression binOp = (BinOpExpression)exp;
				BinOpExpression binOpRight = binOp.Right as BinOpExpression;
				bool flag2 = binOpRight != null && binOpRight.Operation == binOp.Operation && (binOp.Operation == BinOps.Add || binOp.Operation == BinOps.Mul || binOp.Operation == BinOps.Or || binOp.Operation == BinOps.And || binOp.Operation == BinOps.Xor);
				if (flag2)
				{
					binOp.Left = new BinOpExpression
					{
						Left = binOp.Left,
						Operation = binOp.Operation,
						Right = binOpRight.Left
					};
					binOp.Right = binOpRight.Right;
				}
				binOp.Left = NormalizeBinOpTransform.ProcessExpression(binOp.Left);
				binOp.Right = NormalizeBinOpTransform.ProcessExpression(binOp.Right);
				bool flag3 = binOp.Right is LiteralExpression && ((LiteralExpression)binOp.Right).Value == 0U && binOp.Operation == BinOps.Add;
				if (flag3)
				{
					return binOp.Left;
				}
			}
			else
			{
				bool flag4 = exp is ArrayIndexExpression;
				if (flag4)
				{
					((ArrayIndexExpression)exp).Array = NormalizeBinOpTransform.ProcessExpression(((ArrayIndexExpression)exp).Array);
				}
				else
				{
					bool flag5 = exp is UnaryOpExpression;
					if (flag5)
					{
						((UnaryOpExpression)exp).Value = NormalizeBinOpTransform.ProcessExpression(((UnaryOpExpression)exp).Value);
					}
				}
			}
			return exp;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002B98 File Offset: 0x00000D98
		private static void ProcessStatement(Statement st)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				AssignmentStatement assign = (AssignmentStatement)st;
				assign.Target = NormalizeBinOpTransform.ProcessExpression(assign.Target);
				assign.Value = NormalizeBinOpTransform.ProcessExpression(assign.Value);
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002BE0 File Offset: 0x00000DE0
		public static void Run(StatementBlock block)
		{
			foreach (Statement st in block.Statements)
			{
				NormalizeBinOpTransform.ProcessStatement(st);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000020FC File Offset: 0x000002FC
		public NormalizeBinOpTransform()
		{
		}
	}
}
