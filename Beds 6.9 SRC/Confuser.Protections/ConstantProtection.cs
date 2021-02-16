using System;
using Confuser.Core;
using Confuser.Protections.Constants;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000024 RID: 36
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	[AfterProtection(new string[]
	{
		"Ki.RefProxy"
	})]
	internal class ConstantProtection : Protection, IConstantService
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000093B0 File Offset: 0x000075B0
		public override string Name
		{
			get
			{
				return "Constants Protection";
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x000093C8 File Offset: 0x000075C8
		public override string Description
		{
			get
			{
				return "This protection encodes and compresses constants in the code.";
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x000093E0 File Offset: 0x000075E0
		public override string Id
		{
			get
			{
				return "constants";
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x000093F8 File Offset: 0x000075F8
		public override string FullId
		{
			get
			{
				return "Ki.Constants";
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x00007388 File Offset: 0x00005588
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004B53 File Offset: 0x00002D53
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004D66 File Offset: 0x00002F66
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.Constants", typeof(IConstantService), this);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004D85 File Offset: 0x00002F85
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
			pipeline.InsertPostStage(PipelineStage.ProcessModule, new EncodePhase(this));
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004A48 File Offset: 0x00002C48
		public ConstantProtection()
		{
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00004DA4 File Offset: 0x00002FA4
		// Note: this type is marked as 'beforefieldinit'.
		static ConstantProtection()
		{
		}

		// Token: 0x0400003E RID: 62
		public const string _Id = "constants";

		// Token: 0x0400003F RID: 63
		public const string _FullId = "Ki.Constants";

		// Token: 0x04000040 RID: 64
		public const string _ServiceId = "Ki.Constants";

		// Token: 0x04000041 RID: 65
		internal static readonly object ContextKey = new object();
	}
}
