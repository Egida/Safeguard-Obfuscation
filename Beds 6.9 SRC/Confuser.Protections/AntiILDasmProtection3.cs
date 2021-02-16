using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using Microsoft.VisualBasic;

namespace Confuser.Protections
{
	// Token: 0x02000017 RID: 23
	internal class AntiILDasmProtection3 : Protection
	{
		// Token: 0x06000066 RID: 102 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004CC7 File Offset: 0x00002EC7
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.WriteModule, new AntiILDasmProtection3.AntiILDasmPhase(this));
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00007B94 File Offset: 0x00005D94
		public override string Description
		{
			get
			{
				return "Renames the module and assembly.";
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00007BAC File Offset: 0x00005DAC
		public override string FullId
		{
			get
			{
				return "Ki.AntiILDasm3";
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00007BC4 File Offset: 0x00005DC4
		public override string Id
		{
			get
			{
				return "Rename Module";
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00007BC4 File Offset: 0x00005DC4
		public override string Name
		{
			get
			{
				return "Rename Module";
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00005F00 File Offset: 0x00004100
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00007BDC File Offset: 0x00005DDC
		public static string Random(int len)
		{
			string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRST123456789他是说汉语的Ⓟⓡⓞⓣⓔⓒⓣｱ尺Ծｲ乇ζｲ123456789αβγδεζηθικλμνξοπρστυφχψω卍卍卍卍卍卍卍";
			char[] chArray = new char[len - 1 + 1];
			int num = len - 1;
			for (int i = 0; i <= num; i++)
			{
				chArray[i] = str[(int)Math.Round((double)Conversion.Int(VBMath.Rnd() * (float)str.Length))];
			}
			return new string(chArray);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004A48 File Offset: 0x00002C48
		public AntiILDasmProtection3()
		{
		}

		// Token: 0x0400002E RID: 46
		public const string _FullId = "Ki.AntiILDasm3";

		// Token: 0x0400002F RID: 47
		public const string _Id = "Junk";

		// Token: 0x02000018 RID: 24
		private class AntiILDasmPhase : ProtectionPhase
		{
			// Token: 0x0600006F RID: 111 RVA: 0x00004CD8 File Offset: 0x00002ED8
			public AntiILDasmPhase(AntiILDasmProtection3 parent) : base(parent)
			{
			}

			// Token: 0x06000070 RID: 112 RVA: 0x00007C48 File Offset: 0x00005E48
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					ModuleDefMD moduleDefMD = (ModuleDefMD)moduleDef;
					moduleDefMD.Name = "SafeGuard - " + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString();
					moduleDefMD.Assembly.Name = "SafeGuard - " + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString();
				}
			}

			// Token: 0x17000028 RID: 40
			// (get) Token: 0x06000071 RID: 113 RVA: 0x00007D84 File Offset: 0x00005F84
			public override string Name
			{
				get
				{
					return "Renaming";
				}
			}

			// Token: 0x17000029 RID: 41
			// (get) Token: 0x06000072 RID: 114 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x04000030 RID: 48
			private Random cheekycunt = new Random();
		}
	}
}
