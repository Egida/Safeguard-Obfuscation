using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000037 RID: 55
	internal abstract class BamlRecord
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000130 RID: 304
		public abstract BamlRecordType Type { get; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00002787 File Offset: 0x00000987
		// (set) Token: 0x06000132 RID: 306 RVA: 0x0000278F File Offset: 0x0000098F
		public long Position
		{
			[CompilerGenerated]
			get
			{
				return this.<Position>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<Position>k__BackingField = value;
			}
		}

		// Token: 0x06000133 RID: 307
		public abstract void Read(BamlBinaryReader reader);

		// Token: 0x06000134 RID: 308
		public abstract void Write(BamlBinaryWriter writer);

		// Token: 0x06000135 RID: 309 RVA: 0x00002184 File Offset: 0x00000384
		protected BamlRecord()
		{
		}

		// Token: 0x040000D7 RID: 215
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private long <Position>k__BackingField;
	}
}
