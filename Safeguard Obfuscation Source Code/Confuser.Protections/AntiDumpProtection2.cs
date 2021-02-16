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
	// Token: 0x02000019 RID: 25
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.AntiTamper"
	})]
	internal class AntiDumpProtection2 : Protection
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004CEE File Offset: 0x00002EEE
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDumpProtection2.AntiDumpPhase(this));
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00007D9C File Offset: 0x00005F9C
		public override string Description
		{
			get
			{
				return "This protection flood the module.cctor.";
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00007DB4 File Offset: 0x00005FB4
		public override string FullId
		{
			get
			{
				return "Ki.AntiDump2";
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00007DCC File Offset: 0x00005FCC
		public override string Id
		{
			get
			{
				return "module flood";
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00007DE4 File Offset: 0x00005FE4
		public override string Name
		{
			get
			{
				return "Module Flood Protection";
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000079 RID: 121 RVA: 0x000073FC File Offset: 0x000055FC
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004A48 File Offset: 0x00002C48
		public AntiDumpProtection2()
		{
		}

		// Token: 0x04000031 RID: 49
		public const string _FullId = "Ki.AntiDump2";

		// Token: 0x04000032 RID: 50
		public const string _Id = "module flood";

		// Token: 0x0200001A RID: 26
		private class AntiDumpPhase : ProtectionPhase
		{
			// Token: 0x0600007B RID: 123 RVA: 0x00004A51 File Offset: 0x00002C51
			public AntiDumpPhase(AntiDumpProtection2 parent) : base(parent)
			{
			}

			// Token: 0x0600007C RID: 124 RVA: 0x00007DFC File Offset: 0x00005FFC
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef rtType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.AntiDump2");
				IMarkerService marker = context.Registry.GetService<IMarkerService>();
				INameService name = context.Registry.GetService<INameService>();
				foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>())
				{
					IEnumerable<IDnlibDef> members = InjectHelper.Inject(rtType, module.GlobalType, module);
					MethodDef cctor = module.GlobalType.FindStaticConstructor();
					MethodDef init = (MethodDef)members.Single((IDnlibDef method) => method.Name == "Initialize");
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					foreach (IDnlibDef member in members)
					{
						name.MarkHelper(member, marker, (Protection)base.Parent);
					}
				}
			}

			// Token: 0x1700002F RID: 47
			// (get) Token: 0x0600007D RID: 125 RVA: 0x00009104 File Offset: 0x00007304
			public override string Name
			{
				get
				{
					return "Module Flooding";
				}
			}

			// Token: 0x17000030 RID: 48
			// (get) Token: 0x0600007E RID: 126 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x0200001B RID: 27
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x0600007F RID: 127 RVA: 0x00004CFF File Offset: 0x00002EFF
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x06000080 RID: 128 RVA: 0x00004A68 File Offset: 0x00002C68
				public <>c()
				{
				}

				// Token: 0x06000081 RID: 129 RVA: 0x00004A71 File Offset: 0x00002C71
				internal bool <Execute>b__1_0(IDnlibDef method)
				{
					return method.Name == "Initialize";
				}

				// Token: 0x04000033 RID: 51
				public static readonly AntiDumpProtection2.AntiDumpPhase.<>c <>9 = new AntiDumpProtection2.AntiDumpPhase.<>c();

				// Token: 0x04000034 RID: 52
				public static Func<IDnlibDef, bool> <>9__1_0;
			}
		}
	}
}
