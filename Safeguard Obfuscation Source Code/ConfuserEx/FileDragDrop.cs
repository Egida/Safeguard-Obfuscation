using ConfuserEx.ViewModel;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConfuserEx
{
  public class FileDragDrop
  {
    public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (FileDragDrop), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(FileDragDrop.OnCommandChanged)));
    public static ICommand FileCmd = (ICommand) new FileDragDrop.DragDropCommand((Action<Tuple<UIElement, IDataObject>>) (data =>
    {
      Debug.Assert(data.Item2.GetDataPresent(DataFormats.FileDrop));
      if (data.Item1 is TextBox)
      {
        string path = ((string[]) data.Item2.GetData(DataFormats.FileDrop))[0];
        Debug.Assert(File.Exists(path));
        ((TextBox) data.Item1).Text = path;
      }
      else
      {
        if (!(data.Item1 is ListBox))
          throw new NotSupportedException();
        string[] data1 = (string[]) data.Item2.GetData(DataFormats.FileDrop);
        Debug.Assert(((IEnumerable<string>) data1).All<string>((Func<string, bool>) (file => File.Exists(file))));
        IList<StringItem> itemsSource = (IList<StringItem>) ((ItemsControl) data.Item1).ItemsSource;
        foreach (string str in data1)
          itemsSource.Add(new StringItem(str));
      }
    }), (Func<Tuple<UIElement, IDataObject>, bool>) (data => data.Item2.GetDataPresent(DataFormats.FileDrop) && ((IEnumerable<string>) (string[]) data.Item2.GetData(DataFormats.FileDrop)).All<string>((Func<string, bool>) (file => File.Exists(file)))));
    public static ICommand DirectoryCmd = (ICommand) new FileDragDrop.DragDropCommand((Action<Tuple<UIElement, IDataObject>>) (data =>
    {
      Debug.Assert(data.Item2.GetDataPresent(DataFormats.FileDrop));
      if (data.Item1 is TextBox)
      {
        string path = ((string[]) data.Item2.GetData(DataFormats.FileDrop))[0];
        Debug.Assert(Directory.Exists(path));
        ((TextBox) data.Item1).Text = path;
      }
      else
      {
        if (!(data.Item1 is ListBox))
          throw new NotSupportedException();
        string[] data1 = (string[]) data.Item2.GetData(DataFormats.FileDrop);
        Debug.Assert(((IEnumerable<string>) data1).All<string>((Func<string, bool>) (dir => Directory.Exists(dir))));
        IList<StringItem> itemsSource = (IList<StringItem>) ((ItemsControl) data.Item1).ItemsSource;
        foreach (string str in data1)
          itemsSource.Add(new StringItem(str));
      }
    }), (Func<Tuple<UIElement, IDataObject>, bool>) (data => data.Item2.GetDataPresent(DataFormats.FileDrop) && ((IEnumerable<string>) (string[]) data.Item2.GetData(DataFormats.FileDrop)).All<string>((Func<string, bool>) (dir => Directory.Exists(dir)))));

    public static ICommand GetCommand(DependencyObject obj) => (ICommand) obj.GetValue(FileDragDrop.CommandProperty);

    public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(FileDragDrop.CommandProperty, (object) value);

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      UIElement uiElement = (UIElement) d;
      if (e.NewValue != null)
      {
        uiElement.AllowDrop = true;
        uiElement.PreviewDragOver += new DragEventHandler(FileDragDrop.OnDragOver);
        uiElement.PreviewDrop += new DragEventHandler(FileDragDrop.OnDrop);
      }
      else
      {
        uiElement.AllowDrop = false;
        uiElement.PreviewDragOver -= new DragEventHandler(FileDragDrop.OnDragOver);
        uiElement.PreviewDrop -= new DragEventHandler(FileDragDrop.OnDrop);
      }
    }

    private static void OnDragOver(object sender, DragEventArgs e)
    {
      ICommand command = FileDragDrop.GetCommand((DependencyObject) sender);
      e.Effects = DragDropEffects.None;
      if (command is FileDragDrop.DragDropCommand)
      {
        if (command.CanExecute((object) Tuple.Create<UIElement, IDataObject>((UIElement) sender, e.Data)))
          e.Effects = DragDropEffects.Link;
      }
      else if (command.CanExecute((object) e.Data))
        e.Effects = DragDropEffects.Link;
      e.Handled = true;
    }

    private static void OnDrop(object sender, DragEventArgs e)
    {
      ICommand command = FileDragDrop.GetCommand((DependencyObject) sender);
      if (command is FileDragDrop.DragDropCommand)
      {
        if (command.CanExecute((object) Tuple.Create<UIElement, IDataObject>((UIElement) sender, e.Data)))
          command.Execute((object) Tuple.Create<UIElement, IDataObject>((UIElement) sender, e.Data));
      }
      else if (command.CanExecute((object) e.Data))
        command.Execute((object) e.Data);
      e.Handled = true;
    }

    private class DragDropCommand : RelayCommand<Tuple<UIElement, IDataObject>>
    {
      public DragDropCommand(
        Action<Tuple<UIElement, IDataObject>> execute,
        Func<Tuple<UIElement, IDataObject>, bool> canExecute)
        : base(execute, canExecute)
      {
      }
    }
  }
}
