using System;
using System.Collections.Generic;
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

namespace Confuser.Protections.MildReferenceProxy
{
	// Token: 0x0200007E RID: 126
	internal class StrongMode : RPMode
	{
		// Token: 0x060001FB RID: 507 RVA: 0x000103D8 File Offset: 0x0000E5D8
		private MethodDef CreateBridge(RPContext ctx, TypeDef delegateType, FieldDef field, MethodSig sig)
		{
			MethodDefUser item = new MethodDefUser(ctx.Name.RandomName(), sig)
			{
				Attributes = MethodAttributes.Static,
				ImplAttributes = MethodImplAttributes.IL,
				Body = new CilBody()
			};
			item.Body.Instructions.Add(Instruction.Create(OpCodes.Ldsfld, field));
			for (int i = 0; i < item.Parameters.Count; i++)
			{
				item.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, item.Parameters[i]));
			}
			item.Body.Instructions.Add(Instruction.Create(OpCodes.Call, delegateType.FindMethod("Invoke")));
			item.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			delegateType.Methods.Add(item);
			ctx.Context.Registry.GetService<IMarkerService>().Mark(item, ctx.Protection);
			ctx.Name.SetCanRename(item, false);
			return item;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00010500 File Offset: 0x0000E700
		private FieldDef CreateField(RPContext ctx, TypeDef delegateType)
		{
			TypeDef def;
			do
			{
				def = ctx.Module.Types[ctx.Random.NextInt32(ctx.Module.Types.Count)];
			}
			while (def.HasGenericParameters || def.IsGlobalModuleType || def.IsDelegate());
			TypeSig type = new CModOptSig(def, delegateType.ToTypeSig());
			FieldDefUser item = new FieldDefUser("", new FieldSig(type), FieldAttributes.Private | FieldAttributes.FamANDAssem | FieldAttributes.Static);
			item.CustomAttributes.Add(new CustomAttribute(this.GetKeyAttr(ctx).FindInstanceConstructors().First<MethodDef>()));
			delegateType.Fields.Add(item);
			ctx.Marker.Mark(item, ctx.Protection);
			ctx.Name.SetCanRename(item, false);
			return item;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x000105D8 File Offset: 0x0000E7D8
		private void EncodeField(object sender, ModuleWriterListenerEventArgs e)
		{
			ModuleWriterBase base2 = (ModuleWriterBase)sender;
			bool flag = e.WriterEvent == ModuleWriterEvent.MDMemberDefRidsAllocated;
			if (flag)
			{
				Dictionary<TypeDef, Func<int, int>> dictionary = (from entry in this.keyAttrs
				where entry != null
				select entry).ToDictionary((Tuple<TypeDef, Func<int, int>> entry) => entry.Item1, (Tuple<TypeDef, Func<int, int>> entry) => entry.Item2);
				foreach (StrongMode.FieldDesc desc in this.fieldDescs)
				{
					uint raw = base2.MetaData.GetToken(desc.Method).Raw;
					uint num = this.encodeCtx.Random.NextUInt32() | 1U;
					CustomAttribute attribute = desc.Field.CustomAttributes[0];
					int num2 = dictionary[(TypeDef)attribute.AttributeType]((int)MathsUtils.modInv(num));
					attribute.ConstructorArguments.Add(new CAArgument(this.encodeCtx.Module.CorLibTypes.Int32, num2));
					raw *= num;
					raw = (uint)desc.InitDesc.Encoding.Encode(desc.InitDesc.Method, this.encodeCtx, (int)raw);
					char[] chArray = new char[5];
					chArray[desc.InitDesc.OpCodeIndex] = (char)((byte)desc.OpCode ^ desc.OpKey);
					byte[] buffer = this.encodeCtx.Random.NextBytes(4);
					uint num3 = 0U;
					int index = 0;
					for (;;)
					{
						bool flag2 = index < 4;
						if (!flag2)
						{
							break;
						}
						for (;;)
						{
							bool flag3 = buffer[index] == 0;
							if (!flag3)
							{
								break;
							}
							buffer[index] = this.encodeCtx.Random.NextByte();
						}
						chArray[desc.InitDesc.TokenNameOrder[index]] = (char)buffer[index];
						num3 |= (uint)((uint)buffer[index] << desc.InitDesc.TokenByteOrder[index]);
						index++;
					}
					desc.Field.Name = new string(chArray);
					FieldSig fieldSig = desc.Field.FieldSig;
					uint num4 = raw - base2.MetaData.GetToken(((CModOptSig)fieldSig.Type).Modifier).Raw ^ num3;
					fieldSig.ExtraData = new byte[]
					{
						192,
						0,
						0,
						(byte)(num4 >> desc.InitDesc.TokenByteOrder[3]),
						192,
						(byte)(num4 >> desc.InitDesc.TokenByteOrder[2]),
						(byte)(num4 >> desc.InitDesc.TokenByteOrder[1]),
						(byte)(num4 >> desc.InitDesc.TokenByteOrder[0])
					};
				}
			}
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00010918 File Offset: 0x0000EB18
		public override void Finalize(RPContext ctx)
		{
			foreach (KeyValuePair<Tuple<Code, IMethod, IRPEncoding>, Tuple<FieldDef, MethodDef>> pair in this.fields)
			{
				StrongMode.InitMethodDesc initMethod = this.GetInitMethod(ctx, pair.Key.Item3);
				byte num;
				do
				{
					num = ctx.Random.NextByte();
				}
				while (num == (byte)pair.Key.Item1);
				MethodDef def2 = pair.Value.Item1.DeclaringType.FindOrCreateStaticConstructor();
				def2.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, initMethod.Method));
				def2.Body.Instructions.Insert(0, Instruction.CreateLdcI4((int)num));
				def2.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldtoken, pair.Value.Item1));
				StrongMode.FieldDesc item = new StrongMode.FieldDesc
				{
					Field = pair.Value.Item1,
					OpCode = pair.Key.Item1,
					Method = pair.Key.Item2,
					OpKey = num,
					InitDesc = initMethod
				};
				this.fieldDescs.Add(item);
			}
			foreach (TypeDef def3 in ctx.Delegates.Values)
			{
				MethodDef member = def3.FindOrCreateStaticConstructor();
				ctx.Marker.Mark(member, ctx.Protection);
				ctx.Name.SetCanRename(member, false);
			}
			MetaDataOptions metaDataOptions = ctx.Context.CurrentModuleWriterOptions.MetaDataOptions;
			metaDataOptions.Flags |= MetaDataFlags.PreserveExtraSignatureData;
			ctx.Context.CurrentModuleWriterListener.OnWriterEvent += this.EncodeField;
			this.encodeCtx = ctx;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00010B54 File Offset: 0x0000ED54
		private StrongMode.InitMethodDesc GetInitMethod(RPContext ctx, IRPEncoding encoding)
		{
			StrongMode.InitMethodDesc[] descArray;
			bool flag = !this.inits.TryGetValue(encoding, out descArray);
			if (flag)
			{
				descArray = (this.inits[encoding] = new StrongMode.InitMethodDesc[ctx.InitCount]);
			}
			int index = ctx.Random.NextInt32(descArray.Length);
			bool flag2 = descArray[index] == null;
			if (flag2)
			{
				TypeDef runtimeType = ctx.Context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.RefProxyStrong");
				MethodDef injectedMethod = InjectHelper.Inject(runtimeType.FindMethod("Initialize"), ctx.Module);
				ctx.Module.GlobalType.Methods.Add(injectedMethod);
				injectedMethod.Access = MethodAttributes.PrivateScope;
				injectedMethod.Name = ctx.Name.RandomName();
				ctx.Name.SetCanRename(injectedMethod, false);
				ctx.Marker.Mark(injectedMethod, ctx.Protection);
				StrongMode.InitMethodDesc desc = new StrongMode.InitMethodDesc
				{
					Method = injectedMethod
				};
				int[] list = Enumerable.Range(0, 5).ToArray<int>();
				ctx.Random.Shuffle<int>(list);
				desc.OpCodeIndex = list[4];
				desc.TokenNameOrder = new int[4];
				Array.Copy(list, 0, desc.TokenNameOrder, 0, 4);
				desc.TokenByteOrder = (from x in Enumerable.Range(0, 4)
				select x * 8).ToArray<int>();
				ctx.Random.Shuffle<int>(desc.TokenByteOrder);
				int[] destinationArray = new int[9];
				Array.Copy(desc.TokenNameOrder, 0, destinationArray, 0, 4);
				Array.Copy(desc.TokenByteOrder, 0, destinationArray, 4, 4);
				destinationArray[8] = desc.OpCodeIndex;
				MutationHelper.InjectKeys(injectedMethod, Enumerable.Range(0, 9).ToArray<int>(), destinationArray);
				MutationHelper.ReplacePlaceholder(injectedMethod, (Instruction[] arg) => encoding.EmitDecode(injectedMethod, ctx, arg));
				desc.Encoding = encoding;
				descArray[index] = desc;
			}
			return descArray[index];
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00010E34 File Offset: 0x0000F034
		private TypeDef GetKeyAttr(RPContext ctx)
		{
			bool flag = this.keyAttrs == null;
			if (flag)
			{
				this.keyAttrs = new Tuple<TypeDef, Func<int, int>>[16];
			}
			int index = ctx.Random.NextInt32(this.keyAttrs.Length);
			Tuple<TypeDef, Func<int, int>> tuple = this.keyAttrs[index];
			return (tuple != null) ? tuple.Item1 : null;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00010E8C File Offset: 0x0000F08C
		private void ProcessBridge(RPContext ctx, int instrIndex)
		{
			Instruction instruction = ctx.Body.Instructions[instrIndex];
			IMethod operand = (IMethod)instruction.Operand;
			TypeDef def = operand.DeclaringType.ResolveTypeDefThrow();
			bool flag = def.Module.IsILOnly && !def.IsGlobalModuleType;
			if (flag)
			{
				Tuple<Code, IMethod, IRPEncoding> key = Tuple.Create<Code, IMethod, IRPEncoding>(instruction.OpCode.Code, operand, ctx.EncodingHandler);
				Tuple<FieldDef, MethodDef> tuple2;
				bool flag2 = this.fields.TryGetValue(key, out tuple2);
				if (flag2)
				{
					bool flag3 = tuple2.Item2 != null;
					if (flag3)
					{
						instruction.OpCode = OpCodes.Call;
						instruction.Operand = tuple2.Item2;
						return;
					}
				}
				else
				{
					tuple2 = new Tuple<FieldDef, MethodDef>(null, null);
				}
				MethodSig sig = RPMode.CreateProxySignature(ctx, operand, instruction.OpCode.Code == Code.Newobj);
				TypeDef delegateType = RPMode.GetDelegateType(ctx, sig);
				bool flag4 = tuple2.Item1 == null;
				if (flag4)
				{
					tuple2 = new Tuple<FieldDef, MethodDef>(this.CreateField(ctx, delegateType), tuple2.Item2);
				}
				tuple2 = new Tuple<FieldDef, MethodDef>(tuple2.Item1, this.CreateBridge(ctx, delegateType, tuple2.Item1, sig));
				this.fields[key] = tuple2;
				instruction.OpCode = OpCodes.Call;
				instruction.Operand = tuple2.Item2;
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00010FE4 File Offset: 0x0000F1E4
		public override void ProcessCall(RPContext ctx, int instrIndex)
		{
			Instruction instruction = ctx.Body.Instructions[instrIndex];
			TypeDef def = ((IMethod)instruction.Operand).DeclaringType.ResolveTypeDefThrow();
			bool flag = def.Module.IsILOnly && !def.IsGlobalModuleType;
			if (flag)
			{
				int num;
				int num2;
				instruction.CalculateStackUsage(out num, out num2);
				int? nullable = StrongMode.TraceBeginning(ctx, instrIndex, num2);
				bool flag2 = nullable == null;
				if (flag2)
				{
					this.ProcessBridge(ctx, instrIndex);
				}
				else
				{
					this.ProcessInvoke(ctx, instrIndex, nullable.Value);
				}
			}
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00011080 File Offset: 0x0000F280
		private void ProcessInvoke(RPContext ctx, int instrIndex, int argBeginIndex)
		{
			Instruction instruction = ctx.Body.Instructions[instrIndex];
			IMethod operand = (IMethod)instruction.Operand;
			MethodSig sig = RPMode.CreateProxySignature(ctx, operand, instruction.OpCode.Code == Code.Newobj);
			TypeDef delegateType = RPMode.GetDelegateType(ctx, sig);
			Tuple<Code, IMethod, IRPEncoding> key = Tuple.Create<Code, IMethod, IRPEncoding>(instruction.OpCode.Code, operand, ctx.EncodingHandler);
			Tuple<FieldDef, MethodDef> tuple2;
			bool flag = !this.fields.TryGetValue(key, out tuple2);
			if (flag)
			{
				tuple2 = new Tuple<FieldDef, MethodDef>(this.CreateField(ctx, delegateType), null);
				this.fields[key] = tuple2;
			}
			bool flag2 = argBeginIndex == instrIndex;
			if (flag2)
			{
				ctx.Body.Instructions.Insert(instrIndex + 1, new Instruction(OpCodes.Call, delegateType.FindMethod("Invoke")));
				instruction.OpCode = OpCodes.Ldsfld;
				instruction.Operand = tuple2.Item1;
			}
			else
			{
				Instruction instruction2 = ctx.Body.Instructions[argBeginIndex];
				ctx.Body.Instructions.Insert(argBeginIndex + 1, new Instruction(instruction2.OpCode, instruction2.Operand));
				instruction2.OpCode = OpCodes.Ldsfld;
				instruction2.Operand = tuple2.Item1;
				instruction.OpCode = OpCodes.Call;
				instruction.Operand = delegateType.FindMethod("Invoke");
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x000111EC File Offset: 0x0000F3EC
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
				int num = argCount;
				int num2 = index;
				while (num > 0)
				{
					num2--;
					Instruction item = ctx.Body.Instructions[num2];
					bool flag2 = item.OpCode != OpCodes.Pop && item.OpCode != OpCodes.Dup;
					if (flag2)
					{
						FlowControl flowControl = item.OpCode.FlowControl;
						if (flowControl - FlowControl.Break <= 1 || flowControl - FlowControl.Meta <= 1)
						{
							int num3;
							int num4;
							item.CalculateStackUsage(out num3, out num4);
							num += num4;
							num -= num3;
							bool flag3 = !ctx.BranchTargets.Contains(item) || num == 0;
							if (flag3)
							{
								continue;
							}
							return null;
						}
					}
					return null;
				}
				bool flag4 = num < 0;
				if (flag4)
				{
					result = null;
				}
				else
				{
					result = new int?(num2);
				}
			}
			return result;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00005396 File Offset: 0x00003596
		public StrongMode()
		{
		}

		// Token: 0x0400013C RID: 316
		private RPContext encodeCtx;

		// Token: 0x0400013D RID: 317
		private readonly List<StrongMode.FieldDesc> fieldDescs = new List<StrongMode.FieldDesc>();

		// Token: 0x0400013E RID: 318
		private readonly Dictionary<Tuple<Code, IMethod, IRPEncoding>, Tuple<FieldDef, MethodDef>> fields = new Dictionary<Tuple<Code, IMethod, IRPEncoding>, Tuple<FieldDef, MethodDef>>();

		// Token: 0x0400013F RID: 319
		private readonly Dictionary<IRPEncoding, StrongMode.InitMethodDesc[]> inits = new Dictionary<IRPEncoding, StrongMode.InitMethodDesc[]>();

		// Token: 0x04000140 RID: 320
		private Tuple<TypeDef, Func<int, int>>[] keyAttrs;

		// Token: 0x0200007F RID: 127
		private class CodeGen : CILCodeGen
		{
			// Token: 0x06000206 RID: 518 RVA: 0x000053C0 File Offset: 0x000035C0
			public CodeGen(Instruction[] arg, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.arg = arg;
			}

			// Token: 0x06000207 RID: 519 RVA: 0x00011318 File Offset: 0x0000F518
			protected override void LoadVar(Variable var)
			{
				bool flag = var.Name == "{RESULT}";
				if (flag)
				{
					foreach (Instruction instruction in this.arg)
					{
						base.Emit(instruction);
					}
				}
				else
				{
					base.LoadVar(var);
				}
			}

			// Token: 0x04000141 RID: 321
			private readonly Instruction[] arg;
		}

		// Token: 0x02000080 RID: 128
		private class FieldDesc
		{
			// Token: 0x06000208 RID: 520 RVA: 0x00004A68 File Offset: 0x00002C68
			public FieldDesc()
			{
			}

			// Token: 0x04000142 RID: 322
			public FieldDef Field;

			// Token: 0x04000143 RID: 323
			public StrongMode.InitMethodDesc InitDesc;

			// Token: 0x04000144 RID: 324
			public IMethod Method;

			// Token: 0x04000145 RID: 325
			public Code OpCode;

			// Token: 0x04000146 RID: 326
			public byte OpKey;
		}

		// Token: 0x02000081 RID: 129
		private class InitMethodDesc
		{
			// Token: 0x06000209 RID: 521 RVA: 0x00004A68 File Offset: 0x00002C68
			public InitMethodDesc()
			{
			}

			// Token: 0x04000147 RID: 327
			public IRPEncoding Encoding;

			// Token: 0x04000148 RID: 328
			public MethodDef Method;

			// Token: 0x04000149 RID: 329
			public int OpCodeIndex;

			// Token: 0x0400014A RID: 330
			public int[] TokenByteOrder;

			// Token: 0x0400014B RID: 331
			public int[] TokenNameOrder;
		}

		// Token: 0x02000082 RID: 130
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600020A RID: 522 RVA: 0x000053D3 File Offset: 0x000035D3
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600020B RID: 523 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x0600020C RID: 524 RVA: 0x0000529E File Offset: 0x0000349E
			internal bool <EncodeField>b__7_0(Tuple<TypeDef, Func<int, int>> entry)
			{
				return entry != null;
			}

			// Token: 0x0600020D RID: 525 RVA: 0x000052A4 File Offset: 0x000034A4
			internal TypeDef <EncodeField>b__7_1(Tuple<TypeDef, Func<int, int>> entry)
			{
				return entry.Item1;
			}

			// Token: 0x0600020E RID: 526 RVA: 0x000052AC File Offset: 0x000034AC
			internal Func<int, int> <EncodeField>b__7_2(Tuple<TypeDef, Func<int, int>> entry)
			{
				return entry.Item2;
			}

			// Token: 0x0600020F RID: 527 RVA: 0x00005299 File Offset: 0x00003499
			internal int <GetInitMethod>b__9_0(int x)
			{
				return x * 8;
			}

			// Token: 0x0400014C RID: 332
			public static readonly StrongMode.<>c <>9 = new StrongMode.<>c();

			// Token: 0x0400014D RID: 333
			public static Func<Tuple<TypeDef, Func<int, int>>, bool> <>9__7_0;

			// Token: 0x0400014E RID: 334
			public static Func<Tuple<TypeDef, Func<int, int>>, TypeDef> <>9__7_1;

			// Token: 0x0400014F RID: 335
			public static Func<Tuple<TypeDef, Func<int, int>>, Func<int, int>> <>9__7_2;

			// Token: 0x04000150 RID: 336
			public static Func<int, int> <>9__9_0;
		}

		// Token: 0x02000083 RID: 131
		[CompilerGenerated]
		private sealed class <>c__DisplayClass9_0
		{
			// Token: 0x06000210 RID: 528 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass9_0()
			{
			}

			// Token: 0x04000151 RID: 337
			public IRPEncoding encoding;

			// Token: 0x04000152 RID: 338
			public RPContext ctx;
		}

		// Token: 0x02000084 RID: 132
		[CompilerGenerated]
		private sealed class <>c__DisplayClass9_1
		{
			// Token: 0x06000211 RID: 529 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass9_1()
			{
			}

			// Token: 0x06000212 RID: 530 RVA: 0x000053DF File Offset: 0x000035DF
			internal Instruction[] <GetInitMethod>b__1(Instruction[] arg)
			{
				return this.CS$<>8__locals1.encoding.EmitDecode(this.injectedMethod, this.CS$<>8__locals1.ctx, arg);
			}

			// Token: 0x04000153 RID: 339
			public MethodDef injectedMethod;

			// Token: 0x04000154 RID: 340
			public StrongMode.<>c__DisplayClass9_0 CS$<>8__locals1;
		}
	}
}
