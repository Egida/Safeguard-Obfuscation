using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x020000E9 RID: 233
	internal class ExpressionPredicate : IPredicate
	{
		// Token: 0x06000395 RID: 917 RVA: 0x00005C5C File Offset: 0x00003E5C
		public ExpressionPredicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0001D824 File Offset: 0x0001BA24
		public void Init(CilBody body)
		{
			bool flag = this.inited;
			if (!flag)
			{
				this.stateVar = new Local(this.ctx.Method.Module.CorLibTypes.Int32);
				body.Variables.Add(this.stateVar);
				body.InitLocals = true;
				this.Compile(body);
				this.inited = true;
			}
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0001D88C File Offset: 0x0001BA8C
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Stloc, this.stateVar));
			foreach (Instruction instr in this.invCompiled)
			{
				instrs.Add(instr.Clone());
			}
		}

		// Token: 0x06000398 RID: 920 RVA: 0x0001D900 File Offset: 0x0001BB00
		public int GetSwitchKey(int key)
		{
			return this.expCompiled(key);
		}

		// Token: 0x06000399 RID: 921 RVA: 0x0001D920 File Offset: 0x0001BB20
		private void Compile(CilBody body)
		{
			Variable var = new Variable("{VAR}");
			Variable result = new Variable("{RESULT}");
			this.ctx.DynCipher.GenerateExpressionPair(this.ctx.Random, new VariableExpression
			{
				Variable = var
			}, new VariableExpression
			{
				Variable = result
			}, this.ctx.Depth, out this.expression, out this.inverse);
			this.expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{VAR}", typeof(int))
			}).GenerateCIL(this.expression).Compile<Func<int, int>>();
			this.invCompiled = new List<Instruction>();
			new ExpressionPredicate.CodeGen(this.stateVar, this.ctx, this.invCompiled).GenerateCIL(this.inverse);
			body.MaxStack += (ushort)this.ctx.Depth;
		}

		// Token: 0x040002AB RID: 683
		private readonly CFContext ctx;

		// Token: 0x040002AC RID: 684
		private Func<int, int> expCompiled;

		// Token: 0x040002AD RID: 685
		private Expression expression;

		// Token: 0x040002AE RID: 686
		private bool inited;

		// Token: 0x040002AF RID: 687
		private List<Instruction> invCompiled;

		// Token: 0x040002B0 RID: 688
		private Expression inverse;

		// Token: 0x040002B1 RID: 689
		private Local stateVar;

		// Token: 0x020000EA RID: 234
		private class CodeGen : CILCodeGen
		{
			// Token: 0x0600039A RID: 922 RVA: 0x00005C6D File Offset: 0x00003E6D
			public CodeGen(Local state, CFContext ctx, IList<Instruction> instrs) : base(ctx.Method, instrs)
			{
				this.state = state;
			}

			// Token: 0x0600039B RID: 923 RVA: 0x0001DA1C File Offset: 0x0001BC1C
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

			// Token: 0x040002B2 RID: 690
			private readonly Local state;
		}
	}
}
