using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000053 RID: 83
	internal class PropertyWithExtensionRecord : BamlRecord
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000203 RID: 515 RVA: 0x00009DA4 File Offset: 0x00007FA4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyWithExtension;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000204 RID: 516 RVA: 0x00002FA7 File Offset: 0x000011A7
		// (set) Token: 0x06000205 RID: 517 RVA: 0x00002FAF File Offset: 0x000011AF
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

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000206 RID: 518 RVA: 0x00002FB8 File Offset: 0x000011B8
		// (set) Token: 0x06000207 RID: 519 RVA: 0x00002FC0 File Offset: 0x000011C0
		public ushort Flags
		{
			[CompilerGenerated]
			get
			{
				return this.<Flags>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Flags>k__BackingField = value;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000208 RID: 520 RVA: 0x00002FC9 File Offset: 0x000011C9
		// (set) Token: 0x06000209 RID: 521 RVA: 0x00002FD1 File Offset: 0x000011D1
		public ushort ValueId
		{
			[CompilerGenerated]
			get
			{
				return this.<ValueId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<ValueId>k__BackingField = value;
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00002FDA File Offset: 0x000011DA
		public override void Read(BamlBinaryReader reader)
		{
			this.AttributeId = reader.ReadUInt16();
			this.Flags = reader.ReadUInt16();
			this.ValueId = reader.ReadUInt16();
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00003004 File Offset: 0x00001204
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			writer.Write(this.Flags);
			writer.Write(this.ValueId);
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00002798 File Offset: 0x00000998
		public PropertyWithExtensionRecord()
		{
		}

		// Token: 0x04000108 RID: 264
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <AttributeId>k__BackingField;

		// Token: 0x04000109 RID: 265
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <Flags>k__BackingField;

		// Token: 0x0400010A RID: 266
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <ValueId>k__BackingField;
	}
}
