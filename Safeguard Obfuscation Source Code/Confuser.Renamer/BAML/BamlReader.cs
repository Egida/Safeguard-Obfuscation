using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200006E RID: 110
	internal class BamlReader
	{
		// Token: 0x06000290 RID: 656 RVA: 0x0000A18C File Offset: 0x0000838C
		public static BamlDocument ReadDocument(Stream str)
		{
			BamlDocument ret = new BamlDocument();
			BamlBinaryReader reader = new BamlBinaryReader(str);
			BinaryReader rdr = new BinaryReader(str, Encoding.Unicode);
			uint len = rdr.ReadUInt32();
			ret.Signature = new string(rdr.ReadChars((int)(len >> 1)));
			rdr.ReadBytes((int)(((ulong)(len + 3U) & 18446744073709551612UL) - (ulong)len));
			bool flag = ret.Signature != "MSBAML";
			if (flag)
			{
				throw new NotSupportedException();
			}
			ret.ReaderVersion = new BamlDocument.BamlVersion
			{
				Major = reader.ReadUInt16(),
				Minor = reader.ReadUInt16()
			};
			ret.UpdaterVersion = new BamlDocument.BamlVersion
			{
				Major = reader.ReadUInt16(),
				Minor = reader.ReadUInt16()
			};
			ret.WriterVersion = new BamlDocument.BamlVersion
			{
				Major = reader.ReadUInt16(),
				Minor = reader.ReadUInt16()
			};
			bool flag2 = ret.ReaderVersion.Major != 0 || ret.ReaderVersion.Minor != 96 || ret.UpdaterVersion.Major != 0 || ret.UpdaterVersion.Minor != 96 || ret.WriterVersion.Major != 0 || ret.WriterVersion.Minor != 96;
			if (flag2)
			{
				throw new NotSupportedException();
			}
			Dictionary<long, BamlRecord> recs = new Dictionary<long, BamlRecord>();
			while (str.Position < str.Length)
			{
				long pos = str.Position;
				BamlRecord rec;
				switch (reader.ReadByte())
				{
				case 1:
					rec = new DocumentStartRecord();
					break;
				case 2:
					rec = new DocumentEndRecord();
					break;
				case 3:
					rec = new ElementStartRecord();
					break;
				case 4:
					rec = new ElementEndRecord();
					break;
				case 5:
					rec = new PropertyRecord();
					break;
				case 6:
					rec = new PropertyCustomRecord();
					break;
				case 7:
					rec = new PropertyComplexStartRecord();
					break;
				case 8:
					rec = new PropertyComplexEndRecord();
					break;
				case 9:
					rec = new PropertyArrayStartRecord();
					break;
				case 10:
					rec = new PropertyArrayEndRecord();
					break;
				case 11:
					rec = new PropertyListStartRecord();
					break;
				case 12:
					rec = new PropertyListEndRecord();
					break;
				case 13:
					rec = new PropertyDictionaryStartRecord();
					break;
				case 14:
					rec = new PropertyDictionaryEndRecord();
					break;
				case 15:
					rec = new LiteralContentRecord();
					break;
				case 16:
					rec = new TextRecord();
					break;
				case 17:
					rec = new TextWithConverterRecord();
					break;
				case 18:
					rec = new RoutedEventRecord();
					break;
				case 19:
				case 21:
				case 22:
				case 23:
				case 24:
				case 26:
				case 57:
					goto IL_4A0;
				case 20:
					rec = new XmlnsPropertyRecord();
					break;
				case 25:
					rec = new DefAttributeRecord();
					break;
				case 27:
					rec = new PIMappingRecord();
					break;
				case 28:
					rec = new AssemblyInfoRecord();
					break;
				case 29:
					rec = new TypeInfoRecord();
					break;
				case 30:
					rec = new TypeSerializerInfoRecord();
					break;
				case 31:
					rec = new AttributeInfoRecord();
					break;
				case 32:
					rec = new StringInfoRecord();
					break;
				case 33:
					rec = new PropertyStringReferenceRecord();
					break;
				case 34:
					rec = new PropertyTypeReferenceRecord();
					break;
				case 35:
					rec = new PropertyWithExtensionRecord();
					break;
				case 36:
					rec = new PropertyWithConverterRecord();
					break;
				case 37:
					rec = new DeferableContentStartRecord();
					break;
				case 38:
					rec = new DefAttributeKeyStringRecord();
					break;
				case 39:
					rec = new DefAttributeKeyTypeRecord();
					break;
				case 40:
					rec = new KeyElementStartRecord();
					break;
				case 41:
					rec = new KeyElementEndRecord();
					break;
				case 42:
					rec = new ConstructorParametersStartRecord();
					break;
				case 43:
					rec = new ConstructorParametersEndRecord();
					break;
				case 44:
					rec = new ConstructorParameterTypeRecord();
					break;
				case 45:
					rec = new ConnectionIdRecord();
					break;
				case 46:
					rec = new ContentPropertyRecord();
					break;
				case 47:
					rec = new NamedElementStartRecord();
					break;
				case 48:
					rec = new StaticResourceStartRecord();
					break;
				case 49:
					rec = new StaticResourceEndRecord();
					break;
				case 50:
					rec = new StaticResourceIdRecord();
					break;
				case 51:
					rec = new TextWithIdRecord();
					break;
				case 52:
					rec = new PresentationOptionsAttributeRecord();
					break;
				case 53:
					rec = new LineNumberAndPositionRecord();
					break;
				case 54:
					rec = new LinePositionRecord();
					break;
				case 55:
					rec = new OptimizedStaticResourceRecord();
					break;
				case 56:
					rec = new PropertyWithStaticResourceIdRecord();
					break;
				default:
					goto IL_4A0;
				}
				rec.Position = pos;
				rec.Read(reader);
				ret.Add(rec);
				recs.Add(pos, rec);
				continue;
				IL_4A0:
				throw new NotSupportedException();
			}
			Func<long, BamlRecord> <>9__0;
			for (int i = 0; i < ret.Count; i++)
			{
				IBamlDeferRecord defer = ret[i] as IBamlDeferRecord;
				bool flag3 = defer != null;
				if (flag3)
				{
					IBamlDeferRecord bamlDeferRecord = defer;
					BamlDocument doc = ret;
					int index = i;
					Func<long, BamlRecord> resolve;
					if ((resolve = <>9__0) == null)
					{
						resolve = (<>9__0 = ((long _) => recs[_]));
					}
					bamlDeferRecord.ReadDefer(doc, index, resolve);
				}
			}
			return ret;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00002184 File Offset: 0x00000384
		public BamlReader()
		{
		}

		// Token: 0x0200006F RID: 111
		[CompilerGenerated]
		private sealed class <>c__DisplayClass0_0
		{
			// Token: 0x06000292 RID: 658 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass0_0()
			{
			}

			// Token: 0x06000293 RID: 659 RVA: 0x00003469 File Offset: 0x00001669
			internal BamlRecord <ReadDocument>b__0(long _)
			{
				return this.recs[_];
			}

			// Token: 0x0400011F RID: 287
			public Dictionary<long, BamlRecord> recs;

			// Token: 0x04000120 RID: 288
			public Func<long, BamlRecord> <>9__0;
		}
	}
}
