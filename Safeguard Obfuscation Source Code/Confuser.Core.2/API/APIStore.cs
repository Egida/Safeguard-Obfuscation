using System;
using System.Collections.Generic;
using Confuser.Core.Services;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	// Token: 0x020000AF RID: 175
	internal class APIStore : IAPIStore
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.API.APIStore" /> class.
		/// </summary>
		/// <param name="context">The working context.</param>
		// Token: 0x060003E4 RID: 996 RVA: 0x00017E80 File Offset: 0x00016080
		public APIStore(ConfuserContext context)
		{
			this.context = context;
			this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator("APIStore");
			this.dataStores = new SortedList<int, List<IDataStore>>();
			this.predicates = new List<IOpaquePredicateDescriptor>();
		}

		/// <inheritdoc />
		// Token: 0x060003E5 RID: 997 RVA: 0x000039AF File Offset: 0x00001BAF
		public void AddStore(IDataStore dataStore)
		{
			this.dataStores.AddListEntry(dataStore.Priority, dataStore);
		}

		/// <inheritdoc />
		// Token: 0x060003E6 RID: 998 RVA: 0x000039C5 File Offset: 0x00001BC5
		public void AddPredicate(IOpaquePredicateDescriptor predicate)
		{
			this.predicates.Add(predicate);
		}

		/// <inheritdoc />
		// Token: 0x060003E7 RID: 999 RVA: 0x00017ED0 File Offset: 0x000160D0
		public IDataStore GetStore(MethodDef method)
		{
			for (int i = this.dataStores.Count - 1; i >= 0; i--)
			{
				List<IDataStore> list = this.dataStores[i];
				int j = list.Count - 1;
				while (j >= 0)
				{
					bool flag = list[j].IsUsable(method);
					if (flag)
					{
						return list[j];
					}
					i--;
				}
			}
			return null;
		}

		/// <inheritdoc />
		// Token: 0x060003E8 RID: 1000 RVA: 0x00017F50 File Offset: 0x00016150
		public IOpaquePredicateDescriptor GetPredicate(MethodDef method, OpaquePredicateType? type, params int[] argCount)
		{
			IOpaquePredicateDescriptor[] randomPredicates = this.predicates.ToArray();
			this.random.Shuffle<IOpaquePredicateDescriptor>(randomPredicates);
			foreach (IOpaquePredicateDescriptor predicate in randomPredicates)
			{
				bool flag = predicate.IsUsable(method) && (type == null || predicate.Type == type.Value) && (argCount == null || Array.IndexOf<int>(argCount, predicate.ArgumentCount) != -1);
				if (flag)
				{
					return predicate;
				}
			}
			return null;
		}

		// Token: 0x04000291 RID: 657
		private readonly ConfuserContext context;

		// Token: 0x04000292 RID: 658
		private readonly RandomGenerator random;

		// Token: 0x04000293 RID: 659
		private readonly SortedList<int, List<IDataStore>> dataStores;

		// Token: 0x04000294 RID: 660
		private readonly List<IOpaquePredicateDescriptor> predicates;
	}
}
