using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using Confuser.Core;
using Confuser.Renamer.Analyzers;
using Confuser.Renamer.References;
using dnlib.DotNet;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200002D RID: 45
	internal class BAMLAnalyzer
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000EA RID: 234 RVA: 0x00007520 File Offset: 0x00005720
		// (remove) Token: 0x060000EB RID: 235 RVA: 0x00007558 File Offset: 0x00005758
		public event Action<BAMLAnalyzer, BamlElement> AnalyzeElement
		{
			[CompilerGenerated]
			add
			{
				Action<BAMLAnalyzer, BamlElement> action = this.AnalyzeElement;
				Action<BAMLAnalyzer, BamlElement> action2;
				do
				{
					action2 = action;
					Action<BAMLAnalyzer, BamlElement> value2 = (Action<BAMLAnalyzer, BamlElement>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange<Action<BAMLAnalyzer, BamlElement>>(ref this.AnalyzeElement, value2, action2);
				}
				while (action != action2);
			}
			[CompilerGenerated]
			remove
			{
				Action<BAMLAnalyzer, BamlElement> action = this.AnalyzeElement;
				Action<BAMLAnalyzer, BamlElement> action2;
				do
				{
					action2 = action;
					Action<BAMLAnalyzer, BamlElement> value2 = (Action<BAMLAnalyzer, BamlElement>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange<Action<BAMLAnalyzer, BamlElement>>(ref this.AnalyzeElement, value2, action2);
				}
				while (action != action2);
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00007590 File Offset: 0x00005790
		public ConfuserContext Context
		{
			get
			{
				return this.context;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000ED RID: 237 RVA: 0x000075A8 File Offset: 0x000057A8
		public INameService NameService
		{
			get
			{
				return this.service;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000EE RID: 238 RVA: 0x00002623 File Offset: 0x00000823
		// (set) Token: 0x060000EF RID: 239 RVA: 0x0000262B File Offset: 0x0000082B
		public string CurrentBAMLName
		{
			[CompilerGenerated]
			get
			{
				return this.<CurrentBAMLName>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<CurrentBAMLName>k__BackingField = value;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x00002634 File Offset: 0x00000834
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x0000263C File Offset: 0x0000083C
		public ModuleDefMD Module
		{
			[CompilerGenerated]
			get
			{
				return this.<Module>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Module>k__BackingField = value;
			}
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000075C0 File Offset: 0x000057C0
		public BAMLAnalyzer(ConfuserContext context, INameService service)
		{
			this.context = context;
			this.service = service;
			this.PreInit();
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00007658 File Offset: 0x00005858
		private void PreInit()
		{
			foreach (TypeDef type in this.context.Modules.SelectMany((ModuleDefMD m) => m.GetTypes()))
			{
				foreach (PropertyDef property in type.Properties)
				{
					bool flag = property.IsPublic() && !property.IsStatic();
					if (flag)
					{
						this.properties.AddListEntry(property.Name, property);
					}
				}
				foreach (EventDef evt in type.Events)
				{
					bool flag2 = evt.IsPublic() && !evt.IsStatic();
					if (flag2)
					{
						this.events.AddListEntry(evt.Name, evt);
					}
				}
				foreach (MethodDef method in type.Methods)
				{
					bool flag3 = method.IsPublic && !method.IsStatic;
					if (flag3)
					{
						this.methods.AddListEntry(method.Name, method);
					}
				}
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00007858 File Offset: 0x00005A58
		public IEnumerable<PropertyDef> LookupProperty(string name)
		{
			List<PropertyDef> ret;
			bool flag = !this.properties.TryGetValue(name, out ret);
			IEnumerable<PropertyDef> result;
			if (flag)
			{
				result = Enumerable.Empty<PropertyDef>();
			}
			else
			{
				result = ret;
			}
			return result;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007888 File Offset: 0x00005A88
		public IEnumerable<EventDef> LookupEvent(string name)
		{
			List<EventDef> ret;
			bool flag = !this.events.TryGetValue(name, out ret);
			IEnumerable<EventDef> result;
			if (flag)
			{
				result = Enumerable.Empty<EventDef>();
			}
			else
			{
				result = ret;
			}
			return result;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000078B8 File Offset: 0x00005AB8
		public IEnumerable<MethodDef> LookupMethod(string name)
		{
			List<MethodDef> ret;
			bool flag = !this.methods.TryGetValue(name, out ret);
			IEnumerable<MethodDef> result;
			if (flag)
			{
				result = Enumerable.Empty<MethodDef>();
			}
			else
			{
				result = ret;
			}
			return result;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000078E8 File Offset: 0x00005AE8
		public BamlDocument Analyze(ModuleDefMD module, string bamlName, byte[] data)
		{
			this.Module = module;
			this.CurrentBAMLName = bamlName;
			bool isClr = module.IsClr40;
			if (isClr)
			{
				KnownThingsv4 knownThingsv;
				if ((knownThingsv = this.thingsv4) == null)
				{
					knownThingsv = (this.thingsv4 = new KnownThingsv4(this.context, module));
				}
				this.things = knownThingsv;
			}
			else
			{
				KnownThingsv3 knownThingsv2;
				if ((knownThingsv2 = this.thingsv3) == null)
				{
					knownThingsv2 = (this.thingsv3 = new KnownThingsv3(this.context, module));
				}
				this.things = knownThingsv2;
			}
			Debug.Assert(BitConverter.ToInt32(data, 0) == data.Length - 4);
			BamlDocument document = BamlReader.ReadDocument(new MemoryStream(data, 4, data.Length - 4));
			document.RemoveWhere((BamlRecord rec) => rec is LineNumberAndPositionRecord || rec is LinePositionRecord);
			this.PopulateReferences(document);
			BamlElement rootElem = BamlElement.Read(document);
			BamlElement trueRoot = rootElem.Children.Single<BamlElement>();
			Stack<BamlElement> stack = new Stack<BamlElement>();
			stack.Push(rootElem);
			while (stack.Count > 0)
			{
				BamlElement elem = stack.Pop();
				this.ProcessBAMLElement(trueRoot, elem);
				foreach (BamlElement child in elem.Children)
				{
					stack.Push(child);
				}
			}
			return document;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00007A54 File Offset: 0x00005C54
		private void PopulateReferences(BamlDocument document)
		{
			Dictionary<string, List<Tuple<AssemblyDef, string>>> clrNs = new Dictionary<string, List<Tuple<AssemblyDef, string>>>();
			this.assemblyRefs.Clear();
			foreach (AssemblyInfoRecord rec in document.OfType<AssemblyInfoRecord>())
			{
				AssemblyDef assembly = this.context.Resolver.ResolveThrow(rec.AssemblyFullName, this.Module);
				this.assemblyRefs.Add(rec.AssemblyId, assembly);
				bool flag = !this.context.Modules.Any((ModuleDefMD m) => m.Assembly == assembly);
				if (!flag)
				{
					foreach (CustomAttribute attr in assembly.CustomAttributes.FindAll("System.Windows.Markup.XmlnsDefinitionAttribute"))
					{
						clrNs.AddListEntry((UTF8String)attr.ConstructorArguments[0].Value, Tuple.Create<AssemblyDef, string>(assembly, (UTF8String)attr.ConstructorArguments[1].Value));
					}
				}
			}
			this.xmlnsCtx = new BAMLAnalyzer.XmlNsContext(document, this.assemblyRefs);
			this.typeRefs.Clear();
			foreach (TypeInfoRecord rec2 in document.OfType<TypeInfoRecord>())
			{
				short asmId = (short)(rec2.AssemblyId & 4095);
				bool flag2 = asmId == -1;
				AssemblyDef assembly3;
				if (flag2)
				{
					assembly3 = this.things.FrameworkAssembly;
				}
				else
				{
					assembly3 = this.assemblyRefs[(ushort)asmId];
				}
				AssemblyDef assemblyRef = (this.Module.Assembly == assembly3) ? null : assembly3;
				TypeSig typeSig = TypeNameParser.ParseAsTypeSigReflectionThrow(this.Module, rec2.TypeFullName, new BAMLAnalyzer.DummyAssemblyRefFinder(assemblyRef));
				this.typeRefs[rec2.TypeId] = typeSig;
				this.AddTypeSigReference(typeSig, new BAMLTypeReference(typeSig, rec2));
			}
			this.attrRefs.Clear();
			foreach (AttributeInfoRecord rec3 in document.OfType<AttributeInfoRecord>())
			{
				TypeSig declType;
				bool flag3 = this.typeRefs.TryGetValue(rec3.OwnerTypeId, out declType);
				if (flag3)
				{
					TypeDef type = declType.ToBasicTypeDefOrRef().ResolveTypeDefThrow();
					this.attrRefs[rec3.AttributeId] = this.AnalyzeAttributeReference(type, rec3);
				}
				else
				{
					Debug.Assert((short)rec3.OwnerTypeId < 0);
					TypeDef declTypeDef = this.things.Types(-(KnownTypes)rec3.OwnerTypeId);
					this.attrRefs[rec3.AttributeId] = this.AnalyzeAttributeReference(declTypeDef, rec3);
				}
			}
			this.strings.Clear();
			foreach (StringInfoRecord rec4 in document.OfType<StringInfoRecord>())
			{
				this.strings[rec4.StringId] = rec4;
			}
			foreach (PIMappingRecord rec5 in document.OfType<PIMappingRecord>())
			{
				short asmId2 = (short)(rec5.AssemblyId & 4095);
				bool flag4 = asmId2 == -1;
				AssemblyDef assembly2;
				if (flag4)
				{
					assembly2 = this.things.FrameworkAssembly;
				}
				else
				{
					assembly2 = this.assemblyRefs[(ushort)asmId2];
				}
				Tuple<AssemblyDef, string> scope = Tuple.Create<AssemblyDef, string>(assembly2, rec5.ClrNamespace);
				clrNs.AddListEntry(rec5.XmlNamespace, scope);
			}
			this.xmlns.Clear();
			foreach (XmlnsPropertyRecord rec6 in document.OfType<XmlnsPropertyRecord>())
			{
				List<Tuple<AssemblyDef, string>> clrMap;
				bool flag5 = clrNs.TryGetValue(rec6.XmlNamespace, out clrMap);
				if (flag5)
				{
					this.xmlns[rec6.Prefix] = clrMap;
					foreach (Tuple<AssemblyDef, string> scope2 in clrMap)
					{
						this.xmlnsCtx.AddNsMap(scope2, rec6.Prefix);
					}
				}
			}
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00007FC0 File Offset: 0x000061C0
		public TypeDef ResolveType(ushort typeId)
		{
			bool flag = (short)typeId < 0;
			TypeDef result;
			if (flag)
			{
				result = this.things.Types(-(KnownTypes)typeId);
			}
			else
			{
				result = this.typeRefs[typeId].ToBasicTypeDefOrRef().ResolveTypeDefThrow();
			}
			return result;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00008008 File Offset: 0x00006208
		private TypeSig ResolveType(string typeName, out string prefix)
		{
			int index = typeName.IndexOf(':');
			bool flag = index == -1;
			List<Tuple<AssemblyDef, string>> clrNs;
			if (flag)
			{
				prefix = "";
				bool flag2 = !this.xmlns.TryGetValue(prefix, out clrNs);
				if (flag2)
				{
					return null;
				}
			}
			else
			{
				prefix = typeName.Substring(0, index);
				bool flag3 = !this.xmlns.TryGetValue(prefix, out clrNs);
				if (flag3)
				{
					return null;
				}
				typeName = typeName.Substring(index + 1);
			}
			foreach (Tuple<AssemblyDef, string> ns in clrNs)
			{
				TypeSig sig = TypeNameParser.ParseAsTypeSigReflectionThrow(this.Module, ns.Item2 + "." + typeName, new BAMLAnalyzer.DummyAssemblyRefFinder(ns.Item1));
				bool flag4 = sig.ToBasicTypeDefOrRef().ResolveTypeDef() != null;
				if (flag4)
				{
					return sig;
				}
			}
			return null;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00008114 File Offset: 0x00006314
		public Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> ResolveAttribute(ushort attrId)
		{
			bool flag = (short)attrId < 0;
			Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> result;
			if (flag)
			{
				Tuple<KnownTypes, PropertyDef, TypeDef> info = this.things.Properties(-(KnownProperties)attrId);
				result = Tuple.Create<IDnlibDef, AttributeInfoRecord, TypeDef>(info.Item2, null, info.Item3);
			}
			else
			{
				result = this.attrRefs[attrId];
			}
			return result;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00008168 File Offset: 0x00006368
		private void AddTypeSigReference(TypeSig typeSig, INameReference<IDnlibDef> reference)
		{
			foreach (ITypeDefOrRef type in typeSig.FindTypeRefs())
			{
				TypeDef typeDef = type.ResolveTypeDefThrow();
				bool flag = this.context.Modules.Contains((ModuleDefMD)typeDef.Module);
				if (flag)
				{
					this.service.ReduceRenameMode(typeDef, RenameMode.Letters);
					bool flag2 = type is TypeRef;
					if (flag2)
					{
						this.service.AddReference<TypeDef>(typeDef, new TypeRefReference((TypeRef)type, typeDef));
					}
					this.service.AddReference<IDnlibDef>(typeDef, reference);
				}
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00008220 File Offset: 0x00006420
		private void ProcessBAMLElement(BamlElement root, BamlElement elem)
		{
			this.ProcessElementHeader(elem);
			this.ProcessElementBody(root, elem);
			bool flag = this.AnalyzeElement != null;
			if (flag)
			{
				this.AnalyzeElement(this, elem);
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000825C File Offset: 0x0000645C
		private void ProcessElementHeader(BamlElement elem)
		{
			BamlRecordType type = elem.Header.Type;
			if (type > BamlRecordType.PropertyDictionaryStart)
			{
				if (type <= BamlRecordType.ConstructorParametersStart)
				{
					if (type != BamlRecordType.KeyElementStart)
					{
						if (type != BamlRecordType.ConstructorParametersStart)
						{
							return;
						}
						elem.Type = elem.Parent.Type;
						elem.Attribute = elem.Parent.Attribute;
						return;
					}
				}
				else
				{
					if (type == BamlRecordType.NamedElementStart)
					{
						goto IL_A8;
					}
					if (type != BamlRecordType.StaticResourceStart)
					{
						return;
					}
				}
				elem.Type = this.Module.CorLibTypes.Object.TypeDefOrRef.ResolveTypeDef();
				elem.Attribute = null;
				return;
			}
			if (type == BamlRecordType.DocumentStart)
			{
				return;
			}
			if (type != BamlRecordType.ElementStart)
			{
				switch (type)
				{
				case BamlRecordType.PropertyComplexStart:
				case BamlRecordType.PropertyArrayStart:
				case BamlRecordType.PropertyListStart:
				case BamlRecordType.PropertyDictionaryStart:
				{
					Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attrInfo = this.ResolveAttribute(((PropertyComplexStartRecord)elem.Header).AttributeId);
					elem.Type = attrInfo.Item3;
					elem.Attribute = attrInfo.Item1;
					bool flag = elem.Attribute != null;
					if (flag)
					{
						elem.Type = this.GetAttributeType(elem.Attribute);
					}
					return;
				}
				case BamlRecordType.PropertyComplexEnd:
				case BamlRecordType.PropertyArrayEnd:
				case BamlRecordType.PropertyListEnd:
					return;
				default:
					return;
				}
			}
			IL_A8:
			elem.Type = this.ResolveType(((ElementStartRecord)elem.Header).TypeId);
			elem.Attribute = elem.Parent.Attribute;
			bool flag2 = elem.Attribute != null;
			if (flag2)
			{
				elem.Type = this.GetAttributeType(elem.Attribute);
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x000083E0 File Offset: 0x000065E0
		private TypeDef GetAttributeType(IDnlibDef attr)
		{
			ITypeDefOrRef retType = null;
			bool flag = attr is PropertyDef;
			if (flag)
			{
				retType = ((PropertyDef)attr).PropertySig.RetType.ToBasicTypeDefOrRef();
			}
			else
			{
				bool flag2 = attr is EventDef;
				if (flag2)
				{
					retType = ((EventDef)attr).EventType;
				}
			}
			return (retType == null) ? null : retType.ResolveTypeDefThrow();
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00008440 File Offset: 0x00006640
		private void ProcessElementBody(BamlElement root, BamlElement elem)
		{
			foreach (BamlRecord rec in elem.Body)
			{
				bool flag = rec is PropertyRecord;
				if (flag)
				{
					PropertyRecord propRec = (PropertyRecord)rec;
					Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attrInfo = this.ResolveAttribute(propRec.AttributeId);
					TypeDef type = attrInfo.Item3;
					IDnlibDef attr = attrInfo.Item1;
					bool flag2 = attr != null;
					if (flag2)
					{
						type = this.GetAttributeType(attr);
					}
					bool flag3 = attrInfo.Item1 is EventDef;
					if (flag3)
					{
						MethodDef method = root.Type.FindMethod(propRec.Value);
						bool flag4 = method == null;
						if (flag4)
						{
							this.context.Logger.WarnFormat("Cannot resolve method '{0}' in '{1}'.", new object[]
							{
								root.Type.FullName,
								propRec.Value
							});
						}
						else
						{
							BAMLAttributeReference reference = new BAMLAttributeReference(method, propRec);
							this.service.AddReference<IDnlibDef>(method, reference);
						}
					}
					bool flag5 = rec is PropertyWithConverterRecord;
					if (flag5)
					{
						this.ProcessConverter((PropertyWithConverterRecord)rec, type);
					}
				}
				else
				{
					bool flag6 = rec is PropertyComplexStartRecord;
					if (flag6)
					{
						Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attrInfo2 = this.ResolveAttribute(((PropertyComplexStartRecord)rec).AttributeId);
						TypeDef type = attrInfo2.Item3;
						IDnlibDef attr = attrInfo2.Item1;
						bool flag7 = attr != null;
						if (flag7)
						{
							type = this.GetAttributeType(attr);
						}
					}
					else
					{
						bool flag8 = rec is ContentPropertyRecord;
						if (flag8)
						{
							Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attrInfo3 = this.ResolveAttribute(((ContentPropertyRecord)rec).AttributeId);
							TypeDef type = attrInfo3.Item3;
							IDnlibDef attr = attrInfo3.Item1;
							bool flag9 = elem.Attribute != null && attr != null;
							if (flag9)
							{
								type = this.GetAttributeType(attr);
							}
							foreach (BamlElement child in elem.Children)
							{
								child.Type = type;
								child.Attribute = attr;
							}
						}
						else
						{
							bool flag10 = rec is PropertyCustomRecord;
							if (flag10)
							{
								PropertyCustomRecord customRec = (PropertyCustomRecord)rec;
								Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attrInfo4 = this.ResolveAttribute(customRec.AttributeId);
								TypeDef type = attrInfo4.Item3;
								IDnlibDef attr = attrInfo4.Item1;
								bool flag11 = elem.Attribute != null && attr != null;
								if (flag11)
								{
									type = this.GetAttributeType(attr);
								}
								bool flag12 = ((int)customRec.SerializerTypeId & -16385) != 0 && ((int)customRec.SerializerTypeId & -16385) == 137;
								if (flag12)
								{
								}
							}
							else
							{
								bool flag13 = rec is PropertyWithExtensionRecord;
								if (flag13)
								{
									PropertyWithExtensionRecord extRec = (PropertyWithExtensionRecord)rec;
									Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attrInfo5 = this.ResolveAttribute(extRec.AttributeId);
									TypeDef type = attrInfo5.Item3;
									IDnlibDef attr = attrInfo5.Item1;
									bool flag14 = elem.Attribute != null && attr != null;
									if (flag14)
									{
										type = this.GetAttributeType(attr);
									}
									bool flag15 = extRec.Flags == 602;
									if (flag15)
									{
										bool flag16 = (short)extRec.ValueId >= 0;
										if (flag16)
										{
											attrInfo5 = this.ResolveAttribute(extRec.ValueId);
											IDnlibDef attrTarget = attrInfo5.Item1;
											bool flag17 = attrTarget == null;
											if (flag17)
											{
												TypeSig declType;
												bool flag18 = this.typeRefs.TryGetValue(attrInfo5.Item2.OwnerTypeId, out declType);
												TypeDef declTypeDef;
												if (flag18)
												{
													declTypeDef = declType.ToBasicTypeDefOrRef().ResolveTypeDefThrow();
												}
												else
												{
													Debug.Assert((short)attrInfo5.Item2.OwnerTypeId < 0);
													declTypeDef = this.things.Types(-(KnownTypes)attrInfo5.Item2.OwnerTypeId);
												}
												attrTarget = declTypeDef.FindField(attrInfo5.Item2.Name);
											}
											bool flag19 = attrTarget != null;
											if (flag19)
											{
												this.service.AddReference<IDnlibDef>(attrTarget, new BAMLAttributeReference(attrTarget, attrInfo5.Item2));
											}
										}
									}
								}
								else
								{
									bool flag20 = rec is TextRecord;
									if (flag20)
									{
										TextRecord txt = (TextRecord)rec;
										string value = txt.Value;
										bool flag21 = txt is TextWithIdRecord;
										if (flag21)
										{
											value = this.strings[((TextWithIdRecord)txt).ValueId].Value;
										}
										string prefix;
										TypeSig sig = this.ResolveType(value.Trim(), out prefix);
										bool flag22 = sig != null && this.context.Modules.Contains((ModuleDefMD)sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module);
										if (flag22)
										{
											BAMLConverterTypeReference reference2 = new BAMLConverterTypeReference(this.xmlnsCtx, sig, txt);
											this.AddTypeSigReference(sig, reference2);
										}
										else
										{
											this.AnalyzePropertyPath(value);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000894C File Offset: 0x00006B4C
		private void ProcessConverter(PropertyWithConverterRecord rec, TypeDef type)
		{
			TypeDef converter = this.ResolveType(rec.ConverterTypeId);
			bool flag = converter.FullName == "System.ComponentModel.EnumConverter";
			if (flag)
			{
				bool flag2 = type != null && this.context.Modules.Contains((ModuleDefMD)type.Module);
				if (flag2)
				{
					FieldDef enumField = type.FindField(rec.Value);
					bool flag3 = enumField != null;
					if (flag3)
					{
						this.service.AddReference<FieldDef>(enumField, new BAMLEnumReference(enumField, rec));
					}
				}
			}
			else
			{
				bool flag4 = converter.FullName == "System.Windows.Input.CommandConverter";
				if (flag4)
				{
					string cmd = rec.Value.Trim();
					int index = cmd.IndexOf('.');
					bool flag5 = index != -1;
					if (flag5)
					{
						string typeName = cmd.Substring(0, index);
						string prefix;
						TypeSig sig = this.ResolveType(typeName, out prefix);
						bool flag6 = sig != null;
						if (flag6)
						{
							string cmdName = cmd.Substring(index + 1);
							TypeDef typeDef = sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow();
							bool flag7 = this.context.Modules.Contains((ModuleDefMD)typeDef.Module);
							if (flag7)
							{
								PropertyDef property = typeDef.FindProperty(cmdName);
								bool flag8 = property != null;
								if (flag8)
								{
									BAMLConverterMemberReference reference = new BAMLConverterMemberReference(this.xmlnsCtx, sig, property, rec);
									this.AddTypeSigReference(sig, reference);
									this.service.ReduceRenameMode(property, RenameMode.Letters);
									this.service.AddReference<IDnlibDef>(property, reference);
								}
								FieldDef field = typeDef.FindField(cmdName);
								bool flag9 = field != null;
								if (flag9)
								{
									BAMLConverterMemberReference reference2 = new BAMLConverterMemberReference(this.xmlnsCtx, sig, field, rec);
									this.AddTypeSigReference(sig, reference2);
									this.service.ReduceRenameMode(field, RenameMode.Letters);
									this.service.AddReference<IDnlibDef>(field, reference2);
								}
								bool flag10 = property == null && field == null;
								if (flag10)
								{
									this.context.Logger.WarnFormat("Could not resolve command '{0}' in '{1}'.", new object[]
									{
										cmd,
										this.CurrentBAMLName
									});
								}
							}
						}
					}
				}
				else
				{
					bool flag11 = converter.FullName == "System.Windows.Markup.DependencyPropertyConverter";
					if (!flag11)
					{
						bool flag12 = converter.FullName == "System.Windows.PropertyPathConverter";
						if (flag12)
						{
							this.AnalyzePropertyPath(rec.Value);
						}
						else
						{
							bool flag13 = converter.FullName == "System.Windows.Markup.RoutedEventConverter";
							if (!flag13)
							{
								bool flag14 = converter.FullName == "System.Windows.Markup.TypeTypeConverter";
								if (flag14)
								{
									string prefix2;
									TypeSig sig2 = this.ResolveType(rec.Value.Trim(), out prefix2);
									bool flag15 = sig2 != null && this.context.Modules.Contains((ModuleDefMD)sig2.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module);
									if (flag15)
									{
										BAMLConverterTypeReference reference3 = new BAMLConverterTypeReference(this.xmlnsCtx, sig2, rec);
										this.AddTypeSigReference(sig2, reference3);
									}
								}
							}
						}
					}
				}
			}
			Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attrInfo = this.ResolveAttribute(rec.AttributeId);
			string attrName = null;
			bool flag16 = attrInfo.Item1 != null;
			if (flag16)
			{
				attrName = attrInfo.Item1.Name;
			}
			else
			{
				bool flag17 = attrInfo.Item2 != null;
				if (flag17)
				{
					attrName = attrInfo.Item2.Name;
				}
			}
			bool flag18 = attrName == "DisplayMemberPath";
			if (flag18)
			{
				this.AnalyzePropertyPath(rec.Value);
			}
			else
			{
				bool flag19 = attrName == "Source";
				if (flag19)
				{
					string declType = null;
					bool flag20 = attrInfo.Item1 is IMemberDef;
					if (flag20)
					{
						declType = ((IMemberDef)attrInfo.Item1).DeclaringType.FullName;
					}
					else
					{
						bool flag21 = attrInfo.Item2 != null;
						if (flag21)
						{
							declType = this.ResolveType(attrInfo.Item2.OwnerTypeId).FullName;
						}
					}
					bool flag22 = declType == "System.Windows.ResourceDictionary";
					if (flag22)
					{
						string src = rec.Value.ToUpperInvariant();
						bool flag23 = src.EndsWith(".BAML") || src.EndsWith(".XAML");
						if (flag23)
						{
							Match match = WPFAnalyzer.UriPattern.Match(src);
							bool success = match.Success;
							if (success)
							{
								src = match.Groups[1].Value;
							}
							else
							{
								bool flag24 = rec.Value.Contains("/");
								if (flag24)
								{
									this.context.Logger.WarnFormat("Fail to extract XAML name from '{0}'.", new object[]
									{
										rec.Value
									});
								}
							}
							bool flag25 = !src.Contains("//");
							if (flag25)
							{
								Uri rel = new Uri(new Uri(this.packScheme + "application:,,,/" + this.CurrentBAMLName), src);
								src = rel.LocalPath;
							}
							BAMLPropertyReference reference4 = new BAMLPropertyReference(rec);
							src = src.TrimStart(new char[]
							{
								'/'
							});
							string baml = src.Substring(0, src.Length - 5) + ".BAML";
							string xaml = src.Substring(0, src.Length - 5) + ".XAML";
							Dictionary<string, List<IBAMLReference>> bamlRefs = this.service.FindRenamer<WPFAnalyzer>().bamlRefs;
							bamlRefs.AddListEntry(baml, reference4);
							bamlRefs.AddListEntry(xaml, reference4);
							bamlRefs.AddListEntry(Uri.EscapeUriString(baml), reference4);
							bamlRefs.AddListEntry(Uri.EscapeUriString(xaml), reference4);
						}
					}
				}
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00008EE0 File Offset: 0x000070E0
		private Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> AnalyzeAttributeReference(TypeDef declType, AttributeInfoRecord rec)
		{
			IDnlibDef retDef = null;
			ITypeDefOrRef retType = null;
			while (declType != null)
			{
				PropertyDef property = declType.FindProperty(rec.Name);
				bool flag = property != null;
				if (flag)
				{
					retDef = property;
					retType = property.PropertySig.RetType.ToBasicTypeDefOrRef();
					bool flag2 = this.context.Modules.Contains((ModuleDefMD)declType.Module);
					if (flag2)
					{
						this.service.AddReference<IDnlibDef>(property, new BAMLAttributeReference(property, rec));
					}
					break;
				}
				EventDef evt = declType.FindEvent(rec.Name);
				bool flag3 = evt != null;
				if (flag3)
				{
					retDef = evt;
					retType = evt.EventType;
					bool flag4 = this.context.Modules.Contains((ModuleDefMD)declType.Module);
					if (flag4)
					{
						this.service.AddReference<IDnlibDef>(evt, new BAMLAttributeReference(evt, rec));
					}
					break;
				}
				bool flag5 = declType.BaseType == null;
				if (flag5)
				{
					break;
				}
				declType = declType.BaseType.ResolveTypeDefThrow();
			}
			return Tuple.Create<IDnlibDef, AttributeInfoRecord, TypeDef>(retDef, rec, (retType == null) ? null : retType.ResolveTypeDefThrow());
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00009004 File Offset: 0x00007204
		private void AnalyzePropertyPath(string path)
		{
			PropertyPath propertyPath = new PropertyPath(path);
			foreach (PropertyPathPart part in propertyPath.Parts)
			{
				bool flag = part.IsAttachedDP();
				if (flag)
				{
					string type;
					string property;
					part.ExtractAttachedDP(out type, out property);
					bool flag2 = type != null;
					if (flag2)
					{
						string prefix;
						TypeSig sig = this.ResolveType(type, out prefix);
						bool flag3 = sig != null && this.context.Modules.Contains((ModuleDefMD)sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module);
						if (flag3)
						{
							BAMLPathTypeReference reference = new BAMLPathTypeReference(this.xmlnsCtx, sig, part);
							this.AddTypeSigReference(sig, reference);
						}
					}
				}
				else
				{
					List<PropertyDef> candidates;
					bool flag4 = this.properties.TryGetValue(part.Name, out candidates);
					if (flag4)
					{
						foreach (PropertyDef property2 in candidates)
						{
							this.service.SetCanRename(property2, false);
						}
					}
				}
				bool flag5 = part.IndexerArguments != null;
				if (flag5)
				{
					foreach (PropertyPathIndexer indexer in part.IndexerArguments)
					{
						bool flag6 = !string.IsNullOrEmpty(indexer.Type);
						if (flag6)
						{
							string prefix2;
							TypeSig sig2 = this.ResolveType(indexer.Type, out prefix2);
							bool flag7 = sig2 != null && this.context.Modules.Contains((ModuleDefMD)sig2.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module);
							if (flag7)
							{
								BAMLPathTypeReference reference2 = new BAMLPathTypeReference(this.xmlnsCtx, sig2, part);
								this.AddTypeSigReference(sig2, reference2);
							}
						}
					}
				}
			}
		}

		// Token: 0x04000072 RID: 114
		private readonly ConfuserContext context;

		// Token: 0x04000073 RID: 115
		private readonly INameService service;

		// Token: 0x04000074 RID: 116
		private readonly Dictionary<string, List<MethodDef>> methods = new Dictionary<string, List<MethodDef>>();

		// Token: 0x04000075 RID: 117
		private readonly Dictionary<string, List<EventDef>> events = new Dictionary<string, List<EventDef>>();

		// Token: 0x04000076 RID: 118
		private readonly Dictionary<string, List<PropertyDef>> properties = new Dictionary<string, List<PropertyDef>>();

		// Token: 0x04000077 RID: 119
		private readonly Dictionary<ushort, AssemblyDef> assemblyRefs = new Dictionary<ushort, AssemblyDef>();

		// Token: 0x04000078 RID: 120
		private readonly Dictionary<ushort, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef>> attrRefs = new Dictionary<ushort, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef>>();

		// Token: 0x04000079 RID: 121
		private readonly Dictionary<ushort, StringInfoRecord> strings = new Dictionary<ushort, StringInfoRecord>();

		// Token: 0x0400007A RID: 122
		private readonly Dictionary<ushort, TypeSig> typeRefs = new Dictionary<ushort, TypeSig>();

		// Token: 0x0400007B RID: 123
		private readonly Dictionary<string, List<Tuple<AssemblyDef, string>>> xmlns = new Dictionary<string, List<Tuple<AssemblyDef, string>>>();

		// Token: 0x0400007C RID: 124
		private readonly string packScheme = PackUriHelper.UriSchemePack + "://";

		// Token: 0x0400007D RID: 125
		private IKnownThings things;

		// Token: 0x0400007E RID: 126
		private KnownThingsv3 thingsv3;

		// Token: 0x0400007F RID: 127
		private KnownThingsv4 thingsv4;

		// Token: 0x04000080 RID: 128
		private BAMLAnalyzer.XmlNsContext xmlnsCtx;

		// Token: 0x04000081 RID: 129
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<BAMLAnalyzer, BamlElement> AnalyzeElement;

		// Token: 0x04000082 RID: 130
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string <CurrentBAMLName>k__BackingField;

		// Token: 0x04000083 RID: 131
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private ModuleDefMD <Module>k__BackingField;

		// Token: 0x0200002E RID: 46
		private class DummyAssemblyRefFinder : IAssemblyRefFinder
		{
			// Token: 0x06000104 RID: 260 RVA: 0x00002645 File Offset: 0x00000845
			public DummyAssemblyRefFinder(AssemblyDef assemblyDef)
			{
				this.assemblyDef = assemblyDef;
			}

			// Token: 0x06000105 RID: 261 RVA: 0x000091E0 File Offset: 0x000073E0
			public AssemblyRef FindAssemblyRef(TypeRef nonNestedTypeRef)
			{
				return this.assemblyDef.ToAssemblyRef();
			}

			// Token: 0x04000084 RID: 132
			private readonly AssemblyDef assemblyDef;
		}

		// Token: 0x0200002F RID: 47
		internal class XmlNsContext
		{
			// Token: 0x06000106 RID: 262 RVA: 0x00009200 File Offset: 0x00007400
			public XmlNsContext(BamlDocument doc, Dictionary<ushort, AssemblyDef> assemblyRefs)
			{
				this.doc = doc;
				this.assemblyRefs = new Dictionary<AssemblyDef, ushort>();
				foreach (KeyValuePair<ushort, AssemblyDef> entry in assemblyRefs)
				{
					this.assemblyRefs[entry.Value] = entry.Key;
				}
				for (int i = 0; i < doc.Count; i++)
				{
					bool flag = doc[i] is ElementStartRecord;
					if (flag)
					{
						this.rootIndex = i + 1;
						break;
					}
				}
				Debug.Assert(this.rootIndex != -1);
			}

			// Token: 0x06000107 RID: 263 RVA: 0x00002656 File Offset: 0x00000856
			public void AddNsMap(Tuple<AssemblyDef, string> scope, string prefix)
			{
				this.xmlNsMap[scope] = prefix;
			}

			// Token: 0x06000108 RID: 264 RVA: 0x000092D8 File Offset: 0x000074D8
			public string GetPrefix(string clrNs, AssemblyDef assembly)
			{
				string prefix;
				bool flag = !this.xmlNsMap.TryGetValue(Tuple.Create<AssemblyDef, string>(assembly, clrNs), out prefix);
				if (flag)
				{
					object arg = "_";
					int num = this.x;
					this.x = num + 1;
					prefix = arg + num;
					ushort assemblyId = this.assemblyRefs[assembly];
					this.doc.Insert(this.rootIndex, new XmlnsPropertyRecord
					{
						AssemblyIds = new ushort[]
						{
							assemblyId
						},
						Prefix = prefix,
						XmlNamespace = "clr-namespace:" + clrNs
					});
					this.doc.Insert(this.rootIndex - 1, new PIMappingRecord
					{
						AssemblyId = assemblyId,
						ClrNamespace = clrNs,
						XmlNamespace = "clr-namespace:" + clrNs
					});
					this.rootIndex++;
				}
				return prefix;
			}

			// Token: 0x04000085 RID: 133
			private readonly Dictionary<AssemblyDef, ushort> assemblyRefs;

			// Token: 0x04000086 RID: 134
			private readonly BamlDocument doc;

			// Token: 0x04000087 RID: 135
			private readonly Dictionary<Tuple<AssemblyDef, string>, string> xmlNsMap = new Dictionary<Tuple<AssemblyDef, string>, string>();

			// Token: 0x04000088 RID: 136
			private int rootIndex = -1;

			// Token: 0x04000089 RID: 137
			private int x;
		}

		// Token: 0x02000030 RID: 48
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000109 RID: 265 RVA: 0x00002667 File Offset: 0x00000867
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600010A RID: 266 RVA: 0x00002184 File Offset: 0x00000384
			public <>c()
			{
			}

			// Token: 0x0600010B RID: 267 RVA: 0x00002673 File Offset: 0x00000873
			internal IEnumerable<TypeDef> <PreInit>b__31_0(ModuleDefMD m)
			{
				return m.GetTypes();
			}

			// Token: 0x0600010C RID: 268 RVA: 0x0000267B File Offset: 0x0000087B
			internal bool <Analyze>b__35_0(BamlRecord rec)
			{
				return rec is LineNumberAndPositionRecord || rec is LinePositionRecord;
			}

			// Token: 0x0400008A RID: 138
			public static readonly BAMLAnalyzer.<>c <>9 = new BAMLAnalyzer.<>c();

			// Token: 0x0400008B RID: 139
			public static Func<ModuleDefMD, IEnumerable<TypeDef>> <>9__31_0;

			// Token: 0x0400008C RID: 140
			public static Predicate<BamlRecord> <>9__35_0;
		}

		// Token: 0x02000031 RID: 49
		[CompilerGenerated]
		private sealed class <>c__DisplayClass36_0
		{
			// Token: 0x0600010D RID: 269 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass36_0()
			{
			}

			// Token: 0x0600010E RID: 270 RVA: 0x00002691 File Offset: 0x00000891
			internal bool <PopulateReferences>b__0(ModuleDefMD m)
			{
				return m.Assembly == this.assembly;
			}

			// Token: 0x0400008D RID: 141
			public AssemblyDef assembly;
		}
	}
}
