using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Confuser.Runtime
{
	// Token: 0x02000007 RID: 7
	internal static class AntiDebugAntinet
	{
		// Token: 0x06000010 RID: 16 RVA: 0x000020E5 File Offset: 0x000002E5
		private static void Initialize()
		{
			if (!AntiDebugAntinet.InitializeAntiDebugger())
			{
				Environment.FailFast(null);
			}
			AntiDebugAntinet.InitializeAntiProfiler();
			if (AntiDebugAntinet.IsProfilerAttached)
			{
				Environment.FailFast(null);
				AntiDebugAntinet.PreventActiveProfilerFromReceivingProfilingMessages();
			}
		}

		// Token: 0x06000011 RID: 17
		[DllImport("kernel32", CharSet = CharSet.Auto)]
		private static extern uint GetCurrentProcessId();

		// Token: 0x06000012 RID: 18
		[DllImport("kernel32")]
		private static extern bool SetEvent(IntPtr hEvent);

		// Token: 0x06000013 RID: 19 RVA: 0x00002F4C File Offset: 0x0000114C
		private unsafe static bool InitializeAntiDebugger()
		{
			AntiDebugAntinet.Info info = AntiDebugAntinet.GetInfo();
			IntPtr pDebuggerRCThread = AntiDebugAntinet.FindDebuggerRCThreadAddress(info);
			if (pDebuggerRCThread == IntPtr.Zero)
			{
				return false;
			}
			byte* pDebuggerIPCControlBlock = (byte*)((void*)(*(IntPtr*)((byte*)((void*)pDebuggerRCThread) + info.DebuggerRCThread_pDebuggerIPCControlBlock)));
			if (Environment.Version.Major == 2)
			{
				pDebuggerIPCControlBlock = (byte*)((void*)(*(IntPtr*)pDebuggerIPCControlBlock));
			}
			*(int*)pDebuggerIPCControlBlock = 0;
			((byte*)((void*)pDebuggerRCThread))[info.DebuggerRCThread_shouldKeepLooping] = 0;
			AntiDebugAntinet.SetEvent(*(IntPtr*)((byte*)((void*)pDebuggerRCThread) + info.DebuggerRCThread_hEvent1));
			return true;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002FC8 File Offset: 0x000011C8
		private static AntiDebugAntinet.Info GetInfo()
		{
			int major = Environment.Version.Major;
			if (major != 2)
			{
				if (major != 4)
				{
				}
				if (Environment.Version.Revision <= 17020)
				{
					if (IntPtr.Size != 4)
					{
						return AntiDebugAntinet.Infos.info_CLR40_x64;
					}
					return AntiDebugAntinet.Infos.info_CLR40_x86_1;
				}
				else
				{
					if (IntPtr.Size != 4)
					{
						return AntiDebugAntinet.Infos.info_CLR40_x64;
					}
					return AntiDebugAntinet.Infos.info_CLR40_x86_2;
				}
			}
			else
			{
				if (IntPtr.Size != 4)
				{
					return AntiDebugAntinet.Infos.info_CLR20_x64;
				}
				return AntiDebugAntinet.Infos.info_CLR20_x86;
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00003038 File Offset: 0x00001238
		[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		private unsafe static IntPtr FindDebuggerRCThreadAddress(AntiDebugAntinet.Info info)
		{
			uint pid = AntiDebugAntinet.GetCurrentProcessId();
			try
			{
				AntiDebugAntinet.PEInfo peInfo = AntiDebugAntinet.PEInfo.GetCLR();
				if (peInfo == null)
				{
					return IntPtr.Zero;
				}
				IntPtr sectionAddr;
				uint sectionSize;
				if (!peInfo.FindSection(".data", out sectionAddr, out sectionSize))
				{
					return IntPtr.Zero;
				}
				byte* p = (byte*)((void*)sectionAddr);
				byte* end = (byte*)((void*)sectionAddr) + sectionSize;
				while (p + IntPtr.Size == end)
				{
					IntPtr pDebugger = *(IntPtr*)p;
					if (!(pDebugger == IntPtr.Zero))
					{
						try
						{
							if (AntiDebugAntinet.PEInfo.IsAlignedPointer(pDebugger))
							{
								uint pid2 = *(uint*)((byte*)((void*)pDebugger) + info.Debugger_pid);
								if (pid == pid2)
								{
									IntPtr pDebuggerRCThread = *(IntPtr*)((byte*)((void*)pDebugger) + info.Debugger_pDebuggerRCThread);
									if (AntiDebugAntinet.PEInfo.IsAlignedPointer(pDebuggerRCThread))
									{
										IntPtr pDebugger2 = *(IntPtr*)((byte*)((void*)pDebuggerRCThread) + info.DebuggerRCThread_pDebugger);
										if (!(pDebugger != pDebugger2))
										{
											return pDebuggerRCThread;
										}
									}
								}
							}
						}
						catch
						{
						}
					}
					p += IntPtr.Size;
				}
			}
			catch
			{
			}
			return IntPtr.Zero;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00003154 File Offset: 0x00001354
		private static bool IsProfilerAttached
		{
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			get
			{
				try
				{
					if (AntiDebugAntinet.profilerDetector == null)
					{
						return false;
					}
					return AntiDebugAntinet.profilerDetector.IsProfilerAttached();
				}
				catch
				{
				}
				return false;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00003190 File Offset: 0x00001390
		private static bool WasProfilerAttached
		{
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			get
			{
				try
				{
					if (AntiDebugAntinet.profilerDetector == null)
					{
						return false;
					}
					return AntiDebugAntinet.profilerDetector.WasProfilerAttached();
				}
				catch
				{
				}
				return false;
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000210C File Offset: 0x0000030C
		private static bool InitializeAntiProfiler()
		{
			AntiDebugAntinet.profilerDetector = AntiDebugAntinet.CreateProfilerDetector();
			return AntiDebugAntinet.profilerDetector.Init();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002122 File Offset: 0x00000322
		private static AntiDebugAntinet.ProfilerDetector CreateProfilerDetector()
		{
			if (Environment.Version.Major == 2)
			{
				return new AntiDebugAntinet.ProfilerDetectorCLR20();
			}
			return new AntiDebugAntinet.ProfilerDetectorCLR40();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000213C File Offset: 0x0000033C
		private static void PreventActiveProfilerFromReceivingProfilingMessages()
		{
			if (AntiDebugAntinet.profilerDetector == null)
			{
				return;
			}
			AntiDebugAntinet.profilerDetector.PreventActiveProfilerFromReceivingProfilingMessages();
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000031CC File Offset: 0x000013CC
		private static IntPtr GetMax(Dictionary<IntPtr, int> addresses, int minCount)
		{
			IntPtr foundAddr = IntPtr.Zero;
			int maxCount = 0;
			foreach (KeyValuePair<IntPtr, int> kv in addresses)
			{
				if (foundAddr == IntPtr.Zero || maxCount < kv.Value)
				{
					foundAddr = kv.Key;
					maxCount = kv.Value;
				}
			}
			if (maxCount < minCount)
			{
				return IntPtr.Zero;
			}
			return foundAddr;
		}

		// Token: 0x04000011 RID: 17
		private static AntiDebugAntinet.ProfilerDetector profilerDetector;

		// Token: 0x02000008 RID: 8
		private class Info
		{
			// Token: 0x0600001C RID: 28 RVA: 0x000020D5 File Offset: 0x000002D5
			public Info()
			{
			}

			// Token: 0x04000012 RID: 18
			public int DebuggerRCThread_hEvent1;

			// Token: 0x04000013 RID: 19
			public int DebuggerRCThread_pDebugger;

			// Token: 0x04000014 RID: 20
			public int DebuggerRCThread_pDebuggerIPCControlBlock;

			// Token: 0x04000015 RID: 21
			public int DebuggerRCThread_shouldKeepLooping;

			// Token: 0x04000016 RID: 22
			public int Debugger_pDebuggerRCThread;

			// Token: 0x04000017 RID: 23
			public int Debugger_pid;
		}

		// Token: 0x02000009 RID: 9
		private static class Infos
		{
			// Token: 0x0600001D RID: 29 RVA: 0x00003250 File Offset: 0x00001450
			// Note: this type is marked as 'beforefieldinit'.
			static Infos()
			{
			}

			// Token: 0x04000018 RID: 24
			public static readonly AntiDebugAntinet.Info info_CLR20_x86 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 4,
				Debugger_pid = 8,
				DebuggerRCThread_pDebugger = 48,
				DebuggerRCThread_pDebuggerIPCControlBlock = 52,
				DebuggerRCThread_shouldKeepLooping = 60,
				DebuggerRCThread_hEvent1 = 64
			};

			// Token: 0x04000019 RID: 25
			public static readonly AntiDebugAntinet.Info info_CLR20_x64 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 8,
				Debugger_pid = 16,
				DebuggerRCThread_pDebugger = 88,
				DebuggerRCThread_pDebuggerIPCControlBlock = 96,
				DebuggerRCThread_shouldKeepLooping = 112,
				DebuggerRCThread_hEvent1 = 120
			};

			// Token: 0x0400001A RID: 26
			public static readonly AntiDebugAntinet.Info info_CLR40_x86_1 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 8,
				Debugger_pid = 12,
				DebuggerRCThread_pDebugger = 52,
				DebuggerRCThread_pDebuggerIPCControlBlock = 56,
				DebuggerRCThread_shouldKeepLooping = 64,
				DebuggerRCThread_hEvent1 = 68
			};

			// Token: 0x0400001B RID: 27
			public static readonly AntiDebugAntinet.Info info_CLR40_x86_2 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 8,
				Debugger_pid = 12,
				DebuggerRCThread_pDebugger = 48,
				DebuggerRCThread_pDebuggerIPCControlBlock = 52,
				DebuggerRCThread_shouldKeepLooping = 60,
				DebuggerRCThread_hEvent1 = 64
			};

			// Token: 0x0400001C RID: 28
			public static readonly AntiDebugAntinet.Info info_CLR40_x64 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 16,
				Debugger_pid = 24,
				DebuggerRCThread_pDebugger = 88,
				DebuggerRCThread_pDebuggerIPCControlBlock = 96,
				DebuggerRCThread_shouldKeepLooping = 112,
				DebuggerRCThread_hEvent1 = 120
			};
		}

		// Token: 0x0200000A RID: 10
		private abstract class ProfilerDetector
		{
			// Token: 0x0600001E RID: 30
			public abstract bool IsProfilerAttached();

			// Token: 0x0600001F RID: 31
			public abstract bool WasProfilerAttached();

			// Token: 0x06000020 RID: 32
			public abstract bool Init();

			// Token: 0x06000021 RID: 33
			public abstract void PreventActiveProfilerFromReceivingProfilingMessages();

			// Token: 0x06000022 RID: 34 RVA: 0x000020D5 File Offset: 0x000002D5
			protected ProfilerDetector()
			{
			}
		}

		// Token: 0x0200000B RID: 11
		private class ProfilerDetectorCLR20 : AntiDebugAntinet.ProfilerDetector
		{
			// Token: 0x06000023 RID: 35 RVA: 0x00002150 File Offset: 0x00000350
			public unsafe override bool IsProfilerAttached()
			{
				return !(this.profilerStatusFlag == IntPtr.Zero) && (*(uint*)((void*)this.profilerStatusFlag) & 6U) > 0U;
			}

			// Token: 0x06000024 RID: 36 RVA: 0x00002177 File Offset: 0x00000377
			public override bool WasProfilerAttached()
			{
				return this.wasAttached;
			}

			// Token: 0x06000025 RID: 37 RVA: 0x0000217F File Offset: 0x0000037F
			public override bool Init()
			{
				bool result = this.FindProfilerStatus();
				this.wasAttached = this.IsProfilerAttached();
				return result;
			}

			// Token: 0x06000026 RID: 38 RVA: 0x0000337C File Offset: 0x0000157C
			private unsafe bool FindProfilerStatus()
			{
				Dictionary<IntPtr, int> addrCounts = new Dictionary<IntPtr, int>();
				try
				{
					AntiDebugAntinet.PEInfo peInfo = AntiDebugAntinet.PEInfo.GetCLR();
					if (peInfo == null)
					{
						return false;
					}
					IntPtr sectionAddr;
					uint sectionSize;
					if (!peInfo.FindSection(".text", out sectionAddr, out sectionSize))
					{
						return false;
					}
					byte* p = (byte*)((void*)sectionAddr);
					byte* end = (byte*)((void*)sectionAddr) + sectionSize;
					while (p < end)
					{
						if (*p == 246 && p[1] == 5 && p[6] == 6)
						{
							IntPtr addr;
							if (IntPtr.Size == 4)
							{
								addr = new IntPtr(*(uint*)(p + 2));
							}
							else
							{
								addr = new IntPtr((void*)(p + 7 + *(int*)(p + 2)));
							}
							if (AntiDebugAntinet.PEInfo.IsAligned(addr, 4U) && peInfo.IsValidImageAddress(addr, 4U))
							{
								try
								{
									*(int*)((void*)addr) = (int)(*(uint*)((void*)addr));
								}
								catch
								{
									goto IL_DD;
								}
								int count = 0;
								addrCounts.TryGetValue(addr, out count);
								count++;
								addrCounts[addr] = count;
								if (count >= 50)
								{
									break;
								}
							}
						}
						IL_DD:
						p++;
					}
				}
				catch
				{
				}
				IntPtr foundAddr = AntiDebugAntinet.GetMax(addrCounts, 5);
				if (foundAddr == IntPtr.Zero)
				{
					return false;
				}
				this.profilerStatusFlag = foundAddr;
				return true;
			}

			// Token: 0x06000027 RID: 39 RVA: 0x00002193 File Offset: 0x00000393
			public unsafe override void PreventActiveProfilerFromReceivingProfilingMessages()
			{
				if (this.profilerStatusFlag == IntPtr.Zero)
				{
					return;
				}
				*(uint*)((void*)this.profilerStatusFlag) &= 4294967289U;
			}

			// Token: 0x06000028 RID: 40 RVA: 0x000021B9 File Offset: 0x000003B9
			public ProfilerDetectorCLR20()
			{
			}

			// Token: 0x0400001D RID: 29
			private IntPtr profilerStatusFlag;

			// Token: 0x0400001E RID: 30
			private bool wasAttached;
		}

		// Token: 0x0200000C RID: 12
		private class ProfilerDetectorCLR40 : AntiDebugAntinet.ProfilerDetector
		{
			// Token: 0x06000029 RID: 41
			[DllImport("kernel32", CharSet = CharSet.Auto)]
			private static extern uint GetCurrentProcessId();

			// Token: 0x0600002A RID: 42
			[DllImport("kernel32", CharSet = CharSet.Auto)]
			private static extern void Sleep(uint dwMilliseconds);

			// Token: 0x0600002B RID: 43
			[DllImport("kernel32", SetLastError = true)]
			private static extern SafeFileHandle CreateNamedPipe(string lpName, uint dwOpenMode, uint dwPipeMode, uint nMaxInstances, uint nOutBufferSize, uint nInBufferSize, uint nDefaultTimeOut, IntPtr lpSecurityAttributes);

			// Token: 0x0600002C RID: 44
			[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
			private static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

			// Token: 0x0600002D RID: 45
			[DllImport("kernel32")]
			private static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

			// Token: 0x0600002E RID: 46 RVA: 0x000021C1 File Offset: 0x000003C1
			public unsafe override bool IsProfilerAttached()
			{
				return !(this.profilerControlBlock == IntPtr.Zero) && *(uint*)((byte*)((byte*)((void*)this.profilerControlBlock) + IntPtr.Size) + 4) > 0U;
			}

			// Token: 0x0600002F RID: 47 RVA: 0x000021EE File Offset: 0x000003EE
			public override bool WasProfilerAttached()
			{
				return this.wasAttached;
			}

			// Token: 0x06000030 RID: 48 RVA: 0x000021F6 File Offset: 0x000003F6
			public override bool Init()
			{
				bool result = this.FindProfilerControlBlock() & (this.TakeOwnershipOfNamedPipe() || this.CreateNamedPipe()) & this.PatchAttacherThreadProc();
				this.wasAttached = this.IsProfilerAttached();
				return result;
			}

			// Token: 0x06000031 RID: 49 RVA: 0x000034B8 File Offset: 0x000016B8
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe bool TakeOwnershipOfNamedPipe()
			{
				try
				{
					if (this.CreateNamedPipe())
					{
						return true;
					}
					IntPtr threadingModeAddr = AntiDebugAntinet.ProfilerDetectorCLR40.FindThreadingModeAddress();
					IntPtr timeOutOptionAddr = AntiDebugAntinet.ProfilerDetectorCLR40.FindTimeOutOptionAddress();
					if (timeOutOptionAddr == IntPtr.Zero)
					{
						return false;
					}
					if (threadingModeAddr != IntPtr.Zero && *(uint*)((void*)threadingModeAddr) == 2U)
					{
						*(int*)((void*)threadingModeAddr) = 1;
					}
					AntiDebugAntinet.ProfilerDetectorCLR40.FixTimeOutOption(timeOutOptionAddr);
					using (SafeFileHandle hPipe = this.CreatePipeFileHandleWait())
					{
						if (hPipe == null)
						{
							return false;
						}
						if (hPipe.IsInvalid)
						{
							return false;
						}
					}
					return this.CreateNamedPipeWait();
				}
				catch
				{
				}
				return false;
			}

			// Token: 0x06000032 RID: 50 RVA: 0x00003564 File Offset: 0x00001764
			private bool CreateNamedPipeWait()
			{
				for (int timeLeft = 100; timeLeft > 0; timeLeft -= 5)
				{
					if (this.CreateNamedPipe())
					{
						return true;
					}
					AntiDebugAntinet.ProfilerDetectorCLR40.Sleep(5U);
				}
				return this.CreateNamedPipe();
			}

			// Token: 0x06000033 RID: 51 RVA: 0x00003594 File Offset: 0x00001794
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe static void FixTimeOutOption(IntPtr timeOutOptionAddr)
			{
				if (timeOutOptionAddr == IntPtr.Zero)
				{
					return;
				}
				uint oldProtect;
				AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(timeOutOptionAddr, (int)(AntiDebugAntinet.ProfilerDetectorCLR40.ConfigDWORDInfo_defValue + 4U), 64U, out oldProtect);
				try
				{
					*(int*)((byte*)((void*)timeOutOptionAddr) + AntiDebugAntinet.ProfilerDetectorCLR40.ConfigDWORDInfo_defValue) = 0;
				}
				finally
				{
					AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(timeOutOptionAddr, (int)(AntiDebugAntinet.ProfilerDetectorCLR40.ConfigDWORDInfo_defValue + 4U), oldProtect, out oldProtect);
				}
				char* name = *(IntPtr*)((void*)timeOutOptionAddr);
				IntPtr nameAddr = new IntPtr((void*)name);
				AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(nameAddr, "ProfAPIMaxWaitForTriggerMs".Length * 2, 64U, out oldProtect);
				try
				{
					Random rand = new Random();
					for (int i = 0; i < "ProfAPIMaxWaitForTriggerMs".Length; i++)
					{
						name[i] = (char)rand.Next(1, 65535);
					}
				}
				finally
				{
					AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(nameAddr, IntPtr.Size, oldProtect, out oldProtect);
				}
			}

			// Token: 0x06000034 RID: 52 RVA: 0x00003670 File Offset: 0x00001870
			private SafeFileHandle CreatePipeFileHandleWait()
			{
				for (int timeLeft = 100; timeLeft > 0; timeLeft -= 5)
				{
					if (this.CreateNamedPipe())
					{
						return null;
					}
					SafeFileHandle hFile = AntiDebugAntinet.ProfilerDetectorCLR40.CreatePipeFileHandle();
					if (!hFile.IsInvalid)
					{
						return hFile;
					}
					AntiDebugAntinet.ProfilerDetectorCLR40.Sleep(5U);
				}
				return AntiDebugAntinet.ProfilerDetectorCLR40.CreatePipeFileHandle();
			}

			// Token: 0x06000035 RID: 53 RVA: 0x00002223 File Offset: 0x00000423
			private static SafeFileHandle CreatePipeFileHandle()
			{
				return AntiDebugAntinet.ProfilerDetectorCLR40.CreateFile(AntiDebugAntinet.ProfilerDetectorCLR40.GetPipeName(), 3221225472U, 0U, IntPtr.Zero, 3U, 1073741824U, IntPtr.Zero);
			}

			// Token: 0x06000036 RID: 54 RVA: 0x000036B0 File Offset: 0x000018B0
			private static string GetPipeName()
			{
				return string.Format("\\\\.\\pipe\\CPFATP_{0}_v{1}.{2}.{3}", new object[]
				{
					AntiDebugAntinet.ProfilerDetectorCLR40.GetCurrentProcessId(),
					Environment.Version.Major,
					Environment.Version.Minor,
					Environment.Version.Build
				});
			}

			// Token: 0x06000037 RID: 55 RVA: 0x00003710 File Offset: 0x00001910
			private bool CreateNamedPipe()
			{
				if (this.profilerPipe != null && !this.profilerPipe.IsInvalid)
				{
					return true;
				}
				this.profilerPipe = AntiDebugAntinet.ProfilerDetectorCLR40.CreateNamedPipe(AntiDebugAntinet.ProfilerDetectorCLR40.GetPipeName(), 1073741827U, 6U, 1U, 36U, 824U, 1000U, IntPtr.Zero);
				return !this.profilerPipe.IsInvalid;
			}

			// Token: 0x06000038 RID: 56 RVA: 0x0000376C File Offset: 0x0000196C
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe static IntPtr FindThreadingModeAddress()
			{
				try
				{
					AntiDebugAntinet.PEInfo peInfo = AntiDebugAntinet.PEInfo.GetCLR();
					if (peInfo == null)
					{
						return IntPtr.Zero;
					}
					IntPtr sectionAddr;
					uint sectionSize;
					if (!peInfo.FindSection(".text", out sectionAddr, out sectionSize))
					{
						return IntPtr.Zero;
					}
					byte* ptr = (byte*)((void*)sectionAddr);
					byte* end = (byte*)((void*)sectionAddr) + sectionSize;
					while (ptr < end)
					{
						try
						{
							byte* p = ptr;
							if (*p == 131 && p[1] == 61 && p[6] == 2)
							{
								IntPtr addr;
								if (IntPtr.Size == 4)
								{
									addr = new IntPtr(*(uint*)(p + 2));
								}
								else
								{
									addr = new IntPtr((void*)(p + 7 + *(int*)(p + 2)));
								}
								if (AntiDebugAntinet.PEInfo.IsAligned(addr, 4U))
								{
									if (peInfo.IsValidImageAddress(addr))
									{
										p += 7;
										if (*(uint*)((void*)addr) >= 1U && *(uint*)((void*)addr) <= 2U)
										{
											*(int*)((void*)addr) = (int)(*(uint*)((void*)addr));
											if (AntiDebugAntinet.ProfilerDetectorCLR40.NextJz(ref p))
											{
												AntiDebugAntinet.ProfilerDetectorCLR40.SkipRex(ref p);
												if (*p == 131 && p[2] == 0)
												{
													if (p[1] - 232 > 7)
													{
														goto IL_183;
													}
													p += 3;
												}
												else
												{
													if (*p != 133)
													{
														goto IL_183;
													}
													int num = p[1] >> 3 & 7;
													int rm = (int)(p[1] & 7);
													if (num != rm)
													{
														goto IL_183;
													}
													p += 2;
												}
												if (AntiDebugAntinet.ProfilerDetectorCLR40.NextJz(ref p))
												{
													if (AntiDebugAntinet.ProfilerDetectorCLR40.SkipDecReg(ref p))
													{
														if (AntiDebugAntinet.ProfilerDetectorCLR40.NextJz(ref p))
														{
															if (AntiDebugAntinet.ProfilerDetectorCLR40.SkipDecReg(ref p))
															{
																return addr;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
						catch
						{
						}
						IL_183:
						ptr++;
					}
				}
				catch
				{
				}
				return IntPtr.Zero;
			}

			// Token: 0x06000039 RID: 57 RVA: 0x0000394C File Offset: 0x00001B4C
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe static IntPtr FindTimeOutOptionAddress()
			{
				try
				{
					AntiDebugAntinet.PEInfo peInfo = AntiDebugAntinet.PEInfo.GetCLR();
					if (peInfo == null)
					{
						return IntPtr.Zero;
					}
					IntPtr sectionAddr;
					uint sectionSize;
					if (!peInfo.FindSection(".rdata", out sectionAddr, out sectionSize) && !peInfo.FindSection(".text", out sectionAddr, out sectionSize))
					{
						return IntPtr.Zero;
					}
					byte* p = (byte*)((void*)sectionAddr);
					byte* end = (byte*)((void*)sectionAddr) + sectionSize;
					while (p < end)
					{
						try
						{
							char* name = *(IntPtr*)p;
							if (AntiDebugAntinet.PEInfo.IsAligned(new IntPtr((void*)name), 2U))
							{
								if (peInfo.IsValidImageAddress((void*)name))
								{
									if (AntiDebugAntinet.ProfilerDetectorCLR40.Equals(name, "ProfAPIMaxWaitForTriggerMs"))
									{
										return new IntPtr((void*)p);
									}
								}
							}
						}
						catch
						{
						}
						p++;
					}
				}
				catch
				{
				}
				return IntPtr.Zero;
			}

			// Token: 0x0600003A RID: 58 RVA: 0x00003A20 File Offset: 0x00001C20
			private unsafe static bool Equals(char* s1, string s2)
			{
				for (int i = 0; i < s2.Length; i++)
				{
					if (char.ToUpperInvariant(s1[i]) != char.ToUpperInvariant(s2[i]))
					{
						return false;
					}
				}
				return s1[s2.Length] == '\0';
			}

			// Token: 0x0600003B RID: 59 RVA: 0x00002245 File Offset: 0x00000445
			private unsafe static void SkipRex(ref byte* p)
			{
				if (IntPtr.Size != 8)
				{
					return;
				}
				if (*p >= 72 && *p <= 79)
				{
					p++;
				}
			}

			// Token: 0x0600003C RID: 60 RVA: 0x00003A6C File Offset: 0x00001C6C
			private unsafe static bool SkipDecReg(ref byte* p)
			{
				AntiDebugAntinet.ProfilerDetectorCLR40.SkipRex(ref p);
				if (IntPtr.Size == 4 && *p >= 72 && *p <= 79)
				{
					p++;
				}
				else
				{
					if (*p != 255 || *(p + 1) < 200 || *(p + 1) > 207)
					{
						return false;
					}
					p += 2;
				}
				return true;
			}

			// Token: 0x0600003D RID: 61 RVA: 0x00002264 File Offset: 0x00000464
			private unsafe static bool NextJz(ref byte* p)
			{
				if (*p == 116)
				{
					p += 2;
					return true;
				}
				if (*p == 15 && *(p + 1) == 132)
				{
					p += 6;
					return true;
				}
				return false;
			}

			// Token: 0x0600003E RID: 62 RVA: 0x00003ACC File Offset: 0x00001CCC
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe bool PatchAttacherThreadProc()
			{
				IntPtr threadProc = this.FindAttacherThreadProc();
				if (threadProc == IntPtr.Zero)
				{
					return false;
				}
				byte* p = (byte*)((void*)threadProc);
				uint oldProtect;
				AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(new IntPtr((void*)p), 5, 64U, out oldProtect);
				try
				{
					if (IntPtr.Size == 4)
					{
						*p = 51;
						p[1] = 192;
						p[2] = 194;
						p[3] = 4;
						p[4] = 0;
					}
					else
					{
						*p = 51;
						p[1] = 192;
						p[2] = 195;
					}
				}
				finally
				{
					AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(new IntPtr((void*)p), 5, oldProtect, out oldProtect);
				}
				return true;
			}

			// Token: 0x0600003F RID: 63 RVA: 0x00003B6C File Offset: 0x00001D6C
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe IntPtr FindAttacherThreadProc()
			{
				try
				{
					AntiDebugAntinet.PEInfo peInfo = AntiDebugAntinet.PEInfo.GetCLR();
					if (peInfo == null)
					{
						return IntPtr.Zero;
					}
					IntPtr sectionAddr;
					uint sectionSize;
					if (!peInfo.FindSection(".text", out sectionAddr, out sectionSize))
					{
						return IntPtr.Zero;
					}
					byte* p = (byte*)((void*)sectionAddr);
					byte* start = p;
					byte* end = (byte*)((void*)sectionAddr) + sectionSize;
					if (IntPtr.Size == 4)
					{
						while (p < end)
						{
							byte push = *p;
							if (push >= 80 && push <= 87 && p[1] == push && p[2] == push && p[8] == push && p[9] == push && p[3] == 104 && p[10] == 255 && p[11] == 21)
							{
								IntPtr threadProc = new IntPtr(*(uint*)(p + 4));
								if (AntiDebugAntinet.ProfilerDetectorCLR40.CheckThreadProc(start, end, threadProc))
								{
									return threadProc;
								}
							}
							p++;
						}
					}
					else
					{
						while (p < end)
						{
							if ((*p == 69 || p[1] == 51 || p[2] == 201) && (p[3] == 76 || p[4] == 141 || p[5] == 5) && (p[10] == 51 || p[11] == 210) && (p[12] == 51 || p[13] == 201) && (p[14] == 255 || p[15] == 21))
							{
								IntPtr threadProc2 = new IntPtr((void*)(p + 10 + *(int*)(p + 6)));
								if (AntiDebugAntinet.ProfilerDetectorCLR40.CheckThreadProc(start, end, threadProc2))
								{
									return threadProc2;
								}
							}
							p++;
						}
					}
				}
				catch
				{
				}
				return IntPtr.Zero;
			}

			// Token: 0x06000040 RID: 64 RVA: 0x00003D18 File Offset: 0x00001F18
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe static bool CheckThreadProc(byte* codeStart, byte* codeEnd, IntPtr threadProc)
			{
				try
				{
					byte* p = (byte*)((void*)threadProc);
					if (p < codeStart || p >= codeEnd)
					{
						return false;
					}
					for (int i = 0; i < 32; i++)
					{
						if (*(uint*)(p + i) == 16384U)
						{
							return true;
						}
					}
				}
				catch
				{
				}
				return false;
			}

			// Token: 0x06000041 RID: 65 RVA: 0x00003D6C File Offset: 0x00001F6C
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe bool FindProfilerControlBlock()
			{
				Dictionary<IntPtr, int> addrCounts = new Dictionary<IntPtr, int>();
				try
				{
					AntiDebugAntinet.PEInfo peInfo = AntiDebugAntinet.PEInfo.GetCLR();
					if (peInfo == null)
					{
						return false;
					}
					IntPtr sectionAddr;
					uint sectionSize;
					if (!peInfo.FindSection(".text", out sectionAddr, out sectionSize))
					{
						return false;
					}
					byte* p = (byte*)((void*)sectionAddr);
					byte* end = (byte*)((void*)sectionAddr) + sectionSize;
					while (p < end)
					{
						IntPtr addr;
						if (*p == 161 && p[5] == 131 && p[6] == 248 && p[7] == 4)
						{
							if (IntPtr.Size == 4)
							{
								addr = new IntPtr(*(uint*)(p + 1));
								goto IL_14B;
							}
							addr = new IntPtr((void*)(p + 5 + *(int*)(p + 1)));
							goto IL_14B;
						}
						else if (*p == 139 && p[1] == 5 && p[6] == 131 && p[7] == 248 && p[8] == 4)
						{
							if (IntPtr.Size == 4)
							{
								addr = new IntPtr(*(uint*)(p + 2));
								goto IL_14B;
							}
							addr = new IntPtr((void*)(p + 6 + *(int*)(p + 2)));
							goto IL_14B;
						}
						else if (*p == 131 && p[1] == 61 && p[6] == 4)
						{
							if (IntPtr.Size == 4)
							{
								addr = new IntPtr(*(uint*)(p + 2));
								goto IL_14B;
							}
							addr = new IntPtr((void*)(p + 7 + *(int*)(p + 2)));
							goto IL_14B;
						}
						IL_1A7:
						p++;
						continue;
						IL_14B:
						if (!AntiDebugAntinet.PEInfo.IsAligned(addr, 4U) || !peInfo.IsValidImageAddress(addr, 4U))
						{
							goto IL_1A7;
						}
						try
						{
							if (*(uint*)((void*)addr) > 4U)
							{
								goto IL_1A7;
							}
							*(int*)((void*)addr) = (int)(*(uint*)((void*)addr));
						}
						catch
						{
							goto IL_1A7;
						}
						int count = 0;
						addrCounts.TryGetValue(addr, out count);
						count++;
						addrCounts[addr] = count;
						if (count < 50)
						{
							goto IL_1A7;
						}
						break;
					}
				}
				catch
				{
				}
				IntPtr foundAddr = AntiDebugAntinet.GetMax(addrCounts, 5);
				if (foundAddr == IntPtr.Zero)
				{
					return false;
				}
				this.profilerControlBlock = new IntPtr((void*)((byte*)((void*)foundAddr) - (IntPtr.Size + 4)));
				return true;
			}

			// Token: 0x06000042 RID: 66 RVA: 0x00002291 File Offset: 0x00000491
			public unsafe override void PreventActiveProfilerFromReceivingProfilingMessages()
			{
				if (this.profilerControlBlock == IntPtr.Zero)
				{
					return;
				}
				*(int*)((byte*)((byte*)((void*)this.profilerControlBlock) + IntPtr.Size) + 4) = 0;
			}

			// Token: 0x06000043 RID: 67 RVA: 0x000021B9 File Offset: 0x000003B9
			public ProfilerDetectorCLR40()
			{
			}

			// Token: 0x06000044 RID: 68 RVA: 0x000022BB File Offset: 0x000004BB
			// Note: this type is marked as 'beforefieldinit'.
			static ProfilerDetectorCLR40()
			{
			}

			// Token: 0x0400001F RID: 31
			private const uint PIPE_ACCESS_DUPLEX = 3U;

			// Token: 0x04000020 RID: 32
			private const uint PIPE_TYPE_MESSAGE = 4U;

			// Token: 0x04000021 RID: 33
			private const uint PIPE_READMODE_MESSAGE = 2U;

			// Token: 0x04000022 RID: 34
			private const uint FILE_FLAG_OVERLAPPED = 1073741824U;

			// Token: 0x04000023 RID: 35
			private const uint GENERIC_READ = 2147483648U;

			// Token: 0x04000024 RID: 36
			private const uint GENERIC_WRITE = 1073741824U;

			// Token: 0x04000025 RID: 37
			private const uint OPEN_EXISTING = 3U;

			// Token: 0x04000026 RID: 38
			private const uint PAGE_EXECUTE_READWRITE = 64U;

			// Token: 0x04000027 RID: 39
			private const uint ConfigDWORDInfo_name = 0U;

			// Token: 0x04000028 RID: 40
			private const string ProfAPIMaxWaitForTriggerMs_name = "ProfAPIMaxWaitForTriggerMs";

			// Token: 0x04000029 RID: 41
			private static readonly uint ConfigDWORDInfo_defValue = (uint)IntPtr.Size;

			// Token: 0x0400002A RID: 42
			private IntPtr profilerControlBlock;

			// Token: 0x0400002B RID: 43
			private SafeFileHandle profilerPipe;

			// Token: 0x0400002C RID: 44
			private bool wasAttached;
		}

		// Token: 0x0200000D RID: 13
		private class PEInfo
		{
			// Token: 0x06000045 RID: 69 RVA: 0x000022C7 File Offset: 0x000004C7
			public PEInfo(IntPtr addr)
			{
				this.imageBase = addr;
				this.Init();
			}

			// Token: 0x06000046 RID: 70
			[DllImport("kernel32", CharSet = CharSet.Auto)]
			private static extern IntPtr GetModuleHandle(string name);

			// Token: 0x06000047 RID: 71 RVA: 0x00003F9C File Offset: 0x0000219C
			public static AntiDebugAntinet.PEInfo GetCLR()
			{
				IntPtr clrAddr = AntiDebugAntinet.PEInfo.GetCLRAddress();
				if (clrAddr == IntPtr.Zero)
				{
					return null;
				}
				return new AntiDebugAntinet.PEInfo(clrAddr);
			}

			// Token: 0x06000048 RID: 72 RVA: 0x000022DC File Offset: 0x000004DC
			private static IntPtr GetCLRAddress()
			{
				if (Environment.Version.Major == 2)
				{
					return AntiDebugAntinet.PEInfo.GetModuleHandle("mscorwks");
				}
				return AntiDebugAntinet.PEInfo.GetModuleHandle("clr");
			}

			// Token: 0x06000049 RID: 73 RVA: 0x00003FC4 File Offset: 0x000021C4
			private unsafe void Init()
			{
				byte* p = (byte*)((void*)this.imageBase);
				p += *(uint*)(p + 60);
				p += 6;
				this.numSects = (int)(*(ushort*)p);
				p += 18;
				bool is32 = *(ushort*)p == 267;
				uint sizeOfImage = *(uint*)(p + 56);
				this.imageEnd = new IntPtr((void*)((byte*)((void*)this.imageBase) + sizeOfImage));
				p += (is32 ? 96 : 112);
				p += 128;
				this.sectionsAddr = new IntPtr((void*)p);
			}

			// Token: 0x0600004A RID: 74 RVA: 0x00002300 File Offset: 0x00000500
			public unsafe bool IsValidImageAddress(IntPtr addr)
			{
				return this.IsValidImageAddress((void*)addr, 0U);
			}

			// Token: 0x0600004B RID: 75 RVA: 0x0000230F File Offset: 0x0000050F
			public unsafe bool IsValidImageAddress(IntPtr addr, uint size)
			{
				return this.IsValidImageAddress((void*)addr, size);
			}

			// Token: 0x0600004C RID: 76 RVA: 0x0000231E File Offset: 0x0000051E
			public unsafe bool IsValidImageAddress(void* addr)
			{
				return this.IsValidImageAddress(addr, 0U);
			}

			// Token: 0x0600004D RID: 77 RVA: 0x00004040 File Offset: 0x00002240
			public unsafe bool IsValidImageAddress(void* addr, uint size)
			{
				if (addr < (void*)this.imageBase)
				{
					return false;
				}
				if (addr >= (void*)this.imageEnd)
				{
					return false;
				}
				if (size != 0U)
				{
					if ((byte*)addr + size < (byte*)addr)
					{
						return false;
					}
					if ((byte*)addr + size != (byte*)((void*)this.imageEnd))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x0600004E RID: 78 RVA: 0x00004090 File Offset: 0x00002290
			public unsafe bool FindSection(string name, out IntPtr sectionStart, out uint sectionSize)
			{
				byte[] nameBytes = Encoding.UTF8.GetBytes(name + "\0\0\0\0\0\0\0\0");
				for (int i = 0; i < this.numSects; i++)
				{
					byte* p = (byte*)((void*)this.sectionsAddr) + i * 40;
					if (AntiDebugAntinet.PEInfo.CompareSectionName(p, nameBytes))
					{
						sectionStart = new IntPtr((void*)((byte*)((void*)this.imageBase) + *(uint*)(p + 12)));
						sectionSize = Math.Max(*(uint*)(p + 8), *(uint*)(p + 16));
						return true;
					}
				}
				sectionStart = IntPtr.Zero;
				sectionSize = 0U;
				return false;
			}

			// Token: 0x0600004F RID: 79 RVA: 0x00004114 File Offset: 0x00002314
			private unsafe static bool CompareSectionName(byte* sectionName, byte[] nameBytes)
			{
				for (int i = 0; i < 8; i++)
				{
					if (*sectionName != nameBytes[i])
					{
						return false;
					}
					sectionName++;
				}
				return true;
			}

			// Token: 0x06000050 RID: 80 RVA: 0x00002328 File Offset: 0x00000528
			public static bool IsAlignedPointer(IntPtr addr)
			{
				return ((int)addr.ToInt64() & IntPtr.Size - 1) == 0;
			}

			// Token: 0x06000051 RID: 81 RVA: 0x0000233D File Offset: 0x0000053D
			public static bool IsAligned(IntPtr addr, uint alignment)
			{
				return ((uint)addr.ToInt64() & alignment - 1U) == 0U;
			}

			// Token: 0x0400002D RID: 45
			private readonly IntPtr imageBase;

			// Token: 0x0400002E RID: 46
			private IntPtr imageEnd;

			// Token: 0x0400002F RID: 47
			private int numSects;

			// Token: 0x04000030 RID: 48
			private IntPtr sectionsAddr;
		}
	}
}
