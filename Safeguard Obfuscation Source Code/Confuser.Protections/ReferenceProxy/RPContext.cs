using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x0200006F RID: 111
	internal class RPContext
	{
		// Token: 0x060001CF RID: 463 RVA: 0x00004A68 File Offset: 0x00002C68
		public RPContext()
		{
		}

		// Token: 0x040000FB RID: 251
		public ReferenceProxyProtection Protection;

		// Token: 0x040000FC RID: 252
		public CilBody Body;

		// Token: 0x040000FD RID: 253
		public HashSet<Instruction> BranchTargets;

		// Token: 0x040000FE RID: 254
		public ConfuserContext Context;

		// Token: 0x040000FF RID: 255
		public Dictionary<MethodSig, TypeDef> Delegates;

		// Token: 0x04000100 RID: 256
		public int Depth;

		// Token: 0x04000101 RID: 257
		public IDynCipherService DynCipher;

		// Token: 0x04000102 RID: 258
		public EncodingType Encoding;

		// Token: 0x04000103 RID: 259
		public IRPEncoding EncodingHandler;

		// Token: 0x04000104 RID: 260
		public int InitCount;

		// Token: 0x04000105 RID: 261
		public bool InternalAlso;

		// Token: 0x04000106 RID: 262
		public IMarkerService Marker;

		// Token: 0x04000107 RID: 263
		public MethodDef Method;

		// Token: 0x04000108 RID: 264
		public Mode Mode;

		// Token: 0x04000109 RID: 265
		public RPMode ModeHandler;

		// Token: 0x0400010A RID: 266
		public ModuleDef Module;

		// Token: 0x0400010B RID: 267
		public INameService Name;

		// Token: 0x0400010C RID: 268
		public RandomGenerator Random;

		// Token: 0x0400010D RID: 269
		public bool TypeErasure;
	}
}
