using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Confuser.Core;
using Confuser.Core.Project;
using ConfuserEx.Views;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000026 RID: 38
	public class ProjectTabVM : TabViewModel
	{
		// Token: 0x060000F3 RID: 243 RVA: 0x0000260C File Offset: 0x0000080C
		public ProjectTabVM(AppVM app) : base(app, "Project")
		{
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000054B8 File Offset: 0x000036B8
		public ICommand DragDrop
		{
			get
			{
				return new RelayCommand<IDataObject>(delegate(IDataObject data)
				{
					foreach (string file in (string[])data.GetData(DataFormats.FileDrop))
					{
						this.AddModule(file);
					}
				}, delegate(IDataObject data)
				{
					bool flag = !data.GetDataPresent(DataFormats.FileDrop);
					bool result;
					if (flag)
					{
						result = false;
					}
					else
					{
						string[] source = (string[])data.GetData(DataFormats.FileDrop);
						bool flag2 = source.All((string file) => File.Exists(file));
						result = flag2;
					}
					return result;
				});
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x000054FC File Offset: 0x000036FC
		public ICommand ChooseBaseDir
		{
			get
			{
				return new RelayCommand(delegate()
				{
					VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
					vistaFolderBrowserDialog.SelectedPath = base.App.Project.BaseDirectory;
					bool flag = vistaFolderBrowserDialog.ShowDialog() ?? false;
					if (flag)
					{
						base.App.Project.BaseDirectory = vistaFolderBrowserDialog.SelectedPath;
						base.App.Project.OutputDirectory = Path.Combine(base.App.Project.BaseDirectory, "Confused");
					}
				});
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00005520 File Offset: 0x00003720
		public ICommand ChooseOutputDir
		{
			get
			{
				return new RelayCommand(delegate()
				{
					VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
					vistaFolderBrowserDialog.SelectedPath = base.App.Project.OutputDirectory;
					bool flag = vistaFolderBrowserDialog.ShowDialog() ?? false;
					if (flag)
					{
						base.App.Project.OutputDirectory = vistaFolderBrowserDialog.SelectedPath;
					}
				});
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00005544 File Offset: 0x00003744
		public ICommand Add
		{
			get
			{
				return new RelayCommand(delegate()
				{
					VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
					vistaOpenFileDialog.Filter = ".NET assemblies (*.exe, *.dll)|*.exe;*.dll|All Files (*.*)|*.*";
					vistaOpenFileDialog.Multiselect = true;
					bool flag = vistaOpenFileDialog.ShowDialog() ?? false;
					if (flag)
					{
						foreach (string file in vistaOpenFileDialog.FileNames)
						{
							this.AddModule(file);
						}
					}
				});
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00005568 File Offset: 0x00003768
		public ICommand Remove
		{
			get
			{
				return new RelayCommand(delegate()
				{
					Debug.Assert(base.App.Project.Modules.Any((ProjectModuleVM m) => m.IsSelected));
					string messageBoxText = "Are you sure to remove selected modules?\r\nAll settings specific to it would be lost!";
					bool flag = MessageBox.Show(messageBoxText, "ConfuserEx", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
					if (flag)
					{
						foreach (ProjectModuleVM item in (from m in base.App.Project.Modules
						where m.IsSelected
						select m).ToList<ProjectModuleVM>())
						{
							base.App.Project.Modules.Remove(item);
						}
					}
				}, () => base.App.Project.Modules.Any((ProjectModuleVM m) => m.IsSelected));
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00005598 File Offset: 0x00003798
		public ICommand Edit
		{
			get
			{
				return new RelayCommand(delegate()
				{
					Debug.Assert(base.App.Project.Modules.Count((ProjectModuleVM m) => m.IsSelected) == 1);
					new ProjectModuleView(base.App.Project.Modules.Single((ProjectModuleVM m) => m.IsSelected))
					{
						Owner = Application.Current.MainWindow
					}.ShowDialog();
				}, () => base.App.Project.Modules.Count((ProjectModuleVM m) => m.IsSelected) == 1);
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000FA RID: 250 RVA: 0x000055C8 File Offset: 0x000037C8
		public ICommand Advanced
		{
			get
			{
				return new RelayCommand(delegate()
				{
					new ProjectTabAdvancedView(base.App.Project)
					{
						Owner = Application.Current.MainWindow
					}.ShowDialog();
				});
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000055EC File Offset: 0x000037EC
		private void AddModule(string file)
		{
			bool flag = !File.Exists(file);
			if (flag)
			{
				MessageBox.Show(string.Format("File '{0}' does not exists!", file), "ConfuserEx", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			else
			{
				bool flag2 = string.IsNullOrEmpty(base.App.Project.BaseDirectory);
				if (flag2)
				{
					string directoryName = Path.GetDirectoryName(file);
					base.App.Project.BaseDirectory = directoryName;
					base.App.Project.OutputDirectory = Path.Combine(directoryName, "NETSecure");
				}
				ProjectModuleVM projectModuleVM = new ProjectModuleVM(base.App.Project, new ProjectModule());
				try
				{
					projectModuleVM.Path = Utils.GetRelativePath(file, base.App.Project.BaseDirectory);
				}
				catch
				{
					projectModuleVM.Path = file;
				}
				base.App.Project.Modules.Add(projectModuleVM);
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000056E4 File Offset: 0x000038E4
		[CompilerGenerated]
		private void <get_DragDrop>b__2_0(IDataObject data)
		{
			foreach (string file in (string[])data.GetData(DataFormats.FileDrop))
			{
				this.AddModule(file);
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00005720 File Offset: 0x00003920
		[CompilerGenerated]
		private void <get_ChooseBaseDir>b__4_0()
		{
			VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
			vistaFolderBrowserDialog.SelectedPath = base.App.Project.BaseDirectory;
			bool flag = vistaFolderBrowserDialog.ShowDialog() ?? false;
			if (flag)
			{
				base.App.Project.BaseDirectory = vistaFolderBrowserDialog.SelectedPath;
				base.App.Project.OutputDirectory = Path.Combine(base.App.Project.BaseDirectory, "Confused");
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000057B0 File Offset: 0x000039B0
		[CompilerGenerated]
		private void <get_ChooseOutputDir>b__6_0()
		{
			VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
			vistaFolderBrowserDialog.SelectedPath = base.App.Project.OutputDirectory;
			bool flag = vistaFolderBrowserDialog.ShowDialog() ?? false;
			if (flag)
			{
				base.App.Project.OutputDirectory = vistaFolderBrowserDialog.SelectedPath;
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00005814 File Offset: 0x00003A14
		[CompilerGenerated]
		private void <get_Add>b__8_0()
		{
			VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
			vistaOpenFileDialog.Filter = ".NET assemblies (*.exe, *.dll)|*.exe;*.dll|All Files (*.*)|*.*";
			vistaOpenFileDialog.Multiselect = true;
			bool flag = vistaOpenFileDialog.ShowDialog() ?? false;
			if (flag)
			{
				foreach (string file in vistaOpenFileDialog.FileNames)
				{
					this.AddModule(file);
				}
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00005888 File Offset: 0x00003A88
		[CompilerGenerated]
		private void <get_Remove>b__10_0()
		{
			Debug.Assert(base.App.Project.Modules.Any((ProjectModuleVM m) => m.IsSelected));
			string messageBoxText = "Are you sure to remove selected modules?\r\nAll settings specific to it would be lost!";
			bool flag = MessageBox.Show(messageBoxText, "ConfuserEx", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
			if (flag)
			{
				foreach (ProjectModuleVM item in (from m in base.App.Project.Modules
				where m.IsSelected
				select m).ToList<ProjectModuleVM>())
				{
					base.App.Project.Modules.Remove(item);
				}
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000261C File Offset: 0x0000081C
		[CompilerGenerated]
		private bool <get_Remove>b__10_3()
		{
			return base.App.Project.Modules.Any((ProjectModuleVM m) => m.IsSelected);
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00005978 File Offset: 0x00003B78
		[CompilerGenerated]
		private void <get_Edit>b__12_0()
		{
			Debug.Assert(base.App.Project.Modules.Count((ProjectModuleVM m) => m.IsSelected) == 1);
			new ProjectModuleView(base.App.Project.Modules.Single((ProjectModuleVM m) => m.IsSelected))
			{
				Owner = Application.Current.MainWindow
			}.ShowDialog();
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00002652 File Offset: 0x00000852
		[CompilerGenerated]
		private bool <get_Edit>b__12_3()
		{
			return base.App.Project.Modules.Count((ProjectModuleVM m) => m.IsSelected) == 1;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00005A18 File Offset: 0x00003C18
		[CompilerGenerated]
		private void <get_Advanced>b__14_0()
		{
			new ProjectTabAdvancedView(base.App.Project)
			{
				Owner = Application.Current.MainWindow
			}.ShowDialog();
		}

		// Token: 0x02000027 RID: 39
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000105 RID: 261 RVA: 0x0000268B File Offset: 0x0000088B
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000106 RID: 262 RVA: 0x00002119 File Offset: 0x00000319
			public <>c()
			{
			}

			// Token: 0x06000107 RID: 263 RVA: 0x00005A50 File Offset: 0x00003C50
			internal bool <get_DragDrop>b__2_1(IDataObject data)
			{
				bool flag = !data.GetDataPresent(DataFormats.FileDrop);
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					string[] source = (string[])data.GetData(DataFormats.FileDrop);
					bool flag2 = source.All((string file) => File.Exists(file));
					result = flag2;
				}
				return result;
			}

			// Token: 0x06000108 RID: 264 RVA: 0x000021DF File Offset: 0x000003DF
			internal bool <get_DragDrop>b__2_2(string file)
			{
				return File.Exists(file);
			}

			// Token: 0x06000109 RID: 265 RVA: 0x00002697 File Offset: 0x00000897
			internal bool <get_Remove>b__10_1(ProjectModuleVM m)
			{
				return m.IsSelected;
			}

			// Token: 0x0600010A RID: 266 RVA: 0x00002697 File Offset: 0x00000897
			internal bool <get_Remove>b__10_2(ProjectModuleVM m)
			{
				return m.IsSelected;
			}

			// Token: 0x0600010B RID: 267 RVA: 0x00002697 File Offset: 0x00000897
			internal bool <get_Remove>b__10_4(ProjectModuleVM m)
			{
				return m.IsSelected;
			}

			// Token: 0x0600010C RID: 268 RVA: 0x00002697 File Offset: 0x00000897
			internal bool <get_Edit>b__12_1(ProjectModuleVM m)
			{
				return m.IsSelected;
			}

			// Token: 0x0600010D RID: 269 RVA: 0x00002697 File Offset: 0x00000897
			internal bool <get_Edit>b__12_2(ProjectModuleVM m)
			{
				return m.IsSelected;
			}

			// Token: 0x0600010E RID: 270 RVA: 0x00002697 File Offset: 0x00000897
			internal bool <get_Edit>b__12_4(ProjectModuleVM m)
			{
				return m.IsSelected;
			}

			// Token: 0x04000060 RID: 96
			public static readonly ProjectTabVM.<>c <>9 = new ProjectTabVM.<>c();

			// Token: 0x04000061 RID: 97
			public static Func<string, bool> <>9__2_2;

			// Token: 0x04000062 RID: 98
			public static Func<IDataObject, bool> <>9__2_1;

			// Token: 0x04000063 RID: 99
			public static Func<ProjectModuleVM, bool> <>9__10_1;

			// Token: 0x04000064 RID: 100
			public static Func<ProjectModuleVM, bool> <>9__10_2;

			// Token: 0x04000065 RID: 101
			public static Func<ProjectModuleVM, bool> <>9__10_4;

			// Token: 0x04000066 RID: 102
			public static Func<ProjectModuleVM, bool> <>9__12_1;

			// Token: 0x04000067 RID: 103
			public static Func<ProjectModuleVM, bool> <>9__12_2;

			// Token: 0x04000068 RID: 104
			public static Func<ProjectModuleVM, bool> <>9__12_4;
		}
	}
}
