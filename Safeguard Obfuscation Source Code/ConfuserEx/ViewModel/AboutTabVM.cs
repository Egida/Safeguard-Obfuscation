using GalaSoft.MvvmLight.Command;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ConfuserEx.ViewModel
{
  internal class AboutTabVM : TabViewModel
  {
    public AboutTabVM(AppVM app)
      : base(app, "About")
      => this.Icon = (BitmapSource) new IconBitmapDecoder(new Uri("pack://application:,,,/ConfuserEx.ico"), BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default).Frames.First<BitmapFrame>((Func<BitmapFrame, bool>) (frame => frame.Width == 64.0));

    public ICommand LaunchBrowser => (ICommand) new RelayCommand<string>((Action<string>) (site => Process.Start(site)));

    public BitmapSource Icon { get; private set; }
  }
}
