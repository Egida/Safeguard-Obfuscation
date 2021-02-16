using System;
using System.Linq;
using Confuser.Core;
using Confuser.Protections.AntiTamper;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200001F RID: 31
	[AfterProtection(new string[]
	{
		"Ki.Constants"
	})]
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class AntiTamperProtection : Protection, IAntiTamperService
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00009234 File Offset: 0x00007434
		public override string Name
		{
			get
			{
				return "Anti Tamper Protection";
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000090 RID: 144 RVA: 0x0000924C File Offset: 0x0000744C
		public override string Description
		{
			get
			{
				return "This protection ensures the integrity of application.";
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00009264 File Offset: 0x00007464
		public override string Id
		{
			get
			{
				return "anti tamper";
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000092 RID: 146 RVA: 0x0000927C File Offset: 0x0000747C
		public override string FullId
		{
			get
			{
				return "Ki.AntiTamper";
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000073FC File Offset: 0x000055FC
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004D1C File Offset: 0x00002F1C
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.AntiTamper", typeof(IAntiTamperService), this);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004D3B File Offset: 0x00002F3B
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new AntiTamperProtection.InjectPhase(this));
			pipeline.InsertPreStage(PipelineStage.EndModule, new AntiTamperProtection.MDPhase(this));
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004B53 File Offset: 0x00002D53
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004A48 File Offset: 0x00002C48
		public AntiTamperProtection()
		{
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004D5A File Offset: 0x00002F5A
		// Note: this type is marked as 'beforefieldinit'.
		static AntiTamperProtection()
		{
		}

		// Token: 0x04000037 RID: 55
		public const string _Id = "anti tamper";

		// Token: 0x04000038 RID: 56
		public const string _FullId = "Ki.AntiTamper";

		// Token: 0x04000039 RID: 57
		public const string _ServiceId = "Ki.AntiTamper";

		// Token: 0x0400003A RID: 58
		private static readonly object HandlerKey = new object();

		// Token: 0x02000020 RID: 32
		private class InjectPhase : ProtectionPhase
		{
			// Token: 0x06000099 RID: 153 RVA: 0x00004A51 File Offset: 0x00002C51
			public InjectPhase(AntiTamperProtection parent) : base(parent)
			{
			}

			// Token: 0x1700003D RID: 61
			// (get) Token: 0x0600009A RID: 154 RVA: 0x00009294 File Offset: 0x00007494
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Methods;
				}
			}

			// Token: 0x1700003E RID: 62
			// (get) Token: 0x0600009B RID: 155 RVA: 0x000092A8 File Offset: 0x000074A8
			public override string Name
			{
				get
				{
					return "Anti-tamper helpers injection";
				}
			}

			// Token: 0x0600009C RID: 156 RVA: 0x000092C0 File Offset: 0x000074C0
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				bool flag = !parameters.Targets.Any<IDnlibDef>();
				if (!flag)
				{
					AntiTamperProtection.Mode mode = parameters.GetParameter<AntiTamperProtection.Mode>(context, context.CurrentModule, "mode", AntiTamperProtection.Mode.Normal);
					AntiTamperProtection.Mode mode2 = mode;
					IModeHandler modeHandler;
					if (mode2 != AntiTamperProtection.Mode.Normal)
					{
						if (mode2 != AntiTamperProtection.Mode.JIT)
						{
							throw new UnreachableException();
						}
						modeHandler = new JITMode();
					}
					else
					{
						modeHandler = new NormalMode();
					}
					modeHandler.HandleInject((AntiTamperProtection)base.Parent, context, parameters);
					context.Annotations.Set<IModeHandler>(context.CurrentModule, AntiTamperProtection.HandlerKey, modeHandler);
				}
			}
		}

		// Token: 0x02000021 RID: 33
		private class MDPhase : ProtectionPhase
		{
			// Token: 0x0600009D RID: 157 RVA: 0x00004A51 File Offset: 0x00002C51
			public MDPhase(AntiTamperProtection parent) : base(parent)
			{
			}

			// Token: 0x1700003F RID: 63
			// (get) Token: 0x0600009E RID: 158 RVA: 0x00009294 File Offset: 0x00007494
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Methods;
				}
			}

			// Token: 0x17000040 RID: 64
			// (get) Token: 0x0600009F RID: 159 RVA: 0x00009348 File Offset: 0x00007548
			public override string Name
			{
				get
				{
					return "Anti-tamper metadata preparation";
				}
			}

			// Token: 0x060000A0 RID: 160 RVA: 0x00009360 File Offset: 0x00007560
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				bool flag = !parameters.Targets.Any<IDnlibDef>();
				if (!flag)
				{
					IModeHandler modeHandler = context.Annotations.Get<IModeHandler>(context.CurrentModule, AntiTamperProtection.HandlerKey, null);
					modeHandler.HandleMD((AntiTamperProtection)base.Parent, context, parameters);
				}
			}
		}

		// Token: 0x02000022 RID: 34
		private enum Mode
		{
			// Token: 0x0400003C RID: 60
			Normal,
			// Token: 0x0400003D RID: 61
			JIT
		}
	}
}
