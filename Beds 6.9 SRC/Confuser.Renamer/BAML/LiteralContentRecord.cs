using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004A RID: 74
	internal class LiteralContentRecord : SizedBamlRecord
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x00009CF0 File Offset: 0x00007EF0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.LiteralContent;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001CA RID: 458 RVA: 0x00002D93 File Offset: 0x00000F93
		// (set) Token: 0x060001CB RID: 459 RVA: 0x00002D9B File Offset: 0x00000F9B
		public string Value
		{
			[CompilerGenerated]
			get
			{
				return this.<Value>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Value>k__BackingField = value;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001CC RID: 460 RVA: 0x00002DA4 File Offset: 0x00000FA4
		// (set) Token: 0x060001CD RID: 461 RVA: 0x00002DAC File Offset: 0x00000FAC
		public uint Reserved0
		{
			[CompilerGenerated]
			get
			{
				return this.<Reserved0>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Reserved0>k__BackingField = value;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060001CE RID: 462 RVA: 0x00002DB5 File Offset: 0x00000FB5
		// (set) Token: 0x060001CF RID: 463 RVA: 0x00002DBD File Offset: 0x00000FBD
		public uint Reserved1
		{
			[CompilerGenerated]
			get
			{
				return this.<Reserved1>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Reserved1>k__BackingField = value;
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00002DC6 File Offset: 0x00000FC6
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Value = reader.ReadString();
			this.Reserved0 = reader.ReadUInt32();
			this.Reserved1 = reader.ReadUInt32();
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00002DF0 File Offset: 0x00000FF0
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
			writer.Write(this.Reserved0);
			writer.Write(this.Reserved1);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x000027D4 File Offset: 0x000009D4
		public LiteralContentRecord()
		{
		}

		// Token: 0x040000FC RID: 252
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <Value>k__BackingField;

		// Token: 0x040000FD RID: 253
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private uint <Reserved0>k__BackingField;

		// Token: 0x040000FE RID: 254
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private uint <Reserved1>k__BackingField;
	}
}
