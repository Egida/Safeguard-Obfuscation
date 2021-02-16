using ConfuserEx.ViewModel;
using Ookii.Dialogs.Wpf;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace ConfuserEx.Views
{
  public partial class ProjectModuleView : Window, IComponentConnector
  {
    private readonly ProjectModuleVM module;
    internal TextBox PathBox;
    internal TextBox PwdBox;
    private bool _contentLoaded;

    public ProjectModuleView(ProjectModuleVM module)
    {
      this.InitializeComponent();
      this.module = module;
      this.DataContext = (object) module;
      this.PwdBox.IsEnabled = !string.IsNullOrEmpty(this.PathBox.Text);
    }

    private void Done(object sender, RoutedEventArgs e) => this.DialogResult = new bool?(true);

    private void PathBox_TextChanged(object sender, TextChangedEventArgs e) => this.PwdBox.IsEnabled = !string.IsNullOrEmpty(this.PathBox.Text);

    private void ChooseSNKey(object sender, RoutedEventArgs e)
    {
      VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
      vistaOpenFileDialog.Filter = "Supported Key Files (*.snk, *.pfx)|*.snk;*.pfx|All Files (*.*)|*.*";
      bool? nullable = vistaOpenFileDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.GetValueOrDefault())
        return;
      this.module.SNKeyPath = vistaOpenFileDialog.FileName;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/ConfuserEx;component/views/projectmoduleview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, (object) this, handler);

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.PathBox = (TextBox) target;
          this.PathBox.TextChanged += new TextChangedEventHandler(this.PathBox_TextChanged);
          break;
        case 2:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.ChooseSNKey);
          break;
        case 3:
          this.PwdBox = (TextBox) target;
          break;
        case 4:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.Done);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
