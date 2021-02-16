using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Confuser.Core;

namespace ConfuserEx
{
	// Token: 0x02000004 RID: 4
	public partial class CompComboBox : UserControl
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002094 File Offset: 0x00000294
		public CompComboBox()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002C38 File Offset: 0x00000E38
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000020A5 File Offset: 0x000002A5
		public IEnumerable<ConfuserComponent> Components
		{
			get
			{
				return (IEnumerable<ConfuserComponent>)base.GetValue(CompComboBox.ComponentsProperty);
			}
			set
			{
				base.SetValue(CompComboBox.ComponentsProperty, value);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002C5C File Offset: 0x00000E5C
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000020B5 File Offset: 0x000002B5
		public ConfuserComponent SelectedComponent
		{
			get
			{
				return (ConfuserComponent)base.GetValue(CompComboBox.SelectedComponentProperty);
			}
			set
			{
				base.SetValue(CompComboBox.SelectedComponentProperty, value);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002C80 File Offset: 0x00000E80
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000020C5 File Offset: 0x000002C5
		public Dictionary<string, string> Arguments
		{
			get
			{
				return (Dictionary<string, string>)base.GetValue(CompComboBox.ArgumentsProperty);
			}
			set
			{
				base.SetValue(CompComboBox.ArgumentsProperty, value);
			}
		}

		// Token: 0x04000002 RID: 2
		public static readonly DependencyProperty ComponentsProperty = DependencyProperty.Register("Components", typeof(IEnumerable<ConfuserComponent>), typeof(CompComboBox), new UIPropertyMetadata(null));

		// Token: 0x04000003 RID: 3
		public static readonly DependencyProperty SelectedComponentProperty = DependencyProperty.Register("SelectedComponent", typeof(ConfuserComponent), typeof(CompComboBox), new UIPropertyMetadata(null));

		// Token: 0x04000004 RID: 4
		public static readonly DependencyProperty ArgumentsProperty = DependencyProperty.Register("Arguments", typeof(Dictionary<string, string>), typeof(CompComboBox), new UIPropertyMetadata(null));
	}
}
