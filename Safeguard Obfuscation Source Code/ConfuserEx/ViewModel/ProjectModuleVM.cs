using Confuser.Core.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace ConfuserEx.ViewModel
{
  public class ProjectModuleVM : ViewModelBase, IViewModel<ProjectModule>, IRuleContainer
  {
    private readonly ProjectModule module;
    private readonly ProjectVM parent;
    private string asmName = "Unknown";
    private string simpleName;
    private bool isSelected;

    public ProjectModuleVM(ProjectVM parent, ProjectModule module)
    {
      this.parent = parent;
      this.module = module;
      ObservableCollection<ProjectRuleVM> observableCollection = Utils.Wrap<Rule, ProjectRuleVM>(module.Rules, (Func<Rule, ProjectRuleVM>) (rule => new ProjectRuleVM(parent, rule)));
      observableCollection.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, e) => parent.IsModified = true);
      this.Rules = (IList<ProjectRuleVM>) observableCollection;
      if (module.Path == null)
        return;
      this.SimpleName = System.IO.Path.GetFileName(module.Path);
      this.LoadAssemblyName();
    }

    public bool IsSelected
    {
      get => this.isSelected;
      set => this.SetProperty<bool>(ref this.isSelected, value, nameof (IsSelected));
    }

    public ProjectModule Module => this.module;

    public string Path
    {
      get => this.module.Path;
      set
      {
        if (!this.SetProperty<string>(this.module.Path != value, (Action<string>) (val => this.module.Path = val), value, nameof (Path)))
          return;
        this.parent.IsModified = true;
        this.SimpleName = System.IO.Path.GetFileName(this.module.Path);
        this.LoadAssemblyName();
      }
    }

    public string SimpleName
    {
      get => this.simpleName;
      private set => this.SetProperty<string>(ref this.simpleName, value, nameof (SimpleName));
    }

    public string AssemblyName
    {
      get => this.asmName;
      private set => this.SetProperty<string>(ref this.asmName, value, nameof (AssemblyName));
    }

    public string SNKeyPath
    {
      get => this.module.SNKeyPath;
      set
      {
        if (!this.SetProperty<string>(this.module.SNKeyPath != value, (Action<string>) (val => this.module.SNKeyPath = val), value, nameof (SNKeyPath)))
          return;
        this.parent.IsModified = true;
      }
    }

    public string SNKeyPassword
    {
      get => this.module.SNKeyPassword;
      set
      {
        if (!this.SetProperty<string>(this.module.SNKeyPassword != value, (Action<string>) (val => this.module.SNKeyPassword = val), value, nameof (SNKeyPassword)))
          return;
        this.parent.IsModified = true;
      }
    }

    public IList<ProjectRuleVM> Rules { get; private set; }

    ProjectModule IViewModel<ProjectModule>.Model => this.module;

    private void LoadAssemblyName()
    {
      this.AssemblyName = "Loading...";
      ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
      {
        try
        {
          string str = System.IO.Path.Combine(this.parent.BaseDirectory, this.Path);
          if (!string.IsNullOrEmpty(this.parent.FileName))
            str = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.parent.FileName), str);
          this.AssemblyName = System.Reflection.AssemblyName.GetAssemblyName(str).FullName;
        }
        catch
        {
          this.AssemblyName = "Unknown";
        }
      }));
    }
  }
}
