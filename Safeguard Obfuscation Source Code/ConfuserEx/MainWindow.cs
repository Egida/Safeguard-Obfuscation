using Confuser.Core.Project;
using ConfuserEx.ViewModel;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Xml;

namespace ConfuserEx
{
  public class MainWindow : Window, IComponentConnector
  {
    private bool _contentLoaded;

    public MainWindow()
    {
      this.InitializeComponent();
      AppVM app = new AppVM();
      app.Project = new ProjectVM(new ConfuserProject(), (string) null);
      app.FileName = "SafeGuard Obfuscator";
      app.Tabs.Add((TabViewModel) new ProjectTabVM(app));
      app.Tabs.Add((TabViewModel) new SettingsTabVM(app));
      app.Tabs.Add((TabViewModel) new ProtectTabVM(app));
      app.Tabs.Add((TabViewModel) new AboutTabVM(app));
      this.LoadProj(app);
      this.DataContext = (object) app;
    }

    private void OpenMenu(object sender, RoutedEventArgs e)
    {
      Button button = (Button) sender;
      ContextMenu contextMenu = button.ContextMenu;
      contextMenu.PlacementTarget = (UIElement) button;
      contextMenu.Placement = PlacementMode.MousePoint;
      contextMenu.IsOpen = true;
    }

    private void LoadProj(AppVM app)
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      if ((commandLineArgs.Length != 2 ? 1 : (!File.Exists(commandLineArgs[1]) ? 1 : 0)) != 0)
        return;
      string fullPath = Path.GetFullPath(commandLineArgs[1]);
      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(fullPath);
        ConfuserProject proj = new ConfuserProject();
        proj.Load(doc);
        app.Project = new ProjectVM(proj, fullPath);
        app.FileName = fullPath;
      }
      catch
      {
        int num = (int) MessageBox.Show("Invalid project!", "ConfuserEx", MessageBoxButton.OK, MessageBoxImage.Hand);
      }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);
      e.Cancel = !((AppVM) this.DataContext).OnWindowClosing();
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/ConfuserEx;component/mainwindow1.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, (object) this, handler);

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId != 2)
          this._contentLoaded = true;
        else
          ((Selector) target).SelectionChanged += new SelectionChangedEventHandler(this.TabControl_SelectionChanged);
      }
      else
        ((ButtonBase) target).Click += new RoutedEventHandler(this.OpenMenu);
    }
  }
}
