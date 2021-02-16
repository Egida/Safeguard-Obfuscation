using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200001C RID: 28
	internal class AntiILDasmProtection : Protection
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00004D0B File Offset: 0x00002F0B
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiILDasmProtection.AntiILDasmPhase(this));
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000084 RID: 132 RVA: 0x0000911C File Offset: 0x0000731C
		public override string Description
		{
			get
			{
				return "This protection marks the module with a attribute that discourage ILDasm from disassembling it.";
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00009134 File Offset: 0x00007334
		public override string FullId
		{
			get
			{
				return "Ki.AntiILDasm";
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000086 RID: 134 RVA: 0x0000914C File Offset: 0x0000734C
		public override string Id
		{
			get
			{
				return "anti ildasm";
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00009164 File Offset: 0x00007364
		public override string Name
		{
			get
			{
				return "Anti IL Dasm Protection";
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00005F00 File Offset: 0x00004100
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004A48 File Offset: 0x00002C48
		public AntiILDasmProtection()
		{
		}

		// Token: 0x04000035 RID: 53
		public const string _FullId = "Ki.AntiILDasm";

		// Token: 0x04000036 RID: 54
		public const string _Id = "anti ildasm";

		// Token: 0x0200001D RID: 29
		private class AntiILDasmPhase : ProtectionPhase
		{
			// Token: 0x0600008A RID: 138 RVA: 0x00004A51 File Offset: 0x00002C51
			public AntiILDasmPhase(AntiILDasmProtection parent) : base(parent)
			{
			}

			// Token: 0x0600008B RID: 139 RVA: 0x0000917C File Offset: 0x0000737C
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>())
				{
					TypeRef attrRef = module.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute");
					MemberRefUser ctorRef = new MemberRefUser(module, ".ctor", MethodSig.CreateInstance(module.CorLibTypes.Void), attrRef);
					CustomAttribute attr = new CustomAttribute(ctorRef);
					module.CustomAttributes.Add(attr);
				}
			}

			// Token: 0x17000036 RID: 54
			// (get) Token: 0x0600008C RID: 140 RVA: 0x0000921C File Offset: 0x0000741C
			public override string Name
			{
				get
				{
					return "Anti-ILDasm marking";
				}
			}

			// Token: 0x17000037 RID: 55
			// (get) Token: 0x0600008D RID: 141 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}
		}
	}
}
