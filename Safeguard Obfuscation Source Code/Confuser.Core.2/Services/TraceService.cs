using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x0200007F RID: 127
	internal class TraceService : ITraceService
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.Services.TraceService" /> class.
		/// </summary>
		/// <param name="context">The working context.</param>
		// Token: 0x060002E2 RID: 738 RVA: 0x000033FA File Offset: 0x000015FA
		public TraceService(ConfuserContext context)
		{
			this.context = context;
		}

		/// <inheritdoc />
		// Token: 0x060002E3 RID: 739 RVA: 0x00012C54 File Offset: 0x00010E54
		public MethodTrace Trace(MethodDef method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new ArgumentNullException("method");
			}
			return this.cache.GetValueOrDefaultLazy(method, (MethodDef m) => this.cache[m] = new MethodTrace(m)).Trace();
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00012C98 File Offset: 0x00010E98
		[CompilerGenerated]
		private MethodTrace <Trace>b__3_0(MethodDef m)
		{
			return this.cache[m] = new MethodTrace(m);
		}

		// Token: 0x0400022B RID: 555
		private readonly Dictionary<MethodDef, MethodTrace> cache = new Dictionary<MethodDef, MethodTrace>();

		// Token: 0x0400022C RID: 556
		private ConfuserContext context;
	}
}
