using Confuser.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfuserEx
{
  internal class ComponentDiscovery
  {
    private static void CrossDomainLoadComponents()
    {
      ComponentDiscovery.CrossDomainContext data = (ComponentDiscovery.CrossDomainContext) AppDomain.CurrentDomain.GetData("ctx");
      ConfuserEngine.Version.ToString();
      foreach (Module loadedModule in Assembly.LoadFile(data.PluginPath).GetLoadedModules())
      {
        foreach (Type type in loadedModule.GetTypes())
        {
          if (!type.IsAbstract && PluginDiscovery.HasAccessibleDefConstructor(type))
          {
            if (typeof (Protection).IsAssignableFrom(type))
            {
              Protection instance = (Protection) Activator.CreateInstance(type);
              data.AddProtection(ComponentDiscovery.Info.FromComponent((ConfuserComponent) instance, data.PluginPath));
            }
            else if (typeof (Packer).IsAssignableFrom(type))
            {
              Packer instance = (Packer) Activator.CreateInstance(type);
              data.AddPacker(ComponentDiscovery.Info.FromComponent((ConfuserComponent) instance, data.PluginPath));
            }
          }
        }
      }
    }

    public static void LoadComponents(
      IList<ConfuserComponent> protections,
      IList<ConfuserComponent> packers,
      string pluginPath)
    {
      ComponentDiscovery.CrossDomainContext crossDomainContext = new ComponentDiscovery.CrossDomainContext(protections, packers, pluginPath);
      AppDomain domain = AppDomain.CreateDomain("");
      domain.SetData("ctx", (object) crossDomainContext);
      domain.DoCallBack(new CrossAppDomainDelegate(ComponentDiscovery.CrossDomainLoadComponents));
      AppDomain.Unload(domain);
    }

    public static void RemoveComponents(
      IList<ConfuserComponent> protections,
      IList<ConfuserComponent> packers,
      string pluginPath)
    {
      protections.RemoveWhere<ConfuserComponent>((Predicate<ConfuserComponent>) (comp => comp is ComponentDiscovery.InfoComponent && ((ComponentDiscovery.InfoComponent) comp).info.path == pluginPath));
      packers.RemoveWhere<ConfuserComponent>((Predicate<ConfuserComponent>) (comp => comp is ComponentDiscovery.InfoComponent && ((ComponentDiscovery.InfoComponent) comp).info.path == pluginPath));
    }

    private class CrossDomainContext : MarshalByRefObject
    {
      private readonly IList<ConfuserComponent> packers;
      private readonly string pluginPath;
      private readonly IList<ConfuserComponent> protections;

      public CrossDomainContext(
        IList<ConfuserComponent> protections,
        IList<ConfuserComponent> packers,
        string pluginPath)
      {
        this.protections = protections;
        this.packers = packers;
        this.pluginPath = pluginPath;
      }

      public string PluginPath => this.pluginPath;

      public void AddProtection(ComponentDiscovery.Info info)
      {
        foreach (ConfuserComponent protection in (IEnumerable<ConfuserComponent>) this.protections)
        {
          if (protection.Id == info.id)
            return;
        }
        this.protections.Add((ConfuserComponent) new ComponentDiscovery.InfoComponent(info));
      }

      public void AddPacker(ComponentDiscovery.Info info)
      {
        foreach (ConfuserComponent packer in (IEnumerable<ConfuserComponent>) this.packers)
        {
          if (packer.Id == info.id)
            return;
        }
        this.packers.Add((ConfuserComponent) new ComponentDiscovery.InfoComponent(info));
      }
    }

    [Serializable]
    private class Info
    {
      public string desc;
      public string fullId;
      public string id;
      public string name;
      public string path;

      public static ComponentDiscovery.Info FromComponent(
        ConfuserComponent component,
        string pluginPath)
      {
        return new ComponentDiscovery.Info()
        {
          name = component.Name,
          desc = component.Description,
          id = component.Id,
          fullId = component.FullId,
          path = pluginPath
        };
      }
    }

    private class InfoComponent : ConfuserComponent
    {
      public readonly ComponentDiscovery.Info info;

      public InfoComponent(ComponentDiscovery.Info info) => this.info = info;

      public override string Name => this.info.name;

      public override string Description => this.info.desc;

      public override string Id => this.info.id;

      public override string FullId => this.info.fullId;

      protected override void Initialize(ConfuserContext context) => throw new NotSupportedException();

      protected override void PopulatePipeline(ProtectionPipeline pipeline) => throw new NotSupportedException();
    }
  }
}
