using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000036 RID: 54
	internal enum BamlRecordType : byte
	{
		// Token: 0x0400009E RID: 158
		ClrEvent = 19,
		// Token: 0x0400009F RID: 159
		Comment = 23,
		// Token: 0x040000A0 RID: 160
		AssemblyInfo = 28,
		// Token: 0x040000A1 RID: 161
		AttributeInfo = 31,
		// Token: 0x040000A2 RID: 162
		ConstructorParametersStart = 42,
		// Token: 0x040000A3 RID: 163
		ConstructorParametersEnd,
		// Token: 0x040000A4 RID: 164
		ConstructorParameterType,
		// Token: 0x040000A5 RID: 165
		ConnectionId,
		// Token: 0x040000A6 RID: 166
		ContentProperty,
		// Token: 0x040000A7 RID: 167
		DefAttribute = 25,
		// Token: 0x040000A8 RID: 168
		DefAttributeKeyString = 38,
		// Token: 0x040000A9 RID: 169
		DefAttributeKeyType,
		// Token: 0x040000AA RID: 170
		DeferableContentStart = 37,
		// Token: 0x040000AB RID: 171
		DefTag = 24,
		// Token: 0x040000AC RID: 172
		DocumentEnd = 2,
		// Token: 0x040000AD RID: 173
		DocumentStart = 1,
		// Token: 0x040000AE RID: 174
		ElementEnd = 4,
		// Token: 0x040000AF RID: 175
		ElementStart = 3,
		// Token: 0x040000B0 RID: 176
		EndAttributes = 26,
		// Token: 0x040000B1 RID: 177
		KeyElementEnd = 41,
		// Token: 0x040000B2 RID: 178
		KeyElementStart = 40,
		// Token: 0x040000B3 RID: 179
		LastRecordType = 57,
		// Token: 0x040000B4 RID: 180
		LineNumberAndPosition = 53,
		// Token: 0x040000B5 RID: 181
		LinePosition,
		// Token: 0x040000B6 RID: 182
		LiteralContent = 15,
		// Token: 0x040000B7 RID: 183
		NamedElementStart = 47,
		// Token: 0x040000B8 RID: 184
		OptimizedStaticResource = 55,
		// Token: 0x040000B9 RID: 185
		PIMapping = 27,
		// Token: 0x040000BA RID: 186
		PresentationOptionsAttribute = 52,
		// Token: 0x040000BB RID: 187
		ProcessingInstruction = 22,
		// Token: 0x040000BC RID: 188
		Property = 5,
		// Token: 0x040000BD RID: 189
		PropertyArrayEnd = 10,
		// Token: 0x040000BE RID: 190
		PropertyArrayStart = 9,
		// Token: 0x040000BF RID: 191
		PropertyComplexEnd = 8,
		// Token: 0x040000C0 RID: 192
		PropertyComplexStart = 7,
		// Token: 0x040000C1 RID: 193
		PropertyCustom = 6,
		// Token: 0x040000C2 RID: 194
		PropertyDictionaryEnd = 14,
		// Token: 0x040000C3 RID: 195
		PropertyDictionaryStart = 13,
		// Token: 0x040000C4 RID: 196
		PropertyListEnd = 12,
		// Token: 0x040000C5 RID: 197
		PropertyListStart = 11,
		// Token: 0x040000C6 RID: 198
		PropertyStringReference = 33,
		// Token: 0x040000C7 RID: 199
		PropertyTypeReference,
		// Token: 0x040000C8 RID: 200
		PropertyWithConverter = 36,
		// Token: 0x040000C9 RID: 201
		PropertyWithExtension = 35,
		// Token: 0x040000CA RID: 202
		PropertyWithStaticResourceId = 56,
		// Token: 0x040000CB RID: 203
		RoutedEvent = 18,
		// Token: 0x040000CC RID: 204
		StaticResourceEnd = 49,
		// Token: 0x040000CD RID: 205
		StaticResourceId,
		// Token: 0x040000CE RID: 206
		StaticResourceStart = 48,
		// Token: 0x040000CF RID: 207
		StringInfo = 32,
		// Token: 0x040000D0 RID: 208
		Text = 16,
		// Token: 0x040000D1 RID: 209
		TextWithConverter,
		// Token: 0x040000D2 RID: 210
		TextWithId = 51,
		// Token: 0x040000D3 RID: 211
		TypeInfo = 29,
		// Token: 0x040000D4 RID: 212
		TypeSerializerInfo,
		// Token: 0x040000D5 RID: 213
		XmlAttribute = 21,
		// Token: 0x040000D6 RID: 214
		XmlnsProperty = 20
	}
}
