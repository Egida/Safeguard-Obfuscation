using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x0200005A RID: 90
	internal class ExpressionEncoding : IRPEncoding
	{
		// Token: 0x0600018B RID: 395 RVA: 0x0000CFFC File Offset: 0x0000B1FC
		private void Compile(RPContext ctx, CilBody body, out Func<int, int> expCompiled, out Expression inverse)
		{
			Variable var = new Variable("{VAR}");
			Variable result = new Variable("{RESULT}");
			Expression expression;
			ctx.DynCipher.GenerateExpressionPair(ctx.Random, new VariableExpression
			{
				Variable = var
			}, new VariableExpression
			{
				Variable = result
			}, ctx.Depth, out expression, out inverse);
			expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{VAR}", typeof(int))
			}).GenerateCIL(expression).Compile<Func<int, int>>();
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000D090 File Offset: 0x0000B290
		public Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg)
		{
			Tuple<Expression, Func<int, int>> key = this.GetKey(ctx, init);
			List<Instruction> invCompiled = new List<Instruction>();
			new ExpressionEncoding.CodeGen(arg, ctx.Method, invCompiled).GenerateCIL(key.Item1);
			CilBody expr_2D = init.Body;
			CilBody cilBody = expr_2D;
			cilBody.MaxStack += (ushort)ctx.Depth;
			return invCompiled.ToArray();
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000D0F0 File Offset: 0x0000B2F0
		public int Encode(MethodDef init, RPContext ctx, int value)
		{
			Tuple<Expression, Func<int, int>> key = this.GetKey(ctx, init);
			return key.Item2(value);
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000D118 File Offset: 0x0000B318
		private Tuple<Expression, Func<int, int>> GetKey(RPContext ctx, MethodDef init)
		{
			Tuple<Expression, Func<int, int>> ret;
			bool flag = !this.keys.TryGetValue(init, out ret);
			if (flag)
			{
				Func<int, int> keyFunc;
				Expression inverse;
				this.Compile(ctx, init.Body, out keyFunc, out inverse);
				ret = (this.keys[init] = Tuple.Create<Expression, Func<int, int>>(inverse, keyFunc));
			}
			return ret;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00005201 File Offset: 0x00003401
		public ExpressionEncoding()
		{
		}

		// Token: 0x040000C5 RID: 197
		private readonly Dictionary<MethodDef, Tuple<Expression, Func<int, int>>> keys = new Dictionary<MethodDef, Tuple<Expression, Func<int, int>>>();

		// Token: 0x0200005B RID: 91
		private class CodeGen : CILCodeGen
		{
			// Token: 0x06000190 RID: 400 RVA: 0x00005215 File Offset: 0x00003415
			public CodeGen(Instruction[] arg, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.arg = arg;
			}

			// Token: 0x06000191 RID: 401 RVA: 0x0000D170 File Offset: 0x0000B370
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

			// Token: 0x040000C6 RID: 198
			private readonly Instruction[] arg;
		}
	}
}
