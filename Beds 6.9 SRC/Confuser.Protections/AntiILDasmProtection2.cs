using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using Microsoft.VisualBasic;

namespace Confuser.Protections
{
	// Token: 0x02000033 RID: 51
	internal class AntiILDasmProtection2 : Protection
	{
		// Token: 0x060000EB RID: 235 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00004E63 File Offset: 0x00003063
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.WriteModule, new AntiILDasmProtection2.AntiILDasmPhase(this));
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060000ED RID: 237 RVA: 0x0000A3D8 File Offset: 0x000085D8
		public override string Description
		{
			get
			{
				return "This protection adds junk into the output.";
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000A3F0 File Offset: 0x000085F0
		public override string FullId
		{
			get
			{
				return "Ki.AntiILDasm2";
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060000EF RID: 239 RVA: 0x0000A408 File Offset: 0x00008608
		public override string Id
		{
			get
			{
				return "Junk";
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x0000A420 File Offset: 0x00008620
		public override string Name
		{
			get
			{
				return "Add Junk";
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x00005F00 File Offset: 0x00004100
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00007BDC File Offset: 0x00005DDC
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

		// Token: 0x060000F3 RID: 243 RVA: 0x00004A48 File Offset: 0x00002C48
		public AntiILDasmProtection2()
		{
		}

		// Token: 0x04000056 RID: 86
		public const string _FullId = "Ki.AntiILDasm2";

		// Token: 0x04000057 RID: 87
		public const string _Id = "Junk";

		// Token: 0x02000034 RID: 52
		private class AntiILDasmPhase : ProtectionPhase
		{
			// Token: 0x060000F4 RID: 244 RVA: 0x00004E74 File Offset: 0x00003074
			public AntiILDasmPhase(AntiILDasmProtection2 parent) : base(parent)
			{
			}

			// Token: 0x060000F5 RID: 245 RVA: 0x0000A438 File Offset: 0x00008638
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					ModuleDefMD moduleDefMD = (ModuleDefMD)moduleDef;
					if (moduleDefMD.FullName.Contains(".exe"))
					{
						moduleDefMD.Name = "get_ConsistencyGuarantee - " + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString();
						moduleDefMD.Assembly.Name = "remove_RowHeadersDefaultCellStyleChanged - " + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString();
						int num = context.Modules.Count - 1;
						for (int i = 0; i <= num; i++)
						{
							int num2 = (int)Math.Round((double)(Conversion.Int(VBMath.Rnd() * 100f) + 100f));
							for (int j = 0; j <= num2; j++)
							{
								new TypeDefUser(AntiILDasmProtection2.Random(15) + "_ExtensionInfo", AntiILDasmProtection2.Random(10) + "CatchBlockProxy", moduleDefMD.CorLibTypes.Object.TypeDefOrRef).Attributes = TypeAttributes.Public;
								TypeDef item = new TypeDefUser(AntiILDasmProtection2.Random(15) + "LabelExpressionProxy", AntiILDasmProtection2.Random(10) + "_<PrivateImplementationDetails><System_Data_netmodule>", moduleDefMD.CorLibTypes.Object.TypeDefOrRef)
								{
									Attributes = TypeAttributes.Public
								};
								TypeDef item2 = new TypeDefUser(AntiILDasmProtection2.Random(15) + "_BIDEXTINFO", AntiILDasmProtection2.Random(10) + "ColumnError", moduleDefMD.CorLibTypes.Object.TypeDefOrRef)
								{
									Attributes = TypeAttributes.Public
								};
								moduleDefMD.Types.Add(item);
								moduleDefMD.Types.Add(item2);
							}
							moduleDefMD.EntryPoint.Name = "GetValueKind_" + AntiILDasmProtection2.Random(3) + "_GetDeclaredEvent";
						}
					}
					else
					{
						context.Logger.Warn("Junk Cannot be used on a dll");
					}
				}
			}

			// Token: 0x17000068 RID: 104
			// (get) Token: 0x060000F6 RID: 246 RVA: 0x0000A714 File Offset: 0x00008914
			public override string Name
			{
				get
				{
					return "Junk generating";
				}
			}

			// Token: 0x17000069 RID: 105
			// (get) Token: 0x060000F7 RID: 247 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x04000058 RID: 88
			private Random cheekycunt = new Random();
		}
	}
}
