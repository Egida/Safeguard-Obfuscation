using System;
using System.Diagnostics;
using Confuser.Core;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000035 RID: 53
	internal class BAMLPropertyReference : IBAMLReference
	{
		// Token: 0x0600012D RID: 301 RVA: 0x00002776 File Offset: 0x00000976
		public BAMLPropertyReference(PropertyRecord rec)
		{
			this.rec = rec;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00003690 File Offset: 0x00001890
		public bool CanRename(string oldName, string newName)
		{
			return true;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00009710 File Offset: 0x00007910
		public void Rename(string oldName, string newName)
		{
			string value = this.rec.Value;
			bool flag = value.IndexOf(oldName, StringComparison.OrdinalIgnoreCase) != -1;
			if (flag)
			{
				value = newName;
			}
			else
			{
				bool flag2 = oldName.EndsWith(".baml");
				if (!flag2)
				{
					throw new UnreachableException();
				}
				Debug.Assert(newName.EndsWith(".baml"));
				value = newName.Substring(0, newName.Length - 5) + ".xaml";
			}
			this.rec.Value = "pack://application:,,,/" + value;
		}

		// Token: 0x0400009C RID: 156
		private PropertyRecord rec;
	}
}
