using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Confuser.Runtime
{
	// Token: 0x02000032 RID: 50
	internal static class RefProxyStrong
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x000066E8 File Offset: 0x000048E8
		internal static void Initialize(RuntimeFieldHandle field, byte opKey)
		{
			FieldInfo fieldInfo = FieldInfo.GetFieldFromHandle(field);
			byte[] sig = fieldInfo.Module.ResolveSignature(fieldInfo.MetadataToken);
			int len = sig.Length;
			int num = fieldInfo.GetOptionalCustomModifiers()[0].MetadataToken + (int)((int)(fieldInfo.Name[Mutation.KeyI0] ^ (char)sig[--len]) << Mutation.KeyI4) + (int)((int)(fieldInfo.Name[Mutation.KeyI1] ^ (char)sig[--len]) << Mutation.KeyI5) + (int)((int)(fieldInfo.Name[Mutation.KeyI2] ^ (char)sig[--len]) << Mutation.KeyI6);
			len--;
			int token = Mutation.Placeholder<int>(num + (int)((int)(fieldInfo.Name[Mutation.KeyI3] ^ (char)sig[len - 1]) << (Mutation.KeyI7 & 31)));
			token *= fieldInfo.GetCustomAttributes(false)[0].GetHashCode();
			MethodBase method = fieldInfo.Module.ResolveMethod(token);
			Type delegateType = fieldInfo.FieldType;
			if (method.IsStatic)
			{
				fieldInfo.SetValue(null, Delegate.CreateDelegate(delegateType, (MethodInfo)method));
				return;
			}
			DynamicMethod dm = null;
			Type[] argTypes = null;
			foreach (MethodInfo invoke in fieldInfo.FieldType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				if (invoke.DeclaringType == delegateType)
				{
					ParameterInfo[] paramTypes = invoke.GetParameters();
					argTypes = new Type[paramTypes.Length];
					for (int i = 0; i < argTypes.Length; i++)
					{
						argTypes[i] = paramTypes[i].ParameterType;
					}
					Type declType = method.DeclaringType;
					dm = new DynamicMethod("", invoke.ReturnType, argTypes, (declType.IsInterface || declType.IsArray) ? delegateType : declType, true);
					break;
				}
			}
			DynamicILInfo info = dm.GetDynamicILInfo();
			DynamicILInfo dynamicILInfo = info;
			byte[] array = new byte[2];
			array[0] = 7;
			dynamicILInfo.SetLocalSignature(array);
			byte[] code = new byte[7 * argTypes.Length + 6];
			int index = 0;
			ParameterInfo[] mParams = method.GetParameters();
			int mIndex = method.IsConstructor ? 0 : -1;
			for (int j = 0; j < argTypes.Length; j++)
			{
				code[index++] = 14;
				code[index++] = (byte)j;
				Type mType = (mIndex == -1) ? method.DeclaringType : mParams[mIndex].ParameterType;
				if (mType.IsClass && !mType.IsPointer && !mType.IsByRef)
				{
					int cToken = info.GetTokenFor(mType.TypeHandle);
					code[index++] = 116;
					code[index++] = (byte)cToken;
					code[index++] = (byte)(cToken >> 8);
					code[index++] = (byte)(cToken >> 16);
					code[index++] = (byte)(cToken >> 24);
				}
				else
				{
					index += 5;
				}
				mIndex++;
			}
			code[index++] = ((byte)fieldInfo.Name[Mutation.KeyI8] ^ opKey);
			int dmToken = info.GetTokenFor(method.MethodHandle);
			code[index++] = (byte)dmToken;
			code[index++] = (byte)(dmToken >> 8);
			code[index++] = (byte)(dmToken >> 16);
			code[index++] = (byte)(dmToken >> 24);
			code[index] = 42;
			info.SetCode(code, argTypes.Length + 1);
			fieldInfo.SetValue(null, dm.CreateDelegate(delegateType));
		}
	}
}
