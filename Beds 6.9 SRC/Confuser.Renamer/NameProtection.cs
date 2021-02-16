using System;
using System.Collections.Generic;
using System.IO;
using Confuser.Core;

namespace Confuser.Renamer
{
	// Token: 0x0200000B RID: 11
	internal class NameProtection : Protection
	{
		// Token: 0x06000055 RID: 85 RVA: 0x000021C3 File Offset: 0x000003C3
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.Rename", typeof(INameService), new NameService(context));
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000021E7 File Offset: 0x000003E7
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPostStage(PipelineStage.Inspection, new AnalyzePhase(this));
			pipeline.InsertPostStage(PipelineStage.BeginModule, new RenamePhase(this));
			pipeline.InsertPreStage(PipelineStage.EndModule, new PostRenamePhase(this));
			pipeline.InsertPostStage(PipelineStage.SaveModules, new NameProtection.ExportMapPhase(this));
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00004FDC File Offset: 0x000031DC
		public override string Description
		{
			get
			{
				return "This protection obfuscate the symbols' name so the decompiled source code can neither be compiled nor read.";
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00004FF4 File Offset: 0x000031F4
		public override string FullId
		{
			get
			{
				return "Ki.Rename";
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000059 RID: 89 RVA: 0x0000500C File Offset: 0x0000320C
		public override string Id
		{
			get
			{
				return "rename";
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00005024 File Offset: 0x00003224
		public override string Name
		{
			get
			{
				return "Name Protection";
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600005B RID: 91 RVA: 0x0000503C File Offset: 0x0000323C
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002222 File Offset: 0x00000422
		public NameProtection()
		{
		}

		// Token: 0x0400001A RID: 26
		public const string _FullId = "Ki.Rename";

		// Token: 0x0400001B RID: 27
		public const string _Id = "rename";

		// Token: 0x0400001C RID: 28
		public const string _ServiceId = "Ki.Rename";

		// Token: 0x0200000C RID: 12
		private class ExportMapPhase : ProtectionPhase
		{
			// Token: 0x0600005D RID: 93 RVA: 0x000020E0 File Offset: 0x000002E0
			public ExportMapPhase(NameProtection parent) : base(parent)
			{
			}

			// Token: 0x0600005E RID: 94 RVA: 0x00005050 File Offset: 0x00003250
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				NameService srv = (NameService)context.Registry.GetService<INameService>();
				ICollection<KeyValuePair<string, string>> map = srv.GetNameMap();
				bool flag = map.Count == 0;
				if (!flag)
				{
					string path = Path.GetFullPath(Path.Combine(context.OutputDirectory, "symbols.map"));
					string dir = Path.GetDirectoryName(path);
					bool flag2 = !Directory.Exists(dir);
					if (flag2)
					{
						Directory.CreateDirectory(dir);
					}
					using (StreamWriter writer = new StreamWriter(File.OpenWrite(path)))
					{
						foreach (KeyValuePair<string, string> entry in map)
						{
							writer.WriteLine("{0}\t{1}", entry.Key, entry.Value);
						}
					}
				}
			}

			// Token: 0x1700000D RID: 13
			// (get) Token: 0x0600005F RID: 95 RVA: 0x00005144 File Offset: 0x00003344
			public override string Name
			{
				get
				{
					return "Export symbol map";
				}
			}

			// Token: 0x1700000E RID: 14
			// (get) Token: 0x06000060 RID: 96 RVA: 0x00003690 File Offset: 0x00001890
			public override bool ProcessAll
			{
				get
				{
					return true;
				}
			}

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x06000061 RID: 97 RVA: 0x0000515C File Offset: 0x0000335C
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}
		}
	}
}
