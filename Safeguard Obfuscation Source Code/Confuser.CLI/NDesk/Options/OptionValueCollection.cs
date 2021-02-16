using System;
using System.Collections;
using System.Collections.Generic;

namespace NDesk.Options
{
	// Token: 0x02000003 RID: 3
	public class OptionValueCollection : IList, ICollection, IEnumerable, IList<string>, ICollection<string>, IEnumerable<string>
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002651 File Offset: 0x00000851
		internal OptionValueCollection(OptionContext c)
		{
			this.c = c;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000266D File Offset: 0x0000086D
		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.values).CopyTo(array, index);
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002680 File Offset: 0x00000880
		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)this.values).IsSynchronized;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000026A0 File Offset: 0x000008A0
		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this.values).SyncRoot;
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000026BD File Offset: 0x000008BD
		public void Add(string item)
		{
			this.values.Add(item);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000026CD File Offset: 0x000008CD
		public void Clear()
		{
			this.values.Clear();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000026DC File Offset: 0x000008DC
		public bool Contains(string item)
		{
			return this.values.Contains(item);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000026FA File Offset: 0x000008FA
		public void CopyTo(string[] array, int arrayIndex)
		{
			this.values.CopyTo(array, arrayIndex);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000270C File Offset: 0x0000090C
		public bool Remove(string item)
		{
			return this.values.Remove(item);
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000272C File Offset: 0x0000092C
		public int Count
		{
			get
			{
				return this.values.Count;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000274C File Offset: 0x0000094C
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002760 File Offset: 0x00000960
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.values.GetEnumerator();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002784 File Offset: 0x00000984
		public IEnumerator<string> GetEnumerator()
		{
			return this.values.GetEnumerator();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000027A8 File Offset: 0x000009A8
		int IList.Add(object value)
		{
			return ((IList)this.values).Add(value);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000027C8 File Offset: 0x000009C8
		bool IList.Contains(object value)
		{
			return ((IList)this.values).Contains(value);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000027E8 File Offset: 0x000009E8
		int IList.IndexOf(object value)
		{
			return ((IList)this.values).IndexOf(value);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002806 File Offset: 0x00000A06
		void IList.Insert(int index, object value)
		{
			((IList)this.values).Insert(index, value);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002817 File Offset: 0x00000A17
		void IList.Remove(object value)
		{
			((IList)this.values).Remove(value);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002827 File Offset: 0x00000A27
		void IList.RemoveAt(int index)
		{
			((IList)this.values).RemoveAt(index);
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002838 File Offset: 0x00000A38
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000006 RID: 6
		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				((IList)this.values)[index] = value;
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002878 File Offset: 0x00000A78
		public int IndexOf(string item)
		{
			return this.values.IndexOf(item);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002896 File Offset: 0x00000A96
		public void Insert(int index, string item)
		{
			this.values.Insert(index, item);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000028A7 File Offset: 0x00000AA7
		public void RemoveAt(int index)
		{
			this.values.RemoveAt(index);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000028B8 File Offset: 0x00000AB8
		private void AssertValid(int index)
		{
			bool flag = this.c.Option == null;
			if (flag)
			{
				throw new InvalidOperationException("OptionContext.Option is null.");
			}
			bool flag2 = index >= this.c.Option.MaxValueCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			bool flag3 = this.c.Option.OptionValueType == OptionValueType.Required && index >= this.values.Count;
			if (flag3)
			{
				throw new OptionException(string.Format(this.c.OptionSet.MessageLocalizer("Missing required value for option '{0}'."), this.c.OptionName), this.c.OptionName);
			}
		}

		// Token: 0x17000007 RID: 7
		public string this[int index]
		{
			get
			{
				this.AssertValid(index);
				return (index >= this.values.Count) ? null : this.values[index];
			}
			set
			{
				this.values[index] = value;
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000029B8 File Offset: 0x00000BB8
		public List<string> ToList()
		{
			return new List<string>(this.values);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000029D8 File Offset: 0x00000BD8
		public string[] ToArray()
		{
			return this.values.ToArray();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000029F8 File Offset: 0x00000BF8
		public override string ToString()
		{
			return string.Join(", ", this.values.ToArray());
		}

		// Token: 0x04000001 RID: 1
		private List<string> values = new List<string>();

		// Token: 0x04000002 RID: 2
		private OptionContext c;
	}
}
