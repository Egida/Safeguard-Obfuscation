using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000025 RID: 37
	internal class BAMLConverterMemberReference : INameReference<IDnlibDef>, INameReference
	{
		// Token: 0x060000D2 RID: 210 RVA: 0x00002569 File Offset: 0x00000769
		public BAMLConverterMemberReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, IDnlibDef member, PropertyRecord rec)
		{
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			this.member = member;
			this.rec = rec;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00006FB0 File Offset: 0x000051B0
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			string typeName = this.sig.ReflectionName;
			string prefix = this.xmlnsCtx.GetPrefix(this.sig.ReflectionNamespace, this.sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module.Assembly);
			bool flag = !string.IsNullOrEmpty(prefix);
			if (flag)
			{
				typeName = prefix + ":" + typeName;
			}
			this.rec.Value = typeName + "." + this.member.Name;
			return true;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000060 RID: 96
		private readonly IDnlibDef member;

		// Token: 0x04000061 RID: 97
		private readonly PropertyRecord rec;

		// Token: 0x04000062 RID: 98
		private readonly TypeSig sig;

		// Token: 0x04000063 RID: 99
		private readonly BAMLAnalyzer.XmlNsContext xmlnsCtx;
	}
}
