using Confuser.Core.Project;

namespace ConfuserEx.ViewModel
{
  public class ProjectSettingVM<T> : ViewModelBase, IViewModel<SettingItem<T>>
  {
    private readonly ProjectVM parent;
    private readonly SettingItem<T> setting;

    public ProjectSettingVM(ProjectVM parent, SettingItem<T> setting)
    {
      this.parent = parent;
      this.setting = setting;
    }

    public string Id
    {
      get => this.setting.Id;
      set
      {
        if (!this.SetProperty<string>(this.setting.Id != value, (System.Action<string>) (val => this.setting.Id = val), value, nameof (Id)))
          return;
        this.parent.IsModified = true;
      }
    }

    public SettingItemAction Action
    {
      get => this.setting.Action;
      set
      {
        if (!this.SetProperty<SettingItemAction>(this.setting.Action != value, (System.Action<SettingItemAction>) (val => this.setting.Action = val), value, nameof (Action)))
          return;
        this.parent.IsModified = true;
      }
    }

    SettingItem<T> IViewModel<SettingItem<T>>.Model => this.setting;
  }
}
