using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Confuser.Core
{
	/// <summary>
	///     Provides methods to annotate objects.
	/// </summary>
	/// <remarks>
	///     The annotations are stored using <see cref="T:System.WeakReference" />
	/// </remarks>
	// Token: 0x02000027 RID: 39
	public class Annotations
	{
		/// <summary>
		///     Retrieves the annotation on the specified object associated with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="key">The key of annotation.</param>
		/// <param name="defValue">The default value if the specified annotation does not exists on the object.</param>
		/// <returns>The value of annotation, or default value if the annotation does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///     <paramref name="obj" /> or <paramref name="key" /> is <c>null</c>.
		/// </exception>
		// Token: 0x060000D4 RID: 212 RVA: 0x00009688 File Offset: 0x00007888
		public TValue Get<TValue>(object obj, object key, TValue defValue = default(TValue))
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			bool flag2 = key == null;
			if (flag2)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary objAnno;
			bool flag3 = !this.annotations.TryGetValue(obj, out objAnno);
			TValue result;
			if (flag3)
			{
				result = defValue;
			}
			else
			{
				bool flag4 = !objAnno.Contains(key);
				if (flag4)
				{
					result = defValue;
				}
				else
				{
					Type valueType = typeof(TValue);
					bool isValueType = valueType.IsValueType;
					if (isValueType)
					{
						result = (TValue)((object)Convert.ChangeType(objAnno[key], typeof(TValue)));
					}
					else
					{
						result = (TValue)((object)objAnno[key]);
					}
				}
			}
			return result;
		}

		/// <summary>
		///     Retrieves the annotation on the specified object associated with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="key">The key of annotation.</param>
		/// <param name="defValueFactory">The default value factory function.</param>
		/// <returns>The value of annotation, or default value if the annotation does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///     <paramref name="obj" /> or <paramref name="key" /> is <c>null</c>.
		/// </exception>
		// Token: 0x060000D5 RID: 213 RVA: 0x00009738 File Offset: 0x00007938
		public TValue GetLazy<TValue>(object obj, object key, Func<object, TValue> defValueFactory)
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			bool flag2 = key == null;
			if (flag2)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary objAnno;
			bool flag3 = !this.annotations.TryGetValue(obj, out objAnno);
			TValue result;
			if (flag3)
			{
				result = defValueFactory(key);
			}
			else
			{
				bool flag4 = !objAnno.Contains(key);
				if (flag4)
				{
					result = defValueFactory(key);
				}
				else
				{
					Type valueType = typeof(TValue);
					bool isValueType = valueType.IsValueType;
					if (isValueType)
					{
						result = (TValue)((object)Convert.ChangeType(objAnno[key], typeof(TValue)));
					}
					else
					{
						result = (TValue)((object)objAnno[key]);
					}
				}
			}
			return result;
		}

		/// <summary>
		///     Retrieves or create the annotation on the specified object associated with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="key">The key of annotation.</param>
		/// <param name="factory">The factory function to create the annotation value when the annotation does not exist.</param>
		/// <returns>The value of annotation, or the newly created value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///     <paramref name="obj" /> or <paramref name="key" /> is <c>null</c>.
		/// </exception>
		// Token: 0x060000D6 RID: 214 RVA: 0x000097F4 File Offset: 0x000079F4
		public TValue GetOrCreate<TValue>(object obj, object key, Func<object, TValue> factory)
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			bool flag2 = key == null;
			if (flag2)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary objAnno;
			bool flag3 = !this.annotations.TryGetValue(obj, out objAnno);
			if (flag3)
			{
				objAnno = (this.annotations[new Annotations.WeakReferenceKey(obj)] = new ListDictionary());
			}
			bool flag4 = objAnno.Contains(key);
			TValue result;
			if (flag4)
			{
				Type valueType = typeof(TValue);
				bool isValueType = valueType.IsValueType;
				if (isValueType)
				{
					result = (TValue)((object)Convert.ChangeType(objAnno[key], typeof(TValue)));
				}
				else
				{
					result = (TValue)((object)objAnno[key]);
				}
			}
			else
			{
				TValue ret;
				objAnno[key] = (ret = factory(key));
				result = ret;
			}
			return result;
		}

		/// <summary>
		///     Sets an annotation on the specified object.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="key">The key of annotation.</param>
		/// <param name="value">The value of annotation.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///     <paramref name="obj" /> or <paramref name="key" /> is <c>null</c>.
		/// </exception>
		// Token: 0x060000D7 RID: 215 RVA: 0x000098D0 File Offset: 0x00007AD0
		public void Set<TValue>(object obj, object key, TValue value)
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			bool flag2 = key == null;
			if (flag2)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary objAnno;
			bool flag3 = !this.annotations.TryGetValue(obj, out objAnno);
			if (flag3)
			{
				objAnno = (this.annotations[new Annotations.WeakReferenceKey(obj)] = new ListDictionary());
			}
			objAnno[key] = value;
		}

		/// <summary>
		///     Trims the annotations of unreachable objects from this instance.
		/// </summary>
		// Token: 0x060000D8 RID: 216 RVA: 0x00009944 File Offset: 0x00007B44
		public void Trim()
		{
			foreach (object key in from kvp in this.annotations
			where !((Annotations.WeakReferenceKey)kvp.Key).IsAlive
			select kvp.Key)
			{
				this.annotations.Remove(key);
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000254A File Offset: 0x0000074A
		public Annotations()
		{
		}

		// Token: 0x040000F0 RID: 240
		private readonly Dictionary<object, ListDictionary> annotations = new Dictionary<object, ListDictionary>(Annotations.WeakReferenceComparer.Instance);

		/// <summary>
		///     Equality comparer of weak references.
		/// </summary>
		// Token: 0x02000028 RID: 40
		private class WeakReferenceComparer : IEqualityComparer<object>
		{
			/// <summary>
			///     Prevents a default instance of the <see cref="T:Confuser.Core.Annotations.WeakReferenceComparer" /> class from being created.
			/// </summary>
			// Token: 0x060000DA RID: 218 RVA: 0x00002563 File Offset: 0x00000763
			private WeakReferenceComparer()
			{
			}

			/// <inheritdoc />
			// Token: 0x060000DB RID: 219 RVA: 0x000099E4 File Offset: 0x00007BE4
			public bool Equals(object x, object y)
			{
				bool flag = y is Annotations.WeakReferenceKey && !(x is WeakReference);
				bool result;
				if (flag)
				{
					result = this.Equals(y, x);
				}
				else
				{
					Annotations.WeakReferenceKey xWeak = x as Annotations.WeakReferenceKey;
					Annotations.WeakReferenceKey yWeak = y as Annotations.WeakReferenceKey;
					bool flag2 = xWeak != null && yWeak != null;
					if (flag2)
					{
						result = (xWeak.IsAlive && yWeak.IsAlive && xWeak.Target == yWeak.Target);
					}
					else
					{
						bool flag3 = xWeak != null && yWeak == null;
						if (flag3)
						{
							result = (xWeak.IsAlive && xWeak.Target == y);
						}
						else
						{
							bool flag4 = xWeak == null && yWeak == null;
							if (!flag4)
							{
								throw new UnreachableException();
							}
							result = (xWeak.IsAlive && xWeak.Target == y);
						}
					}
				}
				return result;
			}

			/// <inheritdoc />
			// Token: 0x060000DC RID: 220 RVA: 0x00009AB8 File Offset: 0x00007CB8
			public int GetHashCode(object obj)
			{
				bool flag = obj is Annotations.WeakReferenceKey;
				int hashCode;
				if (flag)
				{
					hashCode = ((Annotations.WeakReferenceKey)obj).HashCode;
				}
				else
				{
					hashCode = obj.GetHashCode();
				}
				return hashCode;
			}

			// Token: 0x060000DD RID: 221 RVA: 0x0000256D File Offset: 0x0000076D
			// Note: this type is marked as 'beforefieldinit'.
			static WeakReferenceComparer()
			{
			}

			/// <summary>
			///     The singleton instance of this comparer.
			/// </summary>
			// Token: 0x040000F1 RID: 241
			public static readonly Annotations.WeakReferenceComparer Instance = new Annotations.WeakReferenceComparer();
		}

		/// <summary>
		///     Represent a key using <see cref="T:System.WeakReference" />.
		/// </summary>
		// Token: 0x02000029 RID: 41
		private class WeakReferenceKey : WeakReference
		{
			/// <inheritdoc />
			// Token: 0x060000DE RID: 222 RVA: 0x00002579 File Offset: 0x00000779
			public WeakReferenceKey(object target) : base(target)
			{
				this.HashCode = target.GetHashCode();
			}

			/// <summary>
			///     Gets the hash code of the target object.
			/// </summary>
			/// <value>The hash code.</value>
			// Token: 0x17000001 RID: 1
			// (get) Token: 0x060000DF RID: 223 RVA: 0x00002591 File Offset: 0x00000791
			// (set) Token: 0x060000E0 RID: 224 RVA: 0x00002599 File Offset: 0x00000799
			public int HashCode
			{
				[CompilerGenerated]
				get
				{
					return this.<HashCode>k__BackingField;
				}
				[CompilerGenerated]
				private set
				{
					this.<HashCode>k__BackingField = value;
				}
			}

			// Token: 0x040000F2 RID: 242
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[CompilerGenerated]
			private int <HashCode>k__BackingField;
		}

		// Token: 0x0200002A RID: 42
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060000E1 RID: 225 RVA: 0x000025A2 File Offset: 0x000007A2
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060000E2 RID: 226 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x060000E3 RID: 227 RVA: 0x000025AE File Offset: 0x000007AE
			internal bool <Trim>b__5_0(KeyValuePair<object, ListDictionary> kvp)
			{
				return !((Annotations.WeakReferenceKey)kvp.Key).IsAlive;
			}

			// Token: 0x060000E4 RID: 228 RVA: 0x000025C4 File Offset: 0x000007C4
			internal object <Trim>b__5_1(KeyValuePair<object, ListDictionary> kvp)
			{
				return kvp.Key;
			}

			// Token: 0x040000F3 RID: 243
			public static readonly Annotations.<>c <>9 = new Annotations.<>c();

			// Token: 0x040000F4 RID: 244
			public static Func<KeyValuePair<object, ListDictionary>, bool> <>9__5_0;

			// Token: 0x040000F5 RID: 245
			public static Func<KeyValuePair<object, ListDictionary>, object> <>9__5_1;
		}
	}
}
