using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Elements;
using Confuser.DynCipher.Transforms;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x0200000D RID: 13
	internal class CipherGenerator
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00003120 File Offset: 0x00001320
		public static void GeneratePair(RandomGenerator random, out StatementBlock encrypt, out StatementBlock decrypt)
		{
			double varPrecentage = 1.0 + (random.NextDouble() * 2.0 - 1.0) * 0.2;
			int totalElements = (int)((random.NextDouble() + 1.0) * 35.0 * varPrecentage);
			List<CryptoElement> elems = new List<CryptoElement>();
			for (int i = 0; i < totalElements * 4 / 35; i++)
			{
				elems.Add(new Matrix());
			}
			for (int j = 0; j < totalElements * 10 / 35; j++)
			{
				elems.Add(new NumOp());
			}
			for (int k = 0; k < totalElements * 6 / 35; k++)
			{
				elems.Add(new Swap());
			}
			for (int l = 0; l < totalElements * 9 / 35; l++)
			{
				elems.Add(new BinOp());
			}
			for (int m = 0; m < totalElements * 6 / 35; m++)
			{
				elems.Add(new RotateBit());
			}
			for (int n = 0; n < 16; n++)
			{
				elems.Add(new AddKey(n));
			}
			CipherGenerator.Shuffle<CryptoElement>(random, elems);
			int[] x = Enumerable.Range(0, 16).ToArray<int>();
			int index = 16;
			bool overdue = false;
			foreach (CryptoElement elem in elems)
			{
				elem.Initialize(random);
				for (int i2 = 0; i2 < elem.DataCount; i2++)
				{
					bool flag = index == 16;
					if (flag)
					{
						overdue = true;
						index = 0;
					}
					elem.DataIndexes[i2] = x[index++];
				}
				bool flag2 = overdue;
				if (flag2)
				{
					CipherGenerator.Shuffle<int>(random, x);
					index = 0;
					overdue = false;
				}
			}
			CipherGenContext encryptContext = new CipherGenContext(random, 16);
			foreach (CryptoElement elem2 in elems)
			{
				elem2.Emit(encryptContext);
			}
			encrypt = encryptContext.Block;
			CipherGenerator.PostProcessStatements(encrypt, random);
			CipherGenContext decryptContext = new CipherGenContext(random, 16);
			foreach (CryptoElement elem3 in elems.Reverse<CryptoElement>())
			{
				elem3.EmitInverse(decryptContext);
			}
			decrypt = decryptContext.Block;
			CipherGenerator.PostProcessStatements(decrypt, random);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003404 File Offset: 0x00001604
		private static void PostProcessStatements(StatementBlock block, RandomGenerator random)
		{
			MulToShiftTransform.Run(block);
			NormalizeBinOpTransform.Run(block);
			ExpansionTransform.Run(block);
			ShuffleTransform.Run(block, random);
			ConvertVariables.Run(block);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000342C File Offset: 0x0000162C
		private static void Shuffle<T>(RandomGenerator random, IList<T> arr)
		{
			for (int i = 1; i < arr.Count; i++)
			{
				int j = random.NextInt32(i + 1);
				T tmp = arr[i];
				arr[i] = arr[j];
				arr[j] = tmp;
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000020FC File Offset: 0x000002FC
		public CipherGenerator()
		{
		}

		// Token: 0x0400000A RID: 10
		private const int BINOP_RATIO = 9;

		// Token: 0x0400000B RID: 11
		private const int MAT_RATIO = 4;

		// Token: 0x0400000C RID: 12
		private const int NUMOP_RATIO = 10;

		// Token: 0x0400000D RID: 13
		private const int RATIO_SUM = 35;

		// Token: 0x0400000E RID: 14
		private const int ROTATE_RATIO = 6;

		// Token: 0x0400000F RID: 15
		private const int SWAP_RATIO = 6;

		// Token: 0x04000010 RID: 16
		private const double VARIANCE = 0.2;
	}
}
