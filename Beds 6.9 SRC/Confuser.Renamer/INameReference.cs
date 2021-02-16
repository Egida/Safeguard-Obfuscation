using System;
using Confuser.Core;

namespace Confuser.Renamer
{
	// Token: 0x0200000D RID: 13
	public interface INameReference
	{
		// Token: 0x06000062 RID: 98
		bool UpdateNameReference(ConfuserContext context, INameService service);

		// Token: 0x06000063 RID: 99
		bool ShouldCancelRename();
	}
}
