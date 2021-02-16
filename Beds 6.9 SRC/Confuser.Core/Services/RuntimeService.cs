using System;
using System.IO;
using System.Reflection;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x0200007C RID: 124
	internal class RuntimeService : IRuntimeService
	{
		// Token: 0x060002DA RID: 730 RVA: 0x00012A74 File Offset: 0x00010C74
		public TypeDef GetRuntimeType(string fullName)
		{
			bool flag = this.rtModule == null;
			if (flag)
			{
				Module module = typeof(RuntimeService).Assembly.ManifestModule;
				string rtPath = "Confuser.Runtime.dll";
				bool flag2 = module.FullyQualifiedName[0] != '<';
				if (flag2)
				{
					rtPath = Path.Combine(Path.GetDirectoryName(module.FullyQualifiedName), rtPath);
				}
				this.rtModule = ModuleDefMD.Load(rtPath, new ModuleCreationOptions
				{
					TryToLoadPdbFromDisk = true
				});
				this.rtModule.EnableTypeDefFindCache = true;
			}
			return this.rtModule.Find(fullName, true);
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00002194 File Offset: 0x00000394
		public RuntimeService()
		{
		}

		// Token: 0x04000226 RID: 550
		private ModuleDef rtModule;
	}
}
