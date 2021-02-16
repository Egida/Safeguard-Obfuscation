using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.Renamer.Analyzers;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000006 RID: 6
	internal class NameService : INameService
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00004560 File Offset: 0x00002760
		public NameService(ConfuserContext context)
		{
			this.context = context;
			this.storage = new VTableStorage(context.Logger);
			this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.Rename");
			this.nameSeed = this.random.NextBytes(20);
			this.Renamers = new List<IRenamer>
			{
				new InterReferenceAnalyzer(),
				new VTableAnalyzer(),
				new TypeBlobAnalyzer(),
				new ResourceAnalyzer(),
				new LdtokenEnumAnalyzer()
			};
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000020EB File Offset: 0x000002EB
		// (set) Token: 0x06000028 RID: 40 RVA: 0x000020F3 File Offset: 0x000002F3
		public IList<IRenamer> Renamers
		{
			[CompilerGenerated]
			get
			{
				return this.<Renamers>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Renamers>k__BackingField = value;
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00004628 File Offset: 0x00002828
		public VTableStorage GetVTables()
		{
			return this.storage;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00004640 File Offset: 0x00002840
		public bool CanRename(object obj)
		{
			bool flag = obj is IDnlibDef;
			bool result;
			if (flag)
			{
				bool flag2 = this.analyze == null;
				if (flag2)
				{
					this.analyze = this.context.Pipeline.FindPhase<AnalyzePhase>();
				}
				NameProtection prot = (NameProtection)this.analyze.Parent;
				ProtectionSettings parameters = ProtectionParameters.GetParameters(this.context, (IDnlibDef)obj);
				result = (parameters != null && parameters.ContainsKey(prot) && this.context.Annotations.Get<bool>(obj, NameService.CanRenameKey, true));
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000020FC File Offset: 0x000002FC
		public void SetCanRename(object obj, bool val)
		{
			this.context.Annotations.Set<bool>(obj, NameService.CanRenameKey, val);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000046D8 File Offset: 0x000028D8
		public RenameMode GetRenameMode(object obj)
		{
			return this.context.Annotations.Get<RenameMode>(obj, NameService.RenameModeKey, RenameMode.Sequential);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002117 File Offset: 0x00000317
		public void SetRenameMode(object obj, RenameMode val)
		{
			this.context.Annotations.Set<RenameMode>(obj, NameService.RenameModeKey, val);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00004704 File Offset: 0x00002904
		public void ReduceRenameMode(object obj, RenameMode val)
		{
			RenameMode original = this.GetRenameMode(obj);
			bool flag = original < val;
			if (flag)
			{
				this.context.Annotations.Set<RenameMode>(obj, NameService.RenameModeKey, val);
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000473C File Offset: 0x0000293C
		public void AddReference<T>(T obj, INameReference<T> reference)
		{
			this.context.Annotations.GetOrCreate<List<INameReference>>(obj, NameService.ReferencesKey, (object key) => new List<INameReference>()).Add(reference);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000478C File Offset: 0x0000298C
		public void Analyze(IDnlibDef def)
		{
			bool flag = this.analyze == null;
			if (flag)
			{
				this.analyze = this.context.Pipeline.FindPhase<AnalyzePhase>();
			}
			this.SetOriginalName(def, def.Name);
			bool flag2 = def is TypeDef;
			if (flag2)
			{
				this.GetVTables().GetVTable((TypeDef)def);
				this.SetOriginalNamespace(def, ((TypeDef)def).Namespace);
			}
			this.analyze.Analyze(this, this.context, ProtectionParameters.Empty, def, true);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00004828 File Offset: 0x00002A28
		private void IncrementNameId()
		{
			for (int i = this.nameId.Length - 1; i >= 0; i--)
			{
				byte[] expr_19_cp_0 = this.nameId;
				int expr_19_cp_ = i;
				byte[] array = expr_19_cp_0;
				int num = expr_19_cp_;
				array[num] += 1;
				bool flag = this.nameId[i] > 0;
				if (flag)
				{
					break;
				}
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00004880 File Offset: 0x00002A80
		public string ObfuscateName(string name, RenameMode mode)
		{
			bool flag = string.IsNullOrEmpty(name);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = mode == RenameMode.Empty;
				if (flag2)
				{
					result = "";
				}
				else
				{
					bool flag3 = mode == RenameMode.Debug;
					if (flag3)
					{
						result = "_" + name;
					}
					else
					{
						byte[] buff = Utils.Xor(Utils.SHA1(Encoding.UTF8.GetBytes(name)), this.nameSeed);
						switch (mode)
						{
						case RenameMode.Empty:
							result = "IOCancellationHelper";
							break;
						case RenameMode.Unicode:
							result = Utils.EncodeString(buff, NameService.unicodeCharset) + "MemoryMappedViewStream‮";
							break;
						case RenameMode.ASCII:
							result = Utils.EncodeString(buff, NameService.asciiCharset);
							break;
						case RenameMode.Letters:
							result = Utils.EncodeString(buff, NameService.letterCharset);
							break;
						default:
							switch (mode)
							{
							case RenameMode.Decodable:
							{
								bool flag4 = this.nameMap1.ContainsKey(name);
								if (flag4)
								{
									result = this.nameMap1[name];
								}
								else
								{
									this.IncrementNameId();
									string text = "_" + Utils.EncodeString(buff, NameService.alphaNumCharset) + "_";
									this.nameMap2[text] = name;
									this.nameMap1[name] = text;
									result = text;
								}
								break;
							}
							case RenameMode.Sequential:
							{
								bool flag5 = this.nameMap1.ContainsKey(name);
								if (flag5)
								{
									result = this.nameMap1[name];
								}
								else
								{
									this.IncrementNameId();
									string text2 = string.Concat(new string[]
									{
										"WindowsFoundationEventHandler`1",
										Utils.EncodeString(this.nameId, NameService.alphaNumCharset),
										"ICustomPropertyProvider",
										Utils.EncodeString(this.nameId, NameService.alphaNumCharset),
										"ICustomPropertyProviderProxy`2",
										Utils.EncodeString(this.nameId, NameService.alphaNumCharset),
										"WindowsRuntimeBufferHelper",
										Utils.EncodeString(this.nameId, NameService.alphaNumCharset),
										"ICustomPropertyProviderImpl"
									});
									this.nameMap2[text2] = name;
									this.nameMap1[name] = text2;
									result = text2;
								}
								break;
							}
							case RenameMode.Reversible:
							{
								bool flag6 = this.reversibleRenamer == null;
								if (flag6)
								{
									throw new ArgumentException("Password not provided for reversible renaming.");
								}
								result = this.reversibleRenamer.Encrypt(name);
								break;
							}
							default:
								throw new NotSupportedException("Rename mode '" + mode + "' is not supported.");
							}
							break;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004AFC File Offset: 0x00002CFC
		public string RandomName()
		{
			return this.RandomName(RenameMode.Sequential);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004B18 File Offset: 0x00002D18
		public string RandomName(RenameMode mode)
		{
			return this.ObfuscateName(Utils.ToHexString(this.random.NextBytes(16)), mode);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002132 File Offset: 0x00000332
		public void SetOriginalName(object obj, string name)
		{
			this.context.Annotations.Set<string>(obj, NameService.OriginalNameKey, name);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000214D File Offset: 0x0000034D
		public void SetOriginalNamespace(object obj, string ns)
		{
			this.context.Annotations.Set<string>(obj, NameService.OriginalNamespaceKey, ns);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002168 File Offset: 0x00000368
		public void RegisterRenamer(IRenamer renamer)
		{
			this.Renamers.Add(renamer);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00004B44 File Offset: 0x00002D44
		public T FindRenamer<T>()
		{
			return this.Renamers.OfType<T>().Single<T>();
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00004B68 File Offset: 0x00002D68
		public void MarkHelper(IDnlibDef def, IMarkerService marker, ConfuserComponent parentComp)
		{
			bool flag = marker.IsMarked(def);
			if (!flag)
			{
				bool flag2 = def is MethodDef;
				if (flag2)
				{
					MethodDef methodDef = (MethodDef)def;
					methodDef.Access = MethodAttributes.Assembly;
					bool flag3 = !methodDef.IsSpecialName && !methodDef.IsRuntimeSpecialName && !methodDef.DeclaringType.IsDelegate();
					if (flag3)
					{
						methodDef.Name = this.RandomName();
					}
				}
				else
				{
					bool flag4 = def is FieldDef;
					if (flag4)
					{
						FieldDef fieldDef = (FieldDef)def;
						fieldDef.Access = FieldAttributes.Assembly;
						bool flag5 = !fieldDef.IsSpecialName && !fieldDef.IsRuntimeSpecialName;
						if (flag5)
						{
							fieldDef.Name = this.RandomName();
						}
					}
					else
					{
						bool flag6 = def is TypeDef;
						if (flag6)
						{
							TypeDef typeDef = (TypeDef)def;
							typeDef.Visibility = ((typeDef.DeclaringType == null) ? TypeAttributes.NotPublic : TypeAttributes.NestedAssembly);
							typeDef.Namespace = "CounterCreationDataCollection";
							bool flag7 = !typeDef.IsSpecialName && !typeDef.IsRuntimeSpecialName;
							if (flag7)
							{
								typeDef.Name = this.RandomName();
							}
						}
					}
				}
				this.SetCanRename(def, false);
				this.Analyze(def);
				marker.Mark(def, parentComp);
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00004CC4 File Offset: 0x00002EC4
		public RandomGenerator GetRandom()
		{
			return this.random;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004CDC File Offset: 0x00002EDC
		public IList<INameReference> GetReferences(object obj)
		{
			return this.context.Annotations.GetLazy<List<INameReference>>(obj, NameService.ReferencesKey, (object key) => new List<INameReference>());
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00004D24 File Offset: 0x00002F24
		public string GetOriginalName(object obj)
		{
			return this.context.Annotations.Get<string>(obj, NameService.OriginalNameKey, "");
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004D54 File Offset: 0x00002F54
		public string GetOriginalNamespace(object obj)
		{
			return this.context.Annotations.Get<string>(obj, NameService.OriginalNamespaceKey, "");
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004D84 File Offset: 0x00002F84
		public ICollection<KeyValuePair<string, string>> GetNameMap()
		{
			return this.nameMap2;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004D9C File Offset: 0x00002F9C
		// Note: this type is marked as 'beforefieldinit'.
		static NameService()
		{
		}

		// Token: 0x04000003 RID: 3
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<IRenamer> <Renamers>k__BackingField;

		// Token: 0x04000004 RID: 4
		private static readonly object CanRenameKey = new object();

		// Token: 0x04000005 RID: 5
		private static readonly object RenameModeKey = new object();

		// Token: 0x04000006 RID: 6
		private static readonly object ReferencesKey = new object();

		// Token: 0x04000007 RID: 7
		private static readonly object OriginalNameKey = new object();

		// Token: 0x04000008 RID: 8
		private static readonly object OriginalNamespaceKey = new object();

		// Token: 0x04000009 RID: 9
		private readonly ConfuserContext context;

		// Token: 0x0400000A RID: 10
		private readonly byte[] nameSeed;

		// Token: 0x0400000B RID: 11
		private readonly RandomGenerator random;

		// Token: 0x0400000C RID: 12
		private readonly VTableStorage storage;

		// Token: 0x0400000D RID: 13
		private AnalyzePhase analyze;

		// Token: 0x0400000E RID: 14
		private readonly byte[] nameId = new byte[8];

		// Token: 0x0400000F RID: 15
		private readonly Dictionary<string, string> nameMap1 = new Dictionary<string, string>();

		// Token: 0x04000010 RID: 16
		private readonly Dictionary<string, string> nameMap2 = new Dictionary<string, string>();

		// Token: 0x04000011 RID: 17
		internal ReversibleRenamer reversibleRenamer;

		// Token: 0x04000012 RID: 18
		private static readonly char[] asciiCharset = (from ord in Enumerable.Range(32, 95)
		select (char)ord).Except(new char[]
		{
			'.'
		}).ToArray<char>();

		// Token: 0x04000013 RID: 19
		private static readonly char[] letterCharset = Enumerable.Range(0, 26).SelectMany((int ord) => new char[]
		{
			(char)(97 + ord),
			(char)(65 + ord)
		}).ToArray<char>();

		// Token: 0x04000014 RID: 20
		private static readonly char[] alphaNumCharset = Enumerable.Range(0, 26).SelectMany((int ord) => new char[]
		{
			(char)(97 + ord),
			(char)(65 + ord)
		}).Concat(from ord in Enumerable.Range(0, 10)
		select (char)(48 + ord)).ToArray<char>();

		// Token: 0x04000015 RID: 21
		private static readonly char[] unicodeCharset = new char[0].Concat(from ord in Enumerable.Range(8203, 5)
		select (char)ord).Concat(from ord in Enumerable.Range(8233, 6)
		select (char)ord).Concat(from ord in Enumerable.Range(8298, 6)
		select (char)ord).Except(new char[]
		{
			'\u2029'
		}).ToArray<char>();

		// Token: 0x02000007 RID: 7
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c__11<T>
		{
			// Token: 0x06000040 RID: 64 RVA: 0x00002178 File Offset: 0x00000378
			// Note: this type is marked as 'beforefieldinit'.
			static <>c__11()
			{
			}

			// Token: 0x06000041 RID: 65 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__11()
			{
			}

			// Token: 0x06000042 RID: 66 RVA: 0x0000218D File Offset: 0x0000038D
			internal List<INameReference> <AddReference>b__11_0(object key)
			{
				return new List<INameReference>();
			}

			// Token: 0x04000016 RID: 22
			public static readonly NameService.<>c__11<T> <>9 = new NameService.<>c__11<T>();

			// Token: 0x04000017 RID: 23
			public static Func<object, List<INameReference>> <>9__11_0;
		}

		// Token: 0x02000008 RID: 8
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000043 RID: 67 RVA: 0x00002194 File Offset: 0x00000394
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000044 RID: 68 RVA: 0x00002184 File Offset: 0x00000384
			public <>c()
			{
			}

			// Token: 0x06000045 RID: 69 RVA: 0x0000218D File Offset: 0x0000038D
			internal List<INameReference> <GetReferences>b__23_0(object key)
			{
				return new List<INameReference>();
			}

			// Token: 0x06000046 RID: 70 RVA: 0x000021A0 File Offset: 0x000003A0
			internal char <.cctor>b__45_0(int ord)
			{
				return (char)ord;
			}

			// Token: 0x06000047 RID: 71 RVA: 0x000021A4 File Offset: 0x000003A4
			internal IEnumerable<char> <.cctor>b__45_1(int ord)
			{
				return new char[]
				{
					(char)(97 + ord),
					(char)(65 + ord)
				};
			}

			// Token: 0x06000048 RID: 72 RVA: 0x000021A4 File Offset: 0x000003A4
			internal IEnumerable<char> <.cctor>b__45_2(int ord)
			{
				return new char[]
				{
					(char)(97 + ord),
					(char)(65 + ord)
				};
			}

			// Token: 0x06000049 RID: 73 RVA: 0x000021BC File Offset: 0x000003BC
			internal char <.cctor>b__45_3(int ord)
			{
				return (char)(48 + ord);
			}

			// Token: 0x0600004A RID: 74 RVA: 0x000021A0 File Offset: 0x000003A0
			internal char <.cctor>b__45_4(int ord)
			{
				return (char)ord;
			}

			// Token: 0x0600004B RID: 75 RVA: 0x000021A0 File Offset: 0x000003A0
			internal char <.cctor>b__45_5(int ord)
			{
				return (char)ord;
			}

			// Token: 0x0600004C RID: 76 RVA: 0x000021A0 File Offset: 0x000003A0
			internal char <.cctor>b__45_6(int ord)
			{
				return (char)ord;
			}

			// Token: 0x04000018 RID: 24
			public static readonly NameService.<>c <>9 = new NameService.<>c();

			// Token: 0x04000019 RID: 25
			public static Func<object, List<INameReference>> <>9__23_0;
		}
	}
}
