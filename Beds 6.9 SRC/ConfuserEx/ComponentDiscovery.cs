using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Confuser.Core;

namespace ConfuserEx
{
	// Token: 0x02000007 RID: 7
	internal class ComponentDiscovery
	{
		// Token: 0x0600001E RID: 30 RVA: 0x00002EAC File Offset: 0x000010AC
		private static void CrossDomainLoadComponents()
		{
			ComponentDiscovery.CrossDomainContext crossDomainContext = (ComponentDiscovery.CrossDomainContext)AppDomain.CurrentDomain.GetData("ctx");
			ConfuserEngine.Version.ToString();
			Assembly assembly = Assembly.LoadFile(crossDomainContext.PluginPath);
			foreach (Module module in assembly.GetLoadedModules())
			{
				foreach (Type type in module.GetTypes())
				{
					bool flag = type.IsAbstract || !PluginDiscovery.HasAccessibleDefConstructor(type);
					if (!flag)
					{
						bool flag2 = typeof(Protection).IsAssignableFrom(type);
						if (flag2)
						{
							Protection component = (Protection)Activator.CreateInstance(type);
							crossDomainContext.AddProtection(ComponentDiscovery.Info.FromComponent(component, crossDomainContext.PluginPath));
						}
						else
						{
							bool flag3 = typeof(Packer).IsAssignableFrom(type);
							if (flag3)
							{
								Packer component2 = (Packer)Activator.CreateInstance(type);
								crossDomainContext.AddPacker(ComponentDiscovery.Info.FromComponent(component2, crossDomainContext.PluginPath));
							}
						}
					}
				}
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002FCC File Offset: 0x000011CC
		public static void LoadComponents(IList<ConfuserComponent> protections, IList<ConfuserComponent> packers, string pluginPath)
		{
			ComponentDiscovery.CrossDomainContext data = new ComponentDiscovery.CrossDomainContext(protections, packers, pluginPath);
			AppDomain appDomain = AppDomain.CreateDomain("");
			appDomain.SetData("ctx", data);
			appDomain.DoCallBack(new CrossAppDomainDelegate(ComponentDiscovery.CrossDomainLoadComponents));
			AppDomain.Unload(appDomain);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003018 File Offset: 0x00001218
		public static void RemoveComponents(IList<ConfuserComponent> protections, IList<ConfuserComponent> packers, string pluginPath)
		{
			protections.RemoveWhere((ConfuserComponent comp) => comp is ComponentDiscovery.InfoComponent && ((ComponentDiscovery.InfoComponent)comp).info.path == pluginPath);
			packers.RemoveWhere((ConfuserComponent comp) => comp is ComponentDiscovery.InfoComponent && ((ComponentDiscovery.InfoComponent)comp).info.path == pluginPath);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002119 File Offset: 0x00000319
		public ComponentDiscovery()
		{
		}

		// Token: 0x02000008 RID: 8
		private class CrossDomainContext : MarshalByRefObject
		{
			// Token: 0x06000022 RID: 34 RVA: 0x0000213A File Offset: 0x0000033A
			public CrossDomainContext(IList<ConfuserComponent> protections, IList<ConfuserComponent> packers, string pluginPath)
			{
				this.protections = protections;
				this.packers = packers;
				this.pluginPath = pluginPath;
			}

			// Token: 0x17000005 RID: 5
			// (get) Token: 0x06000023 RID: 35 RVA: 0x0000305C File Offset: 0x0000125C
			public string PluginPath
			{
				get
				{
					return this.pluginPath;
				}
			}

			// Token: 0x06000024 RID: 36 RVA: 0x00003074 File Offset: 0x00001274
			public void AddProtection(ComponentDiscovery.Info info)
			{
				foreach (ConfuserComponent confuserComponent in this.protections)
				{
					bool flag = confuserComponent.Id == info.id;
					if (flag)
					{
						return;
					}
				}
				this.protections.Add(new ComponentDiscovery.InfoComponent(info));
			}

			// Token: 0x06000025 RID: 37 RVA: 0x000030E8 File Offset: 0x000012E8
			public void AddPacker(ComponentDiscovery.Info info)
			{
				foreach (ConfuserComponent confuserComponent in this.packers)
				{
					bool flag = confuserComponent.Id == info.id;
					if (flag)
					{
						return;
					}
				}
				this.packers.Add(new ComponentDiscovery.InfoComponent(info));
			}

			// Token: 0x04000009 RID: 9
			private readonly IList<ConfuserComponent> packers;

			// Token: 0x0400000A RID: 10
			private readonly string pluginPath;

			// Token: 0x0400000B RID: 11
			private readonly IList<ConfuserComponent> protections;
		}

		// Token: 0x02000009 RID: 9
		[Serializable]
		private class Info
		{
			// Token: 0x06000026 RID: 38 RVA: 0x0000315C File Offset: 0x0000135C
			public static ComponentDiscovery.Info FromComponent(ConfuserComponent component, string pluginPath)
			{
				return new ComponentDiscovery.Info
				{
					name = component.Name,
					desc = component.Description,
					id = component.Id,
					fullId = component.FullId,
					path = pluginPath
				};
			}

			// Token: 0x06000027 RID: 39 RVA: 0x00002119 File Offset: 0x00000319
			public Info()
			{
			}

			// Token: 0x0400000C RID: 12
			public string desc;

			// Token: 0x0400000D RID: 13
			public string fullId;

			// Token: 0x0400000E RID: 14
			public string id;

			// Token: 0x0400000F RID: 15
			public string name;

			// Token: 0x04000010 RID: 16
			public string path;
		}

		// Token: 0x0200000A RID: 10
		private class InfoComponent : ConfuserComponent
		{
			// Token: 0x06000028 RID: 40 RVA: 0x00002159 File Offset: 0x00000359
			public InfoComponent(ComponentDiscovery.Info info)
			{
				this.info = info;
			}

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x06000029 RID: 41 RVA: 0x000031AC File Offset: 0x000013AC
			public override string Name
			{
				get
				{
					return this.info.name;
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x0600002A RID: 42 RVA: 0x000031CC File Offset: 0x000013CC
			public override string Description
			{
				get
				{
					return this.info.desc;
				}
			}

			// Token: 0x17000008 RID: 8
			// (get) Token: 0x0600002B RID: 43 RVA: 0x000031EC File Offset: 0x000013EC
			public override string Id
			{
				get
				{
					return this.info.id;
				}
			}

			// Token: 0x17000009 RID: 9
			// (get) Token: 0x0600002C RID: 44 RVA: 0x0000320C File Offset: 0x0000140C
			public override string FullId
			{
				get
				{
					return this.info.fullId;
				}
			}

			// Token: 0x0600002D RID: 45 RVA: 0x0000216A File Offset: 0x0000036A
			protected override void Initialize(ConfuserContext context)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0600002E RID: 46 RVA: 0x0000216A File Offset: 0x0000036A
			protected override void PopulatePipeline(ProtectionPipeline pipeline)
			{
				throw new NotSupportedException();
			}

			// Token: 0x04000011 RID: 17
			public readonly ComponentDiscovery.Info info;
		}

		// Token: 0x0200000B RID: 11
		[CompilerGenerated]
		private sealed class <>c__DisplayClass2_0
		{
			// Token: 0x0600002F RID: 47 RVA: 0x00002119 File Offset: 0x00000319
			public <>c__DisplayClass2_0()
			{
			}

			// Token: 0x06000030 RID: 48 RVA: 0x00002172 File Offset: 0x00000372
			internal bool <RemoveComponents>b__0(ConfuserComponent comp)
			{
				return comp is ComponentDiscovery.InfoComponent && ((ComponentDiscovery.InfoComponent)comp).info.path == this.pluginPath;
			}

			// Token: 0x06000031 RID: 49 RVA: 0x00002172 File Offset: 0x00000372
			internal bool <RemoveComponents>b__1(ConfuserComponent comp)
			{
				return comp is ComponentDiscovery.InfoComponent && ((ComponentDiscovery.InfoComponent)comp).info.path == this.pluginPath;
			}

			// Token: 0x04000012 RID: 18
			public string pluginPath;
		}
	}
}
