using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x0200007B RID: 123
	internal class CaliburnAnalyzer : IRenamer
	{
		// Token: 0x060002C9 RID: 713 RVA: 0x0000357E File Offset: 0x0000177E
		public CaliburnAnalyzer(WPFAnalyzer wpfAnalyzer)
		{
			wpfAnalyzer.AnalyzeBAMLElement += this.AnalyzeBAMLElement;
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00020EB8 File Offset: 0x0001F0B8
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			TypeDef type = def as TypeDef;
			bool flag = type == null || type.DeclaringType != null;
			if (!flag)
			{
				bool flag2 = type.Name.Contains("ViewModel");
				if (flag2)
				{
					string viewNs = type.Namespace.Replace("ViewModels", "Views");
					string viewName = type.Name.Replace("PageViewModel", "Page").Replace("ViewModel", "View");
					TypeDef view = type.Module.Find(viewNs + "." + viewName, true);
					bool flag3 = view != null;
					if (flag3)
					{
						service.SetCanRename(type, false);
						service.SetCanRename(view, false);
					}
					string multiViewNs = type.Namespace + "." + type.Name.Replace("ViewModel", "");
					foreach (TypeDef t in type.Module.Types)
					{
						bool flag4 = t.Namespace == multiViewNs;
						if (flag4)
						{
							service.SetCanRename(type, false);
							service.SetCanRename(t, false);
						}
					}
				}
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00021024 File Offset: 0x0001F224
		private void AnalyzeActionMessage(BAMLAnalyzer analyzer, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attr, string value)
		{
			bool flag = attr.Item2 == null;
			if (!flag)
			{
				TypeDef attrDeclType = analyzer.ResolveType(attr.Item2.OwnerTypeId);
				bool flag2 = attrDeclType.FullName != "Caliburn.Micro.ActionMessage";
				if (!flag2)
				{
					foreach (MethodDef method in analyzer.LookupMethod(value))
					{
						analyzer.NameService.SetCanRename(method, false);
					}
				}
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x000210BC File Offset: 0x0001F2BC
		private void AnalyzeAutoBind(BAMLAnalyzer analyzer, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attr, string value)
		{
			bool flag = !(attr.Item1 is PropertyDef) || ((PropertyDef)attr.Item1).DeclaringType.FullName != "System.Windows.FrameworkElement";
			if (!flag)
			{
				foreach (MethodDef method in analyzer.LookupMethod(value))
				{
					analyzer.NameService.SetCanRename(method, false);
				}
				foreach (PropertyDef method2 in analyzer.LookupProperty(value))
				{
					analyzer.NameService.SetCanRename(method2, false);
				}
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00021198 File Offset: 0x0001F398
		private void AnalyzeBAMLElement(BAMLAnalyzer analyzer, BamlElement elem)
		{
			foreach (BamlRecord rec in elem.Body)
			{
				PropertyWithConverterRecord prop = rec as PropertyWithConverterRecord;
				bool flag = prop != null;
				if (flag)
				{
					Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attr = analyzer.ResolveAttribute(prop.AttributeId);
					string attrName = null;
					bool flag2 = attr.Item2 != null;
					if (flag2)
					{
						attrName = attr.Item2.Name;
					}
					else
					{
						bool flag3 = attr.Item1 != null;
						if (flag3)
						{
							attrName = attr.Item1.Name;
						}
					}
					bool flag4 = attrName == "Attach";
					if (flag4)
					{
						this.AnalyzeMessageAttach(analyzer, attr, prop.Value);
					}
					bool flag5 = attrName == "Name";
					if (flag5)
					{
						this.AnalyzeAutoBind(analyzer, attr, prop.Value);
					}
					bool flag6 = attrName == "MethodName";
					if (flag6)
					{
						this.AnalyzeActionMessage(analyzer, attr, prop.Value);
					}
				}
			}
		}

		// Token: 0x060002CE RID: 718 RVA: 0x000212C4 File Offset: 0x0001F4C4
		private void AnalyzeMessageAttach(BAMLAnalyzer analyzer, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attr, string value)
		{
			bool flag = attr.Item2 == null;
			if (!flag)
			{
				TypeDef attrDeclType = analyzer.ResolveType(attr.Item2.OwnerTypeId);
				bool flag2 = attrDeclType.FullName != "Caliburn.Micro.Message";
				if (!flag2)
				{
					foreach (string msg in value.Split(new char[]
					{
						';'
					}))
					{
						bool flag3 = msg.Contains("=");
						string msgStr;
						if (flag3)
						{
							msgStr = msg.Split(new char[]
							{
								'='
							})[1].Trim(new char[]
							{
								'[',
								']',
								' '
							});
						}
						else
						{
							msgStr = msg.Trim(new char[]
							{
								'[',
								']',
								' '
							});
						}
						bool flag4 = msgStr.StartsWith("Action");
						if (flag4)
						{
							msgStr = msgStr.Substring(6);
						}
						int parenIndex = msgStr.IndexOf('(');
						bool flag5 = parenIndex != -1;
						if (flag5)
						{
							msgStr = msgStr.Substring(0, parenIndex);
						}
						string actName = msgStr.Trim();
						foreach (MethodDef method in analyzer.LookupMethod(actName))
						{
							analyzer.NameService.SetCanRename(method, false);
						}
					}
				}
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}
	}
}
