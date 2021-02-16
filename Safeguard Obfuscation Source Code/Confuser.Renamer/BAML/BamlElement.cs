using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000034 RID: 52
	internal class BamlElement
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600011A RID: 282 RVA: 0x000026FF File Offset: 0x000008FF
		// (set) Token: 0x0600011B RID: 283 RVA: 0x00002707 File Offset: 0x00000907
		public BamlElement Parent
		{
			[CompilerGenerated]
			get
			{
				return this.<Parent>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Parent>k__BackingField = value;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600011C RID: 284 RVA: 0x00002710 File Offset: 0x00000910
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00002718 File Offset: 0x00000918
		public BamlRecord Header
		{
			[CompilerGenerated]
			get
			{
				return this.<Header>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Header>k__BackingField = value;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600011E RID: 286 RVA: 0x00002721 File Offset: 0x00000921
		// (set) Token: 0x0600011F RID: 287 RVA: 0x00002729 File Offset: 0x00000929
		public IList<BamlRecord> Body
		{
			[CompilerGenerated]
			get
			{
				return this.<Body>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Body>k__BackingField = value;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00002732 File Offset: 0x00000932
		// (set) Token: 0x06000121 RID: 289 RVA: 0x0000273A File Offset: 0x0000093A
		public IList<BamlElement> Children
		{
			[CompilerGenerated]
			get
			{
				return this.<Children>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Children>k__BackingField = value;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00002743 File Offset: 0x00000943
		// (set) Token: 0x06000123 RID: 291 RVA: 0x0000274B File Offset: 0x0000094B
		public BamlRecord Footer
		{
			[CompilerGenerated]
			get
			{
				return this.<Footer>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Footer>k__BackingField = value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00002754 File Offset: 0x00000954
		// (set) Token: 0x06000125 RID: 293 RVA: 0x0000275C File Offset: 0x0000095C
		public TypeDef Type
		{
			[CompilerGenerated]
			get
			{
				return this.<Type>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Type>k__BackingField = value;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00002765 File Offset: 0x00000965
		// (set) Token: 0x06000127 RID: 295 RVA: 0x0000276D File Offset: 0x0000096D
		public IDnlibDef Attribute
		{
			[CompilerGenerated]
			get
			{
				return this.<Attribute>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Attribute>k__BackingField = value;
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x000093D0 File Offset: 0x000075D0
		private static bool IsHeader(BamlRecord rec)
		{
			BamlRecordType type = rec.Type;
			if (type <= BamlRecordType.PropertyDictionaryStart)
			{
				if (type != BamlRecordType.DocumentStart && type != BamlRecordType.ElementStart)
				{
					switch (type)
					{
					case BamlRecordType.PropertyComplexStart:
					case BamlRecordType.PropertyArrayStart:
					case BamlRecordType.PropertyListStart:
					case BamlRecordType.PropertyDictionaryStart:
						break;
					case BamlRecordType.PropertyComplexEnd:
					case BamlRecordType.PropertyArrayEnd:
					case BamlRecordType.PropertyListEnd:
						goto IL_5A;
					default:
						goto IL_5A;
					}
				}
			}
			else if (type != BamlRecordType.KeyElementStart && type != BamlRecordType.ConstructorParametersStart && type - BamlRecordType.NamedElementStart > 1)
			{
				goto IL_5A;
			}
			return true;
			IL_5A:
			return false;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000943C File Offset: 0x0000763C
		private static bool IsFooter(BamlRecord rec)
		{
			BamlRecordType type = rec.Type;
			if (type <= BamlRecordType.PropertyDictionaryEnd)
			{
				if (type != BamlRecordType.DocumentEnd && type != BamlRecordType.ElementEnd)
				{
					switch (type)
					{
					case BamlRecordType.PropertyComplexEnd:
					case BamlRecordType.PropertyArrayEnd:
					case BamlRecordType.PropertyListEnd:
					case BamlRecordType.PropertyDictionaryEnd:
						break;
					case BamlRecordType.PropertyArrayStart:
					case BamlRecordType.PropertyListStart:
					case BamlRecordType.PropertyDictionaryStart:
						goto IL_58;
					default:
						goto IL_58;
					}
				}
			}
			else if (type != BamlRecordType.KeyElementEnd && type != BamlRecordType.ConstructorParametersEnd && type != BamlRecordType.StaticResourceEnd)
			{
				goto IL_58;
			}
			return true;
			IL_58:
			return false;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000094A8 File Offset: 0x000076A8
		private static bool IsMatch(BamlRecord header, BamlRecord footer)
		{
			BamlRecordType type = header.Type;
			if (type <= BamlRecordType.PropertyDictionaryStart)
			{
				if (type == BamlRecordType.DocumentStart)
				{
					return footer.Type == BamlRecordType.DocumentEnd;
				}
				if (type != BamlRecordType.ElementStart)
				{
					switch (type)
					{
					case BamlRecordType.PropertyComplexStart:
						return footer.Type == BamlRecordType.PropertyComplexEnd;
					case BamlRecordType.PropertyComplexEnd:
					case BamlRecordType.PropertyArrayEnd:
					case BamlRecordType.PropertyListEnd:
						goto IL_DB;
					case BamlRecordType.PropertyArrayStart:
						return footer.Type == BamlRecordType.PropertyArrayEnd;
					case BamlRecordType.PropertyListStart:
						return footer.Type == BamlRecordType.PropertyListEnd;
					case BamlRecordType.PropertyDictionaryStart:
						return footer.Type == BamlRecordType.PropertyDictionaryEnd;
					default:
						goto IL_DB;
					}
				}
			}
			else if (type <= BamlRecordType.ConstructorParametersStart)
			{
				if (type == BamlRecordType.KeyElementStart)
				{
					return footer.Type == BamlRecordType.KeyElementEnd;
				}
				if (type != BamlRecordType.ConstructorParametersStart)
				{
					goto IL_DB;
				}
				return footer.Type == BamlRecordType.ConstructorParametersEnd;
			}
			else if (type != BamlRecordType.NamedElementStart)
			{
				if (type != BamlRecordType.StaticResourceStart)
				{
					goto IL_DB;
				}
				return footer.Type == BamlRecordType.StaticResourceEnd;
			}
			return footer.Type == BamlRecordType.ElementEnd;
			IL_DB:
			return false;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00009598 File Offset: 0x00007798
		public static BamlElement Read(BamlDocument document)
		{
			Debug.Assert(document.Count > 0 && document[0].Type == BamlRecordType.DocumentStart);
			BamlElement current = null;
			Stack<BamlElement> stack = new Stack<BamlElement>();
			for (int i = 0; i < document.Count; i++)
			{
				bool flag = BamlElement.IsHeader(document[i]);
				if (flag)
				{
					BamlElement prev = current;
					current = new BamlElement();
					current.Header = document[i];
					current.Body = new List<BamlRecord>();
					current.Children = new List<BamlElement>();
					bool flag2 = prev != null;
					if (flag2)
					{
						prev.Children.Add(current);
						current.Parent = prev;
						stack.Push(prev);
					}
				}
				else
				{
					bool flag3 = BamlElement.IsFooter(document[i]);
					if (flag3)
					{
						bool flag4 = current == null;
						if (flag4)
						{
							throw new Exception("Unexpected footer.");
						}
						while (!BamlElement.IsMatch(current.Header, document[i]))
						{
							bool flag5 = stack.Count > 0;
							if (flag5)
							{
								current = stack.Pop();
							}
						}
						current.Footer = document[i];
						bool flag6 = stack.Count > 0;
						if (flag6)
						{
							current = stack.Pop();
						}
					}
					else
					{
						current.Body.Add(document[i]);
					}
				}
			}
			Debug.Assert(stack.Count == 0);
			return current;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00002184 File Offset: 0x00000384
		public BamlElement()
		{
		}

		// Token: 0x04000095 RID: 149
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BamlElement <Parent>k__BackingField;

		// Token: 0x04000096 RID: 150
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BamlRecord <Header>k__BackingField;

		// Token: 0x04000097 RID: 151
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<BamlRecord> <Body>k__BackingField;

		// Token: 0x04000098 RID: 152
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<BamlElement> <Children>k__BackingField;

		// Token: 0x04000099 RID: 153
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BamlRecord <Footer>k__BackingField;

		// Token: 0x0400009A RID: 154
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TypeDef <Type>k__BackingField;

		// Token: 0x0400009B RID: 155
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IDnlibDef <Attribute>k__BackingField;
	}
}
