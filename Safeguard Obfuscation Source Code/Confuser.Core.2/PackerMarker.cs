using System;
using Confuser.Core.Project;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x02000060 RID: 96
	internal class PackerMarker : Marker
	{
		// Token: 0x06000240 RID: 576 RVA: 0x00002F87 File Offset: 0x00001187
		public PackerMarker(StrongNameKey snKey)
		{
			this.snKey = snKey;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00011398 File Offset: 0x0000F598
		protected internal override MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
		{
			MarkerResult result = base.MarkProject(proj, context);
			foreach (ModuleDefMD module in result.Modules)
			{
				context.Annotations.Set<StrongNameKey>(module, Marker.SNKey, this.snKey);
			}
			return result;
		}

		// Token: 0x040001C7 RID: 455
		private readonly StrongNameKey snKey;
	}
}
