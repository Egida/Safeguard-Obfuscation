using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;

namespace Confuser.Core
{
	// Token: 0x02000050 RID: 80
	internal class NativeEraser
	{
		// Token: 0x060001E4 RID: 484 RVA: 0x0000E878 File Offset: 0x0000CA78
		private static void Erase(List<Tuple<uint, uint, byte[]>> sections, IFileSection s)
		{
			foreach (Tuple<uint, uint, byte[]> sect in sections)
			{
				bool flag = (uint)s.StartOffset >= sect.Item1 && (uint)s.EndOffset < sect.Item2;
				if (flag)
				{
					NativeEraser.Erase(sect, (uint)s.StartOffset, (uint)(s.EndOffset - s.StartOffset));
					break;
				}
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000E908 File Offset: 0x0000CB08
		private static void Erase(List<Tuple<uint, uint, byte[]>> sections, uint methodOffset)
		{
			foreach (Tuple<uint, uint, byte[]> sect in sections)
			{
				bool flag = methodOffset >= sect.Item1;
				if (flag)
				{
					uint f = (uint)sect.Item3[(int)((uint)((UIntPtr)(methodOffset - sect.Item1)))];
					uint size;
					switch (f & 7U)
					{
					case 2U:
					case 6U:
						size = (f >> 2) + 1U;
						break;
					case 3U:
					{
						f |= (uint)((uint)sect.Item3[(int)((uint)((UIntPtr)(methodOffset - sect.Item1 + 1U)))] << 8);
						size = (f >> 12) * 4U;
						uint codeSize = BitConverter.ToUInt32(sect.Item3, (int)(methodOffset - sect.Item1 + 4U));
						size += codeSize;
						break;
					}
					case 4U:
					case 5U:
						goto IL_CB;
					default:
						goto IL_CB;
					}
					NativeEraser.Erase(sect, methodOffset, size);
					continue;
					IL_CB:
					break;
				}
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000EA14 File Offset: 0x0000CC14
		public static void Erase(NativeModuleWriter writer, ModuleDefMD module)
		{
			bool flag = writer == null || module == null;
			if (!flag)
			{
				List<Tuple<uint, uint, byte[]>> sections = new List<Tuple<uint, uint, byte[]>>();
				MemoryStream s = new MemoryStream();
				foreach (NativeModuleWriter.OrigSection origSect in writer.OrigSections)
				{
					BinaryReaderChunk oldChunk = origSect.Chunk;
					ImageSectionHeader sectHdr = origSect.PESection;
					s.SetLength(0L);
					oldChunk.WriteTo(new BinaryWriter(s));
					byte[] buf = s.ToArray();
					BinaryReaderChunk newChunk = new BinaryReaderChunk(MemoryImageStream.Create(buf), oldChunk.GetVirtualSize());
					newChunk.SetOffset(oldChunk.FileOffset, oldChunk.RVA);
					origSect.Chunk = newChunk;
					sections.Add(Tuple.Create<uint, uint, byte[]>(sectHdr.PointerToRawData, sectHdr.PointerToRawData + sectHdr.SizeOfRawData, buf));
				}
				IMetaData md = module.MetaData;
				uint row = md.TablesStream.MethodTable.Rows;
				for (uint i = 1U; i <= row; i += 1U)
				{
					RawMethodRow method = md.TablesStream.ReadMethodRow(i);
					bool flag2 = (method.ImplFlags & 3) == 0;
					if (flag2)
					{
						NativeEraser.Erase(sections, (uint)md.PEImage.ToFileOffset((RVA)method.RVA));
					}
				}
				ImageDataDirectory res = md.ImageCor20Header.Resources;
				bool flag3 = res.Size > 0U;
				if (flag3)
				{
					NativeEraser.Erase(sections, (uint)res.StartOffset, res.Size);
				}
				NativeEraser.Erase(sections, md.ImageCor20Header);
				NativeEraser.Erase(sections, md.MetaDataHeader);
				foreach (DotNetStream stream in md.AllStreams)
				{
					NativeEraser.Erase(sections, stream);
				}
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00002CCE File Offset: 0x00000ECE
		private static void Erase(Tuple<uint, uint, byte[]> section, uint offset, uint len)
		{
			Array.Clear(section.Item3, (int)(offset - section.Item1), (int)len);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000EC20 File Offset: 0x0000CE20
		private static void Erase(List<Tuple<uint, uint, byte[]>> sections, uint beginOffset, uint size)
		{
			foreach (Tuple<uint, uint, byte[]> sect in sections)
			{
				bool flag = beginOffset >= sect.Item1 && beginOffset + size < sect.Item2;
				if (flag)
				{
					NativeEraser.Erase(sect, beginOffset, size);
					break;
				}
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00002194 File Offset: 0x00000394
		public NativeEraser()
		{
		}
	}
}
