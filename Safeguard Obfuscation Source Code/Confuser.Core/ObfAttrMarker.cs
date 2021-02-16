using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Confuser.Core.Project;
using Confuser.Core.Project.Patterns;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core
{
	// Token: 0x02000051 RID: 81
	public class ObfAttrMarker : Marker
	{
		// Token: 0x060001EA RID: 490 RVA: 0x0000EC94 File Offset: 0x0000CE94
		private static IEnumerable<ObfAttrMarker.ObfuscationAttributeInfo> ReadObfuscationAttributes(IHasCustomAttribute item)
		{
			List<Tuple<int?, ObfAttrMarker.ObfuscationAttributeInfo>> ret = new List<Tuple<int?, ObfAttrMarker.ObfuscationAttributeInfo>>();
			for (int i = item.CustomAttributes.Count - 1; i >= 0; i--)
			{
				CustomAttribute ca = item.CustomAttributes[i];
				bool flag = ca.TypeFullName != "System.Reflection.ObfuscationAttribute";
				if (!flag)
				{
					ObfAttrMarker.ObfuscationAttributeInfo info = default(ObfAttrMarker.ObfuscationAttributeInfo);
					int? order = null;
					info.Owner = item;
					bool strip = true;
					foreach (CANamedArgument prop in ca.Properties)
					{
						string a = prop.Name;
						if (!(a == "ApplyToMembers"))
						{
							if (!(a == "Exclude"))
							{
								if (!(a == "StripAfterObfuscation"))
								{
									if (!(a == "Feature"))
									{
										throw new NotSupportedException("Unsupported property: " + prop.Name);
									}
									Debug.Assert(prop.Type.ElementType == ElementType.String);
									string feature = (UTF8String)prop.Value;
									Match match = ObfAttrMarker.OrderPattern.Match(feature);
									bool success = match.Success;
									if (success)
									{
										string orderStr = match.Groups[1].Value;
										string f = match.Groups[2].Value;
										int o;
										bool flag2 = !int.TryParse(orderStr, out o);
										if (flag2)
										{
											throw new NotSupportedException(string.Format("Failed to parse feature '{0}' in {1} ", feature, item));
										}
										order = new int?(o);
										feature = f;
									}
									int sepIndex = feature.IndexOf(':');
									bool flag3 = sepIndex == -1;
									if (flag3)
									{
										info.FeatureName = "";
										info.FeatureValue = feature;
									}
									else
									{
										info.FeatureName = feature.Substring(0, sepIndex);
										info.FeatureValue = feature.Substring(sepIndex + 1);
									}
								}
								else
								{
									Debug.Assert(prop.Type.ElementType == ElementType.Boolean);
									strip = (bool)prop.Value;
								}
							}
							else
							{
								Debug.Assert(prop.Type.ElementType == ElementType.Boolean);
								info.Exclude = new bool?((bool)prop.Value);
							}
						}
						else
						{
							Debug.Assert(prop.Type.ElementType == ElementType.Boolean);
							info.ApplyToMembers = new bool?((bool)prop.Value);
						}
					}
					bool flag4 = strip;
					if (flag4)
					{
						item.CustomAttributes.RemoveAt(i);
					}
					ret.Add(Tuple.Create<int?, ObfAttrMarker.ObfuscationAttributeInfo>(order, info));
				}
			}
			ret.Reverse();
			return from pair in ret
			orderby pair.Item1
			select pair.Item2;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000EFD0 File Offset: 0x0000D1D0
		private bool ToInfo(ObfAttrMarker.ObfuscationAttributeInfo attr, out ObfAttrMarker.ProtectionSettingsInfo info)
		{
			info = default(ObfAttrMarker.ProtectionSettingsInfo);
			info.Condition = null;
			info.Exclude = (attr.Exclude ?? true);
			info.ApplyToMember = (attr.ApplyToMembers ?? true);
			info.Settings = attr.FeatureValue;
			bool ok = true;
			try
			{
				new ObfAttrParser(this.protections).ParseProtectionString(null, info.Settings);
			}
			catch
			{
				ok = false;
			}
			bool flag = !ok;
			bool result;
			if (flag)
			{
				this.context.Logger.WarnFormat("Ignoring rule '{0}' in {1}.", new object[]
				{
					info.Settings,
					attr.Owner
				});
				result = false;
			}
			else
			{
				bool flag2 = !string.IsNullOrEmpty(attr.FeatureName);
				if (flag2)
				{
					throw new ArgumentException("Feature name must not be set. Owner=" + attr.Owner);
				}
				bool flag3 = info.Exclude && (!string.IsNullOrEmpty(attr.FeatureName) || !string.IsNullOrEmpty(attr.FeatureValue));
				if (flag3)
				{
					throw new ArgumentException("Feature property cannot be set when Exclude is true. Owner=" + attr.Owner);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000F124 File Offset: 0x0000D324
		private ObfAttrMarker.ProtectionSettingsInfo ToInfo(Rule rule, PatternExpression expr)
		{
			ObfAttrMarker.ProtectionSettingsInfo info = default(ObfAttrMarker.ProtectionSettingsInfo);
			info.Condition = expr;
			info.Exclude = false;
			info.ApplyToMember = true;
			StringBuilder settings = new StringBuilder();
			bool flag = rule.Preset > ProtectionPreset.None;
			if (flag)
			{
				settings.AppendFormat("preset({0});", rule.Preset.ToString().ToLowerInvariant());
			}
			foreach (SettingItem<Protection> item in rule)
			{
				settings.Append((item.Action == SettingItemAction.Add) ? '+' : '-');
				settings.Append(item.Id);
				bool flag2 = item.Count > 0;
				if (flag2)
				{
					settings.Append('(');
					int i = 0;
					foreach (KeyValuePair<string, string> arg in item)
					{
						bool flag3 = i != 0;
						if (flag3)
						{
							settings.Append(',');
						}
						settings.AppendFormat("{0}='{1}'", arg.Key, arg.Value.Replace("'", "\\'"));
						i++;
					}
					settings.Append(')');
				}
				settings.Append(';');
			}
			info.Settings = settings.ToString();
			return info;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00002CE6 File Offset: 0x00000EE6
		private IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> ReadInfos(IHasCustomAttribute item)
		{
			foreach (ObfAttrMarker.ObfuscationAttributeInfo attr in ObfAttrMarker.ReadObfuscationAttributes(item))
			{
				bool flag = !string.IsNullOrEmpty(attr.FeatureName);
				ObfAttrMarker.ProtectionSettingsInfo info;
				if (flag)
				{
					yield return this.AddRule(attr, null);
				}
				else
				{
					bool flag2 = this.ToInfo(attr, out info);
					if (flag2)
					{
						yield return info;
					}
				}
				info = default(ObfAttrMarker.ProtectionSettingsInfo);
				attr = default(ObfAttrMarker.ObfuscationAttributeInfo);
			}
			IEnumerator<ObfAttrMarker.ObfuscationAttributeInfo> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000F2BC File Offset: 0x0000D4BC
		protected internal override void MarkMember(IDnlibDef member, ConfuserContext context)
		{
			ModuleDef module = ((IMemberRef)member).Module;
			ObfAttrMarker.ProtectionSettingsStack stack = context.Annotations.Get<ObfAttrMarker.ProtectionSettingsStack>(module, ObfAttrMarker.ModuleSettingsKey, null);
			using (stack.Apply(member, Enumerable.Empty<ObfAttrMarker.ProtectionSettingsInfo>()))
			{
			}
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000F314 File Offset: 0x0000D514
		protected internal override MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
		{
			this.context = context;
			this.project = proj;
			this.extModules = new List<byte[]>();
			bool flag = proj.Packer != null;
			if (flag)
			{
				bool flag2 = !this.packers.ContainsKey(proj.Packer.Id);
				if (flag2)
				{
					context.Logger.ErrorFormat("Cannot find packer with ID '{0}'.", new object[]
					{
						proj.Packer.Id
					});
					throw new ConfuserException(null);
				}
				this.packer = this.packers[proj.Packer.Id];
				this.packerParams = new Dictionary<string, string>(proj.Packer, StringComparer.OrdinalIgnoreCase);
			}
			List<Tuple<ProjectModule, ModuleDefMD>> modules = new List<Tuple<ProjectModule, ModuleDefMD>>();
			foreach (ProjectModule module3 in proj)
			{
				bool isExternal = module3.IsExternal;
				if (isExternal)
				{
					this.extModules.Add(module3.LoadRaw(proj.BaseDirectory));
				}
				else
				{
					ModuleDefMD modDef = module3.Resolve(proj.BaseDirectory, context.Resolver.DefaultModuleContext);
					context.CheckCancellation();
					context.Resolver.AddToCache(modDef);
					modules.Add(Tuple.Create<ProjectModule, ModuleDefMD>(module3, modDef));
				}
			}
			foreach (Tuple<ProjectModule, ModuleDefMD> module2 in modules)
			{
				context.Logger.InfoFormat("Loading '{0}'...", new object[]
				{
					module2.Item1.Path
				});
				Dictionary<Rule, PatternExpression> rules = base.ParseRules(proj, module2.Item1, context);
				this.MarkModule(module2.Item1, module2.Item2, rules, module2 == modules[0]);
				context.Annotations.Set<Dictionary<Rule, PatternExpression>>(module2.Item2, Marker.RulesKey, rules);
				bool flag3 = this.packer != null;
				if (flag3)
				{
					ProtectionParameters.GetParameters(context, module2.Item2)[this.packer] = this.packerParams;
				}
			}
			bool flag4 = proj.Debug && proj.Packer != null;
			if (flag4)
			{
				context.Logger.Warn("Generated Debug symbols might not be usable with packers!");
			}
			return new MarkerResult((from module in modules
			select module.Item2).ToList<ModuleDefMD>(), this.packer, this.extModules);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000F5C4 File Offset: 0x0000D7C4
		private ObfAttrMarker.ProtectionSettingsInfo AddRule(ObfAttrMarker.ObfuscationAttributeInfo attr, List<ObfAttrMarker.ProtectionSettingsInfo> infos)
		{
			Debug.Assert(attr.FeatureName != null);
			string pattern = attr.FeatureName;
			PatternExpression expr;
			try
			{
				expr = new PatternParser().Parse(pattern);
			}
			catch (Exception ex)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Error when parsing pattern ",
					pattern,
					" in ObfuscationAttribute. Owner=",
					attr.Owner
				}), ex);
			}
			ObfAttrMarker.ProtectionSettingsInfo info = default(ObfAttrMarker.ProtectionSettingsInfo);
			info.Condition = expr;
			info.Exclude = (attr.Exclude ?? true);
			info.ApplyToMember = (attr.ApplyToMembers ?? true);
			info.Settings = attr.FeatureValue;
			bool ok = true;
			try
			{
				new ObfAttrParser(this.protections).ParseProtectionString(null, info.Settings);
			}
			catch
			{
				ok = false;
			}
			bool flag = !ok;
			if (flag)
			{
				this.context.Logger.WarnFormat("Ignoring rule '{0}' in {1}.", new object[]
				{
					info.Settings,
					attr.Owner
				});
			}
			else
			{
				bool flag2 = infos != null;
				if (flag2)
				{
					infos.Add(info);
				}
			}
			return info;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000F724 File Offset: 0x0000D924
		private void MarkModule(ProjectModule projModule, ModuleDefMD module, Dictionary<Rule, PatternExpression> rules, bool isMain)
		{
			string snKeyPath = projModule.SNKeyPath;
			string snKeyPass = projModule.SNKeyPassword;
			ObfAttrMarker.ProtectionSettingsStack stack = new ObfAttrMarker.ProtectionSettingsStack(this.context, this.protections);
			List<ObfAttrMarker.ProtectionSettingsInfo> layer = new List<ObfAttrMarker.ProtectionSettingsInfo>();
			foreach (KeyValuePair<Rule, PatternExpression> rule in rules)
			{
				layer.Add(this.ToInfo(rule.Key, rule.Value));
			}
			foreach (ObfAttrMarker.ObfuscationAttributeInfo attr in ObfAttrMarker.ReadObfuscationAttributes(module.Assembly))
			{
				bool flag = string.IsNullOrEmpty(attr.FeatureName);
				if (flag)
				{
					ObfAttrMarker.ProtectionSettingsInfo info;
					bool flag2 = this.ToInfo(attr, out info);
					if (flag2)
					{
						layer.Add(info);
					}
				}
				else
				{
					bool flag3 = attr.FeatureName.Equals("generate debug symbol", StringComparison.OrdinalIgnoreCase);
					if (flag3)
					{
						bool flag4 = !isMain;
						if (flag4)
						{
							throw new ArgumentException("Only main module can set 'generate debug symbol'.");
						}
						this.project.Debug = bool.Parse(attr.FeatureValue);
					}
					else
					{
						bool flag5 = attr.FeatureName.Equals("random seed", StringComparison.OrdinalIgnoreCase);
						if (flag5)
						{
							bool flag6 = !isMain;
							if (flag6)
							{
								throw new ArgumentException("Only main module can set 'random seed'.");
							}
							this.project.Seed = attr.FeatureValue;
						}
						else
						{
							bool flag7 = attr.FeatureName.Equals("strong name key", StringComparison.OrdinalIgnoreCase);
							if (flag7)
							{
								snKeyPath = Path.Combine(this.project.BaseDirectory, attr.FeatureValue);
							}
							else
							{
								bool flag8 = attr.FeatureName.Equals("strong name key password", StringComparison.OrdinalIgnoreCase);
								if (flag8)
								{
									snKeyPass = attr.FeatureValue;
								}
								else
								{
									bool flag9 = attr.FeatureName.Equals("packer", StringComparison.OrdinalIgnoreCase);
									if (flag9)
									{
										bool flag10 = !isMain;
										if (flag10)
										{
											throw new ArgumentException("Only main module can set 'packer'.");
										}
										new ObfAttrParser(this.packers).ParsePackerString(attr.FeatureValue, out this.packer, out this.packerParams);
									}
									else
									{
										bool flag11 = attr.FeatureName.Equals("external module", StringComparison.OrdinalIgnoreCase);
										if (flag11)
										{
											bool flag12 = !isMain;
											if (flag12)
											{
												throw new ArgumentException("Only main module can add external modules.");
											}
											byte[] rawModule = new ProjectModule
											{
												Path = attr.FeatureValue
											}.LoadRaw(this.project.BaseDirectory);
											this.extModules.Add(rawModule);
										}
										else
										{
											this.AddRule(attr, layer);
										}
									}
								}
							}
						}
					}
				}
			}
			bool debug = this.project.Debug;
			if (debug)
			{
				module.LoadPdb();
			}
			snKeyPath = ((snKeyPath == null) ? null : Path.Combine(this.project.BaseDirectory, snKeyPath));
			StrongNameKey snKey = Marker.LoadSNKey(this.context, snKeyPath, snKeyPass);
			this.context.Annotations.Set<StrongNameKey>(module, Marker.SNKey, snKey);
			using (stack.Apply(module, layer))
			{
				this.ProcessModule(module, stack);
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000FA98 File Offset: 0x0000DC98
		private void ProcessModule(ModuleDefMD module, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			this.context.Annotations.Set<ObfAttrMarker.ProtectionSettingsStack>(module, ObfAttrMarker.ModuleSettingsKey, new ObfAttrMarker.ProtectionSettingsStack(stack));
			foreach (TypeDef type in module.Types)
			{
				using (stack.Apply(type, this.ReadInfos(type)))
				{
					this.ProcessTypeMembers(type, stack);
				}
			}
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000FB34 File Offset: 0x0000DD34
		private void ProcessTypeMembers(TypeDef type, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			foreach (TypeDef nestedType in type.NestedTypes)
			{
				using (stack.Apply(nestedType, this.ReadInfos(nestedType)))
				{
					this.ProcessTypeMembers(nestedType, stack);
				}
			}
			foreach (PropertyDef property in type.Properties)
			{
				using (stack.Apply(property, this.ReadInfos(property)))
				{
					bool flag = property.GetMethod != null;
					if (flag)
					{
						this.ProcessMember(property.GetMethod, stack);
					}
					bool flag2 = property.SetMethod != null;
					if (flag2)
					{
						this.ProcessMember(property.SetMethod, stack);
					}
					foreach (MethodDef i in property.OtherMethods)
					{
						this.ProcessMember(i, stack);
					}
				}
			}
			foreach (EventDef evt in type.Events)
			{
				using (stack.Apply(evt, this.ReadInfos(evt)))
				{
					bool flag3 = evt.AddMethod != null;
					if (flag3)
					{
						this.ProcessMember(evt.AddMethod, stack);
					}
					bool flag4 = evt.RemoveMethod != null;
					if (flag4)
					{
						this.ProcessMember(evt.RemoveMethod, stack);
					}
					bool flag5 = evt.InvokeMethod != null;
					if (flag5)
					{
						this.ProcessMember(evt.InvokeMethod, stack);
					}
					foreach (MethodDef j in evt.OtherMethods)
					{
						this.ProcessMember(j, stack);
					}
				}
			}
			foreach (MethodDef method in type.Methods)
			{
				bool flag6 = method.SemanticsAttributes == MethodSemanticsAttributes.None;
				if (flag6)
				{
					this.ProcessMember(method, stack);
				}
			}
			foreach (FieldDef field in type.Fields)
			{
				this.ProcessMember(field, stack);
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000FE60 File Offset: 0x0000E060
		private void ProcessMember(IDnlibDef member, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			using (stack.Apply(member, this.ReadInfos(member)))
			{
				this.ProcessBody(member as MethodDef, stack);
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000FEA8 File Offset: 0x0000E0A8
		private void ProcessBody(MethodDef method, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			bool flag = method == null || method.Body == null;
			if (!flag)
			{
				TypeDef declType = method.DeclaringType;
				foreach (Instruction instr in method.Body.Instructions)
				{
					bool flag2 = instr.Operand is MethodDef;
					if (flag2)
					{
						TypeDef cgType = ((MethodDef)instr.Operand).DeclaringType;
						bool flag3 = cgType.DeclaringType == declType && cgType.IsCompilerGenerated();
						if (flag3)
						{
							using (stack.Apply(cgType, this.ReadInfos(cgType)))
							{
								this.ProcessTypeMembers(cgType, stack);
							}
						}
					}
				}
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00002CFD File Offset: 0x00000EFD
		public ObfAttrMarker()
		{
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00002D06 File Offset: 0x00000F06
		// Note: this type is marked as 'beforefieldinit'.
		static ObfAttrMarker()
		{
		}

		// Token: 0x04000191 RID: 401
		private static readonly Regex OrderPattern = new Regex("^(\\d+)\\. (.+)$");

		// Token: 0x04000192 RID: 402
		private ConfuserContext context;

		// Token: 0x04000193 RID: 403
		private ConfuserProject project;

		// Token: 0x04000194 RID: 404
		private Packer packer;

		// Token: 0x04000195 RID: 405
		private Dictionary<string, string> packerParams;

		// Token: 0x04000196 RID: 406
		private List<byte[]> extModules;

		// Token: 0x04000197 RID: 407
		private static readonly object ModuleSettingsKey = new object();

		// Token: 0x02000052 RID: 82
		private struct ObfuscationAttributeInfo
		{
			// Token: 0x04000198 RID: 408
			public IHasCustomAttribute Owner;

			// Token: 0x04000199 RID: 409
			public bool? ApplyToMembers;

			// Token: 0x0400019A RID: 410
			public bool? Exclude;

			// Token: 0x0400019B RID: 411
			public string FeatureName;

			// Token: 0x0400019C RID: 412
			public string FeatureValue;
		}

		// Token: 0x02000053 RID: 83
		private struct ProtectionSettingsInfo
		{
			// Token: 0x0400019D RID: 413
			public bool ApplyToMember;

			// Token: 0x0400019E RID: 414
			public bool Exclude;

			// Token: 0x0400019F RID: 415
			public PatternExpression Condition;

			// Token: 0x040001A0 RID: 416
			public string Settings;
		}

		// Token: 0x02000054 RID: 84
		private class ProtectionSettingsStack
		{
			// Token: 0x060001F8 RID: 504 RVA: 0x00002D21 File Offset: 0x00000F21
			public ProtectionSettingsStack(ConfuserContext context, Dictionary<string, Protection> protections)
			{
				this.context = context;
				this.stack = new Stack<Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>>();
				this.parser = new ObfAttrParser(protections);
			}

			// Token: 0x060001F9 RID: 505 RVA: 0x00002D49 File Offset: 0x00000F49
			public ProtectionSettingsStack(ObfAttrMarker.ProtectionSettingsStack copy)
			{
				this.context = copy.context;
				this.stack = new Stack<Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>>(copy.stack);
				this.parser = copy.parser;
			}

			// Token: 0x060001FA RID: 506 RVA: 0x00002D7C File Offset: 0x00000F7C
			private void Pop()
			{
				this.settings = this.stack.Pop().Item1;
			}

			// Token: 0x060001FB RID: 507 RVA: 0x0000FF90 File Offset: 0x0000E190
			public IDisposable Apply(IDnlibDef target, IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> infos)
			{
				bool flag = this.settings == null;
				ProtectionSettings settings;
				if (flag)
				{
					settings = new ProtectionSettings();
				}
				else
				{
					settings = new ProtectionSettings(this.settings);
				}
				ObfAttrMarker.ProtectionSettingsInfo[] infoArray = infos.ToArray<ObfAttrMarker.ProtectionSettingsInfo>();
				bool flag2 = this.stack.Count > 0;
				if (flag2)
				{
					foreach (Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]> i in this.stack.Reverse<Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>>())
					{
						this.ApplyInfo(target, settings, i.Item2, ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.ParentInfo);
					}
				}
				bool flag3 = infoArray.Length != 0;
				IDisposable result;
				if (flag3)
				{
					ProtectionSettings originalSettings = this.settings;
					this.ApplyInfo(target, settings, infoArray, ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoInherits);
					this.settings = new ProtectionSettings(settings);
					this.ApplyInfo(target, settings, infoArray, ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoOnly);
					this.stack.Push(Tuple.Create<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>(originalSettings, infoArray));
					result = new ObfAttrMarker.ProtectionSettingsStack.PopHolder(this);
				}
				else
				{
					result = null;
				}
				ProtectionParameters.SetParameters(this.context, target, settings);
				return result;
			}

			// Token: 0x060001FC RID: 508 RVA: 0x000100A0 File Offset: 0x0000E2A0
			private void ApplyInfo(IDnlibDef context, ProtectionSettings settings, IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> infos, ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType type)
			{
				foreach (ObfAttrMarker.ProtectionSettingsInfo info in infos)
				{
					bool flag = info.Condition != null && !(bool)info.Condition.Evaluate(context);
					if (!flag)
					{
						bool flag2 = info.Condition == null && info.Exclude;
						if (flag2)
						{
							bool flag3 = type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoOnly || (type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoInherits && info.ApplyToMember);
							if (flag3)
							{
								settings.Clear();
							}
						}
						bool flag4 = !string.IsNullOrEmpty(info.Settings);
						if (flag4)
						{
							bool flag5 = (type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.ParentInfo && info.Condition != null && info.ApplyToMember) || type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoOnly || (type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoInherits && info.Condition == null && info.ApplyToMember);
							if (flag5)
							{
								this.parser.ParseProtectionString(settings, info.Settings);
							}
						}
					}
				}
			}

			// Token: 0x040001A1 RID: 417
			private readonly ConfuserContext context;

			// Token: 0x040001A2 RID: 418
			private readonly Stack<Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>> stack;

			// Token: 0x040001A3 RID: 419
			private readonly ObfAttrParser parser;

			// Token: 0x040001A4 RID: 420
			private ProtectionSettings settings;

			// Token: 0x02000055 RID: 85
			private enum ApplyInfoType
			{
				// Token: 0x040001A6 RID: 422
				CurrentInfoOnly,
				// Token: 0x040001A7 RID: 423
				CurrentInfoInherits,
				// Token: 0x040001A8 RID: 424
				ParentInfo
			}

			// Token: 0x02000056 RID: 86
			private class PopHolder : IDisposable
			{
				// Token: 0x060001FD RID: 509 RVA: 0x00002D95 File Offset: 0x00000F95
				public PopHolder(ObfAttrMarker.ProtectionSettingsStack parent)
				{
					this.parent = parent;
				}

				// Token: 0x060001FE RID: 510 RVA: 0x00002DA6 File Offset: 0x00000FA6
				public void Dispose()
				{
					this.parent.Pop();
				}

				// Token: 0x040001A9 RID: 425
				private ObfAttrMarker.ProtectionSettingsStack parent;
			}
		}

		// Token: 0x02000057 RID: 87
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060001FF RID: 511 RVA: 0x00002DB5 File Offset: 0x00000FB5
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000200 RID: 512 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x06000201 RID: 513 RVA: 0x00002DC1 File Offset: 0x00000FC1
			internal int? <ReadObfuscationAttributes>b__4_0(Tuple<int?, ObfAttrMarker.ObfuscationAttributeInfo> pair)
			{
				return pair.Item1;
			}

			// Token: 0x06000202 RID: 514 RVA: 0x00002DC9 File Offset: 0x00000FC9
			internal ObfAttrMarker.ObfuscationAttributeInfo <ReadObfuscationAttributes>b__4_1(Tuple<int?, ObfAttrMarker.ObfuscationAttributeInfo> pair)
			{
				return pair.Item2;
			}

			// Token: 0x06000203 RID: 515 RVA: 0x00002C4E File Offset: 0x00000E4E
			internal ModuleDefMD <MarkProject>b__15_0(Tuple<ProjectModule, ModuleDefMD> module)
			{
				return module.Item2;
			}

			// Token: 0x040001AA RID: 426
			public static readonly ObfAttrMarker.<>c <>9 = new ObfAttrMarker.<>c();

			// Token: 0x040001AB RID: 427
			public static Func<Tuple<int?, ObfAttrMarker.ObfuscationAttributeInfo>, int?> <>9__4_0;

			// Token: 0x040001AC RID: 428
			public static Func<Tuple<int?, ObfAttrMarker.ObfuscationAttributeInfo>, ObfAttrMarker.ObfuscationAttributeInfo> <>9__4_1;

			// Token: 0x040001AD RID: 429
			public static Func<Tuple<ProjectModule, ModuleDefMD>, ModuleDefMD> <>9__15_0;
		}

		// Token: 0x02000058 RID: 88
		[CompilerGenerated]
		private sealed class <ReadInfos>d__7 : IEnumerable<ObfAttrMarker.ProtectionSettingsInfo>, IEnumerator<ObfAttrMarker.ProtectionSettingsInfo>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x06000204 RID: 516 RVA: 0x00002DD1 File Offset: 0x00000FD1
			[DebuggerHidden]
			public <ReadInfos>d__7(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x06000205 RID: 517 RVA: 0x000101B8 File Offset: 0x0000E3B8
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				int num = this.<>1__state;
				if (num == -3 || num - 1 <= 1)
				{
					try
					{
					}
					finally
					{
						this.<>m__Finally1();
					}
				}
			}

			// Token: 0x06000206 RID: 518 RVA: 0x000101F8 File Offset: 0x0000E3F8
			bool IEnumerator.MoveNext()
			{
				bool result;
				try
				{
					switch (this.<>1__state)
					{
					case 0:
						this.<>1__state = -1;
						enumerator = ObfAttrMarker.ReadObfuscationAttributes(item).GetEnumerator();
						this.<>1__state = -3;
						goto IL_106;
					case 1:
						this.<>1__state = -3;
						break;
					case 2:
						this.<>1__state = -3;
						break;
					default:
						return false;
					}
					IL_ED:
					info = default(ObfAttrMarker.ProtectionSettingsInfo);
					attr = default(ObfAttrMarker.ObfuscationAttributeInfo);
					IL_106:
					if (!enumerator.MoveNext())
					{
						this.<>m__Finally1();
						enumerator = null;
						result = false;
					}
					else
					{
						attr = enumerator.Current;
						bool flag = !string.IsNullOrEmpty(attr.FeatureName);
						if (flag)
						{
							this.<>2__current = base.AddRule(attr, null);
							this.<>1__state = 1;
							result = true;
						}
						else
						{
							bool flag2 = base.ToInfo(attr, out info);
							if (!flag2)
							{
								goto IL_ED;
							}
							this.<>2__current = info;
							this.<>1__state = 2;
							result = true;
						}
					}
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return result;
			}

			// Token: 0x06000207 RID: 519 RVA: 0x00002DF1 File Offset: 0x00000FF1
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}

			// Token: 0x17000029 RID: 41
			// (get) Token: 0x06000208 RID: 520 RVA: 0x00002E0E File Offset: 0x0000100E
			ObfAttrMarker.ProtectionSettingsInfo IEnumerator<ObfAttrMarker.ProtectionSettingsInfo>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000209 RID: 521 RVA: 0x0000268B File Offset: 0x0000088B
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x1700002A RID: 42
			// (get) Token: 0x0600020A RID: 522 RVA: 0x00002E16 File Offset: 0x00001016
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0600020B RID: 523 RVA: 0x00010354 File Offset: 0x0000E554
			[DebuggerHidden]
			IEnumerator<ObfAttrMarker.ProtectionSettingsInfo> IEnumerable<ObfAttrMarker.ProtectionSettingsInfo>.GetEnumerator()
			{
				ObfAttrMarker.<ReadInfos>d__7 <ReadInfos>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<ReadInfos>d__ = this;
				}
				else
				{
					<ReadInfos>d__ = new ObfAttrMarker.<ReadInfos>d__7(0);
					<ReadInfos>d__.<>4__this = this;
				}
				<ReadInfos>d__.item = item;
				return <ReadInfos>d__;
			}

			// Token: 0x0600020C RID: 524 RVA: 0x00002E23 File Offset: 0x00001023
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Confuser.Core.ObfAttrMarker.ProtectionSettingsInfo>.GetEnumerator();
			}

			// Token: 0x040001AE RID: 430
			private int <>1__state;

			// Token: 0x040001AF RID: 431
			private ObfAttrMarker.ProtectionSettingsInfo <>2__current;

			// Token: 0x040001B0 RID: 432
			private int <>l__initialThreadId;

			// Token: 0x040001B1 RID: 433
			private IHasCustomAttribute item;

			// Token: 0x040001B2 RID: 434
			public IHasCustomAttribute <>3__item;

			// Token: 0x040001B3 RID: 435
			public ObfAttrMarker <>4__this;

			// Token: 0x040001B4 RID: 436
			private IEnumerator<ObfAttrMarker.ObfuscationAttributeInfo> <>s__1;

			// Token: 0x040001B5 RID: 437
			private ObfAttrMarker.ObfuscationAttributeInfo <attr>5__2;

			// Token: 0x040001B6 RID: 438
			private ObfAttrMarker.ProtectionSettingsInfo <info>5__3;
		}
	}
}
