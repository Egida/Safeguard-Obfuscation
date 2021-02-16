using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NDesk.Options
{
	// Token: 0x02000006 RID: 6
	public abstract class Option
	{
		// Token: 0x06000031 RID: 49 RVA: 0x00002AD8 File Offset: 0x00000CD8
		protected Option(string prototype, string description) : this(prototype, description, 1)
		{
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002AE8 File Offset: 0x00000CE8
		protected Option(string prototype, string description, int maxValueCount)
		{
			bool flag = prototype == null;
			if (flag)
			{
				throw new ArgumentNullException("prototype");
			}
			bool flag2 = prototype.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Cannot be the empty string.", "prototype");
			}
			bool flag3 = maxValueCount < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("maxValueCount");
			}
			this.prototype = prototype;
			this.names = prototype.Split(new char[]
			{
				'|'
			});
			this.description = description;
			this.count = maxValueCount;
			this.type = this.ParsePrototype();
			bool flag4 = this.count == 0 && this.type > OptionValueType.None;
			if (flag4)
			{
				throw new ArgumentException("Cannot provide maxValueCount of 0 for OptionValueType.Required or OptionValueType.Optional.", "maxValueCount");
			}
			bool flag5 = this.type == OptionValueType.None && maxValueCount > 1;
			if (flag5)
			{
				throw new ArgumentException(string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount), "maxValueCount");
			}
			bool flag6 = Array.IndexOf<string>(this.names, "<>") >= 0 && ((this.names.Length == 1 && this.type != OptionValueType.None) || (this.names.Length > 1 && this.MaxValueCount > 1));
			if (flag6)
			{
				throw new ArgumentException("The default option handler '<>' cannot require values.", "prototype");
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002C2C File Offset: 0x00000E2C
		public string Prototype
		{
			get
			{
				return this.prototype;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002C44 File Offset: 0x00000E44
		public string Description
		{
			get
			{
				return this.description;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002C5C File Offset: 0x00000E5C
		public OptionValueType OptionValueType
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002C74 File Offset: 0x00000E74
		public int MaxValueCount
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002C8C File Offset: 0x00000E8C
		public string[] GetNames()
		{
			return (string[])this.names.Clone();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002CB0 File Offset: 0x00000EB0
		public string[] GetValueSeparators()
		{
			bool flag = this.separators == null;
			string[] result;
			if (flag)
			{
				result = new string[0];
			}
			else
			{
				result = (string[])this.separators.Clone();
			}
			return result;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002CE8 File Offset: 0x00000EE8
		protected static T Parse<T>(string value, OptionContext c)
		{
			TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
			T t = default(T);
			try
			{
				bool flag = value != null;
				if (flag)
				{
					t = (T)((object)conv.ConvertFromString(value));
				}
			}
			catch (Exception e)
			{
				throw new OptionException(string.Format(c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."), value, typeof(T).Name, c.OptionName), c.OptionName, e);
			}
			return t;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002D80 File Offset: 0x00000F80
		internal string[] Names
		{
			get
			{
				return this.names;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002D98 File Offset: 0x00000F98
		internal string[] ValueSeparators
		{
			get
			{
				return this.separators;
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002DB0 File Offset: 0x00000FB0
		private OptionValueType ParsePrototype()
		{
			char type = '\0';
			List<string> seps = new List<string>();
			for (int i = 0; i < this.names.Length; i++)
			{
				string name = this.names[i];
				bool flag = name.Length == 0;
				if (flag)
				{
					throw new ArgumentException("Empty option names are not supported.", "prototype");
				}
				int end = name.IndexOfAny(Option.NameTerminator);
				bool flag2 = end == -1;
				if (!flag2)
				{
					this.names[i] = name.Substring(0, end);
					bool flag3 = type == '\0' || type == name[end];
					if (!flag3)
					{
						throw new ArgumentException(string.Format("Conflicting option types: '{0}' vs. '{1}'.", type, name[end]), "prototype");
					}
					type = name[end];
					Option.AddSeparators(name, end, seps);
				}
			}
			bool flag4 = type == '\0';
			OptionValueType result;
			if (flag4)
			{
				result = OptionValueType.None;
			}
			else
			{
				bool flag5 = this.count <= 1 && seps.Count != 0;
				if (flag5)
				{
					throw new ArgumentException(string.Format("Cannot provide key/value separators for Options taking {0} value(s).", this.count), "prototype");
				}
				bool flag6 = this.count > 1;
				if (flag6)
				{
					bool flag7 = seps.Count == 0;
					if (flag7)
					{
						this.separators = new string[]
						{
							":",
							"="
						};
					}
					else
					{
						bool flag8 = seps.Count == 1 && seps[0].Length == 0;
						if (flag8)
						{
							this.separators = null;
						}
						else
						{
							this.separators = seps.ToArray();
						}
					}
				}
				result = ((type == '=') ? OptionValueType.Required : OptionValueType.Optional);
			}
			return result;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002F60 File Offset: 0x00001160
		private static void AddSeparators(string name, int end, ICollection<string> seps)
		{
			int start = -1;
			for (int i = end + 1; i < name.Length; i++)
			{
				char c = name[i];
				if (c != '{')
				{
					if (c != '}')
					{
						bool flag = start == -1;
						if (flag)
						{
							seps.Add(name[i].ToString());
						}
					}
					else
					{
						bool flag2 = start == -1;
						if (flag2)
						{
							throw new ArgumentException(string.Format("Ill-formed name/value separator found in \"{0}\".", name), "prototype");
						}
						seps.Add(name.Substring(start, i - start));
						start = -1;
					}
				}
				else
				{
					bool flag3 = start != -1;
					if (flag3)
					{
						throw new ArgumentException(string.Format("Ill-formed name/value separator found in \"{0}\".", name), "prototype");
					}
					start = i + 1;
				}
			}
			bool flag4 = start != -1;
			if (flag4)
			{
				throw new ArgumentException(string.Format("Ill-formed name/value separator found in \"{0}\".", name), "prototype");
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003049 File Offset: 0x00001249
		public void Invoke(OptionContext c)
		{
			this.OnParseComplete(c);
			c.OptionName = null;
			c.Option = null;
			c.OptionValues.Clear();
		}

		// Token: 0x0600003F RID: 63
		protected abstract void OnParseComplete(OptionContext c);

		// Token: 0x06000040 RID: 64 RVA: 0x00003070 File Offset: 0x00001270
		public override string ToString()
		{
			return this.Prototype;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003088 File Offset: 0x00001288
		// Note: this type is marked as 'beforefieldinit'.
		static Option()
		{
		}

		// Token: 0x0400000C RID: 12
		private string prototype;

		// Token: 0x0400000D RID: 13
		private string description;

		// Token: 0x0400000E RID: 14
		private string[] names;

		// Token: 0x0400000F RID: 15
		private OptionValueType type;

		// Token: 0x04000010 RID: 16
		private int count;

		// Token: 0x04000011 RID: 17
		private string[] separators;

		// Token: 0x04000012 RID: 18
		private static readonly char[] NameTerminator = new char[]
		{
			'=',
			':'
		};
	}
}
