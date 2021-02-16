using Confuser.Core;
using Confuser.Renamer;
using Ookii.Dialogs.Wpf;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace ConfuserEx
{
  public partial class StackTraceDecoder : Window, IComponentConnector
  {
    private readonly Dictionary<string, string> symMap = new Dictionary<string, string>();
    private readonly Regex mapSymbolMatcher = new Regex("_[a-zA-Z0-9]+");
    private readonly Regex passSymbolMatcher = new Regex("[a-zA-Z0-9_$]{23,}");
    private ReversibleRenamer renamer;
    internal RadioButton optSym;
    internal RadioButton optPass;
    internal TextBox PathBox;
    internal TextBox PassBox;
    internal Label status;
    internal TextBox stackTrace;
    private bool _contentLoaded;

    public StackTraceDecoder() => this.InitializeComponent();

    private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!File.Exists(this.PathBox.Text))
        return;
      this.LoadSymMap(this.PathBox.Text);
    }

    private void LoadSymMap(string path)
    {
      string str1 = path;
      if (path.Length > 35)
        str1 = "..." + path.Substring(path.Length - 35, 35);
      try
      {
        this.symMap.Clear();
        using (StreamReader streamReader = new StreamReader((Stream) File.OpenRead(path)))
        {
          for (string str2 = streamReader.ReadLine(); str2 != null; str2 = streamReader.ReadLine())
          {
            int length = str2.IndexOf('\t');
            if (length == -1)
              throw new FileFormatException();
            this.symMap.Add(str2.Substring(0, length), str2.Substring(length + 1));
          }
        }
        this.status.Content = (object) ("Loaded symbol map from '" + str1 + "' successfully.");
      }
      catch
      {
        this.status.Content = (object) ("Failed to load symbol map from '" + str1 + "'.");
      }
    }

    private void ChooseMapPath(object sender, RoutedEventArgs e)
    {
      VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
      vistaOpenFileDialog.Filter = "Symbol maps (*.map)|*.map|All Files (*.*)|*.*";
      bool? nullable = vistaOpenFileDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.GetValueOrDefault())
        return;
      this.PathBox.Text = vistaOpenFileDialog.FileName;
    }

    private void Decode_Click(object sender, RoutedEventArgs e)
    {
      string text = this.stackTrace.Text;
      bool? isChecked = this.optSym.IsChecked;
      if (!isChecked.HasValue || isChecked.GetValueOrDefault())
      {
        this.stackTrace.Text = this.mapSymbolMatcher.Replace(text, new MatchEvaluator(this.DecodeSymbolMap));
      }
      else
      {
        this.renamer = new ReversibleRenamer(this.PassBox.Text);
        this.stackTrace.Text = this.passSymbolMatcher.Replace(text, new MatchEvaluator(this.DecodeSymbolPass));
      }
    }

    private string DecodeSymbolMap(Match match)
    {
      string str = match.Value;
      return this.symMap.GetValueOrDefault<string, string>(str, str);
    }

    private string DecodeSymbolPass(Match match)
    {
      string name = match.Value;
      try
      {
        return this.renamer.Decrypt(name);
      }
      catch
      {
        return name;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/ConfuserEx;component/stacktracedecoder.xaml", UriKind.Relative));
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
          this.optSym = (RadioButton) target;
          break;
        case 2:
          this.optPass = (RadioButton) target;
          break;
        case 3:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.ChooseMapPath);
          break;
        case 4:
          this.PathBox = (TextBox) target;
          this.PathBox.TextChanged += new TextChangedEventHandler(this.PathBox_TextChanged);
          break;
        case 5:
          this.PassBox = (TextBox) target;
          break;
        case 6:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.Decode_Click);
          break;
        case 7:
          this.status = (Label) target;
          break;
        case 8:
          this.stackTrace = (TextBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
