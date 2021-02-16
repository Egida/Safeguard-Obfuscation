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

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x02000057 RID: 87
	internal class x86Encoding : IRPEncoding
	{
		// Token: 0x06000180 RID: 384 RVA: 0x0000CBE4 File Offset: 0x0000ADE4
		private void Compile(RPContext ctx, out Func<int, int> expCompiled, out MethodDef native)
		{
			Variable var = new Variable("{VAR}");
			Variable result = new Variable("{RESULT}");
			CorLibTypeSig int32 = ctx.Module.CorLibTypes.Int32;
			native = new MethodDefUser(ctx.Context.Registry.GetService<INameService>().RandomName(), MethodSig.CreateStatic(int32, int32), MethodAttributes.Static | MethodAttributes.PinvokeImpl);
			native.ImplAttributes = (MethodImplAttributes.Native | MethodImplAttributes.ManagedMask | MethodImplAttributes.PreserveSig);
			ctx.Module.GlobalType.Methods.Add(native);
			ctx.Context.Registry.GetService<IMarkerService>().Mark(native, ctx.Protection);
			ctx.Context.Registry.GetService<INameService>().SetCanRename(native, false);
			x86CodeGen codeGen = new x86CodeGen();
			Expression expression;
			x86Register? reg;
			do
			{
				Expression inverse;
				ctx.DynCipher.GenerateExpressionPair(ctx.Random, new VariableExpression
				{
					Variable = var
				}, new VariableExpression
				{
					Variable = result
				}, ctx.Depth, out expression, out inverse);
				reg = codeGen.GenerateX86(inverse, (Variable v, x86Register r) => new x86Instruction[]
				{
					x86Instruction.Create(x86OpCode.POP, new Ix86Operand[]
					{
						new x86RegisterOperand(r)
					})
				});
			}
			while (reg == null);
			byte[] code = CodeGenUtils.AssembleCode(codeGen, reg.Value);
			expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{VAR}", typeof(int))
			}).GenerateCIL(expression).Compile<Func<int, int>>();
			this.nativeCodes.Add(Tuple.Create<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>(native, code, null));
			bool flag = !this.addedHandler;
			if (flag)
			{
				ctx.Context.CurrentModuleWriterListener.OnWriterEvent += this.InjectNativeCode;
				this.addedHandler = true;
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000CDB0 File Offset: 0x0000AFB0
		public Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg)
		{
			Tuple<MethodDef, Func<int, int>> key = this.GetKey(ctx, init);
			List<Instruction> repl = new List<Instruction>();
			repl.AddRange(arg);
			repl.Add(Instruction.Create(OpCodes.Call, key.Item1));
			return repl.ToArray();
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000CDF8 File Offset: 0x0000AFF8
		public int Encode(MethodDef init, RPContext ctx, int value)
		{
			Tuple<MethodDef, Func<int, int>> key = this.GetKey(ctx, init);
			return key.Item2(value);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000CE20 File Offset: 0x0000B020
		private Tuple<MethodDef, Func<int, int>> GetKey(RPContext ctx, MethodDef init)
		{
			Tuple<MethodDef, Func<int, int>> ret;
			bool flag = !this.keys.TryGetValue(init, out ret);
			if (flag)
			{
				Func<int, int> keyFunc;
				MethodDef native;
				this.Compile(ctx, out keyFunc, out native);
				ret = (this.keys[init] = Tuple.Create<MethodDef, Func<int, int>>(native, keyFunc));
			}
			return ret;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000CE74 File Offset: 0x0000B074
		private void InjectNativeCode(object sender, ModuleWriterListenerEventArgs e)
		{
			ModuleWriterBase writer = (ModuleWriterBase)sender;
			bool flag = e.WriterEvent == ModuleWriterEvent.MDEndWriteMethodBodies;
			if (flag)
			{
				for (int i = 0; i < this.nativeCodes.Count; i++)
				{
					this.nativeCodes[i] = new Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>(this.nativeCodes[i].Item1, this.nativeCodes[i].Item2, writer.MethodBodies.Add(new dnlib.DotNet.Writer.MethodBody(this.nativeCodes[i].Item2)));
				}
			}
			else
			{
				bool flag2 = e.WriterEvent == ModuleWriterEvent.EndCalculateRvasAndFileOffsets;
				if (flag2)
				{
					foreach (Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody> native in this.nativeCodes)
					{
						uint rid = writer.MetaData.GetRid(native.Item1);
						writer.MetaData.TablesHeap.MethodTable[rid].RVA = (uint)native.Item3.RVA;
					}
				}
			}
		}

		// Token: 0x06000185 RID: 389 RVA: 0x000051A3 File Offset: 0x000033A3
		public x86Encoding()
		{
		}

		// Token: 0x040000BF RID: 191
		private bool addedHandler;

		// Token: 0x040000C0 RID: 192
		private readonly Dictionary<MethodDef, Tuple<MethodDef, Func<int, int>>> keys = new Dictionary<MethodDef, Tuple<MethodDef, Func<int, int>>>();

		// Token: 0x040000C1 RID: 193
		private readonly List<Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>> nativeCodes = new List<Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>>();

		// Token: 0x02000058 RID: 88
		private class CodeGen : CILCodeGen
		{
			// Token: 0x06000186 RID: 390 RVA: 0x000051C2 File Offset: 0x000033C2
			public CodeGen(Instruction[] arg, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.arg = arg;
			}

			// Token: 0x06000187 RID: 391 RVA: 0x0000CFA4 File Offset: 0x0000B1A4
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

			// Token: 0x040000C2 RID: 194
			private readonly Instruction[] arg;
		}

		// Token: 0x02000059 RID: 89
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000188 RID: 392 RVA: 0x000051D5 File Offset: 0x000033D5
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000189 RID: 393 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x0600018A RID: 394 RVA: 0x000051E1 File Offset: 0x000033E1
			internal IEnumerable<x86Instruction> <Compile>b__0_0(Variable v, x86Register r)
			{
				return new x86Instruction[]
				{
					x86Instruction.Create(x86OpCode.POP, new Ix86Operand[]
					{
						new x86RegisterOperand(r)
					})
				};
			}

			// Token: 0x040000C3 RID: 195
			public static readonly x86Encoding.<>c <>9 = new x86Encoding.<>c();

			// Token: 0x040000C4 RID: 196
			public static Func<Variable, x86Register, IEnumerable<x86Instruction>> <>9__0_0;
		}
	}
}
