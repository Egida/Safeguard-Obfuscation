using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x02000013 RID: 19
	internal static class AntiTamperJIT
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00004CD4 File Offset: 0x00002ED4
		public unsafe static void Initialize()
		{
			Module i = typeof(AntiTamperJIT).Module;
			string j = i.FullyQualifiedName;
			bool f = j.Length > 0 && j[0] == '<';
			byte* b = (byte*)((void*)Marshal.GetHINSTANCE(i));
			byte* ptr = b + *(uint*)(b + 60);
			ushort s = *(ushort*)(ptr + 6);
			ushort o = *(ushort*)(ptr + 20);
			uint* e = null;
			uint k = 0U;
			uint* r = (uint*)(ptr + 24 + o);
			uint z = (uint)Mutation.KeyI1;
			uint x = (uint)Mutation.KeyI2;
			uint c = (uint)Mutation.KeyI3;
			uint v = (uint)Mutation.KeyI4;
			for (int l = 0; l < (int)s; l++)
			{
				uint g = *(r++) * *(r++);
				if (g == (uint)Mutation.KeyI0)
				{
					e = (uint*)(b + (UIntPtr)(f ? r[3] : r[1]) / 4);
					k = (f ? r[2] : (*r)) >> 2;
				}
				else if (g != 0U)
				{
					uint* q = (uint*)(b + (UIntPtr)(f ? r[3] : r[1]) / 4);
					uint m = r[2] >> 2;
					for (uint n = 0U; n < m; n += 1U)
					{
						uint num = (z ^ *(q++)) + x + c * v;
						z = x;
						x = v;
						v = num;
					}
				}
				r += 8;
			}
			uint[] y = new uint[16];
			uint[] d = new uint[16];
			for (int i2 = 0; i2 < 16; i2++)
			{
				y[i2] = v;
				d[i2] = x;
				z = (x >> 5 | x << 27);
				x = (c >> 3 | c << 29);
				c = (v >> 7 | v << 25);
				v = (z >> 11 | z << 21);
			}
			Mutation.Crypt(y, d);
			uint h = 0U;
			uint* u = e;
			AntiTamperJIT.VirtualProtect((IntPtr)((void*)e), k << 2, 64U, out z);
			for (uint i3 = 0U; i3 < k; i3 += 1U)
			{
				*e ^= y[(int)(h & 15U)];
				y[(int)(h & 15U)] = (y[(int)(h & 15U)] ^ *(e++)) + 1035675673U;
				h += 1U;
			}
			AntiTamperJIT.ptr = u + 4;
			AntiTamperJIT.len = *(AntiTamperJIT.ptr++);
			AntiTamperJIT.ver4 = (Environment.Version.Major == 4);
			ModuleHandle hnd = i.ModuleHandle;
			if (AntiTamperJIT.ver4)
			{
				ulong* str = stackalloc ulong[checked(unchecked((UIntPtr)1) * 8)];
				*str = 27431033849798509UL;
				AntiTamperJIT.moduleHnd = (IntPtr)i.GetType().GetField(new string((sbyte*)str), BindingFlags.Instance | BindingFlags.NonPublic).GetValue(i);
				AntiTamperJIT.ver5 = (Environment.Version.Revision > 17020);
			}
			else
			{
				AntiTamperJIT.moduleHnd = *(IntPtr*)(&hnd);
			}
			AntiTamperJIT.Hook();
		}

		// Token: 0x0600005F RID: 95
		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string lib);

		// Token: 0x06000060 RID: 96
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr lib, string proc);

		// Token: 0x06000061 RID: 97
		[DllImport("kernel32.dll")]
		private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		// Token: 0x06000062 RID: 98 RVA: 0x00004F88 File Offset: 0x00003188
		private unsafe static void Hook()
		{
			ulong* ptr = stackalloc ulong[checked(unchecked((UIntPtr)2) * 8)];
			if (AntiTamperJIT.ver4)
			{
				*ptr = 7218835248827755619UL;
				ptr[1] = 27756UL;
			}
			else
			{
				*ptr = 8388352820681864045UL;
				ptr[1] = 1819042862UL;
			}
			IntPtr lib = AntiTamperJIT.LoadLibrary(new string((sbyte*)ptr));
			*ptr = 127995569530215UL;
			IntPtr intPtr = *((AntiTamperJIT.getJit)Marshal.GetDelegateForFunctionPointer(AntiTamperJIT.GetProcAddress(lib, new string((sbyte*)ptr)), typeof(AntiTamperJIT.getJit)))();
			IntPtr original = *(IntPtr*)((void*)intPtr);
			IntPtr trampoline;
			uint oldPl;
			if (IntPtr.Size == 8)
			{
				trampoline = Marshal.AllocHGlobal(16);
				ulong* tptr = (ulong*)((void*)trampoline);
				*tptr = 18446744073709533256UL;
				tptr[1] = 10416984890032521215UL;
				AntiTamperJIT.VirtualProtect(trampoline, 12U, 64U, out oldPl);
				Marshal.WriteIntPtr(trampoline, 2, original);
			}
			else
			{
				trampoline = Marshal.AllocHGlobal(8);
				ulong* tptr2 = (ulong*)((void*)trampoline);
				*tptr2 = 10439625411221520312UL;
				AntiTamperJIT.VirtualProtect(trampoline, 7U, 64U, out oldPl);
				Marshal.WriteIntPtr(trampoline, 1, original);
			}
			AntiTamperJIT.originalDelegate = (AntiTamperJIT.compileMethod)Marshal.GetDelegateForFunctionPointer(trampoline, typeof(AntiTamperJIT.compileMethod));
			AntiTamperJIT.handler = new AntiTamperJIT.compileMethod(AntiTamperJIT.HookHandler);
			RuntimeHelpers.PrepareDelegate(AntiTamperJIT.originalDelegate);
			RuntimeHelpers.PrepareDelegate(AntiTamperJIT.handler);
			AntiTamperJIT.VirtualProtect(intPtr, (uint)IntPtr.Size, 64U, out oldPl);
			Marshal.WriteIntPtr(intPtr, Marshal.GetFunctionPointerForDelegate(AntiTamperJIT.handler));
			AntiTamperJIT.VirtualProtect(intPtr, (uint)IntPtr.Size, oldPl, out oldPl);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000050F4 File Offset: 0x000032F4
		private unsafe static void ExtractLocalVars(AntiTamperJIT.CORINFO_METHOD_INFO* info, uint len, byte* localVar)
		{
			void* sigInfo;
			if (AntiTamperJIT.ver4)
			{
				if (IntPtr.Size == 8)
				{
					AntiTamperJIT.CORINFO_METHOD_INFO* ptr = info + 1;
					IntPtr intPtr = AntiTamperJIT.ver5 ? 7 : 5;
					sigInfo = (void*)((byte*)((byte*)ptr + intPtr * 4) + sizeof(AntiTamperJIT.CORINFO_SIG_INFO_x64));
				}
				else
				{
					AntiTamperJIT.CORINFO_METHOD_INFO* ptr2 = info + 1;
					IntPtr intPtr2 = AntiTamperJIT.ver5 ? 5 : 4;
					sigInfo = (void*)((byte*)((byte*)ptr2 + intPtr2 * 4) + sizeof(AntiTamperJIT.CORINFO_SIG_INFO_x86));
				}
			}
			else if (IntPtr.Size == 8)
			{
				sigInfo = (void*)(info + 1 + (IntPtr)3 * 4 / (IntPtr)sizeof(AntiTamperJIT.CORINFO_METHOD_INFO) + sizeof(AntiTamperJIT.CORINFO_SIG_INFO_x64) / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO));
			}
			else
			{
				sigInfo = (void*)(info + 1 + (IntPtr)3 * 4 / (IntPtr)sizeof(AntiTamperJIT.CORINFO_METHOD_INFO) + sizeof(AntiTamperJIT.CORINFO_SIG_INFO_x86) / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO));
			}
			if (IntPtr.Size == 8)
			{
				((AntiTamperJIT.CORINFO_SIG_INFO_x64*)sigInfo)->sig = (IntPtr)((void*)localVar);
			}
			else
			{
				((AntiTamperJIT.CORINFO_SIG_INFO_x86*)sigInfo)->sig = (IntPtr)((void*)localVar);
			}
			localVar++;
			byte b = *localVar;
			ushort numArgs;
			IntPtr args;
			if ((b & 128) == 0)
			{
				numArgs = (ushort)b;
				args = (IntPtr)((void*)(localVar + 1));
			}
			else
			{
				numArgs = (ushort)(((int)b & -129) << 8 | (int)localVar[1]);
				args = (IntPtr)((void*)(localVar + 2));
			}
			if (IntPtr.Size == 8)
			{
				AntiTamperJIT.CORINFO_SIG_INFO_x64* sigInfox64 = (AntiTamperJIT.CORINFO_SIG_INFO_x64*)sigInfo;
				sigInfox64->callConv = 0U;
				sigInfox64->retType = 1;
				sigInfox64->flags = 1;
				sigInfox64->numArgs = numArgs;
				sigInfox64->args = args;
				return;
			}
			AntiTamperJIT.CORINFO_SIG_INFO_x86* sigInfox65 = (AntiTamperJIT.CORINFO_SIG_INFO_x86*)sigInfo;
			sigInfox65->callConv = 0U;
			sigInfox65->retType = 1;
			sigInfox65->flags = 1;
			sigInfox65->numArgs = numArgs;
			sigInfox65->args = args;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005240 File Offset: 0x00003440
		private unsafe static uint HookHandler(IntPtr self, AntiTamperJIT.ICorJitInfo* comp, AntiTamperJIT.CORINFO_METHOD_INFO* info, uint flags, byte** nativeEntry, uint* nativeSizeOfCode)
		{
			if (info != null && info->scope == AntiTamperJIT.moduleHnd && *info->ILCode == 20)
			{
				uint token;
				if (AntiTamperJIT.ver5)
				{
					token = ((AntiTamperJIT.getMethodDefFromMethod)Marshal.GetDelegateForFunctionPointer(comp->vfptr[(IntPtr)100 * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], typeof(AntiTamperJIT.getMethodDefFromMethod)))((IntPtr)((void*)comp), info->ftn);
				}
				else
				{
					AntiTamperJIT.ICorClassInfo* clsInfo = AntiTamperJIT.ICorStaticInfo.ICorClassInfo(AntiTamperJIT.ICorDynamicInfo.ICorStaticInfo(AntiTamperJIT.ICorJitInfo.ICorDynamicInfo(comp)));
					int gmdSlot = 12 + (AntiTamperJIT.ver4 ? 2 : 1);
					token = ((AntiTamperJIT.getMethodDefFromMethod)Marshal.GetDelegateForFunctionPointer(clsInfo->vfptr[(IntPtr)gmdSlot * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], typeof(AntiTamperJIT.getMethodDefFromMethod)))((IntPtr)((void*)clsInfo), info->ftn);
				}
				uint lo = 0U;
				uint hi = AntiTamperJIT.len;
				uint? offset = null;
				while (hi >= lo)
				{
					uint mid = lo + (hi - lo >> 1);
					uint midTok = AntiTamperJIT.ptr[(ulong)((ulong)mid << 1) * 4UL / 4UL];
					if (midTok == token)
					{
						offset = new uint?((AntiTamperJIT.ptr + (ulong)((ulong)mid << 1) * 4UL / 4UL)[1]);
						break;
					}
					if (midTok < token)
					{
						lo = mid + 1U;
					}
					else
					{
						hi = mid - 1U;
					}
				}
				if (offset == null)
				{
					return AntiTamperJIT.originalDelegate(self, comp, info, flags, nativeEntry, nativeSizeOfCode);
				}
				uint* dataPtr = AntiTamperJIT.ptr + (ulong)offset.Value * 4UL / 4UL;
				uint dataLen = *(dataPtr++);
				uint* newPtr = (uint*)((void*)Marshal.AllocHGlobal((int)((int)dataLen << 2)));
				try
				{
					AntiTamperJIT.MethodData* data = (AntiTamperJIT.MethodData*)newPtr;
					uint* copyData = newPtr;
					uint state = token * (uint)Mutation.KeyI0;
					uint counter = state;
					for (uint i = 0U; i < dataLen; i += 1U)
					{
						*copyData = (*(dataPtr++) ^ state);
						state += (*(copyData++) ^ counter);
						counter ^= (state >> 5 | state << 27);
					}
					info->ILCodeSize = data->ILCodeSize;
					if (AntiTamperJIT.ver4)
					{
						*(int*)(info + 1) = (int)data->MaxStack;
						*(int*)(info + 1 + 4 / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO)) = (int)data->EHCount;
						*(int*)(info + 1 + (IntPtr)2 * 4 / (IntPtr)sizeof(AntiTamperJIT.CORINFO_METHOD_INFO)) = (int)data->Options;
					}
					else
					{
						*(short*)(info + 1) = (short)((ushort)data->MaxStack);
						*(short*)(info + 1 + 2 / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO)) = (short)((ushort)data->EHCount);
						*(int*)(info + 1 + 4 / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO)) = (int)data->Options;
					}
					byte* body = (byte*)(data + 1);
					info->ILCode = body;
					body += info->ILCodeSize;
					if (data->LocalVars != 0U)
					{
						AntiTamperJIT.ExtractLocalVars(info, data->LocalVars, body);
						body += data->LocalVars;
					}
					AntiTamperJIT.CORINFO_EH_CLAUSE* ehPtr = (AntiTamperJIT.CORINFO_EH_CLAUSE*)body;
					uint ret;
					if (AntiTamperJIT.ver5)
					{
						AntiTamperJIT.CorJitInfoHook corJitInfoHook = AntiTamperJIT.CorJitInfoHook.Hook(comp, info->ftn, ehPtr);
						ret = AntiTamperJIT.originalDelegate(self, comp, info, flags, nativeEntry, nativeSizeOfCode);
						corJitInfoHook.Dispose();
					}
					else
					{
						AntiTamperJIT.CorMethodInfoHook corMethodInfoHook = AntiTamperJIT.CorMethodInfoHook.Hook(comp, info->ftn, ehPtr);
						ret = AntiTamperJIT.originalDelegate(self, comp, info, flags, nativeEntry, nativeSizeOfCode);
						corMethodInfoHook.Dispose();
					}
					return ret;
				}
				finally
				{
					Marshal.FreeHGlobal((IntPtr)((void*)newPtr));
				}
			}
			return AntiTamperJIT.originalDelegate(self, comp, info, flags, nativeEntry, nativeSizeOfCode);
		}

		// Token: 0x04000038 RID: 56
		private unsafe static uint* ptr;

		// Token: 0x04000039 RID: 57
		private static uint len;

		// Token: 0x0400003A RID: 58
		private static IntPtr moduleHnd;

		// Token: 0x0400003B RID: 59
		private static AntiTamperJIT.compileMethod originalDelegate;

		// Token: 0x0400003C RID: 60
		private static bool ver4;

		// Token: 0x0400003D RID: 61
		private static bool ver5;

		// Token: 0x0400003E RID: 62
		private static AntiTamperJIT.compileMethod handler;

		// Token: 0x0400003F RID: 63
		private static bool hasLinkInfo;

		// Token: 0x02000014 RID: 20
		private struct CORINFO_EH_CLAUSE
		{
		}

		// Token: 0x02000015 RID: 21
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct CORINFO_METHOD_INFO
		{
			// Token: 0x04000040 RID: 64
			public IntPtr ftn;

			// Token: 0x04000041 RID: 65
			public IntPtr scope;

			// Token: 0x04000042 RID: 66
			public unsafe byte* ILCode;

			// Token: 0x04000043 RID: 67
			public uint ILCodeSize;
		}

		// Token: 0x02000016 RID: 22
		private struct CORINFO_SIG_INFO_x64
		{
			// Token: 0x04000044 RID: 68
			public uint callConv;

			// Token: 0x04000045 RID: 69
			private uint pad1;

			// Token: 0x04000046 RID: 70
			public IntPtr retTypeClass;

			// Token: 0x04000047 RID: 71
			public IntPtr retTypeSigClass;

			// Token: 0x04000048 RID: 72
			public byte retType;

			// Token: 0x04000049 RID: 73
			public byte flags;

			// Token: 0x0400004A RID: 74
			public ushort numArgs;

			// Token: 0x0400004B RID: 75
			private uint pad2;

			// Token: 0x0400004C RID: 76
			public AntiTamperJIT.CORINFO_SIG_INST_x64 sigInst;

			// Token: 0x0400004D RID: 77
			public IntPtr args;

			// Token: 0x0400004E RID: 78
			public IntPtr sig;

			// Token: 0x0400004F RID: 79
			public IntPtr scope;

			// Token: 0x04000050 RID: 80
			public uint token;

			// Token: 0x04000051 RID: 81
			private uint pad3;
		}

		// Token: 0x02000017 RID: 23
		private struct CORINFO_SIG_INFO_x86
		{
			// Token: 0x04000052 RID: 82
			public uint callConv;

			// Token: 0x04000053 RID: 83
			public IntPtr retTypeClass;

			// Token: 0x04000054 RID: 84
			public IntPtr retTypeSigClass;

			// Token: 0x04000055 RID: 85
			public byte retType;

			// Token: 0x04000056 RID: 86
			public byte flags;

			// Token: 0x04000057 RID: 87
			public ushort numArgs;

			// Token: 0x04000058 RID: 88
			public AntiTamperJIT.CORINFO_SIG_INST_x86 sigInst;

			// Token: 0x04000059 RID: 89
			public IntPtr args;

			// Token: 0x0400005A RID: 90
			public IntPtr sig;

			// Token: 0x0400005B RID: 91
			public IntPtr scope;

			// Token: 0x0400005C RID: 92
			public uint token;
		}

		// Token: 0x02000018 RID: 24
		private struct CORINFO_SIG_INST_x64
		{
		}

		// Token: 0x02000019 RID: 25
		private struct CORINFO_SIG_INST_x86
		{
		}

		// Token: 0x0200001A RID: 26
		private struct ICorClassInfo
		{
			// Token: 0x0400005D RID: 93
			public unsafe readonly IntPtr* vfptr;
		}

		// Token: 0x0200001B RID: 27
		private struct ICorDynamicInfo
		{
			// Token: 0x06000065 RID: 101 RVA: 0x0000234E File Offset: 0x0000054E
			public unsafe static AntiTamperJIT.ICorStaticInfo* ICorStaticInfo(AntiTamperJIT.ICorDynamicInfo* ptr)
			{
				return (AntiTamperJIT.ICorStaticInfo*)(&ptr->vbptr) + ptr->vbptr[(AntiTamperJIT.hasLinkInfo ? 9 : 8) * 4] / sizeof(AntiTamperJIT.ICorStaticInfo);
			}

			// Token: 0x0400005E RID: 94
			public unsafe IntPtr* vfptr;

			// Token: 0x0400005F RID: 95
			public unsafe int* vbptr;
		}

		// Token: 0x0200001C RID: 28
		private struct ICorJitInfo
		{
			// Token: 0x06000066 RID: 102 RVA: 0x0000557C File Offset: 0x0000377C
			public unsafe static AntiTamperJIT.ICorDynamicInfo* ICorDynamicInfo(AntiTamperJIT.ICorJitInfo* ptr)
			{
				AntiTamperJIT.hasLinkInfo = (ptr->vbptr[10] > 0 && ptr->vbptr[10] >> 16 == 0);
				return (AntiTamperJIT.ICorDynamicInfo*)(&ptr->vbptr) + ptr->vbptr[(AntiTamperJIT.hasLinkInfo ? 10 : 9) * 4] / sizeof(AntiTamperJIT.ICorDynamicInfo);
			}

			// Token: 0x04000060 RID: 96
			public unsafe IntPtr* vfptr;

			// Token: 0x04000061 RID: 97
			public unsafe int* vbptr;
		}

		// Token: 0x0200001D RID: 29
		private struct ICorMethodInfo
		{
			// Token: 0x04000062 RID: 98
			public unsafe IntPtr* vfptr;
		}

		// Token: 0x0200001E RID: 30
		private struct ICorModuleInfo
		{
			// Token: 0x04000063 RID: 99
			public unsafe IntPtr* vfptr;
		}

		// Token: 0x0200001F RID: 31
		private struct ICorStaticInfo
		{
			// Token: 0x06000067 RID: 103 RVA: 0x0000236F File Offset: 0x0000056F
			public unsafe static AntiTamperJIT.ICorMethodInfo* ICorMethodInfo(AntiTamperJIT.ICorStaticInfo* ptr)
			{
				return (AntiTamperJIT.ICorMethodInfo*)(&ptr->vbptr) + ptr->vbptr[1] / sizeof(AntiTamperJIT.ICorMethodInfo);
			}

			// Token: 0x06000068 RID: 104 RVA: 0x00002382 File Offset: 0x00000582
			public unsafe static AntiTamperJIT.ICorModuleInfo* ICorModuleInfo(AntiTamperJIT.ICorStaticInfo* ptr)
			{
				return (AntiTamperJIT.ICorModuleInfo*)(&ptr->vbptr) + ptr->vbptr[2] / sizeof(AntiTamperJIT.ICorModuleInfo);
			}

			// Token: 0x06000069 RID: 105 RVA: 0x00002398 File Offset: 0x00000598
			public unsafe static AntiTamperJIT.ICorClassInfo* ICorClassInfo(AntiTamperJIT.ICorStaticInfo* ptr)
			{
				return (AntiTamperJIT.ICorClassInfo*)(&ptr->vbptr) + ptr->vbptr[3] / sizeof(AntiTamperJIT.ICorClassInfo);
			}

			// Token: 0x04000064 RID: 100
			public unsafe IntPtr* vfptr;

			// Token: 0x04000065 RID: 101
			public unsafe int* vbptr;
		}

		// Token: 0x02000020 RID: 32
		private class CorMethodInfoHook
		{
			// Token: 0x0600006A RID: 106 RVA: 0x000023AE File Offset: 0x000005AE
			private unsafe void hookEHInfo(IntPtr self, IntPtr ftn, uint EHnumber, AntiTamperJIT.CORINFO_EH_CLAUSE* clause)
			{
				if (ftn == this.ftn)
				{
					*clause = this.clauses[(ulong)EHnumber * (ulong)((long)sizeof(AntiTamperJIT.CORINFO_EH_CLAUSE)) / (ulong)sizeof(AntiTamperJIT.CORINFO_EH_CLAUSE)];
					return;
				}
				this.o_getEHinfo(self, ftn, EHnumber, clause);
			}

			// Token: 0x0600006B RID: 107 RVA: 0x000023ED File Offset: 0x000005ED
			public unsafe void Dispose()
			{
				Marshal.FreeHGlobal((IntPtr)((void*)this.newVfTbl));
				this.info->vfptr = this.oldVfTbl;
			}

			// Token: 0x0600006C RID: 108 RVA: 0x000055D4 File Offset: 0x000037D4
			public unsafe static AntiTamperJIT.CorMethodInfoHook Hook(AntiTamperJIT.ICorJitInfo* comp, IntPtr ftn, AntiTamperJIT.CORINFO_EH_CLAUSE* clauses)
			{
				AntiTamperJIT.ICorMethodInfo* mtdInfo = AntiTamperJIT.ICorStaticInfo.ICorMethodInfo(AntiTamperJIT.ICorDynamicInfo.ICorStaticInfo(AntiTamperJIT.ICorJitInfo.ICorDynamicInfo(comp)));
				IntPtr* vfTbl = mtdInfo->vfptr;
				IntPtr* newVfTbl = (IntPtr*)((void*)Marshal.AllocHGlobal(27 * IntPtr.Size));
				for (int i = 0; i < 27; i++)
				{
					newVfTbl[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = vfTbl[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)];
				}
				if (AntiTamperJIT.CorMethodInfoHook.ehNum == -1)
				{
					for (int j = 0; j < 27; j++)
					{
						bool isEh = true;
						byte* func = (byte*)((void*)vfTbl[(IntPtr)j * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
						while (*func != 233)
						{
							if ((IntPtr.Size == 8) ? (*func == 72 && func[1] == 129 && func[2] == 233) : (*func == 131 && func[1] == 233))
							{
								isEh = false;
								break;
							}
							func++;
						}
						if (isEh)
						{
							AntiTamperJIT.CorMethodInfoHook.ehNum = j;
							break;
						}
					}
				}
				AntiTamperJIT.CorMethodInfoHook ret = new AntiTamperJIT.CorMethodInfoHook
				{
					ftn = ftn,
					info = mtdInfo,
					clauses = clauses,
					newVfTbl = newVfTbl,
					oldVfTbl = vfTbl
				};
				ret.n_getEHinfo = new AntiTamperJIT.getEHinfo(ret.hookEHInfo);
				ret.o_getEHinfo = (AntiTamperJIT.getEHinfo)Marshal.GetDelegateForFunctionPointer(vfTbl[(IntPtr)AntiTamperJIT.CorMethodInfoHook.ehNum * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], typeof(AntiTamperJIT.getEHinfo));
				newVfTbl[(IntPtr)AntiTamperJIT.CorMethodInfoHook.ehNum * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = Marshal.GetFunctionPointerForDelegate(ret.n_getEHinfo);
				mtdInfo->vfptr = newVfTbl;
				return ret;
			}

			// Token: 0x0600006D RID: 109 RVA: 0x000020D5 File Offset: 0x000002D5
			public CorMethodInfoHook()
			{
			}

			// Token: 0x0600006E RID: 110 RVA: 0x00002410 File Offset: 0x00000610
			// Note: this type is marked as 'beforefieldinit'.
			static CorMethodInfoHook()
			{
			}

			// Token: 0x04000066 RID: 102
			private static int ehNum = -1;

			// Token: 0x04000067 RID: 103
			public unsafe AntiTamperJIT.CORINFO_EH_CLAUSE* clauses;

			// Token: 0x04000068 RID: 104
			public IntPtr ftn;

			// Token: 0x04000069 RID: 105
			public unsafe AntiTamperJIT.ICorMethodInfo* info;

			// Token: 0x0400006A RID: 106
			public AntiTamperJIT.getEHinfo n_getEHinfo;

			// Token: 0x0400006B RID: 107
			public unsafe IntPtr* newVfTbl;

			// Token: 0x0400006C RID: 108
			public AntiTamperJIT.getEHinfo o_getEHinfo;

			// Token: 0x0400006D RID: 109
			public unsafe IntPtr* oldVfTbl;
		}

		// Token: 0x02000021 RID: 33
		private class CorJitInfoHook
		{
			// Token: 0x0600006F RID: 111 RVA: 0x00002418 File Offset: 0x00000618
			private unsafe void hookEHInfo(IntPtr self, IntPtr ftn, uint EHnumber, AntiTamperJIT.CORINFO_EH_CLAUSE* clause)
			{
				if (ftn == this.ftn)
				{
					*clause = this.clauses[(ulong)EHnumber * (ulong)((long)sizeof(AntiTamperJIT.CORINFO_EH_CLAUSE)) / (ulong)sizeof(AntiTamperJIT.CORINFO_EH_CLAUSE)];
					return;
				}
				this.o_getEHinfo(self, ftn, EHnumber, clause);
			}

			// Token: 0x06000070 RID: 112 RVA: 0x00002457 File Offset: 0x00000657
			public unsafe void Dispose()
			{
				Marshal.FreeHGlobal((IntPtr)((void*)this.newVfTbl));
				this.info->vfptr = this.oldVfTbl;
			}

			// Token: 0x06000071 RID: 113 RVA: 0x00005764 File Offset: 0x00003964
			public unsafe static AntiTamperJIT.CorJitInfoHook Hook(AntiTamperJIT.ICorJitInfo* comp, IntPtr ftn, AntiTamperJIT.CORINFO_EH_CLAUSE* clauses)
			{
				IntPtr* vfTbl = comp->vfptr;
				IntPtr* newVfTbl = (IntPtr*)((void*)Marshal.AllocHGlobal(158 * IntPtr.Size));
				for (int i = 0; i < 158; i++)
				{
					newVfTbl[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = vfTbl[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)];
				}
				AntiTamperJIT.CorJitInfoHook ret = new AntiTamperJIT.CorJitInfoHook
				{
					ftn = ftn,
					info = comp,
					clauses = clauses,
					newVfTbl = newVfTbl,
					oldVfTbl = vfTbl
				};
				ret.n_getEHinfo = new AntiTamperJIT.getEHinfo(ret.hookEHInfo);
				ret.o_getEHinfo = (AntiTamperJIT.getEHinfo)Marshal.GetDelegateForFunctionPointer(vfTbl[(IntPtr)8 * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], typeof(AntiTamperJIT.getEHinfo));
				newVfTbl[(IntPtr)8 * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = Marshal.GetFunctionPointerForDelegate(ret.n_getEHinfo);
				comp->vfptr = newVfTbl;
				return ret;
			}

			// Token: 0x06000072 RID: 114 RVA: 0x000020D5 File Offset: 0x000002D5
			public CorJitInfoHook()
			{
			}

			// Token: 0x0400006E RID: 110
			public unsafe AntiTamperJIT.CORINFO_EH_CLAUSE* clauses;

			// Token: 0x0400006F RID: 111
			public IntPtr ftn;

			// Token: 0x04000070 RID: 112
			public unsafe AntiTamperJIT.ICorJitInfo* info;

			// Token: 0x04000071 RID: 113
			public AntiTamperJIT.getEHinfo n_getEHinfo;

			// Token: 0x04000072 RID: 114
			public unsafe IntPtr* newVfTbl;

			// Token: 0x04000073 RID: 115
			public AntiTamperJIT.getEHinfo o_getEHinfo;

			// Token: 0x04000074 RID: 116
			public unsafe IntPtr* oldVfTbl;
		}

		// Token: 0x02000022 RID: 34
		private struct MethodData
		{
			// Token: 0x04000075 RID: 117
			public readonly uint ILCodeSize;

			// Token: 0x04000076 RID: 118
			public readonly uint MaxStack;

			// Token: 0x04000077 RID: 119
			public readonly uint EHCount;

			// Token: 0x04000078 RID: 120
			public readonly uint LocalVars;

			// Token: 0x04000079 RID: 121
			public readonly uint Options;

			// Token: 0x0400007A RID: 122
			public readonly uint MulSeed;
		}

		// Token: 0x02000023 RID: 35
		// (Invoke) Token: 0x06000074 RID: 116
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private unsafe delegate uint compileMethod(IntPtr self, AntiTamperJIT.ICorJitInfo* comp, AntiTamperJIT.CORINFO_METHOD_INFO* info, uint flags, byte** nativeEntry, uint* nativeSizeOfCode);

		// Token: 0x02000024 RID: 36
		// (Invoke) Token: 0x06000078 RID: 120
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private unsafe delegate void getEHinfo(IntPtr self, IntPtr ftn, uint EHnumber, AntiTamperJIT.CORINFO_EH_CLAUSE* clause);

		// Token: 0x02000025 RID: 37
		// (Invoke) Token: 0x0600007C RID: 124
		private unsafe delegate IntPtr* getJit();

		// Token: 0x02000026 RID: 38
		// (Invoke) Token: 0x06000080 RID: 128
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint getMethodDefFromMethod(IntPtr self, IntPtr ftn);
	}
}
