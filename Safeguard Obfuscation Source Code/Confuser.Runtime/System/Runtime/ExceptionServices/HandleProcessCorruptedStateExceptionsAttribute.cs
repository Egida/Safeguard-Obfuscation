using System;

namespace System.Runtime.ExceptionServices
{
	// Token: 0x02000003 RID: 3
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	internal class HandleProcessCorruptedStateExceptionsAttribute : Attribute
	{
		// Token: 0x06000007 RID: 7 RVA: 0x000020DD File Offset: 0x000002DD
		public HandleProcessCorruptedStateExceptionsAttribute()
		{
		}
	}
}
