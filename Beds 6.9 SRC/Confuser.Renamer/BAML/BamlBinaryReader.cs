using System;
using System.IO;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200006C RID: 108
	internal class BamlBinaryReader : BinaryReader
	{
		// Token: 0x0600028C RID: 652 RVA: 0x00003448 File Offset: 0x00001648
		public BamlBinaryReader(Stream stream) : base(stream)
		{
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0000A174 File Offset: 0x00008374
		public int ReadEncodedInt()
		{
			return base.Read7BitEncodedInt();
		}
	}
}
