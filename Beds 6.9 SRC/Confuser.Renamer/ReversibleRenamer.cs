using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Confuser.Renamer
{
	// Token: 0x02000011 RID: 17
	public class ReversibleRenamer
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00005818 File Offset: 0x00003A18
		public ReversibleRenamer(string password)
		{
			this.cipher = new RijndaelManaged();
			using (SHA256 sha = SHA256.Create())
			{
				this.cipher.Key = (this.key = sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005884 File Offset: 0x00003A84
		private static string Base64Encode(byte[] buf)
		{
			return Convert.ToBase64String(buf).Trim(new char[]
			{
				'='
			}).Replace('+', '$').Replace('/', '_');
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000058C0 File Offset: 0x00003AC0
		private static byte[] Base64Decode(string str)
		{
			str = str.Replace('$', '+').Replace('_', '/').PadRight(str.Length + 3 & -4, '=');
			return Convert.FromBase64String(str);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00005900 File Offset: 0x00003B00
		private byte[] GetIV(byte ivId)
		{
			byte[] iv = new byte[this.cipher.BlockSize / 8];
			for (int i = 0; i < iv.Length; i++)
			{
				iv[i] = (ivId ^ this.key[i]);
			}
			return iv;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005948 File Offset: 0x00003B48
		private byte GetIVId(string str)
		{
			byte x = (byte)str[0];
			for (int i = 1; i < str.Length; i++)
			{
				x = x * 3 + (byte)str[i];
			}
			return x;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00005988 File Offset: 0x00003B88
		public string Encrypt(string name)
		{
			byte ivId = this.GetIVId(name);
			this.cipher.IV = this.GetIV(ivId);
			byte[] buf = Encoding.UTF8.GetBytes(name);
			string result;
			using (MemoryStream ms = new MemoryStream())
			{
				ms.WriteByte(ivId);
				using (CryptoStream stream = new CryptoStream(ms, this.cipher.CreateEncryptor(), CryptoStreamMode.Write))
				{
					stream.Write(buf, 0, buf.Length);
				}
				buf = ms.ToArray();
				result = ReversibleRenamer.Base64Encode(buf);
			}
			return result;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00005A34 File Offset: 0x00003C34
		public string Decrypt(string name)
		{
			string @string;
			using (MemoryStream ms = new MemoryStream(ReversibleRenamer.Base64Decode(name)))
			{
				byte ivId = (byte)ms.ReadByte();
				this.cipher.IV = this.GetIV(ivId);
				MemoryStream result = new MemoryStream();
				using (CryptoStream stream = new CryptoStream(ms, this.cipher.CreateDecryptor(), CryptoStreamMode.Read))
				{
					stream.CopyTo(result);
				}
				@string = Encoding.UTF8.GetString(result.ToArray());
			}
			return @string;
		}

		// Token: 0x04000026 RID: 38
		private RijndaelManaged cipher;

		// Token: 0x04000027 RID: 39
		private byte[] key;
	}
}
