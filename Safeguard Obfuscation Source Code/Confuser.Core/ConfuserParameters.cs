using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Project;

namespace Confuser.Core
{
	// Token: 0x02000039 RID: 57
	public class ConfuserParameters
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000135 RID: 309 RVA: 0x0000282D File Offset: 0x00000A2D
		// (set) Token: 0x06000136 RID: 310 RVA: 0x00002835 File Offset: 0x00000A35
		public ConfuserProject Project
		{
			[CompilerGenerated]
			get
			{
				return this.<Project>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Project>k__BackingField = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000137 RID: 311 RVA: 0x0000283E File Offset: 0x00000A3E
		// (set) Token: 0x06000138 RID: 312 RVA: 0x00002846 File Offset: 0x00000A46
		public ILogger Logger
		{
			[CompilerGenerated]
			get
			{
				return this.<Logger>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Logger>k__BackingField = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000139 RID: 313 RVA: 0x0000284F File Offset: 0x00000A4F
		// (set) Token: 0x0600013A RID: 314 RVA: 0x00002857 File Offset: 0x00000A57
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00002860 File Offset: 0x00000A60
		// (set) Token: 0x0600013C RID: 316 RVA: 0x00002868 File Offset: 0x00000A68
		public PluginDiscovery PluginDiscovery
		{
			[CompilerGenerated]
			get
			{
				return this.<PluginDiscovery>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<PluginDiscovery>k__BackingField = value;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00002871 File Offset: 0x00000A71
		// (set) Token: 0x0600013E RID: 318 RVA: 0x00002879 File Offset: 0x00000A79
		public Marker Marker
		{
			[CompilerGenerated]
			get
			{
				return this.<Marker>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Marker>k__BackingField = value;
			}
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000C714 File Offset: 0x0000A914
		internal ILogger GetLogger()
		{
			return this.Logger ?? NullLogger.Instance;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000C738 File Offset: 0x0000A938
		internal PluginDiscovery GetPluginDiscovery()
		{
			return this.PluginDiscovery ?? PluginDiscovery.Instance;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000C75C File Offset: 0x0000A95C
		internal Marker GetMarker()
		{
			return this.Marker ?? new ObfAttrMarker();
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00002194 File Offset: 0x00000394
		public ConfuserParameters()
		{
		}

		// Token: 0x04000134 RID: 308
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ConfuserProject <Project>k__BackingField;

		// Token: 0x04000135 RID: 309
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ILogger <Logger>k__BackingField;

		// Token: 0x04000136 RID: 310
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool <PackerInitiated>k__BackingField;

		// Token: 0x04000137 RID: 311
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PluginDiscovery <PluginDiscovery>k__BackingField;

		// Token: 0x04000138 RID: 312
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Marker <Marker>k__BackingField;
	}
}
