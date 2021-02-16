using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000052 RID: 82
	internal class ConnectionIdRecord : BamlRecord
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060001FD RID: 509 RVA: 0x00009D90 File Offset: 0x00007F90
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ConnectionId;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060001FE RID: 510 RVA: 0x00002F76 File Offset: 0x00001176
		// (set) Token: 0x060001FF RID: 511 RVA: 0x00002F7E File Offset: 0x0000117E
		public uint ConnectionId
		{
			[CompilerGenerated]
			get
			{
				return this.<ConnectionId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<ConnectionId>k__BackingField = value;
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00002F87 File Offset: 0x00001187
		public override void Read(BamlBinaryReader reader)
		{
			this.ConnectionId = reader.ReadUInt32();
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00002F97 File Offset: 0x00001197
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.ConnectionId);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00002798 File Offset: 0x00000998
		public ConnectionIdRecord()
		{
		}

		// Token: 0x04000107 RID: 263
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint <ConnectionId>k__BackingField;
	}
}
