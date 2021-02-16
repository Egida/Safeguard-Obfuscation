using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Renamer;
using Confuser.Renamer.References;
using dnlib.DotNet;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x0200005F RID: 95
	internal abstract class RPMode
	{
		// Token: 0x0600019B RID: 411 RVA: 0x0000D594 File Offset: 0x0000B794
		protected static MethodSig CreateProxySignature(RPContext ctx, IMethod method, bool newObj)
		{
			ModuleDef module = ctx.Module;
			MethodSig result;
			if (newObj)
			{
				TypeSig[] paramTypes = method.MethodSig.Params.Select(delegate(TypeSig type)
				{
					bool flag4 = ctx.TypeErasure && type.IsClassSig && method.MethodSig.HasThis;
					TypeSig result2;
					if (flag4)
					{
						result2 = module.CorLibTypes.Object;
					}
					else
					{
						result2 = type;
					}
					return result2;
				}).ToArray<TypeSig>();
				bool typeErasure = ctx.TypeErasure;
				TypeSig retType;
				if (typeErasure)
				{
					retType = module.CorLibTypes.Object;
				}
				else
				{
					TypeDef declType = method.DeclaringType.ResolveTypeDefThrow();
					retType = RPMode.Import(ctx, declType).ToTypeSig();
				}
				result = MethodSig.CreateStatic(retType, paramTypes);
			}
			else
			{
				IEnumerable<TypeSig> paramTypes2 = method.MethodSig.Params.Select(delegate(TypeSig type)
				{
					bool flag4 = ctx.TypeErasure && type.IsClassSig && method.MethodSig.HasThis;
					TypeSig result2;
					if (flag4)
					{
						result2 = module.CorLibTypes.Object;
					}
					else
					{
						result2 = type;
					}
					return result2;
				});
				bool flag = method.MethodSig.HasThis && !method.MethodSig.ExplicitThis;
				if (flag)
				{
					TypeDef declType2 = method.DeclaringType.ResolveTypeDefThrow();
					bool flag2 = ctx.TypeErasure && !declType2.IsValueType;
					if (flag2)
					{
						paramTypes2 = new CorLibTypeSig[]
						{
							module.CorLibTypes.Object
						}.Concat(paramTypes2);
					}
					else
					{
						paramTypes2 = new TypeSig[]
						{
							RPMode.Import(ctx, declType2).ToTypeSig()
						}.Concat(paramTypes2);
					}
				}
				TypeSig retType2 = method.MethodSig.RetType;
				bool flag3 = ctx.TypeErasure && retType2.IsClassSig;
				if (flag3)
				{
					retType2 = module.CorLibTypes.Object;
				}
				result = MethodSig.CreateStatic(retType2, paramTypes2.ToArray<TypeSig>());
			}
			return result;
		}

		// Token: 0x0600019C RID: 412
		public abstract void Finalize(RPContext ctx);

		// Token: 0x0600019D RID: 413 RVA: 0x0000D778 File Offset: 0x0000B978
		protected static TypeDef GetDelegateType(RPContext ctx, MethodSig sig)
		{
			TypeDef ret;
			bool flag = ctx.Delegates.TryGetValue(sig, out ret);
			TypeDef result;
			if (flag)
			{
				result = ret;
			}
			else
			{
				ret = new TypeDefUser(ctx.Name.ObfuscateName(ctx.Method.DeclaringType.Namespace, RenameMode.Sequential), ctx.Name.RandomName(), ctx.Module.CorLibTypes.GetTypeRef("System", "MulticastDelegate"));
				ret.Attributes = TypeAttributes.Sealed;
				MethodDefUser ctor = new MethodDefUser(".ctor", MethodSig.CreateInstance(ctx.Module.CorLibTypes.Void, ctx.Module.CorLibTypes.Object, ctx.Module.CorLibTypes.IntPtr));
				ctor.Attributes = (MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
				ctor.ImplAttributes = MethodImplAttributes.CodeTypeMask;
				ret.Methods.Add(ctor);
				MethodDefUser invoke = new MethodDefUser("Invoke", sig.Clone());
				invoke.MethodSig.HasThis = true;
				invoke.Attributes = (MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask);
				invoke.ImplAttributes = MethodImplAttributes.CodeTypeMask;
				ret.Methods.Add(invoke);
				ctx.Module.Types.Add(ret);
				foreach (IDnlibDef def in ret.FindDefinitions())
				{
					ctx.Marker.Mark(def, ctx.Protection);
					ctx.Name.SetCanRename(def, false);
				}
				ctx.Delegates[sig] = ret;
				result = ret;
			}
			return result;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000D93C File Offset: 0x0000BB3C
		private static ITypeDefOrRef Import(RPContext ctx, TypeDef typeDef)
		{
			ITypeDefOrRef retTypeRef = new Importer(ctx.Module, ImporterOptions.TryToUseTypeDefs).Import(typeDef);
			bool flag = typeDef.Module != ctx.Module && ctx.Context.Modules.Contains((ModuleDefMD)typeDef.Module);
			if (flag)
			{
				ctx.Name.AddReference<TypeDef>(typeDef, new TypeRefReference((TypeRef)retTypeRef, typeDef));
			}
			return retTypeRef;
		}

		// Token: 0x0600019F RID: 415
		public abstract void ProcessCall(RPContext ctx, int instrIndex);

		// Token: 0x060001A0 RID: 416 RVA: 0x00004A68 File Offset: 0x00002C68
		protected RPMode()
		{
		}

		// Token: 0x02000060 RID: 96
		[CompilerGenerated]
		private sealed class <>c__DisplayClass0_0
		{
			// Token: 0x060001A1 RID: 417 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass0_0()
			{
			}

			// Token: 0x060001A2 RID: 418 RVA: 0x0000D9B0 File Offset: 0x0000BBB0
			internal TypeSig <CreateProxySignature>b__0(TypeSig type)
			{
				bool flag = this.ctx.TypeErasure && type.IsClassSig && this.method.MethodSig.HasThis;
				TypeSig result;
				if (flag)
				{
					result = this.module.CorLibTypes.Object;
				}
				else
				{
					result = type;
				}
				return result;
			}

			// Token: 0x060001A3 RID: 419 RVA: 0x0000D9B0 File Offset: 0x0000BBB0
			internal TypeSig <CreateProxySignature>b__1(TypeSig type)
			{
				bool flag = this.ctx.TypeErasure && type.IsClassSig && this.method.MethodSig.HasThis;
				TypeSig result;
				if (flag)
				{
					result = this.module.CorLibTypes.Object;
				}
				else
				{
					result = type;
				}
				return result;
			}

			// Token: 0x040000C9 RID: 201
			public RPContext ctx;

			// Token: 0x040000CA RID: 202
			public IMethod method;

			// Token: 0x040000CB RID: 203
			public ModuleDef module;
		}
	}
}
