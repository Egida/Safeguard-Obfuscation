using System;
using Confuser.Core;
using Confuser.Protections.Resources;

namespace Confuser.Protections
{
	// Token: 0x0200003A RID: 58
	[AfterProtection(new string[]
	{
		"Ki.Constants"
	})]
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.ControlFlow"
	})]
	internal class ResourceProtection : Protection
	{
		// Token: 0x06000122 RID: 290 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00004F86 File Offset: 0x00003186
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000124 RID: 292 RVA: 0x0000A8A4 File Offset: 0x00008AA4
		public override string Description
		{
			get
			{
				return "This protection encodes and compresses the embedded resources.";
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000125 RID: 293 RVA: 0x0000A8BC File Offset: 0x00008ABC
		public override string FullId
		{
			get
			{
				return "Ki.Resources";
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000126 RID: 294 RVA: 0x0000A8D4 File Offset: 0x00008AD4
		public override string Id
		{
			get
			{
				return "resources";
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000127 RID: 295 RVA: 0x0000A8EC File Offset: 0x00008AEC
		public override string Name
		{
			get
			{
				return "Resources Protection";
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00007388 File Offset: 0x00005588
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00004A48 File Offset: 0x00002C48
		public ResourceProtection()
		{
		}

		// Token: 0x04000068 RID: 104
		public const string _FullId = "Ki.Resources";

		// Token: 0x04000069 RID: 105
		public const string _Id = "resources";

		// Token: 0x0400006A RID: 106
		public const string _ServiceId = "Ki.Resources";
	}
}
