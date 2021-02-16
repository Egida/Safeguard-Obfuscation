using System;
using Confuser.Core;
using Confuser.Protections.MildReferenceProxy;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000035 RID: 53
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	[AfterProtection(new string[]
	{
		"Ki.AntiDebug",
		"Ki.AntiDump"
	})]
	internal class MildReferenceProxyProtection : Protection, IMildReferenceProxyService
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x00004B53 File Offset: 0x00002D53
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00004E8A File Offset: 0x0000308A
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.MildRefProxy", typeof(IMildReferenceProxyService), this);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00004EA9 File Offset: 0x000030A9
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new MildReferenceProxyPhase(this));
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00004EBA File Offset: 0x000030BA
		public override string Description
		{
			get
			{
				return "Encodes and hides references to type/method/fields with indirection method as proxy.";
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00004EC1 File Offset: 0x000030C1
		public override string FullId
		{
			get
			{
				return "Ki.MildRefProxy";
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00004EC8 File Offset: 0x000030C8
		public override string Id
		{
			get
			{
				return "Clean ref proxy";
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00004ECF File Offset: 0x000030CF
		public override string Name
		{
			get
			{
				return "Clelaner Reference Proxy Protection";
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060000FF RID: 255 RVA: 0x00004BE0 File Offset: 0x00002DE0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00004A48 File Offset: 0x00002C48
		public MildReferenceProxyProtection()
		{
		}

		// Token: 0x04000059 RID: 89
		public const string _FullId = "Ki.MildRefProxy";

		// Token: 0x0400005A RID: 90
		public const string _Id = "Mid ref proxy";

		// Token: 0x0400005B RID: 91
		public const string _ServiceId = "Ki.MildRefProxy";
	}
}
