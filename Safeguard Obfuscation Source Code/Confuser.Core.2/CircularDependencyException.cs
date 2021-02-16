using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Core
{
	/// <summary>
	///     The exception that is thrown when there exists circular dependency between protections.
	/// </summary>
	// Token: 0x02000042 RID: 66
	internal class CircularDependencyException : Exception
	{
		// Token: 0x06000168 RID: 360 RVA: 0x000029B8 File Offset: 0x00000BB8
		internal CircularDependencyException(Protection a, Protection b) : base(string.Format("The protections '{0}' and '{1}' has a circular dependency between them.", a, b))
		{
			Debug.Assert(a != null);
			Debug.Assert(b != null);
			this.ProtectionA = a;
			this.ProtectionB = b;
		}

		/// <summary>
		///     First protection that involved in circular dependency.
		/// </summary>
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000169 RID: 361 RVA: 0x000029F2 File Offset: 0x00000BF2
		// (set) Token: 0x0600016A RID: 362 RVA: 0x000029FA File Offset: 0x00000BFA
		public Protection ProtectionA
		{
			[CompilerGenerated]
			get
			{
				return this.<ProtectionA>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ProtectionA>k__BackingField = value;
			}
		}

		/// <summary>
		///     Second protection that involved in circular dependency.
		/// </summary>
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600016B RID: 363 RVA: 0x00002A03 File Offset: 0x00000C03
		// (set) Token: 0x0600016C RID: 364 RVA: 0x00002A0B File Offset: 0x00000C0B
		public Protection ProtectionB
		{
			[CompilerGenerated]
			get
			{
				return this.<ProtectionB>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ProtectionB>k__BackingField = value;
			}
		}

		// Token: 0x0400015B RID: 347
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private Protection <ProtectionA>k__BackingField;

		// Token: 0x0400015C RID: 348
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private Protection <ProtectionB>k__BackingField;
	}
}
