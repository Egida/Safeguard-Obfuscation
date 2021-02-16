using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000086 RID: 134
	public class WinFormsAnalyzer : IRenamer
	{
		// Token: 0x060002FA RID: 762 RVA: 0x00022F68 File Offset: 0x00021168
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			bool flag = def is ModuleDef;
			if (flag)
			{
				foreach (TypeDef type in ((ModuleDef)def).GetTypes())
				{
					foreach (PropertyDef prop in type.Properties)
					{
						this.properties.AddListEntry(prop.Name, prop);
					}
				}
			}
			else
			{
				MethodDef method = def as MethodDef;
				bool flag2 = method == null || !method.HasBody;
				if (!flag2)
				{
					this.AnalyzeMethod(context, service, method);
				}
			}
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00023050 File Offset: 0x00021250
		private void AnalyzeMethod(ConfuserContext context, INameService service, MethodDef method)
		{
			List<Tuple<bool, Instruction>> binding = new List<Tuple<bool, Instruction>>();
			foreach (Instruction instr in method.Body.Instructions)
			{
				bool flag = instr.OpCode.Code == Code.Call || instr.OpCode.Code == Code.Callvirt;
				if (flag)
				{
					IMethod target = (IMethod)instr.Operand;
					bool flag2 = (target.DeclaringType.FullName == "System.Windows.Forms.ControlBindingsCollection" || target.DeclaringType.FullName == "System.Windows.Forms.BindingsCollection") && target.Name == "Add" && target.MethodSig.Params.Count != 1;
					if (flag2)
					{
						binding.Add(Tuple.Create<bool, Instruction>(true, instr));
					}
					else
					{
						bool flag3 = target.DeclaringType.FullName == "System.Windows.Forms.Binding" && target.Name.String == ".ctor";
						if (flag3)
						{
							binding.Add(Tuple.Create<bool, Instruction>(false, instr));
						}
					}
				}
			}
			bool flag4 = binding.Count == 0;
			if (!flag4)
			{
				ITraceService traceSrv = context.Registry.GetService<ITraceService>();
				MethodTrace trace = traceSrv.Trace(method);
				bool erred = false;
				foreach (Tuple<bool, Instruction> instrInfo in binding)
				{
					int[] args = trace.TraceArguments(instrInfo.Item2);
					bool flag5 = args == null;
					if (flag5)
					{
						bool flag6 = !erred;
						if (flag6)
						{
							context.Logger.WarnFormat("Failed to extract binding property name in '{0}'.", new object[]
							{
								method.FullName
							});
						}
						erred = true;
					}
					else
					{
						Instruction propertyName = method.Body.Instructions[args[instrInfo.Item1 ? 1 : 0]];
						bool flag7 = propertyName.OpCode.Code != Code.Ldstr;
						if (flag7)
						{
							bool flag8 = !erred;
							if (flag8)
							{
								context.Logger.WarnFormat("Failed to extract binding property name in '{0}'.", new object[]
								{
									method.FullName
								});
							}
							erred = true;
						}
						else
						{
							List<PropertyDef> props;
							bool flag9 = !this.properties.TryGetValue((string)propertyName.Operand, out props);
							if (flag9)
							{
								bool flag10 = !erred;
								if (flag10)
								{
									context.Logger.WarnFormat("Failed to extract target property in '{0}'.", new object[]
									{
										method.FullName
									});
								}
								erred = true;
							}
							else
							{
								foreach (PropertyDef property in props)
								{
									service.SetCanRename(property, false);
								}
							}
						}
						Instruction dataMember = method.Body.Instructions[args[2 + (instrInfo.Item1 ? 1 : 0)]];
						bool flag11 = dataMember.OpCode.Code != Code.Ldstr;
						if (flag11)
						{
							bool flag12 = !erred;
							if (flag12)
							{
								context.Logger.WarnFormat("Failed to extract binding property name in '{0}'.", new object[]
								{
									method.FullName
								});
							}
							erred = true;
						}
						else
						{
							List<PropertyDef> props2;
							bool flag13 = !this.properties.TryGetValue((string)dataMember.Operand, out props2);
							if (flag13)
							{
								bool flag14 = !erred;
								if (flag14)
								{
									context.Logger.WarnFormat("Failed to extract target property in '{0}'.", new object[]
									{
										method.FullName
									});
								}
								erred = true;
							}
							else
							{
								foreach (PropertyDef property2 in props2)
								{
									service.SetCanRename(property2, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00002F0E File Offset: 0x0000110E
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00003637 File Offset: 0x00001837
		public WinFormsAnalyzer()
		{
		}

		// Token: 0x04000540 RID: 1344
		private Dictionary<string, List<PropertyDef>> properties = new Dictionary<string, List<PropertyDef>>();
	}
}
