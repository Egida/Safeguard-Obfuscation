using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ConfuserEx
{
	// Token: 0x02000012 RID: 18
	public partial class Skin
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00003820 File Offset: 0x00001A20
		public static string GetEmptyPrompt(DependencyObject obj)
		{
			return (string)obj.GetValue(Skin.EmptyPromptProperty);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000021FB File Offset: 0x000003FB
		public static void SetEmptyPrompt(DependencyObject obj, string value)
		{
			obj.SetValue(Skin.EmptyPromptProperty, value);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003844 File Offset: 0x00001A44
		public static bool GetFocusOverlay(DependencyObject obj)
		{
			return (bool)obj.GetValue(Skin.FocusOverlayProperty);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000220B File Offset: 0x0000040B
		public static void SetFocusOverlay(DependencyObject obj, bool value)
		{
			obj.SetValue(Skin.FocusOverlayProperty, value);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003868 File Offset: 0x00001A68
		public static bool GetTabsDisabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(Skin.TabsDisabledProperty);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002220 File Offset: 0x00000420
		public static void SetTabsDisabled(DependencyObject obj, bool value)
		{
			obj.SetValue(Skin.TabsDisabledProperty, value);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x0000388C File Offset: 0x00001A8C
		public static void OnRTBDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs dpe)
		{
			RichTextBox rtb = (RichTextBox)d;
			bool flag = dpe.NewValue != null;
			if (flag)
			{
				rtb.Document = (FlowDocument)dpe.NewValue;
				rtb.TextChanged += delegate(object sender, TextChangedEventArgs e)
				{
					rtb.ScrollToEnd();
				};
			}
			else
			{
				rtb.Document = new FlowDocument();
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003900 File Offset: 0x00001B00
		public static FlowDocument GetRTBDocument(DependencyObject obj)
		{
			return (FlowDocument)obj.GetValue(Skin.RTBDocumentProperty);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002235 File Offset: 0x00000435
		public static void SetRTBDocument(DependencyObject obj, FlowDocument value)
		{
			obj.SetValue(Skin.RTBDocumentProperty, value);
		}

		// Token: 0x0400001E RID: 30
		public static readonly DependencyProperty EmptyPromptProperty = DependencyProperty.RegisterAttached("EmptyPrompt", typeof(string), typeof(Skin), new UIPropertyMetadata(null));

		// Token: 0x0400001F RID: 31
		public static readonly DependencyProperty TabsDisabledProperty = DependencyProperty.RegisterAttached("TabsDisabled", typeof(bool), typeof(Skin), new UIPropertyMetadata(false));

		// Token: 0x04000020 RID: 32
		public static readonly DependencyProperty FocusOverlayProperty = DependencyProperty.RegisterAttached("FocusOverlay", typeof(bool), typeof(Skin), new UIPropertyMetadata(true));

		// Token: 0x04000021 RID: 33
		public static readonly DependencyProperty RTBDocumentProperty = DependencyProperty.RegisterAttached("RTBDocument", typeof(FlowDocument), typeof(Skin), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Skin.OnRTBDocumentChanged)));
	}
}
