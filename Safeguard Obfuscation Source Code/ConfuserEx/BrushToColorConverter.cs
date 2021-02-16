using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ConfuserEx
{
  public class BrushToColorConverter : IValueConverter
  {
    public static readonly BrushToColorConverter Instance = new BrushToColorConverter();

    private BrushToColorConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is SolidColorBrush solidColorBrush ? (object) solidColorBrush.Color : (object) null;

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
