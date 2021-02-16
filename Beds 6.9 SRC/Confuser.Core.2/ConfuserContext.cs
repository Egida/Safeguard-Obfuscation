using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Confuser.Core.Project;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace Confuser.Core
{
	/// <summary>
	///     Context providing information on the current protection process.
	/// </summary>
	// Token: 0x02000068 RID: 104
	public class ConfuserContext
	{
		/// <summary>
		///     Throws a System.OperationCanceledException if protection process has been canceled.
		/// </summary>
		/// <exception cref="T:System.OperationCanceledException">
		///     The protection process is canceled.
		/// </exception>
		// Token: 0x06000256 RID: 598 RVA: 0x00003038 File Offset: 0x00001238
		public void CheckCancellation()
		{
			this.token.ThrowIfCancellationRequested();
		}

		/// <summary>
		///     Requests the current module to be written as mix-mode module, and return the native writer options.
		/// </summary>
		/// <returns>The native writer options.</returns>
		// Token: 0x06000257 RID: 599 RVA: 0x00011494 File Offset: 0x0000F694
		public NativeModuleWriterOptions RequestNative()
		{
			bool flag = this.CurrentModule == null;
			NativeModuleWriterOptions result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = this.CurrentModuleWriterOptions == null;
				if (flag2)
				{
					this.CurrentModuleWriterOptions = new NativeModuleWriterOptions(this.CurrentModule);
				}
				bool flag3 = this.CurrentModuleWriterOptions is NativeModuleWriterOptions;
				if (flag3)
				{
					result = (NativeModuleWriterOptions)this.CurrentModuleWriterOptions;
				}
				else
				{
					NativeModuleWriterOptions newOptions = new NativeModuleWriterOptions(this.CurrentModule, this.CurrentModuleWriterOptions.Listener);
					newOptions.AddCheckSum = this.CurrentModuleWriterOptions.AddCheckSum;
					newOptions.Cor20HeaderOptions = this.CurrentModuleWriterOptions.Cor20HeaderOptions;
					newOptions.Logger = this.CurrentModuleWriterOptions.Logger;
					newOptions.MetaDataLogger = this.CurrentModuleWriterOptions.MetaDataLogger;
					newOptions.MetaDataOptions = this.CurrentModuleWriterOptions.MetaDataOptions;
					newOptions.ModuleKind = this.CurrentModuleWriterOptions.ModuleKind;
					newOptions.PEHeadersOptions = this.CurrentModuleWriterOptions.PEHeadersOptions;
					newOptions.ShareMethodBodies = this.CurrentModuleWriterOptions.ShareMethodBodies;
					newOptions.StrongNameKey = this.CurrentModuleWriterOptions.StrongNameKey;
					newOptions.StrongNamePublicKey = this.CurrentModuleWriterOptions.StrongNamePublicKey;
					newOptions.Win32Resources = this.CurrentModuleWriterOptions.Win32Resources;
					this.CurrentModuleWriterOptions = newOptions;
					result = newOptions;
				}
			}
			return result;
		}

		/// <summary>
		///     Gets the annotation storage.
		/// </summary>
		/// <value>The annotation storage.</value>
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000258 RID: 600 RVA: 0x000115E8 File Offset: 0x0000F7E8
		public Annotations Annotations
		{
			get
			{
				return this.annotations;
			}
		}

		/// <summary>
		///     Gets the base directory.
		/// </summary>
		/// <value>The base directory.</value>
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000259 RID: 601 RVA: 0x00003047 File Offset: 0x00001247
		// (set) Token: 0x0600025A RID: 602 RVA: 0x0000304F File Offset: 0x0000124F
		public string BaseDirectory
		{
			[CompilerGenerated]
			get
			{
				return this.<BaseDirectory>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<BaseDirectory>k__BackingField = value;
			}
		}

		/// <summary>
		/// 	Gets the token used to indicate cancellation
		/// </summary>
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600025B RID: 603 RVA: 0x00011600 File Offset: 0x0000F800
		public CancellationToken CancellationToken
		{
			get
			{
				return this.token;
			}
		}

		/// <summary>
		///     Gets the current module.
		/// </summary>
		/// <value>The current module.</value>
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600025C RID: 604 RVA: 0x00011618 File Offset: 0x0000F818
		public ModuleDefMD CurrentModule
		{
			get
			{
				bool flag = this.CurrentModuleIndex != -1;
				ModuleDefMD result;
				if (flag)
				{
					result = this.Modules[this.CurrentModuleIndex];
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		/// <summary>
		///     Gets the current module index.
		/// </summary>
		/// <value>The current module index.</value>
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600025D RID: 605 RVA: 0x00003058 File Offset: 0x00001258
		// (set) Token: 0x0600025E RID: 606 RVA: 0x00003060 File Offset: 0x00001260
		public int CurrentModuleIndex
		{
			[CompilerGenerated]
			get
			{
				return this.<CurrentModuleIndex>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<CurrentModuleIndex>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets output <c>byte[]</c> of the current module
		/// </summary>
		/// <value>The output <c>byte[]</c>.</value>
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600025F RID: 607 RVA: 0x00003069 File Offset: 0x00001269
		// (set) Token: 0x06000260 RID: 608 RVA: 0x00003071 File Offset: 0x00001271
		public byte[] CurrentModuleOutput
		{
			[CompilerGenerated]
			get
			{
				return this.<CurrentModuleOutput>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<CurrentModuleOutput>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets output <c>byte[]</c> debug symbol of the current module
		/// </summary>
		/// <value>The output <c>byte[]</c> debug symbol.</value>
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000261 RID: 609 RVA: 0x0000307A File Offset: 0x0000127A
		// (set) Token: 0x06000262 RID: 610 RVA: 0x00003082 File Offset: 0x00001282
		public byte[] CurrentModuleSymbol
		{
			[CompilerGenerated]
			get
			{
				return this.<CurrentModuleSymbol>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<CurrentModuleSymbol>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the writer event listener of the current module.
		/// </summary>
		/// <value>The writer event listener.</value>
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000263 RID: 611 RVA: 0x0000308B File Offset: 0x0000128B
		// (set) Token: 0x06000264 RID: 612 RVA: 0x00003093 File Offset: 0x00001293
		public ModuleWriterListener CurrentModuleWriterListener
		{
			[CompilerGenerated]
			get
			{
				return this.<CurrentModuleWriterListener>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<CurrentModuleWriterListener>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the writer options of the current module.
		/// </summary>
		/// <value>The writer options.</value>
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000265 RID: 613 RVA: 0x0000309C File Offset: 0x0000129C
		// (set) Token: 0x06000266 RID: 614 RVA: 0x000030A4 File Offset: 0x000012A4
		public ModuleWriterOptionsBase CurrentModuleWriterOptions
		{
			[CompilerGenerated]
			get
			{
				return this.<CurrentModuleWriterOptions>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<CurrentModuleWriterOptions>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the external modules.
		/// </summary>
		/// <value>The external modules.</value>
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000267 RID: 615 RVA: 0x000030AD File Offset: 0x000012AD
		// (set) Token: 0x06000268 RID: 616 RVA: 0x000030B5 File Offset: 0x000012B5
		public IList<byte[]> ExternalModules
		{
			[CompilerGenerated]
			get
			{
				return this.<ExternalModules>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<ExternalModules>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the logger used for logging events.
		/// </summary>
		/// <value>The logger.</value>
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000269 RID: 617 RVA: 0x000030BE File Offset: 0x000012BE
		// (set) Token: 0x0600026A RID: 618 RVA: 0x000030C6 File Offset: 0x000012C6
		public ILogger Logger
		{
			[CompilerGenerated]
			get
			{
				return this.<Logger>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<Logger>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the modules being protected.
		/// </summary>
		/// <value>The modules being protected.</value>
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600026B RID: 619 RVA: 0x000030CF File Offset: 0x000012CF
		// (set) Token: 0x0600026C RID: 620 RVA: 0x000030D7 File Offset: 0x000012D7
		public IList<ModuleDefMD> Modules
		{
			[CompilerGenerated]
			get
			{
				return this.<Modules>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<Modules>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the output directory.
		/// </summary>
		/// <value>The output directory.</value>
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600026D RID: 621 RVA: 0x000030E0 File Offset: 0x000012E0
		// (set) Token: 0x0600026E RID: 622 RVA: 0x000030E8 File Offset: 0x000012E8
		public string OutputDirectory
		{
			[CompilerGenerated]
			get
			{
				return this.<OutputDirectory>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<OutputDirectory>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the <c>byte[]</c> of modules after protected, or null if module is not protected yet.
		/// </summary>
		/// <value>The list of <c>byte[]</c> of protected modules.</value>
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600026F RID: 623 RVA: 0x000030F1 File Offset: 0x000012F1
		// (set) Token: 0x06000270 RID: 624 RVA: 0x000030F9 File Offset: 0x000012F9
		public IList<byte[]> OutputModules
		{
			[CompilerGenerated]
			get
			{
				return this.<OutputModules>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<OutputModules>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the relative output paths of module, or null if module is not protected yet.
		/// </summary>
		/// <value>The relative output paths of protected modules.</value>
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000271 RID: 625 RVA: 0x00003102 File Offset: 0x00001302
		// (set) Token: 0x06000272 RID: 626 RVA: 0x0000310A File Offset: 0x0000130A
		public IList<string> OutputPaths
		{
			[CompilerGenerated]
			get
			{
				return this.<OutputPaths>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<OutputPaths>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the <c>byte[]</c> of module debug symbols after protected, or null if module is not protected yet.
		/// </summary>
		/// <value>The list of <c>byte[]</c> of module debug symbols.</value>
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000273 RID: 627 RVA: 0x00003113 File Offset: 0x00001313
		// (set) Token: 0x06000274 RID: 628 RVA: 0x0000311B File Offset: 0x0000131B
		public IList<byte[]> OutputSymbols
		{
			[CompilerGenerated]
			get
			{
				return this.<OutputSymbols>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<OutputSymbols>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the packer.
		/// </summary>
		/// <value>The packer.</value>
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000275 RID: 629 RVA: 0x00003124 File Offset: 0x00001324
		// (set) Token: 0x06000276 RID: 630 RVA: 0x0000312C File Offset: 0x0000132C
		public Packer Packer
		{
			[CompilerGenerated]
			get
			{
				return this.<Packer>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<Packer>k__BackingField = value;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000277 RID: 631 RVA: 0x00003135 File Offset: 0x00001335
		// (set) Token: 0x06000278 RID: 632 RVA: 0x0000313D File Offset: 0x0000133D
		internal bool PackerInitiated
		{
			[CompilerGenerated]
			get
			{
				return this.<PackerInitiated>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<PackerInitiated>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the current processing pipeline.
		/// </summary>
		/// <value>The processing pipeline.</value>
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000279 RID: 633 RVA: 0x00003146 File Offset: 0x00001346
		// (set) Token: 0x0600027A RID: 634 RVA: 0x0000314E File Offset: 0x0000134E
		public ProtectionPipeline Pipeline
		{
			[CompilerGenerated]
			get
			{
				return this.<Pipeline>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<Pipeline>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the project being processed.
		/// </summary>
		/// <value>The project.</value>
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600027B RID: 635 RVA: 0x00003157 File Offset: 0x00001357
		// (set) Token: 0x0600027C RID: 636 RVA: 0x0000315F File Offset: 0x0000135F
		public ConfuserProject Project
		{
			[CompilerGenerated]
			get
			{
				return this.<Project>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<Project>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the service registry.
		/// </summary>
		/// <value>The service registry.</value>
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600027D RID: 637 RVA: 0x00011650 File Offset: 0x0000F850
		public ServiceRegistry Registry
		{
			get
			{
				return this.registry;
			}
		}

		/// <summary>
		///     Gets the assembly resolver.
		/// </summary>
		/// <value>The assembly resolver.</value>
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600027E RID: 638 RVA: 0x00003168 File Offset: 0x00001368
		// (set) Token: 0x0600027F RID: 639 RVA: 0x00003170 File Offset: 0x00001370
		public AssemblyResolver Resolver
		{
			[CompilerGenerated]
			get
			{
				return this.<Resolver>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<Resolver>k__BackingField = value;
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00003179 File Offset: 0x00001379
		public ConfuserContext()
		{
		}

		// Token: 0x040001D5 RID: 469
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <BaseDirectory>k__BackingField;

		// Token: 0x040001D6 RID: 470
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private int <CurrentModuleIndex>k__BackingField;

		// Token: 0x040001D7 RID: 471
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private byte[] <CurrentModuleOutput>k__BackingField;

		// Token: 0x040001D8 RID: 472
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte[] <CurrentModuleSymbol>k__BackingField;

		// Token: 0x040001D9 RID: 473
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ModuleWriterListener <CurrentModuleWriterListener>k__BackingField;

		// Token: 0x040001DA RID: 474
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ModuleWriterOptionsBase <CurrentModuleWriterOptions>k__BackingField;

		// Token: 0x040001DB RID: 475
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<byte[]> <ExternalModules>k__BackingField;

		// Token: 0x040001DC RID: 476
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ILogger <Logger>k__BackingField;

		// Token: 0x040001DD RID: 477
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<ModuleDefMD> <Modules>k__BackingField;

		// Token: 0x040001DE RID: 478
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <OutputDirectory>k__BackingField;

		// Token: 0x040001DF RID: 479
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<byte[]> <OutputModules>k__BackingField;

		// Token: 0x040001E0 RID: 480
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<string> <OutputPaths>k__BackingField;

		// Token: 0x040001E1 RID: 481
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<byte[]> <OutputSymbols>k__BackingField;

		// Token: 0x040001E2 RID: 482
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Packer <Packer>k__BackingField;

		// Token: 0x040001E3 RID: 483
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool <PackerInitiated>k__BackingField;

		// Token: 0x040001E4 RID: 484
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ProtectionPipeline <Pipeline>k__BackingField;

		// Token: 0x040001E5 RID: 485
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ConfuserProject <Project>k__BackingField;

		// Token: 0x040001E6 RID: 486
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AssemblyResolver <Resolver>k__BackingField;

		// Token: 0x040001E7 RID: 487
		private readonly Annotations annotations = new Annotations();

		// Token: 0x040001E8 RID: 488
		private readonly ServiceRegistry registry = new ServiceRegistry();

		// Token: 0x040001E9 RID: 489
		internal CancellationToken token;
	}
}
