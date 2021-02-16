using System;
using Confuser.Core;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000A8 RID: 168
	internal interface IModeHandler
	{
		// Token: 0x06000283 RID: 643
		void HandleInject(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters);

		// Token: 0x06000284 RID: 644
		void HandleMD(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters);
	}
}
