using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Compress
{
	// Token: 0x020000FD RID: 253
	internal class ExtractPhase : ProtectionPhase
	{
		// Token: 0x060003DC RID: 988 RVA: 0x00004A51 File Offset: 0x00002C51
		public ExtractPhase(Compressor parent) : base(parent)
		{
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060003DD RID: 989 RVA: 0x000062B4 File Offset: 0x000044B4
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Modules;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060003DE RID: 990 RVA: 0x0001F9B4 File Offset: 0x0001DBB4
		public override string Name
		{
			get
			{
				return "Packer info extraction";
			}
		}

		// Token: 0x060003DF RID: 991 RVA: 0x0001F9CC File Offset: 0x0001DBCC
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool flag = context.Packer == null;
			if (!flag)
			{
				bool flag2 = context.CurrentModule.Kind == ModuleKind.Windows || context.CurrentModule.Kind == ModuleKind.Console;
				bool flag3 = context.Annotations.Get<CompressorContext>(context, Compressor.ContextKey, null) != null;
				if (flag3)
				{
					bool flag4 = flag2;
					if (flag4)
					{
						context.Logger.Error("Too many executable modules!");
						throw new ConfuserException(null);
					}
				}
				else
				{
					bool flag5 = flag2;
					if (flag5)
					{
						CompressorContext compressorContext = new CompressorContext
						{
							ModuleIndex = context.CurrentModuleIndex,
							Assembly = context.CurrentModule.Assembly,
							CompatMode = parameters.GetParameter<bool>(context, null, "compat", false)
						};
						context.Annotations.Set<CompressorContext>(context, Compressor.ContextKey, compressorContext);
						compressorContext.ModuleName = context.CurrentModule.Name;
						compressorContext.EntryPoint = context.CurrentModule.EntryPoint;
						compressorContext.Kind = context.CurrentModule.Kind;
						bool flag6 = !compressorContext.CompatMode;
						if (flag6)
						{
							context.CurrentModule.Name = "ns18";
							context.CurrentModule.EntryPoint = null;
							context.CurrentModule.Kind = ModuleKind.NetModule;
						}
						context.CurrentModuleWriterListener.OnWriterEvent += new ExtractPhase.ResourceRecorder(compressorContext, context.CurrentModule).OnWriterEvent;
					}
				}
			}
		}

		// Token: 0x020000FE RID: 254
		private class ResourceRecorder
		{
			// Token: 0x060003E0 RID: 992 RVA: 0x00005DD4 File Offset: 0x00003FD4
			public ResourceRecorder(CompressorContext ctx, ModuleDef module)
			{
				this.ctx = ctx;
				this.targetModule = module;
			}

			// Token: 0x060003E1 RID: 993 RVA: 0x0001FB44 File Offset: 0x0001DD44
			public void OnWriterEvent(object sender, ModuleWriterListenerEventArgs e)
			{
				bool flag = e.WriterEvent == ModuleWriterEvent.MDEndAddResources;
				if (flag)
				{
					ModuleWriterBase writer = (ModuleWriterBase)sender;
					this.ctx.ManifestResources = new List<Tuple<uint, uint, string>>();
					Dictionary<uint, byte[]> stringDict = writer.MetaData.StringsHeap.GetAllRawData().ToDictionary((KeyValuePair<uint, byte[]> pair) => pair.Key, (KeyValuePair<uint, byte[]> pair) => pair.Value);
					foreach (RawManifestResourceRow resource in writer.MetaData.TablesHeap.ManifestResourceTable)
					{
						this.ctx.ManifestResources.Add(Tuple.Create<uint, uint, string>(resource.Offset, resource.Flags, Encoding.UTF8.GetString(stringDict[resource.Name])));
					}
					this.ctx.EntryPointToken = writer.MetaData.GetToken(this.ctx.EntryPoint).Raw;
				}
			}

			// Token: 0x040002EF RID: 751
			private readonly CompressorContext ctx;

			// Token: 0x040002F0 RID: 752
			private ModuleDef targetModule;

			// Token: 0x020000FF RID: 255
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x060003E2 RID: 994 RVA: 0x00005DEC File Offset: 0x00003FEC
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x060003E3 RID: 995 RVA: 0x00004A68 File Offset: 0x00002C68
				public <>c()
				{
				}

				// Token: 0x060003E4 RID: 996 RVA: 0x00005DF8 File Offset: 0x00003FF8
				internal uint <OnWriterEvent>b__3_0(KeyValuePair<uint, byte[]> pair)
				{
					return pair.Key;
				}

				// Token: 0x060003E5 RID: 997 RVA: 0x00005E01 File Offset: 0x00004001
				internal byte[] <OnWriterEvent>b__3_1(KeyValuePair<uint, byte[]> pair)
				{
					return pair.Value;
				}

				// Token: 0x040002F1 RID: 753
				public static readonly ExtractPhase.ResourceRecorder.<>c <>9 = new ExtractPhase.ResourceRecorder.<>c();

				// Token: 0x040002F2 RID: 754
				public static Func<KeyValuePair<uint, byte[]>, uint> <>9__3_0;

				// Token: 0x040002F3 RID: 755
				public static Func<KeyValuePair<uint, byte[]>, byte[]> <>9__3_1;
			}
		}
	}
}
