using System;
using System.Text;

namespace Confuser.Core.Services
{
	// Token: 0x0200007E RID: 126
	internal class RandomService : IRandomService
	{
		// Token: 0x060002E0 RID: 736 RVA: 0x000033E4 File Offset: 0x000015E4
		public RandomService(string seed)
		{
			this.seed = RandomGenerator.Seed(seed);
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00012BD8 File Offset: 0x00010DD8
		public RandomGenerator GetRandomGenerator(string id)
		{
			bool flag = string.IsNullOrEmpty(id);
			if (flag)
			{
				throw new ArgumentNullException("id");
			}
			byte[] newSeed = this.seed;
			byte[] idHash = Utils.SHA256(Encoding.UTF8.GetBytes(id));
			for (int i = 0; i < 32; i++)
			{
				byte[] expr_36_cp_0 = newSeed;
				int expr_36_cp_ = i;
				byte[] array = expr_36_cp_0;
				int num = expr_36_cp_;
				array[num] ^= idHash[i];
			}
			return new RandomGenerator(Utils.SHA256(newSeed));
		}

		// Token: 0x0400022A RID: 554
		private readonly byte[] seed;
	}
}
