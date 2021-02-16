using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Confuser.Core.Services
{
	// Token: 0x0200007B RID: 123
	public class RandomGenerator
	{
		// Token: 0x060002CC RID: 716 RVA: 0x00003374 File Offset: 0x00001574
		internal RandomGenerator(byte[] seed)
		{
			this.state = (byte[])seed.Clone();
			this.stateFilled = 32;
			this.mixIndex = 0;
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00012688 File Offset: 0x00010888
		public bool NextBoolean()
		{
			byte s = this.state[32 - this.stateFilled];
			this.stateFilled--;
			bool flag = this.stateFilled == 0;
			if (flag)
			{
				this.NextState();
			}
			return s % 2 == 0;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x000126D8 File Offset: 0x000108D8
		public byte NextByte()
		{
			byte ret = this.state[32 - this.stateFilled];
			this.stateFilled--;
			bool flag = this.stateFilled == 0;
			if (flag)
			{
				this.NextState();
			}
			return ret;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00012720 File Offset: 0x00010920
		public byte[] NextBytes(int length)
		{
			byte[] ret = new byte[length];
			this.NextBytes(ret, 0, length);
			return ret;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00012744 File Offset: 0x00010944
		public void NextBytes(byte[] buffer, int offset, int length)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			bool flag3 = length < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			bool flag4 = buffer.Length - offset < length;
			if (flag4)
			{
				throw new ArgumentException("Invalid offset or length.");
			}
			while (length > 0)
			{
				bool flag5 = length >= this.stateFilled;
				if (flag5)
				{
					Buffer.BlockCopy(this.state, 32 - this.stateFilled, buffer, offset, this.stateFilled);
					offset += this.stateFilled;
					length -= this.stateFilled;
					this.stateFilled = 0;
				}
				else
				{
					Buffer.BlockCopy(this.state, 32 - this.stateFilled, buffer, offset, length);
					this.stateFilled -= length;
					length = 0;
				}
				bool flag6 = this.stateFilled == 0;
				if (flag6)
				{
					this.NextState();
				}
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0001284C File Offset: 0x00010A4C
		public double NextDouble()
		{
			return this.NextUInt32() / 4294967296.0;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00012870 File Offset: 0x00010A70
		public int NextInt32()
		{
			return BitConverter.ToInt32(this.NextBytes(4), 0);
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00012890 File Offset: 0x00010A90
		public int NextInt32(int max)
		{
			return (int)((ulong)this.NextUInt32() % (ulong)((long)max));
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x000128B0 File Offset: 0x00010AB0
		public int NextInt32(int min, int max)
		{
			bool flag = max <= min;
			int result;
			if (flag)
			{
				result = min;
			}
			else
			{
				result = min + (int)((ulong)this.NextUInt32() % (ulong)((long)(max - min)));
			}
			return result;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000128E4 File Offset: 0x00010AE4
		private void NextState()
		{
			for (int i = 0; i < 32; i++)
			{
				byte[] expr_10_cp_0 = this.state;
				int expr_10_cp_ = i;
				byte[] array = expr_10_cp_0;
				int num = expr_10_cp_;
				array[num] ^= RandomGenerator.primes[this.mixIndex = (this.mixIndex + 1) % RandomGenerator.primes.Length];
			}
			this.state = this.sha256.ComputeHash(this.state);
			this.stateFilled = 32;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0001295C File Offset: 0x00010B5C
		public uint NextUInt32()
		{
			Random y = new Random();
			byte[] hi = BitConverter.GetBytes(y.Next(500000, 1000000));
			return BitConverter.ToUInt32(hi, 0);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00012994 File Offset: 0x00010B94
		internal static byte[] Seed(string seed)
		{
			bool flag = !string.IsNullOrEmpty(seed);
			byte[] ret;
			if (flag)
			{
				ret = Utils.SHA256(Encoding.UTF8.GetBytes(seed));
			}
			else
			{
				ret = Utils.SHA256(Guid.NewGuid().ToByteArray());
			}
			for (int i = 0; i < 32; i++)
			{
				byte[] expr_39_cp_0 = ret;
				int expr_39_cp_ = i;
				byte[] array = expr_39_cp_0;
				int num = expr_39_cp_;
				array[num] *= RandomGenerator.primes[i % RandomGenerator.primes.Length];
				ret = Utils.SHA256(ret);
			}
			return ret;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00012A20 File Offset: 0x00010C20
		public void Shuffle<T>(IList<T> list)
		{
			for (int i = list.Count - 1; i > 1; i--)
			{
				int j = this.NextInt32(i + 1);
				T tmp = list[j];
				list[j] = list[i];
				list[i] = tmp;
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x000033A9 File Offset: 0x000015A9
		// Note: this type is marked as 'beforefieldinit'.
		static RandomGenerator()
		{
		}

		// Token: 0x04000221 RID: 545
		private int mixIndex;

		// Token: 0x04000222 RID: 546
		private static readonly byte[] primes = new byte[]
		{
			7,
			11,
			23,
			37,
			43,
			59,
			71
		};

		// Token: 0x04000223 RID: 547
		private readonly SHA256Managed sha256 = new SHA256Managed();

		// Token: 0x04000224 RID: 548
		private byte[] state;

		// Token: 0x04000225 RID: 549
		private int stateFilled;
	}
}
