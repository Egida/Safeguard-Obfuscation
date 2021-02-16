using System;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000A9 RID: 169
	internal struct JITEHClause
	{
		// Token: 0x040001BF RID: 447
		public uint ClassTokenOrFilterOffset;

		// Token: 0x040001C0 RID: 448
		public uint Flags;

		// Token: 0x040001C1 RID: 449
		public uint HandlerLength;

		// Token: 0x040001C2 RID: 450
		public uint HandlerOffset;

		// Token: 0x040001C3 RID: 451
		public uint TryLength;

		// Token: 0x040001C4 RID: 452
		public uint TryOffset;
	}
}
