using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers
{
	/// <summary>
	///     Provides methods to mutated injected methods.
	/// </summary>
	// Token: 0x020000AE RID: 174
	public static class MutationHelper
	{
		/// <summary>
		///     Replaces the mutation key placeholder in method with actual key.
		/// </summary>
		/// <param name="method">The method to process.</param>
		/// <param name="keyId">The mutation key ID.</param>
		/// <param name="key">The actual key.</param>
		// Token: 0x060003E0 RID: 992 RVA: 0x00017A6C File Offset: 0x00015C6C
		public static void InjectKey(MethodDef method, int keyId, int key)
		{
			foreach (Instruction instr in method.Body.Instructions)
			{
				bool flag = instr.OpCode == OpCodes.Ldsfld;
				if (flag)
				{
					IField field = (IField)instr.Operand;
					int _keyId;
					bool flag2 = field.DeclaringType.FullName == "Mutation" && MutationHelper.field2index.TryGetValue(field.Name, out _keyId) && _keyId == keyId;
					if (flag2)
					{
						instr.OpCode = OpCodes.Ldc_I4;
						instr.Operand = key;
					}
				}
			}
		}

		/// <summary>
		///     Replaces the mutation key placeholders in method with actual keys.
		/// </summary>
		/// <param name="method">The method to process.</param>
		/// <param name="keyIds">The mutation key IDs.</param>
		/// <param name="keys">The actual keys.</param>
		// Token: 0x060003E1 RID: 993 RVA: 0x00017B38 File Offset: 0x00015D38
		public static void InjectKeys(MethodDef method, int[] keyIds, int[] keys)
		{
			foreach (Instruction instr in method.Body.Instructions)
			{
				bool flag = instr.OpCode == OpCodes.Ldsfld;
				if (flag)
				{
					IField field = (IField)instr.Operand;
					int _keyIndex;
					bool flag2 = field.DeclaringType.FullName == "Mutation" && MutationHelper.field2index.TryGetValue(field.Name, out _keyIndex) && (_keyIndex = Array.IndexOf<int>(keyIds, _keyIndex)) != -1;
					if (flag2)
					{
						instr.OpCode = OpCodes.Ldc_I4;
						instr.Operand = keys[_keyIndex];
					}
				}
			}
		}

		/// <summary>
		///     Replaces the placeholder call in method with actual instruction sequence.
		/// </summary>
		/// <param name="method">The methodto process.</param>
		/// <param name="repl">The function replacing the argument of placeholder call with actual instruction sequence.</param>
		// Token: 0x060003E2 RID: 994 RVA: 0x00017C14 File Offset: 0x00015E14
		public static void ReplacePlaceholder(MethodDef method, Func<Instruction[], Instruction[]> repl)
		{
			MethodTrace trace = new MethodTrace(method).Trace();
			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instr = method.Body.Instructions[i];
				bool flag = instr.OpCode == OpCodes.Call;
				if (flag)
				{
					IMethod operand = (IMethod)instr.Operand;
					bool flag2 = operand.DeclaringType.FullName == "Mutation" && operand.Name == "Placeholder";
					if (flag2)
					{
						int[] argIndexes = trace.TraceArguments(instr);
						bool flag3 = argIndexes == null;
						if (flag3)
						{
							throw new ArgumentException("Failed to trace placeholder argument.");
						}
						int argIndex = argIndexes[0];
						Instruction[] arg = method.Body.Instructions.Skip(argIndex).Take(i - argIndex).ToArray<Instruction>();
						for (int j = 0; j < arg.Length; j++)
						{
							method.Body.Instructions.RemoveAt(argIndex);
						}
						method.Body.Instructions.RemoveAt(argIndex);
						arg = repl(arg);
						for (int k = arg.Length - 1; k >= 0; k--)
						{
							method.Body.Instructions.Insert(argIndex, arg[k]);
						}
						break;
					}
				}
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00017D90 File Offset: 0x00015F90
		// Note: this type is marked as 'beforefieldinit'.
		static MutationHelper()
		{
		}

		// Token: 0x0400028F RID: 655
		private static readonly Dictionary<string, int> field2index = new Dictionary<string, int>
		{
			{
				"KeyI0",
				0
			},
			{
				"KeyI1",
				1
			},
			{
				"KeyI2",
				2
			},
			{
				"KeyI3",
				3
			},
			{
				"KeyI4",
				4
			},
			{
				"KeyI5",
				5
			},
			{
				"KeyI6",
				6
			},
			{
				"KeyI7",
				7
			},
			{
				"KeyI8",
				8
			},
			{
				"KeyI9",
				9
			},
			{
				"KeyI10",
				10
			},
			{
				"KeyI11",
				11
			},
			{
				"KeyI12",
				12
			},
			{
				"KeyI13",
				13
			},
			{
				"KeyI14",
				14
			},
			{
				"KeyI15",
				15
			}
		};

		// Token: 0x04000290 RID: 656
		private const string mutationType = "Mutation";
	}
}
