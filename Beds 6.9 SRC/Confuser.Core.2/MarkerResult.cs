using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Result of the marker.
	/// </summary>
	// Token: 0x0200004D RID: 77
	public class MarkerResult
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.MarkerResult" /> class.
		/// </summary>
		/// <param name="modules">The modules.</param>
		/// <param name="packer">The packer.</param>
		/// <param name="extModules">The external modules.</param>
		// Token: 0x060001D6 RID: 470 RVA: 0x00002C56 File Offset: 0x00000E56
		public MarkerResult(IList<ModuleDefMD> modules, Packer packer, IList<byte[]> extModules)
		{
			this.Modules = modules;
			this.Packer = packer;
			this.ExternalModules = extModules;
		}

		/// <summary>
		///     Gets a list of external modules.
		/// </summary>
		/// <value>The list of external modules.</value>
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x00002C78 File Offset: 0x00000E78
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x00002C80 File Offset: 0x00000E80
		public IList<byte[]> ExternalModules
		{
			[CompilerGenerated]
			get
			{
				return this.<ExternalModules>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ExternalModules>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets a list of modules that is marked.
		/// </summary>
		/// <value>The list of modules.</value>
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x00002C89 File Offset: 0x00000E89
		// (set) Token: 0x060001DA RID: 474 RVA: 0x00002C91 File Offset: 0x00000E91
		public IList<ModuleDefMD> Modules
		{
			[CompilerGenerated]
			get
			{
				return this.<Modules>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Modules>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets the packer if exists.
		/// </summary>
		/// <value>The packer, or null if no packer exists.</value>
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060001DB RID: 475 RVA: 0x00002C9A File Offset: 0x00000E9A
		// (set) Token: 0x060001DC RID: 476 RVA: 0x00002CA2 File Offset: 0x00000EA2
		public Packer Packer
		{
			[CompilerGenerated]
			get
			{
				return this.<Packer>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Packer>k__BackingField = value;
			}
		}

		// Token: 0x0400018C RID: 396
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<byte[]> <ExternalModules>k__BackingField;

		// Token: 0x0400018D RID: 397
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<ModuleDefMD> <Modules>k__BackingField;

		// Token: 0x0400018E RID: 398
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private Packer <Packer>k__BackingField;
	}
}
