using Confuser.Core;
using Confuser.Core.Project;
using ConfuserEx.ViewModel;
using GalaSoft.MvvmLight.Command;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace ConfuserEx.Views
{
  public partial class ProjectRuleView : Window, IComponentConnector
  {
    private readonly ProjectVM proj;
    private readonly ProjectRuleVM rule;
    internal ProjectRuleView View;
    internal TextBox pattern;
    internal Image errorImg;
    internal ListBox prots;
    internal Button AddBtn;
    internal Button RemoveBtn;
    private bool _contentLoaded;

    public ProjectRuleView(ProjectVM proj, ProjectRuleVM rule)
    {
      this.InitializeComponent();
      this.rule = rule;
      this.proj = proj;
      this.DataContext = (object) rule;
      rule.PropertyChanged += new PropertyChangedEventHandler(this.OnPropertyChanged);
      this.CheckValidity();
    }

    public ProjectVM Project => this.proj;

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.AddBtn.Command = (ICommand) new RelayCommand((Action) (() => this.rule.Protections.Add(new ProjectSettingVM<Protection>(this.proj, new SettingItem<Protection>())
      {
        Id = this.proj.Protections[0].Id
      })));
      this.RemoveBtn.Command = (ICommand) new RelayCommand((Action) (() =>
      {
        int selectedIndex = this.prots.SelectedIndex;
        Debug.Assert(selectedIndex != -1);
        this.rule.Protections.RemoveAt(this.prots.SelectedIndex);
        this.prots.SelectedIndex = selectedIndex >= this.rule.Protections.Count ? this.rule.Protections.Count - 1 : selectedIndex;
      }), (Func<bool>) (() => this.prots.SelectedIndex != -1));
    }

    public void Cleanup() => this.rule.PropertyChanged -= new PropertyChangedEventHandler(this.OnPropertyChanged);

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Expression"))
        return;
      this.CheckValidity();
    }

    private void CheckValidity()
    {
      if (this.rule.Expression == null)
      {
        this.pattern.BorderBrush = (Brush) Brushes.Red;
        this.errorImg.Visibility = Visibility.Visible;
      }
      else
      {
        this.pattern.ClearValue(Control.BorderBrushProperty);
        this.errorImg.Visibility = Visibility.Hidden;
      }
    }

    private void Done(object sender, RoutedEventArgs e) => this.DialogResult = new bool?(true);

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/ConfuserEx;component/views/projectruleview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, (object) this, handler);

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.View = (ProjectRuleView) target;
          break;
        case 2:
          this.pattern = (TextBox) target;
          break;
        case 3:
          this.errorImg = (Image) target;
          break;
        case 4:
          this.prots = (ListBox) target;
          break;
        case 5:
          this.AddBtn = (Button) target;
          break;
        case 6:
          this.RemoveBtn = (Button) target;
          break;
        case 7:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.Done);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
