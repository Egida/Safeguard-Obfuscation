using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Confuser.Core
{
	// Token: 0x02000059 RID: 89
	internal struct ObfAttrParser
	{
		// Token: 0x0600020D RID: 525 RVA: 0x00002E2B File Offset: 0x0000102B
		public ObfAttrParser(IDictionary items)
		{
			this.items = items;
			this.str = null;
			this.index = -1;
		}

		// Token: 0x0600020E RID: 526 RVA: 0x000103A8 File Offset: 0x0000E5A8
		private bool ReadId(StringBuilder sb)
		{
			while (this.index < this.str.Length)
			{
				char c = this.str[this.index];
				switch (c)
				{
				case '(':
				case ')':
				case '+':
				case ',':
				case '-':
					goto IL_47;
				case '*':
					break;
				default:
					if (c == ';' || c == '=')
					{
						goto IL_47;
					}
					break;
				}
				string text = this.str;
				int num = this.index;
				this.index = num + 1;
				sb.Append(text[num]);
				continue;
				IL_47:
				return true;
			}
			return false;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00010448 File Offset: 0x0000E648
		private bool ReadString(StringBuilder sb)
		{
			this.Expect('\'');
			while (this.index < this.str.Length)
			{
				char c = this.str[this.index];
				if (c == '\'')
				{
					this.index++;
					return true;
				}
				if (c != '\\')
				{
					sb.Append(this.str[this.index]);
				}
				else
				{
					string text = this.str;
					int num = this.index + 1;
					this.index = num;
					sb.Append(text[num]);
				}
				this.index++;
			}
			return false;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00010504 File Offset: 0x0000E704
		private void Expect(char chr)
		{
			bool flag = this.str[this.index] != chr;
			if (flag)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Expect '",
					chr.ToString(),
					"' at position ",
					this.index + 1,
					"."
				}));
			}
			this.index++;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00010580 File Offset: 0x0000E780
		private char Peek()
		{
			return this.str[this.index];
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00002E43 File Offset: 0x00001043
		private void Next()
		{
			this.index++;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x000105A4 File Offset: 0x0000E7A4
		private bool IsEnd()
		{
			return this.index == this.str.Length;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000105CC File Offset: 0x0000E7CC
		public void ParseProtectionString(IDictionary<ConfuserComponent, Dictionary<string, string>> settings, string str)
		{
			bool flag = str == null;
			if (!flag)
			{
				this.str = str;
				this.index = 0;
				ObfAttrParser.ParseState state = ObfAttrParser.ParseState.Init;
				StringBuilder buffer = new StringBuilder();
				bool protAct = true;
				string protId = null;
				Dictionary<string, string> protParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				while (state != ObfAttrParser.ParseState.End)
				{
					switch (state)
					{
					case ObfAttrParser.ParseState.Init:
					{
						this.ReadId(buffer);
						bool flag2 = buffer.ToString().Equals("preset", StringComparison.OrdinalIgnoreCase);
						if (flag2)
						{
							bool flag3 = this.IsEnd();
							if (flag3)
							{
								throw new ArgumentException("Unexpected end of string in Init state.");
							}
							this.Expect('(');
							buffer.Length = 0;
							state = ObfAttrParser.ParseState.ReadPreset;
						}
						else
						{
							bool flag4 = buffer.Length == 0;
							if (flag4)
							{
								bool flag5 = this.IsEnd();
								if (flag5)
								{
									throw new ArgumentException("Unexpected end of string in Init state.");
								}
								state = ObfAttrParser.ParseState.ReadItemName;
							}
							else
							{
								protAct = true;
								state = ObfAttrParser.ParseState.ProcessItemName;
							}
						}
						break;
					}
					case ObfAttrParser.ParseState.ReadPreset:
					{
						bool flag6 = !this.ReadId(buffer);
						if (flag6)
						{
							throw new ArgumentException("Unexpected end of string in ReadPreset state.");
						}
						this.Expect(')');
						ProtectionPreset preset = (ProtectionPreset)Enum.Parse(typeof(ProtectionPreset), buffer.ToString(), true);
						IEnumerable<Protection> source = this.items.Values.OfType<Protection>();
						Func<Protection, bool> predicate;
						Func<Protection, bool> <>9__0;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = ((Protection prot) => prot.Preset <= preset));
						}
						foreach (Protection item in source.Where(predicate))
						{
							bool flag7 = item.Preset != ProtectionPreset.None && settings != null && !settings.ContainsKey(item);
							if (flag7)
							{
								settings.Add(item, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
							}
						}
						buffer.Length = 0;
						bool flag8 = this.IsEnd();
						if (flag8)
						{
							state = ObfAttrParser.ParseState.End;
						}
						else
						{
							this.Expect(';');
							bool flag9 = this.IsEnd();
							if (flag9)
							{
								state = ObfAttrParser.ParseState.End;
							}
							else
							{
								state = ObfAttrParser.ParseState.ReadItemName;
							}
						}
						break;
					}
					case ObfAttrParser.ParseState.ReadItemName:
					{
						protAct = true;
						bool flag10 = this.Peek() == '+';
						if (flag10)
						{
							protAct = true;
							this.Next();
						}
						else
						{
							bool flag11 = this.Peek() == '-';
							if (flag11)
							{
								protAct = false;
								this.Next();
							}
						}
						this.ReadId(buffer);
						state = ObfAttrParser.ParseState.ProcessItemName;
						break;
					}
					case ObfAttrParser.ParseState.ProcessItemName:
					{
						protId = buffer.ToString();
						buffer.Length = 0;
						bool flag12 = this.IsEnd() || this.Peek() == ';';
						if (flag12)
						{
							state = ObfAttrParser.ParseState.EndItem;
						}
						else
						{
							bool flag13 = this.Peek() == '(';
							if (!flag13)
							{
								throw new ArgumentException("Unexpected character in ProcessItemName state at " + this.index + ".");
							}
							bool flag14 = !protAct;
							if (flag14)
							{
								throw new ArgumentException("No parameters is allowed when removing protection.");
							}
							this.Next();
							state = ObfAttrParser.ParseState.ReadParam;
						}
						break;
					}
					case ObfAttrParser.ParseState.ReadParam:
					{
						bool flag15 = !this.ReadId(buffer);
						if (flag15)
						{
							throw new ArgumentException("Unexpected end of string in ReadParam state.");
						}
						string paramName = buffer.ToString();
						buffer.Length = 0;
						this.Expect('=');
						bool flag16 = !((this.Peek() == '\'') ? this.ReadString(buffer) : this.ReadId(buffer));
						if (flag16)
						{
							throw new ArgumentException("Unexpected end of string in ReadParam state.");
						}
						string paramValue = buffer.ToString();
						buffer.Length = 0;
						protParams.Add(paramName, paramValue);
						bool flag17 = this.Peek() == ',';
						if (flag17)
						{
							this.Next();
							state = ObfAttrParser.ParseState.ReadParam;
						}
						else
						{
							bool flag18 = this.Peek() == ')';
							if (!flag18)
							{
								throw new ArgumentException("Unexpected character in ReadParam state at " + this.index + ".");
							}
							this.Next();
							state = ObfAttrParser.ParseState.EndItem;
						}
						break;
					}
					case ObfAttrParser.ParseState.EndItem:
					{
						bool flag19 = settings != null;
						if (flag19)
						{
							bool flag20 = !this.items.Contains(protId);
							if (flag20)
							{
								throw new KeyNotFoundException("Cannot find protection with id '" + protId + "'.");
							}
							bool flag21 = protAct;
							if (flag21)
							{
								bool flag22 = settings.ContainsKey((Protection)this.items[protId]);
								if (flag22)
								{
									Dictionary<string, string> p = settings[(Protection)this.items[protId]];
									foreach (KeyValuePair<string, string> kvp in protParams)
									{
										p[kvp.Key] = kvp.Value;
									}
								}
								else
								{
									settings[(Protection)this.items[protId]] = protParams;
								}
							}
							else
							{
								settings.Remove((Protection)this.items[protId]);
							}
						}
						protParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
						bool flag23 = this.IsEnd();
						if (flag23)
						{
							state = ObfAttrParser.ParseState.End;
						}
						else
						{
							this.Expect(';');
							bool flag24 = this.IsEnd();
							if (flag24)
							{
								state = ObfAttrParser.ParseState.End;
							}
							else
							{
								state = ObfAttrParser.ParseState.ReadItemName;
							}
						}
						break;
					}
					}
				}
			}
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00010AEC File Offset: 0x0000ECEC
		public void ParsePackerString(string str, out Packer packer, out Dictionary<string, string> packerParams)
		{
			packer = null;
			packerParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			bool flag = str == null;
			if (!flag)
			{
				this.str = str;
				this.index = 0;
				ObfAttrParser.ParseState state = ObfAttrParser.ParseState.ReadItemName;
				StringBuilder buffer = new StringBuilder();
				ProtectionSettings ret = new ProtectionSettings();
				while (state != ObfAttrParser.ParseState.End)
				{
					switch (state)
					{
					case ObfAttrParser.ParseState.ReadItemName:
					{
						this.ReadId(buffer);
						string packerId = buffer.ToString();
						bool flag2 = !this.items.Contains(packerId);
						if (flag2)
						{
							throw new KeyNotFoundException("Cannot find packer with id '" + packerId + "'.");
						}
						packer = (Packer)this.items[packerId];
						buffer.Length = 0;
						bool flag3 = this.IsEnd() || this.Peek() == ';';
						if (flag3)
						{
							state = ObfAttrParser.ParseState.EndItem;
						}
						else
						{
							bool flag4 = this.Peek() == '(';
							if (!flag4)
							{
								throw new ArgumentException("Unexpected character in ReadItemName state at " + this.index + ".");
							}
							this.Next();
							state = ObfAttrParser.ParseState.ReadParam;
						}
						break;
					}
					case ObfAttrParser.ParseState.ReadParam:
					{
						bool flag5 = !this.ReadId(buffer);
						if (flag5)
						{
							throw new ArgumentException("Unexpected end of string in ReadParam state.");
						}
						string paramName = buffer.ToString();
						buffer.Length = 0;
						this.Expect('=');
						bool flag6 = !this.ReadId(buffer);
						if (flag6)
						{
							throw new ArgumentException("Unexpected end of string in ReadParam state.");
						}
						string paramValue = buffer.ToString();
						buffer.Length = 0;
						packerParams.Add(paramName, paramValue);
						bool flag7 = this.Peek() == ',';
						if (flag7)
						{
							this.Next();
							state = ObfAttrParser.ParseState.ReadParam;
						}
						else
						{
							bool flag8 = this.Peek() == ')';
							if (!flag8)
							{
								throw new ArgumentException("Unexpected character in ReadParam state at " + this.index + ".");
							}
							this.Next();
							state = ObfAttrParser.ParseState.EndItem;
						}
						break;
					}
					case ObfAttrParser.ParseState.EndItem:
					{
						bool flag9 = this.IsEnd();
						if (flag9)
						{
							state = ObfAttrParser.ParseState.End;
						}
						else
						{
							this.Expect(';');
							bool flag10 = !this.IsEnd();
							if (flag10)
							{
								throw new ArgumentException("Unexpected character in EndItem state at " + this.index + ".");
							}
							state = ObfAttrParser.ParseState.End;
						}
						break;
					}
					}
				}
			}
		}

		// Token: 0x040001B7 RID: 439
		private readonly IDictionary items;

		// Token: 0x040001B8 RID: 440
		private string str;

		// Token: 0x040001B9 RID: 441
		private int index;

		// Token: 0x0200005A RID: 90
		private enum ParseState
		{
			// Token: 0x040001BB RID: 443
			Init,
			// Token: 0x040001BC RID: 444
			ReadPreset,
			// Token: 0x040001BD RID: 445
			ReadItemName,
			// Token: 0x040001BE RID: 446
			ProcessItemName,
			// Token: 0x040001BF RID: 447
			ReadParam,
			// Token: 0x040001C0 RID: 448
			EndItem,
			// Token: 0x040001C1 RID: 449
			End
		}

		// Token: 0x0200005B RID: 91
		[CompilerGenerated]
		private sealed class <>c__DisplayClass11_0
		{
			// Token: 0x06000216 RID: 534 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass11_0()
			{
			}

			// Token: 0x06000217 RID: 535 RVA: 0x00002E54 File Offset: 0x00001054
			internal bool <ParseProtectionString>b__0(Protection prot)
			{
				return prot.Preset <= this.preset;
			}

			// Token: 0x040001C2 RID: 450
			public ProtectionPreset preset;

			// Token: 0x040001C3 RID: 451
			public Func<Protection, bool> <>9__0;
		}
	}
}
