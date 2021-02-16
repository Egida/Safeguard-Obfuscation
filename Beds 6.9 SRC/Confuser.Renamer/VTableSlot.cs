using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000015 RID: 21
	public class VTableSlot
	{
		// Token: 0x0600007F RID: 127 RVA: 0x0000227B File Offset: 0x0000047B
		internal VTableSlot(MethodDef def, TypeSig decl, VTableSignature signature) : this(def.DeclaringType.ToTypeSig(), def, decl, signature, null)
		{
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00002294 File Offset: 0x00000494
		internal VTableSlot(TypeSig defDeclType, MethodDef def, TypeSig decl, VTableSignature signature, VTableSlot overrides)
		{
			this.MethodDefDeclType = defDeclType;
			this.MethodDef = def;
			this.DeclaringType = decl;
			this.Signature = signature;
			this.Overrides = overrides;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000081 RID: 129 RVA: 0x000022C8 File Offset: 0x000004C8
		// (set) Token: 0x06000082 RID: 130 RVA: 0x000022D0 File Offset: 0x000004D0
		public TypeSig DeclaringType
		{
			[CompilerGenerated]
			get
			{
				return this.<DeclaringType>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<DeclaringType>k__BackingField = value;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000083 RID: 131 RVA: 0x000022D9 File Offset: 0x000004D9
		// (set) Token: 0x06000084 RID: 132 RVA: 0x000022E1 File Offset: 0x000004E1
		public VTableSignature Signature
		{
			[CompilerGenerated]
			get
			{
				return this.<Signature>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<Signature>k__BackingField = value;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000085 RID: 133 RVA: 0x000022EA File Offset: 0x000004EA
		// (set) Token: 0x06000086 RID: 134 RVA: 0x000022F2 File Offset: 0x000004F2
		public TypeSig MethodDefDeclType
		{
			[CompilerGenerated]
			get
			{
				return this.<MethodDefDeclType>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<MethodDefDeclType>k__BackingField = value;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000087 RID: 135 RVA: 0x000022FB File Offset: 0x000004FB
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00002303 File Offset: 0x00000503
		public MethodDef MethodDef
		{
			[CompilerGenerated]
			get
			{
				return this.<MethodDef>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<MethodDef>k__BackingField = value;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000089 RID: 137 RVA: 0x0000230C File Offset: 0x0000050C
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00002314 File Offset: 0x00000514
		public VTableSlot Overrides
		{
			[CompilerGenerated]
			get
			{
				return this.<Overrides>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Overrides>k__BackingField = value;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005E5C File Offset: 0x0000405C
		public VTableSlot OverridedBy(MethodDef method)
		{
			return new VTableSlot(method.DeclaringType.ToTypeSig(), method, this.DeclaringType, this.Signature, this);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005E8C File Offset: 0x0000408C
		internal VTableSlot Clone()
		{
			return new VTableSlot(this.MethodDefDeclType, this.MethodDef, this.DeclaringType, this.Signature, this.Overrides);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005EC4 File Offset: 0x000040C4
		public override string ToString()
		{
			return this.MethodDef.ToString();
		}

		// Token: 0x0400002E RID: 46
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TypeSig <DeclaringType>k__BackingField;

		// Token: 0x0400002F RID: 47
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private VTableSignature <Signature>k__BackingField;

		// Token: 0x04000030 RID: 48
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TypeSig <MethodDefDeclType>k__BackingField;

		// Token: 0x04000031 RID: 49
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private MethodDef <MethodDef>k__BackingField;

		// Token: 0x04000032 RID: 50
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private VTableSlot <Overrides>k__BackingField;
	}
}
