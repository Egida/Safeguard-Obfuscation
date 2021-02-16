using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x02000078 RID: 120
	public interface IMarkerService
	{
		// Token: 0x060002C7 RID: 711
		ConfuserComponent GetHelperParent(IDnlibDef def);

		// Token: 0x060002C8 RID: 712
		bool IsMarked(IDnlibDef def);

		// Token: 0x060002C9 RID: 713
		void Mark(IDnlibDef member, ConfuserComponent parentComp);
	}
}
