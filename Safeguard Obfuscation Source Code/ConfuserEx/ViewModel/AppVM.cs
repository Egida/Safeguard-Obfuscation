using Confuser.Core;
using Confuser.Core.Project;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace ConfuserEx.ViewModel
{
  public class AppVM : ViewModelBase
  {
    private readonly IList<TabViewModel> tabs = (IList<TabViewModel>) new ObservableCollection<TabViewModel>();
    private string fileName;
    private bool navDisabled;
    private bool firstSaved;
    private ProjectVM proj;

    public bool NavigationDisabled
    {
      get => this.navDisabled;
      set => this.SetProperty<bool>(ref this.navDisabled, value, nameof (NavigationDisabled));
    }

    public ProjectVM Project
    {
      get => this.proj;
      set
      {
        if (this.proj != null)
          this.proj.PropertyChanged -= new PropertyChangedEventHandler(this.OnProjectPropertyChanged);
        this.SetProperty<ProjectVM>(ref this.proj, value, nameof (Project));
        if (this.proj == null)
          return;
        this.proj.PropertyChanged += new PropertyChangedEventHandler(this.OnProjectPropertyChanged);
      }
    }

    public string FileName
    {
      get => this.fileName;
      set
      {
        this.SetProperty<string>(ref this.fileName, value, "Project");
        this.OnPropertyChanged("Title");
      }
    }

    public string Title => string.Format("{0}{1} - {2}", (object) Path.GetFileName(this.fileName), this.proj.IsModified ? (object) "*" : (object) "", (object) ConfuserEngine.Version);

    public IList<TabViewModel> Tabs => this.tabs;

    public ICommand NewProject => (ICommand) new RelayCommand(new Action(this.NewProj), (Func<bool>) (() => !this.NavigationDisabled));

    public ICommand OpenProject => (ICommand) new RelayCommand(new Action(this.OpenProj), (Func<bool>) (() => !this.NavigationDisabled));

    public ICommand SaveProject => (ICommand) new RelayCommand((Action) (() => this.SaveProj()), (Func<bool>) (() => !this.NavigationDisabled));

    public ICommand Decode => (ICommand) new RelayCommand((Action) (() =>
    {
      new StackTraceDecoder()
      {
        Owner = Application.Current.MainWindow
      }.ShowDialog();
    }), (Func<bool>) (() => !this.NavigationDisabled));

    public bool OnWindowClosing() => this.PromptSave();

    private bool SaveProj()
    {
      if (!this.firstSaved || !File.Exists(this.FileName))
      {
        VistaSaveFileDialog vistaSaveFileDialog = new VistaSaveFileDialog();
        vistaSaveFileDialog.FileName = this.FileName;
        vistaSaveFileDialog.Filter = "ConfuserEx Projects (*.crproj)|*.crproj|All Files (*.*)|*.*";
        vistaSaveFileDialog.DefaultExt = ".crproj";
        vistaSaveFileDialog.AddExtension = true;
        bool? nullable = vistaSaveFileDialog.ShowDialog(Application.Current.MainWindow);
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 || vistaSaveFileDialog.FileName == null)
          return false;
        this.FileName = vistaSaveFileDialog.FileName;
      }
      ((IViewModel<ConfuserProject>) this.Project).Model.Save().Save(this.FileName);
      this.Project.IsModified = false;
      this.firstSaved = true;
      return true;
    }

    private bool PromptSave()
    {
      if (!this.Project.IsModified)
        return true;
      switch (MessageBox.Show("The current project has unsaved changes. Do you want to save them?", "ConfuserEx", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
      {
        case MessageBoxResult.Cancel:
          return false;
        case MessageBoxResult.Yes:
          return this.SaveProj();
        case MessageBoxResult.No:
          return true;
        default:
          return false;
      }
    }

    private void NewProj()
    {
      if (!this.PromptSave())
        return;
      this.Project = new ProjectVM(new ConfuserProject(), (string) null);
      this.FileName = "Unnamed.crproj";
    }

    private void OpenProj()
    {
      if (!this.PromptSave())
        return;
      VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
      vistaOpenFileDialog.Filter = "ConfuserEx Projects (*.crproj)|*.crproj|All Files (*.*)|*.*";
      bool? nullable = vistaOpenFileDialog.ShowDialog(Application.Current.MainWindow);
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 || vistaOpenFileDialog.FileName == null)
        return;
      string fileName = vistaOpenFileDialog.FileName;
      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(fileName);
        ConfuserProject proj = new ConfuserProject();
        proj.Load(doc);
        this.Project = new ProjectVM(proj, fileName);
        this.FileName = fileName;
      }
      catch
      {
        int num = (int) MessageBox.Show("Invalid project!", "ConfuserEx", MessageBoxButton.OK, MessageBoxImage.Hand);
      }
    }

    private void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IsModified"))
        return;
      this.OnPropertyChanged("Title");
    }

    protected override void OnPropertyChanged(string property)
    {
      base.OnPropertyChanged(property);
      if (!(property == "Project"))
        return;
      this.LoadPlugins();
    }

    private void LoadPlugins()
    {
      foreach (StringItem plugin in (IEnumerable<StringItem>) this.Project.Plugins)
      {
        try
        {
          ComponentDiscovery.LoadComponents((IList<ConfuserComponent>) this.Project.Protections, (IList<ConfuserComponent>) this.Project.Packers, plugin.Item);
        }
        catch
        {
          int num = (int) MessageBox.Show("Failed to load plugin '" + (object) plugin + "'.");
        }
      }
    }
  }
}
