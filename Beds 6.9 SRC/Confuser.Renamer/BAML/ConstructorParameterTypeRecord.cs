using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000063 RID: 99
	internal class ConstructorParameterTypeRecord : BamlRecord
	{
		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000A088 File Offset: 0x00008288
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ConstructorParameterType;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000257 RID: 599 RVA: 0x00003218 File Offset: 0x00001418
		// (set) Token: 0x06000258 RID: 600 RVA: 0x00003220 File Offset: 0x00001420
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

		// Token: 0x06000259 RID: 601 RVA: 0x00003229 File Offset: 0x00001429
		public override void Read(BamlBinaryReader reader)
		{
			this.TypeId = reader.ReadUInt16();
		}

		// Token: 0x0600025A RID: 602 RVA: 0x00003239 File Offset: 0x00001439
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.TypeId);
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00002798 File Offset: 0x00000998
		public ConstructorParameterTypeRecord()
		{
		}

		// Token: 0x04000114 RID: 276
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <TypeId>k__BackingField;
	}
}
