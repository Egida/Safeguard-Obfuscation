using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.MildReferenceProxy
{
	// Token: 0x0200007B RID: 123
	internal class RPContext
	{
		// Token: 0x060001F1 RID: 497 RVA: 0x00004A68 File Offset: 0x00002C68
		public RPContext()
		{
		}

		// Token: 0x04000126 RID: 294
		public CilBody Body;

		// Token: 0x04000127 RID: 295
		public HashSet<Instruction> BranchTargets;

		// Token: 0x04000128 RID: 296
		public ConfuserContext Context;

		// Token: 0x04000129 RID: 297
		public Dictionary<MethodSig, TypeDef> Delegates;

		// Token: 0x0400012A RID: 298
		public int Depth;

		// Token: 0x0400012B RID: 299
		public IDynCipherService DynCipher;

		// Token: 0x0400012C RID: 300
		public EncodingType Encoding;

		// Token: 0x0400012D RID: 301
		public IRPEncoding EncodingHandler;

		// Token: 0x0400012E RID: 302
		public int InitCount;

		// Token: 0x0400012F RID: 303
		public bool InternalAlso;

		// Token: 0x04000130 RID: 304
		public IMarkerService Marker;

		// Token: 0x04000131 RID: 305
		public MethodDef Method;

		// Token: 0x04000132 RID: 306
		public Mode Mode;

		// Token: 0x04000133 RID: 307
		public RPMode ModeHandler;

		// Token: 0x04000134 RID: 308
		public ModuleDef Module;

		// Token: 0x04000135 RID: 309
		public INameService Name;

		// Token: 0x04000136 RID: 310
		public MildReferenceProxyProtection Protection;

		// Token: 0x04000137 RID: 311
		public RandomGenerator Random;

		// Token: 0x04000138 RID: 312
		public bool TypeErasure;
	}
}
