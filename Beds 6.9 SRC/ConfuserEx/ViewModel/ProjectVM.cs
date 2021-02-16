using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Project;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000028 RID: 40
	public class ProjectVM : ViewModelBase, IViewModel<ConfuserProject>, IRuleContainer
	{
		// Token: 0x0600010F RID: 271 RVA: 0x00005AB0 File Offset: 0x00003CB0
		public ProjectVM(ConfuserProject proj, string fileName)
		{
			this.proj = proj;
			this.FileName = fileName;
			ObservableCollection<ProjectModuleVM> observableCollection = Utils.Wrap<ProjectModule, ProjectModuleVM>(proj, (ProjectModule module) => new ProjectModuleVM(this, module));
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.IsModified = true;
			};
			this.Modules = observableCollection;
			ObservableCollection<StringItem> observableCollection2 = Utils.Wrap<string, StringItem>(proj.PluginPaths, (string path) => new StringItem(path));
			observableCollection2.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.IsModified = true;
			};
			this.Plugins = observableCollection2;
			ObservableCollection<StringItem> observableCollection3 = Utils.Wrap<string, StringItem>(proj.ProbePaths, (string path) => new StringItem(path));
			observableCollection3.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.IsModified = true;
			};
			this.ProbePaths = observableCollection3;
			ObservableCollection<ProjectRuleVM> observableCollection4 = Utils.Wrap<Rule, ProjectRuleVM>(proj.Rules, (Rule rule) => new ProjectRuleVM(this, rule));
			observableCollection4.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.IsModified = true;
			};
			this.Rules = observableCollection4;
			this.Protections = new ObservableCollection<ConfuserComponent>();
			this.Packers = new ObservableCollection<ConfuserComponent>();
			ComponentDiscovery.LoadComponents(this.Protections, this.Packers, Assembly.Load("Confuser.Protections").Location);
			ComponentDiscovery.LoadComponents(this.Protections, this.Packers, Assembly.Load("Confuser.Renamer").Location);
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000110 RID: 272 RVA: 0x00005C1C File Offset: 0x00003E1C
		public ConfuserProject Project
		{
			get
			{
				return this.proj;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000111 RID: 273 RVA: 0x00005C34 File Offset: 0x00003E34
		// (set) Token: 0x06000112 RID: 274 RVA: 0x0000269F File Offset: 0x0000089F
		public bool IsModified
		{
			get
			{
				return this.modified;
			}
			set
			{
				base.SetProperty<bool>(ref this.modified, value, "IsModified");
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00005C4C File Offset: 0x00003E4C
		// (set) Token: 0x06000114 RID: 276 RVA: 0x000026B5 File Offset: 0x000008B5
		public string Seed
		{
			get
			{
				return this.proj.Seed;
			}
			set
			{
				base.SetProperty<string>(this.proj.Seed != value, delegate(string val)
				{
					this.proj.Seed = val;
				}, value, "Seed");
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00005C6C File Offset: 0x00003E6C
		// (set) Token: 0x06000116 RID: 278 RVA: 0x000026E2 File Offset: 0x000008E2
		public bool Debug
		{
			get
			{
				return this.proj.Debug;
			}
			set
			{
				base.SetProperty<bool>(this.proj.Debug != value, delegate(bool val)
				{
					this.proj.Debug = val;
				}, value, "Debug");
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00005C8C File Offset: 0x00003E8C
		// (set) Token: 0x06000118 RID: 280 RVA: 0x0000270F File Offset: 0x0000090F
		public string BaseDirectory
		{
			get
			{
				return this.proj.BaseDirectory;
			}
			set
			{
				base.SetProperty<string>(this.proj.BaseDirectory != value, delegate(string val)
				{
					this.proj.BaseDirectory = val;
				}, value, "BaseDirectory");
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00005CAC File Offset: 0x00003EAC
		// (set) Token: 0x0600011A RID: 282 RVA: 0x0000273C File Offset: 0x0000093C
		public string OutputDirectory
		{
			get
			{
				return this.proj.OutputDirectory;
			}
			set
			{
				base.SetProperty<string>(this.proj.OutputDirectory != value, delegate(string val)
				{
					this.proj.OutputDirectory = val;
				}, value, "OutputDirectory");
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00005CCC File Offset: 0x00003ECC
		// (set) Token: 0x0600011C RID: 284 RVA: 0x00005D18 File Offset: 0x00003F18
		public ProjectSettingVM<Packer> Packer
		{
			get
			{
				bool flag = this.proj.Packer == null;
				if (flag)
				{
					this.packer = null;
				}
				else
				{
					this.packer = new ProjectSettingVM<Packer>(this, this.proj.Packer);
				}
				return this.packer;
			}
			set
			{
				bool changed = (value == null && this.proj.Packer != null) || (value != null && this.proj.Packer != ((IViewModel<SettingItem<Packer>>)value).Model);
				base.SetProperty<IViewModel<SettingItem<Packer>>>(changed, delegate(IViewModel<SettingItem<Packer>> val)
				{
					this.proj.Packer = ((val == null) ? null : val.Model);
				}, value, "Packer");
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00002769 File Offset: 0x00000969
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00002771 File Offset: 0x00000971
		public IList<ProjectModuleVM> Modules
		{
			[CompilerGenerated]
			get
			{
				return this.<Modules>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Modules>k__BackingField = value;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600011F RID: 287 RVA: 0x0000277A File Offset: 0x0000097A
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00002782 File Offset: 0x00000982
		public IList<StringItem> Plugins
		{
			[CompilerGenerated]
			get
			{
				return this.<Plugins>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Plugins>k__BackingField = value;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000121 RID: 289 RVA: 0x0000278B File Offset: 0x0000098B
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00002793 File Offset: 0x00000993
		public IList<StringItem> ProbePaths
		{
			[CompilerGenerated]
			get
			{
				return this.<ProbePaths>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ProbePaths>k__BackingField = value;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000123 RID: 291 RVA: 0x0000279C File Offset: 0x0000099C
		// (set) Token: 0x06000124 RID: 292 RVA: 0x000027A4 File Offset: 0x000009A4
		public ObservableCollection<ConfuserComponent> Protections
		{
			[CompilerGenerated]
			get
			{
				return this.<Protections>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Protections>k__BackingField = value;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000125 RID: 293 RVA: 0x000027AD File Offset: 0x000009AD
		// (set) Token: 0x06000126 RID: 294 RVA: 0x000027B5 File Offset: 0x000009B5
		public ObservableCollection<ConfuserComponent> Packers
		{
			[CompilerGenerated]
			get
			{
				return this.<Packers>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Packers>k__BackingField = value;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000127 RID: 295 RVA: 0x000027BE File Offset: 0x000009BE
		// (set) Token: 0x06000128 RID: 296 RVA: 0x000027C6 File Offset: 0x000009C6
		public IList<ProjectRuleVM> Rules
		{
			[CompilerGenerated]
			get
			{
				return this.<Rules>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Rules>k__BackingField = value;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000129 RID: 297 RVA: 0x000027CF File Offset: 0x000009CF
		// (set) Token: 0x0600012A RID: 298 RVA: 0x000027D7 File Offset: 0x000009D7
		public string FileName
		{
			[CompilerGenerated]
			get
			{
				return this.<FileName>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<FileName>k__BackingField = value;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600012B RID: 299 RVA: 0x00005C1C File Offset: 0x00003E1C
		ConfuserProject IViewModel<ConfuserProject>.Model
		{
			get
			{
				return this.proj;
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00005D74 File Offset: 0x00003F74
		protected override void OnPropertyChanged(string property)
		{
			base.OnPropertyChanged(property);
			bool flag = property != "IsModified";
			if (flag)
			{
				this.IsModified = true;
			}
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000027E0 File Offset: 0x000009E0
		[CompilerGenerated]
		private ProjectModuleVM <.ctor>b__3_0(ProjectModule module)
		{
			return new ProjectModuleVM(this, module);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x000027E9 File Offset: 0x000009E9
		[CompilerGenerated]
		private void <.ctor>b__3_1(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.IsModified = true;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000027E9 File Offset: 0x000009E9
		[CompilerGenerated]
		private void <.ctor>b__3_3(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.IsModified = true;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x000027E9 File Offset: 0x000009E9
		[CompilerGenerated]
		private void <.ctor>b__3_5(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.IsModified = true;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000027F3 File Offset: 0x000009F3
		[CompilerGenerated]
		private ProjectRuleVM <.ctor>b__3_6(Rule rule)
		{
			return new ProjectRuleVM(this, rule);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000027E9 File Offset: 0x000009E9
		[CompilerGenerated]
		private void <.ctor>b__3_7(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.IsModified = true;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000027FC File Offset: 0x000009FC
		[CompilerGenerated]
		private void <set_Seed>b__11_0(string val)
		{
			this.proj.Seed = val;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000280B File Offset: 0x00000A0B
		[CompilerGenerated]
		private void <set_Debug>b__14_0(bool val)
		{
			this.proj.Debug = val;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000281A File Offset: 0x00000A1A
		[CompilerGenerated]
		private void <set_BaseDirectory>b__17_0(string val)
		{
			this.proj.BaseDirectory = val;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00002829 File Offset: 0x00000A29
		[CompilerGenerated]
		private void <set_OutputDirectory>b__20_0(string val)
		{
			this.proj.OutputDirectory = val;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00002838 File Offset: 0x00000A38
		[CompilerGenerated]
		private void <set_Packer>b__23_0(IViewModel<SettingItem<Packer>> val)
		{
			this.proj.Packer = ((val == null) ? null : val.Model);
		}

		// Token: 0x04000069 RID: 105
		private readonly ConfuserProject proj;

		// Token: 0x0400006A RID: 106
		private bool modified;

		// Token: 0x0400006B RID: 107
		private ProjectSettingVM<Packer> packer;

		// Token: 0x0400006C RID: 108
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<ProjectModuleVM> <Modules>k__BackingField;

		// Token: 0x0400006D RID: 109
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<StringItem> <Plugins>k__BackingField;

		// Token: 0x0400006E RID: 110
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<StringItem> <ProbePaths>k__BackingField;

		// Token: 0x0400006F RID: 111
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ObservableCollection<ConfuserComponent> <Protections>k__BackingField;

		// Token: 0x04000070 RID: 112
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ObservableCollection<ConfuserComponent> <Packers>k__BackingField;

		// Token: 0x04000071 RID: 113
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<ProjectRuleVM> <Rules>k__BackingField;

		// Token: 0x04000072 RID: 114
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <FileName>k__BackingField;

		// Token: 0x02000029 RID: 41
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000138 RID: 312 RVA: 0x00002852 File Offset: 0x00000A52
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000139 RID: 313 RVA: 0x00002119 File Offset: 0x00000319
			public <>c()
			{
			}

			// Token: 0x0600013A RID: 314 RVA: 0x0000285E File Offset: 0x00000A5E
			internal StringItem <.ctor>b__3_2(string path)
			{
				return new StringItem(path);
			}

			// Token: 0x0600013B RID: 315 RVA: 0x0000285E File Offset: 0x00000A5E
			internal StringItem <.ctor>b__3_4(string path)
			{
				return new StringItem(path);
			}

			// Token: 0x04000073 RID: 115
			public static readonly ProjectVM.<>c <>9 = new ProjectVM.<>c();

			// Token: 0x04000074 RID: 116
			public static Func<string, StringItem> <>9__3_2;

			// Token: 0x04000075 RID: 117
			public static Func<string, StringItem> <>9__3_4;
		}
	}
}
