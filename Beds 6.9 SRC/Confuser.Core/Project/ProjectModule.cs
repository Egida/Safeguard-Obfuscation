using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using dnlib.DotNet;

namespace Confuser.Core.Project
{
	// Token: 0x0200008A RID: 138
	public class ProjectModule
	{
		// Token: 0x06000338 RID: 824 RVA: 0x00003726 File Offset: 0x00001926
		public ProjectModule()
		{
			this.Rules = new List<Rule>();
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0001479C File Offset: 0x0001299C
		public ProjectModule Clone()
		{
			ProjectModule ret = new ProjectModule();
			ret.Path = this.Path;
			ret.IsExternal = this.IsExternal;
			ret.SNKeyPath = this.SNKeyPath;
			ret.SNKeyPassword = this.SNKeyPassword;
			foreach (Rule r in this.Rules)
			{
				ret.Rules.Add(r.Clone());
			}
			return ret;
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00014838 File Offset: 0x00012A38
		internal void Load(XmlElement elem)
		{
			this.Path = elem.Attributes["path"].Value;
			bool flag = elem.Attributes["external"] != null;
			if (flag)
			{
				this.IsExternal = bool.Parse(elem.Attributes["external"].Value);
			}
			else
			{
				this.IsExternal = false;
			}
			bool flag2 = elem.Attributes["snKey"] != null;
			if (flag2)
			{
				this.SNKeyPath = elem.Attributes["snKey"].Value.NullIfEmpty();
			}
			else
			{
				this.SNKeyPath = null;
			}
			bool flag3 = elem.Attributes["snKeyPass"] != null;
			if (flag3)
			{
				this.SNKeyPassword = elem.Attributes["snKeyPass"].Value.NullIfEmpty();
			}
			else
			{
				this.SNKeyPassword = null;
			}
			this.Rules.Clear();
			foreach (XmlElement i in elem.ChildNodes.OfType<XmlElement>())
			{
				Rule rule = new Rule("true", ProtectionPreset.None, false);
				rule.Load(i);
				this.Rules.Add(rule);
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x000149AC File Offset: 0x00012BAC
		public byte[] LoadRaw(string basePath)
		{
			bool flag = basePath == null;
			byte[] result;
			if (flag)
			{
				result = File.ReadAllBytes(this.Path);
			}
			else
			{
				result = File.ReadAllBytes(System.IO.Path.Combine(basePath, this.Path));
			}
			return result;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x000149E8 File Offset: 0x00012BE8
		public ModuleDefMD Resolve(string basePath, ModuleContext context = null)
		{
			bool flag = basePath == null;
			ModuleDefMD result;
			if (flag)
			{
				result = ModuleDefMD.Load(this.Path, context);
			}
			else
			{
				result = ModuleDefMD.Load(System.IO.Path.Combine(basePath, this.Path), context);
			}
			return result;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x00014A24 File Offset: 0x00012C24
		internal XmlElement Save(XmlDocument xmlDoc)
		{
			XmlElement elem = xmlDoc.CreateElement("module", "http://confuser.codeplex.com");
			XmlAttribute nameAttr = xmlDoc.CreateAttribute("path");
			nameAttr.Value = this.Path;
			elem.Attributes.Append(nameAttr);
			bool isExternal = this.IsExternal;
			if (isExternal)
			{
				XmlAttribute extAttr = xmlDoc.CreateAttribute("external");
				extAttr.Value = this.IsExternal.ToString();
				elem.Attributes.Append(extAttr);
			}
			bool flag = this.SNKeyPath != null;
			if (flag)
			{
				XmlAttribute snKeyAttr = xmlDoc.CreateAttribute("snKey");
				snKeyAttr.Value = this.SNKeyPath;
				elem.Attributes.Append(snKeyAttr);
			}
			bool flag2 = this.SNKeyPassword != null;
			if (flag2)
			{
				XmlAttribute snKeyPassAttr = xmlDoc.CreateAttribute("snKeyPass");
				snKeyPassAttr.Value = this.SNKeyPassword;
				elem.Attributes.Append(snKeyPassAttr);
			}
			foreach (Rule i in this.Rules)
			{
				elem.AppendChild(i.Save(xmlDoc));
			}
			return elem;
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00014B74 File Offset: 0x00012D74
		public override string ToString()
		{
			return this.Path;
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600033F RID: 831 RVA: 0x0000373C File Offset: 0x0000193C
		// (set) Token: 0x06000340 RID: 832 RVA: 0x00003744 File Offset: 0x00001944
		public bool IsExternal
		{
			[CompilerGenerated]
			get
			{
				return this.<IsExternal>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<IsExternal>k__BackingField = value;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0000374D File Offset: 0x0000194D
		// (set) Token: 0x06000342 RID: 834 RVA: 0x00003755 File Offset: 0x00001955
		public string Path
		{
			[CompilerGenerated]
			get
			{
				return this.<Path>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Path>k__BackingField = value;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000343 RID: 835 RVA: 0x0000375E File Offset: 0x0000195E
		// (set) Token: 0x06000344 RID: 836 RVA: 0x00003766 File Offset: 0x00001966
		public IList<Rule> Rules
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

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000345 RID: 837 RVA: 0x0000376F File Offset: 0x0000196F
		// (set) Token: 0x06000346 RID: 838 RVA: 0x00003777 File Offset: 0x00001977
		public string SNKeyPassword
		{
			[CompilerGenerated]
			get
			{
				return this.<SNKeyPassword>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<SNKeyPassword>k__BackingField = value;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000347 RID: 839 RVA: 0x00003780 File Offset: 0x00001980
		// (set) Token: 0x06000348 RID: 840 RVA: 0x00003788 File Offset: 0x00001988
		public string SNKeyPath
		{
			[CompilerGenerated]
			get
			{
				return this.<SNKeyPath>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<SNKeyPath>k__BackingField = value;
			}
		}

		// Token: 0x0400024E RID: 590
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool <IsExternal>k__BackingField;

		// Token: 0x0400024F RID: 591
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Path>k__BackingField;

		// Token: 0x04000250 RID: 592
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<Rule> <Rules>k__BackingField;

		// Token: 0x04000251 RID: 593
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <SNKeyPassword>k__BackingField;

		// Token: 0x04000252 RID: 594
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <SNKeyPath>k__BackingField;
	}
}
