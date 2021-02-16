using System;
using System.Linq;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Transforms
{
	// Token: 0x02000008 RID: 8
	internal class ExpansionTransform
	{
		// Token: 0x06000015 RID: 21 RVA: 0x000025C0 File Offset: 0x000007C0
		private static bool ProcessStatement(Statement st, StatementBlock block)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				AssignmentStatement assign = (AssignmentStatement)st;
				bool flag2 = assign.Value is BinOpExpression;
				if (flag2)
				{
					BinOpExpression exp = (BinOpExpression)assign.Value;
					bool flag3 = (exp.Left is BinOpExpression || exp.Right is BinOpExpression) && exp.Left != assign.Target;
					if (flag3)
					{
						block.Statements.Add(new AssignmentStatement
						{
							Target = assign.Target,
							Value = exp.Left
						});
						block.Statements.Add(new AssignmentStatement
						{
							Target = assign.Target,
							Value = new BinOpExpression
							{
								Left = assign.Target,
								Operation = exp.Operation,
								Right = exp.Right
							}
						});
						return true;
					}
				}
			}
			block.Statements.Add(st);
			return false;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000026D8 File Offset: 0x000008D8
		public static void Run(StatementBlock block)
		{
			bool workDone;
			do
			{
				workDone = false;
				Statement[] copy = block.Statements.ToArray<Statement>();
				block.Statements.Clear();
				foreach (Statement st in copy)
				{
					workDone |= ExpansionTransform.ProcessStatement(st, block);
				}
			}
			while (workDone);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000020FC File Offset: 0x000002FC
		public ExpansionTransform()
		{
		}
	}
}
