using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Confuser.Core;
using Confuser.Core.Project;
using ConfuserEx.Views;
using GalaSoft.MvvmLight.Command;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200002C RID: 44
	internal class SettingsTabVM : TabViewModel
	{
		// Token: 0x0600015B RID: 347 RVA: 0x00002AAB File Offset: 0x00000CAB
		public SettingsTabVM(AppVM app) : base(app, "Settings")
		{
			app.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
			{
				bool flag = e.PropertyName == "Project";
				if (flag)
				{
					this.InitProject();
				}
			};
			this.InitProject();
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600015C RID: 348 RVA: 0x000060F0 File Offset: 0x000042F0
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00002AD5 File Offset: 0x00000CD5
		public bool HasPacker
		{
			get
			{
				return this.hasPacker;
			}
			set
			{
				base.SetProperty<bool>(ref this.hasPacker, value, "HasPacker");
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00002AEB File Offset: 0x00000CEB
		// (set) Token: 0x0600015F RID: 351 RVA: 0x00002AF3 File Offset: 0x00000CF3
		public IList ModulesView
		{
			[CompilerGenerated]
			get
			{
				return this.<ModulesView>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ModulesView>k__BackingField = value;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00006108 File Offset: 0x00004308
		// (set) Token: 0x06000161 RID: 353 RVA: 0x00006120 File Offset: 0x00004320
		public IRuleContainer SelectedList
		{
			get
			{
				return this.selectedList;
			}
			set
			{
				bool flag = base.SetProperty<IRuleContainer>(ref this.selectedList, value, "SelectedList");
				if (flag)
				{
					this.SelectedRuleIndex = -1;
				}
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000162 RID: 354 RVA: 0x0000614C File Offset: 0x0000434C
		// (set) Token: 0x06000163 RID: 355 RVA: 0x00002AFC File Offset: 0x00000CFC
		public int SelectedRuleIndex
		{
			get
			{
				return this.selectedRuleIndex;
			}
			set
			{
				base.SetProperty<int>(ref this.selectedRuleIndex, value, "SelectedRuleIndex");
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00006164 File Offset: 0x00004364
		public ICommand Add
		{
			get
			{
				return new RelayCommand(delegate()
				{
					Debug.Assert(this.SelectedList != null);
					ProjectRuleVM projectRuleVM = new ProjectRuleVM(base.App.Project, new Rule("true", ProtectionPreset.None, false));
					projectRuleVM.Pattern = "true";
					this.SelectedList.Rules.Add(projectRuleVM);
					this.SelectedRuleIndex = this.SelectedList.Rules.Count - 1;
				}, () => this.SelectedList != null);
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00006194 File Offset: 0x00004394
		public ICommand Remove
		{
			get
			{
				return new RelayCommand(delegate()
				{
					int num = this.SelectedRuleIndex;
					Debug.Assert(this.SelectedList != null);
					Debug.Assert(num != -1);
					ProjectRuleVM projectRuleVM = this.SelectedList.Rules[num];
					this.SelectedList.Rules.RemoveAt(num);
					this.SelectedRuleIndex = ((num >= this.SelectedList.Rules.Count) ? (this.SelectedList.Rules.Count - 1) : num);
				}, () => this.SelectedRuleIndex != -1 && this.SelectedList != null);
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000166 RID: 358 RVA: 0x000061C4 File Offset: 0x000043C4
		public ICommand Edit
		{
			get
			{
				return new RelayCommand(delegate()
				{
					Debug.Assert(this.SelectedRuleIndex != -1);
					ProjectRuleView projectRuleView = new ProjectRuleView(base.App.Project, this.SelectedList.Rules[this.SelectedRuleIndex]);
					projectRuleView.Owner = Application.Current.MainWindow;
					projectRuleView.ShowDialog();
					projectRuleView.Cleanup();
				}, () => this.SelectedRuleIndex != -1 && this.SelectedList != null);
			}
		}

		// Token: 0x06000167 RID: 359 RVA: 0x000061F4 File Offset: 0x000043F4
		private void InitProject()
		{
			this.ModulesView = new CompositeCollection
			{
				base.App.Project,
				new CollectionContainer
				{
					Collection = base.App.Project.Modules
				}
			};
			this.OnPropertyChanged("ModulesView");
			this.HasPacker = (base.App.Project.Packer != null);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000626C File Offset: 0x0000446C
		protected override void OnPropertyChanged(string property)
		{
			bool flag = property == "HasPacker";
			if (flag)
			{
				bool flag2 = this.hasPacker && base.App.Project.Packer == null;
				if (flag2)
				{
					base.App.Project.Packer = new ProjectSettingVM<Packer>(base.App.Project, new SettingItem<Packer>(null, SettingItemAction.Add)
					{
						Id = base.App.Project.Packers[0].Id
					});
				}
				else
				{
					bool flag3 = !this.hasPacker;
					if (flag3)
					{
						base.App.Project.Packer = null;
					}
				}
			}
			base.OnPropertyChanged(property);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00006324 File Offset: 0x00004524
		[CompilerGenerated]
		private void <.ctor>b__3_0(object sender, PropertyChangedEventArgs e)
		{
			bool flag = e.PropertyName == "Project";
			if (flag)
			{
				this.InitProject();
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00006350 File Offset: 0x00004550
		[CompilerGenerated]
		private void <get_Add>b__18_0()
		{
			Debug.Assert(this.SelectedList != null);
			ProjectRuleVM projectRuleVM = new ProjectRuleVM(base.App.Project, new Rule("true", ProtectionPreset.None, false));
			projectRuleVM.Pattern = "true";
			this.SelectedList.Rules.Add(projectRuleVM);
			this.SelectedRuleIndex = this.SelectedList.Rules.Count - 1;
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00002B12 File Offset: 0x00000D12
		[CompilerGenerated]
		private bool <get_Add>b__18_1()
		{
			return this.SelectedList != null;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000063C4 File Offset: 0x000045C4
		[CompilerGenerated]
		private void <get_Remove>b__20_0()
		{
			int num = this.SelectedRuleIndex;
			Debug.Assert(this.SelectedList != null);
			Debug.Assert(num != -1);
			ProjectRuleVM projectRuleVM = this.SelectedList.Rules[num];
			this.SelectedList.Rules.RemoveAt(num);
			this.SelectedRuleIndex = ((num >= this.SelectedList.Rules.Count) ? (this.SelectedList.Rules.Count - 1) : num);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00002B1D File Offset: 0x00000D1D
		[CompilerGenerated]
		private bool <get_Remove>b__20_1()
		{
			return this.SelectedRuleIndex != -1 && this.SelectedList != null;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00006448 File Offset: 0x00004648
		[CompilerGenerated]
		private void <get_Edit>b__22_0()
		{
			Debug.Assert(this.SelectedRuleIndex != -1);
			ProjectRuleView projectRuleView = new ProjectRuleView(base.App.Project, this.SelectedList.Rules[this.SelectedRuleIndex]);
			projectRuleView.Owner = Application.Current.MainWindow;
			projectRuleView.ShowDialog();
			projectRuleView.Cleanup();
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00002B1D File Offset: 0x00000D1D
		[CompilerGenerated]
		private bool <get_Edit>b__22_1()
		{
			return this.SelectedRuleIndex != -1 && this.SelectedList != null;
		}

		// Token: 0x04000080 RID: 128
		private bool hasPacker;

		// Token: 0x04000081 RID: 129
		private IRuleContainer selectedList;

		// Token: 0x04000082 RID: 130
		private int selectedRuleIndex;

		// Token: 0x04000083 RID: 131
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList <ModulesView>k__BackingField;
	}
}
