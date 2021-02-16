using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Transforms
{
	// Token: 0x0200000B RID: 11
	internal class ShuffleTransform
	{
		// Token: 0x06000021 RID: 33 RVA: 0x00002C34 File Offset: 0x00000E34
		private static IEnumerable<Variable> GetVariableDefinition(Expression exp)
		{
			bool flag = exp is VariableExpression;
			if (flag)
			{
				yield return ((VariableExpression)exp).Variable;
			}
			yield break;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002C44 File Offset: 0x00000E44
		private static IEnumerable<Variable> GetVariableDefinition(Statement st)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				foreach (Variable current in ShuffleTransform.GetVariableDefinition(((AssignmentStatement)st).Target))
				{
					yield return current;
					current = null;
				}
				IEnumerator<Variable> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002C54 File Offset: 0x00000E54
		private static IEnumerable<Variable> GetVariableUsage(Expression exp)
		{
			bool flag = exp is VariableExpression;
			if (flag)
			{
				yield return ((VariableExpression)exp).Variable;
			}
			else
			{
				bool flag2 = exp is ArrayIndexExpression;
				if (flag2)
				{
					foreach (Variable current in ShuffleTransform.GetVariableUsage(((ArrayIndexExpression)exp).Array))
					{
						yield return current;
						current = null;
					}
					IEnumerator<Variable> enumerator = null;
				}
				else
				{
					bool flag3 = exp is BinOpExpression;
					if (flag3)
					{
						foreach (Variable current2 in ShuffleTransform.GetVariableUsage(((BinOpExpression)exp).Left).Concat(ShuffleTransform.GetVariableUsage(((BinOpExpression)exp).Right)))
						{
							yield return current2;
							current2 = null;
						}
						IEnumerator<Variable> enumerator2 = null;
					}
					else
					{
						bool flag4 = exp is UnaryOpExpression;
						if (flag4)
						{
							foreach (Variable current3 in ShuffleTransform.GetVariableUsage(((UnaryOpExpression)exp).Value))
							{
								yield return current3;
								current3 = null;
							}
							IEnumerator<Variable> enumerator3 = null;
						}
					}
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002C64 File Offset: 0x00000E64
		private static IEnumerable<Variable> GetVariableUsage(Statement st)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				foreach (Variable current in ShuffleTransform.GetVariableUsage(((AssignmentStatement)st).Value))
				{
					yield return current;
					current = null;
				}
				IEnumerator<Variable> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002C74 File Offset: 0x00000E74
		public static void Run(StatementBlock block, RandomGenerator random)
		{
			ShuffleTransform.TransformContext transformContext = new ShuffleTransform.TransformContext();
			transformContext.Statements = block.Statements.ToArray<Statement>();
			transformContext.Usages = block.Statements.ToDictionary((Statement s) => s, (Statement s) => ShuffleTransform.GetVariableUsage(s).ToArray<Variable>());
			transformContext.Definitions = block.Statements.ToDictionary((Statement s) => s, (Statement s) => ShuffleTransform.GetVariableDefinition(s).ToArray<Variable>());
			ShuffleTransform.TransformContext context = transformContext;
			for (int i = 0; i < 20; i++)
			{
				foreach (Statement st in context.Statements)
				{
					int index = block.Statements.IndexOf(st);
					ShuffleTransform.GetVariableUsage(st).Concat(ShuffleTransform.GetVariableDefinition(st)).ToArray<Variable>();
					int defIndex = ShuffleTransform.SearchUpwardKill(context, st, block, index);
					int useIndex = ShuffleTransform.SearchDownwardKill(context, st, block, index);
					int newIndex = defIndex + random.NextInt32(1, useIndex - defIndex);
					bool flag = newIndex > index;
					if (flag)
					{
						newIndex--;
					}
					block.Statements.RemoveAt(index);
					block.Statements.Insert(newIndex, st);
				}
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002E08 File Offset: 0x00001008
		private static int SearchDownwardKill(ShuffleTransform.TransformContext context, Statement st, StatementBlock block, int startIndex)
		{
			Variable[] usage = context.Usages[st];
			Variable[] definition = context.Definitions[st];
			for (int i = startIndex + 1; i < block.Statements.Count; i++)
			{
				bool flag = context.Usages[block.Statements[i]].Intersect(definition).Count<Variable>() > 0 || context.Definitions[block.Statements[i]].Intersect(usage).Count<Variable>() > 0;
				if (flag)
				{
					return i;
				}
			}
			return block.Statements.Count - 1;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002EBC File Offset: 0x000010BC
		private static int SearchUpwardKill(ShuffleTransform.TransformContext context, Statement st, StatementBlock block, int startIndex)
		{
			Variable[] usage = context.Usages[st];
			Variable[] definition = context.Definitions[st];
			for (int i = startIndex - 1; i >= 0; i--)
			{
				bool flag = context.Usages[block.Statements[i]].Intersect(definition).Count<Variable>() > 0 || context.Definitions[block.Statements[i]].Intersect(usage).Count<Variable>() > 0;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000020FC File Offset: 0x000002FC
		public ShuffleTransform()
		{
		}

		// Token: 0x04000003 RID: 3
		private const int ITERATION = 20;

		// Token: 0x0200002F RID: 47
		private class TransformContext
		{
			// Token: 0x060000F1 RID: 241 RVA: 0x000020FC File Offset: 0x000002FC
			public TransformContext()
			{
			}

			// Token: 0x0400006A RID: 106
			public Dictionary<Statement, Variable[]> Definitions;

			// Token: 0x0400006B RID: 107
			public Statement[] Statements;

			// Token: 0x0400006C RID: 108
			public Dictionary<Statement, Variable[]> Usages;
		}

		// Token: 0x02000030 RID: 48
		[CompilerGenerated]
		private sealed class <GetVariableDefinition>d__0 : IEnumerable<Variable>, IEnumerable, IEnumerator<Variable>, IDisposable, IEnumerator
		{
			// Token: 0x060000F2 RID: 242 RVA: 0x00006D25 File Offset: 0x00004F25
			[DebuggerHidden]
			public <GetVariableDefinition>d__0(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x060000F3 RID: 243 RVA: 0x00006D45 File Offset: 0x00004F45
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x060000F4 RID: 244 RVA: 0x00006D48 File Offset: 0x00004F48
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
					bool flag = exp is VariableExpression;
					if (flag)
					{
						this.<>2__current = ((VariableExpression)exp).Variable;
						this.<>1__state = 1;
						return true;
					}
				}
				return false;
			}

			// Token: 0x1700002D RID: 45
			// (get) Token: 0x060000F5 RID: 245 RVA: 0x00006DB0 File Offset: 0x00004FB0
			Variable IEnumerator<Variable>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060000F6 RID: 246 RVA: 0x00006DB8 File Offset: 0x00004FB8
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x1700002E RID: 46
			// (get) Token: 0x060000F7 RID: 247 RVA: 0x00006DB0 File Offset: 0x00004FB0
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060000F8 RID: 248 RVA: 0x00006DC0 File Offset: 0x00004FC0
			[DebuggerHidden]
			IEnumerator<Variable> IEnumerable<Variable>.GetEnumerator()
			{
				ShuffleTransform.<GetVariableDefinition>d__0 <GetVariableDefinition>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<GetVariableDefinition>d__ = this;
				}
				else
				{
					<GetVariableDefinition>d__ = new ShuffleTransform.<GetVariableDefinition>d__0(0);
				}
				<GetVariableDefinition>d__.exp = exp;
				return <GetVariableDefinition>d__;
			}

			// Token: 0x060000F9 RID: 249 RVA: 0x00006E08 File Offset: 0x00005008
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Confuser.DynCipher.AST.Variable>.GetEnumerator();
			}

			// Token: 0x0400006D RID: 109
			private int <>1__state;

			// Token: 0x0400006E RID: 110
			private Variable <>2__current;

			// Token: 0x0400006F RID: 111
			private int <>l__initialThreadId;

			// Token: 0x04000070 RID: 112
			private Expression exp;

			// Token: 0x04000071 RID: 113
			public Expression <>3__exp;
		}

		// Token: 0x02000031 RID: 49
		[CompilerGenerated]
		private sealed class <GetVariableDefinition>d__1 : IEnumerable<Variable>, IEnumerable, IEnumerator<Variable>, IDisposable, IEnumerator
		{
			// Token: 0x060000FA RID: 250 RVA: 0x00006E10 File Offset: 0x00005010
			[DebuggerHidden]
			public <GetVariableDefinition>d__1(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x060000FB RID: 251 RVA: 0x00006E30 File Offset: 0x00005030
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				int num = this.<>1__state;
				if (num == -3 || num == 1)
				{
					try
					{
					}
					finally
					{
						this.<>m__Finally1();
					}
				}
			}

			// Token: 0x060000FC RID: 252 RVA: 0x00006E70 File Offset: 0x00005070
			bool IEnumerator.MoveNext()
			{
				bool result;
				try
				{
					int num = this.<>1__state;
					if (num != 0)
					{
						if (num != 1)
						{
							return false;
						}
						this.<>1__state = -3;
						current = null;
					}
					else
					{
						this.<>1__state = -1;
						bool flag = st is AssignmentStatement;
						if (!flag)
						{
							goto IL_BB;
						}
						enumerator = ShuffleTransform.GetVariableDefinition(((AssignmentStatement)st).Target).GetEnumerator();
						this.<>1__state = -3;
					}
					if (enumerator.MoveNext())
					{
						current = enumerator.Current;
						this.<>2__current = current;
						this.<>1__state = 1;
						return true;
					}
					this.<>m__Finally1();
					enumerator = null;
					IL_BB:
					result = false;
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return result;
			}

			// Token: 0x060000FD RID: 253 RVA: 0x00006F58 File Offset: 0x00005158
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}

			// Token: 0x1700002F RID: 47
			// (get) Token: 0x060000FE RID: 254 RVA: 0x00006F75 File Offset: 0x00005175
			Variable IEnumerator<Variable>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060000FF RID: 255 RVA: 0x00006DB8 File Offset: 0x00004FB8
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000030 RID: 48
			// (get) Token: 0x06000100 RID: 256 RVA: 0x00006F75 File Offset: 0x00005175
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000101 RID: 257 RVA: 0x00006F80 File Offset: 0x00005180
			[DebuggerHidden]
			IEnumerator<Variable> IEnumerable<Variable>.GetEnumerator()
			{
				ShuffleTransform.<GetVariableDefinition>d__1 <GetVariableDefinition>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<GetVariableDefinition>d__ = this;
				}
				else
				{
					<GetVariableDefinition>d__ = new ShuffleTransform.<GetVariableDefinition>d__1(0);
				}
				<GetVariableDefinition>d__.st = st;
				return <GetVariableDefinition>d__;
			}

			// Token: 0x06000102 RID: 258 RVA: 0x00006FC8 File Offset: 0x000051C8
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Confuser.DynCipher.AST.Variable>.GetEnumerator();
			}

			// Token: 0x04000072 RID: 114
			private int <>1__state;

			// Token: 0x04000073 RID: 115
			private Variable <>2__current;

			// Token: 0x04000074 RID: 116
			private int <>l__initialThreadId;

			// Token: 0x04000075 RID: 117
			private Statement st;

			// Token: 0x04000076 RID: 118
			public Statement <>3__st;

			// Token: 0x04000077 RID: 119
			private IEnumerator<Variable> <>s__1;

			// Token: 0x04000078 RID: 120
			private Variable <current>5__2;
		}

		// Token: 0x02000032 RID: 50
		[CompilerGenerated]
		private sealed class <GetVariableUsage>d__2 : IEnumerable<Variable>, IEnumerable, IEnumerator<Variable>, IDisposable, IEnumerator
		{
			// Token: 0x06000103 RID: 259 RVA: 0x00006FD0 File Offset: 0x000051D0
			[DebuggerHidden]
			public <GetVariableUsage>d__2(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x06000104 RID: 260 RVA: 0x00006FF0 File Offset: 0x000051F0
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				switch (this.<>1__state)
				{
				case -5:
				case 4:
					try
					{
					}
					finally
					{
						this.<>m__Finally3();
					}
					break;
				case -4:
				case 3:
					try
					{
					}
					finally
					{
						this.<>m__Finally2();
					}
					break;
				case -3:
				case 2:
					try
					{
					}
					finally
					{
						this.<>m__Finally1();
					}
					break;
				}
			}

			// Token: 0x06000105 RID: 261 RVA: 0x00007084 File Offset: 0x00005284
			bool IEnumerator.MoveNext()
			{
				bool result;
				try
				{
					switch (this.<>1__state)
					{
					case 0:
					{
						this.<>1__state = -1;
						bool flag = exp is VariableExpression;
						if (flag)
						{
							this.<>2__current = ((VariableExpression)exp).Variable;
							this.<>1__state = 1;
							return true;
						}
						bool flag2 = exp is ArrayIndexExpression;
						if (flag2)
						{
							enumerator = ShuffleTransform.GetVariableUsage(((ArrayIndexExpression)exp).Array).GetEnumerator();
							this.<>1__state = -3;
						}
						else
						{
							bool flag3 = exp is BinOpExpression;
							if (flag3)
							{
								enumerator2 = ShuffleTransform.GetVariableUsage(((BinOpExpression)exp).Left).Concat(ShuffleTransform.GetVariableUsage(((BinOpExpression)exp).Right)).GetEnumerator();
								this.<>1__state = -4;
								goto IL_1C0;
							}
							bool flag4 = exp is UnaryOpExpression;
							if (flag4)
							{
								enumerator3 = ShuffleTransform.GetVariableUsage(((UnaryOpExpression)exp).Value).GetEnumerator();
								this.<>1__state = -5;
								goto IL_25D;
							}
							goto IL_279;
						}
						break;
					}
					case 1:
						this.<>1__state = -1;
						goto IL_279;
					case 2:
						this.<>1__state = -3;
						current = null;
						break;
					case 3:
						this.<>1__state = -4;
						current2 = null;
						goto IL_1C0;
					case 4:
						this.<>1__state = -5;
						current3 = null;
						goto IL_25D;
					default:
						return false;
					}
					if (!enumerator.MoveNext())
					{
						this.<>m__Finally1();
						enumerator = null;
						goto IL_279;
					}
					current = enumerator.Current;
					this.<>2__current = current;
					this.<>1__state = 2;
					return true;
					IL_1C0:
					if (!enumerator2.MoveNext())
					{
						this.<>m__Finally2();
						enumerator2 = null;
						goto IL_279;
					}
					current2 = enumerator2.Current;
					this.<>2__current = current2;
					this.<>1__state = 3;
					return true;
					IL_25D:
					if (enumerator3.MoveNext())
					{
						current3 = enumerator3.Current;
						this.<>2__current = current3;
						this.<>1__state = 4;
						return true;
					}
					this.<>m__Finally3();
					enumerator3 = null;
					IL_279:
					result = false;
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return result;
			}

			// Token: 0x06000106 RID: 262 RVA: 0x00007334 File Offset: 0x00005534
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}

			// Token: 0x06000107 RID: 263 RVA: 0x00007351 File Offset: 0x00005551
			private void <>m__Finally2()
			{
				this.<>1__state = -1;
				if (enumerator2 != null)
				{
					enumerator2.Dispose();
				}
			}

			// Token: 0x06000108 RID: 264 RVA: 0x0000736E File Offset: 0x0000556E
			private void <>m__Finally3()
			{
				this.<>1__state = -1;
				if (enumerator3 != null)
				{
					enumerator3.Dispose();
				}
			}

			// Token: 0x17000031 RID: 49
			// (get) Token: 0x06000109 RID: 265 RVA: 0x0000738B File Offset: 0x0000558B
			Variable IEnumerator<Variable>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0600010A RID: 266 RVA: 0x00006DB8 File Offset: 0x00004FB8
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000032 RID: 50
			// (get) Token: 0x0600010B RID: 267 RVA: 0x0000738B File Offset: 0x0000558B
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0600010C RID: 268 RVA: 0x00007394 File Offset: 0x00005594
			[DebuggerHidden]
			IEnumerator<Variable> IEnumerable<Variable>.GetEnumerator()
			{
				ShuffleTransform.<GetVariableUsage>d__2 <GetVariableUsage>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<GetVariableUsage>d__ = this;
				}
				else
				{
					<GetVariableUsage>d__ = new ShuffleTransform.<GetVariableUsage>d__2(0);
				}
				<GetVariableUsage>d__.exp = exp;
				return <GetVariableUsage>d__;
			}

			// Token: 0x0600010D RID: 269 RVA: 0x000073DC File Offset: 0x000055DC
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Confuser.DynCipher.AST.Variable>.GetEnumerator();
			}

			// Token: 0x04000079 RID: 121
			private int <>1__state;

			// Token: 0x0400007A RID: 122
			private Variable <>2__current;

			// Token: 0x0400007B RID: 123
			private int <>l__initialThreadId;

			// Token: 0x0400007C RID: 124
			private Expression exp;

			// Token: 0x0400007D RID: 125
			public Expression <>3__exp;

			// Token: 0x0400007E RID: 126
			private IEnumerator<Variable> <>s__1;

			// Token: 0x0400007F RID: 127
			private Variable <current>5__2;

			// Token: 0x04000080 RID: 128
			private IEnumerator<Variable> <>s__3;

			// Token: 0x04000081 RID: 129
			private Variable <current2>5__4;

			// Token: 0x04000082 RID: 130
			private IEnumerator<Variable> <>s__5;

			// Token: 0x04000083 RID: 131
			private Variable <current3>5__6;
		}

		// Token: 0x02000033 RID: 51
		[CompilerGenerated]
		private sealed class <GetVariableUsage>d__3 : IEnumerable<Variable>, IEnumerable, IEnumerator<Variable>, IDisposable, IEnumerator
		{
			// Token: 0x0600010E RID: 270 RVA: 0x000073E4 File Offset: 0x000055E4
			[DebuggerHidden]
			public <GetVariableUsage>d__3(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x0600010F RID: 271 RVA: 0x00007404 File Offset: 0x00005604
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				int num = this.<>1__state;
				if (num == -3 || num == 1)
				{
					try
					{
					}
					finally
					{
						this.<>m__Finally1();
					}
				}
			}

			// Token: 0x06000110 RID: 272 RVA: 0x00007444 File Offset: 0x00005644
			bool IEnumerator.MoveNext()
			{
				bool result;
				try
				{
					int num = this.<>1__state;
					if (num != 0)
					{
						if (num != 1)
						{
							return false;
						}
						this.<>1__state = -3;
						current = null;
					}
					else
					{
						this.<>1__state = -1;
						bool flag = st is AssignmentStatement;
						if (!flag)
						{
							goto IL_BB;
						}
						enumerator = ShuffleTransform.GetVariableUsage(((AssignmentStatement)st).Value).GetEnumerator();
						this.<>1__state = -3;
					}
					if (enumerator.MoveNext())
					{
						current = enumerator.Current;
						this.<>2__current = current;
						this.<>1__state = 1;
						return true;
					}
					this.<>m__Finally1();
					enumerator = null;
					IL_BB:
					result = false;
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return result;
			}

			// Token: 0x06000111 RID: 273 RVA: 0x0000752C File Offset: 0x0000572C
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}

			// Token: 0x17000033 RID: 51
			// (get) Token: 0x06000112 RID: 274 RVA: 0x00007549 File Offset: 0x00005749
			Variable IEnumerator<Variable>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000113 RID: 275 RVA: 0x00006DB8 File Offset: 0x00004FB8
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000034 RID: 52
			// (get) Token: 0x06000114 RID: 276 RVA: 0x00007549 File Offset: 0x00005749
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000115 RID: 277 RVA: 0x00007554 File Offset: 0x00005754
			[DebuggerHidden]
			IEnumerator<Variable> IEnumerable<Variable>.GetEnumerator()
			{
				ShuffleTransform.<GetVariableUsage>d__3 <GetVariableUsage>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<GetVariableUsage>d__ = this;
				}
				else
				{
					<GetVariableUsage>d__ = new ShuffleTransform.<GetVariableUsage>d__3(0);
				}
				<GetVariableUsage>d__.st = st;
				return <GetVariableUsage>d__;
			}

			// Token: 0x06000116 RID: 278 RVA: 0x0000759C File Offset: 0x0000579C
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Confuser.DynCipher.AST.Variable>.GetEnumerator();
			}

			// Token: 0x04000084 RID: 132
			private int <>1__state;

			// Token: 0x04000085 RID: 133
			private Variable <>2__current;

			// Token: 0x04000086 RID: 134
			private int <>l__initialThreadId;

			// Token: 0x04000087 RID: 135
			private Statement st;

			// Token: 0x04000088 RID: 136
			public Statement <>3__st;

			// Token: 0x04000089 RID: 137
			private IEnumerator<Variable> <>s__1;

			// Token: 0x0400008A RID: 138
			private Variable <current>5__2;
		}

		// Token: 0x02000034 RID: 52
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000117 RID: 279 RVA: 0x000075A4 File Offset: 0x000057A4
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000118 RID: 280 RVA: 0x000020FC File Offset: 0x000002FC
			public <>c()
			{
			}

			// Token: 0x06000119 RID: 281 RVA: 0x000075B0 File Offset: 0x000057B0
			internal Statement <Run>b__4_0(Statement s)
			{
				return s;
			}

			// Token: 0x0600011A RID: 282 RVA: 0x000075B3 File Offset: 0x000057B3
			internal Variable[] <Run>b__4_1(Statement s)
			{
				return ShuffleTransform.GetVariableUsage(s).ToArray<Variable>();
			}

			// Token: 0x0600011B RID: 283 RVA: 0x000075B0 File Offset: 0x000057B0
			internal Statement <Run>b__4_2(Statement s)
			{
				return s;
			}

			// Token: 0x0600011C RID: 284 RVA: 0x000075C0 File Offset: 0x000057C0
			internal Variable[] <Run>b__4_3(Statement s)
			{
				return ShuffleTransform.GetVariableDefinition(s).ToArray<Variable>();
			}

			// Token: 0x0400008B RID: 139
			public static readonly ShuffleTransform.<>c <>9 = new ShuffleTransform.<>c();

			// Token: 0x0400008C RID: 140
			public static Func<Statement, Statement> <>9__4_0;

			// Token: 0x0400008D RID: 141
			public static Func<Statement, Variable[]> <>9__4_1;

			// Token: 0x0400008E RID: 142
			public static Func<Statement, Statement> <>9__4_2;

			// Token: 0x0400008F RID: 143
			public static Func<Statement, Variable[]> <>9__4_3;
		}
	}
}
