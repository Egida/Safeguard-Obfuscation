using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Confuser.Core;
using Confuser.Core.Project;
using ConfuserEx.ViewModel;
using GalaSoft.MvvmLight.Command;

namespace ConfuserEx.Views
{
	// Token: 0x02000018 RID: 24
	public partial class ProjectRuleView : Window
	{
		// Token: 0x06000078 RID: 120 RVA: 0x000041D4 File Offset: 0x000023D4
		public ProjectRuleView(ProjectVM proj, ProjectRuleVM rule)
		{
			this.InitializeComponent();
			this.rule = rule;
			this.proj = proj;
			base.DataContext = rule;
			rule.PropertyChanged += this.OnPropertyChanged;
			this.CheckValidity();
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00004220 File Offset: 0x00002420
		public ProjectVM Project
		{
			get
			{
				return this.proj;
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004238 File Offset: 0x00002438
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.AddBtn.Command = new RelayCommand(delegate()
			{
				ProjectSettingVM<Protection> projectSettingVM = new ProjectSettingVM<Protection>(this.proj, new SettingItem<Protection>(null, SettingItemAction.Add));
				projectSettingVM.Id = this.proj.Protections[0].Id;
				this.rule.Protections.Add(projectSettingVM);
			});
			this.RemoveBtn.Command = new RelayCommand(delegate()
			{
				int selectedIndex = this.prots.SelectedIndex;
				Debug.Assert(selectedIndex != -1);
				this.rule.Protections.RemoveAt(this.prots.SelectedIndex);
				this.prots.SelectedIndex = ((selectedIndex >= this.rule.Protections.Count) ? (this.rule.Protections.Count - 1) : selectedIndex);
			}, () => this.prots.SelectedIndex != -1);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00002337 File Offset: 0x00000537
		public void Cleanup()
		{
			this.rule.PropertyChanged -= this.OnPropertyChanged;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004294 File Offset: 0x00002494
		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			bool flag = e.PropertyName == "Expression";
			if (flag)
			{
				this.CheckValidity();
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000042C0 File Offset: 0x000024C0
		private void CheckValidity()
		{
			bool flag = this.rule.Expression == null;
			if (flag)
			{
				this.pattern.BorderBrush = Brushes.Red;
				this.errorImg.Visibility = Visibility.Visible;
			}
			else
			{
				this.pattern.ClearValue(Control.BorderBrushProperty);
				this.errorImg.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00002305 File Offset: 0x00000505
		private void Done(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		// Token: 0x04000034 RID: 52
		private readonly ProjectVM proj;

		// Token: 0x04000035 RID: 53
		private readonly ProjectRuleVM rule;
	}
}
