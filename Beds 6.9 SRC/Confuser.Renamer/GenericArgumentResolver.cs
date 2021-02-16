using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000004 RID: 4
	public struct GenericArgumentResolver
	{
		// Token: 0x06000010 RID: 16 RVA: 0x00004048 File Offset: 0x00002248
		public static TypeSig Resolve(TypeSig typeSig, IList<TypeSig> typeGenArgs)
		{
			bool flag = typeGenArgs == null;
			if (flag)
			{
				throw new ArgumentException("No generic arguments to resolve.");
			}
			GenericArgumentResolver resolver = default(GenericArgumentResolver);
			resolver.genericArguments = new GenericArguments();
			resolver.recursionCounter = default(RecursionCounter);
			bool flag2 = typeGenArgs != null;
			if (flag2)
			{
				resolver.genericArguments.PushTypeArgs(typeGenArgs);
			}
			return resolver.ResolveGenericArgs(typeSig);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000040B0 File Offset: 0x000022B0
		public static MethodSig Resolve(MethodSig methodSig, IList<TypeSig> typeGenArgs)
		{
			bool flag = typeGenArgs == null;
			if (flag)
			{
				throw new ArgumentException("No generic arguments to resolve.");
			}
			GenericArgumentResolver resolver = default(GenericArgumentResolver);
			resolver.genericArguments = new GenericArguments();
			resolver.recursionCounter = default(RecursionCounter);
			bool flag2 = typeGenArgs != null;
			if (flag2)
			{
				resolver.genericArguments.PushTypeArgs(typeGenArgs);
			}
			return resolver.ResolveGenericArgs(methodSig);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00004118 File Offset: 0x00002318
		private bool ReplaceGenericArg(ref TypeSig typeSig)
		{
			bool flag = this.genericArguments == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				TypeSig newTypeSig = this.genericArguments.Resolve(typeSig);
				bool flag2 = newTypeSig != typeSig;
				if (flag2)
				{
					typeSig = newTypeSig;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00004160 File Offset: 0x00002360
		private MethodSig ResolveGenericArgs(MethodSig sig)
		{
			bool flag = sig == null;
			MethodSig result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				bool flag2 = !this.recursionCounter.Increment();
				if (flag2)
				{
					result2 = null;
				}
				else
				{
					MethodSig result = this.ResolveGenericArgs(new MethodSig(sig.GetCallingConvention()), sig);
					this.recursionCounter.Decrement();
					result2 = result;
				}
			}
			return result2;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000041B8 File Offset: 0x000023B8
		private MethodSig ResolveGenericArgs(MethodSig sig, MethodSig old)
		{
			sig.RetType = this.ResolveGenericArgs(old.RetType);
			foreach (TypeSig p in old.Params)
			{
				sig.Params.Add(this.ResolveGenericArgs(p));
			}
			sig.GenParamCount = old.GenParamCount;
			bool flag = sig.ParamsAfterSentinel != null;
			if (flag)
			{
				foreach (TypeSig p2 in old.ParamsAfterSentinel)
				{
					sig.ParamsAfterSentinel.Add(this.ResolveGenericArgs(p2));
				}
			}
			return sig;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000429C File Offset: 0x0000249C
		private TypeSig ResolveGenericArgs(TypeSig typeSig)
		{
			bool flag = !this.recursionCounter.Increment();
			TypeSig result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				bool flag2 = this.ReplaceGenericArg(ref typeSig);
				if (flag2)
				{
					this.recursionCounter.Decrement();
					result2 = typeSig;
				}
				else
				{
					ElementType elementType = typeSig.ElementType;
					TypeSig result;
					switch (elementType)
					{
					case ElementType.Ptr:
						result = new PtrSig(this.ResolveGenericArgs(typeSig.Next));
						goto IL_294;
					case ElementType.ByRef:
						result = new ByRefSig(this.ResolveGenericArgs(typeSig.Next));
						goto IL_294;
					case ElementType.ValueType:
					case ElementType.Class:
					case ElementType.TypedByRef:
					case ElementType.I:
					case ElementType.U:
					case ElementType.R:
					case ElementType.Object:
						break;
					case ElementType.Var:
						result = new GenericVar((typeSig as GenericVar).Number);
						goto IL_294;
					case ElementType.Array:
					{
						ArraySig arraySig = (ArraySig)typeSig;
						List<uint> sizes = new List<uint>(arraySig.Sizes);
						List<int> lbounds = new List<int>(arraySig.LowerBounds);
						result = new ArraySig(this.ResolveGenericArgs(typeSig.Next), arraySig.Rank, sizes, lbounds);
						goto IL_294;
					}
					case ElementType.GenericInst:
					{
						GenericInstSig gis = (GenericInstSig)typeSig;
						List<TypeSig> genArgs = new List<TypeSig>(gis.GenericArguments.Count);
						foreach (TypeSig ga in gis.GenericArguments)
						{
							genArgs.Add(this.ResolveGenericArgs(ga));
						}
						result = new GenericInstSig(this.ResolveGenericArgs(gis.GenericType) as ClassOrValueTypeSig, genArgs);
						goto IL_294;
					}
					case ElementType.ValueArray:
						result = new ValueArraySig(this.ResolveGenericArgs(typeSig.Next), (typeSig as ValueArraySig).Size);
						goto IL_294;
					case ElementType.FnPtr:
						throw new NotSupportedException("FnPtr is not supported.");
					case ElementType.SZArray:
						result = new SZArraySig(this.ResolveGenericArgs(typeSig.Next));
						goto IL_294;
					case ElementType.MVar:
						result = new GenericMVar((typeSig as GenericMVar).Number);
						goto IL_294;
					case ElementType.CModReqd:
						result = new CModReqdSig((typeSig as ModifierSig).Modifier, this.ResolveGenericArgs(typeSig.Next));
						goto IL_294;
					case ElementType.CModOpt:
						result = new CModOptSig((typeSig as ModifierSig).Modifier, this.ResolveGenericArgs(typeSig.Next));
						goto IL_294;
					default:
					{
						bool flag3 = elementType == ElementType.Module;
						if (flag3)
						{
							result = new ModuleSig((typeSig as ModuleSig).Index, this.ResolveGenericArgs(typeSig.Next));
							goto IL_294;
						}
						bool flag4 = elementType == ElementType.Pinned;
						if (flag4)
						{
							result = new PinnedSig(this.ResolveGenericArgs(typeSig.Next));
							goto IL_294;
						}
						break;
					}
					}
					result = typeSig;
					IL_294:
					this.recursionCounter.Decrement();
					result2 = result;
				}
			}
			return result2;
		}

		// Token: 0x04000001 RID: 1
		private GenericArguments genericArguments;

		// Token: 0x04000002 RID: 2
		private RecursionCounter recursionCounter;
	}
}
