using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000E7 RID: 231
	internal class CFContext
	{
		// Token: 0x06000389 RID: 905 RVA: 0x0001C83C File Offset: 0x0001AA3C
		public void AddJump(IList<Instruction> instrs, Instruction target)
		{
			bool addDefOk = false;
			bool flag = !this.Method.Module.IsClr40 && this.JunkCode && !this.Method.DeclaringType.HasGenericParameters && !this.Method.HasGenericParameters && (instrs[0].OpCode.FlowControl == FlowControl.Call || instrs[0].OpCode.FlowControl == FlowControl.Next);
			if (flag)
			{
				switch (this.Random.NextInt32(2))
				{
				case 0:
				{
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_0));
					instrs.Add(Instruction.Create(OpCodes.Brtrue, instrs[0]));
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_1));
					instrs.Add(Instruction.Create(OpCodes.Brfalse, instrs[0]));
					bool flag2 = this.Random.NextBoolean();
					if (flag2)
					{
						TypeDef randomType = this.Method.Module.Types[this.Random.NextInt32(this.Method.Module.Types.Count)];
						bool hasMethods = randomType.HasMethods;
						if (hasMethods)
						{
							instrs.Add(Instruction.Create(OpCodes.Ldtoken, randomType.Methods[this.Random.NextInt32(randomType.Methods.Count)]));
							instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.GetTypeRef("System", "RuntimeMethodHandle")));
						}
					}
					break;
				}
				case 1:
				{
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_0));
					instrs.Add(Instruction.Create(OpCodes.Brtrue, instrs[0]));
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_1));
					instrs.Add(Instruction.Create(OpCodes.Brfalse, instrs[0]));
					bool flag3 = this.Random.NextBoolean();
					if (flag3)
					{
						TypeDef randomType2 = this.Method.Module.Types[this.Random.NextInt32(this.Method.Module.Types.Count)];
						bool hasMethods2 = randomType2.HasMethods;
						if (hasMethods2)
						{
							instrs.Add(Instruction.Create(OpCodes.Ldtoken, randomType2.Methods[this.Random.NextInt32(randomType2.Methods.Count)]));
							instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.GetTypeRef("System", "RuntimeMethodHandle")));
						}
					}
					break;
				}
				case 2:
				{
					bool flag4 = this.Random.NextBoolean();
					if (flag4)
					{
						TypeDef randomType3 = this.Method.Module.Types[this.Random.NextInt32(this.Method.Module.Types.Count)];
						bool hasMethods3 = randomType3.HasMethods;
						if (hasMethods3)
						{
							instrs.Add(Instruction.Create(OpCodes.Ldtoken, randomType3.Methods[this.Random.NextInt32(randomType3.Methods.Count)]));
							instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.GetTypeRef("System", "RuntimeMethodHandle")));
							addDefOk = true;
						}
					}
					bool flag5 = !addDefOk;
					if (flag5)
					{
						instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.Random.NextBoolean() ? 0 : 1));
						instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.Int32.TypeDefOrRef));
					}
					Instruction pop = Instruction.Create(OpCodes.Pop);
					instrs.Add(Instruction.Create(OpCodes.Brfalse, instrs[0]));
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.Random.NextBoolean() ? 0 : 1));
					instrs.Add(pop);
					instrs.Add(pop);
					instrs.Add(pop);
					instrs.Add(pop);
					instrs.Add(pop);
					instrs.Add(pop);
					instrs.Add(pop);
					instrs.Add(pop);
					instrs.Add(pop);
					break;
				}
				}
			}
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0001CDA8 File Offset: 0x0001AFA8
		public void AddJunk(IList<Instruction> instrs)
		{
			bool flag = this.Method.Module.IsClr40 || !this.JunkCode;
			if (!flag)
			{
				switch (this.Random.NextInt32(5))
				{
				case 0:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(131)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)
					{
						Index = 18
					}));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					break;
				case 1:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(1907)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)
					{
						Index = 146
					}));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					break;
				case 2:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(34)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)
					{
						Index = 34
					}));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					break;
				case 3:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(67)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)
					{
						Index = 116
					}));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					break;
				case 4:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(291)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)
					{
						Index = 35
					}));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					break;
				case 5:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(255)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)
					{
						Index = 255
					}));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					break;
				}
			}
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00004A68 File Offset: 0x00002C68
		public CFContext()
		{
		}

		// Token: 0x0400029F RID: 671
		public ConfuserContext Context;

		// Token: 0x040002A0 RID: 672
		public ControlFlowProtection Protection;

		// Token: 0x040002A1 RID: 673
		public int Depth;

		// Token: 0x040002A2 RID: 674
		public IDynCipherService DynCipher;

		// Token: 0x040002A3 RID: 675
		public double Intensity;

		// Token: 0x040002A4 RID: 676
		public bool JunkCode;

		// Token: 0x040002A5 RID: 677
		public MethodDef Method;

		// Token: 0x040002A6 RID: 678
		public PredicateType Predicate;

		// Token: 0x040002A7 RID: 679
		public RandomGenerator Random;

		// Token: 0x040002A8 RID: 680
		public CFType Type;
	}
}
