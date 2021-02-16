using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x02000005 RID: 5
	internal static class AntiDump
	{
		// Token: 0x06000009 RID: 9
		[DllImport("kernel32.dll")]
		private unsafe static extern bool VirtualProtect(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

		// Token: 0x0600000A RID: 10 RVA: 0x000027A0 File Offset: 0x000009A0
		private unsafe static void Initialize()
		{
			Module module = typeof(AntiDump).Module;
			byte* bas = (byte*)((void*)Marshal.GetHINSTANCE(module));
			byte* ptr = bas + 60;
			ptr = bas + *(uint*)ptr;
			ptr += 6;
			ushort sectNum = *(ushort*)ptr;
			ptr += 14;
			ushort optSize = *(ushort*)ptr;
			ptr = ptr + 4 + optSize;
			byte* @new = stackalloc byte[checked(unchecked((UIntPtr)11) * 1)];
			uint old;
			if (module.FullyQualifiedName[0] != '<')
			{
				byte* mdDir = bas + *(uint*)(ptr - 16);
				if (*(uint*)(ptr - 120) != 0U)
				{
					byte* importDir = bas + *(uint*)(ptr - 120);
					byte* oftMod = bas + *(uint*)importDir;
					byte* modName = bas + *(uint*)(importDir + 12);
					byte* funcName = bas + *(uint*)oftMod + 2;
					AntiDump.VirtualProtect(modName, 11, 64U, out old);
					*(int*)@new = 1818522734;
					*(int*)(@new + 4) = 1818504812;
					*(short*)(@new + (IntPtr)4 * 2) = 108;
					@new[10] = 0;
					for (int i = 0; i < 11; i++)
					{
						modName[i] = @new[i];
					}
					AntiDump.VirtualProtect(funcName, 11, 64U, out old);
					*(int*)@new = 1866691662;
					*(int*)(@new + 4) = 1852404846;
					*(short*)(@new + (IntPtr)4 * 2) = 25973;
					@new[10] = 0;
					for (int j = 0; j < 11; j++)
					{
						funcName[j] = @new[j];
					}
				}
				for (int k = 0; k < (int)sectNum; k++)
				{
					AntiDump.VirtualProtect(ptr, 8, 64U, out old);
					Marshal.Copy(new byte[8], 0, (IntPtr)((void*)ptr), 8);
					ptr += 40;
				}
				AntiDump.VirtualProtect(mdDir, 72, 64U, out old);
				byte* mdHdr = bas + *(uint*)(mdDir + 8);
				*(int*)mdDir = 0;
				*(int*)(mdDir + 4) = 0;
				*(int*)(mdDir + (IntPtr)2 * 4) = 0;
				*(int*)(mdDir + (IntPtr)3 * 4) = 0;
				AntiDump.VirtualProtect(mdHdr, 4, 64U, out old);
				*(int*)mdHdr = 0;
				mdHdr += 12;
				mdHdr += *(uint*)mdHdr;
				mdHdr = (mdHdr + 7L & -4L);
				mdHdr += 2;
				ushort numOfStream = (ushort)(*mdHdr);
				mdHdr += 2;
				for (int l = 0; l < (int)numOfStream; l++)
				{
					AntiDump.VirtualProtect(mdHdr, 8, 64U, out old);
					mdHdr += 4;
					mdHdr += 4;
					for (int ii = 0; ii < 8; ii++)
					{
						AntiDump.VirtualProtect(mdHdr, 4, 64U, out old);
						*mdHdr = 0;
						mdHdr++;
						if (*mdHdr == 0)
						{
							mdHdr += 3;
							break;
						}
						*mdHdr = 0;
						mdHdr++;
						if (*mdHdr == 0)
						{
							mdHdr += 2;
							break;
						}
						*mdHdr = 0;
						mdHdr++;
						if (*mdHdr == 0)
						{
							mdHdr++;
							break;
						}
						*mdHdr = 0;
						mdHdr++;
					}
				}
				return;
			}
			uint mdDir2 = *(uint*)(ptr - 16);
			uint importDir2 = *(uint*)(ptr - 120);
			uint[] vAdrs = new uint[(int)sectNum];
			uint[] vSizes = new uint[(int)sectNum];
			uint[] rAdrs = new uint[(int)sectNum];
			for (int m = 0; m < (int)sectNum; m++)
			{
				AntiDump.VirtualProtect(ptr, 8, 64U, out old);
				Marshal.Copy(new byte[8], 0, (IntPtr)((void*)ptr), 8);
				vAdrs[m] = *(uint*)(ptr + 12);
				vSizes[m] = *(uint*)(ptr + 8);
				rAdrs[m] = *(uint*)(ptr + 20);
				ptr += 40;
			}
			if (importDir2 != 0U)
			{
				for (int n = 0; n < (int)sectNum; n++)
				{
					if (vAdrs[n] <= importDir2 && importDir2 < vAdrs[n] + vSizes[n])
					{
						importDir2 = importDir2 - vAdrs[n] + rAdrs[n];
						break;
					}
				}
				byte* importDirPtr = bas + importDir2;
				uint oftMod2 = *(uint*)importDirPtr;
				for (int i2 = 0; i2 < (int)sectNum; i2++)
				{
					if (vAdrs[i2] <= oftMod2 && oftMod2 < vAdrs[i2] + vSizes[i2])
					{
						oftMod2 = oftMod2 - vAdrs[i2] + rAdrs[i2];
						break;
					}
				}
				byte* oftModPtr = bas + oftMod2;
				uint modName2 = *(uint*)(importDirPtr + 12);
				for (int i3 = 0; i3 < (int)sectNum; i3++)
				{
					if (vAdrs[i3] <= modName2 && modName2 < vAdrs[i3] + vSizes[i3])
					{
						modName2 = modName2 - vAdrs[i3] + rAdrs[i3];
						break;
					}
				}
				uint funcName2 = *(uint*)oftModPtr + 2U;
				for (int i4 = 0; i4 < (int)sectNum; i4++)
				{
					if (vAdrs[i4] <= funcName2 && funcName2 < vAdrs[i4] + vSizes[i4])
					{
						funcName2 = funcName2 - vAdrs[i4] + rAdrs[i4];
						break;
					}
				}
				AntiDump.VirtualProtect(bas + modName2, 11, 64U, out old);
				*(int*)@new = 1818522734;
				*(int*)(@new + 4) = 1818504812;
				*(short*)(@new + (IntPtr)4 * 2) = 108;
				@new[10] = 0;
				for (int i5 = 0; i5 < 11; i5++)
				{
					(bas + modName2)[i5] = @new[i5];
				}
				AntiDump.VirtualProtect(bas + funcName2, 11, 64U, out old);
				*(int*)@new = 1866691662;
				*(int*)(@new + 4) = 1852404846;
				*(short*)(@new + (IntPtr)4 * 2) = 25973;
				@new[10] = 0;
				for (int i6 = 0; i6 < 11; i6++)
				{
					(bas + funcName2)[i6] = @new[i6];
				}
			}
			for (int i7 = 0; i7 < (int)sectNum; i7++)
			{
				if (vAdrs[i7] <= mdDir2 && mdDir2 < vAdrs[i7] + vSizes[i7])
				{
					mdDir2 = mdDir2 - vAdrs[i7] + rAdrs[i7];
					break;
				}
			}
			byte* mdDirPtr = bas + mdDir2;
			AntiDump.VirtualProtect(mdDirPtr, 72, 64U, out old);
			uint mdHdr2 = *(uint*)(mdDirPtr + 8);
			for (int i8 = 0; i8 < (int)sectNum; i8++)
			{
				if (vAdrs[i8] <= mdHdr2 && mdHdr2 < vAdrs[i8] + vSizes[i8])
				{
					mdHdr2 = mdHdr2 - vAdrs[i8] + rAdrs[i8];
					break;
				}
			}
			*(int*)mdDirPtr = 0;
			*(int*)(mdDirPtr + 4) = 0;
			*(int*)(mdDirPtr + (IntPtr)2 * 4) = 0;
			*(int*)(mdDirPtr + (IntPtr)3 * 4) = 0;
			byte* mdHdrPtr = bas + mdHdr2;
			AntiDump.VirtualProtect(mdHdrPtr, 4, 64U, out old);
			*(int*)mdHdrPtr = 0;
			mdHdrPtr += 12;
			mdHdrPtr += *(uint*)mdHdrPtr;
			mdHdrPtr = (mdHdrPtr + 7L & -4L);
			mdHdrPtr += 2;
			ushort numOfStream2 = (ushort)(*mdHdrPtr);
			mdHdrPtr += 2;
			for (int i9 = 0; i9 < (int)numOfStream2; i9++)
			{
				AntiDump.VirtualProtect(mdHdrPtr, 8, 64U, out old);
				mdHdrPtr += 4;
				mdHdrPtr += 4;
				for (int ii2 = 0; ii2 < 8; ii2++)
				{
					AntiDump.VirtualProtect(mdHdrPtr, 4, 64U, out old);
					*mdHdrPtr = 0;
					mdHdrPtr++;
					if (*mdHdrPtr == 0)
					{
						mdHdrPtr += 3;
						break;
					}
					*mdHdrPtr = 0;
					mdHdrPtr++;
					if (*mdHdrPtr == 0)
					{
						mdHdrPtr += 2;
						break;
					}
					*mdHdrPtr = 0;
					mdHdrPtr++;
					if (*mdHdrPtr == 0)
					{
						mdHdrPtr++;
						break;
					}
					*mdHdrPtr = 0;
					mdHdrPtr++;
				}
			}
		}
	}
}
