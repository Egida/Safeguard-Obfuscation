using System;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x0200007D RID: 125
	internal class LdtokenEnumAnalyzer : IRenamer
	{
		// Token: 0x060002D6 RID: 726 RVA: 0x000216D8 File Offset: 0x0001F8D8
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			MethodDef method = def as MethodDef;
			bool flag = method == null || !method.HasBody;
			if (!flag)
			{
				for (int i = 0; i < method.Body.Instructions.Count; i++)
				{
					Instruction instr = method.Body.Instructions[i];
					bool flag2 = instr.OpCode.Code == Code.Ldtoken;
					if (flag2)
					{
						bool flag3 = instr.Operand is MemberRef;
						if (flag3)
						{
							IMemberForwarded member = ((MemberRef)instr.Operand).ResolveThrow();
							bool flag4 = context.Modules.Contains((ModuleDefMD)member.Module);
							if (flag4)
							{
								service.SetCanRename(member, false);
							}
						}
						else
						{
							bool flag5 = instr.Operand is IField;
							if (flag5)
							{
								FieldDef field = ((IField)instr.Operand).ResolveThrow();
								bool flag6 = context.Modules.Contains((ModuleDefMD)field.Module);
								if (flag6)
								{
									service.SetCanRename(field, false);
								}
							}
							else
							{
								bool flag7 = instr.Operand is IMethod;
								if (flag7)
								{
									IMethod im = (IMethod)instr.Operand;
									bool flag8 = !im.IsArrayAccessors();
									if (flag8)
									{
										MethodDef j = im.ResolveThrow();
										bool flag9 = context.Modules.Contains((ModuleDefMD)j.Module);
										if (flag9)
										{
											service.SetCanRename(method, false);
										}
									}
								}
								else
								{
									bool flag10 = !(instr.Operand is ITypeDefOrRef);
									if (flag10)
									{
										throw new UnreachableException();
									}
									bool flag11 = !(instr.Operand is TypeSpec);
									if (flag11)
									{
										TypeDef type = ((ITypeDefOrRef)instr.Operand).ResolveTypeDefThrow();
										bool flag12 = context.Modules.Contains((ModuleDefMD)type.Module) && this.HandleTypeOf(context, service, method, i);
										if (flag12)
										{
											TypeDef t = type;
											do
											{
												this.DisableRename(service, t, false);
												t = t.DeclaringType;
											}
											while (t != null);
										}
									}
								}
							}
						}
					}
					else
					{
						bool flag13 = (instr.OpCode.Code == Code.Call || instr.OpCode.Code == Code.Callvirt) && ((IMethod)instr.Operand).Name == "ToString";
						if (flag13)
						{
							this.HandleEnum(context, service, method, i);
						}
						else
						{
							bool flag14 = instr.OpCode.Code == Code.Ldstr;
							if (flag14)
							{
								TypeDef typeDef = method.Module.FindReflection((string)instr.Operand);
								bool flag15 = typeDef != null;
								if (flag15)
								{
									service.AddReference<TypeDef>(typeDef, new StringTypeReference(instr, typeDef));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x000219B8 File Offset: 0x0001FBB8
		private void DisableRename(INameService service, TypeDef typeDef, bool memberOnly = true)
		{
			service.SetCanRename(typeDef, false);
			foreach (MethodDef i in typeDef.Methods)
			{
				service.SetCanRename(i, false);
			}
			foreach (FieldDef field in typeDef.Fields)
			{
				service.SetCanRename(field, false);
			}
			foreach (PropertyDef prop in typeDef.Properties)
			{
				service.SetCanRename(prop, false);
			}
			foreach (EventDef evt in typeDef.Events)
			{
				service.SetCanRename(evt, false);
			}
			foreach (TypeDef nested in typeDef.NestedTypes)
			{
				this.DisableRename(service, nested, false);
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00021B34 File Offset: 0x0001FD34
		private void HandleEnum(ConfuserContext context, INameService service, MethodDef method, int index)
		{
			IMethod target = (IMethod)method.Body.Instructions[index].Operand;
			bool flag = target.FullName == "System.String System.Object::ToString()" || target.FullName == "System.String System.Enum::ToString(System.String)";
			if (flag)
			{
				int prevIndex = index - 1;
				while (prevIndex >= 0 && method.Body.Instructions[prevIndex].OpCode.Code == Code.Nop)
				{
					prevIndex--;
				}
				bool flag2 = prevIndex < 0;
				if (!flag2)
				{
					Instruction prevInstr = method.Body.Instructions[prevIndex];
					bool flag3 = prevInstr.Operand is MemberRef;
					TypeSig targetType;
					if (flag3)
					{
						MemberRef memberRef = (MemberRef)prevInstr.Operand;
						targetType = (memberRef.IsFieldRef ? memberRef.FieldSig.Type : memberRef.MethodSig.RetType);
					}
					else
					{
						bool flag4 = prevInstr.Operand is IField;
						if (flag4)
						{
							targetType = ((IField)prevInstr.Operand).FieldSig.Type;
						}
						else
						{
							bool flag5 = prevInstr.Operand is IMethod;
							if (flag5)
							{
								targetType = ((IMethod)prevInstr.Operand).MethodSig.RetType;
							}
							else
							{
								bool flag6 = prevInstr.Operand is ITypeDefOrRef;
								if (flag6)
								{
									targetType = ((ITypeDefOrRef)prevInstr.Operand).ToTypeSig();
								}
								else
								{
									bool flag7 = prevInstr.GetParameter(method.Parameters) != null;
									if (flag7)
									{
										targetType = prevInstr.GetParameter(method.Parameters).Type;
									}
									else
									{
										bool flag8 = prevInstr.GetLocal(method.Body.Variables) == null;
										if (flag8)
										{
											return;
										}
										targetType = prevInstr.GetLocal(method.Body.Variables).Type;
									}
								}
							}
						}
					}
					ITypeDefOrRef targetTypeRef = targetType.ToBasicTypeDefOrRef();
					bool flag9 = targetTypeRef == null;
					if (!flag9)
					{
						TypeDef targetTypeDef = targetTypeRef.ResolveTypeDefThrow();
						bool flag10 = targetTypeDef != null && targetTypeDef.IsEnum && context.Modules.Contains((ModuleDefMD)targetTypeDef.Module);
						if (flag10)
						{
							this.DisableRename(service, targetTypeDef, true);
						}
					}
				}
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00021D80 File Offset: 0x0001FF80
		private bool HandleTypeOf(ConfuserContext context, INameService service, MethodDef method, int index)
		{
			bool flag = index + 1 >= method.Body.Instructions.Count;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				IMethod gtfh = method.Body.Instructions[index + 1].Operand as IMethod;
				bool flag2 = gtfh == null || gtfh.FullName != "System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)";
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = index + 2 < method.Body.Instructions.Count;
					if (flag3)
					{
						Instruction instr = method.Body.Instructions[index + 2];
						IMethod operand = instr.Operand as IMethod;
						bool flag4 = instr.OpCode == OpCodes.Newobj && operand.FullName == "System.Void System.ComponentModel.ComponentResourceManager::.ctor(System.Type)";
						if (flag4)
						{
							return false;
						}
						string fullName;
						bool flag5 = (instr.OpCode == OpCodes.Call || instr.OpCode == OpCodes.Callvirt) && (fullName = operand.DeclaringType.FullName) != null;
						if (flag5)
						{
							bool flag6 = fullName == "System.Runtime.InteropServices.Marshal";
							if (flag6)
							{
								return false;
							}
							bool flag7 = fullName == "System.Type";
							if (flag7)
							{
								return operand.Name.StartsWith("Get") || operand.Name == "InvokeMember" || operand.Name == "get_AssemblyQualifiedName" || operand.Name == "get_FullName" || operand.Name == "get_Namespace";
							}
							bool flag8 = fullName == "System.Reflection.MemberInfo";
							if (flag8)
							{
								return operand.Name == "get_Name";
							}
							bool flag9 = fullName == "System.Object";
							if (flag9)
							{
								return operand.Name == "ToString";
							}
						}
					}
					bool flag10 = index + 3 < method.Body.Instructions.Count;
					if (flag10)
					{
						Instruction instr2 = method.Body.Instructions[index + 3];
						IMethod operand2 = instr2.Operand as IMethod;
						string fullName2;
						bool flag11 = (instr2.OpCode == OpCodes.Call || instr2.OpCode == OpCodes.Callvirt) && (fullName2 = operand2.DeclaringType.FullName) != null && fullName2 == "System.Runtime.InteropServices.Marshal";
						if (flag11)
						{
							return false;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00002184 File Offset: 0x00000384
		public LdtokenEnumAnalyzer()
		{
		}
	}
}
