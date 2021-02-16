using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200001F RID: 31
	internal class AboutTabVM : TabViewModel
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x00004BE8 File Offset: 0x00002DE8
		public AboutTabVM(AppVM app) : base(app, "About")
		{
			IconBitmapDecoder iconBitmapDecoder = new IconBitmapDecoder(new Uri("pack://application:,,,/ConfuserEx.ico"), BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
			this.Icon = iconBitmapDecoder.Frames.First((BitmapFrame frame) => frame.Width == 64.0);
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00004C48 File Offset: 0x00002E48
		public ICommand LaunchBrowser
		{
			get
			{
				return new RelayCommand<string>(delegate(string site)
				{
					Process.Start(site);
				});
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x0000248B File Offset: 0x0000068B
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00002493 File Offset: 0x00000693
		public BitmapSource Icon
		{
			[CompilerGenerated]
			get
			{
				return this.<Icon>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Icon>k__BackingField = value;
			}
		}

		// Token: 0x0400004E RID: 78
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BitmapSource <Icon>k__BackingField;

		// Token: 0x02000020 RID: 32
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060000B8 RID: 184 RVA: 0x0000249C File Offset: 0x0000069C
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060000B9 RID: 185 RVA: 0x00002119 File Offset: 0x00000319
			public <>c()
			{
			}

			// Token: 0x060000BA RID: 186 RVA: 0x000024A8 File Offset: 0x000006A8
			internal bool <.ctor>b__0_0(BitmapFrame frame)
			{
				return frame.Width == 64.0;
			}

			// Token: 0x060000BB RID: 187 RVA: 0x000024BB File Offset: 0x000006BB
			internal void <get_LaunchBrowser>b__2_0(string site)
			{
				Process.Start(site);
			}

			// Token: 0x0400004F RID: 79
			public static readonly AboutTabVM.<>c <>9 = new AboutTabVM.<>c();

			// Token: 0x04000050 RID: 80
			public static Func<BitmapFrame, bool> <>9__0_0;

			// Token: 0x04000051 RID: 81
			public static Action<string> <>9__2_0;
		}
	}
}
