using System;

namespace Confuser.Renamer
{
	// Token: 0x0200000F RID: 15
	public enum RenameMode
	{
		// Token: 0x0400001E RID: 30
		Empty,
		// Token: 0x0400001F RID: 31
		Unicode,
		// Token: 0x04000020 RID: 32
		ASCII,
		// Token: 0x04000021 RID: 33
		Letters,
		// Token: 0x04000022 RID: 34
		Decodable = 16,
		// Token: 0x04000023 RID: 35
		Sequential,
		// Token: 0x04000024 RID: 36
		Reversible,
		// Token: 0x04000025 RID: 37
		Debug = 32
	}
}
