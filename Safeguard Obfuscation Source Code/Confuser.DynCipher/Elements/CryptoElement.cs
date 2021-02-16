using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x0200001B RID: 27
	internal abstract class CryptoElement
	{
		// Token: 0x0600007B RID: 123 RVA: 0x0000579C File Offset: 0x0000399C
		public CryptoElement(int count)
		{
			this.DataCount = count;
			this.DataIndexes = new int[count];
		}

		// Token: 0x0600007C RID: 124
		public abstract void Emit(CipherGenContext context);

		// Token: 0x0600007D RID: 125
		public abstract void EmitInverse(CipherGenContext context);

		// Token: 0x0600007E RID: 126
		public abstract void Initialize(RandomGenerator random);

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000057BB File Offset: 0x000039BB
		// (set) Token: 0x06000080 RID: 128 RVA: 0x000057C3 File Offset: 0x000039C3
		public int DataCount
		{
			[CompilerGenerated]
			get
			{
				return this.<DataCount>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<DataCount>k__BackingField = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000081 RID: 129 RVA: 0x000057CC File Offset: 0x000039CC
		// (set) Token: 0x06000082 RID: 130 RVA: 0x000057D4 File Offset: 0x000039D4
		public int[] DataIndexes
		{
			[CompilerGenerated]
			get
			{
				return this.<DataIndexes>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<DataIndexes>k__BackingField = value;
			}
		}

		// Token: 0x04000038 RID: 56
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int <DataCount>k__BackingField;

		// Token: 0x04000039 RID: 57
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int[] <DataIndexes>k__BackingField;
	}
}
