using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using dnlib.PE;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using SProtector.SProtector.Tools;

namespace Confuser.Core
{
	/// <summary>
	///     The processing engine of ConfuserEx.
	/// </summary>
	// Token: 0x0200002B RID: 43
	public static class ConfuserEngine
	{
		// Token: 0x060000E5 RID: 229 RVA: 0x00009AEC File Offset: 0x00007CEC
		static ConfuserEngine()
		{
			Assembly assembly = typeof(ConfuserEngine).Assembly;
			AssemblyProductAttribute nameAttr = (AssemblyProductAttribute)assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
			AssemblyInformationalVersionAttribute verAttr = (AssemblyInformationalVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)[0];
			AssemblyCopyrightAttribute cpAttr = (AssemblyCopyrightAttribute)assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
			ConfuserEngine.Version = string.Format("{0} {1}", nameAttr.Product, verAttr.InformationalVersion);
			ConfuserEngine.Copyright = cpAttr.Copyright;
			AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
			{
				Assembly result;
				try
				{
					AssemblyName asmName = new AssemblyName(e.Name);
					foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
					{
						bool flag = asm.GetName().Name == asmName.Name;
						if (flag)
						{
							result = asm;
							return result;
						}
					}
					result = null;
				}
				catch
				{
					result = null;
				}
				return result;
			};
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00009BA0 File Offset: 0x00007DA0
		private static void BeginModule(ConfuserContext context)
		{
			context.Logger.InfoFormat("Processing module '{0}'...", new object[]
			{
				context.CurrentModule.Name
			});
			context.CurrentModuleWriterListener = new ModuleWriterListener();
			context.CurrentModuleWriterListener.OnWriterEvent += delegate(object sender, ModuleWriterListenerEventArgs e)
			{
				context.CheckCancellation();
			};
			context.CurrentModuleWriterOptions = new ModuleWriterOptions(context.CurrentModule, context.CurrentModuleWriterListener);
			ConfuserEngine.CopyPEHeaders(context.CurrentModuleWriterOptions.PEHeadersOptions, context.CurrentModule);
			bool flag = !context.CurrentModule.IsILOnly || context.CurrentModule.VTableFixups != null;
			if (flag)
			{
				context.RequestNative();
			}
			StrongNameKey snKey = context.Annotations.Get<StrongNameKey>(context.CurrentModule, Marker.SNKey, null);
			context.CurrentModuleWriterOptions.InitializeStrongNameSigning(context.CurrentModule, snKey);
			foreach (TypeDef type in context.CurrentModule.GetTypes())
			{
				foreach (MethodDef method in type.Methods)
				{
					bool flag2 = method.Body != null;
					if (flag2)
					{
						method.Body.Instructions.SimplifyMacros(method.Body.Variables, method.Parameters);
					}
				}
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00009D98 File Offset: 0x00007F98
		private static void CopyPEHeaders(PEHeadersOptions writerOptions, ModuleDefMD module)
		{
			IPEImage image = module.MetaData.PEImage;
			writerOptions.MajorImageVersion = new ushort?(image.ImageNTHeaders.OptionalHeader.MajorImageVersion);
			writerOptions.MajorLinkerVersion = new byte?(image.ImageNTHeaders.OptionalHeader.MajorLinkerVersion);
			writerOptions.MajorOperatingSystemVersion = new ushort?(image.ImageNTHeaders.OptionalHeader.MajorOperatingSystemVersion);
			writerOptions.MajorSubsystemVersion = new ushort?(image.ImageNTHeaders.OptionalHeader.MajorSubsystemVersion);
			writerOptions.MinorImageVersion = new ushort?(image.ImageNTHeaders.OptionalHeader.MinorImageVersion);
			writerOptions.MinorLinkerVersion = new byte?(image.ImageNTHeaders.OptionalHeader.MinorLinkerVersion);
			writerOptions.MinorOperatingSystemVersion = new ushort?(image.ImageNTHeaders.OptionalHeader.MinorOperatingSystemVersion);
			writerOptions.MinorSubsystemVersion = new ushort?(image.ImageNTHeaders.OptionalHeader.MinorSubsystemVersion);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00009E8C File Offset: 0x0000808C
		private static void Debug(ConfuserContext context)
		{
			context.Logger.Info("Finalizing...");
			for (int i = 0; i < context.OutputModules.Count; i++)
			{
				bool flag = context.OutputSymbols[i] != null;
				if (flag)
				{
					string path = Path.GetFullPath(Path.Combine(context.OutputDirectory, context.OutputPaths[i]));
					string dir = Path.GetDirectoryName(path);
					bool flag2 = !Directory.Exists(dir);
					if (flag2)
					{
						Directory.CreateDirectory(dir);
					}
					File.WriteAllBytes(Path.ChangeExtension(path, "pdb"), context.OutputSymbols[i]);
				}
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00009F3C File Offset: 0x0000813C
		private static void EndModule(ConfuserContext context)
		{
			string output = context.Modules[context.CurrentModuleIndex].Location;
			bool flag = output != null;
			if (flag)
			{
				bool flag2 = !Path.IsPathRooted(output);
				if (flag2)
				{
					output = Path.Combine(Environment.CurrentDirectory, output);
				}
				output = Utils.GetRelativePath(output, context.BaseDirectory);
			}
			else
			{
				output = context.CurrentModule.Name;
			}
			context.OutputPaths[context.CurrentModuleIndex] = output;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000025CD File Offset: 0x000007CD
		private static IEnumerable<string> GetFrameworkVersions()
		{
			using (RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\"))
			{
				string[] subKeyNames = registryKey.GetSubKeyNames();
				int num2;
				for (int i = 0; i < subKeyNames.Length; i = num2 + 1)
				{
					string text = subKeyNames[i];
					bool flag = text.StartsWith("v");
					if (flag)
					{
						RegistryKey registryKey2 = registryKey.OpenSubKey(text);
						string text2 = (string)registryKey2.GetValue("Version", "");
						string a = registryKey2.GetValue("SP", "").ToString();
						string a2 = registryKey2.GetValue("Install", "").ToString();
						bool flag2 = a2 == "" || (a != "" && a2 == "1");
						if (flag2)
						{
							yield return text + "  " + text2;
						}
						bool flag3 = !(text2 != "");
						if (flag3)
						{
							string[] subKeyNames2 = registryKey2.GetSubKeyNames();
							for (int j = 0; j < subKeyNames2.Length; j = num2 + 1)
							{
								string text3 = subKeyNames2[j];
								RegistryKey registryKey3 = registryKey2.OpenSubKey(text3);
								text2 = (string)registryKey3.GetValue("Version", "");
								bool flag4 = text2 != "";
								if (flag4)
								{
									a = registryKey3.GetValue("SP", "").ToString();
								}
								a2 = registryKey3.GetValue("Install", "").ToString();
								bool flag5 = a2 == "";
								if (flag5)
								{
									yield return text + "  " + text2;
								}
								else
								{
									bool flag6 = a2 == "1";
									if (flag6)
									{
										yield return "  " + text3 + "  " + text2;
									}
								}
								text3 = null;
								registryKey3 = null;
								num2 = j;
							}
							subKeyNames2 = null;
						}
						registryKey2 = null;
						text2 = null;
						a = null;
						a2 = null;
					}
					text = null;
					num2 = i;
				}
				subKeyNames = null;
			}
			RegistryKey registryKey = null;
			using (RegistryKey registryKey4 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
			{
				bool flag7 = registryKey4.GetValue("Release") == null;
				if (flag7)
				{
					yield break;
				}
				int num = (int)registryKey4.GetValue("Release");
				yield return "v4.5 " + num;
			}
			RegistryKey registryKey4 = null;
			yield break;
			yield break;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00009FBC File Offset: 0x000081BC
		private static void Inspection(ConfuserContext context)
		{
			context.Logger.Info("Resolving dependencies...");
			foreach (Tuple<AssemblyRef, ModuleDefMD> tuple in context.Modules.SelectMany((ModuleDefMD module) => from asmRef in module.GetAssemblyRefs()
			select Tuple.Create<AssemblyRef, ModuleDefMD>(asmRef, module)))
			{
				try
				{
					context.Resolver.ResolveThrow(tuple.Item1, tuple.Item2);
				}
				catch (AssemblyResolveException ex)
				{
					context.Logger.ErrorException("Failed to resolve dependency of '" + tuple.Item2.Name + "'.", ex);
					throw new ConfuserException(ex);
				}
			}
			context.Logger.Debug("Checking Strong Name...");
			foreach (ModuleDefMD moduleDefMD in context.Modules)
			{
				StrongNameKey strongNameKey = context.Annotations.Get<StrongNameKey>(moduleDefMD, Marker.SNKey, null);
				bool flag = strongNameKey == null && moduleDefMD.IsStrongNameSigned;
				if (flag)
				{
					context.Logger.WarnFormat("[{0}] SN Key is not provided for a signed module, the output may not be working.", new object[]
					{
						moduleDefMD.Name
					});
				}
				else
				{
					bool flag2 = strongNameKey != null && !moduleDefMD.IsStrongNameSigned;
					if (flag2)
					{
						context.Logger.WarnFormat("[{0}] SN Key is provided for an unsigned module, the output may not be working.", new object[]
						{
							moduleDefMD.Name
						});
					}
					else
					{
						bool flag3 = strongNameKey != null && moduleDefMD.IsStrongNameSigned && !moduleDefMD.Assembly.PublicKey.Data.SequenceEqual(strongNameKey.PublicKey);
						if (flag3)
						{
							context.Logger.WarnFormat("[{0}] Provided SN Key and signed module's public key do not match, the output may not be working.", new object[]
							{
								moduleDefMD.Name
							});
						}
					}
				}
			}
			IMarkerService service = context.Registry.GetService<IMarkerService>();
			context.Logger.Debug("Creating global .cctors...");
			foreach (ModuleDefMD moduleDefMD2 in context.Modules)
			{
				TypeDef typeDef = moduleDefMD2.GlobalType;
				bool flag4 = typeDef == null;
				if (flag4)
				{
					typeDef = new TypeDefUser("", "<Module>", null);
					typeDef.Attributes = dnlib.DotNet.TypeAttributes.NotPublic;
					moduleDefMD2.Types.Add(typeDef);
					service.Mark(typeDef, null);
				}
				MethodDef methodDef = typeDef.FindOrCreateStaticConstructor();
				bool flag5 = !service.IsMarked(methodDef);
				if (flag5)
				{
					service.Mark(methodDef, null);
				}
			}
			context.Logger.Debug("Watermarking...");
			foreach (ModuleDefMD moduleDefMD3 in context.Modules)
			{
				TypeRef typeRef = moduleDefMD3.CorLibTypes.GetTypeRef("System", "Attribute");
				TypeRef typeRef2 = moduleDefMD3.CorLibTypes.GetTypeRef("System", "Attribute");
				TypeDefUser typeDefUser = new TypeDefUser("", "BabelObfuscatorAttribute", typeRef);
				moduleDefMD3.Types.Add(typeDefUser);
				service.Mark(typeDefUser, null);
				TypeDefUser typeDefUser2 = new TypeDefUser("", "NETGuard-v4.5", typeRef2);
				moduleDefMD3.Types.Add(typeDefUser2);
				service.Mark(typeDefUser2, null);
				TypeDefUser typeDefUser3 = new TypeDefUser("", "YanoAttribute", typeRef);
				moduleDefMD3.Types.Add(typeDefUser3);
				service.Mark(typeDefUser3, null);
				TypeDefUser typeDefUser4 = new TypeDefUser("", "ZYXDNGuarder", typeRef);
				moduleDefMD3.Types.Add(typeDefUser4);
				service.Mark(typeDefUser4, null);
				TypeDefUser typeDefUser5 = new TypeDefUser("", "ObfuscatedByGoliath", typeRef);
				moduleDefMD3.Types.Add(typeDefUser5);
				service.Mark(typeDefUser5, null);
				TypeDefUser typeDefUser6 = new TypeDefUser("", "Macrobject", typeRef);
				moduleDefMD3.Types.Add(typeDefUser6);
				service.Mark(typeDefUser6, null);
				TypeDefUser typeDefUser7 = new TypeDefUser("", "DotfuscatorAttribute", typeRef);
				moduleDefMD3.Types.Add(typeDefUser7);
				service.Mark(typeDefUser7, null);
				TypeDefUser typeDefUser8 = new TypeDefUser("", "NETSecure", typeRef);
				moduleDefMD3.Types.Add(typeDefUser8);
				service.Mark(typeDefUser8, null);
				TypeDefUser typeDefUser9 = new TypeDefUser("", "ProtectedWithCryptoObfuscatorAttribute", typeRef);
				moduleDefMD3.Types.Add(typeDefUser9);
				service.Mark(typeDefUser9, null);
				TypeDefUser typeDefUser10 = new TypeDefUser("", "ObfuscatedByAgileDotNetAttribute", typeRef);
				moduleDefMD3.Types.Add(typeDefUser10);
				service.Mark(typeDefUser10, null);
				TypeDefUser typeDefUser11 = new TypeDefUser("", "NineRays.Obfuscator", typeRef);
				moduleDefMD3.Types.Add(typeDefUser11);
				service.Mark(typeDefUser11, null);
				TypeDefUser typeDefUser12 = new TypeDefUser("", "{2df70ad6-d916-4c54-bdbf-65e51d5a06cb}", typeRef);
				moduleDefMD3.Types.Add(typeDefUser12);
				service.Mark(typeDefUser12, null);
				TypeDefUser typeDefUser13 = new TypeDefUser("", "{60fac4f0-f7e4-42a3-9db5-b6c6f67d70ff}", typeRef);
				moduleDefMD3.Types.Add(typeDefUser13);
				service.Mark(typeDefUser13, null);
				TypeDefUser typeDefUser14 = new TypeDefUser("", "{9a6f81eb-8553-4026-9b69-e57d9aa81e38}", typeRef);
				moduleDefMD3.Types.Add(typeDefUser14);
				service.Mark(typeDefUser14, null);
				int num = (int)Math.Round((double)(Conversion.Int(VBMath.Rnd() * 100f) + 100f));
				for (int i = 0; i <= num; i++)
				{
					TypeDefUser typeDefUser15 = new TypeDefUser("", ConfuserEngine.StringProtection.Random(20), typeRef2);
					moduleDefMD3.Types.Add(typeDefUser15);
					service.Mark(typeDefUser15, null);
					TypeDefUser typeDefUser16 = new TypeDefUser("", ConfuserEngine.StringProtection.Random(20), typeRef2);
					moduleDefMD3.Types.Add(typeDefUser16);
					TypeDefUser typeDefUser17 = new TypeDefUser("", ConfuserEngine.StringProtection.Random(20), typeRef2);
					moduleDefMD3.Types.Add(typeDefUser17);
					TypeDefUser typeDefUser18 = new TypeDefUser("", ConfuserEngine.StringProtection.Random(20), typeRef2);
					moduleDefMD3.Types.Add(typeDefUser18);
					TypeDefUser typeDefUser19 = new TypeDefUser("", ConfuserEngine.StringProtection.Random(20), typeRef2);
					moduleDefMD3.Types.Add(typeDefUser19);
					TypeDefUser typeDefUser20 = new TypeDefUser("", ConfuserEngine.StringProtection.Random(20), typeRef2);
					moduleDefMD3.Types.Add(typeDefUser20);
					TypeDefUser typeDefUser21 = new TypeDefUser("", ConfuserEngine.StringProtection.Random(20), typeRef2);
					moduleDefMD3.Types.Add(typeDefUser21);
					TypeDefUser typeDefUser22 = new TypeDefUser("", "<" + ConfuserEngine.StringProtection.Random(20), typeRef2);
					moduleDefMD3.Types.Add(typeDefUser22);
					TypeDefUser typeDefUser23 = new TypeDefUser("", "<M" + ConfuserEngine.StringProtection.Random(20), typeRef2);
					moduleDefMD3.Types.Add(typeDefUser23);
					service.Mark(typeDefUser15, null);
					service.Mark(typeDefUser16, null);
					service.Mark(typeDefUser17, null);
					service.Mark(typeDefUser18, null);
					service.Mark(typeDefUser19, null);
					service.Mark(typeDefUser20, null);
					service.Mark(typeDefUser21, null);
					service.Mark(typeDefUser22, null);
					service.Mark(typeDefUser23, null);
				}
				MethodDefUser methodDefUser = new MethodDefUser(".ctor", MethodSig.CreateInstance(moduleDefMD3.CorLibTypes.Void, moduleDefMD3.CorLibTypes.String), dnlib.DotNet.MethodImplAttributes.IL, dnlib.DotNet.MethodAttributes.FamANDAssem | dnlib.DotNet.MethodAttributes.Family | dnlib.DotNet.MethodAttributes.HideBySig | dnlib.DotNet.MethodAttributes.SpecialName | dnlib.DotNet.MethodAttributes.RTSpecialName);
				methodDefUser.Body = new CilBody();
				methodDefUser.Body.MaxStack = 1;
				methodDefUser.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
				methodDefUser.Body.Instructions.Add(OpCodes.Call.ToInstruction(new MemberRefUser(moduleDefMD3, ".ctor", MethodSig.CreateInstance(moduleDefMD3.CorLibTypes.Void), typeRef)));
				methodDefUser.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
				typeDefUser.Methods.Add(methodDefUser);
				service.Mark(methodDefUser, null);
				CustomAttribute customAttribute = new CustomAttribute(methodDefUser);
				CustomAttribute customAttribute2 = new CustomAttribute(methodDefUser);
				CustomAttribute customAttribute3 = new CustomAttribute(methodDefUser);
				CustomAttribute customAttribute4 = new CustomAttribute(methodDefUser);
				CustomAttribute customAttribute5 = new CustomAttribute(methodDefUser);
				customAttribute.ConstructorArguments.Add(new CAArgument(moduleDefMD3.CorLibTypes.String, ConfuserEngine.Version));
				customAttribute2.ConstructorArguments.Add(new CAArgument(moduleDefMD3.CorLibTypes.String, "This program has been obfuscated by SafeGuard Obfuscation, a .NET obfuscation program."));
				moduleDefMD3.CustomAttributes.Add(customAttribute2);
			}
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000AA50 File Offset: 0x00008C50
		private static void OptimizeMethods(ConfuserContext context)
		{
			foreach (TypeDef type in context.CurrentModule.GetTypes())
			{
				foreach (MethodDef method in type.Methods)
				{
					bool flag = method.Body != null;
					if (flag)
					{
						method.Body.Instructions.OptimizeMacros();
					}
				}
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000AAFC File Offset: 0x00008CFC
		private static void Pack(ConfuserContext context)
		{
			bool flag = context.Packer != null;
			if (flag)
			{
				context.Logger.Info("Packing...");
				context.Packer.Pack(context, new ProtectionParameters(context.Packer, context.Modules.OfType<IDnlibDef>().ToList<IDnlibDef>()));
			}
		}

		/// <summary>
		///     Prints the environment information when error occurred.
		/// </summary>
		/// <param name="context">The working context.</param>
		// Token: 0x060000EE RID: 238 RVA: 0x0000AB54 File Offset: 0x00008D54
		private static void PrintEnvironmentInfo(ConfuserContext context)
		{
			bool packerInitiated = context.PackerInitiated;
			if (!packerInitiated)
			{
				context.Logger.Error("---BEGIN DEBUG INFO---");
				context.Logger.Error("Installed Framework Versions:");
				foreach (string ver in ConfuserEngine.GetFrameworkVersions())
				{
					context.Logger.ErrorFormat("    {0}", new object[]
					{
						ver.Trim()
					});
				}
				context.Logger.Error("");
				bool flag = context.Resolver != null;
				if (flag)
				{
					context.Logger.Error("Cached assemblies:");
					foreach (AssemblyDef asm in context.Resolver.GetCachedAssemblies())
					{
						bool flag2 = string.IsNullOrEmpty(asm.ManifestModule.Location);
						if (flag2)
						{
							context.Logger.ErrorFormat("    {0}", new object[]
							{
								asm.FullName
							});
						}
						else
						{
							context.Logger.ErrorFormat("    {0} ({1})", new object[]
							{
								asm.FullName,
								asm.ManifestModule.Location
							});
						}
						foreach (AssemblyRef reference in asm.Modules.OfType<ModuleDefMD>().SelectMany((ModuleDefMD m) => m.GetAssemblyRefs()))
						{
							context.Logger.ErrorFormat("        {0}", new object[]
							{
								reference.FullName
							});
						}
					}
				}
				context.Logger.Error("---END DEBUG INFO---");
			}
		}

		/// <summary>
		///     Prints the copyright stuff and environment information.
		/// </summary>
		/// <param name="context">The working context.</param>
		// Token: 0x060000EF RID: 239 RVA: 0x0000AD9C File Offset: 0x00008F9C
		private static void PrintInfo(ConfuserContext context)
		{
			bool packerInitiated = context.PackerInitiated;
			if (packerInitiated)
			{
				context.Logger.Info("Protecting packer stub...");
			}
			else
			{
				context.Logger.InfoFormat("{0} {1}", new object[]
				{
					ConfuserEngine.Version,
					ConfuserEngine.Copyright
				});
				Type mono = Type.GetType("Mono.Runtime");
				context.Logger.InfoFormat("Running on {0}, {1}, {2} bits", new object[]
				{
					Environment.OSVersion,
					(mono == null) ? (".NET Framework v" + Environment.Version) : mono.GetMethod("GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null),
					IntPtr.Size * 8
				});
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000025D6 File Offset: 0x000007D6
		private static void ProcessModule(ConfuserContext context)
		{
		}

		/// <summary>
		///     Runs the engine with the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <param name="token">The token used for cancellation.</param>
		/// <returns>Task to run the engine.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///     <paramref name="parameters" />.Project is <c>null</c>.
		/// </exception>
		// Token: 0x060000F1 RID: 241 RVA: 0x0000AE5C File Offset: 0x0000905C
		public static Task Run(ConfuserParameters parameters, CancellationToken? token = null)
		{
			bool flag = parameters.Project == null;
			if (flag)
			{
				throw new ArgumentNullException("parameters");
			}
			bool flag2 = token == null;
			if (flag2)
			{
				token = new CancellationToken?(new CancellationTokenSource().Token);
			}
			return Task.Factory.StartNew(delegate()
			{
				ConfuserEngine.RunInternal(parameters, token.Value);
			}, token.Value);
		}

		/// <summary>
		///     Runs the engine.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <param name="token">The cancellation token.</param>
		// Token: 0x060000F2 RID: 242 RVA: 0x0000AEEC File Offset: 0x000090EC
		private static void RunInternal(ConfuserParameters parameters, CancellationToken token)
		{
			ConfuserContext context = new ConfuserContext();
			context.Logger = parameters.GetLogger();
			context.Project = parameters.Project.Clone();
			context.PackerInitiated = parameters.PackerInitiated;
			context.token = token;
			ConfuserEngine.PrintInfo(context);
			bool ok = false;
			try
			{
				AssemblyResolver asmResolver = new AssemblyResolver();
				asmResolver.EnableTypeDefCache = true;
				asmResolver.DefaultModuleContext = new ModuleContext(asmResolver);
				context.Resolver = asmResolver;
				context.BaseDirectory = Path.Combine(Environment.CurrentDirectory, parameters.Project.BaseDirectory.TrimEnd(new char[]
				{
					Path.DirectorySeparatorChar
				}) + Path.DirectorySeparatorChar.ToString());
				context.OutputDirectory = Path.Combine(parameters.Project.BaseDirectory, parameters.Project.OutputDirectory.TrimEnd(new char[]
				{
					Path.DirectorySeparatorChar
				}) + Path.DirectorySeparatorChar.ToString());
				foreach (string probePath in parameters.Project.ProbePaths)
				{
					asmResolver.PostSearchPaths.Insert(0, Path.Combine(context.BaseDirectory, probePath));
				}
				context.CheckCancellation();
				Marker marker = parameters.GetMarker();
				context.Logger.Debug("Discovering plugins...");
				IList<Protection> prots;
				IList<Packer> packers;
				IList<ConfuserComponent> components;
				parameters.GetPluginDiscovery().GetPlugins(context, out prots, out packers, out components);
				context.Logger.InfoFormat("Discovered {0} protections, {1} packers.", new object[]
				{
					prots.Count,
					packers.Count
				});
				context.CheckCancellation();
				context.Logger.Debug("Resolving component dependency...");
				try
				{
					DependencyResolver resolver = new DependencyResolver(prots);
					prots = resolver.SortDependency();
				}
				catch (CircularDependencyException ex)
				{
					context.Logger.ErrorException("", ex);
					throw new ConfuserException(ex);
				}
				components.Insert(0, new CoreComponent(parameters, marker));
				foreach (Protection prot in prots)
				{
					components.Add(prot);
				}
				foreach (Packer packer in packers)
				{
					components.Add(packer);
				}
				context.CheckCancellation();
				context.Logger.Info("Loading input modules...");
				marker.Initalize(prots, packers);
				MarkerResult markings = marker.MarkProject(parameters.Project, context);
				context.Modules = markings.Modules.ToList<ModuleDefMD>().AsReadOnly();
				foreach (ModuleDefMD module in context.Modules)
				{
					module.EnableTypeDefFindCache = false;
				}
				context.OutputModules = Enumerable.Repeat<byte[]>(null, markings.Modules.Count).ToArray<byte[]>();
				context.OutputSymbols = Enumerable.Repeat<byte[]>(null, markings.Modules.Count).ToArray<byte[]>();
				context.OutputPaths = Enumerable.Repeat<string>(null, markings.Modules.Count).ToArray<string>();
				context.Packer = markings.Packer;
				context.ExternalModules = markings.ExternalModules;
				context.CheckCancellation();
				context.Logger.Info("Initializing...");
				foreach (ConfuserComponent comp in components)
				{
					try
					{
						comp.Initialize(context);
					}
					catch (Exception ex2)
					{
						context.Logger.ErrorException("Error occured during initialization of '" + comp.Name + "'.", ex2);
						throw new ConfuserException(ex2);
					}
					context.CheckCancellation();
				}
				context.CheckCancellation();
				context.Logger.Debug("Building pipeline...");
				ProtectionPipeline pipeline = new ProtectionPipeline();
				context.Pipeline = pipeline;
				foreach (ConfuserComponent comp2 in components)
				{
					comp2.PopulatePipeline(pipeline);
				}
				context.CheckCancellation();
				ConfuserEngine.RunPipeline(pipeline, context);
				ok = true;
			}
			catch (AssemblyResolveException ex3)
			{
				context.Logger.ErrorException("Failed to resolve an assembly, check if all dependencies are present in the correct version.", ex3);
				ConfuserEngine.PrintEnvironmentInfo(context);
			}
			catch (TypeResolveException ex4)
			{
				context.Logger.ErrorException("Failed to resolve a type, check if all dependencies are present in the correct version.", ex4);
				ConfuserEngine.PrintEnvironmentInfo(context);
			}
			catch (MemberRefResolveException ex5)
			{
				context.Logger.ErrorException("Failed to resolve a member, check if all dependencies are present in the correct version.", ex5);
				ConfuserEngine.PrintEnvironmentInfo(context);
			}
			catch (IOException ex6)
			{
				context.Logger.ErrorException("An IO error occurred, check if all input/output locations are readable/writable.", ex6);
			}
			catch (OperationCanceledException)
			{
				context.Logger.Error("Operation cancelled.");
			}
			catch (ConfuserException)
			{
			}
			catch (Exception ex7)
			{
				context.Logger.ErrorException("Unknown error occurred.", ex7);
			}
			finally
			{
				bool flag = context.Resolver != null;
				if (flag)
				{
					context.Resolver.Clear();
				}
				context.Logger.Finish(ok);
			}
		}

		/// <summary>
		///     Runs the protection pipeline.
		/// </summary>
		/// <param name="pipeline">The protection pipeline.</param>
		/// <param name="context">The context.</param>
		// Token: 0x060000F3 RID: 243 RVA: 0x0000B5D4 File Offset: 0x000097D4
		private static void RunPipeline(ProtectionPipeline pipeline, ConfuserContext context)
		{
			Func<IList<IDnlibDef>> getAllDefs = () => context.Modules.SelectMany((ModuleDefMD module) => module.FindDefinitions()).ToList<IDnlibDef>();
			Func<ModuleDef, IList<IDnlibDef>> getModuleDefs = (ModuleDef module) => module.FindDefinitions().ToList<IDnlibDef>();
			context.CurrentModuleIndex = -1;
			pipeline.ExecuteStage(PipelineStage.Inspection, new Action<ConfuserContext>(ConfuserEngine.Inspection), () => getAllDefs(), context);
			ModuleWriterOptionsBase[] options = new ModuleWriterOptionsBase[context.Modules.Count];
			ModuleWriterListener[] listeners = new ModuleWriterListener[context.Modules.Count];
			Func<IList<IDnlibDef>> <>9__4;
			Func<IList<IDnlibDef>> <>9__5;
			Func<IList<IDnlibDef>> <>9__6;
			Func<IList<IDnlibDef>> <>9__7;
			for (int i = 0; i < context.Modules.Count; i++)
			{
				context.CurrentModuleIndex = i;
				context.CurrentModuleWriterOptions = null;
				context.CurrentModuleWriterListener = null;
				PipelineStage stage = PipelineStage.BeginModule;
				Action<ConfuserContext> func = new Action<ConfuserContext>(ConfuserEngine.BeginModule);
				Func<IList<IDnlibDef>> targets;
				if ((targets = <>9__4) == null)
				{
					targets = (<>9__4 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage, func, targets, context);
				PipelineStage stage2 = PipelineStage.ProcessModule;
				Action<ConfuserContext> func2 = new Action<ConfuserContext>(ConfuserEngine.ProcessModule);
				Func<IList<IDnlibDef>> targets2;
				if ((targets2 = <>9__5) == null)
				{
					targets2 = (<>9__5 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage2, func2, targets2, context);
				PipelineStage stage3 = PipelineStage.OptimizeMethods;
				Action<ConfuserContext> func3 = new Action<ConfuserContext>(ConfuserEngine.OptimizeMethods);
				Func<IList<IDnlibDef>> targets3;
				if ((targets3 = <>9__6) == null)
				{
					targets3 = (<>9__6 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage3, func3, targets3, context);
				PipelineStage stage4 = PipelineStage.EndModule;
				Action<ConfuserContext> func4 = new Action<ConfuserContext>(ConfuserEngine.EndModule);
				Func<IList<IDnlibDef>> targets4;
				if ((targets4 = <>9__7) == null)
				{
					targets4 = (<>9__7 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage4, func4, targets4, context);
				options[i] = context.CurrentModuleWriterOptions;
				listeners[i] = context.CurrentModuleWriterListener;
			}
			Func<IList<IDnlibDef>> <>9__8;
			for (int j = 0; j < context.Modules.Count; j++)
			{
				context.CurrentModuleIndex = j;
				context.CurrentModuleWriterOptions = options[j];
				context.CurrentModuleWriterListener = listeners[j];
				PipelineStage stage5 = PipelineStage.WriteModule;
				Action<ConfuserContext> func5 = new Action<ConfuserContext>(ConfuserEngine.WriteModule);
				Func<IList<IDnlibDef>> targets5;
				if ((targets5 = <>9__8) == null)
				{
					targets5 = (<>9__8 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage5, func5, targets5, context);
				context.OutputModules[j] = context.CurrentModuleOutput;
				context.OutputSymbols[j] = context.CurrentModuleSymbol;
				context.CurrentModuleWriterOptions = null;
				context.CurrentModuleWriterListener = null;
				context.CurrentModuleOutput = null;
				context.CurrentModuleSymbol = null;
			}
			context.CurrentModuleIndex = -1;
			pipeline.ExecuteStage(PipelineStage.Debug, new Action<ConfuserContext>(ConfuserEngine.Debug), () => getAllDefs(), context);
			pipeline.ExecuteStage(PipelineStage.Pack, new Action<ConfuserContext>(ConfuserEngine.Pack), () => getAllDefs(), context);
			pipeline.ExecuteStage(PipelineStage.SaveModules, new Action<ConfuserContext>(ConfuserEngine.SaveModules), () => getAllDefs(), context);
			bool flag = !context.PackerInitiated;
			if (flag)
			{
				context.Logger.Info("Done.");
				NotifyIcon notify = new NotifyIcon();
				notify.Icon = SystemIcons.Information;
				notify.BalloonTipText = "Succesfully protected your modules.";
				notify.BalloonTipTitle = "Beds Protector";
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000B9BC File Offset: 0x00009BBC
		private static void SaveModules(ConfuserContext context)
		{
			context.Resolver.Clear();
			for (int i = 0; i < context.OutputModules.Count; i++)
			{
				string path = Path.GetFullPath(Path.Combine(context.OutputDirectory, context.OutputPaths[i]));
				string dir = Path.GetDirectoryName(path);
				bool flag = !Directory.Exists(dir);
				if (flag)
				{
					Directory.CreateDirectory(dir);
				}
				context.Logger.DebugFormat("Saving to '{0}'...", new object[]
				{
					path
				});
				File.WriteAllBytes(path, context.OutputModules[i]);
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000BA60 File Offset: 0x00009C60
		private static void WriteModule(ConfuserContext context)
		{
			context.Logger.Debug("Almost Done...");
			context.Logger.InfoFormat("Writing module '{0}'...", new object[]
			{
				context.CurrentModule.Name
			});
			MemoryStream pdb = null;
			MemoryStream output = new MemoryStream();
			bool flag = context.CurrentModule.PdbState != null;
			if (flag)
			{
				pdb = new MemoryStream();
				context.CurrentModuleWriterOptions.WritePdb = true;
				context.CurrentModuleWriterOptions.PdbFileName = Path.ChangeExtension(Path.GetFileName(context.OutputPaths[context.CurrentModuleIndex]), "pdb");
				context.CurrentModuleWriterOptions.PdbStream = pdb;
			}
			bool flag2 = context.CurrentModuleWriterOptions is ModuleWriterOptions;
			if (flag2)
			{
				context.CurrentModule.Write(output, (ModuleWriterOptions)context.CurrentModuleWriterOptions);
			}
			else
			{
				context.CurrentModule.NativeWrite(output, (NativeModuleWriterOptions)context.CurrentModuleWriterOptions);
			}
			context.CurrentModuleOutput = output.ToArray();
			bool flag3 = context.CurrentModule.PdbState != null;
			if (flag3)
			{
				context.CurrentModuleSymbol = pdb.ToArray();
			}
		}

		// Token: 0x040000F6 RID: 246
		private static StringProtection StringProtection = new StringProtection();

		// Token: 0x040000F7 RID: 247
		private static readonly string Copyright;

		/// <summary>
		///     The version of ConfuserEx.
		/// </summary>
		// Token: 0x040000F8 RID: 248
		public static readonly string Version;

		// Token: 0x0200002C RID: 44
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060000F6 RID: 246 RVA: 0x000025D9 File Offset: 0x000007D9
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060000F7 RID: 247 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x060000F8 RID: 248 RVA: 0x0000BB84 File Offset: 0x00009D84
			internal Assembly <.cctor>b__0_0(object sender, ResolveEventArgs e)
			{
				Assembly result;
				try
				{
					AssemblyName asmName = new AssemblyName(e.Name);
					foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
					{
						bool flag = asm.GetName().Name == asmName.Name;
						if (flag)
						{
							result = asm;
							return result;
						}
					}
					result = null;
				}
				catch
				{
					result = null;
				}
				return result;
			}

			// Token: 0x060000F9 RID: 249 RVA: 0x0000BC10 File Offset: 0x00009E10
			internal IEnumerable<Tuple<AssemblyRef, ModuleDefMD>> <Inspection>b__6_0(ModuleDefMD module)
			{
				ConfuserEngine.<>c__DisplayClass6_0 CS$<>8__locals1 = new ConfuserEngine.<>c__DisplayClass6_0();
				CS$<>8__locals1.module = module;
				return CS$<>8__locals1.module.GetAssemblyRefs().Select(new Func<AssemblyRef, Tuple<AssemblyRef, ModuleDefMD>>(CS$<>8__locals1.<Inspection>b__1));
			}

			// Token: 0x060000FA RID: 250 RVA: 0x000025E5 File Offset: 0x000007E5
			internal IEnumerable<AssemblyRef> <PrintEnvironmentInfo>b__10_0(ModuleDefMD m)
			{
				return m.GetAssemblyRefs();
			}

			// Token: 0x060000FB RID: 251 RVA: 0x000025ED File Offset: 0x000007ED
			internal IEnumerable<IDnlibDef> <RunPipeline>b__15_1(ModuleDefMD module)
			{
				return module.FindDefinitions();
			}

			// Token: 0x060000FC RID: 252 RVA: 0x000025F5 File Offset: 0x000007F5
			internal IList<IDnlibDef> <RunPipeline>b__15_2(ModuleDef module)
			{
				return module.FindDefinitions().ToList<IDnlibDef>();
			}

			// Token: 0x040000F9 RID: 249
			public static readonly ConfuserEngine.<>c <>9 = new ConfuserEngine.<>c();

			// Token: 0x040000FA RID: 250
			public static Func<ModuleDefMD, IEnumerable<Tuple<AssemblyRef, ModuleDefMD>>> <>9__6_0;

			// Token: 0x040000FB RID: 251
			public static Func<ModuleDefMD, IEnumerable<AssemblyRef>> <>9__10_0;

			// Token: 0x040000FC RID: 252
			public static Func<ModuleDefMD, IEnumerable<IDnlibDef>> <>9__15_1;

			// Token: 0x040000FD RID: 253
			public static Func<ModuleDef, IList<IDnlibDef>> <>9__15_2;
		}

		// Token: 0x0200002D RID: 45
		[CompilerGenerated]
		private sealed class <>c__DisplayClass1_0
		{
			// Token: 0x060000FD RID: 253 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass1_0()
			{
			}

			// Token: 0x060000FE RID: 254 RVA: 0x00002602 File Offset: 0x00000802
			internal void <BeginModule>b__0(object sender, ModuleWriterListenerEventArgs e)
			{
				this.context.CheckCancellation();
			}

			// Token: 0x040000FE RID: 254
			public ConfuserContext context;
		}

		// Token: 0x0200002E RID: 46
		[CompilerGenerated]
		private sealed class <GetFrameworkVersions>d__5 : IEnumerable<string>, IEnumerator<string>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x060000FF RID: 255 RVA: 0x00002611 File Offset: 0x00000811
			[DebuggerHidden]
			public <GetFrameworkVersions>d__5(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x06000100 RID: 256 RVA: 0x0000BC48 File Offset: 0x00009E48
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				int num = this.<>1__state;
				switch (num)
				{
				case -6:
				case 4:
					try
					{
					}
					finally
					{
						this.<>m__Finally4();
					}
					break;
				case -5:
				case -4:
				case -3:
				case 1:
				case 2:
				case 3:
					try
					{
						if (num - -5 <= 1 || num - 1 <= 2)
						{
							try
							{
								if (num == -5 || num - 2 <= 1)
								{
									try
									{
									}
									finally
									{
										this.<>m__Finally3();
									}
								}
							}
							finally
							{
								this.<>m__Finally2();
							}
						}
					}
					finally
					{
						this.<>m__Finally1();
					}
					break;
				}
			}

			// Token: 0x06000101 RID: 257 RVA: 0x0000BD18 File Offset: 0x00009F18
			bool IEnumerator.MoveNext()
			{
				bool result;
				try
				{
					switch (this.<>1__state)
					{
					case 0:
						this.<>1__state = -1;
						registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\");
						this.<>1__state = -3;
						this.<>1__state = -4;
						subKeyNames = registryKey.GetSubKeyNames();
						i = 0;
						goto IL_3AD;
					case 1:
						this.<>1__state = -4;
						break;
					case 2:
						this.<>1__state = -5;
						goto IL_32C;
					case 3:
						this.<>1__state = -5;
						goto IL_32C;
					case 4:
						this.<>1__state = -6;
						this.<>m__Finally4();
						registryKey4 = null;
						return false;
					default:
						return false;
					}
					IL_1AB:
					bool flag = !(text2 != "");
					if (flag)
					{
						this.<>1__state = -5;
						subKeyNames2 = registryKey2.GetSubKeyNames();
						j = 0;
						goto IL_34D;
					}
					goto IL_376;
					IL_32C:
					text3 = null;
					registryKey3 = null;
					int num2 = j;
					j = num2 + 1;
					IL_34D:
					if (j >= subKeyNames2.Length)
					{
						subKeyNames2 = null;
						this.<>m__Finally3();
					}
					else
					{
						text3 = subKeyNames2[j];
						registryKey3 = registryKey2.OpenSubKey(text3);
						text2 = (string)registryKey3.GetValue("Version", "");
						bool flag2 = text2 != "";
						if (flag2)
						{
							a = registryKey3.GetValue("SP", "").ToString();
						}
						a2 = registryKey3.GetValue("Install", "").ToString();
						bool flag3 = a2 == "";
						if (flag3)
						{
							this.<>2__current = text + "  " + text2;
							this.<>1__state = 2;
							return true;
						}
						bool flag4 = a2 == "1";
						if (flag4)
						{
							this.<>2__current = "  " + text3 + "  " + text2;
							this.<>1__state = 3;
							return true;
						}
						goto IL_32C;
					}
					IL_376:
					registryKey2 = null;
					text2 = null;
					a = null;
					a2 = null;
					IL_393:
					text = null;
					num2 = i;
					i = num2 + 1;
					IL_3AD:
					if (i >= subKeyNames.Length)
					{
						subKeyNames = null;
						this.<>m__Finally2();
						this.<>m__Finally1();
						registryKey = null;
						registryKey4 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\");
						this.<>1__state = -6;
						bool flag5 = registryKey4.GetValue("Release") == null;
						if (flag5)
						{
							result = false;
							this.<>m__Finally4();
						}
						else
						{
							num = (int)registryKey4.GetValue("Release");
							this.<>2__current = "v4.5 " + num;
							this.<>1__state = 4;
							result = true;
						}
					}
					else
					{
						text = subKeyNames[i];
						bool flag6 = text.StartsWith("v");
						if (!flag6)
						{
							goto IL_393;
						}
						registryKey2 = registryKey.OpenSubKey(text);
						text2 = (string)registryKey2.GetValue("Version", "");
						a = registryKey2.GetValue("SP", "").ToString();
						a2 = registryKey2.GetValue("Install", "").ToString();
						bool flag7 = a2 == "" || (a != "" && a2 == "1");
						if (!flag7)
						{
							goto IL_1AB;
						}
						this.<>2__current = text + "  " + text2;
						this.<>1__state = 1;
						result = true;
					}
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return result;
			}

			// Token: 0x06000102 RID: 258 RVA: 0x00002631 File Offset: 0x00000831
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				if (registryKey != null)
				{
					((IDisposable)registryKey).Dispose();
				}
			}

			// Token: 0x06000103 RID: 259 RVA: 0x0000264E File Offset: 0x0000084E
			private void <>m__Finally2()
			{
				this.<>1__state = -3;
			}

			// Token: 0x06000104 RID: 260 RVA: 0x0000265A File Offset: 0x0000085A
			private void <>m__Finally3()
			{
				this.<>1__state = -4;
			}

			// Token: 0x06000105 RID: 261 RVA: 0x00002666 File Offset: 0x00000866
			private void <>m__Finally4()
			{
				this.<>1__state = -1;
				if (registryKey4 != null)
				{
					((IDisposable)registryKey4).Dispose();
				}
			}

			// Token: 0x17000002 RID: 2
			// (get) Token: 0x06000106 RID: 262 RVA: 0x00002683 File Offset: 0x00000883
			string IEnumerator<string>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000107 RID: 263 RVA: 0x0000268B File Offset: 0x0000088B
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000003 RID: 3
			// (get) Token: 0x06000108 RID: 264 RVA: 0x00002683 File Offset: 0x00000883
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000109 RID: 265 RVA: 0x0000C1DC File Offset: 0x0000A3DC
			[DebuggerHidden]
			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				ConfuserEngine.<GetFrameworkVersions>d__5 result;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					result = this;
				}
				else
				{
					result = new ConfuserEngine.<GetFrameworkVersions>d__5(0);
				}
				return result;
			}

			// Token: 0x0600010A RID: 266 RVA: 0x00002692 File Offset: 0x00000892
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<System.String>.GetEnumerator();
			}

			// Token: 0x040000FF RID: 255
			private int <>1__state;

			// Token: 0x04000100 RID: 256
			private string <>2__current;

			// Token: 0x04000101 RID: 257
			private int <>l__initialThreadId;

			// Token: 0x04000102 RID: 258
			private RegistryKey <registryKey>5__1;

			// Token: 0x04000103 RID: 259
			private string[] <subKeyNames>5__2;

			// Token: 0x04000104 RID: 260
			private int <i>5__3;

			// Token: 0x04000105 RID: 261
			private string <text>5__4;

			// Token: 0x04000106 RID: 262
			private RegistryKey <registryKey2>5__5;

			// Token: 0x04000107 RID: 263
			private string <text2>5__6;

			// Token: 0x04000108 RID: 264
			private string <a>5__7;

			// Token: 0x04000109 RID: 265
			private string <a2>5__8;

			// Token: 0x0400010A RID: 266
			private string[] <subKeyNames2>5__9;

			// Token: 0x0400010B RID: 267
			private int <j>5__10;

			// Token: 0x0400010C RID: 268
			private string <text3>5__11;

			// Token: 0x0400010D RID: 269
			private RegistryKey <registryKey3>5__12;

			// Token: 0x0400010E RID: 270
			private RegistryKey <registryKey4>5__13;

			// Token: 0x0400010F RID: 271
			private int <num>5__14;
		}

		// Token: 0x0200002F RID: 47
		[CompilerGenerated]
		private sealed class <>c__DisplayClass6_0
		{
			// Token: 0x0600010B RID: 267 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass6_0()
			{
			}

			// Token: 0x0600010C RID: 268 RVA: 0x0000269A File Offset: 0x0000089A
			internal Tuple<AssemblyRef, ModuleDefMD> <Inspection>b__1(AssemblyRef asmRef)
			{
				return Tuple.Create<AssemblyRef, ModuleDefMD>(asmRef, this.module);
			}

			// Token: 0x04000110 RID: 272
			public ModuleDefMD module;
		}

		// Token: 0x02000030 RID: 48
		[CompilerGenerated]
		private sealed class <>c__DisplayClass13_0
		{
			// Token: 0x0600010D RID: 269 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass13_0()
			{
			}

			// Token: 0x0600010E RID: 270 RVA: 0x000026A8 File Offset: 0x000008A8
			internal void <Run>b__0()
			{
				ConfuserEngine.RunInternal(this.parameters, this.token.Value);
			}

			// Token: 0x04000111 RID: 273
			public ConfuserParameters parameters;

			// Token: 0x04000112 RID: 274
			public CancellationToken? token;
		}

		// Token: 0x02000031 RID: 49
		[CompilerGenerated]
		private sealed class <>c__DisplayClass15_0
		{
			// Token: 0x0600010F RID: 271 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass15_0()
			{
			}

			// Token: 0x06000110 RID: 272 RVA: 0x000026C2 File Offset: 0x000008C2
			internal IList<IDnlibDef> <RunPipeline>b__0()
			{
				return this.context.Modules.SelectMany(new Func<ModuleDefMD, IEnumerable<IDnlibDef>>(ConfuserEngine.<>c.<>9.<RunPipeline>b__15_1)).ToList<IDnlibDef>();
			}

			// Token: 0x06000111 RID: 273 RVA: 0x000026F8 File Offset: 0x000008F8
			internal IList<IDnlibDef> <RunPipeline>b__3()
			{
				return this.getAllDefs();
			}

			// Token: 0x06000112 RID: 274 RVA: 0x00002705 File Offset: 0x00000905
			internal IList<IDnlibDef> <RunPipeline>b__4()
			{
				return this.getModuleDefs(this.context.CurrentModule);
			}

			// Token: 0x06000113 RID: 275 RVA: 0x00002705 File Offset: 0x00000905
			internal IList<IDnlibDef> <RunPipeline>b__5()
			{
				return this.getModuleDefs(this.context.CurrentModule);
			}

			// Token: 0x06000114 RID: 276 RVA: 0x00002705 File Offset: 0x00000905
			internal IList<IDnlibDef> <RunPipeline>b__6()
			{
				return this.getModuleDefs(this.context.CurrentModule);
			}

			// Token: 0x06000115 RID: 277 RVA: 0x00002705 File Offset: 0x00000905
			internal IList<IDnlibDef> <RunPipeline>b__7()
			{
				return this.getModuleDefs(this.context.CurrentModule);
			}

			// Token: 0x06000116 RID: 278 RVA: 0x00002705 File Offset: 0x00000905
			internal IList<IDnlibDef> <RunPipeline>b__8()
			{
				return this.getModuleDefs(this.context.CurrentModule);
			}

			// Token: 0x06000117 RID: 279 RVA: 0x000026F8 File Offset: 0x000008F8
			internal IList<IDnlibDef> <RunPipeline>b__9()
			{
				return this.getAllDefs();
			}

			// Token: 0x06000118 RID: 280 RVA: 0x000026F8 File Offset: 0x000008F8
			internal IList<IDnlibDef> <RunPipeline>b__10()
			{
				return this.getAllDefs();
			}

			// Token: 0x06000119 RID: 281 RVA: 0x000026F8 File Offset: 0x000008F8
			internal IList<IDnlibDef> <RunPipeline>b__11()
			{
				return this.getAllDefs();
			}

			// Token: 0x04000113 RID: 275
			public ConfuserContext context;

			// Token: 0x04000114 RID: 276
			public Func<IList<IDnlibDef>> getAllDefs;

			// Token: 0x04000115 RID: 277
			public Func<ModuleDef, IList<IDnlibDef>> getModuleDefs;

			// Token: 0x04000116 RID: 278
			public Func<IList<IDnlibDef>> <>9__4;

			// Token: 0x04000117 RID: 279
			public Func<IList<IDnlibDef>> <>9__5;

			// Token: 0x04000118 RID: 280
			public Func<IList<IDnlibDef>> <>9__6;

			// Token: 0x04000119 RID: 281
			public Func<IList<IDnlibDef>> <>9__7;

			// Token: 0x0400011A RID: 282
			public Func<IList<IDnlibDef>> <>9__8;
		}
	}
}
