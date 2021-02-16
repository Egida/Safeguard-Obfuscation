using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000CE RID: 206
	internal class CFContext
	{
		// Token: 0x0600032A RID: 810 RVA: 0x0001A31C File Offset: 0x0001851C
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

		// Token: 0x0600032B RID: 811 RVA: 0x0001A59C File Offset: 0x0001879C
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

		// Token: 0x0600032C RID: 812 RVA: 0x00004A68 File Offset: 0x00002C68
		public CFContext()
		{
		}

		// Token: 0x04000254 RID: 596
		public ConfuserContext Context;

		// Token: 0x04000255 RID: 597
		public int Depth;

		// Token: 0x04000256 RID: 598
		public IDynCipherService DynCipher;

		// Token: 0x04000257 RID: 599
		public double Intensity;

		// Token: 0x04000258 RID: 600
		public bool JunkCode;

		// Token: 0x04000259 RID: 601
		public MethodDef Method;

		// Token: 0x0400025A RID: 602
		public PredicateType Predicate;

		// Token: 0x0400025B RID: 603
		public DupCtrlFlowModProtection Protection;

		// Token: 0x0400025C RID: 604
		public RandomGenerator Random;

		// Token: 0x0400025D RID: 605
		public CFType Type;
	}
}
