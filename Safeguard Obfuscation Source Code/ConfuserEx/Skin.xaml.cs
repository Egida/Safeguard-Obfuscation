using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ConfuserEx
{
  public partial class Skin
  {
    public static readonly DependencyProperty EmptyPromptProperty = DependencyProperty.RegisterAttached("EmptyPrompt", typeof (string), typeof (Skin), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty TabsDisabledProperty = DependencyProperty.RegisterAttached("TabsDisabled", typeof (bool), typeof (Skin), (PropertyMetadata) new UIPropertyMetadata((object) false));
    public static readonly DependencyProperty FocusOverlayProperty = DependencyProperty.RegisterAttached("FocusOverlay", typeof (bool), typeof (Skin), (PropertyMetadata) new UIPropertyMetadata((object) true));
    public static readonly DependencyProperty RTBDocumentProperty = DependencyProperty.RegisterAttached("RTBDocument", typeof (FlowDocument), typeof (Skin), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(Skin.OnRTBDocumentChanged)));

    public static string GetEmptyPrompt(DependencyObject obj) => (string) obj.GetValue(Skin.EmptyPromptProperty);

    public static void SetEmptyPrompt(DependencyObject obj, string value) => obj.SetValue(Skin.EmptyPromptProperty, (object) value);

    public static bool GetFocusOverlay(DependencyObject obj) => (bool) obj.GetValue(Skin.FocusOverlayProperty);

    public static void SetFocusOverlay(DependencyObject obj, bool value) => obj.SetValue(Skin.FocusOverlayProperty, (object) value);

    public static bool GetTabsDisabled(DependencyObject obj) => (bool) obj.GetValue(Skin.TabsDisabledProperty);

    public static void SetTabsDisabled(DependencyObject obj, bool value) => obj.SetValue(Skin.TabsDisabledProperty, (object) value);

    public static void OnRTBDocumentChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs dpe)
    {
      RichTextBox rtb = (RichTextBox) d;
      if (dpe.NewValue != null)
      {
        rtb.Document = (FlowDocument) dpe.NewValue;
        rtb.TextChanged += (TextChangedEventHandler) ((sender, e) => rtb.ScrollToEnd());
      }
      else
        rtb.Document = new FlowDocument();
    }

    public static FlowDocument GetRTBDocument(DependencyObject obj) => (FlowDocument) obj.GetValue(Skin.RTBDocumentProperty);

    public static void SetRTBDocument(DependencyObject obj, FlowDocument value) => obj.SetValue(Skin.RTBDocumentProperty, (object) value);
  }
}
