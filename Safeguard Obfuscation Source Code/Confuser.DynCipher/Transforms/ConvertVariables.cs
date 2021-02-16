using System;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Transforms
{
	// Token: 0x02000007 RID: 7
	internal class ConvertVariables
	{
		// Token: 0x06000011 RID: 17 RVA: 0x000023D8 File Offset: 0x000005D8
		private static Expression ReplaceVar(Expression exp, Variable buff)
		{
			bool flag = !(exp is VariableExpression);
			Expression result;
			if (flag)
			{
				bool flag2 = exp is ArrayIndexExpression;
				if (flag2)
				{
					((ArrayIndexExpression)exp).Array = ConvertVariables.ReplaceVar(((ArrayIndexExpression)exp).Array, buff);
				}
				else
				{
					bool flag3 = exp is BinOpExpression;
					if (flag3)
					{
						((BinOpExpression)exp).Left = ConvertVariables.ReplaceVar(((BinOpExpression)exp).Left, buff);
						((BinOpExpression)exp).Right = ConvertVariables.ReplaceVar(((BinOpExpression)exp).Right, buff);
					}
					else
					{
						bool flag4 = exp is UnaryOpExpression;
						if (flag4)
						{
							((UnaryOpExpression)exp).Value = ConvertVariables.ReplaceVar(((UnaryOpExpression)exp).Value, buff);
						}
					}
				}
				result = exp;
			}
			else
			{
				bool flag5 = ((VariableExpression)exp).Variable.Name[0] != 'v';
				if (flag5)
				{
					result = exp;
				}
				else
				{
					result = new ArrayIndexExpression
					{
						Array = new VariableExpression
						{
							Variable = buff
						},
						Index = (int)(exp as VariableExpression).Variable.Tag
					};
				}
			}
			return result;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002510 File Offset: 0x00000710
		private static Statement ReplaceVar(Statement st, Variable buff)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				((AssignmentStatement)st).Value = ConvertVariables.ReplaceVar(((AssignmentStatement)st).Value, buff);
				((AssignmentStatement)st).Target = ConvertVariables.ReplaceVar(((AssignmentStatement)st).Target, buff);
			}
			return st;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000256C File Offset: 0x0000076C
		public static void Run(StatementBlock block)
		{
			Variable mainBuff = new Variable("{BUFFER}");
			for (int i = 0; i < block.Statements.Count; i++)
			{
				block.Statements[i] = ConvertVariables.ReplaceVar(block.Statements[i], mainBuff);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000020FC File Offset: 0x000002FC
		public ConvertVariables()
		{
		}
	}
}
