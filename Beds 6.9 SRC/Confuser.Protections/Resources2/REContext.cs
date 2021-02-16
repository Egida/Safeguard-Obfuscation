using System;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;

namespace Confuser.Protections.Resources2
{
	// Token: 0x02000056 RID: 86
	internal class REContext
	{
		// Token: 0x0600017F RID: 383 RVA: 0x00004A68 File Offset: 0x00002C68
		public REContext()
		{
		}

		// Token: 0x040000B3 RID: 179
		public ConfuserContext Context;

		// Token: 0x040000B4 RID: 180
		public FieldDef DataField;

		// Token: 0x040000B5 RID: 181
		public FieldDef DataField1;

		// Token: 0x040000B6 RID: 182
		public TypeDef DataType;

		// Token: 0x040000B7 RID: 183
		public IDynCipherService DynCipher;

		// Token: 0x040000B8 RID: 184
		public MethodDef InitMethod;

		// Token: 0x040000B9 RID: 185
		public IMarkerService Marker;

		// Token: 0x040000BA RID: 186
		public Mode Mode;

		// Token: 0x040000BB RID: 187
		public IEncodeMode ModeHandler;

		// Token: 0x040000BC RID: 188
		public ModuleDef Module;

		// Token: 0x040000BD RID: 189
		public INameService Name;

		// Token: 0x040000BE RID: 190
		public RandomGenerator Random;
	}
}
