using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x0200007E RID: 126
	internal class ResourceAnalyzer : IRenamer
	{
		// Token: 0x060002DD RID: 733 RVA: 0x00022020 File Offset: 0x00020220
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDef module = def as ModuleDef;
			bool flag = module == null;
			if (!flag)
			{
				string asmName = module.Assembly.Name.String;
				bool flag2 = !string.IsNullOrEmpty(module.Assembly.Culture) && asmName.EndsWith(".resources");
				if (flag2)
				{
					Regex satellitePattern = new Regex(string.Format("^(.*)\\.{0}\\.resources$", module.Assembly.Culture));
					string nameAsmName = asmName.Substring(0, asmName.Length - ".resources".Length);
					ModuleDef mainModule = context.Modules.SingleOrDefault((ModuleDefMD mod) => mod.Assembly.Name == nameAsmName);
					bool flag3 = mainModule == null;
					if (flag3)
					{
						context.Logger.ErrorFormat("Could not find main assembly of satellite assembly '{0}'.", new object[]
						{
							module.Assembly.FullName
						});
						throw new ConfuserException(null);
					}
					string format = "{0}." + module.Assembly.Culture + ".resources";
					using (IEnumerator<Resource> enumerator = module.Resources.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Resource res = enumerator.Current;
							Match match = satellitePattern.Match(res.Name);
							bool success = match.Success;
							if (success)
							{
								string typeName = match.Groups[1].Value;
								TypeDef type = mainModule.FindReflectionThrow(typeName);
								bool flag4 = type == null;
								if (flag4)
								{
									context.Logger.WarnFormat("Could not find resource type '{0}'.", new object[]
									{
										typeName
									});
								}
								else
								{
									service.ReduceRenameMode(type, RenameMode.ASCII);
									service.AddReference<TypeDef>(type, new ResourceReference(res, type, format));
								}
							}
						}
						return;
					}
				}
				string format2 = "{0}.resources";
				foreach (Resource res2 in module.Resources)
				{
					Match match2 = ResourceAnalyzer.ResourceNamePattern.Match(res2.Name);
					bool flag5 = match2.Success && res2.ResourceType == ResourceType.Embedded;
					if (flag5)
					{
						string typeName2 = match2.Groups[1].Value;
						bool flag6 = !typeName2.EndsWith(".g");
						if (flag6)
						{
							TypeDef type2 = module.FindReflection(typeName2);
							bool flag7 = type2 == null;
							if (flag7)
							{
								context.Logger.WarnFormat("Could not find resource type '{0}'.", new object[]
								{
									typeName2
								});
							}
							else
							{
								service.ReduceRenameMode(type2, RenameMode.ASCII);
								service.AddReference<TypeDef>(type2, new ResourceReference(res2, type2, format2));
							}
						}
					}
				}
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00002184 File Offset: 0x00000384
		public ResourceAnalyzer()
		{
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000359B File Offset: 0x0000179B
		// Note: this type is marked as 'beforefieldinit'.
		static ResourceAnalyzer()
		{
		}

		// Token: 0x04000539 RID: 1337
		private static readonly Regex ResourceNamePattern = new Regex("^(.*)\\.resources$");

		// Token: 0x0200007F RID: 127
		[CompilerGenerated]
		private sealed class <>c__DisplayClass0_0
		{
			// Token: 0x060002E2 RID: 738 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass0_0()
			{
			}

			// Token: 0x060002E3 RID: 739 RVA: 0x000035AC File Offset: 0x000017AC
			internal bool <Analyze>b__0(ModuleDefMD mod)
			{
				return mod.Assembly.Name == this.nameAsmName;
			}

			// Token: 0x0400053A RID: 1338
			public string nameAsmName;
		}
	}
}
