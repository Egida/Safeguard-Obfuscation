using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x02000088 RID: 136
	internal class CEContext
	{
		// Token: 0x0600021E RID: 542 RVA: 0x00004A68 File Offset: 0x00002C68
		public CEContext()
		{
		}

		// Token: 0x0400015B RID: 347
		public ConfuserContext Context;

		// Token: 0x0400015C RID: 348
		public ConstantProtection Protection;

		// Token: 0x0400015D RID: 349
		public ModuleDef Module;

		// Token: 0x0400015E RID: 350
		public FieldDef BufferField;

		// Token: 0x0400015F RID: 351
		public FieldDef DataField;

		// Token: 0x04000160 RID: 352
		public TypeDef DataType;

		// Token: 0x04000161 RID: 353
		public MethodDef InitMethod;

		// Token: 0x04000162 RID: 354
		public int DecoderCount;

		// Token: 0x04000163 RID: 355
		public List<Tuple<MethodDef, DecoderDesc>> Decoders;

		// Token: 0x04000164 RID: 356
		public EncodeElements Elements;

		// Token: 0x04000165 RID: 357
		public List<uint> EncodedBuffer;

		// Token: 0x04000166 RID: 358
		public Mode Mode;

		// Token: 0x04000167 RID: 359
		public IEncodeMode ModeHandler;

		// Token: 0x04000168 RID: 360
		public IDynCipherService DynCipher;

		// Token: 0x04000169 RID: 361
		public IMarkerService Marker;

		// Token: 0x0400016A RID: 362
		public INameService Name;

		// Token: 0x0400016B RID: 363
		public RandomGenerator Random;

		// Token: 0x0400016C RID: 364
		public TypeDef CfgCtxType;

		// Token: 0x0400016D RID: 365
		public MethodDef CfgCtxCtor;

		// Token: 0x0400016E RID: 366
		public MethodDef CfgCtxNext;

		// Token: 0x0400016F RID: 367
		public Dictionary<MethodDef, List<Tuple<Instruction, uint, IMethod>>> ReferenceRepl;
	}
}
