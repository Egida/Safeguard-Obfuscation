using System;
using System.Diagnostics;
using Confuser.Core;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000071 RID: 113
	public class BAMLStringReference : IBAMLReference
	{
		// Token: 0x06000296 RID: 662 RVA: 0x00003477 File Offset: 0x00001677
		public BAMLStringReference(Instruction instr)
		{
			this.instr = instr;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000A87C File Offset: 0x00008A7C
		public bool CanRename(string oldName, string newName)
		{
			return this.instr.OpCode.Code == Code.Ldstr;
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000A8A4 File Offset: 0x00008AA4
		public void Rename(string oldName, string newName)
		{
			string value = (string)this.instr.Operand;
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
				value = value.Replace(oldName.Replace(".baml", string.Empty, StringComparison.InvariantCultureIgnoreCase), newName.Replace(".baml", string.Empty, StringComparison.InvariantCultureIgnoreCase), StringComparison.InvariantCultureIgnoreCase);
			}
			this.instr.Operand = value;
		}

		// Token: 0x04000121 RID: 289
		private Instruction instr;
	}
}
