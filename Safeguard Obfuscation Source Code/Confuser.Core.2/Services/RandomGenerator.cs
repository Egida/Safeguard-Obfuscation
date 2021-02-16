using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Confuser.Core.Services
{
	/// <summary>
	///     A seeded SHA256 PRNG.
	/// </summary>
	// Token: 0x0200007B RID: 123
	public class RandomGenerator
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.Services.RandomGenerator" /> class.
		/// </summary>
		/// <param name="seed">The seed.</param>
		// Token: 0x060002CC RID: 716 RVA: 0x00003374 File Offset: 0x00001574
		internal RandomGenerator(byte[] seed)
		{
			this.state = (byte[])seed.Clone();
			this.stateFilled = 32;
			this.mixIndex = 0;
		}

		/// <summary>
		///     Returns a random boolean value.
		/// </summary>
		/// <returns>Requested random boolean value.</returns>
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

		/// <summary>
		///     Returns a random byte.
		/// </summary>
		/// <returns>Requested random byte.</returns>
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

		/// <summary>
		///     Gets a buffer of random bytes with the specified length.
		/// </summary>
		/// <param name="length">The number of random bytes.</param>
		/// <returns>A buffer of random bytes.</returns>
		// Token: 0x060002CF RID: 719 RVA: 0x00012720 File Offset: 0x00010920
		public byte[] NextBytes(int length)
		{
			byte[] ret = new byte[length];
			this.NextBytes(ret, 0, length);
			return ret;
		}

		/// <summary>
		///     Fills the specified buffer with random bytes.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="offset">The offset of buffer to fill in.</param>
		/// <param name="length">The number of random bytes.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///     <paramref name="offset" /> or <paramref name="length" /> is less than 0.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">Invalid <paramref name="offset" /> or <paramref name="length" />.</exception>
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

		/// <summary>
		///     Returns a random double floating pointer number from 0 (inclusive) to 1 (exclusive).
		/// </summary>
		/// <returns>Requested random number.</returns>
		// Token: 0x060002D1 RID: 721 RVA: 0x0001284C File Offset: 0x00010A4C
		public double NextDouble()
		{
			return this.NextUInt32() / 4294967296.0;
		}

		/// <summary>
		///     Returns a random signed integer.
		/// </summary>
		/// <returns>Requested random number.</returns>
		// Token: 0x060002D2 RID: 722 RVA: 0x00012870 File Offset: 0x00010A70
		public int NextInt32()
		{
			return BitConverter.ToInt32(this.NextBytes(4), 0);
		}

		/// <summary>
		///     Returns a nonnegative random integer that is less than the specified maximum.
		/// </summary>
		/// <param name="max">The exclusive upper bound.</param>
		/// <returns>Requested random number.</returns>
		// Token: 0x060002D3 RID: 723 RVA: 0x00012890 File Offset: 0x00010A90
		public int NextInt32(int max)
		{
			return (int)((ulong)this.NextUInt32() % (ulong)((long)max));
		}

		/// <summary>
		///     Returns a random integer that is within a specified range.
		/// </summary>
		/// <param name="min">The inclusive lower bound.</param>
		/// <param name="max">The exclusive upper bound.</param>
		/// <returns>Requested random number.</returns>
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

		/// <summary>
		///     Refills the state buffer.
		/// </summary>
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

		/// <summary>
		///     Returns a random unsigned integer.
		/// </summary>
		/// <returns>Requested random number.</returns>
		// Token: 0x060002D6 RID: 726 RVA: 0x0001295C File Offset: 0x00010B5C
		public uint NextUInt32()
		{
			Random y = new Random();
			byte[] hi = BitConverter.GetBytes(y.Next(500000, 1000000));
			return BitConverter.ToUInt32(hi, 0);
		}

		/// <summary>
		///     Creates a seed buffer.
		/// </summary>
		/// <param name="seed">The seed data.</param>
		/// <returns>The seed buffer.</returns>
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

		/// <summary>
		///     Shuffles the element in the specified list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list to shuffle.</param>
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

		/// <summary>
		///     The prime numbers used for generation
		/// </summary>
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
