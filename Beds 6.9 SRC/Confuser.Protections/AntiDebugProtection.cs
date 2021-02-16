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
	// Token: 0x02000028 RID: 40
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class AntiDebugProtection : Protection
	{
		// Token: 0x060000BB RID: 187 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00004DCD File Offset: 0x00002FCD
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDebugProtection.AntiDebugPhase(this));
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000BD RID: 189 RVA: 0x000095C8 File Offset: 0x000077C8
		public override string Description
		{
			get
			{
				return "This protection prevents the assembly from being debugged or profiled.";
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000BE RID: 190 RVA: 0x000095E0 File Offset: 0x000077E0
		public override string FullId
		{
			get
			{
				return "Ki.AntiDebug";
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000BF RID: 191 RVA: 0x000095F8 File Offset: 0x000077F8
		public override string Id
		{
			get
			{
				return "anti debug";
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00009610 File Offset: 0x00007810
		public override string Name
		{
			get
			{
				return "Anti Debug Protection";
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00005F00 File Offset: 0x00004100
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00004A48 File Offset: 0x00002C48
		public AntiDebugProtection()
		{
		}

		// Token: 0x04000046 RID: 70
		public const string _FullId = "Ki.AntiDebug";

		// Token: 0x04000047 RID: 71
		public const string _Id = "anti debug";

		// Token: 0x02000029 RID: 41
		private class AntiDebugPhase : ProtectionPhase
		{
			// Token: 0x060000C3 RID: 195 RVA: 0x00004A51 File Offset: 0x00002C51
			public AntiDebugPhase(AntiDebugProtection parent) : base(parent)
			{
			}

			// Token: 0x060000C4 RID: 196 RVA: 0x00009628 File Offset: 0x00007828
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				IRuntimeService rt = context.Registry.GetService<IRuntimeService>();
				IMarkerService marker = context.Registry.GetService<IMarkerService>();
				INameService name = context.Registry.GetService<INameService>();
				foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>())
				{
					AntiDebugProtection.AntiDebugPhase.AntiMode mode = parameters.GetParameter<AntiDebugProtection.AntiDebugPhase.AntiMode>(context, module, "mode", AntiDebugProtection.AntiDebugPhase.AntiMode.Safe);
					TypeDef attr = null;
					TypeDef rtType;
					switch (mode)
					{
					case AntiDebugProtection.AntiDebugPhase.AntiMode.Safe:
						rtType = rt.GetRuntimeType("Confuser.Runtime.AntiDebugSafe");
						break;
					case AntiDebugProtection.AntiDebugPhase.AntiMode.Win32:
						rtType = rt.GetRuntimeType("Confuser.Runtime.AntiDebugWin32");
						break;
					case AntiDebugProtection.AntiDebugPhase.AntiMode.Antinet:
						rtType = rt.GetRuntimeType("Confuser.Runtime.AntiDebugAntinet");
						attr = rt.GetRuntimeType("System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute");
						module.Types.Add(attr = InjectHelper.Inject(attr, module));
						foreach (IDnlibDef member in attr.FindDefinitions())
						{
							marker.Mark(member, (Protection)base.Parent);
							name.Analyze(member);
						}
						name.SetCanRename(attr, false);
						break;
					default:
						throw new UnreachableException();
					}
					IEnumerable<IDnlibDef> members = InjectHelper.Inject(rtType, module.GlobalType, module);
					MethodDef cctor = module.GlobalType.FindStaticConstructor();
					MethodDef init = (MethodDef)members.Single((IDnlibDef method) => method.Name == "Initialize");
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
					foreach (IDnlibDef member2 in members)
					{
						marker.Mark(member2, (Protection)base.Parent);
						name.Analyze(member2);
						bool ren = true;
						bool flag = member2 is MethodDef;
						if (flag)
						{
							MethodDef method2 = (MethodDef)member2;
							bool flag2 = method2.Access == MethodAttributes.Public;
							if (flag2)
							{
								method2.Access = MethodAttributes.Assembly;
							}
							bool flag3 = !method2.IsConstructor;
							if (flag3)
							{
								method2.IsSpecialName = false;
							}
							else
							{
								ren = false;
							}
							CustomAttribute ca = method2.CustomAttributes.Find("System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute");
							bool flag4 = ca != null;
							if (flag4)
							{
								ca.Constructor = attr.FindMethod(".ctor");
							}
						}
						else
						{
							bool flag5 = member2 is FieldDef;
							if (flag5)
							{
								FieldDef field = (FieldDef)member2;
								bool flag6 = field.Access == FieldAttributes.Public;
								if (flag6)
								{
									field.Access = FieldAttributes.Assembly;
								}
								bool isLiteral = field.IsLiteral;
								if (isLiteral)
								{
									field.DeclaringType.Fields.Remove(field);
									continue;
								}
							}
						}
						bool flag7 = ren;
						if (flag7)
						{
							member2.Name = name.ObfuscateName(member2.Name, RenameMode.Unicode);
							name.SetCanRename(member2, false);
						}
					}
				}
			}

			// Token: 0x17000052 RID: 82
			// (get) Token: 0x060000C5 RID: 197 RVA: 0x0000629C File Offset: 0x0000449C
			public override string Name
			{
				get
				{
					return "Anti-debug injection";
				}
			}

			// Token: 0x17000053 RID: 83
			// (get) Token: 0x060000C6 RID: 198 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x0200002A RID: 42
			private enum AntiMode
			{
				// Token: 0x04000049 RID: 73
				Safe,
				// Token: 0x0400004A RID: 74
				Win32,
				// Token: 0x0400004B RID: 75
				Antinet
			}

			// Token: 0x0200002B RID: 43
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x060000C7 RID: 199 RVA: 0x00004DDE File Offset: 0x00002FDE
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x060000C8 RID: 200 RVA: 0x00004A68 File Offset: 0x00002C68
				public <>c()
				{
				}

				// Token: 0x060000C9 RID: 201 RVA: 0x00004A71 File Offset: 0x00002C71
				internal bool <Execute>b__1_0(IDnlibDef method)
				{
					return method.Name == "Initialize";
				}

				// Token: 0x0400004C RID: 76
				public static readonly AntiDebugProtection.AntiDebugPhase.<>c <>9 = new AntiDebugProtection.AntiDebugPhase.<>c();

				// Token: 0x0400004D RID: 77
				public static Func<IDnlibDef, bool> <>9__1_0;
			}
		}
	}
}
