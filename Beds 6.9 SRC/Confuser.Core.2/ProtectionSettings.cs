using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	/// <summary>
	///     Protection settings for a certain component
	/// </summary>
	// Token: 0x02000066 RID: 102
	public class ProtectionSettings : Dictionary<ConfuserComponent, Dictionary<string, string>>
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionSettings" /> class.
		/// </summary>
		// Token: 0x06000253 RID: 595 RVA: 0x0000302E File Offset: 0x0000122E
		public ProtectionSettings()
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionSettings" /> class
		///     from an existing <see cref="T:Confuser.Core.ProtectionSettings" />.
		/// </summary>
		/// <param name="settings">The settings to copy from.</param>
		// Token: 0x06000254 RID: 596 RVA: 0x00011408 File Offset: 0x0000F608
		public ProtectionSettings(ProtectionSettings settings)
		{
			foreach (KeyValuePair<ConfuserComponent, Dictionary<string, string>> i in settings)
			{
				base.Add(i.Key, new Dictionary<string, string>(i.Value));
			}
		}

		/// <summary>
		///     Determines whether the settings is empty.
		/// </summary>
		/// <returns><c>true</c> if the settings is empty; otherwise, <c>false</c>.</returns>
		// Token: 0x06000255 RID: 597 RVA: 0x00011478 File Offset: 0x0000F678
		public bool IsEmpty()
		{
			return base.Count == 0;
		}
	}
}
