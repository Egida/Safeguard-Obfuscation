using System;
using System.IO;
using System.Threading;
using Confuser.Core.Project;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x0200005E RID: 94
	public abstract class Packer : ConfuserComponent
	{
		// Token: 0x0600022F RID: 559
		protected internal abstract void Pack(ConfuserContext context, ProtectionParameters parameters);

		// Token: 0x06000230 RID: 560 RVA: 0x0001109C File Offset: 0x0000F29C
		protected void ProtectStub(ConfuserContext context, string fileName, byte[] module, StrongNameKey snKey, Protection prot = null)
		{
			string tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			string outDir = Path.Combine(tmpDir, Path.GetRandomFileName());
			Directory.CreateDirectory(tmpDir);
			for (int i = 0; i < context.OutputModules.Count; i++)
			{
				string path = Path.GetFullPath(Path.Combine(tmpDir, context.OutputPaths[i]));
				string dir = Path.GetDirectoryName(path);
				bool flag = !Directory.Exists(dir);
				if (flag)
				{
					Directory.CreateDirectory(dir);
				}
				File.WriteAllBytes(path, context.OutputModules[i]);
			}
			File.WriteAllBytes(Path.Combine(tmpDir, fileName), module);
			ConfuserProject proj = new ConfuserProject();
			proj.Seed = context.Project.Seed;
			foreach (Rule rule in context.Project.Rules)
			{
				proj.Rules.Add(rule);
			}
			proj.Add(new ProjectModule
			{
				Path = fileName
			});
			proj.BaseDirectory = tmpDir;
			proj.OutputDirectory = outDir;
			foreach (string path2 in context.Project.ProbePaths)
			{
				proj.ProbePaths.Add(path2);
			}
			proj.ProbePaths.Add(context.Project.BaseDirectory);
			PluginDiscovery discovery = null;
			bool flag2 = prot != null;
			if (flag2)
			{
				Rule rule2 = new Rule("true", ProtectionPreset.None, false)
				{
					Preset = ProtectionPreset.None,
					Inherit = true,
					Pattern = "true"
				};
				rule2.Add(new SettingItem<Protection>(null, SettingItemAction.Add)
				{
					Id = prot.Id,
					Action = SettingItemAction.Add
				});
				proj.Rules.Add(rule2);
				discovery = new PackerDiscovery(prot);
			}
			try
			{
				ConfuserEngine.Run(new ConfuserParameters
				{
					Logger = new PackerLogger(context.Logger),
					PluginDiscovery = discovery,
					Marker = new PackerMarker(snKey),
					Project = proj,
					PackerInitiated = true
				}, new CancellationToken?(context.token)).Wait();
			}
			catch (AggregateException ex)
			{
				context.Logger.Error("Failed to protect packer stub.");
				throw new ConfuserException(ex);
			}
			context.OutputModules = new byte[][]
			{
				File.ReadAllBytes(Path.Combine(outDir, fileName))
			};
			context.OutputPaths = new string[]
			{
				fileName
			};
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00002EA7 File Offset: 0x000010A7
		protected Packer()
		{
		}
	}
}
