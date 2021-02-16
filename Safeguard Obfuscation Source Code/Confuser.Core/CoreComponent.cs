using System;
using Confuser.Core.API;
using Confuser.Core.Services;

namespace Confuser.Core
{
	// Token: 0x0200003A RID: 58
	public class CoreComponent : ConfuserComponent
	{
		// Token: 0x06000143 RID: 323 RVA: 0x00002882 File Offset: 0x00000A82
		internal CoreComponent(ConfuserParameters parameters, Marker marker)
		{
			this.parameters = parameters;
			this.marker = marker;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000144 RID: 324 RVA: 0x0000C780 File Offset: 0x0000A980
		public override string Name
		{
			get
			{
				return "Confuser Core";
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000145 RID: 325 RVA: 0x0000C798 File Offset: 0x0000A998
		public override string Description
		{
			get
			{
				return "Initialization of Confuser core services.";
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000146 RID: 326 RVA: 0x0000C7B0 File Offset: 0x0000A9B0
		public override string Id
		{
			get
			{
				return "Confuser.Core";
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000147 RID: 327 RVA: 0x0000C7B0 File Offset: 0x0000A9B0
		public override string FullId
		{
			get
			{
				return "Confuser.Core";
			}
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000C7C8 File Offset: 0x0000A9C8
		protected internal override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Confuser.Random", typeof(IRandomService), new RandomService(this.parameters.Project.Seed));
			context.Registry.RegisterService("Confuser.Marker", typeof(IMarkerService), new MarkerService(context, this.marker));
			context.Registry.RegisterService("Confuser.Trace", typeof(ITraceService), new TraceService(context));
			context.Registry.RegisterService("Confuser.Runtime", typeof(IRuntimeService), new RuntimeService());
			context.Registry.RegisterService("Confuser.Compression", typeof(ICompressionService), new CompressionService(context));
			context.Registry.RegisterService("Confuser.APIStore", typeof(IAPIStore), new APIStore(context));
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000025D6 File Offset: 0x000007D6
		protected internal override void PopulatePipeline(ProtectionPipeline pipeline)
		{
		}

		// Token: 0x04000139 RID: 313
		public const string _RandomServiceId = "Confuser.Random";

		// Token: 0x0400013A RID: 314
		public const string _MarkerServiceId = "Confuser.Marker";

		// Token: 0x0400013B RID: 315
		public const string _TraceServiceId = "Confuser.Trace";

		// Token: 0x0400013C RID: 316
		public const string _RuntimeServiceId = "Confuser.Runtime";

		// Token: 0x0400013D RID: 317
		public const string _CompressionServiceId = "Confuser.Compression";

		// Token: 0x0400013E RID: 318
		public const string _APIStoreId = "Confuser.APIStore";

		// Token: 0x0400013F RID: 319
		private readonly Marker marker;

		// Token: 0x04000140 RID: 320
		private readonly ConfuserParameters parameters;
	}
}
