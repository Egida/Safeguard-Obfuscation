using System;
using Confuser.Core;
using Confuser.Protections.DupCtrlFlowMod;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200000E RID: 14
	[AfterProtection(new string[]
	{
		"Ki.AntiTamper"
	})]
	internal class DupCtrlFlowModProtection : Protection, DupCtrlFlowModService
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00004B53 File Offset: 0x00002D53
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00004B94 File Offset: 0x00002D94
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.DupCtrlFlowMod", typeof(DupCtrlFlowModService), this);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00004BB3 File Offset: 0x00002DB3
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new ControlFlowPhase(this));
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00004BC4 File Offset: 0x00002DC4
		public override string Description
		{
			get
			{
				return "Dynamic switch-base states variable encoding.";
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00004BCB File Offset: 0x00002DCB
		public override string FullId
		{
			get
			{
				return "Ki.DupCtrlFlowMod";
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00004BD2 File Offset: 0x00002DD2
		public override string Id
		{
			get
			{
				return "Dup CtrlFlow";
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00004BD9 File Offset: 0x00002DD9
		public override string Name
		{
			get
			{
				return "Duplicate Control Flow";
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00004BE0 File Offset: 0x00002DE0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004A48 File Offset: 0x00002C48
		public DupCtrlFlowModProtection()
		{
		}

		// Token: 0x0400001D RID: 29
		public const string _FullId = "Ki.DupCtrlFlowMod";

		// Token: 0x0400001E RID: 30
		public const string _Id = "Duplocate Cflow";

		// Token: 0x0400001F RID: 31
		public const string _ServiceId = "Ki.DupCtrlFlowMod";
	}
}
