using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Confuser.Core.Project;
using Confuser.Core.Project.Patterns;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Resolves and marks the modules with protection settings according to the rules.
	/// </summary>
	// Token: 0x0200004B RID: 75
	public class Marker
	{
		/// <summary>
		///     Applies the rules to the target definition.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="target">The target definition.</param>
		/// <param name="rules">The rules.</param>
		/// <param name="baseSettings">The base settings.</param>
		// Token: 0x060001C6 RID: 454 RVA: 0x0000DFB4 File Offset: 0x0000C1B4
		protected void ApplyRules(ConfuserContext context, IDnlibDef target, Dictionary<Rule, PatternExpression> rules, ProtectionSettings baseSettings = null)
		{
			ProtectionSettings ret = (baseSettings == null) ? new ProtectionSettings() : new ProtectionSettings(baseSettings);
			foreach (KeyValuePair<Rule, PatternExpression> i in rules)
			{
				bool flag = (bool)i.Value.Evaluate(target);
				if (flag)
				{
					bool flag2 = !i.Key.Inherit;
					if (flag2)
					{
						ret.Clear();
					}
					this.FillPreset(i.Key.Preset, ret);
					foreach (SettingItem<Protection> prot in i.Key)
					{
						bool flag3 = prot.Action == SettingItemAction.Add;
						if (flag3)
						{
							ret[this.protections[prot.Id]] = new Dictionary<string, string>(prot, StringComparer.OrdinalIgnoreCase);
						}
						else
						{
							ret.Remove(this.protections[prot.Id]);
						}
					}
				}
			}
			ProtectionParameters.SetParameters(context, target, ret);
		}

		/// <summary>
		///     Fills the protection settings with the specified preset.
		/// </summary>
		/// <param name="preset">The preset.</param>
		/// <param name="settings">The settings.</param>
		// Token: 0x060001C7 RID: 455 RVA: 0x0000E108 File Offset: 0x0000C308
		private void FillPreset(ProtectionPreset preset, ProtectionSettings settings)
		{
			foreach (Protection prot in this.protections.Values)
			{
				bool flag = prot.Preset <= preset && !settings.ContainsKey(prot);
				if (flag)
				{
					settings.Add(prot, new Dictionary<string, string>());
				}
			}
		}

		/// <summary>
		///     Initalizes the Marker with specified protections and packers.
		/// </summary>
		/// <param name="protections">The protections.</param>
		/// <param name="packers">The packers.</param>
		// Token: 0x060001C8 RID: 456 RVA: 0x0000E188 File Offset: 0x0000C388
		public virtual void Initalize(IList<Protection> protections, IList<Packer> packers)
		{
			this.protections = protections.ToDictionary((Protection prot) => prot.Id, (Protection prot) => prot, StringComparer.OrdinalIgnoreCase);
			this.packers = packers.ToDictionary((Packer packer) => packer.Id, (Packer packer) => packer, StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		///     Loads the Strong Name Key at the specified path with a optional password.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="path">The path to the key.</param>
		/// <param name="pass">
		///     The password of the certificate at <paramref name="path" /> if
		///     it is a pfx file; otherwise, <c>null</c>.
		/// </param>
		/// <returns>The loaded Strong Name Key.</returns>
		// Token: 0x060001C9 RID: 457 RVA: 0x0000E234 File Offset: 0x0000C434
		public static StrongNameKey LoadSNKey(ConfuserContext context, string path, string pass)
		{
			bool flag = path == null;
			StrongNameKey result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				StrongNameKey result;
				try
				{
					bool flag2 = pass != null;
					if (flag2)
					{
						X509Certificate2 cert = new X509Certificate2();
						cert.Import(path, pass, X509KeyStorageFlags.Exportable);
						RSACryptoServiceProvider rsa = cert.PrivateKey as RSACryptoServiceProvider;
						bool flag3 = rsa == null;
						if (flag3)
						{
							throw new ArgumentException("RSA key does not present in the certificate.", "path");
						}
						result = new StrongNameKey(rsa.ExportCspBlob(true));
					}
					else
					{
						result = new StrongNameKey(path);
					}
				}
				catch (Exception ex)
				{
					context.Logger.ErrorException("Cannot load the Strong Name Key located at: " + path, ex);
					throw new ConfuserException(ex);
				}
				result2 = result;
			}
			return result2;
		}

		/// <summary>
		///     Marks the member definition.
		/// </summary>
		/// <param name="member">The member definition.</param>
		/// <param name="context">The working context.</param>
		// Token: 0x060001CA RID: 458 RVA: 0x0000E2F0 File Offset: 0x0000C4F0
		protected internal virtual void MarkMember(IDnlibDef member, ConfuserContext context)
		{
			ModuleDef module = ((IMemberRef)member).Module;
			Dictionary<Rule, PatternExpression> rules = context.Annotations.Get<Dictionary<Rule, PatternExpression>>(module, Marker.RulesKey, null);
			this.ApplyRules(context, member, rules, null);
		}

		/// <summary>
		///     Loads the assembly and marks the project.
		/// </summary>
		/// <param name="proj">The project.</param>
		/// <param name="context">The working context.</param>
		/// <returns><see cref="T:Confuser.Core.MarkerResult" /> storing the marked modules and packer information.</returns>
		// Token: 0x060001CB RID: 459 RVA: 0x0000E328 File Offset: 0x0000C528
		protected internal virtual MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
		{
			Packer packer = null;
			Dictionary<string, string> packerParams = null;
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
				bool debug = proj.Debug;
				if (debug)
				{
					context.Logger.Warn("Generated Debug symbols might not be usable with packers!");
				}
				packer = this.packers[proj.Packer.Id];
				packerParams = new Dictionary<string, string>(proj.Packer, StringComparer.OrdinalIgnoreCase);
			}
			List<Tuple<ProjectModule, ModuleDefMD>> modules = new List<Tuple<ProjectModule, ModuleDefMD>>();
			List<byte[]> extModules = new List<byte[]>();
			foreach (ProjectModule module3 in proj)
			{
				bool isExternal = module3.IsExternal;
				if (isExternal)
				{
					extModules.Add(module3.LoadRaw(proj.BaseDirectory));
				}
				else
				{
					ModuleDefMD modDef = module3.Resolve(proj.BaseDirectory, context.Resolver.DefaultModuleContext);
					context.CheckCancellation();
					bool debug2 = proj.Debug;
					if (debug2)
					{
						modDef.LoadPdb();
					}
					context.Resolver.AddToCache(modDef);
					modules.Add(Tuple.Create<ProjectModule, ModuleDefMD>(module3, modDef));
				}
			}
			foreach (Tuple<ProjectModule, ModuleDefMD> module4 in modules)
			{
				context.Logger.InfoFormat("Loading '{0}'...", new object[]
				{
					module4.Item1.Path
				});
				Dictionary<Rule, PatternExpression> rules = this.ParseRules(proj, module4.Item1, context);
				context.Annotations.Set<StrongNameKey>(module4.Item2, Marker.SNKey, Marker.LoadSNKey(context, (module4.Item1.SNKeyPath == null) ? null : Path.Combine(proj.BaseDirectory, module4.Item1.SNKeyPath), module4.Item1.SNKeyPassword));
				context.Annotations.Set<Dictionary<Rule, PatternExpression>>(module4.Item2, Marker.RulesKey, rules);
				foreach (IDnlibDef def in module4.Item2.FindDefinitions())
				{
					this.ApplyRules(context, def, rules, null);
					context.CheckCancellation();
				}
				bool flag3 = packerParams != null;
				if (flag3)
				{
					ProtectionParameters.GetParameters(context, module4.Item2)[packer] = packerParams;
				}
			}
			return new MarkerResult((from module in modules
			select module.Item2).ToList<ModuleDefMD>(), packer, extModules);
		}

		/// <summary>
		///     Parses the rules' patterns.
		/// </summary>
		/// <param name="proj">The project.</param>
		/// <param name="module">The module description.</param>
		/// <param name="context">The working context.</param>
		/// <returns>Parsed rule patterns.</returns>
		/// <exception cref="T:System.ArgumentException">
		///     One of the rules has invalid pattern.
		/// </exception>
		// Token: 0x060001CC RID: 460 RVA: 0x0000E66C File Offset: 0x0000C86C
		protected Dictionary<Rule, PatternExpression> ParseRules(ConfuserProject proj, ProjectModule module, ConfuserContext context)
		{
			Dictionary<Rule, PatternExpression> ret = new Dictionary<Rule, PatternExpression>();
			PatternParser parser = new PatternParser();
			foreach (Rule rule in proj.Rules.Concat(module.Rules))
			{
				try
				{
					ret.Add(rule, parser.Parse(rule.Pattern));
				}
				catch (InvalidPatternException ex)
				{
					context.Logger.ErrorFormat("Invalid rule pattern: " + rule.Pattern + ".", new object[]
					{
						ex
					});
					throw new ConfuserException(ex);
				}
				foreach (SettingItem<Protection> setting in rule)
				{
					bool flag = !this.protections.ContainsKey(setting.Id);
					if (flag)
					{
						context.Logger.ErrorFormat("Cannot find protection with ID '{0}'.", new object[]
						{
							setting.Id
						});
						throw new ConfuserException(null);
					}
				}
			}
			return ret;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00002194 File Offset: 0x00000394
		public Marker()
		{
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00002C24 File Offset: 0x00000E24
		// Note: this type is marked as 'beforefieldinit'.
		static Marker()
		{
		}

		/// <summary>
		///     The packers available to use.
		/// </summary>
		// Token: 0x04000182 RID: 386
		protected Dictionary<string, Packer> packers;

		/// <summary>
		///     The protections available to use.
		/// </summary>
		// Token: 0x04000183 RID: 387
		protected Dictionary<string, Protection> protections;

		/// <summary>
		///     Annotation key of rules.
		/// </summary>
		// Token: 0x04000184 RID: 388
		public static readonly object RulesKey = new object();

		/// <summary>
		///     Annotation key of Strong Name Key.
		/// </summary>
		// Token: 0x04000185 RID: 389
		public static readonly object SNKey = new object();

		// Token: 0x0200004C RID: 76
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060001CF RID: 463 RVA: 0x00002C3A File Offset: 0x00000E3A
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060001D0 RID: 464 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x060001D1 RID: 465 RVA: 0x00002C46 File Offset: 0x00000E46
			internal string <Initalize>b__2_0(Protection prot)
			{
				return prot.Id;
			}

			// Token: 0x060001D2 RID: 466 RVA: 0x00002942 File Offset: 0x00000B42
			internal Protection <Initalize>b__2_1(Protection prot)
			{
				return prot;
			}

			// Token: 0x060001D3 RID: 467 RVA: 0x00002C46 File Offset: 0x00000E46
			internal string <Initalize>b__2_2(Packer packer)
			{
				return packer.Id;
			}

			// Token: 0x060001D4 RID: 468 RVA: 0x00002942 File Offset: 0x00000B42
			internal Packer <Initalize>b__2_3(Packer packer)
			{
				return packer;
			}

			// Token: 0x060001D5 RID: 469 RVA: 0x00002C4E File Offset: 0x00000E4E
			internal ModuleDefMD <MarkProject>b__5_0(Tuple<ProjectModule, ModuleDefMD> module)
			{
				return module.Item2;
			}

			// Token: 0x04000186 RID: 390
			public static readonly Marker.<>c <>9 = new Marker.<>c();

			// Token: 0x04000187 RID: 391
			public static Func<Protection, string> <>9__2_0;

			// Token: 0x04000188 RID: 392
			public static Func<Protection, Protection> <>9__2_1;

			// Token: 0x04000189 RID: 393
			public static Func<Packer, string> <>9__2_2;

			// Token: 0x0400018A RID: 394
			public static Func<Packer, Packer> <>9__2_3;

			// Token: 0x0400018B RID: 395
			public static Func<Tuple<ProjectModule, ModuleDefMD>, ModuleDefMD> <>9__5_0;
		}
	}
}
