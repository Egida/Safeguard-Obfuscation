using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000016 RID: 22
	public class VTable
	{
		// Token: 0x0600008E RID: 142 RVA: 0x0000231D File Offset: 0x0000051D
		internal VTable(TypeSig type)
		{
			this.Type = type;
			this.Slots = new List<VTableSlot>();
			this.InterfaceSlots = new Dictionary<TypeSig, IList<VTableSlot>>();
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00002347 File Offset: 0x00000547
		// (set) Token: 0x06000090 RID: 144 RVA: 0x0000234F File Offset: 0x0000054F
		public TypeSig Type
		{
			[CompilerGenerated]
			get
			{
				return this.<Type>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Type>k__BackingField = value;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00002358 File Offset: 0x00000558
		// (set) Token: 0x06000092 RID: 146 RVA: 0x00002360 File Offset: 0x00000560
		public IList<VTableSlot> Slots
		{
			[CompilerGenerated]
			get
			{
				return this.<Slots>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Slots>k__BackingField = value;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00002369 File Offset: 0x00000569
		// (set) Token: 0x06000094 RID: 148 RVA: 0x00002371 File Offset: 0x00000571
		public IDictionary<TypeSig, IList<VTableSlot>> InterfaceSlots
		{
			[CompilerGenerated]
			get
			{
				return this.<InterfaceSlots>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<InterfaceSlots>k__BackingField = value;
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005EE4 File Offset: 0x000040E4
		public IEnumerable<VTableSlot> FindSlots(IMethod method)
		{
			return from slot in this.Slots.Concat(this.InterfaceSlots.SelectMany((KeyValuePair<TypeSig, IList<VTableSlot>> iface) => iface.Value))
			where slot.MethodDef == method
			select slot;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005F4C File Offset: 0x0000414C
		public static VTable ConstructVTable(TypeDef typeDef, VTableStorage storage)
		{
			VTable ret = new VTable(typeDef.ToTypeSig());
			Dictionary<VTableSignature, MethodDef> virtualMethods = (from method in typeDef.Methods
			where method.IsVirtual
			select method).ToDictionary((MethodDef method) => VTableSignature.FromMethod(method), (MethodDef method) => method);
			VTable.VTableConstruction vTbl = new VTable.VTableConstruction();
			VTable baseVTbl = storage.GetVTable(typeDef.GetBaseTypeThrow());
			bool flag = baseVTbl != null;
			if (flag)
			{
				VTable.Inherits(vTbl, baseVTbl);
			}
			foreach (InterfaceImpl iface in typeDef.Interfaces)
			{
				VTable ifaceVTbl = storage.GetVTable(iface.Interface);
				bool flag2 = ifaceVTbl != null;
				if (flag2)
				{
					VTable.Implements(vTbl, virtualMethods, ifaceVTbl, iface.Interface.ToTypeSig());
				}
			}
			bool flag3 = !typeDef.IsInterface;
			if (flag3)
			{
				foreach (Dictionary<VTableSignature, VTableSlot> iface2 in vTbl.InterfaceSlots.Values)
				{
					foreach (KeyValuePair<VTableSignature, VTableSlot> entry in iface2.ToList<KeyValuePair<VTableSignature, VTableSlot>>())
					{
						bool flag4 = !entry.Value.MethodDef.DeclaringType.IsInterface;
						if (!flag4)
						{
							MethodDef impl;
							bool flag5 = virtualMethods.TryGetValue(entry.Key, out impl);
							if (flag5)
							{
								iface2[entry.Key] = entry.Value.OverridedBy(impl);
							}
							else
							{
								VTableSlot implSlot;
								bool flag6 = vTbl.SlotsMap.TryGetValue(entry.Key, out implSlot);
								if (flag6)
								{
									iface2[entry.Key] = entry.Value.OverridedBy(implSlot.MethodDef);
								}
							}
						}
					}
				}
			}
			foreach (KeyValuePair<VTableSignature, MethodDef> method3 in virtualMethods)
			{
				bool isNewSlot = method3.Value.IsNewSlot;
				VTableSlot slot3;
				if (isNewSlot)
				{
					slot3 = new VTableSlot(method3.Value, typeDef.ToTypeSig(), method3.Key);
				}
				else
				{
					bool flag7 = vTbl.SlotsMap.TryGetValue(method3.Key, out slot3);
					if (flag7)
					{
						Debug.Assert(!slot3.MethodDef.IsFinal);
						slot3 = slot3.OverridedBy(method3.Value);
					}
					else
					{
						slot3 = new VTableSlot(method3.Value, typeDef.ToTypeSig(), method3.Key);
					}
				}
				vTbl.SlotsMap[method3.Key] = slot3;
				vTbl.AllSlots.Add(slot3);
			}
			foreach (KeyValuePair<VTableSignature, MethodDef> method2 in virtualMethods)
			{
				foreach (MethodOverride impl2 in method2.Value.Overrides)
				{
					Debug.Assert(impl2.MethodBody == method2.Value);
					MethodDef targetMethod = impl2.MethodDeclaration.ResolveThrow();
					bool isInterface = targetMethod.DeclaringType.IsInterface;
					if (isInterface)
					{
						TypeSig iface3 = impl2.MethodDeclaration.DeclaringType.ToTypeSig();
						VTable.CheckKeyExist<TypeSig, Dictionary<VTableSignature, VTableSlot>>(storage, vTbl.InterfaceSlots, iface3, "MethodImpl Iface");
						Dictionary<VTableSignature, VTableSlot> ifaceVTbl2 = vTbl.InterfaceSlots[iface3];
						VTableSignature signature = VTableSignature.FromMethod(impl2.MethodDeclaration);
						VTable.CheckKeyExist<VTableSignature, VTableSlot>(storage, ifaceVTbl2, signature, "MethodImpl Iface Sig");
						VTableSlot targetSlot = ifaceVTbl2[signature];
						while (targetSlot.Overrides != null)
						{
							targetSlot = targetSlot.Overrides;
						}
						Debug.Assert(targetSlot.MethodDef.DeclaringType.IsInterface);
						ifaceVTbl2[targetSlot.Signature] = targetSlot.OverridedBy(method2.Value);
					}
					else
					{
						VTableSlot targetSlot2 = vTbl.AllSlots.Single((VTableSlot slot) => slot.MethodDef == targetMethod);
						VTable.CheckKeyExist<VTableSignature, VTableSlot>(storage, vTbl.SlotsMap, targetSlot2.Signature, "MethodImpl Normal Sig");
						targetSlot2 = vTbl.SlotsMap[targetSlot2.Signature];
						while (targetSlot2.MethodDef.DeclaringType == typeDef)
						{
							targetSlot2 = targetSlot2.Overrides;
						}
						vTbl.SlotsMap[targetSlot2.Signature] = targetSlot2.OverridedBy(method2.Value);
					}
				}
			}
			ret.InterfaceSlots = vTbl.InterfaceSlots.ToDictionary((KeyValuePair<TypeSig, Dictionary<VTableSignature, VTableSlot>> kvp) => kvp.Key, (KeyValuePair<TypeSig, Dictionary<VTableSignature, VTableSlot>> kvp) => kvp.Value.Values.ToList<VTableSlot>());
			foreach (VTableSlot slot2 in vTbl.AllSlots)
			{
				ret.Slots.Add(slot2);
			}
			return ret;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000065B0 File Offset: 0x000047B0
		private static void Implements(VTable.VTableConstruction vTbl, Dictionary<VTableSignature, MethodDef> virtualMethods, VTable ifaceVTbl, TypeSig iface)
		{
			Func<VTableSlot, VTableSlot> implLookup = delegate(VTableSlot slot)
			{
				MethodDef impl;
				bool flag3 = virtualMethods.TryGetValue(slot.Signature, out impl) && impl.IsNewSlot && !impl.DeclaringType.IsInterface;
				VTableSlot result;
				if (flag3)
				{
					VTableSlot targetSlot = slot;
					while (targetSlot.Overrides != null && !targetSlot.MethodDef.DeclaringType.IsInterface)
					{
						targetSlot = targetSlot.Overrides;
					}
					Debug.Assert(targetSlot.MethodDef.DeclaringType.IsInterface);
					result = targetSlot.OverridedBy(impl);
				}
				else
				{
					result = slot;
				}
				return result;
			};
			bool flag = vTbl.InterfaceSlots.ContainsKey(iface);
			if (flag)
			{
				vTbl.InterfaceSlots[iface] = vTbl.InterfaceSlots[iface].Values.ToDictionary((VTableSlot slot) => slot.Signature, implLookup);
			}
			else
			{
				vTbl.InterfaceSlots.Add(iface, ifaceVTbl.Slots.ToDictionary((VTableSlot slot) => slot.Signature, implLookup));
			}
			foreach (KeyValuePair<TypeSig, IList<VTableSlot>> baseIface in ifaceVTbl.InterfaceSlots)
			{
				bool flag2 = vTbl.InterfaceSlots.ContainsKey(baseIface.Key);
				if (flag2)
				{
					vTbl.InterfaceSlots[baseIface.Key] = vTbl.InterfaceSlots[baseIface.Key].Values.ToDictionary((VTableSlot slot) => slot.Signature, implLookup);
				}
				else
				{
					vTbl.InterfaceSlots.Add(baseIface.Key, baseIface.Value.ToDictionary((VTableSlot slot) => slot.Signature, implLookup));
				}
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000675C File Offset: 0x0000495C
		private static void Inherits(VTable.VTableConstruction vTbl, VTable baseVTbl)
		{
			foreach (VTableSlot slot2 in baseVTbl.Slots)
			{
				vTbl.AllSlots.Add(slot2);
				vTbl.SlotsMap[slot2.Signature] = slot2;
			}
			foreach (KeyValuePair<TypeSig, IList<VTableSlot>> iface in baseVTbl.InterfaceSlots)
			{
				Debug.Assert(!vTbl.InterfaceSlots.ContainsKey(iface.Key));
				vTbl.InterfaceSlots.Add(iface.Key, iface.Value.ToDictionary((VTableSlot slot) => slot.Signature, (VTableSlot slot) => slot));
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006880 File Offset: 0x00004A80
		[Conditional("DEBUG")]
		private static void CheckKeyExist<TKey, TValue>(VTableStorage storage, IDictionary<TKey, TValue> dictionary, TKey key, string name)
		{
			bool flag = !dictionary.ContainsKey(key);
			if (flag)
			{
				storage.GetLogger().ErrorFormat("{0} not found: {1}", new object[]
				{
					name,
					key
				});
				foreach (TKey i in dictionary.Keys)
				{
					storage.GetLogger().ErrorFormat("    {0}", new object[]
					{
						i
					});
				}
			}
		}

		// Token: 0x04000033 RID: 51
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private TypeSig <Type>k__BackingField;

		// Token: 0x04000034 RID: 52
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<VTableSlot> <Slots>k__BackingField;

		// Token: 0x04000035 RID: 53
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IDictionary<TypeSig, IList<VTableSlot>> <InterfaceSlots>k__BackingField;

		// Token: 0x02000017 RID: 23
		private class VTableConstruction
		{
			// Token: 0x0600009A RID: 154 RVA: 0x0000237A File Offset: 0x0000057A
			public VTableConstruction()
			{
			}

			// Token: 0x04000036 RID: 54
			public List<VTableSlot> AllSlots = new List<VTableSlot>();

			// Token: 0x04000037 RID: 55
			public Dictionary<VTableSignature, VTableSlot> SlotsMap = new Dictionary<VTableSignature, VTableSlot>();

			// Token: 0x04000038 RID: 56
			public Dictionary<TypeSig, Dictionary<VTableSignature, VTableSlot>> InterfaceSlots = new Dictionary<TypeSig, Dictionary<VTableSignature, VTableSlot>>(VTable.VTableConstruction.TypeSigComparer.Instance);

			// Token: 0x02000018 RID: 24
			private class TypeSigComparer : IEqualityComparer<TypeSig>
			{
				// Token: 0x0600009B RID: 155 RVA: 0x00006920 File Offset: 0x00004B20
				public bool Equals(TypeSig x, TypeSig y)
				{
					return default(SigComparer).Equals(x, y);
				}

				// Token: 0x0600009C RID: 156 RVA: 0x00006948 File Offset: 0x00004B48
				public int GetHashCode(TypeSig obj)
				{
					return default(SigComparer).GetHashCode(obj);
				}

				// Token: 0x0600009D RID: 157 RVA: 0x00002184 File Offset: 0x00000384
				public TypeSigComparer()
				{
				}

				// Token: 0x0600009E RID: 158 RVA: 0x000023A9 File Offset: 0x000005A9
				// Note: this type is marked as 'beforefieldinit'.
				static TypeSigComparer()
				{
				}

				// Token: 0x04000039 RID: 57
				public static readonly VTable.VTableConstruction.TypeSigComparer Instance = new VTable.VTableConstruction.TypeSigComparer();
			}
		}

		// Token: 0x02000019 RID: 25
		[CompilerGenerated]
		private sealed class <>c__DisplayClass14_0
		{
			// Token: 0x0600009F RID: 159 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass14_0()
			{
			}

			// Token: 0x060000A0 RID: 160 RVA: 0x000023B5 File Offset: 0x000005B5
			internal bool <FindSlots>b__1(VTableSlot slot)
			{
				return slot.MethodDef == this.method;
			}

			// Token: 0x0400003A RID: 58
			public IMethod method;
		}

		// Token: 0x0200001A RID: 26
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060000A1 RID: 161 RVA: 0x000023C5 File Offset: 0x000005C5
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060000A2 RID: 162 RVA: 0x00002184 File Offset: 0x00000384
			public <>c()
			{
			}

			// Token: 0x060000A3 RID: 163 RVA: 0x000023D1 File Offset: 0x000005D1
			internal IEnumerable<VTableSlot> <FindSlots>b__14_0(KeyValuePair<TypeSig, IList<VTableSlot>> iface)
			{
				return iface.Value;
			}

			// Token: 0x060000A4 RID: 164 RVA: 0x000023DA File Offset: 0x000005DA
			internal bool <ConstructVTable>b__15_0(MethodDef method)
			{
				return method.IsVirtual;
			}

			// Token: 0x060000A5 RID: 165 RVA: 0x000023E2 File Offset: 0x000005E2
			internal VTableSignature <ConstructVTable>b__15_1(MethodDef method)
			{
				return VTableSignature.FromMethod(method);
			}

			// Token: 0x060000A6 RID: 166 RVA: 0x000023EA File Offset: 0x000005EA
			internal MethodDef <ConstructVTable>b__15_2(MethodDef method)
			{
				return method;
			}

			// Token: 0x060000A7 RID: 167 RVA: 0x000023ED File Offset: 0x000005ED
			internal TypeSig <ConstructVTable>b__15_4(KeyValuePair<TypeSig, Dictionary<VTableSignature, VTableSlot>> kvp)
			{
				return kvp.Key;
			}

			// Token: 0x060000A8 RID: 168 RVA: 0x000023F6 File Offset: 0x000005F6
			internal IList<VTableSlot> <ConstructVTable>b__15_5(KeyValuePair<TypeSig, Dictionary<VTableSignature, VTableSlot>> kvp)
			{
				return kvp.Value.Values.ToList<VTableSlot>();
			}

			// Token: 0x060000A9 RID: 169 RVA: 0x00002409 File Offset: 0x00000609
			internal VTableSignature <Implements>b__16_1(VTableSlot slot)
			{
				return slot.Signature;
			}

			// Token: 0x060000AA RID: 170 RVA: 0x00002409 File Offset: 0x00000609
			internal VTableSignature <Implements>b__16_2(VTableSlot slot)
			{
				return slot.Signature;
			}

			// Token: 0x060000AB RID: 171 RVA: 0x00002409 File Offset: 0x00000609
			internal VTableSignature <Implements>b__16_3(VTableSlot slot)
			{
				return slot.Signature;
			}

			// Token: 0x060000AC RID: 172 RVA: 0x00002409 File Offset: 0x00000609
			internal VTableSignature <Implements>b__16_4(VTableSlot slot)
			{
				return slot.Signature;
			}

			// Token: 0x060000AD RID: 173 RVA: 0x00002409 File Offset: 0x00000609
			internal VTableSignature <Inherits>b__17_0(VTableSlot slot)
			{
				return slot.Signature;
			}

			// Token: 0x060000AE RID: 174 RVA: 0x000023EA File Offset: 0x000005EA
			internal VTableSlot <Inherits>b__17_1(VTableSlot slot)
			{
				return slot;
			}

			// Token: 0x0400003B RID: 59
			public static readonly VTable.<>c <>9 = new VTable.<>c();

			// Token: 0x0400003C RID: 60
			public static Func<KeyValuePair<TypeSig, IList<VTableSlot>>, IEnumerable<VTableSlot>> <>9__14_0;

			// Token: 0x0400003D RID: 61
			public static Func<MethodDef, bool> <>9__15_0;

			// Token: 0x0400003E RID: 62
			public static Func<MethodDef, VTableSignature> <>9__15_1;

			// Token: 0x0400003F RID: 63
			public static Func<MethodDef, MethodDef> <>9__15_2;

			// Token: 0x04000040 RID: 64
			public static Func<KeyValuePair<TypeSig, Dictionary<VTableSignature, VTableSlot>>, TypeSig> <>9__15_4;

			// Token: 0x04000041 RID: 65
			public static Func<KeyValuePair<TypeSig, Dictionary<VTableSignature, VTableSlot>>, IList<VTableSlot>> <>9__15_5;

			// Token: 0x04000042 RID: 66
			public static Func<VTableSlot, VTableSignature> <>9__16_1;

			// Token: 0x04000043 RID: 67
			public static Func<VTableSlot, VTableSignature> <>9__16_2;

			// Token: 0x04000044 RID: 68
			public static Func<VTableSlot, VTableSignature> <>9__16_3;

			// Token: 0x04000045 RID: 69
			public static Func<VTableSlot, VTableSignature> <>9__16_4;

			// Token: 0x04000046 RID: 70
			public static Func<VTableSlot, VTableSignature> <>9__17_0;

			// Token: 0x04000047 RID: 71
			public static Func<VTableSlot, VTableSlot> <>9__17_1;
		}

		// Token: 0x0200001B RID: 27
		[CompilerGenerated]
		private sealed class <>c__DisplayClass15_0
		{
			// Token: 0x060000AF RID: 175 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass15_0()
			{
			}

			// Token: 0x060000B0 RID: 176 RVA: 0x00002411 File Offset: 0x00000611
			internal bool <ConstructVTable>b__3(VTableSlot slot)
			{
				return slot.MethodDef == this.targetMethod;
			}

			// Token: 0x04000048 RID: 72
			public MethodDef targetMethod;
		}

		// Token: 0x0200001C RID: 28
		[CompilerGenerated]
		private sealed class <>c__DisplayClass16_0
		{
			// Token: 0x060000B1 RID: 177 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass16_0()
			{
			}

			// Token: 0x060000B2 RID: 178 RVA: 0x0000696C File Offset: 0x00004B6C
			internal VTableSlot <Implements>b__0(VTableSlot slot)
			{
				MethodDef impl;
				bool flag = this.virtualMethods.TryGetValue(slot.Signature, out impl) && impl.IsNewSlot && !impl.DeclaringType.IsInterface;
				VTableSlot result;
				if (flag)
				{
					VTableSlot targetSlot = slot;
					while (targetSlot.Overrides != null && !targetSlot.MethodDef.DeclaringType.IsInterface)
					{
						targetSlot = targetSlot.Overrides;
					}
					Debug.Assert(targetSlot.MethodDef.DeclaringType.IsInterface);
					result = targetSlot.OverridedBy(impl);
				}
				else
				{
					result = slot;
				}
				return result;
			}

			// Token: 0x04000049 RID: 73
			public Dictionary<VTableSignature, MethodDef> virtualMethods;
		}
	}
}
