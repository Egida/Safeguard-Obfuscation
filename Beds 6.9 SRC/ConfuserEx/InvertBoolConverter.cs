using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace ConfuserEx
{
	// Token: 0x02000011 RID: 17
	internal class InvertBoolConverter : IValueConverter
	{
		// Token: 0x0600004A RID: 74 RVA: 0x00002076 File Offset: 0x00000276
		private InvertBoolConverter()
		{
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000037D8 File Offset: 0x000019D8
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.Assert(value is bool);
			Debug.Assert(targetType == typeof(bool));
			return !(bool)value;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000216A File Offset: 0x0000036A
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000021EF File Offset: 0x000003EF
		// Note: this type is marked as 'beforefieldinit'.
		static InvertBoolConverter()
		{
		}

		// Token: 0x0400001D RID: 29
		public static readonly InvertBoolConverter Instance = new InvertBoolConverter();
	}
}
