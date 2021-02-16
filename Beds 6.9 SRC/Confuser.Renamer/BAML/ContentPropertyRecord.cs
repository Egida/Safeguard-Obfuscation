using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000057 RID: 87
	internal class ContentPropertyRecord : BamlRecord
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600021F RID: 543 RVA: 0x00009DF4 File Offset: 0x00007FF4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ContentProperty;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000220 RID: 544 RVA: 0x00003103 File Offset: 0x00001303
		// (set) Token: 0x06000221 RID: 545 RVA: 0x0000310B File Offset: 0x0000130B
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

		// Token: 0x06000222 RID: 546 RVA: 0x00003114 File Offset: 0x00001314
		public override void Read(BamlBinaryReader reader)
		{
			this.AttributeId = reader.ReadUInt16();
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00003124 File Offset: 0x00001324
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00002798 File Offset: 0x00000998
		public ContentPropertyRecord()
		{
		}

		// Token: 0x0400010E RID: 270
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <AttributeId>k__BackingField;
	}
}
