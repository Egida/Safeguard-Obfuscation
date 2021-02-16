using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Renamer.Analyzers;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000002 RID: 2
	internal class AnalyzePhase : ProtectionPhase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000020E0 File Offset: 0x000002E0
		public AnalyzePhase(NameProtection parent) : base(parent)
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00003690 File Offset: 0x00001890
		public override bool ProcessAll
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x000036A4 File Offset: 0x000018A4
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.AllDefinitions;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000036B8 File Offset: 0x000018B8
		public override string Name
		{
			get
			{
				return "Name analysis";
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000036D0 File Offset: 0x000018D0
		private void ParseParameters(IDnlibDef def, ConfuserContext context, NameService service, ProtectionParameters parameters)
		{
			RenameMode? mode = parameters.GetParameter<RenameMode?>(context, def, "mode", null);
			bool flag = mode != null;
			if (flag)
			{
				service.SetRenameMode(def, mode.Value);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00003714 File Offset: 0x00001914
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			NameService service = (NameService)context.Registry.GetService<INameService>();
			context.Logger.Debug("Building VTables & identifier list...");
			foreach (IDnlibDef def in parameters.Targets.WithProgress(context.Logger))
			{
				this.ParseParameters(def, context, service, parameters);
				bool flag = def is ModuleDef;
				if (flag)
				{
					ModuleDef module = (ModuleDef)def;
					using (IEnumerator<Resource> enumerator2 = module.Resources.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Resource res = enumerator2.Current;
							service.SetOriginalName(res, res.Name);
						}
						goto IL_BA;
					}
					goto IL_B8;
				}
				goto IL_B8;
				IL_BA:
				bool flag2 = def is TypeDef;
				if (flag2)
				{
					service.GetVTables().GetVTable((TypeDef)def);
					service.SetOriginalNamespace(def, ((TypeDef)def).Namespace);
				}
				context.CheckCancellation();
				continue;
				IL_B8:
				service.SetOriginalName(def, def.Name);
				goto IL_BA;
			}
			context.Logger.Debug("Analyzing...");
			this.RegisterRenamers(context, service);
			IList<IRenamer> arg_120_0 = service.Renamers;
			foreach (IDnlibDef def2 in parameters.Targets.WithProgress(context.Logger))
			{
				this.Analyze(service, context, parameters, def2, true);
				context.CheckCancellation();
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000038EC File Offset: 0x00001AEC
		private void RegisterRenamers(ConfuserContext context, NameService service)
		{
			bool wpf = false;
			bool caliburn = false;
			bool winforms = false;
			foreach (ModuleDefMD module in context.Modules)
			{
				foreach (AssemblyRef asmRef in module.GetAssemblyRefs())
				{
					bool flag = asmRef.Name == "WindowsBase" || asmRef.Name == "PresentationCore" || asmRef.Name == "PresentationFramework" || asmRef.Name == "System.Xaml";
					if (flag)
					{
						wpf = true;
					}
					else
					{
						bool flag2 = asmRef.Name == "Caliburn.Micro";
						if (flag2)
						{
							caliburn = true;
						}
						else
						{
							bool flag3 = asmRef.Name == "System.Windows.Forms";
							if (flag3)
							{
								winforms = true;
							}
						}
					}
				}
			}
			bool flag4 = wpf;
			if (flag4)
			{
				WPFAnalyzer wpfAnalyzer = new WPFAnalyzer();
				context.Logger.Debug("WPF found, enabling compatibility.");
				service.Renamers.Add(wpfAnalyzer);
				bool flag5 = caliburn;
				if (flag5)
				{
					context.Logger.Debug("Caliburn.Micro found, enabling compatibility.");
					service.Renamers.Add(new CaliburnAnalyzer(wpfAnalyzer));
				}
			}
			bool flag6 = winforms;
			if (flag6)
			{
				WinFormsAnalyzer winformsAnalyzer = new WinFormsAnalyzer();
				context.Logger.Debug("WinForms found, enabling compatibility.");
				service.Renamers.Add(winformsAnalyzer);
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00003AA8 File Offset: 0x00001CA8
		internal void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, IDnlibDef def, bool runAnalyzer)
		{
			bool flag = def is TypeDef;
			if (flag)
			{
				this.Analyze(service, context, parameters, (TypeDef)def);
			}
			else
			{
				bool flag2 = def is MethodDef;
				if (flag2)
				{
					this.Analyze(service, context, parameters, (MethodDef)def);
				}
				else
				{
					bool flag3 = def is FieldDef;
					if (flag3)
					{
						this.Analyze(service, context, parameters, (FieldDef)def);
					}
					else
					{
						bool flag4 = def is PropertyDef;
						if (flag4)
						{
							this.Analyze(service, context, parameters, (PropertyDef)def);
						}
						else
						{
							bool flag5 = def is EventDef;
							if (flag5)
							{
								this.Analyze(service, context, parameters, (EventDef)def);
							}
							else
							{
								bool flag6 = def is ModuleDef;
								if (flag6)
								{
									string pass = parameters.GetParameter<string>(context, def, "password", null);
									bool flag7 = pass != null;
									if (flag7)
									{
										service.reversibleRenamer = new ReversibleRenamer(pass);
									}
									service.SetCanRename(def, false);
								}
							}
						}
					}
				}
			}
			bool flag8 = !runAnalyzer || parameters.GetParameter<bool>(context, def, "forceRen", false);
			if (!flag8)
			{
				foreach (IRenamer renamer in service.Renamers)
				{
					renamer.Analyze(context, service, parameters, def);
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00003C28 File Offset: 0x00001E28
		private static bool IsVisibleOutside(ConfuserContext context, ProtectionParameters parameters, IMemberDef def)
		{
			TypeDef type = def as TypeDef;
			bool flag = type == null;
			if (flag)
			{
				type = def.DeclaringType;
			}
			bool? renPublic = parameters.GetParameter<bool?>(context, type, "renPublic", null);
			bool flag2 = renPublic == null;
			bool result;
			if (flag2)
			{
				result = type.IsVisibleOutside(true);
			}
			else
			{
				result = (type.IsVisibleOutside(false) && renPublic.Value);
			}
			return result;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00003C9C File Offset: 0x00001E9C
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, TypeDef type)
		{
			bool flag = AnalyzePhase.IsVisibleOutside(context, parameters, type);
			if (flag)
			{
				service.SetCanRename(type, false);
			}
			else
			{
				bool flag2 = type.IsRuntimeSpecialName || type.IsGlobalModuleType;
				if (flag2)
				{
					service.SetCanRename(type, false);
				}
				else
				{
					bool flag3 = type.FullName == "ConfusedBy";
					if (flag3)
					{
						service.SetCanRename(type, false);
					}
				}
			}
			bool parameter = parameters.GetParameter<bool>(context, type, "forceRen", false);
			if (!parameter)
			{
				bool flag4 = type.InheritsFromCorlib("System.Attribute");
				if (flag4)
				{
					service.ReduceRenameMode(type, RenameMode.ASCII);
				}
				bool flag5 = type.InheritsFrom("System.Configuration.SettingsBase");
				if (flag5)
				{
					service.SetCanRename(type, false);
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00003D60 File Offset: 0x00001F60
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, MethodDef method)
		{
			bool flag = method.DeclaringType.IsVisibleOutside(true) && (method.IsFamily || method.IsFamilyOrAssembly || method.IsPublic) && AnalyzePhase.IsVisibleOutside(context, parameters, method);
			if (flag)
			{
				service.SetCanRename(method, false);
			}
			else
			{
				bool isRuntimeSpecialName = method.IsRuntimeSpecialName;
				if (isRuntimeSpecialName)
				{
					service.SetCanRename(method, false);
				}
				else
				{
					bool parameter = parameters.GetParameter<bool>(context, method, "forceRen", false);
					if (!parameter)
					{
						bool flag2 = method.DeclaringType.IsComImport() && !method.HasAttribute("System.Runtime.InteropServices.DispIdAttribute");
						if (flag2)
						{
							service.SetCanRename(method, false);
						}
						else
						{
							bool flag3 = method.DeclaringType.IsDelegate();
							if (flag3)
							{
								service.SetCanRename(method, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00003E34 File Offset: 0x00002034
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, FieldDef field)
		{
			bool flag = field.DeclaringType.IsVisibleOutside(true) && (field.IsFamily || field.IsFamilyOrAssembly || field.IsPublic) && AnalyzePhase.IsVisibleOutside(context, parameters, field);
			if (flag)
			{
				service.SetCanRename(field, false);
			}
			else
			{
				bool isRuntimeSpecialName = field.IsRuntimeSpecialName;
				if (isRuntimeSpecialName)
				{
					service.SetCanRename(field, false);
				}
				else
				{
					bool parameter = parameters.GetParameter<bool>(context, field, "forceRen", false);
					if (!parameter)
					{
						bool flag2 = field.DeclaringType.IsSerializable && !field.IsNotSerialized;
						if (flag2)
						{
							service.SetCanRename(field, false);
						}
						else
						{
							bool flag3 = field.IsLiteral && field.DeclaringType.IsEnum;
							if (flag3)
							{
								service.SetCanRename(field, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00003F10 File Offset: 0x00002110
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, PropertyDef property)
		{
			bool flag = property.DeclaringType.IsVisibleOutside(true) && AnalyzePhase.IsVisibleOutside(context, parameters, property);
			if (flag)
			{
				service.SetCanRename(property, false);
			}
			else
			{
				bool isRuntimeSpecialName = property.IsRuntimeSpecialName;
				if (isRuntimeSpecialName)
				{
					service.SetCanRename(property, false);
				}
				else
				{
					bool parameter = parameters.GetParameter<bool>(context, property, "forceRen", false);
					if (!parameter)
					{
						bool flag2 = property.DeclaringType.Implements("System.ComponentModel.INotifyPropertyChanged");
						if (flag2)
						{
							service.SetCanRename(property, false);
						}
						else
						{
							bool flag3 = property.DeclaringType.Name.String.Contains("AnonymousType");
							if (flag3)
							{
								service.SetCanRename(property, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00003FC8 File Offset: 0x000021C8
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, EventDef evt)
		{
			bool flag = evt.DeclaringType.IsVisibleOutside(true) && AnalyzePhase.IsVisibleOutside(context, parameters, evt);
			if (flag)
			{
				service.SetCanRename(evt, false);
			}
			else
			{
				bool isRuntimeSpecialName = evt.IsRuntimeSpecialName;
				if (isRuntimeSpecialName)
				{
					service.SetCanRename(evt, false);
				}
			}
		}
	}
}
