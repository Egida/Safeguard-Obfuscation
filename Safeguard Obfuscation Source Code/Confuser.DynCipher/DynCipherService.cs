using System;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher
{
	// Token: 0x02000004 RID: 4
	internal class DynCipherService : IDynCipherService
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000020DE File Offset: 0x000002DE
		public void GenerateCipherPair(RandomGenerator random, out StatementBlock encrypt, out StatementBlock decrypt)
		{
			CipherGenerator.GeneratePair(random, out encrypt, out decrypt);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000020EA File Offset: 0x000002EA
		public void GenerateExpressionPair(RandomGenerator random, Expression var, Expression result, int depth, out Expression expression, out Expression inverse)
		{
			ExpressionGenerator.GeneratePair(random, var, result, depth, out expression, out inverse);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000020FC File Offset: 0x000002FC
		public DynCipherService()
		{
		}
	}
}
