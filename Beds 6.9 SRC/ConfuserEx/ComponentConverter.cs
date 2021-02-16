using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using Confuser.Core;

namespace ConfuserEx
{
	// Token: 0x02000005 RID: 5
	internal class ComponentConverter : Freezable, IValueConverter
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002D90 File Offset: 0x00000F90
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000020D5 File Offset: 0x000002D5
		public IList<ConfuserComponent> Components
		{
			get
			{
				return (IList<ConfuserComponent>)base.GetValue(ComponentConverter.ComponentsProperty);
			}
			set
			{
				base.SetValue(ComponentConverter.ComponentsProperty, value);
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002DB4 File Offset: 0x00000FB4
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.Assert(value is string || value == null);
			Debug.Assert(targetType == typeof(ConfuserComponent));
			Debug.Assert(this.Components != null);
			bool flag = value == null;
			object result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.Components.Single((ConfuserComponent comp) => comp.Id == (string)value);
			}
			return result;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002E40 File Offset: 0x00001040
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.Assert(value is ConfuserComponent || value == null);
			Debug.Assert(targetType == typeof(string));
			bool flag = value == null;
			object result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = ((ConfuserComponent)value).Id;
			}
			return result;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002E94 File Offset: 0x00001094
		protected override Freezable CreateInstanceCore()
		{
			return new ComponentConverter();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000020E5 File Offset: 0x000002E5
		public ComponentConverter()
		{
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000020EE File Offset: 0x000002EE
		// Note: this type is marked as 'beforefieldinit'.
		static ComponentConverter()
		{
		}

		// Token: 0x04000007 RID: 7
		public static readonly DependencyProperty ComponentsProperty = DependencyProperty.Register("Components", typeof(IList<ConfuserComponent>), typeof(ComponentConverter), new UIPropertyMetadata(null));

		// Token: 0x02000006 RID: 6
		[CompilerGenerated]
		private sealed class <>c__DisplayClass4_0
		{
			// Token: 0x0600001C RID: 28 RVA: 0x00002119 File Offset: 0x00000319
			public <>c__DisplayClass4_0()
			{
			}

			// Token: 0x0600001D RID: 29 RVA: 0x00002122 File Offset: 0x00000322
			internal bool <Convert>b__0(ConfuserComponent comp)
			{
				return comp.Id == (string)this.value;
			}

			// Token: 0x04000008 RID: 8
			public object value;
		}
	}
}
