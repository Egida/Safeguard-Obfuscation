using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.DupCtrlFlowMod
{
	// Token: 0x020000D1 RID: 209
	internal class ExpressionPredicate : IPredicate
	{
		// Token: 0x06000336 RID: 822 RVA: 0x000059DA File Offset: 0x00003BDA
		public ExpressionPredicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0001A9E4 File Offset: 0x00018BE4
		private void Compile(CilBody body)
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
			this.ctx.DynCipher.GenerateExpressionPair(this.ctx.Random, var, result, this.ctx.Depth, out this.expression, out this.inverse);
			this.expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{VAR}", typeof(int))
			}).GenerateCIL(this.expression).Compile<Func<int, int>>();
			this.invCompiled = new List<Instruction>();
			new ExpressionPredicate.CodeGen(this.stateVar, this.ctx, this.invCompiled).GenerateCIL(this.inverse);
			body.MaxStack += (ushort)this.ctx.Depth;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0001AAE4 File Offset: 0x00018CE4
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Stloc, this.stateVar));
			foreach (Instruction instruction in this.invCompiled)
			{
				instrs.Add(instruction.Clone());
			}
		}

		// Token: 0x06000339 RID: 825 RVA: 0x000059EB File Offset: 0x00003BEB
		public int GetSwitchKey(int key)
		{
			return this.expCompiled(key);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0001AB5C File Offset: 0x00018D5C
		public void Init(CilBody body)
		{
			bool flag = !this.inited;
			if (flag)
			{
				this.stateVar = new Local(this.ctx.Method.Module.CorLibTypes.Int32);
				body.Variables.Add(this.stateVar);
				body.InitLocals = true;
				this.Compile(body);
				this.inited = true;
			}
		}

		// Token: 0x04000263 RID: 611
		private readonly CFContext ctx;

		// Token: 0x04000264 RID: 612
		private Func<int, int> expCompiled;

		// Token: 0x04000265 RID: 613
		private Expression expression;

		// Token: 0x04000266 RID: 614
		private bool inited;

		// Token: 0x04000267 RID: 615
		private List<Instruction> invCompiled;

		// Token: 0x04000268 RID: 616
		private Expression inverse;

		// Token: 0x04000269 RID: 617
		private Local stateVar;

		// Token: 0x020000D2 RID: 210
		private class CodeGen : CILCodeGen
		{
			// Token: 0x0600033B RID: 827 RVA: 0x000059F9 File Offset: 0x00003BF9
			public CodeGen(Local state, CFContext ctx, IList<Instruction> instrs) : base(ctx.Method, instrs)
			{
				this.state = state;
			}

			// Token: 0x0600033C RID: 828 RVA: 0x0001ABC8 File Offset: 0x00018DC8
			protected override Local Var(Variable var)
			{
				bool flag = var.Name == "{RESULT}";
				Local result;
				if (flag)
				{
					result = this.state;
				}
				else
				{
					result = base.Var(var);
				}
				return result;
			}

			// Token: 0x0400026A RID: 618
			private readonly Local state;
		}
	}
}
