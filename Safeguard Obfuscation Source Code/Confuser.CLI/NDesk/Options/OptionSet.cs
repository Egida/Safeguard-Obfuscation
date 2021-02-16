using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NDesk.Options
{
	// Token: 0x02000009 RID: 9
	public class OptionSet : KeyedCollection<string, Option>
	{
		// Token: 0x0600004C RID: 76 RVA: 0x00003122 File Offset: 0x00001322
		public OptionSet() : this((string f) => f)
		{
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000314B File Offset: 0x0000134B
		public OptionSet(Converter<string, string> localizer)
		{
			this.localizer = localizer;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600004E RID: 78 RVA: 0x0000316C File Offset: 0x0000136C
		public Converter<string, string> MessageLocalizer
		{
			get
			{
				return this.localizer;
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003184 File Offset: 0x00001384
		protected override string GetKeyForItem(Option item)
		{
			bool flag = item == null;
			if (flag)
			{
				throw new ArgumentNullException("option");
			}
			bool flag2 = item.Names != null && item.Names.Length != 0;
			if (flag2)
			{
				return item.Names[0];
			}
			throw new InvalidOperationException("Option has no names!");
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000031D8 File Offset: 0x000013D8
		[Obsolete("Use KeyedCollection.this[string]")]
		protected Option GetOptionForName(string option)
		{
			bool flag = option == null;
			if (flag)
			{
				throw new ArgumentNullException("option");
			}
			Option result;
			try
			{
				result = base[option];
			}
			catch (KeyNotFoundException)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000321C File Offset: 0x0000141C
		protected override void InsertItem(int index, Option item)
		{
			base.InsertItem(index, item);
			this.AddImpl(item);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003230 File Offset: 0x00001430
		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
			Option p = base.Items[index];
			for (int i = 1; i < p.Names.Length; i++)
			{
				base.Dictionary.Remove(p.Names[i]);
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003280 File Offset: 0x00001480
		protected override void SetItem(int index, Option item)
		{
			base.SetItem(index, item);
			this.RemoveItem(index);
			this.AddImpl(item);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x0000329C File Offset: 0x0000149C
		private void AddImpl(Option option)
		{
			bool flag = option == null;
			if (flag)
			{
				throw new ArgumentNullException("option");
			}
			List<string> added = new List<string>(option.Names.Length);
			try
			{
				for (int i = 1; i < option.Names.Length; i++)
				{
					base.Dictionary.Add(option.Names[i], option);
					added.Add(option.Names[i]);
				}
			}
			catch (Exception)
			{
				foreach (string name in added)
				{
					base.Dictionary.Remove(name);
				}
				throw;
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003368 File Offset: 0x00001568
		public new OptionSet Add(Option option)
		{
			base.Add(option);
			return this;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003384 File Offset: 0x00001584
		public OptionSet Add(string prototype, Action<string> action)
		{
			return this.Add(prototype, null, action);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000033A0 File Offset: 0x000015A0
		public OptionSet Add(string prototype, string description, Action<string> action)
		{
			bool flag = action == null;
			if (flag)
			{
				throw new ArgumentNullException("action");
			}
			Option p = new OptionSet.ActionOption(prototype, description, 1, delegate(OptionValueCollection v)
			{
				action(v[0]);
			});
			base.Add(p);
			return this;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000033F8 File Offset: 0x000015F8
		public OptionSet Add(string prototype, OptionAction<string, string> action)
		{
			return this.Add(prototype, null, action);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003414 File Offset: 0x00001614
		public OptionSet Add(string prototype, string description, OptionAction<string, string> action)
		{
			bool flag = action == null;
			if (flag)
			{
				throw new ArgumentNullException("action");
			}
			Option p = new OptionSet.ActionOption(prototype, description, 2, delegate(OptionValueCollection v)
			{
				action(v[0], v[1]);
			});
			base.Add(p);
			return this;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000346C File Offset: 0x0000166C
		public OptionSet Add<T>(string prototype, Action<T> action)
		{
			return this.Add<T>(prototype, null, action);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003488 File Offset: 0x00001688
		public OptionSet Add<T>(string prototype, string description, Action<T> action)
		{
			return this.Add(new OptionSet.ActionOption<T>(prototype, description, action));
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000034A8 File Offset: 0x000016A8
		public OptionSet Add<TKey, TValue>(string prototype, OptionAction<TKey, TValue> action)
		{
			return this.Add<TKey, TValue>(prototype, null, action);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000034C4 File Offset: 0x000016C4
		public OptionSet Add<TKey, TValue>(string prototype, string description, OptionAction<TKey, TValue> action)
		{
			return this.Add(new OptionSet.ActionOption<TKey, TValue>(prototype, description, action));
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000034E4 File Offset: 0x000016E4
		protected virtual OptionContext CreateOptionContext()
		{
			return new OptionContext(this);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000034FC File Offset: 0x000016FC
		public List<string> Parse(IEnumerable<string> arguments)
		{
			OptionContext c = this.CreateOptionContext();
			c.OptionIndex = -1;
			bool process = true;
			List<string> unprocessed = new List<string>();
			Option def = base.Contains("<>") ? base["<>"] : null;
			foreach (string argument in arguments)
			{
				OptionContext optionContext = c;
				int optionIndex = optionContext.OptionIndex + 1;
				optionContext.OptionIndex = optionIndex;
				bool flag = argument == "--";
				if (flag)
				{
					process = false;
				}
				else
				{
					bool flag2 = !process;
					if (flag2)
					{
						OptionSet.Unprocessed(unprocessed, def, c, argument);
					}
					else
					{
						bool flag3 = !this.Parse(argument, c);
						if (flag3)
						{
							OptionSet.Unprocessed(unprocessed, def, c, argument);
						}
					}
				}
			}
			bool flag4 = c.Option != null;
			if (flag4)
			{
				c.Option.Invoke(c);
			}
			return unprocessed;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003600 File Offset: 0x00001800
		private static bool Unprocessed(ICollection<string> extra, Option def, OptionContext c, string argument)
		{
			bool flag = def == null;
			bool result;
			if (flag)
			{
				extra.Add(argument);
				result = false;
			}
			else
			{
				c.OptionValues.Add(argument);
				c.Option = def;
				c.Option.Invoke(c);
				result = false;
			}
			return result;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000364C File Offset: 0x0000184C
		protected bool GetOptionParts(string argument, out string flag, out string name, out string sep, out string value)
		{
			bool flag2 = argument == null;
			if (flag2)
			{
				throw new ArgumentNullException("argument");
			}
			string text;
			value = (text = null);
			sep = (text = text);
			name = (text = text);
			flag = text;
			Match i = this.ValueOption.Match(argument);
			bool flag3 = !i.Success;
			bool result;
			if (flag3)
			{
				result = false;
			}
			else
			{
				flag = i.Groups["flag"].Value;
				name = i.Groups["name"].Value;
				bool flag4 = i.Groups["sep"].Success && i.Groups["value"].Success;
				if (flag4)
				{
					sep = i.Groups["sep"].Value;
					value = i.Groups["value"].Value;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003740 File Offset: 0x00001940
		protected virtual bool Parse(string argument, OptionContext c)
		{
			bool flag = c.Option != null;
			bool result;
			if (flag)
			{
				this.ParseValue(argument, c);
				result = true;
			}
			else
			{
				string f;
				string i;
				string s;
				string v;
				bool flag2 = !this.GetOptionParts(argument, out f, out i, out s, out v);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = base.Contains(i);
					if (flag3)
					{
						Option p = base[i];
						c.OptionName = f + i;
						c.Option = p;
						OptionValueType optionValueType = p.OptionValueType;
						if (optionValueType != OptionValueType.None)
						{
							if (optionValueType - OptionValueType.Optional <= 1)
							{
								this.ParseValue(v, c);
							}
						}
						else
						{
							c.OptionValues.Add(i);
							c.Option.Invoke(c);
						}
						result = true;
					}
					else
					{
						bool flag4 = this.ParseBool(argument, i, c);
						if (flag4)
						{
							result = true;
						}
						else
						{
							bool flag5 = this.ParseBundledValue(f, string.Concat(new string[]
							{
								i + s + v
							}), c);
							result = flag5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003848 File Offset: 0x00001A48
		private void ParseValue(string option, OptionContext c)
		{
			bool flag = option != null;
			if (flag)
			{
				string[] array;
				if (c.Option.ValueSeparators == null)
				{
					(array = new string[1])[0] = option;
				}
				else
				{
					array = option.Split(c.Option.ValueSeparators, StringSplitOptions.None);
				}
				foreach (string o in array)
				{
					c.OptionValues.Add(o);
				}
			}
			bool flag2 = c.OptionValues.Count == c.Option.MaxValueCount || c.Option.OptionValueType == OptionValueType.Optional;
			if (flag2)
			{
				c.Option.Invoke(c);
			}
			else
			{
				bool flag3 = c.OptionValues.Count > c.Option.MaxValueCount;
				if (flag3)
				{
					throw new OptionException(this.localizer(string.Format("Error: Found {0} option values when expecting {1}.", c.OptionValues.Count, c.Option.MaxValueCount)), c.OptionName);
				}
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000394C File Offset: 0x00001B4C
		private bool ParseBool(string option, string n, OptionContext c)
		{
			string rn;
			bool flag = n.Length >= 1 && (n[n.Length - 1] == '+' || n[n.Length - 1] == '-') && base.Contains(rn = n.Substring(0, n.Length - 1));
			bool result;
			if (flag)
			{
				Option p = base[rn];
				string v = (n[n.Length - 1] == '+') ? option : null;
				c.OptionName = option;
				c.Option = p;
				c.OptionValues.Add(v);
				p.Invoke(c);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000039F8 File Offset: 0x00001BF8
		private bool ParseBundledValue(string f, string n, OptionContext c)
		{
			bool flag = f != "-";
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int i = 0;
				while (i < n.Length)
				{
					string opt = f + n[i].ToString();
					string rn = n[i].ToString();
					bool flag2 = !base.Contains(rn);
					if (flag2)
					{
						bool flag3 = i == 0;
						if (flag3)
						{
							return false;
						}
						throw new OptionException(string.Format(this.localizer("Cannot bundle unregistered option '{0}'."), opt), opt);
					}
					else
					{
						Option p = base[rn];
						OptionValueType optionValueType = p.OptionValueType;
						if (optionValueType != OptionValueType.None)
						{
							if (optionValueType - OptionValueType.Optional > 1)
							{
								throw new InvalidOperationException("Unknown OptionValueType: " + p.OptionValueType);
							}
							string v = n.Substring(i + 1);
							c.Option = p;
							c.OptionName = opt;
							this.ParseValue((v.Length != 0) ? v : null, c);
							return true;
						}
						else
						{
							OptionSet.Invoke(c, opt, n, p);
							i++;
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003B2B File Offset: 0x00001D2B
		private static void Invoke(OptionContext c, string name, string value, Option option)
		{
			c.OptionName = name;
			c.Option = option;
			c.OptionValues.Add(value);
			option.Invoke(c);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003B54 File Offset: 0x00001D54
		public void WriteOptionDescriptions(TextWriter o)
		{
			foreach (Option p in this)
			{
				int written = 0;
				bool flag = !this.WriteOptionPrototype(o, p, ref written);
				if (!flag)
				{
					bool flag2 = written < 29;
					if (flag2)
					{
						o.Write(new string(' ', 29 - written));
					}
					else
					{
						o.WriteLine();
						o.Write(new string(' ', 29));
					}
					List<string> lines = OptionSet.GetLines(this.localizer(OptionSet.GetDescription(p.Description)));
					o.WriteLine(lines[0]);
					string prefix = new string(' ', 31);
					for (int i = 1; i < lines.Count; i++)
					{
						o.Write(prefix);
						o.WriteLine(lines[i]);
					}
				}
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003C5C File Offset: 0x00001E5C
		private bool WriteOptionPrototype(TextWriter o, Option p, ref int written)
		{
			string[] names = p.Names;
			int i = OptionSet.GetNextOptionIndex(names, 0);
			bool flag = i == names.Length;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = names[i].Length == 1;
				if (flag2)
				{
					OptionSet.Write(o, ref written, "  -");
					OptionSet.Write(o, ref written, names[0]);
				}
				else
				{
					OptionSet.Write(o, ref written, "      --");
					OptionSet.Write(o, ref written, names[0]);
				}
				for (i = OptionSet.GetNextOptionIndex(names, i + 1); i < names.Length; i = OptionSet.GetNextOptionIndex(names, i + 1))
				{
					OptionSet.Write(o, ref written, ", ");
					OptionSet.Write(o, ref written, (names[i].Length == 1) ? "-" : "--");
					OptionSet.Write(o, ref written, names[i]);
				}
				bool flag3 = p.OptionValueType == OptionValueType.Optional || p.OptionValueType == OptionValueType.Required;
				if (flag3)
				{
					bool flag4 = p.OptionValueType == OptionValueType.Optional;
					if (flag4)
					{
						OptionSet.Write(o, ref written, this.localizer("["));
					}
					OptionSet.Write(o, ref written, this.localizer("=" + OptionSet.GetArgumentName(0, p.MaxValueCount, p.Description)));
					string sep = (p.ValueSeparators != null && p.ValueSeparators.Length != 0) ? p.ValueSeparators[0] : " ";
					for (int c = 1; c < p.MaxValueCount; c++)
					{
						OptionSet.Write(o, ref written, this.localizer(sep + OptionSet.GetArgumentName(c, p.MaxValueCount, p.Description)));
					}
					bool flag5 = p.OptionValueType == OptionValueType.Optional;
					if (flag5)
					{
						OptionSet.Write(o, ref written, this.localizer("]"));
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003E40 File Offset: 0x00002040
		private static int GetNextOptionIndex(string[] names, int i)
		{
			while (i < names.Length && names[i] == "<>")
			{
				i++;
			}
			return i;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003E76 File Offset: 0x00002076
		private static void Write(TextWriter o, ref int n, string s)
		{
			n += s.Length;
			o.Write(s);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003E8C File Offset: 0x0000208C
		private static string GetArgumentName(int index, int maxIndex, string description)
		{
			bool flag = description == null;
			string result;
			if (flag)
			{
				result = ((maxIndex == 1) ? "VALUE" : ("VALUE" + (index + 1)));
			}
			else
			{
				bool flag2 = maxIndex == 1;
				string[] nameStart;
				if (flag2)
				{
					nameStart = new string[]
					{
						"{0:",
						"{"
					};
				}
				else
				{
					nameStart = new string[]
					{
						"{" + index + ":"
					};
				}
				for (int i = 0; i < nameStart.Length; i++)
				{
					int j = 0;
					int start;
					do
					{
						start = description.IndexOf(nameStart[i], j);
					}
					while (start >= 0 && j != 0 && description[j++ - 1] == '{');
					bool flag3 = start == -1;
					if (!flag3)
					{
						int end = description.IndexOf("}", start);
						bool flag4 = end == -1;
						if (!flag4)
						{
							return description.Substring(start + nameStart[i].Length, end - start - nameStart[i].Length);
						}
					}
				}
				result = ((maxIndex == 1) ? "VALUE" : ("VALUE" + (index + 1)));
			}
			return result;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003FCC File Offset: 0x000021CC
		private static string GetDescription(string description)
		{
			bool flag = description == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				StringBuilder sb = new StringBuilder(description.Length);
				int start = -1;
				int i = 0;
				while (i < description.Length)
				{
					char c = description[i];
					if (c != ':')
					{
						if (c != '{')
						{
							if (c != '}')
							{
								goto IL_103;
							}
							bool flag2 = start < 0;
							if (flag2)
							{
								bool flag3 = i + 1 == description.Length || description[i + 1] != '}';
								if (flag3)
								{
									throw new InvalidOperationException("Invalid option description: " + description);
								}
								i++;
								sb.Append("}");
							}
							else
							{
								sb.Append(description.Substring(start, i - start));
								start = -1;
							}
						}
						else
						{
							bool flag4 = i == start;
							if (flag4)
							{
								sb.Append('{');
								start = -1;
							}
							else
							{
								bool flag5 = start < 0;
								if (flag5)
								{
									start = i + 1;
								}
							}
						}
					}
					else
					{
						bool flag6 = start < 0;
						if (flag6)
						{
							goto IL_103;
						}
						start = i + 1;
					}
					IL_11E:
					i++;
					continue;
					IL_103:
					bool flag7 = start < 0;
					if (flag7)
					{
						sb.Append(description[i]);
					}
					goto IL_11E;
				}
				result = sb.ToString();
			}
			return result;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000411C File Offset: 0x0000231C
		private static List<string> GetLines(string description)
		{
			List<string> lines = new List<string>();
			bool flag = string.IsNullOrEmpty(description);
			List<string> result;
			if (flag)
			{
				lines.Add(string.Empty);
				result = lines;
			}
			else
			{
				int length = 49;
				int start = 0;
				int end;
				do
				{
					end = OptionSet.GetLineEnd(start, length, description);
					bool cont = false;
					bool flag2 = end < description.Length;
					if (flag2)
					{
						char c = description[end];
						bool flag3 = c == '-' || (char.IsWhiteSpace(c) && c != '\n');
						if (flag3)
						{
							end++;
						}
						else
						{
							bool flag4 = c != '\n';
							if (flag4)
							{
								cont = true;
								end--;
							}
						}
					}
					lines.Add(description.Substring(start, end - start));
					bool flag5 = cont;
					if (flag5)
					{
						List<string> list = lines;
						int index = lines.Count - 1;
						list[index] += "-";
					}
					start = end;
					bool flag6 = start < description.Length && description[start] == '\n';
					if (flag6)
					{
						start++;
					}
				}
				while (end < description.Length);
				result = lines;
			}
			return result;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004244 File Offset: 0x00002444
		private static int GetLineEnd(int start, int length, string description)
		{
			int end = Math.Min(start + length, description.Length);
			int sep = -1;
			int i = start;
			while (i < end)
			{
				char c = description[i];
				if (c <= ' ')
				{
					switch (c)
					{
					case '\t':
					case '\v':
						goto IL_60;
					case '\n':
						return i;
					default:
						if (c == ' ')
						{
							goto IL_60;
						}
						break;
					}
				}
				else
				{
					switch (c)
					{
					case ',':
					case '-':
					case '.':
						goto IL_60;
					default:
						if (c == ';')
						{
							goto IL_60;
						}
						break;
					}
				}
				IL_69:
				i++;
				continue;
				IL_60:
				sep = i;
				goto IL_69;
			}
			bool flag = sep == -1 || end == description.Length;
			if (flag)
			{
				return end;
			}
			return sep;
		}

		// Token: 0x04000014 RID: 20
		private Converter<string, string> localizer;

		// Token: 0x04000015 RID: 21
		private readonly Regex ValueOption = new Regex("^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$");

		// Token: 0x04000016 RID: 22
		private const int OptionWidth = 29;

		// Token: 0x0200000C RID: 12
		private sealed class ActionOption : Option
		{
			// Token: 0x06000085 RID: 133 RVA: 0x00004524 File Offset: 0x00002724
			public ActionOption(string prototype, string description, int count, Action<OptionValueCollection> action) : base(prototype, description, count)
			{
				bool flag = action == null;
				if (flag)
				{
					throw new ArgumentNullException("action");
				}
				this.action = action;
			}

			// Token: 0x06000086 RID: 134 RVA: 0x00004558 File Offset: 0x00002758
			protected override void OnParseComplete(OptionContext c)
			{
				this.action(c.OptionValues);
			}

			// Token: 0x0400001E RID: 30
			private Action<OptionValueCollection> action;
		}

		// Token: 0x0200000D RID: 13
		private sealed class ActionOption<T> : Option
		{
			// Token: 0x06000087 RID: 135 RVA: 0x00004570 File Offset: 0x00002770
			public ActionOption(string prototype, string description, Action<T> action) : base(prototype, description, 1)
			{
				bool flag = action == null;
				if (flag)
				{
					throw new ArgumentNullException("action");
				}
				this.action = action;
			}

			// Token: 0x06000088 RID: 136 RVA: 0x000045A2 File Offset: 0x000027A2
			protected override void OnParseComplete(OptionContext c)
			{
				this.action(Option.Parse<T>(c.OptionValues[0], c));
			}

			// Token: 0x0400001F RID: 31
			private Action<T> action;
		}

		// Token: 0x0200000E RID: 14
		private sealed class ActionOption<TKey, TValue> : Option
		{
			// Token: 0x06000089 RID: 137 RVA: 0x000045C4 File Offset: 0x000027C4
			public ActionOption(string prototype, string description, OptionAction<TKey, TValue> action) : base(prototype, description, 2)
			{
				bool flag = action == null;
				if (flag)
				{
					throw new ArgumentNullException("action");
				}
				this.action = action;
			}

			// Token: 0x0600008A RID: 138 RVA: 0x000045F6 File Offset: 0x000027F6
			protected override void OnParseComplete(OptionContext c)
			{
				this.action(Option.Parse<TKey>(c.OptionValues[0], c), Option.Parse<TValue>(c.OptionValues[1], c));
			}

			// Token: 0x04000020 RID: 32
			private OptionAction<TKey, TValue> action;
		}

		// Token: 0x0200000F RID: 15
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600008B RID: 139 RVA: 0x00004629 File Offset: 0x00002829
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600008C RID: 140 RVA: 0x00002648 File Offset: 0x00000848
			public <>c()
			{
			}

			// Token: 0x0600008D RID: 141 RVA: 0x00004638 File Offset: 0x00002838
			internal string <.ctor>b__0_0(string f)
			{
				return f;
			}

			// Token: 0x04000021 RID: 33
			public static readonly OptionSet.<>c <>9 = new OptionSet.<>c();

			// Token: 0x04000022 RID: 34
			public static Converter<string, string> <>9__0_0;
		}

		// Token: 0x02000010 RID: 16
		[CompilerGenerated]
		private sealed class <>c__DisplayClass14_0
		{
			// Token: 0x0600008E RID: 142 RVA: 0x00002648 File Offset: 0x00000848
			public <>c__DisplayClass14_0()
			{
			}

			// Token: 0x0600008F RID: 143 RVA: 0x0000464B File Offset: 0x0000284B
			internal void <Add>b__0(OptionValueCollection v)
			{
				this.action(v[0]);
			}

			// Token: 0x04000023 RID: 35
			public Action<string> action;
		}

		// Token: 0x02000011 RID: 17
		[CompilerGenerated]
		private sealed class <>c__DisplayClass16_0
		{
			// Token: 0x06000090 RID: 144 RVA: 0x00002648 File Offset: 0x00000848
			public <>c__DisplayClass16_0()
			{
			}

			// Token: 0x06000091 RID: 145 RVA: 0x00004661 File Offset: 0x00002861
			internal void <Add>b__0(OptionValueCollection v)
			{
				this.action(v[0], v[1]);
			}

			// Token: 0x04000024 RID: 36
			public OptionAction<string, string> action;
		}
	}
}
