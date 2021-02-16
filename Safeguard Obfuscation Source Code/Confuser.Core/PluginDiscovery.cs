using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Confuser.Core
{
	// Token: 0x0200005C RID: 92
	public class PluginDiscovery
	{
		// Token: 0x06000218 RID: 536 RVA: 0x00002563 File Offset: 0x00000763
		protected PluginDiscovery()
		{
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00002E67 File Offset: 0x00001067
		public void GetPlugins(ConfuserContext context, out IList<Protection> protections, out IList<Packer> packers, out IList<ConfuserComponent> components)
		{
			protections = new List<Protection>();
			packers = new List<Packer>();
			components = new List<ConfuserComponent>();
			this.GetPluginsInternal(context, protections, packers, components);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00010D3C File Offset: 0x0000EF3C
		public static bool HasAccessibleDefConstructor(Type type)
		{
			ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
			bool flag = ctor == null;
			return !flag && ctor.IsPublic;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00010D70 File Offset: 0x0000EF70
		protected static void AddPlugins(ConfuserContext context, IList<Protection> protections, IList<Packer> packers, IList<ConfuserComponent> components, Assembly asm)
		{
			foreach (Module module in asm.GetLoadedModules())
			{
				foreach (Type i in module.GetTypes())
				{
					bool flag = i.IsAbstract || !PluginDiscovery.HasAccessibleDefConstructor(i);
					if (!flag)
					{
						bool flag2 = typeof(Protection).IsAssignableFrom(i);
						if (flag2)
						{
							try
							{
								protections.Add((Protection)Activator.CreateInstance(i));
							}
							catch (Exception ex)
							{
								context.Logger.ErrorException("Failed to instantiate protection '" + i.Name + "'.", ex);
							}
						}
						else
						{
							bool flag3 = typeof(Packer).IsAssignableFrom(i);
							if (flag3)
							{
								try
								{
									packers.Add((Packer)Activator.CreateInstance(i));
								}
								catch (Exception ex2)
								{
									context.Logger.ErrorException("Failed to instantiate packer '" + i.Name + "'.", ex2);
								}
							}
							else
							{
								bool flag4 = typeof(ConfuserComponent).IsAssignableFrom(i);
								if (flag4)
								{
									try
									{
										components.Add((ConfuserComponent)Activator.CreateInstance(i));
									}
									catch (Exception ex3)
									{
										context.Logger.ErrorException("Failed to instantiate component '" + i.Name + "'.", ex3);
									}
								}
							}
						}
					}
				}
			}
			context.CheckCancellation();
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00010F2C File Offset: 0x0000F12C
		protected virtual void GetPluginsInternal(ConfuserContext context, IList<Protection> protections, IList<Packer> packers, IList<ConfuserComponent> components)
		{
			try
			{
				Assembly protAsm = Assembly.Load("Confuser.Protections");
				PluginDiscovery.AddPlugins(context, protections, packers, components, protAsm);
			}
			catch (Exception ex)
			{
				context.Logger.WarnException("Failed to load built-in protections.", ex);
			}
			try
			{
				Assembly renameAsm = Assembly.Load("Confuser.Renamer");
				PluginDiscovery.AddPlugins(context, protections, packers, components, renameAsm);
			}
			catch (Exception ex2)
			{
				context.Logger.WarnException("Failed to load renamer.", ex2);
			}
			try
			{
				Assembly renameAsm2 = Assembly.Load("Confuser.DynCipher");
				PluginDiscovery.AddPlugins(context, protections, packers, components, renameAsm2);
			}
			catch (Exception ex3)
			{
				context.Logger.WarnException("Failed to load dynamic cipher library.", ex3);
			}
			foreach (string pluginPath in context.Project.PluginPaths)
			{
				string realPath = Path.Combine(context.BaseDirectory, pluginPath);
				try
				{
					Assembly plugin = Assembly.LoadFile(realPath);
					PluginDiscovery.AddPlugins(context, protections, packers, components, plugin);
				}
				catch (Exception ex4)
				{
					context.Logger.WarnException("Failed to load plugin '" + pluginPath + "'.", ex4);
				}
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00002E8F File Offset: 0x0000108F
		// Note: this type is marked as 'beforefieldinit'.
		static PluginDiscovery()
		{
		}

		// Token: 0x040001C4 RID: 452
		internal static readonly PluginDiscovery Instance = new PluginDiscovery();
	}
}
