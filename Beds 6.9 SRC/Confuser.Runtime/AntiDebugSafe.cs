using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Confuser.Runtime
{
	// Token: 0x02000033 RID: 51
	internal static class AntiDebugSafe
	{
		// Token: 0x060000B7 RID: 183 RVA: 0x00006A48 File Offset: 0x00004C48
		private static void Initialize()
		{
			string x = "COR";
			MethodInfo method = typeof(Environment).GetMethod("GetEnvironmentVariable", new Type[]
			{
				typeof(string)
			});
			if (method != null && (method.Invoke(null, new object[]
			{
				x + "_PROFILER"
			}) != null || method.Invoke(null, new object[]
			{
				x + "_ENABLE_PROFILING"
			}) != null))
			{
				Environment.FailFast(null);
			}
			new Thread(new ParameterizedThreadStart(AntiDebugSafe.Worker))
			{
				IsBackground = true
			}.Start(null);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00006AE4 File Offset: 0x00004CE4
		private static void Worker(object thread)
		{
			Thread th = thread as Thread;
			if (th == null)
			{
				th = new Thread(new ParameterizedThreadStart(AntiDebugSafe.Worker));
				th.IsBackground = true;
				th.Start(Thread.CurrentThread);
				Thread.Sleep(500);
			}
			for (;;)
			{
				if (Debugger.IsAttached || Debugger.IsLogging())
				{
					Environment.FailFast(null);
				}
				if (!th.IsAlive)
				{
					Environment.FailFast(null);
				}
				Thread.Sleep(1000);
			}
		}
	}
}
