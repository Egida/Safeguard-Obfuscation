using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace ConfuserEx
{
	// Token: 0x02000015 RID: 21
	public partial class App : Application
	{
		// Token: 0x06000065 RID: 101 RVA: 0x00003DD0 File Offset: 0x00001FD0
		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string text = args.Name.Contains(',') ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");
			text = text.Replace(".", "_");
			bool flag = text.EndsWith("_resources");
			Assembly result;
			if (flag)
			{
				result = null;
			}
			else
			{
				ResourceManager resourceManager = new ResourceManager(base.GetType().Namespace + ".Properties.Resources", Assembly.GetExecutingAssembly());
				byte[] rawAssembly = (byte[])resourceManager.GetObject(text);
				result = Assembly.Load(rawAssembly);
			}
			return result;
		}
	}
}
