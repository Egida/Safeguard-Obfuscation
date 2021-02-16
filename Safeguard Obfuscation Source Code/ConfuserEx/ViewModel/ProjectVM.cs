using Confuser.Core;
using Confuser.Core.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;

namespace ConfuserEx.ViewModel
{
  public class ProjectVM : ViewModelBase, IViewModel<ConfuserProject>, IRuleContainer
  {
    private readonly ConfuserProject proj;
    private bool modified;
    private ProjectSettingVM<Confuser.Core.Packer> packer;

    public ProjectVM(ConfuserProject proj, string fileName)
    {
      this.proj = proj;
      this.FileName = fileName;
      ObservableCollection<ProjectModuleVM> observableCollection1 = Utils.Wrap<ProjectModule, ProjectModuleVM>((IList<ProjectModule>) proj, (Func<ProjectModule, ProjectModuleVM>) (module => new ProjectModuleVM(this, module)));
      observableCollection1.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, e) => this.IsModified = true);
      this.Modules = (IList<ProjectModuleVM>) observableCollection1;
      ObservableCollection<StringItem> observableCollection2 = Utils.Wrap<string, StringItem>(proj.PluginPaths, (Func<string, StringItem>) (path => new StringItem(path)));
      observableCollection2.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, e) => this.IsModified = true);
      this.Plugins = (IList<StringItem>) observableCollection2;
      ObservableCollection<StringItem> observableCollection3 = Utils.Wrap<string, StringItem>(proj.ProbePaths, (Func<string, StringItem>) (path => new StringItem(path)));
      observableCollection3.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, e) => this.IsModified = true);
      this.ProbePaths = (IList<StringItem>) observableCollection3;
      ObservableCollection<ProjectRuleVM> observableCollection4 = Utils.Wrap<Rule, ProjectRuleVM>(proj.Rules, (Func<Rule, ProjectRuleVM>) (rule => new ProjectRuleVM(this, rule)));
      observableCollection4.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, e) => this.IsModified = true);
      this.Rules = (IList<ProjectRuleVM>) observableCollection4;
      this.Protections = new ObservableCollection<ConfuserComponent>();
      this.Packers = new ObservableCollection<ConfuserComponent>();
      ComponentDiscovery.LoadComponents((IList<ConfuserComponent>) this.Protections, (IList<ConfuserComponent>) this.Packers, Assembly.Load("Confuser.Protections").Location);
      ComponentDiscovery.LoadComponents((IList<ConfuserComponent>) this.Protections, (IList<ConfuserComponent>) this.Packers, Assembly.Load("Confuser.Renamer").Location);
    }

    public ConfuserProject Project => this.proj;

    public bool IsModified
    {
      get => this.modified;
      set => this.SetProperty<bool>(ref this.modified, value, nameof (IsModified));
    }

    public string Seed
    {
      get => this.proj.Seed;
      set => this.SetProperty<string>(this.proj.Seed != value, (Action<string>) (val => this.proj.Seed = val), value, nameof (Seed));
    }

    public bool Debug
    {
      get => this.proj.Debug;
      set => this.SetProperty<bool>(this.proj.Debug != value, (Action<bool>) (val => this.proj.Debug = val), value, nameof (Debug));
    }

    public string BaseDirectory
    {
      get => this.proj.BaseDirectory;
      set => this.SetProperty<string>(this.proj.BaseDirectory != value, (Action<string>) (val => this.proj.BaseDirectory = val), value, nameof (BaseDirectory));
    }

    public string OutputDirectory
    {
      get => this.proj.OutputDirectory;
      set => this.SetProperty<string>(this.proj.OutputDirectory != value, (Action<string>) (val => this.proj.OutputDirectory = val), value, nameof (OutputDirectory));
    }

    public ProjectSettingVM<Confuser.Core.Packer> Packer
    {
      get
      {
        this.packer = this.proj.Packer != null ? new ProjectSettingVM<Confuser.Core.Packer>(this, this.proj.Packer) : (ProjectSettingVM<Confuser.Core.Packer>) null;
        return this.packer;
      }
      set
      {
        IViewModel<SettingItem<Confuser.Core.Packer>> viewModel = (IViewModel<SettingItem<Confuser.Core.Packer>>) value;
        this.SetProperty<IViewModel<SettingItem<Confuser.Core.Packer>>>(viewModel == null && this.proj.Packer != null || viewModel != null && this.proj.Packer != viewModel.Model, (Action<IViewModel<SettingItem<Confuser.Core.Packer>>>) (val => this.proj.Packer = val == null ? (SettingItem<Confuser.Core.Packer>) null : val.Model), viewModel, nameof (Packer));
      }
    }

    public IList<ProjectModuleVM> Modules { get; private set; }

    public IList<StringItem> Plugins { get; private set; }

    public IList<StringItem> ProbePaths { get; private set; }

    public ObservableCollection<ConfuserComponent> Protections { get; private set; }

    public ObservableCollection<ConfuserComponent> Packers { get; private set; }

    public IList<ProjectRuleVM> Rules { get; private set; }

    public string FileName { get; set; }

    ConfuserProject IViewModel<ConfuserProject>.Model => this.proj;

    protected override void OnPropertyChanged(string property)
    {
      base.OnPropertyChanged(property);
      if (!(property != "IsModified"))
        return;
      this.IsModified = true;
    }
  }
}
