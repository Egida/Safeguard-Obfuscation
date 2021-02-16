using Confuser.Core;
using Confuser.Core.Project;
using GalaSoft.MvvmLight.Command;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ConfuserEx.ViewModel
{
  internal class ProtectTabVM : TabViewModel, ILogger
  {
    private readonly Paragraph documentContent;
    private CancellationTokenSource cancelSrc;
    private double? progress = new double?(0.0);
    private bool? result;
    private DateTime begin;

    public ProtectTabVM(AppVM app)
      : base(app, "Protect!")
    {
      this.documentContent = new Paragraph();
      this.LogDocument = new FlowDocument();
      this.LogDocument.Blocks.Add((Block) this.documentContent);
    }

    public ICommand ProtectCmd => (ICommand) new RelayCommand(new Action(this.DoProtect), (Func<bool>) (() => !this.App.NavigationDisabled));

    public ICommand CancelCmd => (ICommand) new RelayCommand(new Action(this.DoCancel), (Func<bool>) (() => this.App.NavigationDisabled));

    public double? Progress
    {
      get => this.progress;
      set => this.SetProperty<double?>(ref this.progress, value, nameof (Progress));
    }

    public FlowDocument LogDocument { get; private set; }

    public bool? Result
    {
      get => this.result;
      set => this.SetProperty<bool?>(ref this.result, value, nameof (Result));
    }

    private void DoProtect()
    {
      ConfuserParameters parameters = new ConfuserParameters();
      parameters.Project = ((IViewModel<ConfuserProject>) this.App.Project).Model;
      if (File.Exists(this.App.FileName))
        Environment.CurrentDirectory = Path.GetDirectoryName(this.App.FileName);
      parameters.Logger = (ILogger) this;
      this.documentContent.Inlines.Clear();
      this.cancelSrc = new CancellationTokenSource();
      this.Result = new bool?();
      this.Progress = new double?();
      this.begin = DateTime.Now;
      this.App.NavigationDisabled = true;
      ConfuserEngine.Run(parameters, new CancellationToken?(this.cancelSrc.Token)).ContinueWith<DispatcherOperation>((Func<Task, DispatcherOperation>) (_ => Application.Current.Dispatcher.BeginInvoke((Delegate) (() =>
      {
        this.Progress = new double?(0.0);
        this.App.NavigationDisabled = false;
        CommandManager.InvalidateRequerySuggested();
      }))));
    }

    private void DoCancel() => this.cancelSrc.Cancel();

    private void AppendLine(string format, Brush foreground, params object[] args) => Application.Current.Dispatcher.BeginInvoke((Delegate) (() =>
    {
      InlineCollection inlines = this.documentContent.Inlines;
      inlines.Add((Inline) new Run(string.Format(format, args))
      {
        Foreground = foreground
      });
      this.documentContent.Inlines.Add((Inline) new LineBreak());
    }));

    void ILogger.Debug(string msg) => this.AppendLine("[DEBUG] {0}", (Brush) Brushes.Gray, (object) msg);

    void ILogger.DebugFormat(string format, params object[] args) => this.AppendLine("[DEBUG] {0}", (Brush) Brushes.Gray, (object) string.Format(format, args));

    void ILogger.Info(string msg) => this.AppendLine(" [INFO] {0}", (Brush) Brushes.White, (object) msg);

    void ILogger.InfoFormat(string format, params object[] args) => this.AppendLine(" [INFO] {0}", (Brush) Brushes.White, (object) string.Format(format, args));

    void ILogger.Warn(string msg) => this.AppendLine(" [WARN] {0}", (Brush) Brushes.Yellow, (object) msg);

    void ILogger.WarnFormat(string format, params object[] args) => this.AppendLine(" [WARN] {0}", (Brush) Brushes.Yellow, (object) string.Format(format, args));

    void ILogger.WarnException(string msg, Exception ex)
    {
      this.AppendLine(" [WARN] {0}", (Brush) Brushes.Yellow, (object) msg);
      this.AppendLine("Exception: {0}", (Brush) Brushes.Yellow, (object) ex);
    }

    void ILogger.Error(string msg) => this.AppendLine("[ERROR] {0}", (Brush) Brushes.Red, (object) msg);

    void ILogger.ErrorFormat(string format, params object[] args) => this.AppendLine("[ERROR] {0}", (Brush) Brushes.Red, (object) string.Format(format, args));

    void ILogger.ErrorException(string msg, Exception ex)
    {
      this.AppendLine("[ERROR] {0}", (Brush) Brushes.Red, (object) msg);
      this.AppendLine("Exception: {0}", (Brush) Brushes.Red, (object) ex);
    }

    void ILogger.Progress(int progress, int overall) => this.Progress = new double?((double) progress / (double) overall);

    void ILogger.EndProgress() => this.Progress = new double?();

    void ILogger.Finish(bool successful)
    {
      DateTime now = DateTime.Now;
      string shortTimeString = now.ToShortTimeString();
      TimeSpan timeSpan = now.Subtract(this.begin);
      // ISSUE: variable of a boxed type
      __Boxed<int> totalMinutes = (ValueType) (int) timeSpan.TotalMinutes;
      timeSpan = now.Subtract(this.begin);
      // ISSUE: variable of a boxed type
      __Boxed<int> seconds = (ValueType) timeSpan.Seconds;
      string str = string.Format("at {0}, {1}:{2:d2} elapsed.", (object) shortTimeString, (object) totalMinutes, (object) seconds);
      if (successful)
        this.AppendLine("Finished {0}", (Brush) Brushes.Lime, (object) str);
      else
        this.AppendLine("Failed {0}", (Brush) Brushes.Red, (object) str);
      this.Result = new bool?(successful);
    }
  }
}
