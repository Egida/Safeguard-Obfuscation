using Confuser.Core;
using Confuser.Core.Project;
using ConfuserEx.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ConfuserEx.ViewModel
{
  internal class SettingsTabVM : TabViewModel
  {
    private bool hasPacker;
    private IRuleContainer selectedList;
    private int selectedRuleIndex;

    public SettingsTabVM(AppVM app)
      : base(app, "Settings")
    {
      app.PropertyChanged += (PropertyChangedEventHandler) ((sender, e) =>
      {
        if (!(e.PropertyName == "Project"))
          return;
        this.InitProject();
      });
      this.InitProject();
    }

    public bool HasPacker
    {
      get => this.hasPacker;
      set => this.SetProperty<bool>(ref this.hasPacker, value, nameof (HasPacker));
    }

    public IList ModulesView { get; private set; }

    public IRuleContainer SelectedList
    {
      get => this.selectedList;
      set
      {
        if (!this.SetProperty<IRuleContainer>(ref this.selectedList, value, nameof (SelectedList)))
          return;
        this.SelectedRuleIndex = -1;
      }
    }

    public int SelectedRuleIndex
    {
      get => this.selectedRuleIndex;
      set => this.SetProperty<int>(ref this.selectedRuleIndex, value, nameof (SelectedRuleIndex));
    }

    public ICommand Add => (ICommand) new RelayCommand((Action) (() =>
    {
      Debug.Assert(this.SelectedList != null);
      this.SelectedList.Rules.Add(new ProjectRuleVM(this.App.Project, new Rule())
      {
        Pattern = "true"
      });
      this.SelectedRuleIndex = this.SelectedList.Rules.Count - 1;
    }), (Func<bool>) (() => this.SelectedList != null));

    public ICommand Remove => (ICommand) new RelayCommand((Action) (() =>
    {
      int selectedRuleIndex = this.SelectedRuleIndex;
      Debug.Assert(this.SelectedList != null);
      Debug.Assert(selectedRuleIndex != -1);
      ProjectRuleVM rule = this.SelectedList.Rules[selectedRuleIndex];
      this.SelectedList.Rules.RemoveAt(selectedRuleIndex);
      this.SelectedRuleIndex = selectedRuleIndex >= this.SelectedList.Rules.Count ? this.SelectedList.Rules.Count - 1 : selectedRuleIndex;
    }), (Func<bool>) (() => this.SelectedRuleIndex != -1 && this.SelectedList != null));

    public ICommand Edit => (ICommand) new RelayCommand((Action) (() =>
    {
      Debug.Assert(this.SelectedRuleIndex != -1);
      ProjectRuleView projectRuleView = new ProjectRuleView(this.App.Project, this.SelectedList.Rules[this.SelectedRuleIndex]);
      projectRuleView.Owner = Application.Current.MainWindow;
      projectRuleView.ShowDialog();
      projectRuleView.Cleanup();
    }), (Func<bool>) (() => this.SelectedRuleIndex != -1 && this.SelectedList != null));

    private void InitProject()
    {
      this.ModulesView = (IList) new CompositeCollection()
      {
        (object) this.App.Project,
        (object) new CollectionContainer()
        {
          Collection = (IEnumerable) this.App.Project.Modules
        }
      };
      this.OnPropertyChanged("ModulesView");
      this.HasPacker = this.App.Project.Packer != null;
    }

    protected override void OnPropertyChanged(string property)
    {
      if (property == "HasPacker")
      {
        if (this.hasPacker && this.App.Project.Packer == null)
          this.App.Project.Packer = new ProjectSettingVM<Packer>(this.App.Project, new SettingItem<Packer>()
          {
            Id = this.App.Project.Packers[0].Id
          });
        else if (!this.hasPacker)
          this.App.Project.Packer = (ProjectSettingVM<Packer>) null;
      }
      base.OnPropertyChanged(property);
    }
  }
}
