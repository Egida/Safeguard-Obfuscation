using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000040 RID: 64
	internal class PropertyCustomRecord : SizedBamlRecord
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000172 RID: 370 RVA: 0x000099E0 File Offset: 0x00007BE0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyCustom;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000173 RID: 371 RVA: 0x000029C3 File Offset: 0x00000BC3
		// (set) Token: 0x06000174 RID: 372 RVA: 0x000029CB File Offset: 0x00000BCB
		public ushort AttributeId
		{
			[CompilerGenerated]
			get
			{
				return this.<AttributeId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<AttributeId>k__BackingField = value;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000175 RID: 373 RVA: 0x000029D4 File Offset: 0x00000BD4
		// (set) Token: 0x06000176 RID: 374 RVA: 0x000029DC File Offset: 0x00000BDC
		public ushort SerializerTypeId
		{
			[CompilerGenerated]
			get
			{
				return this.<SerializerTypeId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<SerializerTypeId>k__BackingField = value;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000177 RID: 375 RVA: 0x000029E5 File Offset: 0x00000BE5
		// (set) Token: 0x06000178 RID: 376 RVA: 0x000029ED File Offset: 0x00000BED
		public byte[] Data
		{
			[CompilerGenerated]
			get
			{
				return this.<Data>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Data>k__BackingField = value;
			}
		}

		// Token: 0x06000179 RID: 377 RVA: 0x000099F4 File Offset: 0x00007BF4
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			long pos = reader.BaseStream.Position;
			this.AttributeId = reader.ReadUInt16();
			this.SerializerTypeId = reader.ReadUInt16();
			this.Data = reader.ReadBytes(size - (int)(reader.BaseStream.Position - pos));
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000029F6 File Offset: 0x00000BF6
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			writer.Write(this.SerializerTypeId);
			writer.Write(this.Data);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000027D4 File Offset: 0x000009D4
		public PropertyCustomRecord()
		{
		}

		// Token: 0x040000E5 RID: 229
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <AttributeId>k__BackingField;

		// Token: 0x040000E6 RID: 230
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <SerializerTypeId>k__BackingField;

		// Token: 0x040000E7 RID: 231
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private byte[] <Data>k__BackingField;
	}
}
