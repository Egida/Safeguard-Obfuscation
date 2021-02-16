using System;

namespace Confuser.Core
{
	/// <summary>
	///     Base class of Confuser protections.
	/// </summary>
	/// <remarks>
	///     A parameterless constructor must exists in derived classes to enable plugin discovery.
	/// </remarks>
	// Token: 0x02000062 RID: 98
	public abstract class Protection : ConfuserComponent
	{
		/// <summary>
		///     Gets the preset this protection is in.
		/// </summary>
		/// <value>The protection's preset.</value>
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000244 RID: 580
		public abstract ProtectionPreset Preset { get; }

		// Token: 0x06000245 RID: 581 RVA: 0x00002EA7 File Offset: 0x000010A7
		protected Protection()
		{
		}
	}
}
