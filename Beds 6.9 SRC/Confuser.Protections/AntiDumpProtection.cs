using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x02000025 RID: 37
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class AntiDumpProtection : Protection
	{
		// Token: 0x060000AC RID: 172 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004DB0 File Offset: 0x00002FB0
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDumpProtection.AntiDumpPhase(this));
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00009410 File Offset: 0x00007610
		public override string Description
		{
			get
			{
				return "This protection prevents the assembly from being dumped from memory.";
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00009428 File Offset: 0x00007628
		public override string FullId
		{
			get
			{
				return "Ki.AntiDump";
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00009440 File Offset: 0x00007640
		public override string Id
		{
			get
			{
				return "anti dump";
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00009458 File Offset: 0x00007658
		public override string Name
		{
			get
			{
				return "Anti Dump Protection";
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x000073FC File Offset: 0x000055FC
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004A48 File Offset: 0x00002C48
		public AntiDumpProtection()
		{
		}

		// Token: 0x04000042 RID: 66
		public const string _FullId = "Ki.AntiDump";

		// Token: 0x04000043 RID: 67
		public const string _Id = "anti dump";

		// Token: 0x02000026 RID: 38
		private class AntiDumpPhase : ProtectionPhase
		{
			// Token: 0x060000B4 RID: 180 RVA: 0x00004A51 File Offset: 0x00002C51
			public AntiDumpPhase(AntiDumpProtection parent) : base(parent)
			{
			}

			// Token: 0x060000B5 RID: 181 RVA: 0x00009470 File Offset: 0x00007670
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef rtType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.AntiDump");
				IMarkerService marker = context.Registry.GetService<IMarkerService>();
				INameService name = context.Registry.GetService<INameService>();
				foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>())
				{
					IEnumerable<IDnlibDef> members = InjectHelper.Inject(rtType, module.GlobalType, module);
					MethodDef cctor = module.GlobalType.FindStaticConstructor();
					MethodDef init = (MethodDef)members.Single((IDnlibDef method) => method.Name == "Initialize");
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					foreach (IDnlibDef member in members)
					{
						name.MarkHelper(member, marker, (Protection)base.Parent);
					}
				}
			}

			// Token: 0x1700004B RID: 75
			// (get) Token: 0x060000B6 RID: 182 RVA: 0x000095B0 File Offset: 0x000077B0
			public override string Name
			{
				get
				{
					return "Anti-dump injection";
				}
			}

			// Token: 0x1700004C RID: 76
			// (get) Token: 0x060000B7 RID: 183 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x02000027 RID: 39
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x060000B8 RID: 184 RVA: 0x00004DC1 File Offset: 0x00002FC1
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x060000B9 RID: 185 RVA: 0x00004A68 File Offset: 0x00002C68
				public <>c()
				{
				}

				// Token: 0x060000BA RID: 186 RVA: 0x00004A71 File Offset: 0x00002C71
				internal bool <Execute>b__1_0(IDnlibDef method)
				{
					return method.Name == "Initialize";
				}

				// Token: 0x04000044 RID: 68
				public static readonly AntiDumpProtection.AntiDumpPhase.<>c <>9 = new AntiDumpProtection.AntiDumpPhase.<>c();

				// Token: 0x04000045 RID: 69
				public static Func<IDnlibDef, bool> <>9__1_0;
			}
		}
	}
}
