using System;
using System.IO;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200006D RID: 109
	internal class BamlBinaryWriter : BinaryWriter
	{
		// Token: 0x0600028E RID: 654 RVA: 0x00003453 File Offset: 0x00001653
		public BamlBinaryWriter(Stream stream) : base(stream)
		{
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000345E File Offset: 0x0000165E
		public void WriteEncodedInt(int val)
		{
			base.Write7BitEncodedInt(val);
		}
	}
}
