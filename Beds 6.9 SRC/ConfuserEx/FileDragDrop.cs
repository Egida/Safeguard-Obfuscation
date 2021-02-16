using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ConfuserEx.ViewModel;
using GalaSoft.MvvmLight.Command;

namespace ConfuserEx
{
	// Token: 0x0200000E RID: 14
	public class FileDragDrop
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00003294 File Offset: 0x00001494
		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(FileDragDrop.CommandProperty);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000021B7 File Offset: 0x000003B7
		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(FileDragDrop.CommandProperty, value);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000032B8 File Offset: 0x000014B8
		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UIElement uielement = (UIElement)d;
			bool flag = e.NewValue != null;
			if (flag)
			{
				uielement.AllowDrop = true;
				uielement.PreviewDragOver += FileDragDrop.OnDragOver;
				uielement.PreviewDrop += FileDragDrop.OnDrop;
			}
			else
			{
				uielement.AllowDrop = false;
				uielement.PreviewDragOver -= FileDragDrop.OnDragOver;
				uielement.PreviewDrop -= FileDragDrop.OnDrop;
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003340 File Offset: 0x00001540
		private static void OnDragOver(object sender, DragEventArgs e)
		{
			ICommand command = FileDragDrop.GetCommand((DependencyObject)sender);
			e.Effects = DragDropEffects.None;
			bool flag = command is FileDragDrop.DragDropCommand;
			if (flag)
			{
				bool flag2 = command.CanExecute(Tuple.Create<UIElement, IDataObject>((UIElement)sender, e.Data));
				if (flag2)
				{
					e.Effects = DragDropEffects.Link;
				}
			}
			else
			{
				bool flag3 = command.CanExecute(e.Data);
				if (flag3)
				{
					e.Effects = DragDropEffects.Link;
				}
			}
			e.Handled = true;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000033B8 File Offset: 0x000015B8
		private static void OnDrop(object sender, DragEventArgs e)
		{
			ICommand command = FileDragDrop.GetCommand((DependencyObject)sender);
			bool flag = command is FileDragDrop.DragDropCommand;
			if (flag)
			{
				bool flag2 = command.CanExecute(Tuple.Create<UIElement, IDataObject>((UIElement)sender, e.Data));
				if (flag2)
				{
					command.Execute(Tuple.Create<UIElement, IDataObject>((UIElement)sender, e.Data));
				}
			}
			else
			{
				bool flag3 = command.CanExecute(e.Data);
				if (flag3)
				{
					command.Execute(e.Data);
				}
			}
			e.Handled = true;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002119 File Offset: 0x00000319
		public FileDragDrop()
		{
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003440 File Offset: 0x00001640
		// Note: this type is marked as 'beforefieldinit'.
		static FileDragDrop()
		{
		}

		// Token: 0x04000015 RID: 21
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(FileDragDrop), new UIPropertyMetadata(null, new PropertyChangedCallback(FileDragDrop.OnCommandChanged)));

		// Token: 0x04000016 RID: 22
		public static ICommand FileCmd = new FileDragDrop.DragDropCommand(delegate(Tuple<UIElement, IDataObject> data)
		{
			Debug.Assert(data.Item2.GetDataPresent(DataFormats.FileDrop));
			bool flag = data.Item1 is TextBox;
			if (flag)
			{
				string text = ((string[])data.Item2.GetData(DataFormats.FileDrop))[0];
				Debug.Assert(File.Exists(text));
				((TextBox)data.Item1).Text = text;
			}
			else
			{
				bool flag2 = data.Item1 is ListBox;
				if (!flag2)
				{
					throw new NotSupportedException();
				}
				string[] array = (string[])data.Item2.GetData(DataFormats.FileDrop);
				Debug.Assert(array.All((string file) => File.Exists(file)));
				IList<StringItem> list = (IList<StringItem>)((ListBox)data.Item1).ItemsSource;
				foreach (string item in array)
				{
					list.Add(new StringItem(item));
				}
			}
		}, delegate(Tuple<UIElement, IDataObject> data)
		{
			bool flag = !data.Item2.GetDataPresent(DataFormats.FileDrop);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				result = ((string[])data.Item2.GetData(DataFormats.FileDrop)).All((string file) => File.Exists(file));
			}
			return result;
		});

		// Token: 0x04000017 RID: 23
		public static ICommand DirectoryCmd = new FileDragDrop.DragDropCommand(delegate(Tuple<UIElement, IDataObject> data)
		{
			Debug.Assert(data.Item2.GetDataPresent(DataFormats.FileDrop));
			bool flag = data.Item1 is TextBox;
			if (flag)
			{
				string text = ((string[])data.Item2.GetData(DataFormats.FileDrop))[0];
				Debug.Assert(Directory.Exists(text));
				((TextBox)data.Item1).Text = text;
			}
			else
			{
				bool flag2 = data.Item1 is ListBox;
				if (!flag2)
				{
					throw new NotSupportedException();
				}
				string[] array = (string[])data.Item2.GetData(DataFormats.FileDrop);
				Debug.Assert(array.All((string dir) => Directory.Exists(dir)));
				IList<StringItem> list = (IList<StringItem>)((ListBox)data.Item1).ItemsSource;
				foreach (string item in array)
				{
					list.Add(new StringItem(item));
				}
			}
		}, delegate(Tuple<UIElement, IDataObject> data)
		{
			bool flag = !data.Item2.GetDataPresent(DataFormats.FileDrop);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				result = ((string[])data.Item2.GetData(DataFormats.FileDrop)).All((string dir) => Directory.Exists(dir));
			}
			return result;
		});

		// Token: 0x0200000F RID: 15
		private class DragDropCommand : RelayCommand<Tuple<UIElement, IDataObject>>
		{
			// Token: 0x0600003F RID: 63 RVA: 0x000021C7 File Offset: 0x000003C7
			public DragDropCommand(Action<Tuple<UIElement, IDataObject>> execute, Func<Tuple<UIElement, IDataObject>, bool> canExecute) : base(execute, canExecute)
			{
			}
		}

		// Token: 0x02000010 RID: 16
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000040 RID: 64 RVA: 0x000021D3 File Offset: 0x000003D3
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000041 RID: 65 RVA: 0x00002119 File Offset: 0x00000319
			public <>c()
			{
			}

			// Token: 0x06000042 RID: 66 RVA: 0x000034D8 File Offset: 0x000016D8
			internal void <.cctor>b__10_0(Tuple<UIElement, IDataObject> data)
			{
				Debug.Assert(data.Item2.GetDataPresent(DataFormats.FileDrop));
				bool flag = data.Item1 is TextBox;
				if (flag)
				{
					string text = ((string[])data.Item2.GetData(DataFormats.FileDrop))[0];
					Debug.Assert(File.Exists(text));
					((TextBox)data.Item1).Text = text;
				}
				else
				{
					bool flag2 = data.Item1 is ListBox;
					if (!flag2)
					{
						throw new NotSupportedException();
					}
					string[] array = (string[])data.Item2.GetData(DataFormats.FileDrop);
					Debug.Assert(array.All((string file) => File.Exists(file)));
					IList<StringItem> list = (IList<StringItem>)((ListBox)data.Item1).ItemsSource;
					foreach (string item in array)
					{
						list.Add(new StringItem(item));
					}
				}
			}

			// Token: 0x06000043 RID: 67 RVA: 0x000021DF File Offset: 0x000003DF
			internal bool <.cctor>b__10_1(string file)
			{
				return File.Exists(file);
			}

			// Token: 0x06000044 RID: 68 RVA: 0x000035F0 File Offset: 0x000017F0
			internal bool <.cctor>b__10_2(Tuple<UIElement, IDataObject> data)
			{
				bool flag = !data.Item2.GetDataPresent(DataFormats.FileDrop);
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					result = ((string[])data.Item2.GetData(DataFormats.FileDrop)).All((string file) => File.Exists(file));
				}
				return result;
			}

			// Token: 0x06000045 RID: 69 RVA: 0x000021DF File Offset: 0x000003DF
			internal bool <.cctor>b__10_3(string file)
			{
				return File.Exists(file);
			}

			// Token: 0x06000046 RID: 70 RVA: 0x00003658 File Offset: 0x00001858
			internal void <.cctor>b__10_4(Tuple<UIElement, IDataObject> data)
			{
				Debug.Assert(data.Item2.GetDataPresent(DataFormats.FileDrop));
				bool flag = data.Item1 is TextBox;
				if (flag)
				{
					string text = ((string[])data.Item2.GetData(DataFormats.FileDrop))[0];
					Debug.Assert(Directory.Exists(text));
					((TextBox)data.Item1).Text = text;
				}
				else
				{
					bool flag2 = data.Item1 is ListBox;
					if (!flag2)
					{
						throw new NotSupportedException();
					}
					string[] array = (string[])data.Item2.GetData(DataFormats.FileDrop);
					Debug.Assert(array.All((string dir) => Directory.Exists(dir)));
					IList<StringItem> list = (IList<StringItem>)((ListBox)data.Item1).ItemsSource;
					foreach (string item in array)
					{
						list.Add(new StringItem(item));
					}
				}
			}

			// Token: 0x06000047 RID: 71 RVA: 0x000021E7 File Offset: 0x000003E7
			internal bool <.cctor>b__10_5(string dir)
			{
				return Directory.Exists(dir);
			}

			// Token: 0x06000048 RID: 72 RVA: 0x00003770 File Offset: 0x00001970
			internal bool <.cctor>b__10_6(Tuple<UIElement, IDataObject> data)
			{
				bool flag = !data.Item2.GetDataPresent(DataFormats.FileDrop);
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					result = ((string[])data.Item2.GetData(DataFormats.FileDrop)).All((string dir) => Directory.Exists(dir));
				}
				return result;
			}

			// Token: 0x06000049 RID: 73 RVA: 0x000021E7 File Offset: 0x000003E7
			internal bool <.cctor>b__10_7(string dir)
			{
				return Directory.Exists(dir);
			}

			// Token: 0x04000018 RID: 24
			public static readonly FileDragDrop.<>c <>9 = new FileDragDrop.<>c();

			// Token: 0x04000019 RID: 25
			public static Func<string, bool> <>9__10_1;

			// Token: 0x0400001A RID: 26
			public static Func<string, bool> <>9__10_3;

			// Token: 0x0400001B RID: 27
			public static Func<string, bool> <>9__10_5;

			// Token: 0x0400001C RID: 28
			public static Func<string, bool> <>9__10_7;
		}
	}
}
