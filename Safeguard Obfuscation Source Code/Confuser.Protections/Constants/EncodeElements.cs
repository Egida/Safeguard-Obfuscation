using System;

namespace Confuser.Protections.Constants
{
	// Token: 0x02000095 RID: 149
	[Flags]
	internal enum EncodeElements
	{
		// Token: 0x04000190 RID: 400
		Strings = 1,
		// Token: 0x04000191 RID: 401
		Numbers = 2,
		// Token: 0x04000192 RID: 402
		Primitive = 4,
		// Token: 0x04000193 RID: 403
		Initializers = 8
	}
}
