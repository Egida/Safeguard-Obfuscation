using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000072 RID: 114
	internal interface IBAMLReference
	{
		// Token: 0x06000299 RID: 665
		bool CanRename(string oldName, string newName);

		// Token: 0x0600029A RID: 666
		void Rename(string oldName, string newName);
	}
}
