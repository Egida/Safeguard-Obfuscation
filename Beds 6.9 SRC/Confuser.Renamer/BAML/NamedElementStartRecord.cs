using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200006B RID: 107
	internal class NamedElementStartRecord : ElementStartRecord
	{
		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000286 RID: 646 RVA: 0x0000A128 File Offset: 0x00008328
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.NamedElementStart;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000287 RID: 647 RVA: 0x0000341A File Offset: 0x0000161A
		// (set) Token: 0x06000288 RID: 648 RVA: 0x00003422 File Offset: 0x00001622
		public string RuntimeName
		{
			[CompilerGenerated]
			get
			{
				return this.<RuntimeName>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<RuntimeName>k__BackingField = value;
			}
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000342B File Offset: 0x0000162B
		public override void Read(BamlBinaryReader reader)
		{
			base.TypeId = reader.ReadUInt16();
			this.RuntimeName = reader.ReadString();
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000A13C File Offset: 0x0000833C
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(base.TypeId);
			bool flag = this.RuntimeName != null;
			if (flag)
			{
				writer.Write(this.RuntimeName);
			}
		}

		// Token: 0x0600028B RID: 651 RVA: 0x000032F7 File Offset: 0x000014F7
		public NamedElementStartRecord()
		{
		}

		// Token: 0x0400011E RID: 286
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <RuntimeName>k__BackingField;
	}
}
