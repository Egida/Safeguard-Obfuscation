using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000009 RID: 9
	internal class PostRenamePhase : ProtectionPhase
	{
		// Token: 0x0600004D RID: 77 RVA: 0x000020E0 File Offset: 0x000002E0
		public PostRenamePhase(NameProtection parent) : base(parent)
		{
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004F18 File Offset: 0x00003118
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			NameService service = (NameService)context.Registry.GetService<INameService>();
			foreach (IRenamer renamer in service.Renamers)
			{
				foreach (IDnlibDef def in parameters.Targets)
				{
					renamer.PostRename(context, service, parameters, def);
				}
				context.CheckCancellation();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00004FC4 File Offset: 0x000031C4
		public override string Name
		{
			get
			{
				return "Post-renaming";
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00003690 File Offset: 0x00001890
		public override bool ProcessAll
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000051 RID: 81 RVA: 0x000036A4 File Offset: 0x000018A4
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.AllDefinitions;
			}
		}
	}
}
