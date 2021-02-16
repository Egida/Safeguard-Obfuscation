using System;
using Confuser.Core;
using Confuser.Protections.ReferenceProxy;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000038 RID: 56
	[AfterProtection(new string[]
	{
		"Ki.AntiDebug",
		"Ki.AntiDump"
	})]
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class ReferenceProxyProtection : Protection, IReferenceProxyService
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600010E RID: 270 RVA: 0x0000A7BC File Offset: 0x000089BC
		public override string Name
		{
			get
			{
				return "Reference Proxy Protection";
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600010F RID: 271 RVA: 0x0000A7D4 File Offset: 0x000089D4
		public override string Description
		{
			get
			{
				return "This protection encodes and hides references to type/method/fields.";
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000110 RID: 272 RVA: 0x0000A7EC File Offset: 0x000089EC
		public override string Id
		{
			get
			{
				return "ref proxy";
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000111 RID: 273 RVA: 0x0000A804 File Offset: 0x00008A04
		public override string FullId
		{
			get
			{
				return "Ki.RefProxy";
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00007388 File Offset: 0x00005588
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00004B53 File Offset: 0x00002D53
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00004F15 File Offset: 0x00003115
		public void ExcludeTarget(ConfuserContext context, MethodDef method)
		{
			context.Annotations.Set<object>(method, ReferenceProxyProtection.TargetExcluded, ReferenceProxyProtection.TargetExcluded);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000A81C File Offset: 0x00008A1C
		public bool IsTargeted(ConfuserContext context, MethodDef method)
		{
			return context.Annotations.Get<object>(method, ReferenceProxyProtection.Targeted, null) != null;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00004F2F File Offset: 0x0000312F
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.RefProxy", typeof(IReferenceProxyService), this);
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00004F4E File Offset: 0x0000314E
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new ReferenceProxyPhase(this));
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00004A48 File Offset: 0x00002C48
		public ReferenceProxyProtection()
		{
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00004F5F File Offset: 0x0000315F
		// Note: this type is marked as 'beforefieldinit'.
		static ReferenceProxyProtection()
		{
		}

		// Token: 0x04000060 RID: 96
		public const string _Id = "ref proxy";

		// Token: 0x04000061 RID: 97
		public const string _FullId = "Ki.RefProxy";

		// Token: 0x04000062 RID: 98
		public const string _ServiceId = "Ki.RefProxy";

		// Token: 0x04000063 RID: 99
		internal static object TargetExcluded = new object();

		// Token: 0x04000064 RID: 100
		internal static object Targeted = new object();
	}
}
