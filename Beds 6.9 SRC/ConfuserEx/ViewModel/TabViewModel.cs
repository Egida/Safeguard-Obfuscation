using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200002D RID: 45
	public abstract class TabViewModel : ViewModelBase
	{
		// Token: 0x06000170 RID: 368 RVA: 0x00002B34 File Offset: 0x00000D34
		protected TabViewModel(AppVM app, string header)
		{
			this.App = app;
			this.Header = header;
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000171 RID: 369 RVA: 0x00002B4E File Offset: 0x00000D4E
		// (set) Token: 0x06000172 RID: 370 RVA: 0x00002B56 File Offset: 0x00000D56
		public AppVM App
		{
			[CompilerGenerated]
			get
			{
				return this.<App>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<App>k__BackingField = value;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00002B5F File Offset: 0x00000D5F
		// (set) Token: 0x06000174 RID: 372 RVA: 0x00002B67 File Offset: 0x00000D67
		public string Header
		{
			[CompilerGenerated]
			get
			{
				return this.<Header>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Header>k__BackingField = value;
			}
		}

		// Token: 0x04000084 RID: 132
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AppVM <App>k__BackingField;

		// Token: 0x04000085 RID: 133
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Header>k__BackingField;
	}
}
