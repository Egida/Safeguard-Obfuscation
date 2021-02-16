using Confuser.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ConfuserEx
{
  internal class ComponentConverter : Freezable, IValueConverter
  {
    public static readonly DependencyProperty ComponentsProperty = DependencyProperty.Register(nameof (Components), typeof (IList<ConfuserComponent>), typeof (ComponentConverter), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));

    public IList<ConfuserComponent> Components
    {
      get => (IList<ConfuserComponent>) this.GetValue(ComponentConverter.ComponentsProperty);
      set => this.SetValue(ComponentConverter.ComponentsProperty, (object) value);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Debug.Assert(value is string || value == null);
      Debug.Assert(targetType == typeof (ConfuserComponent));
      Debug.Assert(this.Components != null);
      return value == null ? (object) null : (object) this.Components.Single<ConfuserComponent>((Func<ConfuserComponent, bool>) (comp => comp.Id == (string) value));
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      Debug.Assert(value is ConfuserComponent || value == null);
      Debug.Assert(targetType == typeof (string));
      return value == null ? (object) null : (object) ((ConfuserComponent) value).Id;
    }

    protected override Freezable CreateInstanceCore() => (Freezable) new ComponentConverter();
  }
}
