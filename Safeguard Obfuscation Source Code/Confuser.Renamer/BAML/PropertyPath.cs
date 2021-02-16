using System;
using System.Collections.Generic;
using System.Text;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200007A RID: 122
	internal class PropertyPath
	{
		// Token: 0x060002C2 RID: 706 RVA: 0x00003550 File Offset: 0x00001750
		public PropertyPath(string path)
		{
			this.parts = PropertyPath.Parse(path);
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x00020860 File Offset: 0x0001EA60
		public PropertyPathPart[] Parts
		{
			get
			{
				return this.parts;
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00020878 File Offset: 0x0001EA78
		private static PropertyPathPart ReadIndexer(string path, ref int index, bool? isHiera)
		{
			index++;
			List<PropertyPathIndexer> args = new List<PropertyPathIndexer>();
			StringBuilder typeString = new StringBuilder();
			StringBuilder valueString = new StringBuilder();
			bool trim = false;
			int level = 0;
			int state = 0;
			while (state != 3)
			{
				char c = path[index];
				switch (state)
				{
				case 0:
				{
					bool flag = c == '(';
					if (flag)
					{
						index++;
						state = 1;
					}
					else
					{
						bool flag2 = c == '^';
						if (flag2)
						{
							StringBuilder stringBuilder = valueString;
							int num = index + 1;
							index = num;
							stringBuilder.Append(path[num]);
							index++;
							state = 2;
						}
						else
						{
							bool flag3 = char.IsWhiteSpace(c);
							if (flag3)
							{
								index++;
							}
							else
							{
								StringBuilder stringBuilder2 = valueString;
								int num = index;
								index = num + 1;
								stringBuilder2.Append(path[num]);
								state = 2;
							}
						}
					}
					break;
				}
				case 1:
				{
					bool flag4 = c == ')';
					if (flag4)
					{
						index++;
						state = 2;
					}
					else
					{
						bool flag5 = c == '^';
						if (flag5)
						{
							StringBuilder stringBuilder3 = typeString;
							int num = index + 1;
							index = num;
							stringBuilder3.Append(path[num]);
							index++;
						}
						else
						{
							StringBuilder stringBuilder4 = typeString;
							int num = index;
							index = num + 1;
							stringBuilder4.Append(path[num]);
						}
					}
					break;
				}
				case 2:
				{
					bool flag6 = c == '[';
					if (flag6)
					{
						StringBuilder stringBuilder5 = valueString;
						int num = index;
						index = num + 1;
						stringBuilder5.Append(path[num]);
						level++;
						trim = false;
					}
					else
					{
						bool flag7 = c == '^';
						if (flag7)
						{
							StringBuilder stringBuilder6 = valueString;
							int num = index + 1;
							index = num;
							stringBuilder6.Append(path[num]);
							index++;
							trim = false;
						}
						else
						{
							bool flag8 = level > 0 && c == ']';
							if (flag8)
							{
								level--;
								StringBuilder stringBuilder7 = valueString;
								int num = index;
								index = num + 1;
								stringBuilder7.Append(path[num]);
								trim = false;
							}
							else
							{
								bool flag9 = c == ']' || c == ',';
								if (flag9)
								{
									string value = valueString.ToString();
									bool flag10 = trim;
									if (flag10)
									{
										value.TrimEnd(new char[0]);
									}
									args.Add(new PropertyPathIndexer
									{
										Type = typeString.ToString(),
										Value = value
									});
									valueString.Length = 0;
									typeString.Length = 0;
									trim = false;
									index++;
									bool flag11 = c == ',';
									if (flag11)
									{
										state = 0;
									}
									else
									{
										state = 3;
									}
								}
								else
								{
									StringBuilder stringBuilder8 = valueString;
									int num = index;
									index = num + 1;
									stringBuilder8.Append(path[num]);
									bool flag12 = c == ' ' && level == 0;
									trim = flag12;
								}
							}
						}
					}
					break;
				}
				}
			}
			return new PropertyPathPart(true, isHiera, "Item")
			{
				IndexerArguments = args.ToArray()
			};
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00020B50 File Offset: 0x0001ED50
		private static PropertyPathPart ReadProperty(string path, ref int index, bool? isHiera)
		{
			int begin = index;
			while (index < path.Length && path[index] == '.')
			{
				index++;
			}
			int level = 0;
			while (index < path.Length && (level > 0 || Array.IndexOf<char>(PropertyPath.SpecialChars, path[index]) == -1))
			{
				bool flag = path[index] == '(';
				if (flag)
				{
					level++;
				}
				else
				{
					bool flag2 = path[index] == ')';
					if (flag2)
					{
						level--;
					}
				}
				index++;
			}
			string name = path.Substring(begin, index - begin).Trim();
			return new PropertyPathPart(false, isHiera, name);
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00020C10 File Offset: 0x0001EE10
		private static PropertyPathPart[] Parse(string path)
		{
			bool flag = string.IsNullOrEmpty(path);
			PropertyPathPart[] result;
			if (flag)
			{
				result = new PropertyPathPart[]
				{
					new PropertyPathPart(true, null, "")
				};
			}
			else
			{
				List<PropertyPathPart> ret = new List<PropertyPathPart>();
				bool? isHiera = null;
				int index = 0;
				while (index < path.Length)
				{
					bool flag2 = char.IsWhiteSpace(path[index]);
					if (flag2)
					{
						index++;
					}
					else
					{
						char c = path[index];
						char c2 = c;
						if (c2 != '.')
						{
							if (c2 != '/')
							{
								if (c2 != '[')
								{
									ret.Add(PropertyPath.ReadProperty(path, ref index, isHiera));
									isHiera = null;
								}
								else
								{
									ret.Add(PropertyPath.ReadIndexer(path, ref index, isHiera));
									isHiera = null;
								}
							}
							else
							{
								isHiera = new bool?(true);
								index++;
							}
						}
						else
						{
							isHiera = new bool?(false);
							index++;
						}
					}
				}
				result = ret.ToArray();
			}
			return result;
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00020D10 File Offset: 0x0001EF10
		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			foreach (PropertyPathPart part in this.parts)
			{
				bool flag = part.IsHierarchical != null;
				if (flag)
				{
					bool value = part.IsHierarchical.Value;
					if (value)
					{
						ret.Append("/");
					}
					else
					{
						ret.Append(".");
					}
				}
				ret.Append(part.Name);
				bool isIndexer = part.IsIndexer;
				if (isIndexer)
				{
					PropertyPathIndexer[] args = part.IndexerArguments;
					for (int i = 0; i < args.Length; i++)
					{
						bool flag2 = i == 0;
						if (flag2)
						{
							ret.Append("[");
						}
						else
						{
							ret.Append(",");
						}
						bool flag3 = !string.IsNullOrEmpty(args[i].Type);
						if (flag3)
						{
							ret.AppendFormat("({0})", args[i].Type);
						}
						bool flag4 = !string.IsNullOrEmpty(args[i].Value);
						if (flag4)
						{
							foreach (char c in args[i].Value)
							{
								bool flag5 = c == '[' || c == ']' || c == ' ';
								if (flag5)
								{
									ret.Append("^");
								}
								ret.Append(c);
							}
						}
					}
					ret.Append("]");
				}
			}
			return ret.ToString();
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x00003566 File Offset: 0x00001766
		// Note: this type is marked as 'beforefieldinit'.
		static PropertyPath()
		{
		}

		// Token: 0x04000537 RID: 1335
		private static readonly char[] SpecialChars = new char[]
		{
			'.',
			'/',
			'[',
			']'
		};

		// Token: 0x04000538 RID: 1336
		private readonly PropertyPathPart[] parts;
	}
}
