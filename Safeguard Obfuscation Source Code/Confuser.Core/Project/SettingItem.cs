using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Confuser.Core.Project
{
	// Token: 0x0200008D RID: 141
	public class SettingItem<T> : Dictionary<string, string>
	{
		// Token: 0x06000356 RID: 854 RVA: 0x00003815 File Offset: 0x00001A15
		public SettingItem(string id = null, SettingItemAction action = SettingItemAction.Add)
		{
			this.Id = id;
			this.Action = action;
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00014EEC File Offset: 0x000130EC
		public SettingItem<T> Clone()
		{
			SettingItem<T> item = new SettingItem<T>(this.Id, this.Action);
			foreach (KeyValuePair<string, string> entry in this)
			{
				item.Add(entry.Key, entry.Value);
			}
			return item;
		}

		// Token: 0x06000358 RID: 856 RVA: 0x00014F64 File Offset: 0x00013164
		internal void Load(XmlElement elem)
		{
			this.Id = elem.Attributes["id"].Value;
			bool flag = elem.Attributes["action"] != null;
			if (flag)
			{
				this.Action = (SettingItemAction)Enum.Parse(typeof(SettingItemAction), elem.Attributes["action"].Value, true);
			}
			else
			{
				this.Action = SettingItemAction.Add;
			}
			base.Clear();
			foreach (XmlElement i in elem.ChildNodes.OfType<XmlElement>())
			{
				base.Add(i.Attributes["name"].Value, i.Attributes["value"].Value);
			}
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00015060 File Offset: 0x00013260
		internal XmlElement Save(XmlDocument xmlDoc)
		{
			XmlElement elem = xmlDoc.CreateElement((typeof(T) == typeof(Packer)) ? "packer" : "protection", "http://confuser.codeplex.com");
			XmlAttribute idAttr = xmlDoc.CreateAttribute("id");
			idAttr.Value = this.Id;
			elem.Attributes.Append(idAttr);
			bool flag = this.Action > SettingItemAction.Add;
			if (flag)
			{
				XmlAttribute pAttr = xmlDoc.CreateAttribute("action");
				pAttr.Value = this.Action.ToString().ToLower();
				elem.Attributes.Append(pAttr);
			}
			foreach (KeyValuePair<string, string> i in this)
			{
				XmlElement arg = xmlDoc.CreateElement("argument", "http://confuser.codeplex.com");
				XmlAttribute nameAttr = xmlDoc.CreateAttribute("name");
				nameAttr.Value = i.Key;
				arg.Attributes.Append(nameAttr);
				XmlAttribute valAttr = xmlDoc.CreateAttribute("value");
				valAttr.Value = i.Value;
				arg.Attributes.Append(valAttr);
				elem.AppendChild(arg);
			}
			return elem;
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600035A RID: 858 RVA: 0x0000382F File Offset: 0x00001A2F
		// (set) Token: 0x0600035B RID: 859 RVA: 0x00003837 File Offset: 0x00001A37
		public SettingItemAction Action
		{
			[CompilerGenerated]
			get
			{
				return this.<Action>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Action>k__BackingField = value;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600035C RID: 860 RVA: 0x00003840 File Offset: 0x00001A40
		// (set) Token: 0x0600035D RID: 861 RVA: 0x00003848 File Offset: 0x00001A48
		public string Id
		{
			[CompilerGenerated]
			get
			{
				return this.<Id>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Id>k__BackingField = value;
			}
		}

		// Token: 0x04000257 RID: 599
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private SettingItemAction <Action>k__BackingField;

		// Token: 0x04000258 RID: 600
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Id>k__BackingField;
	}
}
