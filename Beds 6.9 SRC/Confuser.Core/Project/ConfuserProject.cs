using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;

namespace Confuser.Core.Project
{
	// Token: 0x02000082 RID: 130
	public class ConfuserProject : List<ProjectModule>
	{
		// Token: 0x060002F3 RID: 755 RVA: 0x00003468 File Offset: 0x00001668
		public ConfuserProject()
		{
			this.ProbePaths = new List<string>();
			this.PluginPaths = new List<string>();
			this.Rules = new List<Rule>();
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00013594 File Offset: 0x00011794
		public ConfuserProject Clone()
		{
			ConfuserProject ret = new ConfuserProject();
			ret.Seed = this.Seed;
			ret.Debug = this.Debug;
			ret.OutputDirectory = this.OutputDirectory;
			ret.BaseDirectory = this.BaseDirectory;
			ret.Packer = ((this.Packer == null) ? null : this.Packer.Clone());
			ret.ProbePaths = new List<string>(this.ProbePaths);
			ret.PluginPaths = new List<string>(this.PluginPaths);
			foreach (ProjectModule module in this)
			{
				ret.Add(module.Clone());
			}
			foreach (Rule r in this.Rules)
			{
				ret.Rules.Add(r);
			}
			return ret;
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x000136B8 File Offset: 0x000118B8
		public void Load(XmlDocument doc)
		{
			doc.Schemas.Add(ConfuserProject.Schema);
			List<XmlSchemaException> exceptions = new List<XmlSchemaException>();
			doc.Validate(delegate(object sender, ValidationEventArgs e)
			{
				bool flag8 = e.Severity > XmlSeverityType.Error;
				if (!flag8)
				{
					exceptions.Add(e.Exception);
				}
			});
			bool flag = exceptions.Count > 0;
			if (flag)
			{
				throw new ProjectValidationException(exceptions);
			}
			XmlElement docElem = doc.DocumentElement;
			this.OutputDirectory = docElem.Attributes["outputDir"].Value;
			this.BaseDirectory = docElem.Attributes["baseDir"].Value;
			bool flag2 = docElem.Attributes["seed"] != null;
			if (flag2)
			{
				this.Seed = docElem.Attributes["seed"].Value.NullIfEmpty();
			}
			else
			{
				this.Seed = null;
			}
			bool flag3 = docElem.Attributes["debug"] != null;
			if (flag3)
			{
				this.Debug = bool.Parse(docElem.Attributes["debug"].Value);
			}
			else
			{
				this.Debug = false;
			}
			this.Packer = null;
			base.Clear();
			this.ProbePaths.Clear();
			this.PluginPaths.Clear();
			this.Rules.Clear();
			foreach (XmlElement i in docElem.ChildNodes.OfType<XmlElement>())
			{
				bool flag4 = i.Name == "rule";
				if (flag4)
				{
					Rule rule = new Rule("true", ProtectionPreset.None, false);
					rule.Load(i);
					this.Rules.Add(rule);
				}
				else
				{
					bool flag5 = i.Name == "packer";
					if (flag5)
					{
						this.Packer = new SettingItem<Packer>(null, SettingItemAction.Add);
						this.Packer.Load(i);
					}
					else
					{
						bool flag6 = i.Name == "probePath";
						if (flag6)
						{
							this.ProbePaths.Add(i.InnerText);
						}
						else
						{
							bool flag7 = i.Name == "plugin";
							if (flag7)
							{
								this.PluginPaths.Add(i.InnerText);
							}
							else
							{
								ProjectModule asm = new ProjectModule();
								asm.Load(i);
								base.Add(asm);
							}
						}
					}
				}
			}
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00013968 File Offset: 0x00011B68
		public XmlDocument Save()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Schemas.Add(ConfuserProject.Schema);
			XmlElement elem = xmlDoc.CreateElement("project", "http://confuser.codeplex.com");
			XmlAttribute outputAttr = xmlDoc.CreateAttribute("outputDir");
			outputAttr.Value = this.OutputDirectory;
			elem.Attributes.Append(outputAttr);
			XmlAttribute baseAttr = xmlDoc.CreateAttribute("baseDir");
			baseAttr.Value = this.BaseDirectory;
			elem.Attributes.Append(baseAttr);
			bool flag = this.Seed != null;
			if (flag)
			{
				XmlAttribute seedAttr = xmlDoc.CreateAttribute("seed");
				seedAttr.Value = this.Seed;
				elem.Attributes.Append(seedAttr);
			}
			bool debug = this.Debug;
			if (debug)
			{
				XmlAttribute debugAttr = xmlDoc.CreateAttribute("debug");
				debugAttr.Value = this.Debug.ToString().ToLower();
				elem.Attributes.Append(debugAttr);
			}
			foreach (Rule i in this.Rules)
			{
				elem.AppendChild(i.Save(xmlDoc));
			}
			bool flag2 = this.Packer != null;
			if (flag2)
			{
				elem.AppendChild(this.Packer.Save(xmlDoc));
			}
			foreach (ProjectModule j in this)
			{
				elem.AppendChild(j.Save(xmlDoc));
			}
			foreach (string k in this.ProbePaths)
			{
				XmlElement path = xmlDoc.CreateElement("probePath", "http://confuser.codeplex.com");
				path.InnerText = k;
				elem.AppendChild(path);
			}
			foreach (string l in this.PluginPaths)
			{
				XmlElement path2 = xmlDoc.CreateElement("plugin", "http://confuser.codeplex.com");
				path2.InnerText = l;
				elem.AppendChild(path2);
			}
			xmlDoc.AppendChild(elem);
			return xmlDoc;
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x00003496 File Offset: 0x00001696
		// (set) Token: 0x060002F8 RID: 760 RVA: 0x0000349E File Offset: 0x0000169E
		public string BaseDirectory
		{
			[CompilerGenerated]
			get
			{
				return this.<BaseDirectory>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<BaseDirectory>k__BackingField = value;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x000034A7 File Offset: 0x000016A7
		// (set) Token: 0x060002FA RID: 762 RVA: 0x000034AF File Offset: 0x000016AF
		public bool Debug
		{
			[CompilerGenerated]
			get
			{
				return this.<Debug>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Debug>k__BackingField = value;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060002FB RID: 763 RVA: 0x000034B8 File Offset: 0x000016B8
		// (set) Token: 0x060002FC RID: 764 RVA: 0x000034C0 File Offset: 0x000016C0
		public string OutputDirectory
		{
			[CompilerGenerated]
			get
			{
				return this.<OutputDirectory>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<OutputDirectory>k__BackingField = value;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060002FD RID: 765 RVA: 0x000034C9 File Offset: 0x000016C9
		// (set) Token: 0x060002FE RID: 766 RVA: 0x000034D1 File Offset: 0x000016D1
		public SettingItem<Packer> Packer
		{
			[CompilerGenerated]
			get
			{
				return this.<Packer>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Packer>k__BackingField = value;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060002FF RID: 767 RVA: 0x000034DA File Offset: 0x000016DA
		// (set) Token: 0x06000300 RID: 768 RVA: 0x000034E2 File Offset: 0x000016E2
		public IList<string> PluginPaths
		{
			[CompilerGenerated]
			get
			{
				return this.<PluginPaths>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<PluginPaths>k__BackingField = value;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000301 RID: 769 RVA: 0x000034EB File Offset: 0x000016EB
		// (set) Token: 0x06000302 RID: 770 RVA: 0x000034F3 File Offset: 0x000016F3
		public IList<string> ProbePaths
		{
			[CompilerGenerated]
			get
			{
				return this.<ProbePaths>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ProbePaths>k__BackingField = value;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000303 RID: 771 RVA: 0x000034FC File Offset: 0x000016FC
		// (set) Token: 0x06000304 RID: 772 RVA: 0x00003504 File Offset: 0x00001704
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

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000305 RID: 773 RVA: 0x0000350D File Offset: 0x0000170D
		// (set) Token: 0x06000306 RID: 774 RVA: 0x00003515 File Offset: 0x00001715
		public string Seed
		{
			[CompilerGenerated]
			get
			{
				return this.<Seed>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Seed>k__BackingField = value;
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0000351E File Offset: 0x0000171E
		// Note: this type is marked as 'beforefieldinit'.
		static ConfuserProject()
		{
		}

		// Token: 0x04000233 RID: 563
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <BaseDirectory>k__BackingField;

		// Token: 0x04000234 RID: 564
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool <Debug>k__BackingField;

		// Token: 0x04000235 RID: 565
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <OutputDirectory>k__BackingField;

		// Token: 0x04000236 RID: 566
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private SettingItem<Packer> <Packer>k__BackingField;

		// Token: 0x04000237 RID: 567
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<string> <PluginPaths>k__BackingField;

		// Token: 0x04000238 RID: 568
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<string> <ProbePaths>k__BackingField;

		// Token: 0x04000239 RID: 569
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<Rule> <Rules>k__BackingField;

		// Token: 0x0400023A RID: 570
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <Seed>k__BackingField;

		// Token: 0x0400023B RID: 571
		public const string Namespace = "http://confuser.codeplex.com";

		// Token: 0x0400023C RID: 572
		public static readonly XmlSchema Schema = XmlSchema.Read(typeof(ConfuserProject).Assembly.GetManifestResourceStream("Confuser.Core.Project.ConfuserPrj.xsd"), null);

		// Token: 0x02000083 RID: 131
		[CompilerGenerated]
		private sealed class <>c__DisplayClass2_0
		{
			// Token: 0x06000308 RID: 776 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass2_0()
			{
			}

			// Token: 0x06000309 RID: 777 RVA: 0x00013C04 File Offset: 0x00011E04
			internal void <Load>b__0(object sender, ValidationEventArgs e)
			{
				bool flag = e.Severity > XmlSeverityType.Error;
				if (!flag)
				{
					this.exceptions.Add(e.Exception);
				}
			}

			// Token: 0x0400023D RID: 573
			public List<XmlSchemaException> exceptions;
		}
	}
}
