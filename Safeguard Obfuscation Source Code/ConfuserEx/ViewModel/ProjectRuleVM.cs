using Confuser.Core;
using Confuser.Core.Project;
using Confuser.Core.Project.Patterns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ConfuserEx.ViewModel
{
  public class ProjectRuleVM : ViewModelBase, IViewModel<Rule>
  {
    private readonly ProjectVM parent;
    private readonly Rule rule;
    private string error;
    private PatternExpression exp;

    public ProjectRuleVM(ProjectVM parent, Rule rule)
    {
      this.parent = parent;
      this.rule = rule;
      ObservableCollection<ProjectSettingVM<Protection>> observableCollection = Utils.Wrap<SettingItem<Protection>, ProjectSettingVM<Protection>>((IList<SettingItem<Protection>>) rule, (Func<SettingItem<Protection>, ProjectSettingVM<Protection>>) (setting => new ProjectSettingVM<Protection>(parent, setting)));
      observableCollection.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, e) => parent.IsModified = true);
      this.Protections = (IList<ProjectSettingVM<Protection>>) observableCollection;
      this.ParseExpression();
    }

    public ProjectVM Project => this.parent;

    public string Pattern
    {
      get => this.rule.Pattern;
      set
      {
        if (!this.SetProperty<string>(this.rule.Pattern != value, (Action<string>) (val => this.rule.Pattern = val), value, nameof (Pattern)))
          return;
        this.parent.IsModified = true;
        this.ParseExpression();
      }
    }

    public PatternExpression Expression
    {
      get => this.exp;
      set => this.SetProperty<PatternExpression>(ref this.exp, value, nameof (Expression));
    }

    public string ExpressionError
    {
      get => this.error;
      set => this.SetProperty<string>(ref this.error, value, nameof (ExpressionError));
    }

    public ProtectionPreset Preset
    {
      get => this.rule.Preset;
      set
      {
        if (!this.SetProperty<ProtectionPreset>(this.rule.Preset != value, (Action<ProtectionPreset>) (val => this.rule.Preset = val), value, nameof (Preset)))
          return;
        this.parent.IsModified = true;
      }
    }

    public bool Inherit
    {
      get => this.rule.Inherit;
      set
      {
        if (!this.SetProperty<bool>(this.rule.Inherit != value, (Action<bool>) (val => this.rule.Inherit = val), value, nameof (Inherit)))
          return;
        this.parent.IsModified = true;
      }
    }

    public IList<ProjectSettingVM<Protection>> Protections { get; private set; }

    Rule IViewModel<Rule>.Model => this.rule;

    private void ParseExpression()
    {
      if (this.Pattern == null)
        return;
      PatternExpression patternExpression;
      try
      {
        patternExpression = new PatternParser().Parse(this.Pattern);
        this.ExpressionError = (string) null;
      }
      catch (Exception ex)
      {
        this.ExpressionError = ex.Message;
        patternExpression = (PatternExpression) null;
      }
      this.Expression = patternExpression;
    }
  }
}
