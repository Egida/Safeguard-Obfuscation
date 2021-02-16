using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000028 RID: 40
	internal class OverrideDirectiveReference : INameReference<MethodDef>, INameReference
	{
		// Token: 0x060000DB RID: 219 RVA: 0x000025C0 File Offset: 0x000007C0
		public OverrideDirectiveReference(VTableSlot thisSlot, VTableSlot baseSlot)
		{
			this.thisSlot = thisSlot;
			this.baseSlot = baseSlot;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000070B4 File Offset: 0x000052B4
		private void AddImportReference(ConfuserContext context, INameService service, ModuleDef module, MethodDef method, MemberRef methodRef)
		{
			bool flag = method.Module != module && context.Modules.Contains((ModuleDefMD)method.Module);
			if (flag)
			{
				TypeRef declType = (TypeRef)methodRef.DeclaringType.ScopeType;
				service.AddReference<TypeDef>(method.DeclaringType, new TypeRefReference(declType, method.DeclaringType));
				service.AddReference<IDnlibDef>(method, new MemberRefReference(methodRef, method));
				List<ITypeDefOrRef> typeRefs = methodRef.MethodSig.Params.SelectMany((TypeSig param) => param.FindTypeRefs()).ToList<ITypeDefOrRef>();
				typeRefs.AddRange(methodRef.MethodSig.RetType.FindTypeRefs());
				foreach (ITypeDefOrRef typeRef in typeRefs)
				{
					TypeDef def = typeRef.ResolveTypeDefThrow();
					bool flag2 = def.Module != module && context.Modules.Contains((ModuleDefMD)def.Module);
					if (flag2)
					{
						service.AddReference<TypeDef>(def, new TypeRefReference((TypeRef)typeRef, def));
					}
				}
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00007208 File Offset: 0x00005408
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			MethodDef method = this.thisSlot.MethodDef;
			bool flag = this.baseSlot.MethodDefDeclType is GenericInstSig;
			IMethod target;
			if (flag)
			{
				GenericInstSig declType = (GenericInstSig)this.baseSlot.MethodDefDeclType;
				MemberRef targetRef = new MemberRefUser(method.Module, this.baseSlot.MethodDef.Name, this.baseSlot.MethodDef.MethodSig, declType.ToTypeDefOrRef());
				targetRef = new Importer(method.Module, ImporterOptions.TryToUseTypeDefs).Import(targetRef);
				service.AddReference<IDnlibDef>(this.baseSlot.MethodDef, new MemberRefReference(targetRef, this.baseSlot.MethodDef));
				target = targetRef;
			}
			else
			{
				target = this.baseSlot.MethodDef;
				bool flag2 = target.Module != method.Module;
				if (flag2)
				{
					target = new Importer(method.Module, ImporterOptions.TryToUseTypeDefs).Import(this.baseSlot.MethodDef);
					bool flag3 = target is MemberRef;
					if (flag3)
					{
						service.AddReference<IDnlibDef>(this.baseSlot.MethodDef, new MemberRefReference((MemberRef)target, this.baseSlot.MethodDef));
					}
				}
			}
			target.MethodSig = new Importer(method.Module, ImporterOptions.TryToUseTypeDefs).Import(method.MethodSig);
			bool flag4 = target is MemberRef;
			if (flag4)
			{
				this.AddImportReference(context, service, method.Module, this.baseSlot.MethodDef, (MemberRef)target);
			}
			bool flag5 = method.Overrides.Any((MethodOverride impl) => default(SigComparer).Equals(impl.MethodDeclaration.MethodSig, target.MethodSig) && default(SigComparer).Equals(impl.MethodDeclaration.DeclaringType.ResolveTypeDef(), target.DeclaringType.ResolveTypeDef()));
			bool result;
			if (flag5)
			{
				result = true;
			}
			else
			{
				method.Overrides.Add(new MethodOverride(method, (IMethodDefOrRef)target));
				result = true;
			}
			return result;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00007410 File Offset: 0x00005610
		public bool ShouldCancelRename()
		{
			return this.baseSlot.MethodDefDeclType is GenericInstSig && this.thisSlot.MethodDef.Module.IsClr20;
		}

		// Token: 0x04000068 RID: 104
		private readonly VTableSlot baseSlot;

		// Token: 0x04000069 RID: 105
		private readonly VTableSlot thisSlot;

		// Token: 0x02000029 RID: 41
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060000DF RID: 223 RVA: 0x000025D8 File Offset: 0x000007D8
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060000E0 RID: 224 RVA: 0x00002184 File Offset: 0x00000384
			public <>c()
			{
			}

			// Token: 0x060000E1 RID: 225 RVA: 0x000025E4 File Offset: 0x000007E4
			internal IEnumerable<ITypeDefOrRef> <AddImportReference>b__3_0(TypeSig param)
			{
				return param.FindTypeRefs();
			}

			// Token: 0x0400006A RID: 106
			public static readonly OverrideDirectiveReference.<>c <>9 = new OverrideDirectiveReference.<>c();

			// Token: 0x0400006B RID: 107
			public static Func<TypeSig, IEnumerable<ITypeDefOrRef>> <>9__3_0;
		}

		// Token: 0x0200002A RID: 42
		[CompilerGenerated]
		private sealed class <>c__DisplayClass4_0
		{
			// Token: 0x060000E2 RID: 226 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass4_0()
			{
			}

			// Token: 0x060000E3 RID: 227 RVA: 0x0000744C File Offset: 0x0000564C
			internal bool <UpdateNameReference>b__0(MethodOverride impl)
			{
				return default(SigComparer).Equals(impl.MethodDeclaration.MethodSig, this.target.MethodSig) && default(SigComparer).Equals(impl.MethodDeclaration.DeclaringType.ResolveTypeDef(), this.target.DeclaringType.ResolveTypeDef());
			}

			// Token: 0x0400006C RID: 108
			public IMethod target;
		}
	}
}
