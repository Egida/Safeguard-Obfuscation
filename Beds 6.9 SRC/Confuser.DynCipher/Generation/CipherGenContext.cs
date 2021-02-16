using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x0200000C RID: 12
	internal class CipherGenContext
	{
		// Token: 0x06000029 RID: 41 RVA: 0x00002F5C File Offset: 0x0000115C
		public CipherGenContext(RandomGenerator random, int dataVarCount)
		{
			this.random = random;
			this.Block = new StatementBlock();
			this.dataVars = new Variable[dataVarCount];
			for (int i = 0; i < dataVarCount; i++)
			{
				this.dataVars[i] = new Variable("v" + i)
				{
					Tag = i
				};
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002FE8 File Offset: 0x000011E8
		public IDisposable AcquireTempVar(out VariableExpression exp)
		{
			bool flag = this.tempVars.Count == 0;
			Variable var;
			if (flag)
			{
				object arg = "t";
				int num = this.tempVarCounter;
				this.tempVarCounter = num + 1;
				var = new Variable(arg + num);
			}
			else
			{
				var = this.tempVars[this.random.NextInt32(this.tempVars.Count)];
				this.tempVars.Remove(var);
			}
			exp = new VariableExpression
			{
				Variable = var
			};
			return new CipherGenContext.TempVarHolder(this, var);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003084 File Offset: 0x00001284
		public CipherGenContext Emit(Statement statement)
		{
			this.Block.Statements.Add(statement);
			return this;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000030AC File Offset: 0x000012AC
		public Expression GetDataExpression(int index)
		{
			return new VariableExpression
			{
				Variable = this.dataVars[index]
			};
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000030D4 File Offset: 0x000012D4
		public Expression GetKeyExpression(int index)
		{
			return new ArrayIndexExpression
			{
				Array = new VariableExpression
				{
					Variable = this.keyVar
				},
				Index = index
			};
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000310C File Offset: 0x0000130C
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00003114 File Offset: 0x00001314
		public StatementBlock Block
		{
			[CompilerGenerated]
			get
			{
				return this.<Block>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Block>k__BackingField = value;
			}
		}

		// Token: 0x04000004 RID: 4
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private StatementBlock <Block>k__BackingField;

		// Token: 0x04000005 RID: 5
		private readonly Variable[] dataVars;

		// Token: 0x04000006 RID: 6
		private readonly Variable keyVar = new Variable("{KEY}");

		// Token: 0x04000007 RID: 7
		private readonly RandomGenerator random;

		// Token: 0x04000008 RID: 8
		private int tempVarCounter;

		// Token: 0x04000009 RID: 9
		private readonly List<Variable> tempVars = new List<Variable>();

		// Token: 0x02000035 RID: 53
		private struct TempVarHolder : IDisposable
		{
			// Token: 0x0600011D RID: 285 RVA: 0x000075CD File Offset: 0x000057CD
			public TempVarHolder(CipherGenContext p, Variable v)
			{
				this.parent = p;
				this.tempVar = v;
			}

			// Token: 0x0600011E RID: 286 RVA: 0x000075DE File Offset: 0x000057DE
			public void Dispose()
			{
				this.parent.tempVars.Add(this.tempVar);
			}

			// Token: 0x04000090 RID: 144
			private readonly CipherGenContext parent;

			// Token: 0x04000091 RID: 145
			private readonly Variable tempVar;
		}
	}
}
