using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections
{
	// Token: 0x02000030 RID: 48
	[BeforeProtection(new string[]
	{
		"Ki.FakeNative"
	})]
	internal class InvalidMetadataProtection : Protection
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00009EE8 File Offset: 0x000080E8
		public override string Name
		{
			get
			{
				return "Invalid Metadata Protection";
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00009F00 File Offset: 0x00008100
		public override string Description
		{
			get
			{
				return "This protection adds invalid metadata to modules to prevent disassembler/decompiler from opening them.";
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00009F18 File Offset: 0x00008118
		public override string Id
		{
			get
			{
				return "invalid metadata";
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00009F30 File Offset: 0x00008130
		public override string FullId
		{
			get
			{
				return "Ki.InvalidMD";
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000DC RID: 220 RVA: 0x000073FC File Offset: 0x000055FC
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00004A34 File Offset: 0x00002C34
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00004E1E File Offset: 0x0000301E
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPostStage(PipelineStage.BeginModule, new InvalidMetadataProtection.InvalidMDPhase(this));
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00004A48 File Offset: 0x00002C48
		public InvalidMetadataProtection()
		{
		}

		// Token: 0x04000050 RID: 80
		public const string _Id = "invalid metadata";

		// Token: 0x04000051 RID: 81
		public const string _FullId = "Ki.InvalidMD";

		// Token: 0x02000031 RID: 49
		private class InvalidMDPhase : ProtectionPhase
		{
			// Token: 0x060000E0 RID: 224 RVA: 0x00004A51 File Offset: 0x00002C51
			public InvalidMDPhase(InvalidMetadataProtection parent) : base(parent)
			{
			}

			// Token: 0x17000060 RID: 96
			// (get) Token: 0x060000E1 RID: 225 RVA: 0x000062B4 File Offset: 0x000044B4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000061 RID: 97
			// (get) Token: 0x060000E2 RID: 226 RVA: 0x00009F48 File Offset: 0x00008148
			public override string Name
			{
				get
				{
					return "Invalid metadata addition";
				}
			}

			// Token: 0x060000E3 RID: 227 RVA: 0x00009F60 File Offset: 0x00008160
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				bool flag = parameters.Targets.Contains(context.CurrentModule);
				if (flag)
				{
					this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.InvalidMD");
					context.CurrentModuleWriterListener.OnWriterEvent += this.OnWriterEvent;
				}
			}

			// Token: 0x060000E4 RID: 228 RVA: 0x00009FB8 File Offset: 0x000081B8
			private void Randomize<T>(MDTable<T> table) where T : IRawRow
			{
				List<T> rows = table.ToList<T>();
				this.random.Shuffle<T>(rows);
				table.Reset();
				foreach (T row in rows)
				{
					table.Add(row);
				}
			}

			// Token: 0x060000E5 RID: 229 RVA: 0x0000A028 File Offset: 0x00008228
			private void OnWriterEvent(object sender, ModuleWriterListenerEventArgs e)
			{
				ModuleWriterBase moduleWriterBase = (ModuleWriterBase)sender;
				bool flag = e.WriterEvent == ModuleWriterEvent.MDEndCreateTables;
				if (flag)
				{
					int num = this.random.NextInt32(100, 150);
					for (int i = 0; i < num; i++)
					{
						moduleWriterBase.MetaData.TablesHeap.ENCLogTable.Add(new RawENCLogRow(this.random.NextUInt32(), this.random.NextUInt32()));
					}
					num = this.random.NextInt32(100, 150);
					for (int j = 0; j < num; j++)
					{
						moduleWriterBase.MetaData.TablesHeap.ENCMapTable.Add(new RawENCMapRow(this.random.NextUInt32()));
					}
					this.Randomize<RawManifestResourceRow>(moduleWriterBase.MetaData.TablesHeap.ManifestResourceTable);
					moduleWriterBase.TheOptions.MetaDataOptions.TablesHeapOptions.ExtraData = new uint?(this.random.NextUInt32());
					moduleWriterBase.TheOptions.MetaDataOptions.TablesHeapOptions.UseENC = new bool?(false);
					MetaDataHeaderOptions metaDataHeaderOptions = moduleWriterBase.TheOptions.MetaDataOptions.MetaDataHeaderOptions;
					MetaDataHeaderOptions metaDataHeaderOptions2 = metaDataHeaderOptions;
					MetaDataHeaderOptions metaDataHeaderOptions3 = metaDataHeaderOptions2;
					metaDataHeaderOptions3.VersionString += "¶";
					MetaDataHeaderOptions metaDataHeaderOptions4 = moduleWriterBase.TheOptions.MetaDataOptions.MetaDataHeaderOptions;
					MetaDataHeaderOptions metaDataHeaderOptions5 = metaDataHeaderOptions4;
					metaDataHeaderOptions5.VersionString += "\0\0\0\0";
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeapsEnd.Add(new InvalidMetadataProtection.RawHeap("#GUID", Guid.NewGuid().ToByteArray()));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeapsEnd.Add(new InvalidMetadataProtection.RawHeap("#GUID", Guid.NewGuid().ToByteArray()));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeaps.Add(new MyHeap("#US "));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeaps.Add(new MyHeap("#Strings "));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeaps.Add(new MyHeap("#SafeGuard"));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeaps.Add(new MyHeap("#乃乇刀 ﾶけ乇刀刀ﾉํํํํํํํํํํɹ̶̨͈̰̙̯̯̟̥"));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeapsEnd.Add(new MyHeap("#SafeGuard"));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeapsEnd.Add(new MyHeap("#Strings "));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeapsEnd.Add(new InvalidMetadataProtection.RawHeap("#GUID", Guid.NewGuid().ToByteArray()));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeapsEnd.Add(new InvalidMetadataProtection.RawHeap("#Strings", new byte[100]));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeapsEnd.Add(new InvalidMetadataProtection.RawHeap("#Blob", new byte[100]));
					moduleWriterBase.TheOptions.MetaDataOptions.OtherHeapsEnd.Add(new InvalidMetadataProtection.RawHeap("#Schema", new byte[100]));
				}
				else
				{
					bool flag2 = e.WriterEvent == ModuleWriterEvent.MDOnAllTablesSorted;
					if (flag2)
					{
						moduleWriterBase.MetaData.TablesHeap.DeclSecurityTable.Add(new RawDeclSecurityRow(4661, 1454859485U, 978978445U));
					}
				}
			}

			// Token: 0x060000E6 RID: 230 RVA: 0x00004E2F File Offset: 0x0000302F
			// Note: this type is marked as 'beforefieldinit'.
			static InvalidMDPhase()
			{
			}

			// Token: 0x04000052 RID: 82
			private RandomGenerator random;

			// Token: 0x04000053 RID: 83
			private static Random R = new Random();
		}

		// Token: 0x02000032 RID: 50
		private class RawHeap : HeapBase
		{
			// Token: 0x060000E7 RID: 231 RVA: 0x00004E3B File Offset: 0x0000303B
			public RawHeap(string name, byte[] content)
			{
				this.name = name;
				this.content = content;
			}

			// Token: 0x17000062 RID: 98
			// (get) Token: 0x060000E8 RID: 232 RVA: 0x0000A3A4 File Offset: 0x000085A4
			public override string Name
			{
				get
				{
					return this.name;
				}
			}

			// Token: 0x060000E9 RID: 233 RVA: 0x0000A3BC File Offset: 0x000085BC
			public override uint GetRawLength()
			{
				return (uint)this.content.Length;
			}

			// Token: 0x060000EA RID: 234 RVA: 0x00004E53 File Offset: 0x00003053
			protected override void WriteToImpl(BinaryWriter writer)
			{
				writer.Write(this.content);
			}

			// Token: 0x04000054 RID: 84
			private readonly byte[] content;

			// Token: 0x04000055 RID: 85
			private readonly string name;
		}
	}
}
