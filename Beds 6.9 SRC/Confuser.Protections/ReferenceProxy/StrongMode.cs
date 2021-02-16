using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x02000061 RID: 97
	internal class StrongMode : RPMode
	{
		// Token: 0x060001A4 RID: 420 RVA: 0x0000DA04 File Offset: 0x0000BC04
		private static int? TraceBeginning(RPContext ctx, int index, int argCount)
		{
			bool flag = ctx.BranchTargets.Contains(ctx.Body.Instructions[index]);
			int? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int currentStack = argCount;
				int currentIndex = index;
				while (currentStack > 0)
				{
					currentIndex--;
					Instruction currentInstr = ctx.Body.Instructions[currentIndex];
					bool flag2 = currentInstr.OpCode == OpCodes.Pop || currentInstr.OpCode == OpCodes.Dup;
					if (flag2)
					{
						return null;
					}
					FlowControl flowControl = currentInstr.OpCode.FlowControl;
					if (flowControl - FlowControl.Break > 1 && flowControl - FlowControl.Meta > 1)
					{
						return null;
					}
					int push;
					int pop;
					currentInstr.CalculateStackUsage(out push, out pop);
					currentStack += pop;
					currentStack -= push;
					bool flag3 = ctx.BranchTargets.Contains(currentInstr) && currentStack != 0;
					if (flag3)
					{
						return null;
					}
				}
				bool flag4 = currentStack < 0;
				if (flag4)
				{
					result = null;
				}
				else
				{
					result = new int?(currentIndex);
				}
			}
			return result;
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000DB38 File Offset: 0x0000BD38
		public override void ProcessCall(RPContext ctx, int instrIndex)
		{
			Instruction invoke = ctx.Body.Instructions[instrIndex];
			TypeDef declType = ((IMethod)invoke.Operand).DeclaringType.ResolveTypeDefThrow();
			bool flag = !declType.Module.IsILOnly;
			if (!flag)
			{
				bool isGlobalModuleType = declType.IsGlobalModuleType;
				if (!isGlobalModuleType)
				{
					int push;
					int pop;
					invoke.CalculateStackUsage(out push, out pop);
					int? begin = StrongMode.TraceBeginning(ctx, instrIndex, pop);
					bool fallBack = begin == null;
					bool flag2 = fallBack;
					if (flag2)
					{
						this.ProcessBridge(ctx, instrIndex);
					}
					else
					{
						this.ProcessInvoke(ctx, instrIndex, begin.Value);
					}
				}
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000DBDC File Offset: 0x0000BDDC
		private void ProcessBridge(RPContext ctx, int instrIndex)
		{
			Instruction instr = ctx.Body.Instructions[instrIndex];
			IMethod target = (IMethod)instr.Operand;
			TypeDef declType = target.DeclaringType.ResolveTypeDefThrow();
			bool flag = !declType.Module.IsILOnly;
			if (!flag)
			{
				bool isGlobalModuleType = declType.IsGlobalModuleType;
				if (!isGlobalModuleType)
				{
					Tuple<Code, IMethod, IRPEncoding> key = Tuple.Create<Code, IMethod, IRPEncoding>(instr.OpCode.Code, target, ctx.EncodingHandler);
					Tuple<FieldDef, MethodDef> proxy;
					bool flag2 = this.fields.TryGetValue(key, out proxy);
					if (flag2)
					{
						bool flag3 = proxy.Item2 != null;
						if (flag3)
						{
							instr.OpCode = OpCodes.Call;
							instr.Operand = proxy.Item2;
							return;
						}
					}
					else
					{
						proxy = new Tuple<FieldDef, MethodDef>(null, null);
					}
					MethodSig sig = RPMode.CreateProxySignature(ctx, target, instr.OpCode.Code == Code.Newobj);
					TypeDef delegateType = RPMode.GetDelegateType(ctx, sig);
					bool flag4 = proxy.Item1 == null;
					if (flag4)
					{
						proxy = new Tuple<FieldDef, MethodDef>(this.CreateField(ctx, delegateType), proxy.Item2);
					}
					Debug.Assert(proxy.Item2 == null);
					proxy = new Tuple<FieldDef, MethodDef>(proxy.Item1, this.CreateBridge(ctx, delegateType, proxy.Item1, sig));
					this.fields[key] = proxy;
					instr.OpCode = OpCodes.Call;
					instr.Operand = proxy.Item2;
					MethodDef targetDef = target.ResolveMethodDef();
					bool flag5 = targetDef != null;
					if (flag5)
					{
						ctx.Context.Annotations.Set<object>(targetDef, ReferenceProxyProtection.Targeted, ReferenceProxyProtection.Targeted);
					}
				}
			}
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000DD78 File Offset: 0x0000BF78
		private void ProcessInvoke(RPContext ctx, int instrIndex, int argBeginIndex)
		{
			Instruction instr = ctx.Body.Instructions[instrIndex];
			IMethod target = (IMethod)instr.Operand;
			MethodSig sig = RPMode.CreateProxySignature(ctx, target, instr.OpCode.Code == Code.Newobj);
			TypeDef delegateType = RPMode.GetDelegateType(ctx, sig);
			Tuple<Code, IMethod, IRPEncoding> key = Tuple.Create<Code, IMethod, IRPEncoding>(instr.OpCode.Code, target, ctx.EncodingHandler);
			Tuple<FieldDef, MethodDef> proxy;
			bool flag = !this.fields.TryGetValue(key, out proxy);
			if (flag)
			{
				proxy = new Tuple<FieldDef, MethodDef>(this.CreateField(ctx, delegateType), null);
				this.fields[key] = proxy;
			}
			bool flag2 = argBeginIndex == instrIndex;
			if (flag2)
			{
				ctx.Body.Instructions.Insert(instrIndex + 1, new Instruction(OpCodes.Call, delegateType.FindMethod("Invoke")));
				instr.OpCode = OpCodes.Ldsfld;
				instr.Operand = proxy.Item1;
			}
			else
			{
				Instruction argBegin = ctx.Body.Instructions[argBeginIndex];
				ctx.Body.Instructions.Insert(argBeginIndex + 1, new Instruction(argBegin.OpCode, argBegin.Operand));
				argBegin.OpCode = OpCodes.Ldsfld;
				argBegin.Operand = proxy.Item1;
				instr.OpCode = OpCodes.Call;
				instr.Operand = delegateType.FindMethod("Invoke");
			}
			MethodDef targetDef = target.ResolveMethodDef();
			bool flag3 = targetDef != null;
			if (flag3)
			{
				ctx.Context.Annotations.Set<object>(targetDef, ReferenceProxyProtection.Targeted, ReferenceProxyProtection.Targeted);
			}
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000DF14 File Offset: 0x0000C114
		private MethodDef CreateBridge(RPContext ctx, TypeDef delegateType, FieldDef field, MethodSig sig)
		{
			MethodDefUser method = new MethodDefUser(ctx.Name.RandomName(), sig);
			method.Attributes = MethodAttributes.Static;
			method.ImplAttributes = MethodImplAttributes.IL;
			method.Body = new CilBody();
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldsfld, field));
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, method.Parameters[i]));
			}
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, delegateType.FindMethod("Invoke")));
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			delegateType.Methods.Add(method);
			ctx.Context.Registry.GetService<IMarkerService>().Mark(method, ctx.Protection);
			ctx.Name.SetCanRename(method, false);
			return method;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000E038 File Offset: 0x0000C238
		private FieldDef CreateField(RPContext ctx, TypeDef delegateType)
		{
			TypeDef randomType;
			do
			{
				randomType = ctx.Module.Types[ctx.Random.NextInt32(ctx.Module.Types.Count)];
			}
			while (randomType.HasGenericParameters || randomType.IsGlobalModuleType || randomType.IsDelegate());
			TypeSig fieldType = new CModOptSig(randomType, delegateType.ToTypeSig());
			FieldDefUser field = new FieldDefUser("", new FieldSig(fieldType), FieldAttributes.Private | FieldAttributes.FamANDAssem | FieldAttributes.Static);
			field.CustomAttributes.Add(new CustomAttribute(this.GetKeyAttr(ctx).FindInstanceConstructors().First<MethodDef>()));
			delegateType.Fields.Add(field);
			ctx.Marker.Mark(field, ctx.Protection);
			ctx.Name.SetCanRename(field, false);
			return field;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000E110 File Offset: 0x0000C310
		private TypeDef GetKeyAttr(RPContext ctx)
		{
			bool flag = this.keyAttrs == null;
			if (flag)
			{
				this.keyAttrs = new Tuple<TypeDef, Func<int, int>>[16];
			}
			int index = ctx.Random.NextInt32(this.keyAttrs.Length);
			bool flag2 = this.keyAttrs[index] == null;
			if (flag2)
			{
				TypeDef rtType = ctx.Context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.RefProxyKey");
				TypeDef injectedAttr = InjectHelper.Inject(rtType, ctx.Module);
				injectedAttr.Name = ctx.Name.RandomName();
				injectedAttr.Namespace = string.Empty;
				Variable var = new Variable("{VAR}");
				Variable result = new Variable("{RESULT}");
				Expression expression;
				Expression inverse;
				ctx.DynCipher.GenerateExpressionPair(ctx.Random, new VariableExpression
				{
					Variable = var
				}, new VariableExpression
				{
					Variable = result
				}, ctx.Depth, out expression, out inverse);
				Func<int, int> expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
				{
					Tuple.Create<string, Type>("{VAR}", typeof(int))
				}).GenerateCIL(expression).Compile<Func<int, int>>();
				MethodDef ctor = injectedAttr.FindMethod(".ctor");
				MutationHelper.ReplacePlaceholder(ctor, delegate(Instruction[] arg)
				{
					List<Instruction> invCompiled = new List<Instruction>();
					new StrongMode.CodeGen(arg, ctor, invCompiled).GenerateCIL(inverse);
					return invCompiled.ToArray();
				});
				this.keyAttrs[index] = Tuple.Create<TypeDef, Func<int, int>>(injectedAttr, expCompiled);
				ctx.Module.AddAsNonNestedType(injectedAttr);
				foreach (IDnlibDef def in injectedAttr.FindDefinitions())
				{
					bool flag3 = def.Name == "GetHashCode";
					if (flag3)
					{
						ctx.Name.MarkHelper(def, ctx.Marker, ctx.Protection);
						((MethodDef)def).Access = MethodAttributes.Public;
					}
					else
					{
						ctx.Name.MarkHelper(def, ctx.Marker, ctx.Protection);
					}
				}
			}
			return this.keyAttrs[index].Item1;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000E34C File Offset: 0x0000C54C
		private StrongMode.InitMethodDesc GetInitMethod(RPContext ctx, IRPEncoding encoding)
		{
			StrongMode.InitMethodDesc[] initDescs;
			bool flag = !this.inits.TryGetValue(encoding, out initDescs);
			if (flag)
			{
				initDescs = (this.inits[encoding] = new StrongMode.InitMethodDesc[ctx.InitCount]);
			}
			int index = ctx.Random.NextInt32(initDescs.Length);
			bool flag2 = initDescs[index] == null;
			if (flag2)
			{
				TypeDef rtType = ctx.Context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.RefProxyStrong");
				MethodDef injectedMethod = InjectHelper.Inject(rtType.FindMethod("Initialize"), ctx.Module);
				ctx.Module.GlobalType.Methods.Add(injectedMethod);
				injectedMethod.Access = MethodAttributes.PrivateScope;
				injectedMethod.Name = ctx.Name.RandomName();
				ctx.Name.SetCanRename(injectedMethod, false);
				ctx.Marker.Mark(injectedMethod, ctx.Protection);
				StrongMode.InitMethodDesc desc = new StrongMode.InitMethodDesc
				{
					Method = injectedMethod
				};
				int[] order = Enumerable.Range(0, 5).ToArray<int>();
				ctx.Random.Shuffle<int>(order);
				desc.OpCodeIndex = order[4];
				desc.TokenNameOrder = new int[4];
				Array.Copy(order, 0, desc.TokenNameOrder, 0, 4);
				desc.TokenByteOrder = (from x in Enumerable.Range(0, 4)
				select x * 8).ToArray<int>();
				ctx.Random.Shuffle<int>(desc.TokenByteOrder);
				int[] keyInjection = new int[9];
				Array.Copy(desc.TokenNameOrder, 0, keyInjection, 0, 4);
				Array.Copy(desc.TokenByteOrder, 0, keyInjection, 4, 4);
				keyInjection[8] = desc.OpCodeIndex;
				MutationHelper.InjectKeys(injectedMethod, Enumerable.Range(0, 9).ToArray<int>(), keyInjection);
				MutationHelper.ReplacePlaceholder(injectedMethod, (Instruction[] arg) => encoding.EmitDecode(injectedMethod, ctx, arg));
				desc.Encoding = encoding;
				initDescs[index] = desc;
			}
			return initDescs[index];
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000E62C File Offset: 0x0000C82C
		public override void Finalize(RPContext ctx)
		{
			foreach (KeyValuePair<Tuple<Code, IMethod, IRPEncoding>, Tuple<FieldDef, MethodDef>> field in this.fields)
			{
				StrongMode.InitMethodDesc init = this.GetInitMethod(ctx, field.Key.Item3);
				byte opKey;
				do
				{
					opKey = ctx.Random.NextByte();
				}
				while (opKey == (byte)field.Key.Item1);
				TypeDef delegateType = field.Value.Item1.DeclaringType;
				MethodDef cctor = delegateType.FindOrCreateStaticConstructor();
				cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init.Method));
				cctor.Body.Instructions.Insert(0, Instruction.CreateLdcI4((int)opKey));
				cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldtoken, field.Value.Item1));
				this.fieldDescs.Add(new StrongMode.FieldDesc
				{
					Field = field.Value.Item1,
					OpCode = field.Key.Item1,
					Method = field.Key.Item2,
					OpKey = opKey,
					InitDesc = init
				});
			}
			foreach (TypeDef delegateType2 in ctx.Delegates.Values)
			{
				MethodDef cctor2 = delegateType2.FindOrCreateStaticConstructor();
				ctx.Marker.Mark(cctor2, ctx.Protection);
				ctx.Name.SetCanRename(cctor2, false);
			}
			ctx.Context.CurrentModuleWriterOptions.MetaDataOptions.Flags |= MetaDataFlags.PreserveExtraSignatureData;
			ctx.Context.CurrentModuleWriterListener.OnWriterEvent += this.EncodeField;
			this.encodeCtx = ctx;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000E860 File Offset: 0x0000CA60
		private void EncodeField(object sender, ModuleWriterListenerEventArgs e)
		{
			ModuleWriterBase writer = (ModuleWriterBase)sender;
			bool flag = e.WriterEvent == ModuleWriterEvent.MDMemberDefRidsAllocated && this.keyAttrs != null;
			if (flag)
			{
				Dictionary<TypeDef, Func<int, int>> keyFuncs = (from entry in this.keyAttrs
				where entry != null
				select entry).ToDictionary((Tuple<TypeDef, Func<int, int>> entry) => entry.Item1, (Tuple<TypeDef, Func<int, int>> entry) => entry.Item2);
				foreach (StrongMode.FieldDesc desc in this.fieldDescs)
				{
					uint token = writer.MetaData.GetToken(desc.Method).Raw;
					uint key = this.encodeCtx.Random.NextUInt32() | 1U;
					CustomAttribute ca = desc.Field.CustomAttributes[0];
					int encodedKey = keyFuncs[(TypeDef)ca.AttributeType]((int)MathsUtils.modInv(key));
					ca.ConstructorArguments.Add(new CAArgument(this.encodeCtx.Module.CorLibTypes.Int32, encodedKey));
					token *= key;
					token = (uint)desc.InitDesc.Encoding.Encode(desc.InitDesc.Method, this.encodeCtx, (int)token);
					char[] name = new char[5];
					name[desc.InitDesc.OpCodeIndex] = (char)((byte)desc.OpCode ^ desc.OpKey);
					byte[] nameKey = this.encodeCtx.Random.NextBytes(4);
					uint encodedNameKey = 0U;
					for (int i = 0; i < 4; i++)
					{
						while (nameKey[i] == 0)
						{
							nameKey[i] = this.encodeCtx.Random.NextByte();
						}
						name[desc.InitDesc.TokenNameOrder[i]] = (char)nameKey[i];
						encodedNameKey |= (uint)((uint)nameKey[i] << desc.InitDesc.TokenByteOrder[i]);
					}
					desc.Field.Name = new string(name);
					FieldSig sig = desc.Field.FieldSig;
					uint encodedToken = token - writer.MetaData.GetToken(((CModOptSig)sig.Type).Modifier).Raw ^ encodedNameKey;
					sig.ExtraData = new byte[]
					{
						192,
						0,
						0,
						(byte)(encodedToken >> desc.InitDesc.TokenByteOrder[3]),
						192,
						(byte)(encodedToken >> desc.InitDesc.TokenByteOrder[2]),
						(byte)(encodedToken >> desc.InitDesc.TokenByteOrder[1]),
						(byte)(encodedToken >> desc.InitDesc.TokenByteOrder[0])
					};
				}
			}
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00005250 File Offset: 0x00003450
		public StrongMode()
		{
		}

		// Token: 0x040000CC RID: 204
		private readonly List<StrongMode.FieldDesc> fieldDescs = new List<StrongMode.FieldDesc>();

		// Token: 0x040000CD RID: 205
		private readonly Dictionary<Tuple<Code, IMethod, IRPEncoding>, Tuple<FieldDef, MethodDef>> fields = new Dictionary<Tuple<Code, IMethod, IRPEncoding>, Tuple<FieldDef, MethodDef>>();

		// Token: 0x040000CE RID: 206
		private readonly Dictionary<IRPEncoding, StrongMode.InitMethodDesc[]> inits = new Dictionary<IRPEncoding, StrongMode.InitMethodDesc[]>();

		// Token: 0x040000CF RID: 207
		private RPContext encodeCtx;

		// Token: 0x040000D0 RID: 208
		private Tuple<TypeDef, Func<int, int>>[] keyAttrs;

		// Token: 0x02000062 RID: 98
		private class CodeGen : CILCodeGen
		{
			// Token: 0x060001AF RID: 431 RVA: 0x0000527A File Offset: 0x0000347A
			public CodeGen(Instruction[] arg, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.arg = arg;
			}

			// Token: 0x060001B0 RID: 432 RVA: 0x0000EBA4 File Offset: 0x0000CDA4
			protected override void LoadVar(Variable var)
			{
				bool flag = var.Name == "{RESULT}";
				if (flag)
				{
					foreach (Instruction instr in this.arg)
					{
						base.Emit(instr);
					}
				}
				else
				{
					base.LoadVar(var);
				}
			}

			// Token: 0x040000D1 RID: 209
			private readonly Instruction[] arg;
		}

		// Token: 0x02000063 RID: 99
		private class FieldDesc
		{
			// Token: 0x060001B1 RID: 433 RVA: 0x00004A68 File Offset: 0x00002C68
			public FieldDesc()
			{
			}

			// Token: 0x040000D2 RID: 210
			public FieldDef Field;

			// Token: 0x040000D3 RID: 211
			public StrongMode.InitMethodDesc InitDesc;

			// Token: 0x040000D4 RID: 212
			public IMethod Method;

			// Token: 0x040000D5 RID: 213
			public Code OpCode;

			// Token: 0x040000D6 RID: 214
			public byte OpKey;
		}

		// Token: 0x02000064 RID: 100
		private class InitMethodDesc
		{
			// Token: 0x060001B2 RID: 434 RVA: 0x00004A68 File Offset: 0x00002C68
			public InitMethodDesc()
			{
			}

			// Token: 0x040000D7 RID: 215
			public IRPEncoding Encoding;

			// Token: 0x040000D8 RID: 216
			public MethodDef Method;

			// Token: 0x040000D9 RID: 217
			public int OpCodeIndex;

			// Token: 0x040000DA RID: 218
			public int[] TokenByteOrder;

			// Token: 0x040000DB RID: 219
			public int[] TokenNameOrder;
		}

		// Token: 0x02000065 RID: 101
		[CompilerGenerated]
		private sealed class <>c__DisplayClass11_0
		{
			// Token: 0x060001B3 RID: 435 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass11_0()
			{
			}

			// Token: 0x060001B4 RID: 436 RVA: 0x0000EBF4 File Offset: 0x0000CDF4
			internal Instruction[] <GetKeyAttr>b__0(Instruction[] arg)
			{
				List<Instruction> invCompiled = new List<Instruction>();
				new StrongMode.CodeGen(arg, this.ctor, invCompiled).GenerateCIL(this.inverse);
				return invCompiled.ToArray();
			}

			// Token: 0x040000DC RID: 220
			public MethodDef ctor;

			// Token: 0x040000DD RID: 221
			public Expression inverse;
		}

		// Token: 0x02000066 RID: 102
		[CompilerGenerated]
		private sealed class <>c__DisplayClass12_0
		{
			// Token: 0x060001B5 RID: 437 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass12_0()
			{
			}

			// Token: 0x040000DE RID: 222
			public IRPEncoding encoding;

			// Token: 0x040000DF RID: 223
			public RPContext ctx;
		}

		// Token: 0x02000067 RID: 103
		[CompilerGenerated]
		private sealed class <>c__DisplayClass12_1
		{
			// Token: 0x060001B6 RID: 438 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass12_1()
			{
			}

			// Token: 0x060001B7 RID: 439 RVA: 0x0000EC2C File Offset: 0x0000CE2C
			internal Instruction[] <GetInitMethod>b__1(Instruction[] arg)
			{
				return this.CS$<>8__locals1.encoding.EmitDecode(this.injectedMethod, this.CS$<>8__locals1.ctx, arg);
			}

			// Token: 0x040000E0 RID: 224
			public MethodDef injectedMethod;

			// Token: 0x040000E1 RID: 225
			public StrongMode.<>c__DisplayClass12_0 CS$<>8__locals1;
		}

		// Token: 0x02000068 RID: 104
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060001B8 RID: 440 RVA: 0x0000528D File Offset: 0x0000348D
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060001B9 RID: 441 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x060001BA RID: 442 RVA: 0x00005299 File Offset: 0x00003499
			internal int <GetInitMethod>b__12_0(int x)
			{
				return x * 8;
			}

			// Token: 0x060001BB RID: 443 RVA: 0x0000529E File Offset: 0x0000349E
			internal bool <EncodeField>b__14_0(Tuple<TypeDef, Func<int, int>> entry)
			{
				return entry != null;
			}

			// Token: 0x060001BC RID: 444 RVA: 0x000052A4 File Offset: 0x000034A4
			internal TypeDef <EncodeField>b__14_1(Tuple<TypeDef, Func<int, int>> entry)
			{
				return entry.Item1;
			}

			// Token: 0x060001BD RID: 445 RVA: 0x000052AC File Offset: 0x000034AC
			internal Func<int, int> <EncodeField>b__14_2(Tuple<TypeDef, Func<int, int>> entry)
			{
				return entry.Item2;
			}

			// Token: 0x040000E2 RID: 226
			public static readonly StrongMode.<>c <>9 = new StrongMode.<>c();

			// Token: 0x040000E3 RID: 227
			public static Func<int, int> <>9__12_0;

			// Token: 0x040000E4 RID: 228
			public static Func<Tuple<TypeDef, Func<int, int>>, bool> <>9__14_0;

			// Token: 0x040000E5 RID: 229
			public static Func<Tuple<TypeDef, Func<int, int>>, TypeDef> <>9__14_1;

			// Token: 0x040000E6 RID: 230
			public static Func<Tuple<TypeDef, Func<int, int>>, Func<int, int>> <>9__14_2;
		}
	}
}
