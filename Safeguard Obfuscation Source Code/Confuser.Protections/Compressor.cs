using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Protections.Compress;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using dnlib.PE;

namespace Confuser.Protections
{
	// Token: 0x02000006 RID: 6
	internal class Compressor : Packer
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000062C8 File Offset: 0x000044C8
		public override string Name
		{
			get
			{
				return "Compressing Packer";
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000062E0 File Offset: 0x000044E0
		public override string Description
		{
			get
			{
				return "This packer reduces the size of output.";
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000062F8 File Offset: 0x000044F8
		public override string Id
		{
			get
			{
				return "compressor";
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00006310 File Offset: 0x00004510
		public override string FullId
		{
			get
			{
				return "Ki.Compressor";
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00004A83 File Offset: 0x00002C83
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.WriteModule, new ExtractPhase(this));
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00006328 File Offset: 0x00004528
		protected override void Pack(ConfuserContext context, ProtectionParameters parameters)
		{
			CompressorContext ctx = context.Annotations.Get<CompressorContext>(context, Compressor.ContextKey, null);
			bool flag = ctx == null;
			if (flag)
			{
				context.Logger.Error("No executable module!");
				throw new ConfuserException(null);
			}
			ModuleDefMD originModule = context.Modules[ctx.ModuleIndex];
			ctx.OriginModuleDef = originModule;
			ModuleDefUser stubModule = new ModuleDefUser(ctx.ModuleName, originModule.Mvid, originModule.CorLibTypes.AssemblyRef);
			bool compatMode = ctx.CompatMode;
			if (compatMode)
			{
				AssemblyDefUser assembly = new AssemblyDefUser(originModule.Assembly);
				AssemblyDefUser assemblyDefUser = assembly;
				assemblyDefUser.Name += ".cr";
				assembly.Modules.Add(stubModule);
			}
			else
			{
				ctx.Assembly.Modules.Insert(0, stubModule);
				this.ImportAssemblyTypeReferences(originModule, stubModule);
			}
			stubModule.Characteristics = originModule.Characteristics;
			stubModule.Cor20HeaderFlags = originModule.Cor20HeaderFlags;
			stubModule.Cor20HeaderRuntimeVersion = originModule.Cor20HeaderRuntimeVersion;
			stubModule.DllCharacteristics = originModule.DllCharacteristics;
			stubModule.EncBaseId = originModule.EncBaseId;
			stubModule.EncId = originModule.EncId;
			stubModule.Generation = originModule.Generation;
			stubModule.Kind = ctx.Kind;
			stubModule.Machine = originModule.Machine;
			stubModule.RuntimeVersion = originModule.RuntimeVersion;
			stubModule.TablesHeaderVersion = originModule.TablesHeaderVersion;
			stubModule.Win32Resources = originModule.Win32Resources;
			this.InjectStub(context, ctx, parameters, stubModule);
			StrongNameKey snKey = context.Annotations.Get<StrongNameKey>(originModule, Marker.SNKey, null);
			using (MemoryStream ms = new MemoryStream())
			{
				stubModule.Write(ms, new ModuleWriterOptions(stubModule, new Compressor.KeyInjector(ctx))
				{
					StrongNameKey = snKey
				});
				context.CheckCancellation();
				base.ProtectStub(context, context.OutputPaths[ctx.ModuleIndex], ms.ToArray(), snKey, new StubProtection(ctx, originModule));
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00006540 File Offset: 0x00004740
		private static string GetId(byte[] module)
		{
			IMetaData md = MetaDataCreator.CreateMetaData(new PEImage(module));
			RawAssemblyRow assemblyRow = md.TablesStream.ReadAssemblyRow(1U);
			return Compressor.GetId(new AssemblyNameInfo
			{
				Name = md.StringsStream.ReadNoNull(assemblyRow.Name),
				Culture = md.StringsStream.ReadNoNull(assemblyRow.Locale),
				PublicKeyOrToken = new PublicKey(md.BlobStream.Read(assemblyRow.PublicKey)),
				HashAlgId = (AssemblyHashAlgorithm)assemblyRow.HashAlgId,
				Version = new Version((int)assemblyRow.MajorVersion, (int)assemblyRow.MinorVersion, (int)assemblyRow.BuildNumber, (int)assemblyRow.RevisionNumber),
				Attributes = (AssemblyAttributes)assemblyRow.Flags
			});
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00006604 File Offset: 0x00004804
		private static string GetId(IAssembly assembly)
		{
			return new AssemblyName(assembly.FullName).FullName.ToUpperInvariant();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000662C File Offset: 0x0000482C
		private void PackModules(ConfuserContext context, CompressorContext compCtx, ModuleDef stubModule, ICompressionService comp, RandomGenerator random)
		{
			int maxLen = 0;
			Dictionary<string, byte[]> modules = new Dictionary<string, byte[]>();
			for (int i = 0; i < context.OutputModules.Count; i++)
			{
				bool flag = i == compCtx.ModuleIndex;
				if (!flag)
				{
					string id = Compressor.GetId(context.Modules[i].Assembly);
					modules.Add(id, context.OutputModules[i]);
					int strLen = Encoding.UTF8.GetByteCount(id);
					bool flag2 = strLen > maxLen;
					if (flag2)
					{
						maxLen = strLen;
					}
				}
			}
			foreach (byte[] extModule in context.ExternalModules)
			{
				string name = Compressor.GetId(extModule).ToUpperInvariant();
				modules.Add(name, extModule);
				int strLen2 = Encoding.UTF8.GetByteCount(name);
				bool flag3 = strLen2 > maxLen;
				if (flag3)
				{
					maxLen = strLen2;
				}
			}
			byte[] key = random.NextBytes(4 + maxLen);
			key[0] = (byte)compCtx.EntryPointToken;
			key[1] = (byte)(compCtx.EntryPointToken >> 8);
			key[2] = (byte)(compCtx.EntryPointToken >> 16);
			key[3] = (byte)(compCtx.EntryPointToken >> 24);
			for (int j = 4; j < key.Length; j++)
			{
				byte[] array = key;
				int num = j;
				array[num] |= 1;
			}
			compCtx.KeySig = key;
			int moduleIndex = 0;
			Action<double> <>9__0;
			foreach (KeyValuePair<string, byte[]> entry in modules)
			{
				byte[] name2 = Encoding.UTF8.GetBytes(entry.Key);
				for (int k = 0; k < name2.Length; k++)
				{
					byte[] array2 = name2;
					int num2 = k;
					array2[num2] *= key[k + 4];
				}
				uint state = 7339873U;
				foreach (byte chr in name2)
				{
					state = state * 6176543U + (uint)chr;
				}
				byte[] value = entry.Value;
				uint seed = state;
				Action<double> progressFunc;
				if ((progressFunc = <>9__0) == null)
				{
					progressFunc = (<>9__0 = delegate(double progress)
					{
						progress = (progress + (double)moduleIndex) / (double)modules.Count;
						context.Logger.Progress((int)(progress * 10000.0), 10000);
					});
				}
				byte[] encrypted = compCtx.Encrypt(comp, value, seed, progressFunc);
				context.CheckCancellation();
				EmbeddedResource resource = new EmbeddedResource(Convert.ToBase64String(name2), encrypted, ManifestResourceAttributes.Private);
				stubModule.Resources.Add(resource);
				int moduleIndex2 = moduleIndex;
				moduleIndex = moduleIndex2 + 1;
			}
			context.Logger.EndProgress();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00006930 File Offset: 0x00004B30
		private void InjectData(ModuleDef stubModule, MethodDef method, byte[] data)
		{
			TypeDefUser dataType = new TypeDefUser("", "DataType", stubModule.CorLibTypes.GetTypeRef("System", "ValueType"));
			dataType.Layout = dnlib.DotNet.TypeAttributes.ExplicitLayout;
			dataType.Visibility = dnlib.DotNet.TypeAttributes.NestedPrivate;
			dataType.IsSealed = true;
			dataType.ClassLayout = new ClassLayoutUser(1, (uint)data.Length);
			stubModule.GlobalType.NestedTypes.Add(dataType);
			FieldDefUser dataField = new FieldDefUser("DataField", new FieldSig(dataType.ToTypeSig()))
			{
				IsStatic = true,
				HasFieldRVA = true,
				InitialValue = data,
				Access = dnlib.DotNet.FieldAttributes.PrivateScope
			};
			stubModule.GlobalType.Fields.Add(dataField);
			MutationHelper.ReplacePlaceholder(method, delegate(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Dup));
				repl.Add(Instruction.Create(OpCodes.Ldtoken, dataField));
				repl.Add(Instruction.Create(OpCodes.Call, stubModule.Import(typeof(RuntimeHelpers).GetMethod("InitializeArray"))));
				return repl.ToArray();
			});
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00006A30 File Offset: 0x00004C30
		private void InjectStub(ConfuserContext context, CompressorContext compCtx, ProtectionParameters parameters, ModuleDef stubModule)
		{
			IRuntimeService rt = context.Registry.GetService<IRuntimeService>();
			RandomGenerator random = context.Registry.GetService<IRandomService>().GetRandomGenerator(this.Id);
			ICompressionService comp = context.Registry.GetService<ICompressionService>();
			TypeDef rtType = rt.GetRuntimeType(compCtx.CompatMode ? "Confuser.Runtime.CompressorCompat" : "Confuser.Runtime.Compressor");
			IEnumerable<IDnlibDef> defs = InjectHelper.Inject(rtType, stubModule.GlobalType, stubModule);
			Mode parameter = parameters.GetParameter<Mode>(context, context.CurrentModule, "key", Mode.Normal);
			if (parameter != Mode.Normal)
			{
				if (parameter != Mode.Dynamic)
				{
					throw new UnreachableException();
				}
				compCtx.Deriver = new DynamicDeriver();
			}
			else
			{
				compCtx.Deriver = new NormalDeriver();
			}
			compCtx.Deriver.Init(context, random);
			context.Logger.Debug("Encrypting modules...");
			MethodDef entryPoint = defs.OfType<MethodDef>().Single((MethodDef method) => method.Name == "Main");
			stubModule.EntryPoint = entryPoint;
			bool flag = compCtx.EntryPoint.HasAttribute("System.STAThreadAttribute");
			if (flag)
			{
				TypeRef attrType = stubModule.CorLibTypes.GetTypeRef("System", "STAThreadAttribute");
				MethodSig ctorSig = MethodSig.CreateInstance(stubModule.CorLibTypes.Void);
				entryPoint.CustomAttributes.Add(new CustomAttribute(new MemberRefUser(stubModule, ".ctor", ctorSig, attrType)));
			}
			else
			{
				bool flag2 = compCtx.EntryPoint.HasAttribute("System.MTAThreadAttribute");
				if (flag2)
				{
					TypeRef attrType2 = stubModule.CorLibTypes.GetTypeRef("System", "MTAThreadAttribute");
					MethodSig ctorSig2 = MethodSig.CreateInstance(stubModule.CorLibTypes.Void);
					entryPoint.CustomAttributes.Add(new CustomAttribute(new MemberRefUser(stubModule, ".ctor", ctorSig2, attrType2)));
				}
			}
			uint seed = random.NextUInt32();
			compCtx.OriginModule = context.OutputModules[compCtx.ModuleIndex];
			byte[] encryptedModule = compCtx.Encrypt(comp, compCtx.OriginModule, seed, delegate(double progress)
			{
				context.Logger.Progress((int)(progress * 10000.0), 10000);
			});
			context.Logger.EndProgress();
			context.CheckCancellation();
			compCtx.EncryptedModule = encryptedModule;
			MutationHelper.InjectKeys(entryPoint, new int[]
			{
				0,
				1
			}, new int[]
			{
				encryptedModule.Length >> 2,
				(int)seed
			});
			this.InjectData(stubModule, entryPoint, encryptedModule);
			MethodDef decrypter = defs.OfType<MethodDef>().Single((MethodDef method) => method.Name == "Decrypt");
			decrypter.Body.SimplifyMacros(decrypter.Parameters);
			List<Instruction> instrs = decrypter.Body.Instructions.ToList<Instruction>();
			for (int i = 0; i < instrs.Count; i++)
			{
				Instruction instr = instrs[i];
				bool flag3 = instr.OpCode == OpCodes.Call;
				if (flag3)
				{
					IMethod method2 = (IMethod)instr.Operand;
					bool flag4 = method2.DeclaringType.Name == "Mutation" && method2.Name == "Crypt";
					if (flag4)
					{
						Instruction ldDst = instrs[i - 2];
						Instruction ldSrc = instrs[i - 1];
						Debug.Assert(ldDst.OpCode == OpCodes.Ldloc && ldSrc.OpCode == OpCodes.Ldloc);
						instrs.RemoveAt(i);
						instrs.RemoveAt(i - 1);
						instrs.RemoveAt(i - 2);
						instrs.InsertRange(i - 2, compCtx.Deriver.EmitDerivation(decrypter, context, (Local)ldDst.Operand, (Local)ldSrc.Operand));
					}
					else
					{
						bool flag5 = method2.DeclaringType.Name == "Lzma" && method2.Name == "Decompress";
						if (flag5)
						{
							MethodDef decomp = comp.GetRuntimeDecompressor(stubModule, delegate(IDnlibDef member)
							{
							});
							instr.Operand = decomp;
						}
					}
				}
			}
			decrypter.Body.Instructions.Clear();
			foreach (Instruction instr2 in instrs)
			{
				decrypter.Body.Instructions.Add(instr2);
			}
			this.PackModules(context, compCtx, stubModule, comp, random);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00006F2C File Offset: 0x0000512C
		private void ImportAssemblyTypeReferences(ModuleDef originModule, ModuleDef stubModule)
		{
			AssemblyDef assembly = stubModule.Assembly;
			foreach (CustomAttribute ca in assembly.CustomAttributes)
			{
				bool flag = ca.AttributeType.Scope == originModule;
				if (flag)
				{
					ca.Constructor = (ICustomAttributeType)stubModule.Import(ca.Constructor);
				}
			}
			foreach (CustomAttribute ca2 in assembly.DeclSecurities.SelectMany((DeclSecurity declSec) => declSec.CustomAttributes))
			{
				bool flag2 = ca2.AttributeType.Scope == originModule;
				if (flag2)
				{
					ca2.Constructor = (ICustomAttributeType)stubModule.Import(ca2.Constructor);
				}
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00004A94 File Offset: 0x00002C94
		public Compressor()
		{
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00004A9D File Offset: 0x00002C9D
		// Note: this type is marked as 'beforefieldinit'.
		static Compressor()
		{
		}

		// Token: 0x04000009 RID: 9
		public const string _Id = "compressor";

		// Token: 0x0400000A RID: 10
		public const string _FullId = "Ki.Compressor";

		// Token: 0x0400000B RID: 11
		public const string _ServiceId = "Ki.Compressor";

		// Token: 0x0400000C RID: 12
		public static readonly object ContextKey = new object();

		// Token: 0x02000007 RID: 7
		private class KeyInjector : IModuleWriterListener
		{
			// Token: 0x0600001F RID: 31 RVA: 0x00004AA9 File Offset: 0x00002CA9
			public KeyInjector(CompressorContext ctx)
			{
				this.ctx = ctx;
			}

			// Token: 0x06000020 RID: 32 RVA: 0x0000703C File Offset: 0x0000523C
			public void OnWriterEvent(ModuleWriterBase writer, ModuleWriterEvent evt)
			{
				bool flag = evt == ModuleWriterEvent.MDBeginCreateTables;
				if (flag)
				{
					uint signature = writer.MetaData.BlobHeap.Add(this.ctx.KeySig);
					uint num = writer.MetaData.TablesHeap.StandAloneSigTable.Add(new RawStandAloneSigRow(signature));
					Debug.Assert(num == 1U);
					uint num2 = 285212672U | num;
					this.ctx.KeyToken = num2;
					MutationHelper.InjectKey(writer.Module.EntryPoint, 2, (int)num2);
				}
				else
				{
					bool flag2 = evt == ModuleWriterEvent.MDBeginAddResources && !this.ctx.CompatMode;
					if (flag2)
					{
						byte[] data = SHA1.Create().ComputeHash(this.ctx.OriginModule);
						uint hashValue = writer.MetaData.BlobHeap.Add(data);
						MDTable<RawFileRow> fileTable = writer.MetaData.TablesHeap.FileTable;
						uint rid = fileTable.Add(new RawFileRow(0U, writer.MetaData.StringsHeap.Add("SafeGuard"), hashValue));
						uint implementation = CodedToken.Implementation.Encode(new MDToken(Table.File, rid));
						MDTable<RawManifestResourceRow> manifestResourceTable = writer.MetaData.TablesHeap.ManifestResourceTable;
						foreach (Tuple<uint, uint, string> tuple in this.ctx.ManifestResources)
						{
							manifestResourceTable.Add(new RawManifestResourceRow(tuple.Item1, tuple.Item2, writer.MetaData.StringsHeap.Add(tuple.Item3), implementation));
						}
						MDTable<RawExportedTypeRow> exportedTypeTable = writer.MetaData.TablesHeap.ExportedTypeTable;
						foreach (TypeDef typeDef in this.ctx.OriginModuleDef.GetTypes())
						{
							bool flag3 = !typeDef.IsVisibleOutside(true);
							if (!flag3)
							{
								exportedTypeTable.Add(new RawExportedTypeRow((uint)typeDef.Attributes, 0U, writer.MetaData.StringsHeap.Add(typeDef.Name), writer.MetaData.StringsHeap.Add(typeDef.Namespace), implementation));
							}
						}
					}
				}
			}

			// Token: 0x0400000D RID: 13
			private readonly CompressorContext ctx;
		}

		// Token: 0x02000008 RID: 8
		[CompilerGenerated]
		private sealed class <>c__DisplayClass17_0
		{
			// Token: 0x06000021 RID: 33 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass17_0()
			{
			}

			// Token: 0x06000022 RID: 34 RVA: 0x00004ABA File Offset: 0x00002CBA
			internal void <PackModules>b__0(double progress)
			{
				progress = (progress + (double)this.moduleIndex) / (double)this.modules.Count;
				this.context.Logger.Progress((int)(progress * 10000.0), 10000);
			}

			// Token: 0x0400000E RID: 14
			public int moduleIndex;

			// Token: 0x0400000F RID: 15
			public Dictionary<string, byte[]> modules;

			// Token: 0x04000010 RID: 16
			public ConfuserContext context;

			// Token: 0x04000011 RID: 17
			public Action<double> <>9__0;
		}

		// Token: 0x02000009 RID: 9
		[CompilerGenerated]
		private sealed class <>c__DisplayClass18_0
		{
			// Token: 0x06000023 RID: 35 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass18_0()
			{
			}

			// Token: 0x06000024 RID: 36 RVA: 0x000072A8 File Offset: 0x000054A8
			internal Instruction[] <InjectData>b__0(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Dup));
				repl.Add(Instruction.Create(OpCodes.Ldtoken, this.dataField));
				repl.Add(Instruction.Create(OpCodes.Call, this.stubModule.Import(typeof(RuntimeHelpers).GetMethod("InitializeArray"))));
				return repl.ToArray();
			}

			// Token: 0x04000012 RID: 18
			public FieldDefUser dataField;

			// Token: 0x04000013 RID: 19
			public ModuleDef stubModule;
		}

		// Token: 0x0200000A RID: 10
		[CompilerGenerated]
		private sealed class <>c__DisplayClass19_0
		{
			// Token: 0x06000025 RID: 37 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass19_0()
			{
			}

			// Token: 0x06000026 RID: 38 RVA: 0x00004AF7 File Offset: 0x00002CF7
			internal void <InjectStub>b__1(double progress)
			{
				this.context.Logger.Progress((int)(progress * 10000.0), 10000);
			}

			// Token: 0x04000014 RID: 20
			public ConfuserContext context;
		}

		// Token: 0x0200000B RID: 11
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000027 RID: 39 RVA: 0x00004B1B File Offset: 0x00002D1B
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000028 RID: 40 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x06000029 RID: 41 RVA: 0x00004B27 File Offset: 0x00002D27
			internal bool <InjectStub>b__19_0(MethodDef method)
			{
				return method.Name == "Main";
			}

			// Token: 0x0600002A RID: 42 RVA: 0x00004B39 File Offset: 0x00002D39
			internal bool <InjectStub>b__19_2(MethodDef method)
			{
				return method.Name == "Decrypt";
			}

			// Token: 0x0600002B RID: 43 RVA: 0x00004A34 File Offset: 0x00002C34
			internal void <InjectStub>b__19_3(IDnlibDef member)
			{
			}

			// Token: 0x0600002C RID: 44 RVA: 0x00004B4B File Offset: 0x00002D4B
			internal IEnumerable<CustomAttribute> <ImportAssemblyTypeReferences>b__20_0(DeclSecurity declSec)
			{
				return declSec.CustomAttributes;
			}

			// Token: 0x04000015 RID: 21
			public static readonly Compressor.<>c <>9 = new Compressor.<>c();

			// Token: 0x04000016 RID: 22
			public static Func<MethodDef, bool> <>9__19_0;

			// Token: 0x04000017 RID: 23
			public static Func<MethodDef, bool> <>9__19_2;

			// Token: 0x04000018 RID: 24
			public static Action<IDnlibDef> <>9__19_3;

			// Token: 0x04000019 RID: 25
			public static Func<DeclSecurity, IEnumerable<CustomAttribute>> <>9__20_0;
		}
	}
}
