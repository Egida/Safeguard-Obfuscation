using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.MildReferenceProxy
{
	// Token: 0x02000071 RID: 113
	internal class ExpressionEncoding : IRPEncoding
	{
		// Token: 0x060001D0 RID: 464 RVA: 0x0000F354 File Offset: 0x0000D554
		private void Compile(RPContext ctx, CilBody body, out Func<int, int> expCompiled, out Expression inverse)
		{
			Variable variable = new Variable("{VAR}");
			Variable variable2 = new Variable("{RESULT}");
			VariableExpression var = new VariableExpression
			{
				Variable = variable
			};
			VariableExpression result = new VariableExpression
			{
				Variable = variable2
			};
			Expression expression;
			ctx.DynCipher.GenerateExpressionPair(ctx.Random, var, result, ctx.Depth, out expression, out inverse);
			expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{VAR}", typeof(int))
			}).GenerateCIL(expression).Compile<Func<int, int>>();
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000F3F0 File Offset: 0x0000D5F0
		public Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg)
		{
			Tuple<Expression, Func<int, int>> key = this.GetKey(ctx, init);
			List<Instruction> instrs = new List<Instruction>();
			new ExpressionEncoding.CodeGen(arg, ctx.Method, instrs).GenerateCIL(key.Item1);
			CilBody body = init.Body;
			body.MaxStack += (ushort)ctx.Depth;
			return instrs.ToArray();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00005303 File Offset: 0x00003503
		public int Encode(MethodDef init, RPContext ctx, int value)
		{
			return this.GetKey(ctx, init).Item2(value);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000F450 File Offset: 0x0000D650
		private Tuple<Expression, Func<int, int>> GetKey(RPContext ctx, MethodDef init)
		{
			Tuple<Expression, Func<int, int>> tuple;
			bool flag = !this.keys.TryGetValue(init, out tuple);
			if (flag)
			{
				Func<int, int> func;
				Expression expression;
				this.Compile(ctx, init.Body, out func, out expression);
				tuple = (this.keys[init] = Tuple.Create<Expression, Func<int, int>>(expression, func));
			}
			return tuple;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00005318 File Offset: 0x00003518
		public ExpressionEncoding()
		{
		}

		// Token: 0x04000112 RID: 274
		private readonly Dictionary<MethodDef, Tuple<Expression, Func<int, int>>> keys = new Dictionary<MethodDef, Tuple<Expression, Func<int, int>>>();

		// Token: 0x02000072 RID: 114
		private class CodeGen : CILCodeGen
		{
			// Token: 0x060001D5 RID: 469 RVA: 0x0000532C File Offset: 0x0000352C
			public CodeGen(Instruction[] arg, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.arg = arg;
			}

			// Token: 0x060001D6 RID: 470 RVA: 0x0000F4A4 File Offset: 0x0000D6A4
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

			// Token: 0x04000113 RID: 275
			private readonly Instruction[] arg;
		}
	}
}
