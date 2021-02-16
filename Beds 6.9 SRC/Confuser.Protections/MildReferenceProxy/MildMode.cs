using System;
using System.Collections.Generic;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.MildReferenceProxy
{
	// Token: 0x02000074 RID: 116
	internal class MildMode : RPMode
	{
		// Token: 0x060001D9 RID: 473 RVA: 0x00004A34 File Offset: 0x00002C34
		public override void Finalize(RPContext ctx)
		{
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000F4F8 File Offset: 0x0000D6F8
		public override void ProcessCall(RPContext ctx, int instrIndex)
		{
			Instruction instruction = ctx.Body.Instructions[instrIndex];
			IMethod operand = (IMethod)instruction.Operand;
			bool flag = !operand.DeclaringType.ResolveTypeDefThrow().IsValueType && (operand.ResolveThrow().IsPublic || operand.ResolveThrow().IsAssembly);
			if (flag)
			{
				Tuple<Code, TypeDef, IMethod> key = Tuple.Create<Code, TypeDef, IMethod>(instruction.OpCode.Code, ctx.Method.DeclaringType, operand);
				MethodDef def;
				bool flag2 = !this.proxies.TryGetValue(key, out def);
				if (flag2)
				{
					MethodSig methodSig = RPMode.CreateProxySignature(ctx, operand, instruction.OpCode.Code == Code.Newobj);
					def = new MethodDefUser(ctx.Name.RandomName(), methodSig)
					{
						Attributes = MethodAttributes.Static,
						ImplAttributes = MethodImplAttributes.IL
					};
					ctx.Method.DeclaringType.Methods.Add(def);
					bool flag3 = instruction.OpCode.Code == Code.Call && operand.ResolveThrow().IsVirtual;
					if (flag3)
					{
						def.IsStatic = false;
						methodSig.HasThis = true;
						methodSig.Params.RemoveAt(0);
					}
					ctx.Marker.Mark(def, ctx.Protection);
					ctx.Name.Analyze(def);
					ctx.Name.SetCanRename(def, false);
					def.Body = new CilBody();
					for (int i = 0; i < def.Parameters.Count; i++)
					{
						def.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, def.Parameters[i]));
					}
					def.Body.Instructions.Add(Instruction.Create(instruction.OpCode, operand));
					def.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
					this.proxies[key] = def;
				}
				instruction.OpCode = OpCodes.Call;
				bool hasGenericParameters = ctx.Method.DeclaringType.HasGenericParameters;
				if (hasGenericParameters)
				{
					GenericVar[] genArgs = new GenericVar[ctx.Method.DeclaringType.GenericParameters.Count];
					for (int j = 0; j < genArgs.Length; j++)
					{
						genArgs[j] = new GenericVar(j);
					}
					instruction.Operand = new MemberRefUser(ctx.Module, def.Name, def.MethodSig, new GenericInstSig((ClassOrValueTypeSig)ctx.Method.DeclaringType.ToTypeSig(), genArgs).ToTypeDefOrRef());
				}
				else
				{
					instruction.Operand = def;
				}
			}
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000533F File Offset: 0x0000353F
		public MildMode()
		{
		}

		// Token: 0x04000114 RID: 276
		private readonly Dictionary<Tuple<Code, TypeDef, IMethod>, MethodDef> proxies = new Dictionary<Tuple<Code, TypeDef, IMethod>, MethodDef>();
	}
}
