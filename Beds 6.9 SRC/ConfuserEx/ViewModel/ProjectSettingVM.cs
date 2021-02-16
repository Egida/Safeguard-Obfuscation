using System;
using System.Runtime.CompilerServices;
using Confuser.Core.Project;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200001D RID: 29
	public class ProjectSettingVM<T> : ViewModelBase, IViewModel<SettingItem<T>>
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x00002432 File Offset: 0x00000632
		public ProjectSettingVM(ProjectVM parent, SettingItem<T> setting)
		{
			this.parent = parent;
			this.setting = setting;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00004AE8 File Offset: 0x00002CE8
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00004B08 File Offset: 0x00002D08
		public string Id
		{
			get
			{
				return this.setting.Id;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.setting.Id != value, delegate(string val)
				{
					this.setting.Id = val;
				}, value, "Id");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00004B50 File Offset: 0x00002D50
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00004B70 File Offset: 0x00002D70
		public SettingItemAction Action
		{
			get
			{
				return this.setting.Action;
			}
			set
			{
				bool flag = base.SetProperty<SettingItemAction>(this.setting.Action != value, delegate(SettingItemAction val)
				{
					this.setting.Action = val;
				}, value, "Action");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000AC RID: 172 RVA: 0x00004BB8 File Offset: 0x00002DB8
		SettingItem<T> IViewModel<SettingItem<!0>>.Model
		{
			get
			{
				return this.setting;
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000244A File Offset: 0x0000064A
		[CompilerGenerated]
		private void <set_Id>b__5_0(string val)
		{
			this.setting.Id = val;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00002459 File Offset: 0x00000659
		[CompilerGenerated]
		private void <set_Action>b__8_0(SettingItemAction val)
		{
			this.setting.Action = val;
		}

		// Token: 0x0400004B RID: 75
		private readonly ProjectVM parent;

		// Token: 0x0400004C RID: 76
		private readonly SettingItem<T> setting;
	}
}
