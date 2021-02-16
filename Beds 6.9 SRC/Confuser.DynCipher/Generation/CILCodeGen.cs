using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Confuser.DynCipher.AST;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x0200000E RID: 14
	public class CILCodeGen
	{
		// Token: 0x06000034 RID: 52 RVA: 0x0000347B File Offset: 0x0000167B
		public CILCodeGen(MethodDef method, IList<Instruction> instrs)
		{
			this.Method = method;
			this.Instructions = instrs;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000034A0 File Offset: 0x000016A0
		public void Commit(CilBody body)
		{
			foreach (Local i in this.localMap.Values)
			{
				body.InitLocals = true;
				body.Variables.Add(i);
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000350C File Offset: 0x0000170C
		protected void Emit(Instruction instr)
		{
			this.Instructions.Add(instr);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000351C File Offset: 0x0000171C
		private void EmitLoad(Expression exp)
		{
			bool flag = exp is ArrayIndexExpression;
			if (flag)
			{
				ArrayIndexExpression arrIndex = (ArrayIndexExpression)exp;
				this.EmitLoad(arrIndex.Array);
				this.Emit(Instruction.CreateLdcI4(arrIndex.Index));
				this.Emit(Instruction.Create(OpCodes.Ldelem_U4));
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
					this.Emit(Instruction.Create(op));
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
						this.Emit(Instruction.Create(op2));
					}
					else
					{
						bool flag4 = exp is LiteralExpression;
						if (flag4)
						{
							LiteralExpression literal = (LiteralExpression)exp;
							this.Emit(Instruction.CreateLdcI4((int)literal.Value));
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

		// Token: 0x06000038 RID: 56 RVA: 0x00003704 File Offset: 0x00001904
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
				Instruction lbl = Instruction.Create(OpCodes.Nop);
				Instruction dup = Instruction.Create(OpCodes.Dup);
				this.Emit(Instruction.CreateLdcI4(loop.Begin));
				this.Emit(Instruction.Create(OpCodes.Br, dup));
				this.Emit(Instruction.CreateLdcI4(loop.Begin));
				this.Emit(lbl);
				foreach (Statement child3 in loop.Statements)
				{
					this.EmitStatement(child3);
				}
				this.Emit(Instruction.CreateLdcI4(1));
				this.Emit(Instruction.Create(OpCodes.Add));
				this.Emit(dup);
				this.Emit(Instruction.CreateLdcI4(loop.Limit));
				this.Emit(Instruction.Create(OpCodes.Blt, lbl));
				this.Emit(Instruction.Create(OpCodes.Pop));
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000038C4 File Offset: 0x00001AC4
		private void EmitStore(Expression exp, Expression value)
		{
			bool flag = exp is ArrayIndexExpression;
			if (flag)
			{
				ArrayIndexExpression arrIndex = (ArrayIndexExpression)exp;
				this.EmitLoad(arrIndex.Array);
				this.Emit(Instruction.CreateLdcI4(arrIndex.Index));
				this.EmitLoad(value);
				this.Emit(Instruction.Create(OpCodes.Stelem_I4));
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

		// Token: 0x0600003A RID: 58 RVA: 0x00003953 File Offset: 0x00001B53
		public void GenerateCIL(Expression expression)
		{
			this.EmitLoad(expression);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000395E File Offset: 0x00001B5E
		public void GenerateCIL(Statement statement)
		{
			this.EmitStatement(statement);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003969 File Offset: 0x00001B69
		protected virtual void LoadVar(Variable var)
		{
			this.Emit(Instruction.Create(OpCodes.Ldloc, this.Var(var)));
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003984 File Offset: 0x00001B84
		protected virtual void StoreVar(Variable var)
		{
			this.Emit(Instruction.Create(OpCodes.Stloc, this.Var(var)));
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000039A0 File Offset: 0x00001BA0
		protected virtual Local Var(Variable var)
		{
			Local ret;
			bool flag = !this.localMap.TryGetValue(var.Name, out ret);
			if (flag)
			{
				ret = new Local(this.Method.Module.CorLibTypes.UInt32);
				ret.Name = var.Name;
				this.localMap[var.Name] = ret;
			}
			return ret;
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00003A0A File Offset: 0x00001C0A
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00003A12 File Offset: 0x00001C12
		public IList<Instruction> Instructions
		{
			[CompilerGenerated]
			get
			{
				return this.<Instructions>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Instructions>k__BackingField = value;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00003A1B File Offset: 0x00001C1B
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00003A23 File Offset: 0x00001C23
		public MethodDef Method
		{
			[CompilerGenerated]
			get
			{
				return this.<Method>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Method>k__BackingField = value;
			}
		}

		// Token: 0x04000011 RID: 17
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<Instruction> <Instructions>k__BackingField;

		// Token: 0x04000012 RID: 18
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MethodDef <Method>k__BackingField;

		// Token: 0x04000013 RID: 19
		private readonly Dictionary<string, Local> localMap = new Dictionary<string, Local>();
	}
}
