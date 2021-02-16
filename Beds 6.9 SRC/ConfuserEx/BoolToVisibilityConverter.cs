using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ConfuserEx
{
	// Token: 0x0200000C RID: 12
	internal class BoolToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000032 RID: 50 RVA: 0x00002076 File Offset: 0x00000276
		private BoolToVisibilityConverter()
		{
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000322C File Offset: 0x0000142C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.Assert(value is bool);
			Debug.Assert(targetType == typeof(Visibility));
			return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000216A File Offset: 0x0000036A
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000219A File Offset: 0x0000039A
		// Note: this type is marked as 'beforefieldinit'.
		static BoolToVisibilityConverter()
		{
		}

		// Token: 0x04000013 RID: 19
		public static readonly BoolToVisibilityConverter Instance = new BoolToVisibilityConverter();
	}
}
