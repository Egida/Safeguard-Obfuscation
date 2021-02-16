using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	// Token: 0x02000061 RID: 97
	internal class PackerDiscovery : PluginDiscovery
	{
		// Token: 0x06000242 RID: 578 RVA: 0x00002F98 File Offset: 0x00001198
		public PackerDiscovery(Protection prot)
		{
			this.prot = prot;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00002FA9 File Offset: 0x000011A9
		protected override void GetPluginsInternal(ConfuserContext context, IList<Protection> protections, IList<Packer> packers, IList<ConfuserComponent> components)
		{
			base.GetPluginsInternal(context, protections, packers, components);
			protections.Add(this.prot);
		}

		// Token: 0x040001C8 RID: 456
		private readonly Protection prot;
	}
}
