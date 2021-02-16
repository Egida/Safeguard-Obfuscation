using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Confuser.Core;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Compress
{
	// Token: 0x02000104 RID: 260
	internal class StubProtection : Protection
	{
		// Token: 0x060003F5 RID: 1013 RVA: 0x00005EA0 File Offset: 0x000040A0
		internal StubProtection(CompressorContext ctx, ModuleDef originModule)
		{
			this.ctx = ctx;
			this.originModule = originModule;
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x000201F8 File Offset: 0x0001E3F8
		public override string Name
		{
			get
			{
				return "Compressor Stub Protection";
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060003F7 RID: 1015 RVA: 0x00020210 File Offset: 0x0001E410
		public override string Description
		{
			get
			{
				return "Do some extra works on the protected stub.";
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00020228 File Offset: 0x0001E428
		public override string Id
		{
			get
			{
				return "Ki.Compressor.Protection";
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00020228 File Offset: 0x0001E428
		public override string FullId
		{
			get
			{
				return "Ki.Compressor.Protection";
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x00020240 File Offset: 0x0001E440
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.None;
			}
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00020254 File Offset: 0x0001E454
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			bool flag = !this.ctx.CompatMode;
			if (flag)
			{
				pipeline.InsertPreStage(PipelineStage.Inspection, new StubProtection.InjPhase(this));
			}
			pipeline.InsertPostStage(PipelineStage.BeginModule, new StubProtection.SigPhase(this));
		}

		// Token: 0x04000309 RID: 777
		private readonly CompressorContext ctx;

		// Token: 0x0400030A RID: 778
		private readonly ModuleDef originModule;

		// Token: 0x02000105 RID: 261
		private class InjPhase : ProtectionPhase
		{
			// Token: 0x060003FD RID: 1021 RVA: 0x00004A51 File Offset: 0x00002C51
			public InjPhase(StubProtection parent) : base(parent)
			{
			}

			// Token: 0x170000C5 RID: 197
			// (get) Token: 0x060003FE RID: 1022 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x170000C6 RID: 198
			// (get) Token: 0x060003FF RID: 1023 RVA: 0x00020290 File Offset: 0x0001E490
			public override bool ProcessAll
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170000C7 RID: 199
			// (get) Token: 0x06000400 RID: 1024 RVA: 0x000202A4 File Offset: 0x0001E4A4
			public override string Name
			{
				get
				{
					return "Module injection";
				}
			}

			// Token: 0x06000401 RID: 1025 RVA: 0x000202BC File Offset: 0x0001E4BC
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				ModuleDef originModule = ((StubProtection)base.Parent).originModule;
				originModule.Assembly.Modules.Remove(originModule);
				context.Modules[0].Assembly.Modules.Add(((StubProtection)base.Parent).originModule);
			}
		}

		// Token: 0x02000106 RID: 262
		private class SigPhase : ProtectionPhase
		{
			// Token: 0x06000402 RID: 1026 RVA: 0x00004A51 File Offset: 0x00002C51
			public SigPhase(StubProtection parent) : base(parent)
			{
			}

			// Token: 0x170000C8 RID: 200
			// (get) Token: 0x06000403 RID: 1027 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x170000C9 RID: 201
			// (get) Token: 0x06000404 RID: 1028 RVA: 0x0002031C File Offset: 0x0001E51C
			public override string Name
			{
				get
				{
					return "Packer info encoding";
				}
			}

			// Token: 0x06000405 RID: 1029 RVA: 0x00020334 File Offset: 0x0001E534
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				FieldDef field = context.CurrentModule.Types[0].FindField("DataField");
				Debug.Assert(field != null);
				context.Registry.GetService<INameService>().SetCanRename(field, true);
				context.CurrentModuleWriterListener.OnWriterEvent += delegate(object sender, ModuleWriterListenerEventArgs e)
				{
					bool flag = e.WriterEvent == ModuleWriterEvent.MDBeginCreateTables;
					if (flag)
					{
						ModuleWriterBase writer = (ModuleWriterBase)sender;
						StubProtection prot = (StubProtection)base.Parent;
						uint blob = writer.MetaData.BlobHeap.Add(prot.ctx.KeySig);
						uint rid = writer.MetaData.TablesHeap.StandAloneSigTable.Add(new RawStandAloneSigRow(blob));
						Debug.Assert((285212672U | rid) == prot.ctx.KeyToken);
						bool compatMode = prot.ctx.CompatMode;
						if (!compatMode)
						{
							byte[] hash = SHA1.Create().ComputeHash(prot.ctx.OriginModule);
							uint hashBlob = writer.MetaData.BlobHeap.Add(hash);
							MDTable<RawFileRow> fileTbl = writer.MetaData.TablesHeap.FileTable;
							uint fileRid = fileTbl.Add(new RawFileRow(0U, writer.MetaData.StringsHeap.Add("Beds-Protector"), hashBlob));
						}
					}
				};
			}

			// Token: 0x06000406 RID: 1030 RVA: 0x00020398 File Offset: 0x0001E598
			[CompilerGenerated]
			private void <Execute>b__5_0(object sender, ModuleWriterListenerEventArgs e)
			{
				bool flag = e.WriterEvent == ModuleWriterEvent.MDBeginCreateTables;
				if (flag)
				{
					ModuleWriterBase writer = (ModuleWriterBase)sender;
					StubProtection prot = (StubProtection)base.Parent;
					uint blob = writer.MetaData.BlobHeap.Add(prot.ctx.KeySig);
					uint rid = writer.MetaData.TablesHeap.StandAloneSigTable.Add(new RawStandAloneSigRow(blob));
					Debug.Assert((285212672U | rid) == prot.ctx.KeyToken);
					bool compatMode = prot.ctx.CompatMode;
					if (!compatMode)
					{
						byte[] hash = SHA1.Create().ComputeHash(prot.ctx.OriginModule);
						uint hashBlob = writer.MetaData.BlobHeap.Add(hash);
						MDTable<RawFileRow> fileTbl = writer.MetaData.TablesHeap.FileTable;
						uint fileRid = fileTbl.Add(new RawFileRow(0U, writer.MetaData.StringsHeap.Add("Beds-Protector"), hashBlob));
					}
				}
			}
		}
	}
}
