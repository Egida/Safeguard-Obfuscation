using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x0200001D RID: 29
	public class VTableStorage
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x00002421 File Offset: 0x00000621
		public VTableStorage(Confuser.Core.ILogger logger)
		{
			this.logger = logger;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00006A04 File Offset: 0x00004C04
		public Confuser.Core.ILogger GetLogger()
		{
			return this.logger;
		}

		// Token: 0x1700001C RID: 28
		public VTable this[TypeDef type]
		{
			get
			{
				return this.storage.GetValueOrDefault(type, null);
			}
			internal set
			{
				this.storage[type] = value;
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00006A3C File Offset: 0x00004C3C
		private VTable GetOrConstruct(TypeDef type)
		{
			VTable ret;
			bool flag = !this.storage.TryGetValue(type, out ret);
			if (flag)
			{
				ret = (this.storage[type] = VTable.ConstructVTable(type, this));
			}
			return ret;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00006A7C File Offset: 0x00004C7C
		public VTable GetVTable(ITypeDefOrRef type)
		{
			bool flag = type == null;
			VTable result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = type is TypeDef;
				if (flag2)
				{
					result = this.GetOrConstruct((TypeDef)type);
				}
				else
				{
					bool flag3 = type is TypeRef;
					if (flag3)
					{
						result = this.GetOrConstruct(((TypeRef)type).ResolveThrow());
					}
					else
					{
						bool flag4 = type is TypeSpec;
						if (!flag4)
						{
							throw new UnreachableException();
						}
						TypeSig sig = ((TypeSpec)type).TypeSig;
						bool flag5 = sig is TypeDefOrRefSig;
						if (flag5)
						{
							TypeDef typeDef = ((TypeDefOrRefSig)sig).TypeDefOrRef.ResolveTypeDefThrow();
							result = this.GetOrConstruct(typeDef);
						}
						else
						{
							bool flag6 = sig is GenericInstSig;
							if (!flag6)
							{
								throw new NotSupportedException("Unexpected type: " + type);
							}
							GenericInstSig genInst = (GenericInstSig)sig;
							TypeDef openType = genInst.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
							VTable vTable = this.GetOrConstruct(openType);
							result = VTableStorage.ResolveGenericArgument(openType, genInst, vTable);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00006B88 File Offset: 0x00004D88
		private static VTableSlot ResolveSlot(TypeDef openType, VTableSlot slot, IList<TypeSig> genArgs)
		{
			MethodSig newSig = GenericArgumentResolver.Resolve(slot.Signature.MethodSig, genArgs);
			TypeSig newDecl = slot.MethodDefDeclType;
			bool flag = default(SigComparer).Equals(newDecl, openType);
			if (flag)
			{
				newDecl = new GenericInstSig((ClassOrValueTypeSig)openType.ToTypeSig(), genArgs.ToArray<TypeSig>());
			}
			else
			{
				newDecl = GenericArgumentResolver.Resolve(newDecl, genArgs);
			}
			return new VTableSlot(newDecl, slot.MethodDef, slot.DeclaringType, new VTableSignature(newSig, slot.Signature.Name), slot.Overrides);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00006C18 File Offset: 0x00004E18
		private static VTable ResolveGenericArgument(TypeDef openType, GenericInstSig genInst, VTable vTable)
		{
			Debug.Assert(default(SigComparer).Equals(openType, vTable.Type));
			VTable ret = new VTable(genInst);
			foreach (VTableSlot slot2 in vTable.Slots)
			{
				ret.Slots.Add(VTableStorage.ResolveSlot(openType, slot2, genInst.GenericArguments));
			}
			Func<VTableSlot, VTableSlot> <>9__0;
			foreach (KeyValuePair<TypeSig, IList<VTableSlot>> iface in vTable.InterfaceSlots)
			{
				IDictionary<TypeSig, IList<VTableSlot>> interfaceSlots = ret.InterfaceSlots;
				TypeSig key = GenericArgumentResolver.Resolve(iface.Key, genInst.GenericArguments);
				IEnumerable<VTableSlot> value = iface.Value;
				Func<VTableSlot, VTableSlot> selector;
				if ((selector = <>9__0) == null)
				{
					selector = (<>9__0 = ((VTableSlot slot) => VTableStorage.ResolveSlot(openType, slot, genInst.GenericArguments)));
				}
				interfaceSlots.Add(key, value.Select(selector).ToList<VTableSlot>());
			}
			return ret;
		}

		// Token: 0x0400004A RID: 74
		private Dictionary<TypeDef, VTable> storage = new Dictionary<TypeDef, VTable>();

		// Token: 0x0400004B RID: 75
		private Confuser.Core.ILogger logger;

		// Token: 0x0200001E RID: 30
		[CompilerGenerated]
		private sealed class <>c__DisplayClass10_0
		{
			// Token: 0x060000BB RID: 187 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass10_0()
			{
			}

			// Token: 0x060000BC RID: 188 RVA: 0x0000244E File Offset: 0x0000064E
			internal VTableSlot <ResolveGenericArgument>b__0(VTableSlot slot)
			{
				return VTableStorage.ResolveSlot(this.openType, slot, this.genInst.GenericArguments);
			}

			// Token: 0x0400004C RID: 76
			public TypeDef openType;

			// Token: 0x0400004D RID: 77
			public GenericInstSig genInst;

			// Token: 0x0400004E RID: 78
			public Func<VTableSlot, VTableSlot> <>9__0;
		}
	}
}
