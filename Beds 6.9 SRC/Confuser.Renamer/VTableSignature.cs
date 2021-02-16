using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000014 RID: 20
	public class VTableSignature
	{
		// Token: 0x06000074 RID: 116 RVA: 0x0000223F File Offset: 0x0000043F
		internal VTableSignature(MethodSig sig, string name)
		{
			this.MethodSig = sig;
			this.Name = name;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00002259 File Offset: 0x00000459
		// (set) Token: 0x06000076 RID: 118 RVA: 0x00002261 File Offset: 0x00000461
		public MethodSig MethodSig
		{
			[CompilerGenerated]
			get
			{
				return this.<MethodSig>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<MethodSig>k__BackingField = value;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000077 RID: 119 RVA: 0x0000226A File Offset: 0x0000046A
		// (set) Token: 0x06000078 RID: 120 RVA: 0x00002272 File Offset: 0x00000472
		public string Name
		{
			[CompilerGenerated]
			get
			{
				return this.<Name>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Name>k__BackingField = value;
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005CE4 File Offset: 0x00003EE4
		public static VTableSignature FromMethod(IMethod method)
		{
			MethodSig sig = method.MethodSig;
			TypeSig declType = method.DeclaringType.ToTypeSig();
			bool flag = declType is GenericInstSig;
			if (flag)
			{
				sig = GenericArgumentResolver.Resolve(sig, ((GenericInstSig)declType).GenericArguments);
			}
			return new VTableSignature(sig, method.Name);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005D3C File Offset: 0x00003F3C
		public override bool Equals(object obj)
		{
			VTableSignature other = obj as VTableSignature;
			bool flag = other == null;
			return !flag && default(SigComparer).Equals(this.MethodSig, other.MethodSig) && this.Name.Equals(other.Name, StringComparison.Ordinal);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005D98 File Offset: 0x00003F98
		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 7 + default(SigComparer).GetHashCode(this.MethodSig);
			return hash * 7 + this.Name.GetHashCode();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005DD8 File Offset: 0x00003FD8
		public static bool operator ==(VTableSignature a, VTableSignature b)
		{
			bool flag = a == b;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = !object.Equals(a, null) && object.Equals(b, null);
				result = (!flag2 && a.Equals(b));
			}
			return result;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00005E18 File Offset: 0x00004018
		public static bool operator !=(VTableSignature a, VTableSignature b)
		{
			return !(a == b);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005E34 File Offset: 0x00004034
		public override string ToString()
		{
			return FullNameCreator.MethodFullName("", this.Name, this.MethodSig);
		}

		// Token: 0x0400002C RID: 44
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private MethodSig <MethodSig>k__BackingField;

		// Token: 0x0400002D RID: 45
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string <Name>k__BackingField;
	}
}
