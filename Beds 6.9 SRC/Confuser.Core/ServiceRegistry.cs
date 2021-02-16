using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	// Token: 0x0200006F RID: 111
	public class ServiceRegistry : IServiceProvider
	{
		// Token: 0x0600029C RID: 668 RVA: 0x00011CF0 File Offset: 0x0000FEF0
		object IServiceProvider.GetService(Type serviceType)
		{
			return this.services.GetValueOrDefault(serviceType, null);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00011D10 File Offset: 0x0000FF10
		public T GetService<T>()
		{
			return (T)((object)this.services.GetValueOrDefault(typeof(T), null));
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00011D40 File Offset: 0x0000FF40
		public void RegisterService(string serviceId, Type serviceType, object service)
		{
			bool flag = !this.serviceIds.Add(serviceId);
			if (flag)
			{
				throw new ArgumentException("Service with ID '" + this.serviceIds + "' has already registered.", "serviceId");
			}
			bool flag2 = this.services.ContainsKey(serviceType);
			if (flag2)
			{
				throw new ArgumentException("Service with type '" + service.GetType().Name + "' has already registered.", "service");
			}
			this.services.Add(serviceType, service);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00011DC4 File Offset: 0x0000FFC4
		public bool Contains(string serviceId)
		{
			return this.serviceIds.Contains(serviceId);
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00003288 File Offset: 0x00001488
		public ServiceRegistry()
		{
		}

		// Token: 0x0400020D RID: 525
		private readonly HashSet<string> serviceIds = new HashSet<string>();

		// Token: 0x0400020E RID: 526
		private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
	}
}
