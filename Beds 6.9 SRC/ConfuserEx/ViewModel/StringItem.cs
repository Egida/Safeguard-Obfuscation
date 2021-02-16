using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200001E RID: 30
	public class StringItem : IViewModel<string>
	{
		// Token: 0x060000AF RID: 175 RVA: 0x00002468 File Offset: 0x00000668
		public StringItem(string item)
		{
			this.Item = item;
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x0000247A File Offset: 0x0000067A
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x00002482 File Offset: 0x00000682
		public string Item
		{
			[CompilerGenerated]
			get
			{
				return this.<Item>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Item>k__BackingField = value;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00004BD0 File Offset: 0x00002DD0
		string IViewModel<string>.Model
		{
			get
			{
				return this.Item;
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004BD0 File Offset: 0x00002DD0
		public override string ToString()
		{
			return this.Item;
		}

		// Token: 0x0400004D RID: 77
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Item>k__BackingField;
	}
}
