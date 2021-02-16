using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000054 RID: 84
	internal class PropertyTypeReferenceRecord : PropertyComplexStartRecord
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600020D RID: 525 RVA: 0x00009DB8 File Offset: 0x00007FB8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyTypeReference;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0000302E File Offset: 0x0000122E
		// (set) Token: 0x0600020F RID: 527 RVA: 0x00003036 File Offset: 0x00001236
		public ushort TypeId
		{
			[CompilerGenerated]
			get
			{
				return this.<TypeId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<TypeId>k__BackingField = value;
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000303F File Offset: 0x0000123F
		public override void Read(BamlBinaryReader reader)
		{
			base.Read(reader);
			this.TypeId = reader.ReadUInt16();
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00003057 File Offset: 0x00001257
		public override void Write(BamlBinaryWriter writer)
		{
			base.Write(writer);
			writer.Write(this.TypeId);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000306F File Offset: 0x0000126F
		public PropertyTypeReferenceRecord()
		{
		}

		// Token: 0x0400010B RID: 267
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <TypeId>k__BackingField;
	}
}
