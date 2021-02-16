using System;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x0200003B RID: 59
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.CtrlFlowMod"
	})]
	internal class StackUnfConfusion : Protection
	{
		// Token: 0x0600012A RID: 298 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00004F97 File Offset: 0x00003197
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new StackUnfConfusion.StackUnfConfusionPhase(this));
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00004FA8 File Offset: 0x000031A8
		public override string Description
		{
			get
			{
				return "This confusion will add a piece of code in the front of the methods and cause decompilers to crash.";
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600012D RID: 301 RVA: 0x00004FAF File Offset: 0x000031AF
		public override string FullId
		{
			get
			{
				return "Ki.StackUn";
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00004FB6 File Offset: 0x000031B6
		public override string Id
		{
			get
			{
				return "stack underflow";
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600012F RID: 303 RVA: 0x00004FBD File Offset: 0x000031BD
		public override string Name
		{
			get
			{
				return "Stack Underflow Confusion";
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00004FC4 File Offset: 0x000031C4
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00004A48 File Offset: 0x00002C48
		public StackUnfConfusion()
		{
		}

		// Token: 0x0400006B RID: 107
		public const string _FullId = "Ki.StackUn";

		// Token: 0x0400006C RID: 108
		public const string _Id = "stack underflow";

		// Token: 0x0200003C RID: 60
		private class StackUnfConfusionPhase : ProtectionPhase
		{
			// Token: 0x06000132 RID: 306 RVA: 0x00004A51 File Offset: 0x00002C51
			public StackUnfConfusionPhase(StackUnfConfusion parent) : base(parent)
			{
			}

			// Token: 0x06000133 RID: 307 RVA: 0x0000A904 File Offset: 0x00008B04
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (IDnlibDef dnlibDef in parameters.Targets)
				{
					MethodDef def = (MethodDef)dnlibDef;
					bool flag = def != null && !def.HasBody;
					if (flag)
					{
						break;
					}
					CilBody body = def.Body;
					Instruction target = body.Instructions[0];
					Instruction item = Instruction.Create(OpCodes.Br_S, target);
					Instruction instruction4 = Instruction.Create(OpCodes.Pop);
					Random random = new Random();
					Instruction instruction5;
					switch (random.Next(0, 2))
					{
					case 0:
						instruction5 = Instruction.Create(OpCodes.Ldnull);
						break;
					case 1:
						instruction5 = Instruction.Create(OpCodes.Ldc_I4_0);
						break;
					case 2:
						instruction5 = Instruction.Create(OpCodes.Ldstr, "");
						break;
					default:
						instruction5 = Instruction.Create(OpCodes.Ldc_I8, (long)random.Next());
						break;
					}
					body.Instructions.Insert(0, instruction5);
					body.Instructions.Insert(1, instruction4);
					body.Instructions.Insert(2, item);
					foreach (ExceptionHandler handler in body.ExceptionHandlers)
					{
						bool flag2 = handler.TryStart == target;
						if (flag2)
						{
							handler.TryStart = item;
						}
						else
						{
							bool flag3 = handler.HandlerStart == target;
							if (flag3)
							{
								handler.HandlerStart = item;
							}
							else
							{
								bool flag4 = handler.FilterStart == target;
								if (flag4)
								{
									handler.FilterStart = item;
								}
							}
						}
					}
				}
			}

			// Token: 0x17000087 RID: 135
			// (get) Token: 0x06000134 RID: 308 RVA: 0x00004FBD File Offset: 0x000031BD
			public override string Name
			{
				get
				{
					return "Stack Underflow Confusion";
				}
			}

			// Token: 0x17000088 RID: 136
			// (get) Token: 0x06000135 RID: 309 RVA: 0x00004BE0 File Offset: 0x00002DE0
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
