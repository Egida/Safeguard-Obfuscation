using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000025 RID: 37
	public abstract class Expression
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x000068BD File Offset: 0x00004ABD
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x000068C5 File Offset: 0x00004AC5
		public object Tag
		{
			[CompilerGenerated]
			get
			{
				return this.<Tag>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Tag>k__BackingField = value;
			}
		}

		// Token: 0x060000C2 RID: 194
		public abstract override string ToString();

		// Token: 0x060000C3 RID: 195 RVA: 0x000068D0 File Offset: 0x00004AD0
		public static BinOpExpression operator +(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Add
			};
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006900 File Offset: 0x00004B00
		public static BinOpExpression operator -(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Sub
			};
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006930 File Offset: 0x00004B30
		public static BinOpExpression operator *(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Mul
			};
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006960 File Offset: 0x00004B60
		public static BinOpExpression operator >>(Expression a, int b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = (uint)b,
				Operation = BinOps.Rsh
			};
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00006994 File Offset: 0x00004B94
		public static BinOpExpression operator <<(Expression a, int b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = (uint)b,
				Operation = BinOps.Lsh
			};
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000069C8 File Offset: 0x00004BC8
		public static BinOpExpression operator |(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Or
			};
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000069F8 File Offset: 0x00004BF8
		public static BinOpExpression operator &(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.And
			};
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00006A28 File Offset: 0x00004C28
		public static BinOpExpression operator ^(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Xor
			};
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00006A58 File Offset: 0x00004C58
		public static UnaryOpExpression operator ~(Expression val)
		{
			return new UnaryOpExpression
			{
				Value = val,
				Operation = UnaryOps.Not
			};
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00006A80 File Offset: 0x00004C80
		public static UnaryOpExpression operator -(Expression val)
		{
			return new UnaryOpExpression
			{
				Value = val,
				Operation = UnaryOps.Negate
			};
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000020FC File Offset: 0x000002FC
		protected Expression()
		{
		}

		// Token: 0x04000059 RID: 89
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object <Tag>k__BackingField;
	}
}
