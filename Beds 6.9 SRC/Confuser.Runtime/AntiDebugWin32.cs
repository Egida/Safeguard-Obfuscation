using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Confuser.Runtime
{
	// Token: 0x02000006 RID: 6
	internal static class AntiDebugWin32
	{
		// Token: 0x0600000B RID: 11
		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr hObject);

		// Token: 0x0600000C RID: 12 RVA: 0x00002DF4 File Offset: 0x00000FF4
		private static void Initialize()
		{
			string x = "COR";
			if (Environment.GetEnvironmentVariable(x + "_PROFILER") != null || Environment.GetEnvironmentVariable(x + "_ENABLE_PROFILING") != null)
			{
				Environment.FailFast(null);
			}
			new Thread(new ParameterizedThreadStart(AntiDebugWin32.Worker))
			{
				IsBackground = true
			}.Start(null);
		}

		// Token: 0x0600000D RID: 13
		[DllImport("kernel32.dll")]
		private static extern bool IsDebuggerPresent();

		// Token: 0x0600000E RID: 14
		[DllImport("kernel32.dll")]
		private static extern int OutputDebugString(string str);

		// Token: 0x0600000F RID: 15 RVA: 0x00002E50 File Offset: 0x00001050
		private static void Worker(object thread)
		{
			Thread th = thread as Thread;
			if (th == null)
			{
				th = new Thread(new ParameterizedThreadStart(AntiDebugWin32.Worker));
				th.IsBackground = true;
				th.Start(Thread.CurrentThread);
				Thread.Sleep(500);
			}
			for (;;)
			{
				if (Debugger.IsAttached || Debugger.IsLogging())
				{
					Environment.FailFast("");
				}
				if (AntiDebugWin32.IsDebuggerPresent())
				{
					Environment.FailFast("");
				}
				Process currentProcess = Process.GetCurrentProcess();
				if (currentProcess.Handle == IntPtr.Zero)
				{
					Environment.FailFast("");
				}
				currentProcess.Close();
				if (AntiDebugWin32.OutputDebugString("") > IntPtr.Size)
				{
					Environment.FailFast("");
				}
				try
				{
					AntiDebugWin32.CloseHandle(IntPtr.Zero);
				}
				catch
				{
					Environment.FailFast("");
				}
				if (!th.IsAlive)
				{
					Environment.FailFast("");
				}
				Thread.Sleep(1000);
			}
		}
	}
}
