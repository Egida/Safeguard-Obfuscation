using System;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x0200007C RID: 124
	internal class InterReferenceAnalyzer : IRenamer
	{
		// Token: 0x060002D1 RID: 721 RVA: 0x0002144C File Offset: 0x0001F64C
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDefMD module = def as ModuleDefMD;
			bool flag = module == null;
			if (!flag)
			{
				MDTable table = module.TablesStream.Get(Table.Method);
				uint len = table.Rows;
				for (uint i = 1U; i <= len; i += 1U)
				{
					MethodDef methodDef = module.ResolveMethod(i);
					foreach (MethodOverride ov in methodDef.Overrides)
					{
						this.ProcessMemberRef(context, service, module, ov.MethodBody);
						this.ProcessMemberRef(context, service, module, ov.MethodDeclaration);
					}
					bool hasBody = methodDef.HasBody;
					if (hasBody)
					{
						foreach (Instruction instr in methodDef.Body.Instructions)
						{
							bool flag2 = instr.Operand is MemberRef || instr.Operand is MethodSpec;
							if (flag2)
							{
								this.ProcessMemberRef(context, service, module, (IMemberRef)instr.Operand);
							}
						}
					}
				}
				table = module.TablesStream.Get(Table.TypeRef);
				len = table.Rows;
				for (uint j = 1U; j <= len; j += 1U)
				{
					TypeRef typeRef = module.ResolveTypeRef(j);
					TypeDef typeDef = typeRef.ResolveTypeDefThrow();
					bool flag3 = typeDef.Module != module && context.Modules.Contains((ModuleDefMD)typeDef.Module);
					if (flag3)
					{
						service.AddReference<TypeDef>(typeDef, new TypeRefReference(typeRef, typeDef));
					}
				}
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00021628 File Offset: 0x0001F828
		private void ProcessMemberRef(ConfuserContext context, INameService service, ModuleDefMD module, IMemberRef r)
		{
			MemberRef memberRef = r as MemberRef;
			bool flag = r is MethodSpec;
			if (flag)
			{
				memberRef = (((MethodSpec)r).Method as MemberRef);
			}
			bool flag2 = memberRef != null;
			if (flag2)
			{
				bool flag3 = memberRef.DeclaringType.TryGetArraySig() != null;
				if (!flag3)
				{
					TypeDef declType = memberRef.DeclaringType.ResolveTypeDefThrow();
					bool flag4 = declType.Module != module && context.Modules.Contains((ModuleDefMD)declType.Module);
					if (flag4)
					{
						IDnlibDef memberDef = (IDnlibDef)declType.ResolveThrow(memberRef);
						service.AddReference<IDnlibDef>(memberDef, new MemberRefReference(memberRef, memberDef));
					}
				}
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00002184 File Offset: 0x00000384
		public InterReferenceAnalyzer()
		{
		}
	}
}
