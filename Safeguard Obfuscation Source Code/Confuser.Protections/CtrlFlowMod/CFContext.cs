using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000B9 RID: 185
	internal class CFContext
	{
		// Token: 0x060002D9 RID: 729 RVA: 0x00017F7C File Offset: 0x0001617C
		public void AddJump(IList<Instruction> instrs, Instruction target)
		{
			bool flag2 = !this.Method.Module.IsClr40 && this.JunkCode && !this.Method.DeclaringType.HasGenericParameters && !this.Method.HasGenericParameters && (instrs[0].OpCode.FlowControl == FlowControl.Call || instrs[0].OpCode.FlowControl == FlowControl.Next);
			if (flag2)
			{
				switch (this.Random.NextInt32(3))
				{
				case 0:
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_0));
					instrs.Add(Instruction.Create(OpCodes.Brtrue, instrs[0]));
					break;
				case 1:
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_1));
					instrs.Add(Instruction.Create(OpCodes.Brfalse, instrs[0]));
					break;
				case 2:
				{
					bool flag = false;
					bool flag3 = this.Random.NextBoolean();
					if (flag3)
					{
						TypeDef def = this.Method.Module.Types[this.Random.NextInt32(this.Method.Module.Types.Count)];
						bool hasMethods = def.HasMethods;
						if (hasMethods)
						{
							instrs.Add(Instruction.Create(OpCodes.Ldtoken, def.Methods[this.Random.NextInt32(def.Methods.Count)]));
							instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.GetTypeRef("System", "RuntimeMethodHandle")));
							flag = true;
						}
					}
					bool flag4 = !flag;
					if (flag4)
					{
						instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.Random.NextBoolean() ? 0 : 1));
						instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.Int32.TypeDefOrRef));
					}
					Instruction item = Instruction.Create(OpCodes.Pop);
					instrs.Add(Instruction.Create(OpCodes.Brfalse, instrs[0]));
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.Random.NextBoolean() ? 0 : 1));
					instrs.Add(item);
					break;
				}
				}
			}
			instrs.Add(Instruction.Create(OpCodes.Br, target));
		}

		// Token: 0x060002DA RID: 730 RVA: 0x000181FC File Offset: 0x000163FC
		public void AddJunk(IList<Instruction> instrs)
		{
			bool flag = !this.Method.Module.IsClr40 && this.JunkCode;
			if (flag)
			{
				switch (this.Random.NextInt32(6))
				{
				case 0:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					break;
				case 1:
					instrs.Add(Instruction.Create(OpCodes.Dup));
					break;
				case 2:
					instrs.Add(Instruction.Create(OpCodes.Throw));
					break;
				case 3:
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(255)));
					break;
				case 4:
				{
					Local local = new Local(null)
					{
						Index = 255
					};
					instrs.Add(Instruction.Create(OpCodes.Ldloc, local));
					break;
				}
				case 5:
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					break;
				}
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00004A68 File Offset: 0x00002C68
		public CFContext()
		{
		}

		// Token: 0x04000213 RID: 531
		public ConfuserContext Context;

		// Token: 0x04000214 RID: 532
		public int Depth;

		// Token: 0x04000215 RID: 533
		public IDynCipherService DynCipher;

		// Token: 0x04000216 RID: 534
		public double Intensity;

		// Token: 0x04000217 RID: 535
		public bool JunkCode;

		// Token: 0x04000218 RID: 536
		public MethodDef Method;

		// Token: 0x04000219 RID: 537
		public PredicateType Predicate;

		// Token: 0x0400021A RID: 538
		public CtrlFlowModProtection Protection;

		// Token: 0x0400021B RID: 539
		public RandomGenerator Random;

		// Token: 0x0400021C RID: 540
		public CFType Type;
	}
}
