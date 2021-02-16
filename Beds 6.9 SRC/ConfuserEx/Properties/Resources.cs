using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ConfuserEx.Properties
{
	// Token: 0x02000032 RID: 50
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06000182 RID: 386 RVA: 0x00002076 File Offset: 0x00000276
		internal Resources()
		{
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00006968 File Offset: 0x00004B68
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				bool flag = Resources.resourceMan == null;
				if (flag)
				{
					ResourceManager resourceManager = new ResourceManager("ConfuserEx.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000184 RID: 388 RVA: 0x000069B0 File Offset: 0x00004BB0
		// (set) Token: 0x06000185 RID: 389 RVA: 0x00002B7E File Offset: 0x00000D7E
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x0400008A RID: 138
		private static ResourceManager resourceMan;

		// Token: 0x0400008B RID: 139
		private static CultureInfo resourceCulture;
	}
}
