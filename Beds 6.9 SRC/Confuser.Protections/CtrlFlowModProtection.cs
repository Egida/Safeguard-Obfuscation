using System;
using Confuser.Core;
using Confuser.Protections.CtrlFlowMod;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200000F RID: 15
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class CtrlFlowModProtection : Protection, ICtrlFlowModService
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00004B53 File Offset: 0x00002D53
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004BE3 File Offset: 0x00002DE3
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.CtrlFlowMod", typeof(ICtrlFlowModService), this);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00004C02 File Offset: 0x00002E02
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new ControlFlowPhase(this));
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00004BC4 File Offset: 0x00002DC4
		public override string Description
		{
			get
			{
				return "Dynamic switch-base states variable encoding.";
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00004C13 File Offset: 0x00002E13
		public override string FullId
		{
			get
			{
				return "Ki.CtrlFlowMod";
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00004C1A File Offset: 0x00002E1A
		public override string Id
		{
			get
			{
				return "Spec CtrlFlow";
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00004C21 File Offset: 0x00002E21
		public override string Name
		{
			get
			{
				return "Special Control Flow";
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00004BE0 File Offset: 0x00002DE0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004A48 File Offset: 0x00002C48
		public CtrlFlowModProtection()
		{
		}

		// Token: 0x04000020 RID: 32
		public const string _FullId = "Ki.CtrlFlowMod";

		// Token: 0x04000021 RID: 33
		public const string _Id = "Switch Cflow";

		// Token: 0x04000022 RID: 34
		public const string _ServiceId = "Ki.CtrlFlowMod";
	}
}
