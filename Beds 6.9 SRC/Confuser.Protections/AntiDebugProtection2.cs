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
	// Token: 0x02000002 RID: 2
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class AntiDebugProtection2 : Protection
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00004A37 File Offset: 0x00002C37
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDebugProtection2.AntiDebugPhase2(this));
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00005EB8 File Offset: 0x000040B8
		public override string Description
		{
			get
			{
				return "This protection prevents DNSpy and many other programs from debugging..";
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00005ED0 File Offset: 0x000040D0
		public override string FullId
		{
			get
			{
				return "Ki.AntiDebug2";
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00005EE8 File Offset: 0x000040E8
		public override string Id
		{
			get
			{
				return "Anti Debug Antinet";
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00005EE8 File Offset: 0x000040E8
		public override string Name
		{
			get
			{
				return "Anti Debug Antinet";
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00005F00 File Offset: 0x00004100
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00004A48 File Offset: 0x00002C48
		public AntiDebugProtection2()
		{
		}

		// Token: 0x04000001 RID: 1
		public const string _FullId = "Ki.AntiDebug2";

		// Token: 0x04000002 RID: 2
		public const string _Id = "Anti Debug Antinet";

		// Token: 0x02000003 RID: 3
		private class AntiDebugPhase2 : ProtectionPhase
		{
			// Token: 0x06000009 RID: 9 RVA: 0x00004A51 File Offset: 0x00002C51
			public AntiDebugPhase2(AntiDebugProtection2 parent) : base(parent)
			{
			}

			// Token: 0x0600000A RID: 10 RVA: 0x00005F14 File Offset: 0x00004114
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				IRuntimeService rt = context.Registry.GetService<IRuntimeService>();
				IMarkerService marker = context.Registry.GetService<IMarkerService>();
				INameService name = context.Registry.GetService<INameService>();
				foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>())
				{
					AntiDebugProtection2.AntiDebugPhase2.AntiMode mode = parameters.GetParameter<AntiDebugProtection2.AntiDebugPhase2.AntiMode>(context, module, "mode", AntiDebugProtection2.AntiDebugPhase2.AntiMode.Antinet);
					TypeDef attr = null;
					TypeDef rtType;
					switch (mode)
					{
					case AntiDebugProtection2.AntiDebugPhase2.AntiMode.Safe:
						rtType = rt.GetRuntimeType("Confuser.Runtime.AntiDebugSafe");
						break;
					case AntiDebugProtection2.AntiDebugPhase2.AntiMode.Win32:
						rtType = rt.GetRuntimeType("Confuser.Runtime.AntiDebugWin32");
						break;
					case AntiDebugProtection2.AntiDebugPhase2.AntiMode.Antinet:
						rtType = rt.GetRuntimeType("Confuser.Runtime.AntiDebugAntinet");
						attr = rt.GetRuntimeType("System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute");
						module.Types.Add(attr = InjectHelper.Inject(attr, module));
						foreach (IDnlibDef member in attr.FindDefinitions())
						{
							marker.Mark(member, (Protection)base.Parent);
							name.Analyze(member);
						}
						name.SetCanRename(attr, true);
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
							member2.Name = name.ObfuscateName(member2.Name, RenameMode.Sequential);
							name.SetCanRename(member2, true);
						}
					}
				}
			}

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x0600000B RID: 11 RVA: 0x0000629C File Offset: 0x0000449C
			public override string Name
			{
				get
				{
					return "Anti-debug injection";
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x0600000C RID: 12 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x02000004 RID: 4
			private enum AntiMode
			{
				// Token: 0x04000004 RID: 4
				Safe,
				// Token: 0x04000005 RID: 5
				Win32,
				// Token: 0x04000006 RID: 6
				Antinet
			}

			// Token: 0x02000005 RID: 5
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x0600000D RID: 13 RVA: 0x00004A5C File Offset: 0x00002C5C
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x0600000E RID: 14 RVA: 0x00004A68 File Offset: 0x00002C68
				public <>c()
				{
				}

				// Token: 0x0600000F RID: 15 RVA: 0x00004A71 File Offset: 0x00002C71
				internal bool <Execute>b__1_0(IDnlibDef method)
				{
					return method.Name == "Initialize";
				}

				// Token: 0x04000007 RID: 7
				public static readonly AntiDebugProtection2.AntiDebugPhase2.<>c <>9 = new AntiDebugProtection2.AntiDebugPhase2.<>c();

				// Token: 0x04000008 RID: 8
				public static Func<IDnlibDef, bool> <>9__1_0;
			}
		}
	}
}
