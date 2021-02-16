using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Confuser.Core.Project;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000024 RID: 36
	public class ProjectModuleVM : ViewModelBase, IViewModel<ProjectModule>, IRuleContainer
	{
		// Token: 0x060000DA RID: 218 RVA: 0x000051D8 File Offset: 0x000033D8
		public ProjectModuleVM(ProjectVM parent, ProjectModule module)
		{
			this.parent = parent;
			this.module = module;
			ObservableCollection<ProjectRuleVM> observableCollection = Utils.Wrap<Rule, ProjectRuleVM>(module.Rules, (Rule rule) => new ProjectRuleVM(parent, rule));
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				parent.IsModified = true;
			};
			this.Rules = observableCollection;
			bool flag = module.Path != null;
			if (flag)
			{
				this.SimpleName = System.IO.Path.GetFileName(module.Path);
				this.LoadAssemblyName();
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00005274 File Offset: 0x00003474
		// (set) Token: 0x060000DC RID: 220 RVA: 0x0000254E File Offset: 0x0000074E
		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			set
			{
				base.SetProperty<bool>(ref this.isSelected, value, "IsSelected");
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000DD RID: 221 RVA: 0x0000528C File Offset: 0x0000348C
		public ProjectModule Module
		{
			get
			{
				return this.module;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000DE RID: 222 RVA: 0x000052A4 File Offset: 0x000034A4
		// (set) Token: 0x060000DF RID: 223 RVA: 0x000052C4 File Offset: 0x000034C4
		public string Path
		{
			get
			{
				return this.module.Path;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.module.Path != value, delegate(string val)
				{
					this.module.Path = val;
				}, value, "Path");
				if (flag)
				{
					this.parent.IsModified = true;
					this.SimpleName = System.IO.Path.GetFileName(this.module.Path);
					this.LoadAssemblyName();
				}
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x0000532C File Offset: 0x0000352C
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x00002564 File Offset: 0x00000764
		public string SimpleName
		{
			get
			{
				return this.simpleName;
			}
			private set
			{
				base.SetProperty<string>(ref this.simpleName, value, "SimpleName");
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00005344 File Offset: 0x00003544
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x0000257A File Offset: 0x0000077A
		public string AssemblyName
		{
			get
			{
				return this.asmName;
			}
			private set
			{
				base.SetProperty<string>(ref this.asmName, value, "AssemblyName");
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x0000535C File Offset: 0x0000355C
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x0000537C File Offset: 0x0000357C
		public string SNKeyPath
		{
			get
			{
				return this.module.SNKeyPath;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.module.SNKeyPath != value, delegate(string val)
				{
					this.module.SNKeyPath = val;
				}, value, "SNKeyPath");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x000053C4 File Offset: 0x000035C4
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x000053E4 File Offset: 0x000035E4
		public string SNKeyPassword
		{
			get
			{
				return this.module.SNKeyPassword;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.module.SNKeyPassword != value, delegate(string val)
				{
					this.module.SNKeyPassword = val;
				}, value, "SNKeyPassword");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00002590 File Offset: 0x00000790
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00002598 File Offset: 0x00000798
		public IList<ProjectRuleVM> Rules
		{
			[CompilerGenerated]
			get
			{
				return this.<Rules>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Rules>k__BackingField = value;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000EA RID: 234 RVA: 0x0000528C File Offset: 0x0000348C
		ProjectModule IViewModel<ProjectModule>.Model
		{
			get
			{
				return this.module;
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000025A1 File Offset: 0x000007A1
		private void LoadAssemblyName()
		{
			this.AssemblyName = "Loading...";
			ThreadPool.QueueUserWorkItem(delegate(object _)
			{
				try
				{
					string text = System.IO.Path.Combine(this.parent.BaseDirectory, this.Path);
					bool flag = !string.IsNullOrEmpty(this.parent.FileName);
					if (flag)
					{
						text = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.parent.FileName), text);
					}
					AssemblyName assemblyName = System.Reflection.AssemblyName.GetAssemblyName(text);
					this.AssemblyName = assemblyName.FullName;
				}
				catch
				{
					this.AssemblyName = "Unknown";
				}
			});
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000025C2 File Offset: 0x000007C2
		[CompilerGenerated]
		private void <set_Path>b__13_0(string val)
		{
			this.module.Path = val;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000025D1 File Offset: 0x000007D1
		[CompilerGenerated]
		private void <set_SNKeyPath>b__22_0(string val)
		{
			this.module.SNKeyPath = val;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x000025E0 File Offset: 0x000007E0
		[CompilerGenerated]
		private void <set_SNKeyPassword>b__25_0(string val)
		{
			this.module.SNKeyPassword = val;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000542C File Offset: 0x0000362C
		[CompilerGenerated]
		private void <LoadAssemblyName>b__32_0(object _)
		{
			try
			{
				string text = System.IO.Path.Combine(this.parent.BaseDirectory, this.Path);
				bool flag = !string.IsNullOrEmpty(this.parent.FileName);
				if (flag)
				{
					text = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.parent.FileName), text);
				}
				AssemblyName assemblyName = System.Reflection.AssemblyName.GetAssemblyName(text);
				this.AssemblyName = assemblyName.FullName;
			}
			catch
			{
				this.AssemblyName = "Unknown";
			}
		}

		// Token: 0x04000059 RID: 89
		private readonly ProjectModule module;

		// Token: 0x0400005A RID: 90
		private readonly ProjectVM parent;

		// Token: 0x0400005B RID: 91
		private string asmName = "Unknown";

		// Token: 0x0400005C RID: 92
		private string simpleName;

		// Token: 0x0400005D RID: 93
		private bool isSelected;

		// Token: 0x0400005E RID: 94
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<ProjectRuleVM> <Rules>k__BackingField;

		// Token: 0x02000025 RID: 37
		[CompilerGenerated]
		private sealed class <>c__DisplayClass5_0
		{
			// Token: 0x060000F0 RID: 240 RVA: 0x00002119 File Offset: 0x00000319
			public <>c__DisplayClass5_0()
			{
			}

			// Token: 0x060000F1 RID: 241 RVA: 0x000025EF File Offset: 0x000007EF
			internal ProjectRuleVM <.ctor>b__0(Rule rule)
			{
				return new ProjectRuleVM(this.parent, rule);
			}

			// Token: 0x060000F2 RID: 242 RVA: 0x000025FD File Offset: 0x000007FD
			internal void <.ctor>b__1(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.parent.IsModified = true;
			}

			// Token: 0x0400005F RID: 95
			public ProjectVM parent;
		}
	}
}
