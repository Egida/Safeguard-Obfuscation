using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003A RID: 58
	internal class XmlnsPropertyRecord : SizedBamlRecord
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000140 RID: 320 RVA: 0x000098AC File Offset: 0x00007AAC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.XmlnsProperty;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000141 RID: 321 RVA: 0x000027A1 File Offset: 0x000009A1
		// (set) Token: 0x06000142 RID: 322 RVA: 0x000027A9 File Offset: 0x000009A9
		public string Prefix
		{
			[CompilerGenerated]
			get
			{
				return this.<Prefix>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Prefix>k__BackingField = value;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000143 RID: 323 RVA: 0x000027B2 File Offset: 0x000009B2
		// (set) Token: 0x06000144 RID: 324 RVA: 0x000027BA File Offset: 0x000009BA
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

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000145 RID: 325 RVA: 0x000027C3 File Offset: 0x000009C3
		// (set) Token: 0x06000146 RID: 326 RVA: 0x000027CB File Offset: 0x000009CB
		public ushort[] AssemblyIds
		{
			[CompilerGenerated]
			get
			{
				return this.<AssemblyIds>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<AssemblyIds>k__BackingField = value;
			}
		}

		// Token: 0x06000147 RID: 327 RVA: 0x000098C0 File Offset: 0x00007AC0
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Prefix = reader.ReadString();
			this.XmlNamespace = reader.ReadString();
			this.AssemblyIds = new ushort[(int)reader.ReadUInt16()];
			for (int i = 0; i < this.AssemblyIds.Length; i++)
			{
				this.AssemblyIds[i] = reader.ReadUInt16();
			}
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00009920 File Offset: 0x00007B20
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Prefix);
			writer.Write(this.XmlNamespace);
			writer.Write((ushort)this.AssemblyIds.Length);
			foreach (ushort i in this.AssemblyIds)
			{
				writer.Write(i);
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000027D4 File Offset: 0x000009D4
		public XmlnsPropertyRecord()
		{
		}

		// Token: 0x040000D8 RID: 216
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Prefix>k__BackingField;

		// Token: 0x040000D9 RID: 217
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <XmlNamespace>k__BackingField;

		// Token: 0x040000DA RID: 218
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort[] <AssemblyIds>k__BackingField;
	}
}
