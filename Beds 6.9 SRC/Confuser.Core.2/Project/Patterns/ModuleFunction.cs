using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	/// <summary>
	///     A function that compare the module of definition.
	/// </summary>
	// Token: 0x0200009B RID: 155
	public class ModuleFunction : PatternFunction
	{
		/// <inheritdoc />
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600038E RID: 910 RVA: 0x00015A18 File Offset: 0x00013C18
		public override string Name
		{
			get
			{
				return "module";
			}
		}

		/// <inheritdoc />
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600038F RID: 911 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		/// <inheritdoc />
		// Token: 0x06000390 RID: 912 RVA: 0x00015A30 File Offset: 0x00013C30
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = !(definition is IOwnerModule) && !(definition is IModule);
			object result;
			if (flag)
			{
				result = false;
			}
			else
			{
				object name = base.Arguments[0].Evaluate(definition);
				bool flag2 = definition is IModule;
				if (flag2)
				{
					result = (((IModule)definition).Name == name.ToString());
				}
				else
				{
					result = (((IOwnerModule)definition).Module.Name == name.ToString());
				}
			}
			return result;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0000385A File Offset: 0x00001A5A
		public ModuleFunction()
		{
		}

		// Token: 0x04000268 RID: 616
		internal const string FnName = "module";
	}
}
