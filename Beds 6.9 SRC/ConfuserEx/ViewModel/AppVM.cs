using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Confuser.Core;
using Confuser.Core.Project;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000021 RID: 33
	public class AppVM : ViewModelBase
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00004C80 File Offset: 0x00002E80
		// (set) Token: 0x060000BD RID: 189 RVA: 0x000024C4 File Offset: 0x000006C4
		public bool NavigationDisabled
		{
			get
			{
				return this.navDisabled;
			}
			set
			{
				base.SetProperty<bool>(ref this.navDisabled, value, "NavigationDisabled");
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00004C98 File Offset: 0x00002E98
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00004CB0 File Offset: 0x00002EB0
		public ProjectVM Project
		{
			get
			{
				return this.proj;
			}
			set
			{
				bool flag = this.proj != null;
				if (flag)
				{
					this.proj.PropertyChanged -= this.OnProjectPropertyChanged;
				}
				base.SetProperty<ProjectVM>(ref this.proj, value, "Project");
				bool flag2 = this.proj != null;
				if (flag2)
				{
					this.proj.PropertyChanged += this.OnProjectPropertyChanged;
				}
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00004D1C File Offset: 0x00002F1C
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x000024DA File Offset: 0x000006DA
		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				base.SetProperty<string>(ref this.fileName, value, "Project");
				this.OnPropertyChanged("Title");
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00004D34 File Offset: 0x00002F34
		public string Title
		{
			get
			{
				return string.Format("{0}{1} - {2}", Path.GetFileName(this.fileName), this.proj.IsModified ? "*" : "", ConfuserEngine.Version);
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00004D7C File Offset: 0x00002F7C
		public IList<TabViewModel> Tabs
		{
			get
			{
				return this.tabs;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00004D94 File Offset: 0x00002F94
		public ICommand NewProject
		{
			get
			{
				return new RelayCommand(new Action(this.NewProj), () => !this.NavigationDisabled);
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00004DC4 File Offset: 0x00002FC4
		public ICommand OpenProject
		{
			get
			{
				return new RelayCommand(new Action(this.OpenProj), () => !this.NavigationDisabled);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00004DF4 File Offset: 0x00002FF4
		public ICommand SaveProject
		{
			get
			{
				return new RelayCommand(delegate()
				{
					this.SaveProj();
				}, () => !this.NavigationDisabled);
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00004E24 File Offset: 0x00003024
		public ICommand Decode
		{
			get
			{
				return new RelayCommand(delegate()
				{
					new StackTraceDecoder
					{
						Owner = Application.Current.MainWindow
					}.ShowDialog();
				}, () => !this.NavigationDisabled);
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00004E68 File Offset: 0x00003068
		public bool OnWindowClosing()
		{
			return this.PromptSave();
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00004E80 File Offset: 0x00003080
		private bool SaveProj()
		{
			bool flag = !this.firstSaved || !File.Exists(this.FileName);
			if (flag)
			{
				VistaSaveFileDialog vistaSaveFileDialog = new VistaSaveFileDialog();
				vistaSaveFileDialog.FileName = this.FileName;
				vistaSaveFileDialog.Filter = "ConfuserEx Projects (*.crproj)|*.crproj|All Files (*.*)|*.*";
				vistaSaveFileDialog.DefaultExt = ".crproj";
				vistaSaveFileDialog.AddExtension = true;
				bool flag2 = !(vistaSaveFileDialog.ShowDialog(Application.Current.MainWindow) ?? false) || vistaSaveFileDialog.FileName == null;
				if (flag2)
				{
					return false;
				}
				this.FileName = vistaSaveFileDialog.FileName;
			}
			ConfuserProject model = ((IViewModel<ConfuserProject>)this.Project).Model;
			model.Save().Save(this.FileName);
			this.Project.IsModified = false;
			this.firstSaved = true;
			return true;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00004F64 File Offset: 0x00003164
		private bool PromptSave()
		{
			bool flag = !this.Project.IsModified;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				MessageBoxResult messageBoxResult = MessageBox.Show("The current project has unsaved changes. Do you want to save them?", "ConfuserEx", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				if (messageBoxResult != MessageBoxResult.Cancel)
				{
					if (messageBoxResult != MessageBoxResult.Yes)
					{
						result = (messageBoxResult == MessageBoxResult.No);
					}
					else
					{
						result = this.SaveProj();
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00004FC4 File Offset: 0x000031C4
		private void NewProj()
		{
			bool flag = !this.PromptSave();
			if (!flag)
			{
				this.Project = new ProjectVM(new ConfuserProject(), null);
				this.FileName = "Unnamed.crproj";
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005000 File Offset: 0x00003200
		private void OpenProj()
		{
			bool flag = !this.PromptSave();
			if (!flag)
			{
				VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
				vistaOpenFileDialog.Filter = "ConfuserEx Projects (*.crproj)|*.crproj|All Files (*.*)|*.*";
				bool flag2 = (vistaOpenFileDialog.ShowDialog(Application.Current.MainWindow) ?? false) && vistaOpenFileDialog.FileName != null;
				if (flag2)
				{
					string filename = vistaOpenFileDialog.FileName;
					try
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(filename);
						ConfuserProject confuserProject = new ConfuserProject();
						confuserProject.Load(xmlDocument);
						this.Project = new ProjectVM(confuserProject, filename);
						this.FileName = filename;
					}
					catch
					{
						MessageBox.Show("Invalid project!", "ConfuserEx", MessageBoxButton.OK, MessageBoxImage.Hand);
					}
				}
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000050DC File Offset: 0x000032DC
		private void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			bool flag = e.PropertyName == "IsModified";
			if (flag)
			{
				this.OnPropertyChanged("Title");
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000510C File Offset: 0x0000330C
		protected override void OnPropertyChanged(string property)
		{
			base.OnPropertyChanged(property);
			bool flag = property == "Project";
			if (flag)
			{
				this.LoadPlugins();
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00005138 File Offset: 0x00003338
		private void LoadPlugins()
		{
			foreach (StringItem stringItem in this.Project.Plugins)
			{
				try
				{
					ComponentDiscovery.LoadComponents(this.Project.Protections, this.Project.Packers, stringItem.Item);
				}
				catch
				{
					MessageBox.Show("Failed to load plugin '" + stringItem + "'.");
				}
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000024FC File Offset: 0x000006FC
		public AppVM()
		{
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00002510 File Offset: 0x00000710
		[CompilerGenerated]
		private bool <get_NewProject>b__19_0()
		{
			return !this.NavigationDisabled;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00002510 File Offset: 0x00000710
		[CompilerGenerated]
		private bool <get_OpenProject>b__21_0()
		{
			return !this.NavigationDisabled;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000251B File Offset: 0x0000071B
		[CompilerGenerated]
		private void <get_SaveProject>b__23_0()
		{
			this.SaveProj();
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00002510 File Offset: 0x00000710
		[CompilerGenerated]
		private bool <get_SaveProject>b__23_1()
		{
			return !this.NavigationDisabled;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00002510 File Offset: 0x00000710
		[CompilerGenerated]
		private bool <get_Decode>b__25_1()
		{
			return !this.NavigationDisabled;
		}

		// Token: 0x04000052 RID: 82
		private readonly IList<TabViewModel> tabs = new ObservableCollection<TabViewModel>();

		// Token: 0x04000053 RID: 83
		private string fileName;

		// Token: 0x04000054 RID: 84
		private bool navDisabled;

		// Token: 0x04000055 RID: 85
		private bool firstSaved;

		// Token: 0x04000056 RID: 86
		private ProjectVM proj;

		// Token: 0x02000022 RID: 34
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060000D6 RID: 214 RVA: 0x00002524 File Offset: 0x00000724
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060000D7 RID: 215 RVA: 0x00002119 File Offset: 0x00000319
			public <>c()
			{
			}

			// Token: 0x060000D8 RID: 216 RVA: 0x00002530 File Offset: 0x00000730
			internal void <get_Decode>b__25_0()
			{
				new StackTraceDecoder
				{
					Owner = Application.Current.MainWindow
				}.ShowDialog();
			}

			// Token: 0x04000057 RID: 87
			public static readonly AppVM.<>c <>9 = new AppVM.<>c();

			// Token: 0x04000058 RID: 88
			public static Action <>9__25_0;
		}
	}
}
