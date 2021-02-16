using System;
using System.Collections.Generic;
using System.Linq;

namespace Confuser.Renamer
{
	// Token: 0x02000003 RID: 3
	public static class Class1
	{
		// Token: 0x0600000F RID: 15 RVA: 0x0000401C File Offset: 0x0000221C
		public static T RandomElement<T>(this IEnumerable<T> coll)
		{
			Random rnd = new Random();
			return coll.ElementAt(rnd.Next(coll.Count<T>()));
		}
	}
}
