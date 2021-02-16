using System;
using Microsoft.VisualBasic;

namespace SProtector.SProtector.Tools
{
	// Token: 0x02000026 RID: 38
	public class StringProtection
	{
		// Token: 0x060000D1 RID: 209 RVA: 0x00009500 File Offset: 0x00007700
		public string CryptDecrypt(string text)
		{
			string str = "";
			int num = text.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				char ch = Convert.ToChar(text.Substring(i, 1));
				int num2 = (int)ch;
				int charCode = num2 + 13;
				bool flag = num2 <= 90 && num2 >= 65;
				if (flag)
				{
					bool flag2 = charCode > 90;
					char ch2;
					if (flag2)
					{
						int num3 = charCode - 90;
						ch2 = Strings.ChrW(64 + num3);
					}
					else
					{
						ch2 = Strings.ChrW(charCode);
					}
					str += ch2.ToString();
				}
				else
				{
					bool flag3 = num2 <= 122 && num2 >= 97;
					if (flag3)
					{
						bool flag4 = charCode > 122;
						char ch3;
						if (flag4)
						{
							int num4 = charCode - 122;
							ch3 = Strings.ChrW(96 + num4);
						}
						else
						{
							ch3 = Strings.ChrW(charCode);
						}
						str += ch3.ToString();
					}
					else
					{
						str += ch.ToString();
					}
				}
			}
			return str;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000961C File Offset: 0x0000781C
		public string Random(int len)
		{
			string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRST123456789他是说汉语的Ⓟⓡⓞⓣⓔⓒⓣｱ尺Ծｲ乇ζｲ123456789αβγδεζηθικλμνξοπρστυφχψω卍卍卍卍卍卍卍";
			char[] chArray = new char[len - 1 + 1];
			int num = len - 1;
			for (int i = 0; i <= num; i++)
			{
				chArray[i] = str[(int)Math.Round((double)Conversion.Int(VBMath.Rnd() * (float)str.Length))];
			}
			return new string(chArray);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00002194 File Offset: 0x00000394
		public StringProtection()
		{
		}
	}
}
