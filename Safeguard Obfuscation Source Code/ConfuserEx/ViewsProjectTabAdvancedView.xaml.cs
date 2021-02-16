using Confuser.Core;
using ConfuserEx.ViewModel;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace ConfuserEx.Views
{
  public partial class ProjectTabAdvancedView : Window, IComponentConnector
  {
    private readonly ProjectVM project;
    internal ListBox ProbePaths;
    internal Button AddProbe;
    internal Button RemoveProbe;
    internal ListBox PluginPaths;
    internal Button AddPlugin;
    internal Button RemovePlugin;
    private bool _contentLoaded;

    public ProjectTabAdvancedView(ProjectVM project)
    {
      this.InitializeComponent();
      this.project = project;
      this.DataContext = (object) project;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.AddPlugin.Command = (ICommand) new RelayCommand((Action) (() =>
      {
        VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
        vistaOpenFileDialog.Filter = ".NET assemblies (*.exe, *.dll)|*.exe;*.dll|All Files (*.*)|*.*";
        vistaOpenFileDialog.Multiselect = true;
        bool? nullable = vistaOpenFileDialog.ShowDialog();
        if (!nullable.HasValue || !nullable.GetValueOrDefault())
          return;
        foreach (string fileName in vistaOpenFileDialog.FileNames)
        {
          try
          {
            ComponentDiscovery.LoadComponents((IList<ConfuserComponent>) this.project.Protections, (IList<ConfuserComponent>) this.project.Packers, fileName);
            this.project.Plugins.Add(new StringItem(fileName));
          }
          catch
          {
            int num = (int) MessageBox.Show("Failed to load plugin '" + fileName + "'.");
          }
        }
      }));
      this.RemovePlugin.Command = (ICommand) new RelayCommand((Action) (() =>
      {
        int selectedIndex = this.PluginPaths.SelectedIndex;
        Debug.Assert(selectedIndex != -1);
        ComponentDiscovery.RemoveComponents((IList<ConfuserComponent>) this.project.Protections, (IList<ConfuserComponent>) this.project.Packers, this.project.Plugins[selectedIndex].Item);
        this.project.Plugins.RemoveAt(selectedIndex);
        this.PluginPaths.SelectedIndex = selectedIndex >= this.project.Plugins.Count ? this.project.Plugins.Count - 1 : selectedIndex;
      }), (Func<bool>) (() => this.PluginPaths.SelectedIndex != -1));
      this.AddProbe.Command = (ICommand) new RelayCommand((Action) (() =>
      {
        VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
        bool? nullable = folderBrowserDialog.ShowDialog();
        if (!nullable.HasValue || !nullable.GetValueOrDefault())
          return;
        this.project.ProbePaths.Add(new StringItem(folderBrowserDialog.SelectedPath));
      }));
      this.RemoveProbe.Command = (ICommand) new RelayCommand((Action) (() =>
      {
        int selectedIndex = this.ProbePaths.SelectedIndex;
        Debug.Assert(selectedIndex != -1);
        this.project.ProbePaths.RemoveAt(selectedIndex);
        this.ProbePaths.SelectedIndex = selectedIndex >= this.project.ProbePaths.Count ? this.project.ProbePaths.Count - 1 : selectedIndex;
      }), (Func<bool>) (() => this.ProbePaths.SelectedIndex != -1));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/ConfuserEx;component/views/projecttabadvancedview.xaml", UriKind.Relative));
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
          this.ProbePaths = (ListBox) target;
          break;
        case 2:
          this.AddProbe = (Button) target;
          break;
        case 3:
          this.RemoveProbe = (Button) target;
          break;
        case 4:
          this.PluginPaths = (ListBox) target;
          break;
        case 5:
          this.AddPlugin = (Button) target;
          break;
        case 6:
          this.RemovePlugin = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
