using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers
{
	// Token: 0x020000AB RID: 171
	public static class InjectHelper
	{
		// Token: 0x060003CE RID: 974 RVA: 0x00016E80 File Offset: 0x00015080
		private static TypeDefUser Clone(TypeDef origin)
		{
			TypeDefUser ret = new TypeDefUser(origin.Namespace, origin.Name);
			ret.Attributes = origin.Attributes;
			bool flag = origin.ClassLayout != null;
			if (flag)
			{
				ret.ClassLayout = new ClassLayoutUser(origin.ClassLayout.PackingSize, origin.ClassSize);
			}
			foreach (GenericParam genericParam in origin.GenericParameters)
			{
				ret.GenericParameters.Add(new GenericParamUser(genericParam.Number, genericParam.Flags, "-"));
			}
			return ret;
		}

		// Token: 0x060003CF RID: 975 RVA: 0x00016F40 File Offset: 0x00015140
		private static MethodDefUser Clone(MethodDef origin)
		{
			MethodDefUser ret = new MethodDefUser(origin.Name, null, origin.ImplAttributes, origin.Attributes);
			foreach (GenericParam genericParam in origin.GenericParameters)
			{
				ret.GenericParameters.Add(new GenericParamUser(genericParam.Number, genericParam.Flags, "-"));
			}
			return ret;
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00016FD0 File Offset: 0x000151D0
		private static FieldDefUser Clone(FieldDef origin)
		{
			return new FieldDefUser(origin.Name, null, origin.Attributes);
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00016FF8 File Offset: 0x000151F8
		private static TypeDef PopulateContext(TypeDef typeDef, InjectHelper.InjectContext ctx)
		{
			IDnlibDef existing;
			bool flag = !ctx.Map.TryGetValue(typeDef, out existing);
			TypeDef ret;
			if (flag)
			{
				ret = InjectHelper.Clone(typeDef);
				ctx.Map[typeDef] = ret;
			}
			else
			{
				ret = (TypeDef)existing;
			}
			foreach (TypeDef nestedType in typeDef.NestedTypes)
			{
				ret.NestedTypes.Add(InjectHelper.PopulateContext(nestedType, ctx));
			}
			foreach (MethodDef method in typeDef.Methods)
			{
				ret.Methods.Add((MethodDef)(ctx.Map[method] = InjectHelper.Clone(method)));
			}
			foreach (FieldDef field in typeDef.Fields)
			{
				ret.Fields.Add((FieldDef)(ctx.Map[field] = InjectHelper.Clone(field)));
			}
			return ret;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00017164 File Offset: 0x00015364
		private static void CopyTypeDef(TypeDef typeDef, InjectHelper.InjectContext ctx)
		{
			TypeDef newTypeDef = (TypeDef)ctx.Map[typeDef];
			newTypeDef.BaseType = (ITypeDefOrRef)ctx.Importer.Import(typeDef.BaseType);
			foreach (InterfaceImpl iface in typeDef.Interfaces)
			{
				newTypeDef.Interfaces.Add(new InterfaceImplUser((ITypeDefOrRef)ctx.Importer.Import(iface.Interface)));
			}
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0001720C File Offset: 0x0001540C
		private static void CopyMethodDef(MethodDef methodDef, InjectHelper.InjectContext ctx)
		{
			MethodDef newMethodDef = (MethodDef)ctx.Map[methodDef];
			newMethodDef.Signature = ctx.Importer.Import(methodDef.Signature);
			newMethodDef.Parameters.UpdateParameterTypes();
			bool flag = methodDef.ImplMap != null;
			if (flag)
			{
				newMethodDef.ImplMap = new ImplMapUser(new ModuleRefUser(ctx.TargetModule, methodDef.ImplMap.Module.Name), methodDef.ImplMap.Name, methodDef.ImplMap.Attributes);
			}
			foreach (CustomAttribute ca in methodDef.CustomAttributes)
			{
				newMethodDef.CustomAttributes.Add(new CustomAttribute((ICustomAttributeType)ctx.Importer.Import(ca.Constructor)));
			}
			bool hasBody = methodDef.HasBody;
			if (hasBody)
			{
				newMethodDef.Body = new CilBody(methodDef.Body.InitLocals, new List<Instruction>(), new List<ExceptionHandler>(), new List<Local>());
				newMethodDef.Body.MaxStack = methodDef.Body.MaxStack;
				Dictionary<object, object> bodyMap = new Dictionary<object, object>();
				foreach (Local local in methodDef.Body.Variables)
				{
					Local newLocal = new Local(ctx.Importer.Import(local.Type));
					newMethodDef.Body.Variables.Add(newLocal);
					newLocal.Name = local.Name;
					newLocal.PdbAttributes = local.PdbAttributes;
					bodyMap[local] = newLocal;
				}
				foreach (Instruction instr in methodDef.Body.Instructions)
				{
					Instruction newInstr = new Instruction(instr.OpCode, instr.Operand);
					newInstr.SequencePoint = instr.SequencePoint;
					bool flag2 = newInstr.Operand is IType;
					if (flag2)
					{
						newInstr.Operand = ctx.Importer.Import((IType)newInstr.Operand);
					}
					else
					{
						bool flag3 = newInstr.Operand is IMethod;
						if (flag3)
						{
							newInstr.Operand = ctx.Importer.Import((IMethod)newInstr.Operand);
						}
						else
						{
							bool flag4 = newInstr.Operand is IField;
							if (flag4)
							{
								newInstr.Operand = ctx.Importer.Import((IField)newInstr.Operand);
							}
						}
					}
					newMethodDef.Body.Instructions.Add(newInstr);
					bodyMap[instr] = newInstr;
				}
				Func<Instruction, Instruction> <>9__0;
				foreach (Instruction instr2 in newMethodDef.Body.Instructions)
				{
					bool flag5 = instr2.Operand != null && bodyMap.ContainsKey(instr2.Operand);
					if (flag5)
					{
						instr2.Operand = bodyMap[instr2.Operand];
					}
					else
					{
						bool flag6 = instr2.Operand is Instruction[];
						if (flag6)
						{
							Instruction instruction = instr2;
							IEnumerable<Instruction> source = (Instruction[])instr2.Operand;
							Func<Instruction, Instruction> selector;
							if ((selector = <>9__0) == null)
							{
								selector = (<>9__0 = ((Instruction target) => (Instruction)bodyMap[target]));
							}
							instruction.Operand = source.Select(selector).ToArray<Instruction>();
						}
					}
				}
				foreach (ExceptionHandler eh in methodDef.Body.ExceptionHandlers)
				{
					newMethodDef.Body.ExceptionHandlers.Add(new ExceptionHandler(eh.HandlerType)
					{
						CatchType = ((eh.CatchType == null) ? null : ((ITypeDefOrRef)ctx.Importer.Import(eh.CatchType))),
						TryStart = (Instruction)bodyMap[eh.TryStart],
						TryEnd = (Instruction)bodyMap[eh.TryEnd],
						HandlerStart = (Instruction)bodyMap[eh.HandlerStart],
						HandlerEnd = (Instruction)bodyMap[eh.HandlerEnd],
						FilterStart = ((eh.FilterStart == null) ? null : ((Instruction)bodyMap[eh.FilterStart]))
					});
				}
				newMethodDef.Body.SimplifyMacros(newMethodDef.Parameters);
			}
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000177A0 File Offset: 0x000159A0
		private static void CopyFieldDef(FieldDef fieldDef, InjectHelper.InjectContext ctx)
		{
			FieldDef newFieldDef = (FieldDef)ctx.Map[fieldDef];
			newFieldDef.Signature = ctx.Importer.Import(fieldDef.Signature);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x000177DC File Offset: 0x000159DC
		private static void Copy(TypeDef typeDef, InjectHelper.InjectContext ctx, bool copySelf)
		{
			if (copySelf)
			{
				InjectHelper.CopyTypeDef(typeDef, ctx);
			}
			foreach (TypeDef nestedType in typeDef.NestedTypes)
			{
				InjectHelper.Copy(nestedType, ctx, true);
			}
			foreach (MethodDef method in typeDef.Methods)
			{
				InjectHelper.CopyMethodDef(method, ctx);
			}
			foreach (FieldDef field in typeDef.Fields)
			{
				InjectHelper.CopyFieldDef(field, ctx);
			}
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x000178C4 File Offset: 0x00015AC4
		public static TypeDef Inject(TypeDef typeDef, ModuleDef target)
		{
			InjectHelper.InjectContext ctx = new InjectHelper.InjectContext(typeDef.Module, target);
			InjectHelper.PopulateContext(typeDef, ctx);
			InjectHelper.Copy(typeDef, ctx, true);
			return (TypeDef)ctx.Map[typeDef];
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00017908 File Offset: 0x00015B08
		public static MethodDef Inject(MethodDef methodDef, ModuleDef target)
		{
			InjectHelper.InjectContext ctx = new InjectHelper.InjectContext(methodDef.Module, target);
			ctx.Map[methodDef] = InjectHelper.Clone(methodDef);
			InjectHelper.CopyMethodDef(methodDef, ctx);
			return (MethodDef)ctx.Map[methodDef];
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00017954 File Offset: 0x00015B54
		public static IEnumerable<IDnlibDef> Inject(TypeDef typeDef, TypeDef newType, ModuleDef target)
		{
			InjectHelper.InjectContext ctx = new InjectHelper.InjectContext(typeDef.Module, target);
			ctx.Map[typeDef] = newType;
			InjectHelper.PopulateContext(typeDef, ctx);
			InjectHelper.Copy(typeDef, ctx, false);
			return ctx.Map.Values.Except(new TypeDef[]
			{
				newType
			});
		}

		// Token: 0x020000AC RID: 172
		private class InjectContext : ImportResolver
		{
			// Token: 0x060003D9 RID: 985 RVA: 0x0000395F File Offset: 0x00001B5F
			public InjectContext(ModuleDef module, ModuleDef target)
			{
				this.OriginModule = module;
				this.TargetModule = target;
				this.importer = new Importer(target, ImporterOptions.TryToUseTypeDefs);
				this.importer.Resolver = this;
			}

			// Token: 0x17000090 RID: 144
			// (get) Token: 0x060003DA RID: 986 RVA: 0x000179AC File Offset: 0x00015BAC
			public Importer Importer
			{
				get
				{
					return this.importer;
				}
			}

			// Token: 0x060003DB RID: 987 RVA: 0x000179C4 File Offset: 0x00015BC4
			public override TypeDef Resolve(TypeDef typeDef)
			{
				bool flag = this.Map.ContainsKey(typeDef);
				TypeDef result;
				if (flag)
				{
					result = (TypeDef)this.Map[typeDef];
				}
				else
				{
					result = null;
				}
				return result;
			}

			// Token: 0x060003DC RID: 988 RVA: 0x000179FC File Offset: 0x00015BFC
			public override MethodDef Resolve(MethodDef methodDef)
			{
				bool flag = this.Map.ContainsKey(methodDef);
				MethodDef result;
				if (flag)
				{
					result = (MethodDef)this.Map[methodDef];
				}
				else
				{
					result = null;
				}
				return result;
			}

			// Token: 0x060003DD RID: 989 RVA: 0x00017A34 File Offset: 0x00015C34
			public override FieldDef Resolve(FieldDef fieldDef)
			{
				bool flag = this.Map.ContainsKey(fieldDef);
				FieldDef result;
				if (flag)
				{
					result = (FieldDef)this.Map[fieldDef];
				}
				else
				{
					result = null;
				}
				return result;
			}

			// Token: 0x04000289 RID: 649
			public readonly Dictionary<IDnlibDef, IDnlibDef> Map = new Dictionary<IDnlibDef, IDnlibDef>();

			// Token: 0x0400028A RID: 650
			public readonly ModuleDef OriginModule;

			// Token: 0x0400028B RID: 651
			public readonly ModuleDef TargetModule;

			// Token: 0x0400028C RID: 652
			private readonly Importer importer;
		}

		// Token: 0x020000AD RID: 173
		[CompilerGenerated]
		private sealed class <>c__DisplayClass5_0
		{
			// Token: 0x060003DE RID: 990 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass5_0()
			{
			}

			// Token: 0x060003DF RID: 991 RVA: 0x0000399C File Offset: 0x00001B9C
			internal Instruction <CopyMethodDef>b__0(Instruction target)
			{
				return (Instruction)this.bodyMap[target];
			}

			// Token: 0x0400028D RID: 653
			public Dictionary<object, object> bodyMap;

			// Token: 0x0400028E RID: 654
			public Func<Instruction, Instruction> <>9__0;
		}
	}
}
