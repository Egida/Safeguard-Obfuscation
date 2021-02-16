using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x0200007D RID: 125
	internal class MarkerService : IMarkerService
	{
		// Token: 0x060002DC RID: 732 RVA: 0x000033C1 File Offset: 0x000015C1
		public MarkerService(ConfuserContext context, Marker marker)
		{
			this.context = context;
			this.marker = marker;
			this.helperParents = new Dictionary<IDnlibDef, ConfuserComponent>();
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00012B10 File Offset: 0x00010D10
		public ConfuserComponent GetHelperParent(IDnlibDef def)
		{
			ConfuserComponent parent;
			bool flag = !this.helperParents.TryGetValue(def, out parent);
			ConfuserComponent result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = parent;
			}
			return result;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00012B40 File Offset: 0x00010D40
		public bool IsMarked(IDnlibDef def)
		{
			return ProtectionParameters.GetParameters(this.context, def) != null;
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00012B64 File Offset: 0x00010D64
		public void Mark(IDnlibDef member, ConfuserComponent parentComp)
		{
			bool flag = member == null;
			if (flag)
			{
				throw new ArgumentNullException("member");
			}
			bool flag2 = member is ModuleDef;
			if (flag2)
			{
				throw new ArgumentException("New ModuleDef cannot be marked.");
			}
			bool flag3 = this.IsMarked(member);
			if (!flag3)
			{
				this.marker.MarkMember(member, this.context);
				bool flag4 = parentComp != null;
				if (flag4)
				{
					this.helperParents[member] = parentComp;
				}
			}
		}

		// Token: 0x04000227 RID: 551
		private readonly ConfuserContext context;

		// Token: 0x04000228 RID: 552
		private readonly Dictionary<IDnlibDef, ConfuserComponent> helperParents;

		// Token: 0x04000229 RID: 553
		private readonly Marker marker;
	}
}
