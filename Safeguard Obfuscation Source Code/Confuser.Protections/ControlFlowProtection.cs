using System;
using Confuser.Core;
using Confuser.Protections.ControlFlow;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200000D RID: 13
	internal class ControlFlowProtection : Protection, IControlFlowService
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00007328 File Offset: 0x00005528
		public override string Name
		{
			get
			{
				return "Control Flow (AT) Protection";
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00007340 File Offset: 0x00005540
		public override string Description
		{
			get
			{
				return "This protection mangles the code in the methods so that decompilers cannot decompile the methods.";
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00007358 File Offset: 0x00005558
		public override string Id
		{
			get
			{
				return "ctrl flow";
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00007370 File Offset: 0x00005570
		public override string FullId
		{
			get
			{
				return "Ki.ControlFlow";
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00007388 File Offset: 0x00005588
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004B53 File Offset: 0x00002D53
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004B64 File Offset: 0x00002D64
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.ControlFlow", typeof(IControlFlowService), this);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00004B83 File Offset: 0x00002D83
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new ControlFlowPhase(this));
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004A48 File Offset: 0x00002C48
		public ControlFlowProtection()
		{
		}

		// Token: 0x0400001A RID: 26
		public const string _Id = "ctrl flow";

		// Token: 0x0400001B RID: 27
		public const string _FullId = "Ki.ControlFlow";

		// Token: 0x0400001C RID: 28
		public const string _ServiceId = "Ki.ControlFlow";
	}
}
