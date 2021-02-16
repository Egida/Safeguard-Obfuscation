using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.Renamer.BAML;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.IO;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000087 RID: 135
	internal class WPFAnalyzer : IRenamer
	{
		// Token: 0x060002FF RID: 767 RVA: 0x000234B8 File Offset: 0x000216B8
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			MethodDef method = def as MethodDef;
			bool flag = method != null;
			if (flag)
			{
				bool flag2 = !method.HasBody;
				if (flag2)
				{
					return;
				}
				this.AnalyzeMethod(context, service, method);
			}
			ModuleDefMD module = def as ModuleDefMD;
			bool flag3 = module != null;
			if (flag3)
			{
				this.AnalyzeResources(context, service, module);
			}
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00023510 File Offset: 0x00021710
		private void AnalyzeMethod(ConfuserContext context, INameService service, MethodDef method)
		{
			List<Tuple<bool, Instruction>> dpRegInstrs = new List<Tuple<bool, Instruction>>();
			List<Instruction> routedEvtRegInstrs = new List<Instruction>();
			foreach (Instruction instr in method.Body.Instructions)
			{
				bool flag = instr.OpCode.Code == Code.Call || instr.OpCode.Code == Code.Callvirt;
				if (flag)
				{
					IMethod regMethod = (IMethod)instr.Operand;
					bool flag2 = regMethod.DeclaringType.FullName == "System.Windows.DependencyProperty" && regMethod.Name.String.StartsWith("Register");
					if (flag2)
					{
						dpRegInstrs.Add(Tuple.Create<bool, Instruction>(regMethod.Name.String.StartsWith("RegisterAttached"), instr));
					}
					else
					{
						bool flag3 = regMethod.DeclaringType.FullName == "System.Windows.EventManager" && regMethod.Name.String == "RegisterRoutedEvent";
						if (flag3)
						{
							routedEvtRegInstrs.Add(instr);
						}
					}
				}
				else
				{
					bool flag4 = instr.OpCode == OpCodes.Ldstr;
					if (flag4)
					{
						string operand = ((string)instr.Operand).ToUpperInvariant();
						bool flag5 = operand.EndsWith(".BAML") || operand.EndsWith(".XAML");
						if (flag5)
						{
							Match match = WPFAnalyzer.UriPattern.Match(operand);
							bool success = match.Success;
							if (success)
							{
								operand = match.Groups[1].Value;
							}
							BAMLStringReference reference = new BAMLStringReference(instr);
							operand = operand.TrimStart(new char[]
							{
								'/'
							});
							string baml = operand.Substring(0, operand.Length - 5) + ".BAML";
							string xaml = operand.Substring(0, operand.Length - 5) + ".XAML";
							this.bamlRefs.AddListEntry(baml, reference);
							this.bamlRefs.AddListEntry(xaml, reference);
						}
					}
				}
			}
			bool flag6 = dpRegInstrs.Count == 0;
			if (!flag6)
			{
				ITraceService traceSrv = context.Registry.GetService<ITraceService>();
				MethodTrace trace = traceSrv.Trace(method);
				bool erred = false;
				foreach (Tuple<bool, Instruction> instrInfo in dpRegInstrs)
				{
					int[] args = trace.TraceArguments(instrInfo.Item2);
					bool flag7 = args == null;
					if (flag7)
					{
						bool flag8 = !erred;
						if (flag8)
						{
							context.Logger.WarnFormat("Failed to extract dependency property name in '{0}'.", new object[]
							{
								method.FullName
							});
						}
						erred = true;
					}
					else
					{
						Instruction ldstr = method.Body.Instructions[args[0]];
						bool flag9 = ldstr.OpCode.Code != Code.Ldstr;
						if (flag9)
						{
							bool flag10 = !erred;
							if (flag10)
							{
								context.Logger.WarnFormat("Failed to extract dependency property name in '{0}'.", new object[]
								{
									method.FullName
								});
							}
							erred = true;
						}
						else
						{
							string name = (string)ldstr.Operand;
							TypeDef declType = method.DeclaringType;
							bool found = false;
							bool item = instrInfo.Item1;
							if (item)
							{
								MethodDef accessor;
								bool flag11 = (accessor = declType.FindMethod("Get" + name)) != null && accessor.IsStatic;
								if (flag11)
								{
									service.SetCanRename(accessor, false);
									found = true;
								}
								bool flag12 = (accessor = declType.FindMethod("Set" + name)) != null && accessor.IsStatic;
								if (flag12)
								{
									service.SetCanRename(accessor, false);
									found = true;
								}
							}
							PropertyDef property;
							bool flag13 = (property = declType.FindProperty(name)) != null;
							if (flag13)
							{
								service.SetCanRename(property, false);
								found = true;
								bool flag14 = property.GetMethod != null;
								if (flag14)
								{
									service.SetCanRename(property.GetMethod, false);
								}
								bool flag15 = property.SetMethod != null;
								if (flag15)
								{
									service.SetCanRename(property.SetMethod, false);
								}
								bool hasOtherMethods = property.HasOtherMethods;
								if (hasOtherMethods)
								{
									foreach (MethodDef accessor2 in property.OtherMethods)
									{
										service.SetCanRename(accessor2, false);
									}
								}
							}
							bool flag16 = !found;
							if (flag16)
							{
								bool item2 = instrInfo.Item1;
								if (item2)
								{
									context.Logger.WarnFormat("Failed to find the accessors of attached dependency property '{0}' in type '{1}'.", new object[]
									{
										name,
										declType.FullName
									});
								}
								else
								{
									context.Logger.WarnFormat("Failed to find the CLR property of normal dependency property '{0}' in type '{1}'.", new object[]
									{
										name,
										declType.FullName
									});
								}
							}
						}
					}
				}
				erred = false;
				foreach (Instruction instr2 in routedEvtRegInstrs)
				{
					int[] args2 = trace.TraceArguments(instr2);
					bool flag17 = args2 == null;
					if (flag17)
					{
						bool flag18 = !erred;
						if (flag18)
						{
							context.Logger.WarnFormat("Failed to extract routed event name in '{0}'.", new object[]
							{
								method.FullName
							});
						}
						erred = true;
					}
					else
					{
						Instruction ldstr2 = method.Body.Instructions[args2[0]];
						bool flag19 = ldstr2.OpCode.Code != Code.Ldstr;
						if (flag19)
						{
							bool flag20 = !erred;
							if (flag20)
							{
								context.Logger.WarnFormat("Failed to extract routed event name in '{0}'.", new object[]
								{
									method.FullName
								});
							}
							erred = true;
						}
						else
						{
							string name2 = (string)ldstr2.Operand;
							TypeDef declType2 = method.DeclaringType;
							EventDef eventDef;
							bool flag21 = (eventDef = declType2.FindEvent(name2)) == null;
							if (flag21)
							{
								context.Logger.WarnFormat("Failed to find the CLR event of routed event '{0}' in type '{1}'.", new object[]
								{
									name2,
									declType2.FullName
								});
							}
							else
							{
								service.SetCanRename(eventDef, false);
								bool flag22 = eventDef.AddMethod != null;
								if (flag22)
								{
									service.SetCanRename(eventDef.AddMethod, false);
								}
								bool flag23 = eventDef.RemoveMethod != null;
								if (flag23)
								{
									service.SetCanRename(eventDef.RemoveMethod, false);
								}
								bool flag24 = eventDef.InvokeMethod != null;
								if (flag24)
								{
									service.SetCanRename(eventDef.InvokeMethod, false);
								}
								bool hasOtherMethods2 = eventDef.HasOtherMethods;
								if (hasOtherMethods2)
								{
									foreach (MethodDef accessor3 in eventDef.OtherMethods)
									{
										service.SetCanRename(accessor3, false);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00023CB0 File Offset: 0x00021EB0
		private void AnalyzeResources(ConfuserContext context, INameService service, ModuleDefMD module)
		{
			bool flag = this.analyzer == null;
			if (flag)
			{
				this.analyzer = new BAMLAnalyzer(context, service);
				this.analyzer.AnalyzeElement += this.AnalyzeBAMLElement;
			}
			Dictionary<string, Dictionary<string, BamlDocument>> wpfResInfo = new Dictionary<string, Dictionary<string, BamlDocument>>();
			foreach (EmbeddedResource res in module.Resources.OfType<EmbeddedResource>())
			{
				Match match = WPFAnalyzer.ResourceNamePattern.Match(res.Name);
				bool success = match.Success;
				if (success)
				{
					Dictionary<string, BamlDocument> resInfo = new Dictionary<string, BamlDocument>();
					res.Data.Position = 0L;
					ResourceReader reader = new ResourceReader(new ImageStream(res.Data));
					IDictionaryEnumerator enumerator = reader.GetEnumerator();
					while (enumerator.MoveNext())
					{
						string name = (string)enumerator.Key;
						bool flag2 = name.EndsWith(".baml");
						if (flag2)
						{
							string typeName;
							byte[] data;
							reader.GetResourceData(name, out typeName, out data);
							BamlDocument document = this.analyzer.Analyze(module, name, data);
							document.DocumentName = name;
							resInfo.Add(name, document);
						}
					}
					bool flag3 = resInfo.Count > 0;
					if (flag3)
					{
						wpfResInfo.Add(res.Name, resInfo);
					}
				}
			}
			bool flag4 = wpfResInfo.Count > 0;
			if (flag4)
			{
				context.Annotations.Set<Dictionary<string, Dictionary<string, BamlDocument>>>(module, WPFAnalyzer.BAMLKey, wpfResInfo);
			}
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00023E54 File Offset: 0x00022054
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDefMD module = def as ModuleDefMD;
			bool flag = module == null;
			if (!flag)
			{
				Dictionary<string, Dictionary<string, BamlDocument>> wpfResInfo = context.Annotations.Get<Dictionary<string, Dictionary<string, BamlDocument>>>(module, WPFAnalyzer.BAMLKey, null);
				bool flag2 = wpfResInfo == null;
				if (!flag2)
				{
					foreach (EmbeddedResource res in module.Resources.OfType<EmbeddedResource>())
					{
						Dictionary<string, BamlDocument> resInfo;
						bool flag3 = wpfResInfo.TryGetValue(res.Name, out resInfo);
						if (flag3)
						{
							MemoryStream stream = new MemoryStream();
							ResourceWriter writer = new ResourceWriter(stream);
							res.Data.Position = 0L;
							ResourceReader reader = new ResourceReader(new ImageStream(res.Data));
							IDictionaryEnumerator enumerator = reader.GetEnumerator();
							while (enumerator.MoveNext())
							{
								string name = (string)enumerator.Key;
								string typeName;
								byte[] data;
								reader.GetResourceData(name, out typeName, out data);
								BamlDocument document;
								bool flag4 = resInfo.TryGetValue(name, out document);
								if (flag4)
								{
									MemoryStream docStream = new MemoryStream();
									docStream.Position = 4L;
									BamlWriter.WriteDocument(document, docStream);
									docStream.Position = 0L;
									docStream.Write(BitConverter.GetBytes((int)docStream.Length - 4), 0, 4);
									data = docStream.ToArray();
									name = document.DocumentName;
								}
								writer.AddResourceData(name, typeName, data);
							}
							writer.Generate();
							res.Data = MemoryImageStream.Create(stream.ToArray());
						}
					}
				}
			}
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00024010 File Offset: 0x00022210
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDefMD module = def as ModuleDefMD;
			bool flag = module == null || !parameters.GetParameter<bool>(context, def, "renXaml", true);
			if (!flag)
			{
				Dictionary<string, Dictionary<string, BamlDocument>> wpfResInfo = context.Annotations.Get<Dictionary<string, Dictionary<string, BamlDocument>>>(module, WPFAnalyzer.BAMLKey, null);
				bool flag2 = wpfResInfo == null;
				if (!flag2)
				{
					foreach (Dictionary<string, BamlDocument> res in wpfResInfo.Values)
					{
						foreach (BamlDocument doc in res.Values)
						{
							List<IBAMLReference> references;
							bool flag3 = this.bamlRefs.TryGetValue(doc.DocumentName, out references);
							if (flag3)
							{
								string newName = doc.DocumentName.ToUpperInvariant();
								string[] completePath = newName.Split(new string[]
								{
									"/"
								}, StringSplitOptions.RemoveEmptyEntries);
								string newShinyName = string.Empty;
								for (int i = 0; i <= completePath.Length - 2; i++)
								{
									newShinyName = newShinyName + completePath[i].ToLowerInvariant() + "/";
								}
								bool flag4 = newName.EndsWith(".BAML");
								if (flag4)
								{
									newName = newShinyName + service.RandomName(RenameMode.Letters).ToLowerInvariant() + ".baml";
								}
								else
								{
									bool flag5 = newName.EndsWith(".XAML");
									if (flag5)
									{
										newName = newShinyName + service.RandomName(RenameMode.Letters).ToLowerInvariant() + ".xaml";
									}
								}
								context.Logger.Debug(string.Format("Preserving virtual paths. Replaced {0} with {1}", doc.DocumentName, newName));
								bool renameOk = true;
								foreach (IBAMLReference bamlRef in references)
								{
									bool flag6 = !bamlRef.CanRename(doc.DocumentName, newName);
									if (flag6)
									{
										renameOk = false;
										break;
									}
								}
								bool flag7 = renameOk;
								if (flag7)
								{
									foreach (IBAMLReference bamlRef2 in references)
									{
										bamlRef2.Rename(doc.DocumentName, newName);
									}
									doc.DocumentName = newName;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000304 RID: 772 RVA: 0x000242F0 File Offset: 0x000224F0
		// (remove) Token: 0x06000305 RID: 773 RVA: 0x00024328 File Offset: 0x00022528
		public event Action<BAMLAnalyzer, BamlElement> AnalyzeBAMLElement
		{
			[CompilerGenerated]
			add
			{
				Action<BAMLAnalyzer, BamlElement> action = this.AnalyzeBAMLElement;
				Action<BAMLAnalyzer, BamlElement> action2;
				do
				{
					action2 = action;
					Action<BAMLAnalyzer, BamlElement> value2 = (Action<BAMLAnalyzer, BamlElement>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange<Action<BAMLAnalyzer, BamlElement>>(ref this.AnalyzeBAMLElement, value2, action2);
				}
				while (action != action2);
			}
			[CompilerGenerated]
			remove
			{
				Action<BAMLAnalyzer, BamlElement> action = this.AnalyzeBAMLElement;
				Action<BAMLAnalyzer, BamlElement> action2;
				do
				{
					action2 = action;
					Action<BAMLAnalyzer, BamlElement> value2 = (Action<BAMLAnalyzer, BamlElement>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange<Action<BAMLAnalyzer, BamlElement>>(ref this.AnalyzeBAMLElement, value2, action2);
				}
				while (action != action2);
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0000364B File Offset: 0x0000184B
		public WPFAnalyzer()
		{
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00003664 File Offset: 0x00001864
		// Note: this type is marked as 'beforefieldinit'.
		static WPFAnalyzer()
		{
		}

		// Token: 0x04000541 RID: 1345
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<BAMLAnalyzer, BamlElement> AnalyzeBAMLElement;

		// Token: 0x04000542 RID: 1346
		private BAMLAnalyzer analyzer;

		// Token: 0x04000543 RID: 1347
		private static readonly object BAMLKey = new object();

		// Token: 0x04000544 RID: 1348
		internal Dictionary<string, List<IBAMLReference>> bamlRefs = new Dictionary<string, List<IBAMLReference>>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000545 RID: 1349
		private static readonly Regex ResourceNamePattern = new Regex("^.*\\.g\\.resources$");

		// Token: 0x04000546 RID: 1350
		internal static readonly Regex UriPattern = new Regex(";COMPONENT/(.+\\.[BX]AML)$");
	}
}
