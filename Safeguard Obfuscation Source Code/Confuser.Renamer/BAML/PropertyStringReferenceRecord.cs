using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000055 RID: 85
	internal class PropertyStringReferenceRecord : PropertyComplexStartRecord
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000213 RID: 531 RVA: 0x00009DCC File Offset: 0x00007FCC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyStringReference;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000214 RID: 532 RVA: 0x00003078 File Offset: 0x00001278
		// (set) Token: 0x06000215 RID: 533 RVA: 0x00003080 File Offset: 0x00001280
		public ushort StringId
		{
			[CompilerGenerated]
			get
			{
				return this.<StringId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<StringId>k__BackingField = value;
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00003089 File Offset: 0x00001289
		public override void Read(BamlBinaryReader reader)
		{
			base.Read(reader);
			this.StringId = reader.ReadUInt16();
		}

		// Token: 0x06000217 RID: 535 RVA: 0x000030A1 File Offset: 0x000012A1
		public override void Write(BamlBinaryWriter writer)
		{
			base.Write(writer);
			writer.Write(this.StringId);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000306F File Offset: 0x0000126F
		public PropertyStringReferenceRecord()
		{
		}

		// Token: 0x0400010C RID: 268
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <StringId>k__BackingField;
	}
}
