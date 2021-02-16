using System;

namespace Confuser.Runtime
{
	// Token: 0x02000031 RID: 49
	internal class RefProxyKey : Attribute
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x000026BD File Offset: 0x000008BD
		public RefProxyKey(int key)
		{
			this.key = Mutation.Placeholder<int>(key);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000026D1 File Offset: 0x000008D1
		public override int GetHashCode()
		{
			return this.key;
		}

		// Token: 0x040000B8 RID: 184
		private readonly int key;
	}
}
