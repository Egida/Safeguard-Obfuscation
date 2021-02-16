using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Xml;
using Confuser.Core.Project;
using ConfuserEx.ViewModel;

namespace ConfuserEx
{
	// Token: 0x02000016 RID: 22
	public class MainWindow : Window, IComponentConnector
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00003EE8 File Offset: 0x000020E8
		public MainWindow()
		{
			this.InitializeComponent();
			AppVM appVM = new AppVM();
			appVM.Project = new ProjectVM(new ConfuserProject(), null);
			appVM.FileName = "SafeGuard Obfuscator";
			appVM.Tabs.Add(new ProjectTabVM(appVM));
			appVM.Tabs.Add(new SettingsTabVM(appVM));
			appVM.Tabs.Add(new ProtectTabVM(appVM));
			appVM.Tabs.Add(new AboutTabVM(appVM));
			this.LoadProj(appVM);
			base.DataContext = appVM;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003F78 File Offset: 0x00002178
		private void OpenMenu(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			ContextMenu contextMenu = button.ContextMenu;
			contextMenu.PlacementTarget = button;
			contextMenu.Placement = PlacementMode.MousePoint;
			contextMenu.IsOpen = true;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003FA8 File Offset: 0x000021A8
		private void LoadProj(AppVM app)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs.Length == 2 && File.Exists(commandLineArgs[1]))
			{
				string fullPath = Path.GetFullPath(commandLineArgs[1]);
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(fullPath);
					ConfuserProject confuserProject = new ConfuserProject();
					confuserProject.Load(xmlDocument);
					app.Project = new ProjectVM(confuserProject, fullPath);
					app.FileName = fullPath;
				}
				catch
				{
					MessageBox.Show("Invalid project!", "ConfuserEx", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002298 File Offset: 0x00000498
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			e.Cancel = !((AppVM)base.DataContext).OnWindowClosing();
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000022BA File Offset: 0x000004BA
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004030 File Offset: 0x00002230
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!this._contentLoaded)
			{
				this._contentLoaded = true;
				Uri resourceLocator = new Uri("/ConfuserEx;component/mainwindow1.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000022BC File Offset: 0x000004BC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00004060 File Offset: 0x00002260
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((Button)target).Click += this.OpenMenu;
				return;
			}
			if (connectionId != 2)
			{
				this._contentLoaded = true;
				return;
			}
			((TabControl)target).SelectionChanged += this.TabControl_SelectionChanged;
		}

		// Token: 0x0400002F RID: 47
		private bool _contentLoaded;
	}
}
