using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000056 RID: 86
	internal class PropertyWithStaticResourceIdRecord : StaticResourceIdRecord
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000219 RID: 537 RVA: 0x00009DE0 File Offset: 0x00007FE0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyWithStaticResourceId;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600021A RID: 538 RVA: 0x000030B9 File Offset: 0x000012B9
		// (set) Token: 0x0600021B RID: 539 RVA: 0x000030C1 File Offset: 0x000012C1
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

		// Token: 0x0600021C RID: 540 RVA: 0x000030CA File Offset: 0x000012CA
		public override void Read(BamlBinaryReader reader)
		{
			this.AttributeId = reader.ReadUInt16();
			base.Read(reader);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x000030E2 File Offset: 0x000012E2
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			base.Write(writer);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x000030FA File Offset: 0x000012FA
		public PropertyWithStaticResourceIdRecord()
		{
		}

		// Token: 0x0400010D RID: 269
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <AttributeId>k__BackingField;
	}
}
