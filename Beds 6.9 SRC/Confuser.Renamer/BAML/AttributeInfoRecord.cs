using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000045 RID: 69
	internal class AttributeInfoRecord : SizedBamlRecord
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x00009C8C File Offset: 0x00007E8C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.AttributeInfo;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x00002BD8 File Offset: 0x00000DD8
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x00002BE0 File Offset: 0x00000DE0
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

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x00002BE9 File Offset: 0x00000DE9
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x00002BF1 File Offset: 0x00000DF1
		public ushort OwnerTypeId
		{
			[CompilerGenerated]
			get
			{
				return this.<OwnerTypeId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<OwnerTypeId>k__BackingField = value;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x00002BFA File Offset: 0x00000DFA
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x00002C02 File Offset: 0x00000E02
		public byte AttributeUsage
		{
			[CompilerGenerated]
			get
			{
				return this.<AttributeUsage>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<AttributeUsage>k__BackingField = value;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00002C0B File Offset: 0x00000E0B
		// (set) Token: 0x060001AB RID: 427 RVA: 0x00002C13 File Offset: 0x00000E13
		public string Name
		{
			[CompilerGenerated]
			get
			{
				return this.<Name>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Name>k__BackingField = value;
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00002C1C File Offset: 0x00000E1C
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.AttributeId = reader.ReadUInt16();
			this.OwnerTypeId = reader.ReadUInt16();
			this.AttributeUsage = reader.ReadByte();
			this.Name = reader.ReadString();
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00002C53 File Offset: 0x00000E53
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			writer.Write(this.OwnerTypeId);
			writer.Write(this.AttributeUsage);
			writer.Write(this.Name);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x000027D4 File Offset: 0x000009D4
		public AttributeInfoRecord()
		{
		}

		// Token: 0x040000F3 RID: 243
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <AttributeId>k__BackingField;

		// Token: 0x040000F4 RID: 244
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <OwnerTypeId>k__BackingField;

		// Token: 0x040000F5 RID: 245
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte <AttributeUsage>k__BackingField;

		// Token: 0x040000F6 RID: 246
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <Name>k__BackingField;
	}
}
