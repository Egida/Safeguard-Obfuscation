using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003D RID: 61
	internal class AssemblyInfoRecord : SizedBamlRecord
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600015C RID: 348 RVA: 0x000099A4 File Offset: 0x00007BA4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.AssemblyInfo;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600015D RID: 349 RVA: 0x000028C0 File Offset: 0x00000AC0
		// (set) Token: 0x0600015E RID: 350 RVA: 0x000028C8 File Offset: 0x00000AC8
		public ushort AssemblyId
		{
			[CompilerGenerated]
			get
			{
				return this.<AssemblyId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<AssemblyId>k__BackingField = value;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600015F RID: 351 RVA: 0x000028D1 File Offset: 0x00000AD1
		// (set) Token: 0x06000160 RID: 352 RVA: 0x000028D9 File Offset: 0x00000AD9
		public string AssemblyFullName
		{
			[CompilerGenerated]
			get
			{
				return this.<AssemblyFullName>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<AssemblyFullName>k__BackingField = value;
			}
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000028E2 File Offset: 0x00000AE2
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.AssemblyId = reader.ReadUInt16();
			this.AssemblyFullName = reader.ReadString();
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000028FF File Offset: 0x00000AFF
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.AssemblyId);
			writer.Write(this.AssemblyFullName);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x000027D4 File Offset: 0x000009D4
		public AssemblyInfoRecord()
		{
		}

		// Token: 0x040000E0 RID: 224
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <AssemblyId>k__BackingField;

		// Token: 0x040000E1 RID: 225
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <AssemblyFullName>k__BackingField;
	}
}
