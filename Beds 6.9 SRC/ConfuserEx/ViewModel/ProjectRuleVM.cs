using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.Core.Project.Patterns;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200001B RID: 27
	public class ProjectRuleVM : ViewModelBase, IViewModel<Rule>
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00004870 File Offset: 0x00002A70
		public ProjectRuleVM(ProjectVM parent, Rule rule)
		{
			this.parent = parent;
			this.rule = rule;
			ObservableCollection<ProjectSettingVM<Protection>> observableCollection = Utils.Wrap<SettingItem<Protection>, ProjectSettingVM<Protection>>(rule, (SettingItem<Protection> setting) => new ProjectSettingVM<Protection>(parent, setting));
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				parent.IsModified = true;
			};
			this.Protections = observableCollection;
			this.ParseExpression();
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000092 RID: 146 RVA: 0x000048DC File Offset: 0x00002ADC
		public ProjectVM Project
		{
			get
			{
				return this.parent;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000048F4 File Offset: 0x00002AF4
		// (set) Token: 0x06000094 RID: 148 RVA: 0x00004914 File Offset: 0x00002B14
		public string Pattern
		{
			get
			{
				return this.rule.Pattern;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.rule.Pattern != value, delegate(string val)
				{
					this.rule.Pattern = val;
				}, value, "Pattern");
				if (flag)
				{
					this.parent.IsModified = true;
					this.ParseExpression();
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00004968 File Offset: 0x00002B68
		// (set) Token: 0x06000096 RID: 150 RVA: 0x000023AB File Offset: 0x000005AB
		public PatternExpression Expression
		{
			get
			{
				return this.exp;
			}
			set
			{
				base.SetProperty<PatternExpression>(ref this.exp, value, "Expression");
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00004980 File Offset: 0x00002B80
		// (set) Token: 0x06000098 RID: 152 RVA: 0x000023C1 File Offset: 0x000005C1
		public string ExpressionError
		{
			get
			{
				return this.error;
			}
			set
			{
				base.SetProperty<string>(ref this.error, value, "ExpressionError");
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00004998 File Offset: 0x00002B98
		// (set) Token: 0x0600009A RID: 154 RVA: 0x000049B8 File Offset: 0x00002BB8
		public ProtectionPreset Preset
		{
			get
			{
				return this.rule.Preset;
			}
			set
			{
				bool flag = base.SetProperty<ProtectionPreset>(this.rule.Preset != value, delegate(ProtectionPreset val)
				{
					this.rule.Preset = val;
				}, value, "Preset");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00004A00 File Offset: 0x00002C00
		// (set) Token: 0x0600009C RID: 156 RVA: 0x00004A20 File Offset: 0x00002C20
		public bool Inherit
		{
			get
			{
				return this.rule.Inherit;
			}
			set
			{
				bool flag = base.SetProperty<bool>(this.rule.Inherit != value, delegate(bool val)
				{
					this.rule.Inherit = val;
				}, value, "Inherit");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600009D RID: 157 RVA: 0x000023D7 File Offset: 0x000005D7
		// (set) Token: 0x0600009E RID: 158 RVA: 0x000023DF File Offset: 0x000005DF
		public IList<ProjectSettingVM<Protection>> Protections
		{
			[CompilerGenerated]
			get
			{
				return this.<Protections>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Protections>k__BackingField = value;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00004A68 File Offset: 0x00002C68
		Rule IViewModel<Rule>.Model
		{
			get
			{
				return this.rule;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004A80 File Offset: 0x00002C80
		private void ParseExpression()
		{
			bool flag = this.Pattern == null;
			if (!flag)
			{
				PatternExpression expression;
				try
				{
					expression = new PatternParser().Parse(this.Pattern);
					this.ExpressionError = null;
				}
				catch (Exception ex)
				{
					this.ExpressionError = ex.Message;
					expression = null;
				}
				this.Expression = expression;
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000023E8 File Offset: 0x000005E8
		[CompilerGenerated]
		private void <set_Pattern>b__9_0(string val)
		{
			this.rule.Pattern = val;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000023F7 File Offset: 0x000005F7
		[CompilerGenerated]
		private void <set_Preset>b__18_0(ProtectionPreset val)
		{
			this.rule.Preset = val;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00002406 File Offset: 0x00000606
		[CompilerGenerated]
		private void <set_Inherit>b__21_0(bool val)
		{
			this.rule.Inherit = val;
		}

		// Token: 0x04000045 RID: 69
		private readonly ProjectVM parent;

		// Token: 0x04000046 RID: 70
		private readonly Rule rule;

		// Token: 0x04000047 RID: 71
		private string error;

		// Token: 0x04000048 RID: 72
		private PatternExpression exp;

		// Token: 0x04000049 RID: 73
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<ProjectSettingVM<Protection>> <Protections>k__BackingField;

		// Token: 0x0200001C RID: 28
		[CompilerGenerated]
		private sealed class <>c__DisplayClass4_0
		{
			// Token: 0x060000A4 RID: 164 RVA: 0x00002119 File Offset: 0x00000319
			public <>c__DisplayClass4_0()
			{
			}

			// Token: 0x060000A5 RID: 165 RVA: 0x00002415 File Offset: 0x00000615
			internal ProjectSettingVM<Protection> <.ctor>b__0(SettingItem<Protection> setting)
			{
				return new ProjectSettingVM<Protection>(this.parent, setting);
			}

			// Token: 0x060000A6 RID: 166 RVA: 0x00002423 File Offset: 0x00000623
			internal void <.ctor>b__1(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.parent.IsModified = true;
			}

			// Token: 0x0400004A RID: 74
			public ProjectVM parent;
		}
	}
}
