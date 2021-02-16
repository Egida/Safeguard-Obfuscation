using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000024 RID: 36
	internal class BAMLConverterTypeReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000CE RID: 206 RVA: 0x0000252B File Offset: 0x0000072B
		public BAMLConverterTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, PropertyRecord rec)
		{
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			this.propRec = rec;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000254A File Offset: 0x0000074A
		public BAMLConverterTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, TextRecord rec)
		{
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			this.textRec = rec;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00006F18 File Offset: 0x00005118
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			string name = this.sig.ReflectionName;
			string prefix = this.xmlnsCtx.GetPrefix(this.sig.ReflectionNamespace, this.sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module.Assembly);
			bool flag = !string.IsNullOrEmpty(prefix);
			if (flag)
			{
				name = prefix + ":" + name;
			}
			bool flag2 = this.propRec != null;
			if (flag2)
			{
				this.propRec.Value = name;
			}
			else
			{
				this.textRec.Value = name;
			}
			return true;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x0400005C RID: 92
		private readonly PropertyRecord propRec;

		// Token: 0x0400005D RID: 93
		private readonly TypeSig sig;

		// Token: 0x0400005E RID: 94
		private readonly TextRecord textRec;

		// Token: 0x0400005F RID: 95
		private readonly BAMLAnalyzer.XmlNsContext xmlnsCtx;
	}
}
