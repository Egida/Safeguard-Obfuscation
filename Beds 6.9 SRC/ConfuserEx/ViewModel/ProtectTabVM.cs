using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Confuser.Core;
using Confuser.Core.Project;
using GalaSoft.MvvmLight.Command;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200002A RID: 42
	internal class ProtectTabVM : TabViewModel, ILogger
	{
		// Token: 0x0600013C RID: 316 RVA: 0x00005DA4 File Offset: 0x00003FA4
		public ProtectTabVM(AppVM app) : base(app, "Protect!")
		{
			this.documentContent = new Paragraph();
			this.LogDocument = new FlowDocument();
			this.LogDocument.Blocks.Add(this.documentContent);
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00005E04 File Offset: 0x00004004
		public ICommand ProtectCmd
		{
			get
			{
				return new RelayCommand(new Action(this.DoProtect), () => !base.App.NavigationDisabled);
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600013E RID: 318 RVA: 0x00005E34 File Offset: 0x00004034
		public ICommand CancelCmd
		{
			get
			{
				return new RelayCommand(new Action(this.DoCancel), () => base.App.NavigationDisabled);
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600013F RID: 319 RVA: 0x00005E64 File Offset: 0x00004064
		// (set) Token: 0x06000140 RID: 320 RVA: 0x00002866 File Offset: 0x00000A66
		public double? Progress
		{
			get
			{
				return this.progress;
			}
			set
			{
				base.SetProperty<double?>(ref this.progress, value, "Progress");
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000141 RID: 321 RVA: 0x0000287C File Offset: 0x00000A7C
		// (set) Token: 0x06000142 RID: 322 RVA: 0x00002884 File Offset: 0x00000A84
		public FlowDocument LogDocument
		{
			[CompilerGenerated]
			get
			{
				return this.<LogDocument>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<LogDocument>k__BackingField = value;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00005E7C File Offset: 0x0000407C
		// (set) Token: 0x06000144 RID: 324 RVA: 0x0000288D File Offset: 0x00000A8D
		public bool? Result
		{
			get
			{
				return this.result;
			}
			set
			{
				base.SetProperty<bool?>(ref this.result, value, "Result");
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00005E94 File Offset: 0x00004094
		private void DoProtect()
		{
			ConfuserParameters confuserParameters = new ConfuserParameters();
			confuserParameters.Project = ((IViewModel<ConfuserProject>)base.App.Project).Model;
			bool flag = File.Exists(base.App.FileName);
			if (flag)
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(base.App.FileName);
			}
			confuserParameters.Logger = this;
			this.documentContent.Inlines.Clear();
			this.cancelSrc = new CancellationTokenSource();
			this.Result = null;
			this.Progress = null;
			this.begin = DateTime.Now;
			base.App.NavigationDisabled = true;
			ConfuserEngine.Run(confuserParameters, new CancellationToken?(this.cancelSrc.Token)).ContinueWith<DispatcherOperation>((Task _) => Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.Progress = new double?(0.0);
				base.App.NavigationDisabled = false;
				CommandManager.InvalidateRequerySuggested();
			}), new object[0]));
		}

		// Token: 0x06000146 RID: 326 RVA: 0x000028A3 File Offset: 0x00000AA3
		private void DoCancel()
		{
			this.cancelSrc.Cancel();
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00005F70 File Offset: 0x00004170
		private void AppendLine(string format, Brush foreground, params object[] args)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.documentContent.Inlines.Add(new Run(string.Format(format, args))
				{
					Foreground = foreground
				});
				this.documentContent.Inlines.Add(new LineBreak());
			}), new object[0]);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x000028B2 File Offset: 0x00000AB2
		void ILogger.Debug(string msg)
		{
			this.AppendLine("[DEBUG] {0}", Brushes.Gray, new object[]
			{
				msg
			});
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000028D0 File Offset: 0x00000AD0
		void ILogger.DebugFormat(string format, params object[] args)
		{
			this.AppendLine("[DEBUG] {0}", Brushes.Gray, new object[]
			{
				string.Format(format, args)
			});
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000028F4 File Offset: 0x00000AF4
		void ILogger.Info(string msg)
		{
			this.AppendLine(" [INFO] {0}", Brushes.White, new object[]
			{
				msg
			});
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00002912 File Offset: 0x00000B12
		void ILogger.InfoFormat(string format, params object[] args)
		{
			this.AppendLine(" [INFO] {0}", Brushes.White, new object[]
			{
				string.Format(format, args)
			});
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00002936 File Offset: 0x00000B36
		void ILogger.Warn(string msg)
		{
			this.AppendLine(" [WARN] {0}", Brushes.Yellow, new object[]
			{
				msg
			});
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00002954 File Offset: 0x00000B54
		void ILogger.WarnFormat(string format, params object[] args)
		{
			this.AppendLine(" [WARN] {0}", Brushes.Yellow, new object[]
			{
				string.Format(format, args)
			});
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00002978 File Offset: 0x00000B78
		void ILogger.WarnException(string msg, Exception ex)
		{
			this.AppendLine(" [WARN] {0}", Brushes.Yellow, new object[]
			{
				msg
			});
			this.AppendLine("Exception: {0}", Brushes.Yellow, new object[]
			{
				ex
			});
		}

		// Token: 0x0600014F RID: 335 RVA: 0x000029B1 File Offset: 0x00000BB1
		void ILogger.Error(string msg)
		{
			this.AppendLine("[ERROR] {0}", Brushes.Red, new object[]
			{
				msg
			});
		}

		// Token: 0x06000150 RID: 336 RVA: 0x000029CF File Offset: 0x00000BCF
		void ILogger.ErrorFormat(string format, params object[] args)
		{
			this.AppendLine("[ERROR] {0}", Brushes.Red, new object[]
			{
				string.Format(format, args)
			});
		}

		// Token: 0x06000151 RID: 337 RVA: 0x000029F3 File Offset: 0x00000BF3
		void ILogger.ErrorException(string msg, Exception ex)
		{
			this.AppendLine("[ERROR] {0}", Brushes.Red, new object[]
			{
				msg
			});
			this.AppendLine("Exception: {0}", Brushes.Red, new object[]
			{
				ex
			});
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00002A2C File Offset: 0x00000C2C
		void ILogger.Progress(int progress, int overall)
		{
			this.Progress = new double?((double)progress / (double)overall);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00005FC4 File Offset: 0x000041C4
		void ILogger.EndProgress()
		{
			this.Progress = null;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00005FE4 File Offset: 0x000041E4
		void ILogger.Finish(bool successful)
		{
			DateTime now = DateTime.Now;
			string text = string.Format("at {0}, {1}:{2:d2} elapsed.", now.ToShortTimeString(), (int)now.Subtract(this.begin).TotalMinutes, now.Subtract(this.begin).Seconds);
			if (successful)
			{
				this.AppendLine("Finished {0}", Brushes.Lime, new object[]
				{
					text
				});
			}
			else
			{
				this.AppendLine("Failed {0}", Brushes.Red, new object[]
				{
					text
				});
			}
			this.Result = new bool?(successful);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00002A40 File Offset: 0x00000C40
		[CompilerGenerated]
		private bool <get_ProtectCmd>b__6_0()
		{
			return !base.App.NavigationDisabled;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00002A50 File Offset: 0x00000C50
		[CompilerGenerated]
		private bool <get_CancelCmd>b__8_0()
		{
			return base.App.NavigationDisabled;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00002A5D File Offset: 0x00000C5D
		[CompilerGenerated]
		private DispatcherOperation <DoProtect>b__19_0(Task _)
		{
			return Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.Progress = new double?(0.0);
				base.App.NavigationDisabled = false;
				CommandManager.InvalidateRequerySuggested();
			}), new object[0]);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00002A80 File Offset: 0x00000C80
		[CompilerGenerated]
		private void <DoProtect>b__19_1()
		{
			this.Progress = new double?(0.0);
			base.App.NavigationDisabled = false;
			CommandManager.InvalidateRequerySuggested();
		}

		// Token: 0x04000076 RID: 118
		private readonly Paragraph documentContent;

		// Token: 0x04000077 RID: 119
		private CancellationTokenSource cancelSrc;

		// Token: 0x04000078 RID: 120
		private double? progress = new double?(0.0);

		// Token: 0x04000079 RID: 121
		private bool? result;

		// Token: 0x0400007A RID: 122
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private FlowDocument <LogDocument>k__BackingField;

		// Token: 0x0400007B RID: 123
		private DateTime begin;

		// Token: 0x0200002B RID: 43
		[CompilerGenerated]
		private sealed class <>c__DisplayClass21_0
		{
			// Token: 0x06000159 RID: 345 RVA: 0x00002119 File Offset: 0x00000319
			public <>c__DisplayClass21_0()
			{
			}

			// Token: 0x0600015A RID: 346 RVA: 0x0000608C File Offset: 0x0000428C
			internal void <AppendLine>b__0()
			{
				this.<>4__this.documentContent.Inlines.Add(new Run(string.Format(this.format, this.args))
				{
					Foreground = this.foreground
				});
				this.<>4__this.documentContent.Inlines.Add(new LineBreak());
			}

			// Token: 0x0400007C RID: 124
			public string format;

			// Token: 0x0400007D RID: 125
			public object[] args;

			// Token: 0x0400007E RID: 126
			public Brush foreground;

			// Token: 0x0400007F RID: 127
			public ProtectTabVM <>4__this;
		}
	}
}
