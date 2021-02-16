using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000080 RID: 128
	internal class TypeBlobAnalyzer : IRenamer
	{
		// Token: 0x060002E4 RID: 740 RVA: 0x0002231C File Offset: 0x0002051C
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDefMD module = def as ModuleDefMD;
			bool flag = module == null;
			if (!flag)
			{
				MDTable table = module.TablesStream.Get(Table.Method);
				uint len = table.Rows;
				IEnumerable<MethodDef> methods = from rid in Enumerable.Range(1, (int)len)
				select module.ResolveMethod((uint)rid);
				foreach (MethodDef method in methods)
				{
					foreach (MethodOverride methodImpl in method.Overrides)
					{
						bool flag2 = methodImpl.MethodBody is MemberRef;
						if (flag2)
						{
							this.AnalyzeMemberRef(context, service, (MemberRef)methodImpl.MethodBody);
						}
						bool flag3 = methodImpl.MethodDeclaration is MemberRef;
						if (flag3)
						{
							this.AnalyzeMemberRef(context, service, (MemberRef)methodImpl.MethodDeclaration);
						}
					}
					bool hasBody = method.HasBody;
					if (hasBody)
					{
						foreach (Instruction instr in method.Body.Instructions)
						{
							bool flag4 = instr.Operand is MemberRef;
							if (flag4)
							{
								this.AnalyzeMemberRef(context, service, (MemberRef)instr.Operand);
							}
							else
							{
								bool flag5 = instr.Operand is MethodSpec;
								if (flag5)
								{
									MethodSpec spec = (MethodSpec)instr.Operand;
									bool flag6 = spec.Method is MemberRef;
									if (flag6)
									{
										this.AnalyzeMemberRef(context, service, (MemberRef)spec.Method);
									}
								}
							}
						}
					}
				}
				table = module.TablesStream.Get(Table.CustomAttribute);
				len = table.Rows;
				IEnumerable<CustomAttribute> attrs = (from rid in Enumerable.Range(1, (int)len)
				select module.ResolveHasCustomAttribute(module.TablesStream.ReadCustomAttributeRow((uint)rid).Parent)).Distinct<IHasCustomAttribute>().SelectMany((IHasCustomAttribute owner) => owner.CustomAttributes);
				foreach (CustomAttribute attr in attrs)
				{
					bool flag7 = attr.Constructor is MemberRef;
					if (flag7)
					{
						this.AnalyzeMemberRef(context, service, (MemberRef)attr.Constructor);
					}
					foreach (CAArgument arg in attr.ConstructorArguments)
					{
						this.AnalyzeCAArgument(context, service, arg);
					}
					foreach (CANamedArgument arg2 in attr.Fields)
					{
						this.AnalyzeCAArgument(context, service, arg2.Argument);
					}
					foreach (CANamedArgument arg3 in attr.Properties)
					{
						this.AnalyzeCAArgument(context, service, arg3.Argument);
					}
					TypeDef attrType = attr.AttributeType.ResolveTypeDefThrow();
					bool flag8 = context.Modules.Contains((ModuleDefMD)attrType.Module);
					if (flag8)
					{
						foreach (CANamedArgument fieldArg in attr.Fields)
						{
							FieldDef field = attrType.FindField(fieldArg.Name, new FieldSig(fieldArg.Type));
							bool flag9 = field == null;
							if (flag9)
							{
								context.Logger.WarnFormat("Failed to resolve CA field '{0}::{1} : {2}'.", new object[]
								{
									attrType,
									fieldArg.Name,
									fieldArg.Type
								});
							}
							else
							{
								service.AddReference<IDnlibDef>(field, new CAMemberReference(fieldArg, field));
							}
						}
						foreach (CANamedArgument propertyArg in attr.Properties)
						{
							PropertyDef property = attrType.FindProperty(propertyArg.Name, new PropertySig(true, propertyArg.Type));
							bool flag10 = property == null;
							if (flag10)
							{
								context.Logger.WarnFormat("Failed to resolve CA property '{0}::{1} : {2}'.", new object[]
								{
									attrType,
									propertyArg.Name,
									propertyArg.Type
								});
							}
							else
							{
								service.AddReference<IDnlibDef>(property, new CAMemberReference(propertyArg, property));
							}
						}
					}
				}
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x000228F4 File Offset: 0x00020AF4
		private void AnalyzeCAArgument(ConfuserContext context, INameService service, CAArgument arg)
		{
			bool flag = arg.Type.DefinitionAssembly.IsCorLib() && arg.Type.FullName == "System.Type";
			if (flag)
			{
				TypeSig typeSig = (TypeSig)arg.Value;
				using (IEnumerator<ITypeDefOrRef> enumerator = typeSig.FindTypeRefs().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ITypeDefOrRef typeRef = enumerator.Current;
						TypeDef typeDef = typeRef.ResolveTypeDefThrow();
						bool flag2 = context.Modules.Contains((ModuleDefMD)typeDef.Module);
						if (flag2)
						{
							bool flag3 = typeRef is TypeRef;
							if (flag3)
							{
								service.AddReference<TypeDef>(typeDef, new TypeRefReference((TypeRef)typeRef, typeDef));
							}
							service.ReduceRenameMode(typeDef, RenameMode.ASCII);
						}
					}
					return;
				}
			}
			bool flag4 = arg.Value is CAArgument[];
			if (flag4)
			{
				foreach (CAArgument elem in (CAArgument[])arg.Value)
				{
					this.AnalyzeCAArgument(context, service, elem);
				}
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00022A34 File Offset: 0x00020C34
		private void AnalyzeMemberRef(ConfuserContext context, INameService service, MemberRef memberRef)
		{
			ITypeDefOrRef declType = memberRef.DeclaringType;
			TypeSpec typeSpec = declType as TypeSpec;
			bool flag = typeSpec == null;
			if (!flag)
			{
				TypeSig sig = typeSpec.TypeSig;
				while (sig.Next != null)
				{
					sig = sig.Next;
				}
				bool flag2 = sig is GenericInstSig;
				if (flag2)
				{
					GenericInstSig inst = (GenericInstSig)sig;
					TypeDef openType = inst.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
					bool flag3 = !context.Modules.Contains((ModuleDefMD)openType.Module) || memberRef.IsArrayAccessors();
					if (!flag3)
					{
						bool isFieldRef = memberRef.IsFieldRef;
						IDnlibDef member;
						if (isFieldRef)
						{
							member = memberRef.ResolveFieldThrow();
						}
						else
						{
							bool flag4 = !memberRef.IsMethodRef;
							if (flag4)
							{
								throw new UnreachableException();
							}
							member = memberRef.ResolveMethodThrow();
						}
						service.AddReference<IDnlibDef>(member, new MemberRefReference(memberRef, member));
					}
				}
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00002184 File Offset: 0x00000384
		public TypeBlobAnalyzer()
		{
		}

		// Token: 0x02000081 RID: 129
		[CompilerGenerated]
		private sealed class <>c__DisplayClass0_0
		{
			// Token: 0x060002EA RID: 746 RVA: 0x00002184 File Offset: 0x00000384
			public <>c__DisplayClass0_0()
			{
			}

			// Token: 0x060002EB RID: 747 RVA: 0x000035C4 File Offset: 0x000017C4
			internal MethodDef <Analyze>b__0(int rid)
			{
				return this.module.ResolveMethod((uint)rid);
			}

			// Token: 0x060002EC RID: 748 RVA: 0x000035D2 File Offset: 0x000017D2
			internal IHasCustomAttribute <Analyze>b__1(int rid)
			{
				return this.module.ResolveHasCustomAttribute(this.module.TablesStream.ReadCustomAttributeRow((uint)rid).Parent);
			}

			// Token: 0x0400053B RID: 1339
			public ModuleDefMD module;
		}

		// Token: 0x02000082 RID: 130
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060002ED RID: 749 RVA: 0x000035F5 File Offset: 0x000017F5
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060002EE RID: 750 RVA: 0x00002184 File Offset: 0x00000384
			public <>c()
			{
			}

			// Token: 0x060002EF RID: 751 RVA: 0x00003601 File Offset: 0x00001801
			internal IEnumerable<CustomAttribute> <Analyze>b__0_2(IHasCustomAttribute owner)
			{
				return owner.CustomAttributes;
			}

			// Token: 0x0400053C RID: 1340
			public static readonly TypeBlobAnalyzer.<>c <>9 = new TypeBlobAnalyzer.<>c();

			// Token: 0x0400053D RID: 1341
			public static Func<IHasCustomAttribute, IEnumerable<CustomAttribute>> <>9__0_2;
		}
	}
}
