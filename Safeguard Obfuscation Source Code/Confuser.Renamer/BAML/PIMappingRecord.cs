using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003C RID: 60
	internal class PIMappingRecord : SizedBamlRecord
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00009990 File Offset: 0x00007B90
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PIMapping;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00002839 File Offset: 0x00000A39
		// (set) Token: 0x06000154 RID: 340 RVA: 0x00002841 File Offset: 0x00000A41
		public string XmlNamespace
		{
			[CompilerGenerated]
			get
			{
				return this.<XmlNamespace>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<XmlNamespace>k__BackingField = value;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000155 RID: 341 RVA: 0x0000284A File Offset: 0x00000A4A
		// (set) Token: 0x06000156 RID: 342 RVA: 0x00002852 File Offset: 0x00000A52
		public string ClrNamespace
		{
			[CompilerGenerated]
			get
			{
				return this.<ClrNamespace>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<ClrNamespace>k__BackingField = value;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000157 RID: 343 RVA: 0x0000285B File Offset: 0x00000A5B
		// (set) Token: 0x06000158 RID: 344 RVA: 0x00002863 File Offset: 0x00000A63
		public ushort AssemblyId
		{
			[CompilerGenerated]
			get
			{
				return this.<AssemblyId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<AssemblyId>k__BackingField = value;
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000286C File Offset: 0x00000A6C
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.XmlNamespace = reader.ReadString();
			this.ClrNamespace = reader.ReadString();
			this.AssemblyId = reader.ReadUInt16();
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00002896 File Offset: 0x00000A96
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.XmlNamespace);
			writer.Write(this.ClrNamespace);
			writer.Write(this.AssemblyId);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x000027D4 File Offset: 0x000009D4
		public PIMappingRecord()
		{
		}

		// Token: 0x040000DD RID: 221
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <XmlNamespace>k__BackingField;

		// Token: 0x040000DE RID: 222
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <ClrNamespace>k__BackingField;

		// Token: 0x040000DF RID: 223
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <AssemblyId>k__BackingField;
	}
}
