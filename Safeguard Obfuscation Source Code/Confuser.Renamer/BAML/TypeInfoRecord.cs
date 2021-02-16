using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000043 RID: 67
	internal class TypeInfoRecord : SizedBamlRecord
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00009C64 File Offset: 0x00007E64
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.TypeInfo;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000194 RID: 404 RVA: 0x00002B06 File Offset: 0x00000D06
		// (set) Token: 0x06000195 RID: 405 RVA: 0x00002B0E File Offset: 0x00000D0E
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

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000196 RID: 406 RVA: 0x00002B17 File Offset: 0x00000D17
		// (set) Token: 0x06000197 RID: 407 RVA: 0x00002B1F File Offset: 0x00000D1F
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

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000198 RID: 408 RVA: 0x00002B28 File Offset: 0x00000D28
		// (set) Token: 0x06000199 RID: 409 RVA: 0x00002B30 File Offset: 0x00000D30
		public string TypeFullName
		{
			[CompilerGenerated]
			get
			{
				return this.<TypeFullName>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<TypeFullName>k__BackingField = value;
			}
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00002B39 File Offset: 0x00000D39
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.TypeId = reader.ReadUInt16();
			this.AssemblyId = reader.ReadUInt16();
			this.TypeFullName = reader.ReadString();
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00002B63 File Offset: 0x00000D63
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.TypeId);
			writer.Write(this.AssemblyId);
			writer.Write(this.TypeFullName);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x000027D4 File Offset: 0x000009D4
		public TypeInfoRecord()
		{
		}

		// Token: 0x040000EF RID: 239
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ushort <TypeId>k__BackingField;

		// Token: 0x040000F0 RID: 240
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <AssemblyId>k__BackingField;

		// Token: 0x040000F1 RID: 241
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <TypeFullName>k__BackingField;
	}
}
