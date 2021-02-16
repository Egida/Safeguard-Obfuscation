using System;
using System.Diagnostics;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000038 RID: 56
	internal abstract class SizedBamlRecord : BamlRecord
	{
		// Token: 0x06000136 RID: 310 RVA: 0x0000979C File Offset: 0x0000799C
		public override void Read(BamlBinaryReader reader)
		{
			long pos = reader.BaseStream.Position;
			int size = reader.ReadEncodedInt();
			this.ReadData(reader, size - (int)(reader.BaseStream.Position - pos));
			Debug.Assert(reader.BaseStream.Position - pos == (long)size);
		}

		// Token: 0x06000137 RID: 311 RVA: 0x000097EC File Offset: 0x000079EC
		private int SizeofEncodedInt(int val)
		{
			bool flag = (val & -128) == 0;
			int result;
			if (flag)
			{
				result = 1;
			}
			else
			{
				bool flag2 = (val & -16384) == 0;
				if (flag2)
				{
					result = 2;
				}
				else
				{
					bool flag3 = (val & -2097152) == 0;
					if (flag3)
					{
						result = 3;
					}
					else
					{
						bool flag4 = (val & -268435456) == 0;
						if (flag4)
						{
							result = 4;
						}
						else
						{
							result = 5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000984C File Offset: 0x00007A4C
		public override void Write(BamlBinaryWriter writer)
		{
			long pos = writer.BaseStream.Position;
			this.WriteData(writer);
			int size = (int)(writer.BaseStream.Position - pos);
			size = this.SizeofEncodedInt(this.SizeofEncodedInt(size) + size) + size;
			writer.BaseStream.Position = pos;
			writer.WriteEncodedInt(size);
			this.WriteData(writer);
		}

		// Token: 0x06000139 RID: 313
		protected abstract void ReadData(BamlBinaryReader reader, int size);

		// Token: 0x0600013A RID: 314
		protected abstract void WriteData(BamlBinaryWriter writer);

		// Token: 0x0600013B RID: 315 RVA: 0x00002798 File Offset: 0x00000998
		protected SizedBamlRecord()
		{
		}
	}
}
