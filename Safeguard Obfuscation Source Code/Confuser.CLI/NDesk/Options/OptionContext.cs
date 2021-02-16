using System;

namespace NDesk.Options
{
	// Token: 0x02000004 RID: 4
	public class OptionContext
	{
		// Token: 0x06000028 RID: 40 RVA: 0x00002A1F File Offset: 0x00000C1F
		public OptionContext(OptionSet set)
		{
			this.set = set;
			this.c = new OptionValueCollection(this);
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002A3C File Offset: 0x00000C3C
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002A54 File Offset: 0x00000C54
		public Option Option
		{
			get
			{
				return this.option;
			}
			set
			{
				this.option = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002A60 File Offset: 0x00000C60
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002A78 File Offset: 0x00000C78
		public string OptionName
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002A84 File Offset: 0x00000C84
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002A9C File Offset: 0x00000C9C
		public int OptionIndex
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002AA8 File Offset: 0x00000CA8
		public OptionSet OptionSet
		{
			get
			{
				return this.set;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002AC0 File Offset: 0x00000CC0
		public OptionValueCollection OptionValues
		{
			get
			{
				return this.c;
			}
		}

		// Token: 0x04000003 RID: 3
		private Option option;

		// Token: 0x04000004 RID: 4
		private string name;

		// Token: 0x04000005 RID: 5
		private int index;

		// Token: 0x04000006 RID: 6
		private OptionSet set;

		// Token: 0x04000007 RID: 7
		private OptionValueCollection c;
	}
}
