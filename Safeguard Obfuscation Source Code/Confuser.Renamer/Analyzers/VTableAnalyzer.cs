using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000083 RID: 131
	internal class VTableAnalyzer : IRenamer
	{
		// Token: 0x060002F0 RID: 752 RVA: 0x00022B24 File Offset: 0x00020D24
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			bool flag = def is TypeDef;
			if (flag)
			{
				TypeDef type = (TypeDef)def;
				bool isInterface = type.IsInterface;
				if (isInterface)
				{
					return;
				}
				VTable vTbl = service.GetVTables()[type];
				using (IEnumerator<IList<VTableSlot>> enumerator = vTbl.InterfaceSlots.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IList<VTableSlot> ifaceVTbl = enumerator.Current;
						foreach (VTableSlot slot in ifaceVTbl)
						{
							bool flag2 = slot.Overrides != null;
							if (flag2)
							{
								bool baseUnderCtrl = context.Modules.Contains(slot.MethodDef.DeclaringType.Module as ModuleDefMD);
								bool ifaceUnderCtrl = context.Modules.Contains(slot.Overrides.MethodDef.DeclaringType.Module as ModuleDefMD);
								bool flag3 = (!baseUnderCtrl && ifaceUnderCtrl) || !service.CanRename(slot.MethodDef);
								if (flag3)
								{
									service.SetCanRename(slot.Overrides.MethodDef, false);
								}
								else
								{
									bool flag4 = (baseUnderCtrl && !ifaceUnderCtrl) || !service.CanRename(slot.Overrides.MethodDef);
									if (flag4)
									{
										service.SetCanRename(slot.MethodDef, false);
									}
								}
							}
						}
					}
					return;
				}
			}
			bool flag5 = def is MethodDef;
			if (flag5)
			{
				MethodDef method = (MethodDef)def;
				bool flag6 = !method.IsVirtual;
				if (!flag6)
				{
					VTable vTbl2 = service.GetVTables()[method.DeclaringType];
					VTableSignature.FromMethod(method);
					IEnumerable<VTableSlot> slots = vTbl2.FindSlots(method);
					bool flag7 = !method.IsAbstract;
					if (flag7)
					{
						using (IEnumerator<VTableSlot> enumerator2 = slots.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								VTableSlot slot2 = enumerator2.Current;
								bool flag8 = slot2.Overrides != null;
								if (flag8)
								{
									service.AddReference<MethodDef>(method, new OverrideDirectiveReference(slot2, slot2.Overrides));
									service.AddReference<MethodDef>(slot2.Overrides.MethodDef, new OverrideDirectiveReference(slot2, slot2.Overrides));
								}
							}
							return;
						}
					}
					foreach (VTableSlot slot3 in slots)
					{
						bool flag9 = slot3.Overrides != null;
						if (flag9)
						{
							service.SetCanRename(method, false);
							service.SetCanRename(slot3.Overrides.MethodDef, false);
						}
					}
				}
			}
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00022E64 File Offset: 0x00021064
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			MethodDef method = def as MethodDef;
			bool flag = method == null || !method.IsVirtual || method.Overrides.Count == 0;
			if (!flag)
			{
				new HashSet<IMethodDefOrRef>(VTableAnalyzer.MethodDefOrRefComparer.Instance);
				method.Overrides.RemoveWhere((MethodOverride impl) => VTableAnalyzer.MethodDefOrRefComparer.Instance.Equals(impl.MethodDeclaration, method));
			}
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00002184 File Offset: 0x00000384
		public VTableAnalyzer()
		{
		}

		// Token: 0x02000084 RID: 132
		private class MethodDefOrRefComparer : IEqualityComparer<IMethodDefOrRef>
		{
			// Token: 0x060002F4 RID: 756 RVA: 0x00003609 File Offset: 0x00001809
			private MethodDefOrRefComparer()
			{
			}

			// Token: 0x060002F5 RID: 757 RVA: 0x00022EE0 File Offset: 0x000210E0
			public bool Equals(IMethodDefOrRef x, IMethodDefOrRef y)
			{
				return default(SigComparer).Equals(x, y) && default(SigComparer).Equals(x.DeclaringType, y.DeclaringType);
			}

			// Token: 0x060002F6 RID: 758 RVA: 0x00022F28 File Offset: 0x00021128
			public int GetHashCode(IMethodDefOrRef obj)
			{
				return default(SigComparer).GetHashCode(obj) * 5 + default(SigComparer).GetHashCode(obj.DeclaringType);
			}

			// Token: 0x060002F7 RID: 759 RVA: 0x00003613 File Offset: 0x00001813
			// Note: this type is marked as 'beforefieldinit'.
			static MethodDefOrRefComparer()
			{
			}

			// Token: 0x0400053E RID: 1342
			public static readonly VTableAnalyzer.MethodDefOrRefComparer Instance = new VTableAnalyzer.MethodDefOrRefComparer();
		}

		// Token: 0x02000085 RID: 133
		[CompilerGenerated]
		private sealed class <>c__DisplayClass1_0
		{
			// Token: 0x060002F8 RID: 760 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass1_0()
			{
			}

			// Token: 0x060002F9 RID: 761 RVA: 0x0000361F File Offset: 0x0000181F
			internal bool <PostRename>b__0(MethodOverride impl)
			{
				return VTableAnalyzer.MethodDefOrRefComparer.Instance.Equals(impl.MethodDeclaration, this.method);
			}

			// Token: 0x0400053F RID: 1343
			public MethodDef method;
		}
	}
}
