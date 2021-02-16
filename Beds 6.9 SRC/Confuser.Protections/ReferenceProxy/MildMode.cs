using System;
using System.Collections.Generic;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x0200005D RID: 93
	internal class MildMode : RPMode
	{
		// Token: 0x06000196 RID: 406 RVA: 0x00004A34 File Offset: 0x00002C34
		public override void Finalize(RPContext ctx)
		{
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000D2D4 File Offset: 0x0000B4D4
		public override void ProcessCall(RPContext ctx, int instrIndex)
		{
			Instruction invoke = ctx.Body.Instructions[instrIndex];
			IMethod target = (IMethod)invoke.Operand;
			bool isValueType = target.DeclaringType.ResolveTypeDefThrow().IsValueType;
			if (!isValueType)
			{
				bool flag = !target.ResolveThrow().IsPublic && !target.ResolveThrow().IsAssembly;
				if (!flag)
				{
					Tuple<Code, TypeDef, IMethod> key = Tuple.Create<Code, TypeDef, IMethod>(invoke.OpCode.Code, ctx.Method.DeclaringType, target);
					MethodDef proxy;
					bool flag2 = !this.proxies.TryGetValue(key, out proxy);
					if (flag2)
					{
						MethodSig sig = RPMode.CreateProxySignature(ctx, target, invoke.OpCode.Code == Code.Newobj);
						proxy = new MethodDefUser(ctx.Name.RandomName(), sig);
						proxy.Attributes = MethodAttributes.Static;
						proxy.ImplAttributes = MethodImplAttributes.IL;
						ctx.Method.DeclaringType.Methods.Add(proxy);
						bool flag3 = invoke.OpCode.Code == Code.Call && target.ResolveThrow().IsVirtual;
						if (flag3)
						{
							proxy.IsStatic = false;
							sig.HasThis = true;
							sig.Params.RemoveAt(0);
						}
						ctx.Marker.Mark(proxy, ctx.Protection);
						ctx.Name.Analyze(proxy);
						ctx.Name.SetCanRename(proxy, false);
						proxy.Body = new CilBody();
						for (int i = 0; i < proxy.Parameters.Count; i++)
						{
							proxy.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, proxy.Parameters[i]));
						}
						proxy.Body.Instructions.Add(Instruction.Create(invoke.OpCode, target));
						proxy.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
						this.proxies[key] = proxy;
					}
					invoke.OpCode = OpCodes.Call;
					bool hasGenericParameters = ctx.Method.DeclaringType.HasGenericParameters;
					if (hasGenericParameters)
					{
						GenericVar[] genArgs = new GenericVar[ctx.Method.DeclaringType.GenericParameters.Count];
						for (int j = 0; j < genArgs.Length; j++)
						{
							genArgs[j] = new GenericVar(j);
						}
						invoke.Operand = new MemberRefUser(ctx.Module, proxy.Name, proxy.MethodSig, new GenericInstSig((ClassOrValueTypeSig)ctx.Method.DeclaringType.ToTypeSig(), genArgs).ToTypeDefOrRef());
					}
					else
					{
						invoke.Operand = proxy;
					}
				}
			}
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000523C File Offset: 0x0000343C
		public MildMode()
		{
		}

		// Token: 0x040000C8 RID: 200
		private readonly Dictionary<Tuple<Code, TypeDef, IMethod>, MethodDef> proxies = new Dictionary<Tuple<Code, TypeDef, IMethod>, MethodDef>();
	}
}
