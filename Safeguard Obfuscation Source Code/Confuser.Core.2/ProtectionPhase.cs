using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Confuser.Core
{
	/// <summary>
	///     Base class of protection phases.
	/// </summary>
	// Token: 0x02000065 RID: 101
	public abstract class ProtectionPhase
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionPhase" /> class.
		/// </summary>
		/// <param name="parent">The parent component of this phase.</param>
		// Token: 0x0600024C RID: 588 RVA: 0x0000300B File Offset: 0x0000120B
		public ProtectionPhase(ConfuserComponent parent)
		{
			this.Parent = parent;
		}

		/// <summary>
		///     Executes the protection phase.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="parameters">The parameters of protection.</param>
		// Token: 0x0600024D RID: 589
		protected internal abstract void Execute(ConfuserContext context, ProtectionParameters parameters);

		/// <summary>
		///     Gets the name of the phase.
		/// </summary>
		/// <value>The name of phase.</value>
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600024E RID: 590
		public abstract string Name { get; }

		/// <summary>
		///     Gets the parent component.
		/// </summary>
		/// <value>The parent component.</value>
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600024F RID: 591 RVA: 0x0000301D File Offset: 0x0000121D
		// (set) Token: 0x06000250 RID: 592 RVA: 0x00003025 File Offset: 0x00001225
		public ConfuserComponent Parent
		{
			[CompilerGenerated]
			get
			{
				return this.<Parent>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Parent>k__BackingField = value;
			}
		}

		/// <summary>
		///     Gets a value indicating whether this phase process all targets, not just the targets that requires the component.
		/// </summary>
		/// <value><c>true</c> if this phase process all targets; otherwise, <c>false</c>.</value>
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000251 RID: 593 RVA: 0x0000DECC File Offset: 0x0000C0CC
		public virtual bool ProcessAll
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		///     Gets the targets of protection.
		/// </summary>
		/// <value>The protection targets.</value>
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000252 RID: 594
		public abstract ProtectionTargets Targets { get; }

		// Token: 0x040001CB RID: 459
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ConfuserComponent <Parent>k__BackingField;
	}
}
