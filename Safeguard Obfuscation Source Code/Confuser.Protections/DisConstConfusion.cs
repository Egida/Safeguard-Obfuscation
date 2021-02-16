using System;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x0200002C RID: 44
	[AfterProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.CtrlFlowMod"
	})]
	internal class DisConstConfusion : Protection
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00004DEA File Offset: 0x00002FEA
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new DisConstConfusion.DisConstConfusionPhase(this));
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00004DFB File Offset: 0x00002FFB
		public override string Description
		{
			get
			{
				return "This protection disintegrate the constants in the code into expressions.";
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00004E02 File Offset: 0x00003002
		public override string FullId
		{
			get
			{
				return "Ki.disconstant";
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00004E09 File Offset: 0x00003009
		public override string Id
		{
			get
			{
				return "Const disint";
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00004E10 File Offset: 0x00003010
		public override string Name
		{
			get
			{
				return "Disinigrate Constatnts";
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00004BE0 File Offset: 0x00002DE0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00004A48 File Offset: 0x00002C48
		public DisConstConfusion()
		{
		}

		// Token: 0x0400004E RID: 78
		public const string _FullId = "Ki.disconstant";

		// Token: 0x0400004F RID: 79
		public const string _Id = "Const disint";

		// Token: 0x0200002D RID: 45
		private class DisConstConfusionPhase : ProtectionPhase
		{
			// Token: 0x060000D2 RID: 210 RVA: 0x00004A51 File Offset: 0x00002C51
			public DisConstConfusionPhase(DisConstConfusion parent) : base(parent)
			{
			}

			// Token: 0x060000D3 RID: 211 RVA: 0x000099B0 File Offset: 0x00007BB0
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				for (int i = 1; i < 1; i++)
				{
					foreach (IDnlibDef dnlibDef in parameters.Targets)
					{
						MethodDef def = (MethodDef)dnlibDef;
						CilBody body = def.Body;
						body.SimplifyBranches();
						Random random = new Random();
						int num2 = 0;
						while (num2 < body.Instructions.Count)
						{
							bool flag2 = body.Instructions[num2].IsLdcI4();
							if (flag2)
							{
								int num3 = body.Instructions[num2].GetLdcI4Value();
								int num4 = random.Next(5, 40);
								body.Instructions[num2].OpCode = OpCodes.Ldc_I4;
								body.Instructions[num2].Operand = num4 * num3;
								body.Instructions.Insert(num2 + 1, Instruction.Create(OpCodes.Ldc_I4, num4));
								body.Instructions.Insert(num2 + 2, Instruction.Create(OpCodes.Div));
								num2 += 3;
							}
							else
							{
								num2++;
							}
						}
						Random random2 = new Random();
						int num5 = 0;
						ITypeDefOrRef type = null;
						for (int j = 0; j < def.Body.Instructions.Count; j++)
						{
							Instruction instruction = def.Body.Instructions[j];
							bool flag3 = !instruction.IsLdcI4();
							if (!flag3)
							{
								switch (random2.Next(1, 8))
								{
								case 1:
									type = def.Module.Import(typeof(int));
									num5 = 4;
									break;
								case 2:
									type = def.Module.Import(typeof(sbyte));
									num5 = 1;
									break;
								case 3:
									type = def.Module.Import(typeof(byte));
									num5 = 1;
									break;
								case 4:
									type = def.Module.Import(typeof(bool));
									num5 = 1;
									break;
								case 5:
									type = def.Module.Import(typeof(decimal));
									num5 = 16;
									break;
								case 6:
									type = def.Module.Import(typeof(short));
									num5 = 2;
									break;
								case 7:
									type = def.Module.Import(typeof(long));
									num5 = 8;
									break;
								}
								int num6 = random2.Next(1, 1000);
								bool flag = Convert.ToBoolean(random2.Next(0, 2));
								switch ((num5 != 0) ? ((Convert.ToInt32(instruction.Operand) % num5 == 0) ? random2.Next(1, 5) : random2.Next(1, 4)) : random2.Next(1, 4))
								{
								case 1:
									def.Body.Instructions.Insert(j + 1, Instruction.Create(OpCodes.Sizeof, type));
									def.Body.Instructions.Insert(j + 2, Instruction.Create(OpCodes.Add));
									instruction.Operand = Convert.ToInt32(instruction.Operand) - num5 + (flag ? (-num6) : num6);
									goto IL_466;
								case 2:
									def.Body.Instructions.Insert(j + 1, Instruction.Create(OpCodes.Sizeof, type));
									def.Body.Instructions.Insert(j + 2, Instruction.Create(OpCodes.Sub));
									instruction.Operand = Convert.ToInt32(instruction.Operand) + num5 + (flag ? (-num6) : num6);
									goto IL_466;
								case 3:
									def.Body.Instructions.Insert(j + 1, Instruction.Create(OpCodes.Sizeof, type));
									def.Body.Instructions.Insert(j + 2, Instruction.Create(OpCodes.Add));
									instruction.Operand = Convert.ToInt32(instruction.Operand) - num5 + (flag ? (-num6) : num6);
									goto IL_466;
								case 4:
									def.Body.Instructions.Insert(j + 1, Instruction.Create(OpCodes.Sizeof, type));
									def.Body.Instructions.Insert(j + 2, Instruction.Create(OpCodes.Mul));
									instruction.Operand = Convert.ToInt32(instruction.Operand) / num5;
									break;
								default:
									goto IL_466;
								}
								IL_4B2:
								j += 2;
								goto IL_4BA;
								IL_466:
								def.Body.Instructions.Insert(j + 3, Instruction.CreateLdcI4(num6));
								def.Body.Instructions.Insert(j + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
								j += 2;
								goto IL_4B2;
							}
							IL_4BA:;
						}
						body.OptimizeBranches();
					}
				}
			}

			// Token: 0x17000059 RID: 89
			// (get) Token: 0x060000D4 RID: 212 RVA: 0x00004E17 File Offset: 0x00003017
			public override string Name
			{
				get
				{
					return "Constant Disintegration";
				}
			}

			// Token: 0x1700005A RID: 90
			// (get) Token: 0x060000D5 RID: 213 RVA: 0x00004BE0 File Offset: 0x00002DE0
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Methods;
				}
			}
		}
	}
}
