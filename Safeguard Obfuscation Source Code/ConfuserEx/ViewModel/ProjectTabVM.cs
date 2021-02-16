using Confuser.Core.Project;
using ConfuserEx.Views;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ConfuserEx.ViewModel
{
  public class ProjectTabVM : TabViewModel
  {
    public ProjectTabVM(AppVM app)
      : base(app, "Project")
    {
    }

    public ICommand DragDrop => (ICommand) new RelayCommand<IDataObject>((Action<IDataObject>) (data =>
    {
      foreach (string file in (string[]) data.GetData(DataFormats.FileDrop))
        this.AddModule(file);
    }), (Func<IDataObject, bool>) (data => data.GetDataPresent(DataFormats.FileDrop) && ((IEnumerable<string>) (string[]) data.GetData(DataFormats.FileDrop)).All<string>((Func<string, bool>) (file => File.Exists(file)))));

    public ICommand ChooseBaseDir => (ICommand) new RelayCommand((Action) (() =>
    {
      VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
      folderBrowserDialog.SelectedPath = this.App.Project.BaseDirectory;
      bool? nullable = folderBrowserDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.GetValueOrDefault())
        return;
      this.App.Project.BaseDirectory = folderBrowserDialog.SelectedPath;
      this.App.Project.OutputDirectory = Path.Combine(this.App.Project.BaseDirectory, "Confused");
    }));

    public ICommand ChooseOutputDir => (ICommand) new RelayCommand((Action) (() =>
    {
      VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
      folderBrowserDialog.SelectedPath = this.App.Project.OutputDirectory;
      bool? nullable = folderBrowserDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.GetValueOrDefault())
        return;
      this.App.Project.OutputDirectory = folderBrowserDialog.SelectedPath;
    }));

    public ICommand Add => (ICommand) new RelayCommand((Action) (() =>
    {
      VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
      vistaOpenFileDialog.Filter = ".NET assemblies (*.exe, *.dll)|*.exe;*.dll|All Files (*.*)|*.*";
      vistaOpenFileDialog.Multiselect = true;
      bool? nullable = vistaOpenFileDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.GetValueOrDefault())
        return;
      foreach (string fileName in vistaOpenFileDialog.FileNames)
        this.AddModule(fileName);
    }));

    public ICommand Remove => (ICommand) new RelayCommand((Action) (() =>
    {
      Debug.Assert(this.App.Project.Modules.Any<ProjectModuleVM>((Func<ProjectModuleVM, bool>) (m => m.IsSelected)));
      if (MessageBox.Show("Are you sure to remove selected modules?\r\nAll settings specific to it would be lost!", "ConfuserEx", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        return;
      foreach (ProjectModuleVM projectModuleVm in this.App.Project.Modules.Where<ProjectModuleVM>((Func<ProjectModuleVM, bool>) (m => m.IsSelected)).ToList<ProjectModuleVM>())
        this.App.Project.Modules.Remove(projectModuleVm);
    }), (Func<bool>) (() => this.App.Project.Modules.Any<ProjectModuleVM>((Func<ProjectModuleVM, bool>) (m => m.IsSelected))));

    public ICommand Edit => (ICommand) new RelayCommand((Action) (() =>
    {
      Debug.Assert(this.App.Project.Modules.Count<ProjectModuleVM>((Func<ProjectModuleVM, bool>) (m => m.IsSelected)) == 1);
      new ProjectModuleView(this.App.Project.Modules.Single<ProjectModuleVM>((Func<ProjectModuleVM, bool>) (m => m.IsSelected)))
      {
        Owner = Application.Current.MainWindow
      }.ShowDialog();
    }), (Func<bool>) (() => this.App.Project.Modules.Count<ProjectModuleVM>((Func<ProjectModuleVM, bool>) (m => m.IsSelected)) == 1));

    public ICommand Advanced => (ICommand) new RelayCommand((Action) (() =>
    {
      new ProjectTabAdvancedView(this.App.Project)
      {
        Owner = Application.Current.MainWindow
      }.ShowDialog();
    }));

    private void AddModule(string file)
    {
      if (!File.Exists(file))
      {
        int num = (int) MessageBox.Show(string.Format("File '{0}' does not exists!", (object) file), "ConfuserEx", MessageBoxButton.OK, MessageBoxImage.Hand);
      }
      else
      {
        if (string.IsNullOrEmpty(this.App.Project.BaseDirectory))
        {
          string directoryName = Path.GetDirectoryName(file);
          this.App.Project.BaseDirectory = directoryName;
          this.App.Project.OutputDirectory = Path.Combine(directoryName, "NETSecure");
        }
        ProjectModuleVM projectModuleVm = new ProjectModuleVM(this.App.Project, new ProjectModule());
        try
        {
          projectModuleVm.Path = Confuser.Core.Utils.GetRelativePath(file, this.App.Project.BaseDirectory);
        }
        catch
        {
          projectModuleVm.Path = file;
        }
        this.App.Project.Modules.Add(projectModuleVm);
      }
    }
  }
}
