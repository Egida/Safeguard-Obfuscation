using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Confuser.Core.Project
{
	/// <summary>
	///     A rule that control how <see cref="T:Confuser.Core.Protection" />s are applied to module
	/// </summary>
	// Token: 0x0200008C RID: 140
	public class Rule : List<SettingItem<Protection>>
	{
		/// <summary>
		/// Initialize this rule instance
		/// </summary>
		/// <param name="pattern">The pattern</param>
		/// <param name="preset">The preset</param>
		/// <param name="inherit">Inherits protection</param>
		// Token: 0x0600034C RID: 844 RVA: 0x000037C0 File Offset: 0x000019C0
		public Rule(string pattern = "true", ProtectionPreset preset = ProtectionPreset.None, bool inherit = false)
		{
			this.Pattern = pattern;
			this.Preset = preset;
			this.Inherit = inherit;
		}

		/// <summary>
		///     Clones this instance.
		/// </summary>
		/// <returns>A duplicated rule.</returns>
		// Token: 0x0600034D RID: 845 RVA: 0x00014B8C File Offset: 0x00012D8C
		public Rule Clone()
		{
			Rule ret = new Rule("true", ProtectionPreset.None, false);
			ret.Preset = this.Preset;
			ret.Pattern = this.Pattern;
			ret.Inherit = this.Inherit;
			foreach (SettingItem<Protection> i in this)
			{
				SettingItem<Protection> item = new SettingItem<Protection>(null, SettingItemAction.Add);
				item.Id = i.Id;
				item.Action = i.Action;
				foreach (string j in i.Keys)
				{
					item.Add(j, i[j]);
				}
				ret.Add(item);
			}
			return ret;
		}

		/// <summary>
		///     Loads the rule description from XML element.
		/// </summary>
		/// <param name="elem">The serialized module description.</param>
		// Token: 0x0600034E RID: 846 RVA: 0x00014C94 File Offset: 0x00012E94
		internal void Load(XmlElement elem)
		{
			this.Pattern = elem.Attributes["pattern"].Value;
			bool flag = elem.Attributes["preset"] != null;
			if (flag)
			{
				this.Preset = (ProtectionPreset)Enum.Parse(typeof(ProtectionPreset), elem.Attributes["preset"].Value, true);
			}
			else
			{
				this.Preset = ProtectionPreset.None;
			}
			bool flag2 = elem.Attributes["inherit"] != null;
			if (flag2)
			{
				this.Inherit = bool.Parse(elem.Attributes["inherit"].Value);
			}
			else
			{
				this.Inherit = true;
			}
			base.Clear();
			foreach (XmlElement i in elem.ChildNodes.OfType<XmlElement>())
			{
				SettingItem<Protection> x = new SettingItem<Protection>(null, SettingItemAction.Add);
				x.Load(i);
				base.Add(x);
			}
		}

		/// <summary>
		///     Saves the rule description as XML element.
		/// </summary>
		/// <param name="xmlDoc">The root XML document.</param>
		/// <returns>The serialized rule description.</returns>
		// Token: 0x0600034F RID: 847 RVA: 0x00014DC0 File Offset: 0x00012FC0
		internal XmlElement Save(XmlDocument xmlDoc)
		{
			XmlElement elem = xmlDoc.CreateElement("rule", "http://confuser.codeplex.com");
			XmlAttribute ruleAttr = xmlDoc.CreateAttribute("pattern");
			ruleAttr.Value = this.Pattern;
			elem.Attributes.Append(ruleAttr);
			bool flag = this.Preset > ProtectionPreset.None;
			if (flag)
			{
				XmlAttribute pAttr = xmlDoc.CreateAttribute("preset");
				pAttr.Value = this.Preset.ToString().ToLower();
				elem.Attributes.Append(pAttr);
			}
			bool flag2 = !this.Inherit;
			if (flag2)
			{
				XmlAttribute attr = xmlDoc.CreateAttribute("inherit");
				attr.Value = this.Inherit.ToString().ToLower();
				elem.Attributes.Append(attr);
			}
			foreach (SettingItem<Protection> i in this)
			{
				elem.AppendChild(i.Save(xmlDoc));
			}
			return elem;
		}

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="T:Confuser.Core.Project.Rule" /> inherits settings from earlier rules.
		/// </summary>
		/// <value><c>true</c> if it inherits settings; otherwise, <c>false</c>.</value>
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000350 RID: 848 RVA: 0x000037E2 File Offset: 0x000019E2
		// (set) Token: 0x06000351 RID: 849 RVA: 0x000037EA File Offset: 0x000019EA
		public bool Inherit
		{
			[CompilerGenerated]
			get
			{
				return this.<Inherit>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Inherit>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets or sets the pattern that determine the target components of the rule.
		/// </summary>
		/// <value>The pattern expression.</value>
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000352 RID: 850 RVA: 0x000037F3 File Offset: 0x000019F3
		// (set) Token: 0x06000353 RID: 851 RVA: 0x000037FB File Offset: 0x000019FB
		public string Pattern
		{
			[CompilerGenerated]
			get
			{
				return this.<Pattern>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Pattern>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets or sets the protection preset this rule uses.
		/// </summary>
		/// <value>The protection preset.</value>
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000354 RID: 852 RVA: 0x00003804 File Offset: 0x00001A04
		// (set) Token: 0x06000355 RID: 853 RVA: 0x0000380C File Offset: 0x00001A0C
		public ProtectionPreset Preset
		{
			[CompilerGenerated]
			get
			{
				return this.<Preset>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Preset>k__BackingField = value;
			}
		}

		// Token: 0x04000254 RID: 596
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool <Inherit>k__BackingField;

		// Token: 0x04000255 RID: 597
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <Pattern>k__BackingField;

		// Token: 0x04000256 RID: 598
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ProtectionPreset <Preset>k__BackingField;
	}
}
