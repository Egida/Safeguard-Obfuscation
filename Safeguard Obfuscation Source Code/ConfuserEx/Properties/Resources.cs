using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ConfuserEx.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ConfuserEx.Properties.Resources.resourceMan == null)
          ConfuserEx.Properties.Resources.resourceMan = new ResourceManager("ConfuserEx.Properties.Resources", typeof (ConfuserEx.Properties.Resources).Assembly);
        return ConfuserEx.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ConfuserEx.Properties.Resources.resourceCulture;
      set => ConfuserEx.Properties.Resources.resourceCulture = value;
    }
  }
}
