﻿using System;

// Token: 0x02000002 RID: 2
internal class Mutation
{
	// Token: 0x06000001 RID: 1 RVA: 0x000020D0 File Offset: 0x000002D0
	public static T Placeholder<T>(T val)
	{
		return val;
	}

	// Token: 0x06000002 RID: 2 RVA: 0x000026FC File Offset: 0x000008FC
	public static T Value<T>()
	{
		return default(T);
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000026FC File Offset: 0x000008FC
	public static T Value<T, Arg0>(Arg0 arg0)
	{
		return default(T);
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000020D3 File Offset: 0x000002D3
	public static void Crypt(uint[] data, uint[] key)
	{
	}

	// Token: 0x06000005 RID: 5 RVA: 0x000020D5 File Offset: 0x000002D5
	public Mutation()
	{
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002714 File Offset: 0x00000914
	// Note: this type is marked as 'beforefieldinit'.
	static Mutation()
	{
	}

	// Token: 0x04000001 RID: 1
	public static readonly int KeyI0 = 10;

	// Token: 0x04000002 RID: 2
	public static readonly int KeyI1 = 34;

	// Token: 0x04000003 RID: 3
	public static readonly int KeyI2 = 66;

	// Token: 0x04000004 RID: 4
	public static readonly int KeyI3 = 375;

	// Token: 0x04000005 RID: 5
	public static readonly int KeyI4 = 993;

	// Token: 0x04000006 RID: 6
	public static readonly int KeyI5 = 81;

	// Token: 0x04000007 RID: 7
	public static readonly int KeyI6 = 86;

	// Token: 0x04000008 RID: 8
	public static readonly int KeyI7 = 733;

	// Token: 0x04000009 RID: 9
	public static readonly int KeyI8 = 835;

	// Token: 0x0400000A RID: 10
	public static readonly int KeyI9 = 259;

	// Token: 0x0400000B RID: 11
	public static readonly int KeyI10 = 110;

	// Token: 0x0400000C RID: 12
	public static readonly int KeyI11 = 111;

	// Token: 0x0400000D RID: 13
	public static readonly int KeyI12 = 112;

	// Token: 0x0400000E RID: 14
	public static readonly int KeyI13 = 113;

	// Token: 0x0400000F RID: 15
	public static readonly int KeyI14 = 114;

	// Token: 0x04000010 RID: 16
	public static readonly int KeyI15 = 115;
}
