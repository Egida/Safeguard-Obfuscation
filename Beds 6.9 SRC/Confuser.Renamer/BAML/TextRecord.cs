using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000047 RID: 71
	internal class TextRecord : SizedBamlRecord
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00009CB4 File Offset: 0x00007EB4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.Text;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x00002CE6 File Offset: 0x00000EE6
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x00002CEE File Offset: 0x00000EEE
		public string Value
		{
			[CompilerGenerated]
			get
			{
				return this.<Value>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Value>k__BackingField = value;
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00002CF7 File Offset: 0x00000EF7
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Value = reader.ReadString();
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00002D07 File Offset: 0x00000F07
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x000027D4 File Offset: 0x000009D4
		public TextRecord()
		{
		}

		// Token: 0x040000F9 RID: 249
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Value>k__BackingField;
	}
}
