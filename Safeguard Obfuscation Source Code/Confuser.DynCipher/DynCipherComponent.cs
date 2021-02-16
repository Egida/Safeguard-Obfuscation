using System;
using Confuser.Core;

namespace Confuser.DynCipher
{
	// Token: 0x02000002 RID: 2
	internal class DynCipherComponent : ConfuserComponent
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override string Name
		{
			get
			{
				return "Dynamic Cipher";
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		public override string Description
		{
			get
			{
				return "Provides dynamic cipher generation services.";
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
		public override string Id
		{
			get
			{
				return "Confuser.DynCipher";
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002098 File Offset: 0x00000298
		public override string FullId
		{
			get
			{
				return "Confuser.DynCipher";
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020AF File Offset: 0x000002AF
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Confuser.DynCipher", typeof(IDynCipherService), new DynCipherService());
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020D2 File Offset: 0x000002D2
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020D5 File Offset: 0x000002D5
		public DynCipherComponent()
		{
		}

		// Token: 0x04000001 RID: 1
		public const string _ServiceId = "Confuser.DynCipher";
	}
}
