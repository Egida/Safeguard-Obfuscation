using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005F RID: 95
	internal class PropertyComplexStartRecord : BamlRecord
	{
		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000244 RID: 580 RVA: 0x0000A038 File Offset: 0x00008238
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyComplexStart;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000245 RID: 581 RVA: 0x000031E7 File Offset: 0x000013E7
		// (set) Token: 0x06000246 RID: 582 RVA: 0x000031EF File Offset: 0x000013EF
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

		// Token: 0x06000247 RID: 583 RVA: 0x000031F8 File Offset: 0x000013F8
		public override void Read(BamlBinaryReader reader)
		{
			this.AttributeId = reader.ReadUInt16();
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00003208 File Offset: 0x00001408
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00002798 File Offset: 0x00000998
		public PropertyComplexStartRecord()
		{
		}

		// Token: 0x04000113 RID: 275
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <AttributeId>k__BackingField;
	}
}
