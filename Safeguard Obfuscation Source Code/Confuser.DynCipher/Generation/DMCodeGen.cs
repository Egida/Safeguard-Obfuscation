using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x0200000F RID: 15
	public class DMCodeGen
	{
		// Token: 0x06000043 RID: 67 RVA: 0x00003A2C File Offset: 0x00001C2C
		public DMCodeGen(Type returnType, Tuple<string, Type>[] parameters)
		{
			this.dm = new DynamicMethod("", returnType, (from param in parameters
			select param.Item2).ToArray<Type>(), true);
			this.paramMap = new Dictionary<string, int>();
			for (int i = 0; i < parameters.Length; i++)
			{
				this.paramMap.Add(parameters[i].Item1, i);
			}
			this.ilGen = this.dm.GetILGenerator();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003AD0 File Offset: 0x00001CD0
		public T Compile<T>()
		{
			this.ilGen.Emit(OpCodes.Ret);
			return (T)((object)this.dm.CreateDelegate(typeof(T)));
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003B10 File Offset: 0x00001D10
		private void EmitLoad(Expression exp)
		{
			bool flag = exp is ArrayIndexExpression;
			if (flag)
			{
				ArrayIndexExpression arrIndex = (ArrayIndexExpression)exp;
				this.EmitLoad(arrIndex.Array);
				this.ilGen.Emit(OpCodes.Ldc_I4, arrIndex.Index);
				this.ilGen.Emit(OpCodes.Ldelem_U4);
			}
			else
			{
				bool flag2 = exp is BinOpExpression;
				if (flag2)
				{
					BinOpExpression binOp = (BinOpExpression)exp;
					this.EmitLoad(binOp.Left);
					this.EmitLoad(binOp.Right);
					OpCode op;
					switch (binOp.Operation)
					{
					case BinOps.Add:
						op = OpCodes.Add;
						break;
					case BinOps.Sub:
						op = OpCodes.Sub;
						break;
					case BinOps.Div:
						op = OpCodes.Div;
						break;
					case BinOps.Mul:
						op = OpCodes.Mul;
						break;
					case BinOps.Or:
						op = OpCodes.Or;
						break;
					case BinOps.And:
						op = OpCodes.And;
						break;
					case BinOps.Xor:
						op = OpCodes.Xor;
						break;
					case BinOps.Lsh:
						op = OpCodes.Shl;
						break;
					case BinOps.Rsh:
						op = OpCodes.Shr_Un;
						break;
					default:
						throw new NotSupportedException();
					}
					this.ilGen.Emit(op);
				}
				else
				{
					bool flag3 = exp is UnaryOpExpression;
					if (flag3)
					{
						UnaryOpExpression unaryOp = (UnaryOpExpression)exp;
						this.EmitLoad(unaryOp.Value);
						UnaryOps operation = unaryOp.Operation;
						OpCode op2;
						if (operation != UnaryOps.Not)
						{
							if (operation != UnaryOps.Negate)
							{
								throw new NotSupportedException();
							}
							op2 = OpCodes.Neg;
						}
						else
						{
							op2 = OpCodes.Not;
						}
						this.ilGen.Emit(op2);
					}
					else
					{
						bool flag4 = exp is LiteralExpression;
						if (flag4)
						{
							LiteralExpression literal = (LiteralExpression)exp;
							this.ilGen.Emit(OpCodes.Ldc_I4, (int)literal.Value);
						}
						else
						{
							bool flag5 = exp is VariableExpression;
							if (!flag5)
							{
								throw new NotSupportedException();
							}
							VariableExpression var = (VariableExpression)exp;
							this.LoadVar(var.Variable);
						}
					}
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003D04 File Offset: 0x00001F04
		private void EmitStatement(Statement statement)
		{
			bool flag = statement is AssignmentStatement;
			if (flag)
			{
				AssignmentStatement assignment = (AssignmentStatement)statement;
				this.EmitStore(assignment.Target, assignment.Value);
			}
			else
			{
				bool flag2 = statement is LoopStatement;
				if (!flag2)
				{
					bool flag3 = statement is StatementBlock;
					if (flag3)
					{
						using (IEnumerator<Statement> enumerator2 = ((StatementBlock)statement).Statements.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Statement child2 = enumerator2.Current;
								this.EmitStatement(child2);
							}
							return;
						}
					}
					throw new NotSupportedException();
				}
				LoopStatement loop = (LoopStatement)statement;
				Label lbl = this.ilGen.DefineLabel();
				Label dup = this.ilGen.DefineLabel();
				this.ilGen.Emit(OpCodes.Ldc_I4, loop.Begin);
				this.ilGen.Emit(OpCodes.Br, dup);
				this.ilGen.Emit(OpCodes.Ldc_I4, loop.Begin);
				this.ilGen.MarkLabel(lbl);
				foreach (Statement child3 in loop.Statements)
				{
					this.EmitStatement(child3);
				}
				this.ilGen.Emit(OpCodes.Ldc_I4_1);
				this.ilGen.Emit(OpCodes.Add);
				this.ilGen.MarkLabel(dup);
				this.ilGen.Emit(OpCodes.Dup);
				this.ilGen.Emit(OpCodes.Ldc_I4, loop.Limit);
				this.ilGen.Emit(OpCodes.Blt, lbl);
				this.ilGen.Emit(OpCodes.Pop);
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003EF4 File Offset: 0x000020F4
		private void EmitStore(Expression exp, Expression value)
		{
			bool flag = exp is ArrayIndexExpression;
			if (flag)
			{
				ArrayIndexExpression arrIndex = (ArrayIndexExpression)exp;
				this.EmitLoad(arrIndex.Array);
				this.ilGen.Emit(OpCodes.Ldc_I4, arrIndex.Index);
				this.EmitLoad(value);
				this.ilGen.Emit(OpCodes.Stelem_I4);
			}
			else
			{
				bool flag2 = exp is VariableExpression;
				if (!flag2)
				{
					throw new NotSupportedException();
				}
				VariableExpression var = (VariableExpression)exp;
				this.EmitLoad(value);
				this.StoreVar(var.Variable);
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003F88 File Offset: 0x00002188
		public DMCodeGen GenerateCIL(Expression expression)
		{
			this.EmitLoad(expression);
			return this;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003FA4 File Offset: 0x000021A4
		public DMCodeGen GenerateCIL(Statement statement)
		{
			this.EmitStatement(statement);
			return this;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003FC0 File Offset: 0x000021C0
		protected virtual void LoadVar(Variable var)
		{
			bool flag = this.paramMap.ContainsKey(var.Name);
			if (flag)
			{
				this.ilGen.Emit(OpCodes.Ldarg, this.paramMap[var.Name]);
			}
			else
			{
				this.ilGen.Emit(OpCodes.Ldloc, this.Var(var));
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004020 File Offset: 0x00002220
		protected virtual void StoreVar(Variable var)
		{
			bool flag = this.paramMap.ContainsKey(var.Name);
			if (flag)
			{
				this.ilGen.Emit(OpCodes.Starg, this.paramMap[var.Name]);
			}
			else
			{
				this.ilGen.Emit(OpCodes.Stloc, this.Var(var));
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004080 File Offset: 0x00002280
		protected virtual LocalBuilder Var(Variable var)
		{
			LocalBuilder ret;
			bool flag = !this.localMap.TryGetValue(var.Name, out ret);
			if (flag)
			{
				ret = this.ilGen.DeclareLocal(typeof(int));
				this.localMap[var.Name] = ret;
			}
			return ret;
		}

		// Token: 0x04000014 RID: 20
		private readonly DynamicMethod dm;

		// Token: 0x04000015 RID: 21
		private readonly ILGenerator ilGen;

		// Token: 0x04000016 RID: 22
		private readonly Dictionary<string, LocalBuilder> localMap = new Dictionary<string, LocalBuilder>();

		// Token: 0x04000017 RID: 23
		private readonly Dictionary<string, int> paramMap;

		// Token: 0x02000036 RID: 54
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600011F RID: 287 RVA: 0x000075F8 File Offset: 0x000057F8
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000120 RID: 288 RVA: 0x000020FC File Offset: 0x000002FC
			public <>c()
			{
			}

			// Token: 0x06000121 RID: 289 RVA: 0x00007604 File Offset: 0x00005804
			internal Type <.ctor>b__0_0(Tuple<string, Type> param)
			{
				return param.Item2;
			}

			// Token: 0x04000092 RID: 146
			public static readonly DMCodeGen.<>c <>9 = new DMCodeGen.<>c();

			// Token: 0x04000093 RID: 147
			public static Func<Tuple<string, Type>, Type> <>9__0_0;
		}
	}
}
