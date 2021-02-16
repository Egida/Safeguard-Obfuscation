using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.CtrlFlowMod
{
	// Token: 0x020000BC RID: 188
	internal class ExpressionPredicate : IPredicate
	{
		// Token: 0x060002E5 RID: 741 RVA: 0x000057C1 File Offset: 0x000039C1
		public ExpressionPredicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00018750 File Offset: 0x00016950
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

		// Token: 0x060002E7 RID: 743 RVA: 0x00018850 File Offset: 0x00016A50
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Stloc, this.stateVar));
			foreach (Instruction instruction in this.invCompiled)
			{
				instrs.Add(instruction.Clone());
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x000057D2 File Offset: 0x000039D2
		public int GetSwitchKey(int key)
		{
			return this.expCompiled(key);
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x000188C8 File Offset: 0x00016AC8
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

		// Token: 0x04000222 RID: 546
		private readonly CFContext ctx;

		// Token: 0x04000223 RID: 547
		private Func<int, int> expCompiled;

		// Token: 0x04000224 RID: 548
		private Expression expression;

		// Token: 0x04000225 RID: 549
		private bool inited;

		// Token: 0x04000226 RID: 550
		private List<Instruction> invCompiled;

		// Token: 0x04000227 RID: 551
		private Expression inverse;

		// Token: 0x04000228 RID: 552
		private Local stateVar;

		// Token: 0x020000BD RID: 189
		private class CodeGen : CILCodeGen
		{
			// Token: 0x060002EA RID: 746 RVA: 0x000057E0 File Offset: 0x000039E0
			public CodeGen(Local state, CFContext ctx, IList<Instruction> instrs) : base(ctx.Method, instrs)
			{
				this.state = state;
			}

			// Token: 0x060002EB RID: 747 RVA: 0x00018934 File Offset: 0x00016B34
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

			// Token: 0x04000229 RID: 553
			private readonly Local state;
		}
	}
}
