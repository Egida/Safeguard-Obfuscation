using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Renamer;
using Confuser.Renamer.References;
using dnlib.DotNet;

namespace Confuser.Protections.MildReferenceProxy
{
	// Token: 0x0200007C RID: 124
	internal abstract class RPMode
	{
		// Token: 0x060001F2 RID: 498 RVA: 0x0000538C File Offset: 0x0000358C
		protected RPMode()
		{
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000FF44 File Offset: 0x0000E144
		protected static MethodSig CreateProxySignature(RPContext ctx, IMethod method, bool newObj)
		{
			Func<TypeSig, TypeSig> selector = null;
			Func<TypeSig, TypeSig> func2 = null;
			ModuleDef module = ctx.Module;
			MethodSig result;
			if (newObj)
			{
				bool flag = selector == null;
				if (flag)
				{
					selector = delegate(TypeSig type)
					{
						bool flag6 = ctx.TypeErasure && type.IsClassSig && method.MethodSig.HasThis;
						TypeSig result2;
						if (flag6)
						{
							result2 = module.CorLibTypes.Object;
						}
						else
						{
							result2 = type;
						}
						return result2;
					};
				}
				TypeSig[] argTypes = method.MethodSig.Params.Select(selector).ToArray<TypeSig>();
				bool typeErasure = ctx.TypeErasure;
				TypeSig sig;
				if (typeErasure)
				{
					sig = module.CorLibTypes.Object;
				}
				else
				{
					TypeDef typeDef = method.DeclaringType.ResolveTypeDefThrow();
					sig = RPMode.Import(ctx, typeDef).ToTypeSig();
				}
				result = MethodSig.CreateStatic(sig, argTypes);
			}
			else
			{
				bool flag2 = func2 == null;
				if (flag2)
				{
					func2 = delegate(TypeSig type)
					{
						bool flag6 = ctx.TypeErasure && type.IsClassSig && method.MethodSig.HasThis;
						TypeSig result2;
						if (flag6)
						{
							result2 = module.CorLibTypes.Object;
						}
						else
						{
							result2 = type;
						}
						return result2;
					};
				}
				IEnumerable<TypeSig> second = method.MethodSig.Params.Select(func2);
				bool flag3 = method.MethodSig.HasThis && !method.MethodSig.ExplicitThis;
				if (flag3)
				{
					TypeDef def2 = method.DeclaringType.ResolveTypeDefThrow();
					bool flag4 = ctx.TypeErasure && !def2.IsValueType;
					if (flag4)
					{
						second = new CorLibTypeSig[]
						{
							module.CorLibTypes.Object
						}.Concat(second);
					}
					else
					{
						second = new TypeSig[]
						{
							RPMode.Import(ctx, def2).ToTypeSig()
						}.Concat(second);
					}
				}
				TypeSig retType = method.MethodSig.RetType;
				bool flag5 = ctx.TypeErasure && retType.IsClassSig;
				if (flag5)
				{
					retType = module.CorLibTypes.Object;
				}
				result = MethodSig.CreateStatic(retType, second.ToArray<TypeSig>());
			}
			return result;
		}

		// Token: 0x060001F4 RID: 500
		public abstract void Finalize(RPContext ctx);

		// Token: 0x060001F5 RID: 501 RVA: 0x0001014C File Offset: 0x0000E34C
		protected static TypeDef GetDelegateType(RPContext ctx, MethodSig sig)
		{
			TypeDef def;
			bool flag = !ctx.Delegates.TryGetValue(sig, out def);
			if (flag)
			{
				def = new TypeDefUser(ctx.Name.ObfuscateName(ctx.Method.DeclaringType.Namespace, RenameMode.Sequential), ctx.Name.RandomName(), ctx.Module.CorLibTypes.GetTypeRef("System", "MulticastDelegate"))
				{
					Attributes = TypeAttributes.Sealed
				};
				MethodDefUser item = new MethodDefUser(".ctor", MethodSig.CreateInstance(ctx.Module.CorLibTypes.Void, ctx.Module.CorLibTypes.Object, ctx.Module.CorLibTypes.IntPtr))
				{
					Attributes = (MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName),
					ImplAttributes = MethodImplAttributes.CodeTypeMask
				};
				def.Methods.Add(item);
				MethodDefUser user2 = new MethodDefUser("Invoke", sig.Clone())
				{
					MethodSig = 
					{
						HasThis = true
					},
					Attributes = (MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask),
					ImplAttributes = MethodImplAttributes.CodeTypeMask
				};
				def.Methods.Add(user2);
				ctx.Module.Types.Add(def);
				foreach (IDnlibDef def2 in def.FindDefinitions())
				{
					ctx.Marker.Mark(def2, ctx.Protection);
					ctx.Name.SetCanRename(def2, false);
				}
				ctx.Delegates[sig] = def;
			}
			return def;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00010310 File Offset: 0x0000E510
		private static ITypeDefOrRef Import(RPContext ctx, TypeDef typeDef)
		{
			ITypeDefOrRef ref2 = new Importer(ctx.Module, ImporterOptions.TryToUseTypeDefs).Import(typeDef);
			bool flag = typeDef.Module != ctx.Module && ctx.Context.Modules.Contains((ModuleDefMD)typeDef.Module);
			if (flag)
			{
				ctx.Name.AddReference<TypeDef>(typeDef, new TypeRefReference((TypeRef)ref2, typeDef));
			}
			return ref2;
		}

		// Token: 0x060001F7 RID: 503
		public abstract void ProcessCall(RPContext ctx, int instrIndex);

		// Token: 0x0200007D RID: 125
		[CompilerGenerated]
		private sealed class <>c__DisplayClass1_0
		{
			// Token: 0x060001F8 RID: 504 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass1_0()
			{
			}

			// Token: 0x060001F9 RID: 505 RVA: 0x00010384 File Offset: 0x0000E584
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

			// Token: 0x060001FA RID: 506 RVA: 0x00010384 File Offset: 0x0000E584
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

			// Token: 0x04000139 RID: 313
			public RPContext ctx;

			// Token: 0x0400013A RID: 314
			public IMethod method;

			// Token: 0x0400013B RID: 315
			public ModuleDef module;
		}
	}
}
