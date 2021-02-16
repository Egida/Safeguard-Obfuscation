using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000070 RID: 112
	internal class BamlWriter
	{
		// Token: 0x06000294 RID: 660 RVA: 0x0000A6F0 File Offset: 0x000088F0
		public static void WriteDocument(BamlDocument doc, Stream str)
		{
			BamlBinaryWriter writer = new BamlBinaryWriter(str);
			BinaryWriter wtr = new BinaryWriter(str, Encoding.Unicode);
			int len = doc.Signature.Length * 2;
			wtr.Write(len);
			wtr.Write(doc.Signature.ToCharArray());
			wtr.Write(new byte[(len + 3 & -4) - len]);
			writer.Write(doc.ReaderVersion.Major);
			writer.Write(doc.ReaderVersion.Minor);
			writer.Write(doc.UpdaterVersion.Major);
			writer.Write(doc.UpdaterVersion.Minor);
			writer.Write(doc.WriterVersion.Major);
			writer.Write(doc.WriterVersion.Minor);
			List<int> defers = new List<int>();
			for (int i = 0; i < doc.Count; i++)
			{
				BamlRecord rec = doc[i];
				rec.Position = str.Position;
				writer.Write((byte)rec.Type);
				rec.Write(writer);
				bool flag = rec is IBamlDeferRecord;
				if (flag)
				{
					defers.Add(i);
				}
			}
			foreach (int j in defers)
			{
				(doc[j] as IBamlDeferRecord).WriteDefer(doc, j, writer);
			}
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00002184 File Offset: 0x00000384
		public BamlWriter()
		{
		}
	}
}
