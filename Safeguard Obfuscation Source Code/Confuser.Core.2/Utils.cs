using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Confuser.Core
{
	/// <summary>
	///     Provides a set of utility methods
	/// </summary>
	// Token: 0x02000071 RID: 113
	public static class Utils
	{
		/// <summary>
		///     Gets the value associated with the specified key, or default value if the key does not exists.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="defValue">The default value.</param>
		/// <returns>The value associated with the specified key, or the default value if the key does not exists</returns>
		// Token: 0x060002A2 RID: 674 RVA: 0x00011DE4 File Offset: 0x0000FFE4
		public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defValue = default(TValue))
		{
			TValue ret;
			bool flag = dictionary.TryGetValue(key, out ret);
			TValue result;
			if (flag)
			{
				result = ret;
			}
			else
			{
				result = defValue;
			}
			return result;
		}

		/// <summary>
		///     Gets the value associated with the specified key, or default value if the key does not exists.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="defValueFactory">The default value factory function.</param>
		/// <returns>The value associated with the specified key, or the default value if the key does not exists</returns>
		// Token: 0x060002A3 RID: 675 RVA: 0x00011E08 File Offset: 0x00010008
		public static TValue GetValueOrDefaultLazy<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defValueFactory)
		{
			TValue ret;
			bool flag = dictionary.TryGetValue(key, out ret);
			TValue result;
			if (flag)
			{
				result = ret;
			}
			else
			{
				result = defValueFactory(key);
			}
			return result;
		}

		/// <summary>
		///     Adds the specified key and value to the multi dictionary.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="self">The dictionary to add to.</param>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add.</param>
		/// <exception cref="T:System.ArgumentNullException">key is <c>null</c>.</exception>
		// Token: 0x060002A4 RID: 676 RVA: 0x00011E34 File Offset: 0x00010034
		public static void AddListEntry<TKey, TValue>(this IDictionary<TKey, List<TValue>> self, TKey key, TValue value)
		{
			bool flag = key == null;
			if (flag)
			{
				throw new ArgumentNullException("key");
			}
			List<TValue> list;
			bool flag2 = !self.TryGetValue(key, out list);
			if (flag2)
			{
				list = (self[key] = new List<TValue>());
			}
			list.Add(value);
		}

		/// <summary>
		///     Obtains the relative path from the specified base path.
		/// </summary>
		/// <param name="filespec">The file path.</param>
		/// <param name="folder">The base path.</param>
		/// <returns>The path of <paramref name="filespec" /> relative to <paramref name="folder" />.</returns>
		// Token: 0x060002A5 RID: 677 RVA: 0x00011E84 File Offset: 0x00010084
		public static string GetRelativePath(string filespec, string folder)
		{
			Uri pathUri = new Uri(filespec);
			bool flag = !folder.EndsWith(Path.DirectorySeparatorChar.ToString());
			if (flag)
			{
				folder += Path.DirectorySeparatorChar.ToString();
			}
			Uri folderUri = new Uri(folder);
			return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}

		/// <summary>
		///     If the input string is empty, return null; otherwise, return the original input string.
		/// </summary>
		/// <param name="val">The input string.</param>
		/// <returns><c>null</c> if the input string is empty; otherwise, the original input string.</returns>
		// Token: 0x060002A6 RID: 678 RVA: 0x00011EF4 File Offset: 0x000100F4
		public static string NullIfEmpty(this string val)
		{
			bool flag = string.IsNullOrEmpty(val);
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = val;
			}
			return result;
		}

		/// <summary>
		///     Compute the SHA1 hash of the input buffer.
		/// </summary>
		/// <param name="buffer">The input buffer.</param>
		/// <returns>The SHA1 hash of the input buffer.</returns>
		// Token: 0x060002A7 RID: 679 RVA: 0x00011F18 File Offset: 0x00010118
		public static byte[] SHA1(byte[] buffer)
		{
			SHA1Managed sha = new SHA1Managed();
			return sha.ComputeHash(buffer);
		}

		/// <summary>
		///     Xor the values in the two buffer together.
		/// </summary>
		/// <param name="buffer1">The input buffer 1.</param>
		/// <param name="buffer2">The input buffer 2.</param>
		/// <returns>The result buffer.</returns>
		/// <exception cref="T:System.ArgumentException">Length of the two buffers are not equal.</exception>
		// Token: 0x060002A8 RID: 680 RVA: 0x00011F38 File Offset: 0x00010138
		public static byte[] Xor(byte[] buffer1, byte[] buffer2)
		{
			bool flag = buffer1.Length != buffer2.Length;
			if (flag)
			{
				throw new ArgumentException("Length mismatched.");
			}
			byte[] ret = new byte[buffer1.Length];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = (buffer1[i] ^ buffer2[i]);
			}
			return ret;
		}

		/// <summary>
		///     Compute the SHA256 hash of the input buffer.
		/// </summary>
		/// <param name="buffer">The input buffer.</param>
		/// <returns>The SHA256 hash of the input buffer.</returns>
		// Token: 0x060002A9 RID: 681 RVA: 0x00011F90 File Offset: 0x00010190
		public static byte[] SHA256(byte[] buffer)
		{
			SHA256Managed sha = new SHA256Managed();
			return sha.ComputeHash(buffer);
		}

		/// <summary>
		///     Encoding the buffer to a string using specified charset.
		/// </summary>
		/// <param name="buff">The input buffer.</param>
		/// <param name="charset">The charset.</param>
		/// <returns>The encoded string.</returns>
		// Token: 0x060002AA RID: 682 RVA: 0x00011FB0 File Offset: 0x000101B0
		public static string EncodeString(byte[] buff, char[] charset)
		{
			int current = (int)buff[0];
			StringBuilder ret = new StringBuilder();
			for (int i = 1; i < buff.Length; i++)
			{
				for (current = (current << 8) + (int)buff[i]; current >= charset.Length; current /= charset.Length)
				{
					ret.Append(charset[current % charset.Length]);
				}
			}
			bool flag = current != 0;
			if (flag)
			{
				ret.Append(charset[current % charset.Length]);
			}
			return ret.ToString();
		}

		/// <summary>
		///     Returns a new string in which all occurrences of a specified string in
		///     <paramref name="str" /><paramref name="str" /> are replaced with another specified string.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.String" /> equivalent to <paramref name="str" /> but with all instances of
		///     <paramref name="oldValue" />
		///     replaced with <paramref name="newValue" />.
		/// </returns>
		/// <param name="str">A string to do the replace in. </param>
		/// <param name="oldValue">A string to be replaced. </param>
		/// <param name="newValue">A string to replace all occurrences of <paramref name="oldValue" />. </param>
		/// <param name="comparison">One of the <see cref="T:System.StringComparison" /> values. </param>
		/// <remarks>Adopted from http://stackoverflow.com/a/244933 </remarks>
		// Token: 0x060002AB RID: 683 RVA: 0x00012030 File Offset: 0x00010230
		public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
		{
			StringBuilder sb = new StringBuilder();
			int previousIndex = 0;
			for (int index = str.IndexOf(oldValue, comparison); index != -1; index = str.IndexOf(oldValue, index, comparison))
			{
				sb.Append(str.Substring(previousIndex, index - previousIndex));
				sb.Append(newValue);
				index += oldValue.Length;
				previousIndex = index;
			}
			sb.Append(str.Substring(previousIndex));
			return sb.ToString();
		}

		/// <summary>
		///     Encode the buffer to a hexadecimal string.
		/// </summary>
		/// <param name="buff">The input buffer.</param>
		/// <returns>A hexadecimal representation of input buffer.</returns>
		// Token: 0x060002AC RID: 684 RVA: 0x000120A8 File Offset: 0x000102A8
		public static string ToHexString(byte[] buff)
		{
			char[] ret = new char[buff.Length * 2];
			int i = 0;
			foreach (byte val in buff)
			{
				ret[i++] = Utils.hexCharset[val >> 4];
				ret[i++] = Utils.hexCharset[(int)(val & 15)];
			}
			return new string(ret);
		}

		/// <summary>
		///     Removes all elements that match the conditions defined by the specified predicate from a the list.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="self" />.</typeparam>
		/// <param name="self">The list to remove from.</param>
		/// <param name="match">The predicate that defines the conditions of the elements to remove.</param>
		/// <returns><paramref name="self" /> for method chaining.</returns>
		// Token: 0x060002AD RID: 685 RVA: 0x0001210C File Offset: 0x0001030C
		public static IList<T> RemoveWhere<T>(this IList<T> self, Predicate<T> match)
		{
			for (int i = self.Count - 1; i >= 0; i--)
			{
				bool flag = match(self[i]);
				if (flag)
				{
					self.RemoveAt(i);
				}
			}
			return self;
		}

		/// <summary>
		///     Returns a <see cref="T:System.Collections.Generic.IEnumerable`1" /> that log the progress of iterating the specified list.
		/// </summary>
		/// <typeparam name="T">The type of list element</typeparam>
		/// <param name="enumerable">The list.</param>
		/// <param name="logger">The logger.</param>
		/// <returns>A wrapper of the list.</returns>
		// Token: 0x060002AE RID: 686 RVA: 0x000032B6 File Offset: 0x000014B6
		public static IEnumerable<T> WithProgress<T>(this IEnumerable<T> enumerable, ILogger logger)
		{
			List<T> list = new List<T>(enumerable);
			int i;
			int num;
			for (i = 0; i < list.Count; i = num + 1)
			{
				logger.Progress(i, list.Count);
				yield return list[i];
				num = i;
			}
			logger.Progress(i, list.Count);
			logger.EndProgress();
			yield break;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x000032CD File Offset: 0x000014CD
		// Note: this type is marked as 'beforefieldinit'.
		static Utils()
		{
		}

		// Token: 0x0400020F RID: 527
		private static readonly char[] hexCharset = "0123456789abcdef".ToCharArray();

		// Token: 0x02000072 RID: 114
		[CompilerGenerated]
		private sealed class <WithProgress>d__13<T> : IEnumerable<T>, IEnumerator<T>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x060002B0 RID: 688 RVA: 0x000032DE File Offset: 0x000014DE
			[DebuggerHidden]
			public <WithProgress>d__13(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x060002B1 RID: 689 RVA: 0x0000280B File Offset: 0x00000A0B
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x060002B2 RID: 690 RVA: 0x00012154 File Offset: 0x00010354
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
					int num2 = i;
					i = num2 + 1;
				}
				else
				{
					this.<>1__state = -1;
					list = new List<T>(enumerable);
					i = 0;
				}
				if (i >= list.Count)
				{
					logger.Progress(i, list.Count);
					logger.EndProgress();
					return false;
				}
				logger.Progress(i, list.Count);
				this.<>2__current = list[i];
				this.<>1__state = 1;
				return true;
			}

			// Token: 0x17000049 RID: 73
			// (get) Token: 0x060002B3 RID: 691 RVA: 0x000032FE File Offset: 0x000014FE
			T IEnumerator<!0>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060002B4 RID: 692 RVA: 0x0000268B File Offset: 0x0000088B
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x1700004A RID: 74
			// (get) Token: 0x060002B5 RID: 693 RVA: 0x00003306 File Offset: 0x00001506
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060002B6 RID: 694 RVA: 0x00012234 File Offset: 0x00010434
			[DebuggerHidden]
			IEnumerator<T> IEnumerable<!0>.GetEnumerator()
			{
				Utils.<WithProgress>d__13<T> <WithProgress>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<WithProgress>d__ = this;
				}
				else
				{
					<WithProgress>d__ = new Utils.<WithProgress>d__13<T>(0);
				}
				<WithProgress>d__.enumerable = enumerable;
				<WithProgress>d__.logger = logger;
				return <WithProgress>d__;
			}

			// Token: 0x060002B7 RID: 695 RVA: 0x00003313 File Offset: 0x00001513
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();
			}

			// Token: 0x04000210 RID: 528
			private int <>1__state;

			// Token: 0x04000211 RID: 529
			private T <>2__current;

			// Token: 0x04000212 RID: 530
			private int <>l__initialThreadId;

			// Token: 0x04000213 RID: 531
			private IEnumerable<T> enumerable;

			// Token: 0x04000214 RID: 532
			public IEnumerable<T> <>3__enumerable;

			// Token: 0x04000215 RID: 533
			private ILogger logger;

			// Token: 0x04000216 RID: 534
			public ILogger <>3__logger;

			// Token: 0x04000217 RID: 535
			private List<T> <list>5__1;

			// Token: 0x04000218 RID: 536
			private int <i>5__2;
		}
	}
}
