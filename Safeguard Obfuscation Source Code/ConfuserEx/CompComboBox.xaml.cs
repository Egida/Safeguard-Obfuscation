using Confuser.Core;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ConfuserEx
{
  public partial class CompComboBox : UserControl, IComponentConnector
  {
    public static readonly DependencyProperty ComponentsProperty = DependencyProperty.Register(nameof (Components), typeof (IEnumerable<ConfuserComponent>), typeof (CompComboBox), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty SelectedComponentProperty = DependencyProperty.Register(nameof (SelectedComponent), typeof (ConfuserComponent), typeof (CompComboBox), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ArgumentsProperty = DependencyProperty.Register(nameof (Arguments), typeof (Dictionary<string, string>), typeof (CompComboBox), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
    internal CompComboBox Root;
    private bool _contentLoaded;

    public CompComboBox() => this.InitializeComponent();

    public IEnumerable<ConfuserComponent> Components
    {
      get => (IEnumerable<ConfuserComponent>) this.GetValue(CompComboBox.ComponentsProperty);
      set => this.SetValue(CompComboBox.ComponentsProperty, (object) value);
    }

    public ConfuserComponent SelectedComponent
    {
      get => (ConfuserComponent) this.GetValue(CompComboBox.SelectedComponentProperty);
      set => this.SetValue(CompComboBox.SelectedComponentProperty, (object) value);
    }

    public Dictionary<string, string> Arguments
    {
      get => (Dictionary<string, string>) this.GetValue(CompComboBox.ArgumentsProperty);
      set => this.SetValue(CompComboBox.ArgumentsProperty, (object) value);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/ConfuserEx;component/compcombobox.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.Root = (CompComboBox) target;
      else
        this._contentLoaded = true;
    }
  }
}
