using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.MildReferenceProxy
{
	// Token: 0x02000085 RID: 133
	internal class x86Encoding : IRPEncoding
	{
		// Token: 0x06000213 RID: 531 RVA: 0x0001136C File Offset: 0x0000F56C
		private void Compile(RPContext ctx, out Func<int, int> expCompiled, out MethodDef native)
		{
			Variable variable = new Variable("{VAR}");
			Variable variable2 = new Variable("{RESULT}");
			CorLibTypeSig retType = ctx.Module.CorLibTypes.Int32;
			native = new MethodDefUser(ctx.Context.Registry.GetService<INameService>().RandomName(), MethodSig.CreateStatic(retType, retType), MethodAttributes.Static | MethodAttributes.PinvokeImpl);
			native.ImplAttributes = (MethodImplAttributes.Native | MethodImplAttributes.ManagedMask | MethodImplAttributes.PreserveSig);
			ctx.Module.GlobalType.Methods.Add(native);
			ctx.Context.Registry.GetService<IMarkerService>().Mark(native, ctx.Protection);
			ctx.Context.Registry.GetService<INameService>().SetCanRename(native, false);
			x86CodeGen codeGen = new x86CodeGen();
			Expression expression;
			x86Register? nullable;
			do
			{
				VariableExpression var = new VariableExpression
				{
					Variable = variable
				};
				VariableExpression result = new VariableExpression
				{
					Variable = variable2
				};
				Expression expression2;
				ctx.DynCipher.GenerateExpressionPair(ctx.Random, var, result, ctx.Depth, out expression, out expression2);
				nullable = codeGen.GenerateX86(expression2, (Variable v, x86Register r) => new x86Instruction[]
				{
					x86Instruction.Create(x86OpCode.POP, new Ix86Operand[]
					{
						new x86RegisterOperand(r)
					})
				});
			}
			while (nullable == null);
			byte[] buffer = CodeGenUtils.AssembleCode(codeGen, nullable.Value);
			expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{VAR}", typeof(int))
			}).GenerateCIL(expression).Compile<Func<int, int>>();
			this.nativeCodes.Add(Tuple.Create<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>(native, buffer, null));
			bool flag = !this.addedHandler;
			if (flag)
			{
				ctx.Context.CurrentModuleWriterListener.OnWriterEvent += this.InjectNativeCode;
				this.addedHandler = true;
			}
		}

		// Token: 0x06000214 RID: 532 RVA: 0x00011544 File Offset: 0x0000F744
		public Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg)
		{
			Tuple<MethodDef, Func<int, int>> key = this.GetKey(ctx, init);
			List<Instruction> list = new List<Instruction>();
			list.AddRange(arg);
			list.Add(Instruction.Create(OpCodes.Call, key.Item1));
			return list.ToArray();
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00005403 File Offset: 0x00003603
		public int Encode(MethodDef init, RPContext ctx, int value)
		{
			return this.GetKey(ctx, init).Item2(value);
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0001158C File Offset: 0x0000F78C
		private Tuple<MethodDef, Func<int, int>> GetKey(RPContext ctx, MethodDef init)
		{
			Tuple<MethodDef, Func<int, int>> tuple;
			bool flag = !this.keys.TryGetValue(init, out tuple);
			if (flag)
			{
				Func<int, int> func;
				MethodDef def;
				this.Compile(ctx, out func, out def);
				tuple = (this.keys[init] = Tuple.Create<MethodDef, Func<int, int>>(def, func));
			}
			return tuple;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x000115DC File Offset: 0x0000F7DC
		private void InjectNativeCode(object sender, ModuleWriterListenerEventArgs e)
		{
			ModuleWriterBase base2 = (ModuleWriterBase)sender;
			bool flag = e.WriterEvent == ModuleWriterEvent.MDEndWriteMethodBodies;
			if (flag)
			{
				for (int i = 0; i < this.nativeCodes.Count; i++)
				{
					this.nativeCodes[i] = new Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>(this.nativeCodes[i].Item1, this.nativeCodes[i].Item2, base2.MethodBodies.Add(new dnlib.DotNet.Writer.MethodBody(this.nativeCodes[i].Item2)));
				}
			}
			else
			{
				bool flag2 = e.WriterEvent == ModuleWriterEvent.EndCalculateRvasAndFileOffsets;
				if (flag2)
				{
					foreach (Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody> tuple in this.nativeCodes)
					{
						uint rid = base2.MetaData.GetRid(tuple.Item1);
						base2.MetaData.TablesHeap.MethodTable[rid].RVA = (uint)tuple.Item3.RVA;
					}
				}
			}
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00005418 File Offset: 0x00003618
		public x86Encoding()
		{
		}

		// Token: 0x04000155 RID: 341
		private bool addedHandler;

		// Token: 0x04000156 RID: 342
		private readonly Dictionary<MethodDef, Tuple<MethodDef, Func<int, int>>> keys = new Dictionary<MethodDef, Tuple<MethodDef, Func<int, int>>>();

		// Token: 0x04000157 RID: 343
		private readonly List<Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>> nativeCodes = new List<Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>>();

		// Token: 0x02000086 RID: 134
		private class CodeGen : CILCodeGen
		{
			// Token: 0x06000219 RID: 537 RVA: 0x00005437 File Offset: 0x00003637
			public CodeGen(Instruction[] arg, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.arg = arg;
			}

			// Token: 0x0600021A RID: 538 RVA: 0x0001170C File Offset: 0x0000F90C
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

			// Token: 0x04000158 RID: 344
			private readonly Instruction[] arg;
		}

		// Token: 0x02000087 RID: 135
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600021B RID: 539 RVA: 0x0000544A File Offset: 0x0000364A
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600021C RID: 540 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x0600021D RID: 541 RVA: 0x000051E1 File Offset: 0x000033E1
			internal IEnumerable<x86Instruction> <Compile>b__3_0(Variable v, x86Register r)
			{
				return new x86Instruction[]
				{
					x86Instruction.Create(x86OpCode.POP, new Ix86Operand[]
					{
						new x86RegisterOperand(r)
					})
				};
			}

			// Token: 0x04000159 RID: 345
			public static readonly x86Encoding.<>c <>9 = new x86Encoding.<>c();

			// Token: 0x0400015A RID: 346
			public static Func<Variable, x86Register, IEnumerable<x86Instruction>> <>9__3_0;
		}
	}
}
