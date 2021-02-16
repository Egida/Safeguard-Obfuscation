using System;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;

namespace Confuser.Protections.Resources
{
	// Token: 0x02000049 RID: 73
	internal class REContext
	{
		// Token: 0x0600015A RID: 346 RVA: 0x00004A68 File Offset: 0x00002C68
		public REContext()
		{
		}

		// Token: 0x0400008A RID: 138
		public ConfuserContext Context;

		// Token: 0x0400008B RID: 139
		public FieldDef DataField;

		// Token: 0x0400008C RID: 140
		public FieldDef DataField1;

		// Token: 0x0400008D RID: 141
		public TypeDef DataType;

		// Token: 0x0400008E RID: 142
		public IDynCipherService DynCipher;

		// Token: 0x0400008F RID: 143
		public MethodDef InitMethod;

		// Token: 0x04000090 RID: 144
		public IMarkerService Marker;

		// Token: 0x04000091 RID: 145
		public Mode Mode;

		// Token: 0x04000092 RID: 146
		public IEncodeMode ModeHandler;

		// Token: 0x04000093 RID: 147
		public ModuleDef Module;

		// Token: 0x04000094 RID: 148
		public INameService Name;

		// Token: 0x04000095 RID: 149
		public RandomGenerator Random;
	}
}
