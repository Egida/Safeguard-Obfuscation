using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace ConfuserEx
{
  public partial class App : Application
  {
    private bool _contentLoaded;

    private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      string name = (args.Name.Contains<char>(',') ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "")).Replace(".", "_");
      return name.EndsWith("_resources") ? (Assembly) null : Assembly.Load((byte[]) new ResourceManager(this.GetType().Namespace + ".Properties.Resources", Assembly.GetExecutingAssembly()).GetObject(name));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      this.StartupUri = new Uri("MainWindow1.xaml", UriKind.Relative);
      Application.LoadComponent((object) this, new Uri("/ConfuserEx;component/app.xaml", UriKind.Relative));
    }

    [STAThread]
    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public static void Main()
    {
      App app = new App();
      app.InitializeComponent();
      app.Run();
    }
  }
}
