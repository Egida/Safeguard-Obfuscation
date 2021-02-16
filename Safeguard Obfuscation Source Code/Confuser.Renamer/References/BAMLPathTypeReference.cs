using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000020 RID: 32
	internal class BAMLPathTypeReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x0000247F File Offset: 0x0000067F
		public BAMLPathTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, PropertyPathIndexer indexer)
		{
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			this.indexer = indexer;
			this.attachedDP = null;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000024A5 File Offset: 0x000006A5
		public BAMLPathTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, PropertyPathPart attachedDP)
		{
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			this.indexer = null;
			this.attachedDP = attachedDP;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00006DAC File Offset: 0x00004FAC
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			string name = this.sig.ReflectionName;
			string prefix = this.xmlnsCtx.GetPrefix(this.sig.ReflectionNamespace, this.sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module.Assembly);
			bool flag = !string.IsNullOrEmpty(prefix);
			if (flag)
			{
				name = prefix + ":" + name;
			}
			bool flag2 = this.indexer != null;
			if (flag2)
			{
				this.indexer.Type = name;
			}
			else
			{
				string oldType;
				string property;
				this.attachedDP.ExtractAttachedDP(out oldType, out property);
				this.attachedDP.Name = string.Format("({0}.{1})", name, property);
			}
			return true;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00006D98 File Offset: 0x00004F98
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000051 RID: 81
		private readonly PropertyPathPart attachedDP;

		// Token: 0x04000052 RID: 82
		private readonly PropertyPathIndexer indexer;

		// Token: 0x04000053 RID: 83
		private readonly TypeSig sig;

		// Token: 0x04000054 RID: 84
		private readonly BAMLAnalyzer.XmlNsContext xmlnsCtx;
	}
}
