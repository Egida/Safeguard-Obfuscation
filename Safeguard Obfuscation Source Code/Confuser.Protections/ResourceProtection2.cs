using System;
using Confuser.Core;
using Confuser.Protections.Resources2;

namespace Confuser.Protections
{
	// Token: 0x02000039 RID: 57
	[AfterProtection(new string[]
	{
		"Ki.Constants"
	})]
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.ControlFlow"
	})]
	internal class ResourceProtection2 : Protection
	{
		// Token: 0x0600011A RID: 282 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00004F75 File Offset: 0x00003175
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600011C RID: 284 RVA: 0x0000A844 File Offset: 0x00008A44
		public override string Description
		{
			get
			{
				return "This protection dynamically encodes and compresses the embedded resources.";
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600011D RID: 285 RVA: 0x0000A85C File Offset: 0x00008A5C
		public override string FullId
		{
			get
			{
				return "Ki.Resources2";
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600011E RID: 286 RVA: 0x0000A874 File Offset: 0x00008A74
		public override string Id
		{
			get
			{
				return "resources2";
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600011F RID: 287 RVA: 0x0000A88C File Offset: 0x00008A8C
		public override string Name
		{
			get
			{
				return "Dynamic Resource Protection";
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00007388 File Offset: 0x00005588
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00004A48 File Offset: 0x00002C48
		public ResourceProtection2()
		{
		}

		// Token: 0x04000065 RID: 101
		public const string _FullId = "Ki.Resources2";

		// Token: 0x04000066 RID: 102
		public const string _Id = "dy resources";

		// Token: 0x04000067 RID: 103
		public const string _ServiceId = "Ki.Resources2";
	}
}
