using System;

namespace Confuser.Runtime
{
	// Token: 0x02000012 RID: 18
	internal struct CFGCtx
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00004B9C File Offset: 0x00002D9C
		public CFGCtx(uint seed)
		{
			seed = (this.eat = seed * 949127444U);
			seed = (this.rrac = seed * 2190575175U);
			seed = (this.hhqA = seed * 422073891U);
			seed = (this.ggteee = seed * 1646872663U);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004BEC File Offset: 0x00002DEC
		public uint Next(byte f, uint qqq)
		{
			if ((f & 128) != 0)
			{
				switch (f & 3)
				{
				case 0:
					this.eat = qqq;
					break;
				case 1:
					this.rrac = qqq;
					break;
				case 2:
					this.hhqA = qqq;
					break;
				case 3:
					this.ggteee = qqq;
					break;
				}
			}
			else
			{
				switch (f & 3)
				{
				case 0:
					this.eat ^= qqq;
					break;
				case 1:
					this.rrac += qqq;
					break;
				case 2:
					this.hhqA ^= qqq;
					break;
				case 3:
					this.ggteee -= qqq;
					break;
				}
			}
			switch (f >> 2 & 3)
			{
			case 0:
				return this.eat;
			case 1:
				return this.rrac;
			case 2:
				return this.hhqA;
			default:
				return this.ggteee;
			}
		}

		// Token: 0x04000034 RID: 52
		private uint eat;

		// Token: 0x04000035 RID: 53
		private uint rrac;

		// Token: 0x04000036 RID: 54
		private uint hhqA;

		// Token: 0x04000037 RID: 55
		private uint ggteee;
	}
}
