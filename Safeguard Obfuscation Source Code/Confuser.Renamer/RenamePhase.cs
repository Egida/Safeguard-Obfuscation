using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer
{
	// Token: 0x02000010 RID: 16
	internal class RenamePhase : ProtectionPhase
	{
		// Token: 0x06000064 RID: 100 RVA: 0x000020E0 File Offset: 0x000002E0
		public RenamePhase(NameProtection parent) : base(parent)
		{
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000065 RID: 101 RVA: 0x000036A4 File Offset: 0x000018A4
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.AllDefinitions;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00005170 File Offset: 0x00003370
		public override string Name
		{
			get
			{
				return "Renaming";
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005188 File Offset: 0x00003388
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			NameService nameService = (NameService)context.Registry.GetService<INameService>();
			context.Logger.Debug("Renaming...");
			foreach (IRenamer renamer in nameService.Renamers)
			{
				foreach (IDnlibDef def in parameters.Targets)
				{
					renamer.PreRename(context, nameService, parameters, def);
				}
				context.CheckCancellation();
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (IDnlibDef dnlibDef in parameters.Targets.WithProgress(context.Logger))
			{
				bool flag = dnlibDef is ModuleDef && parameters.GetParameter<bool>(context, dnlibDef, "rickroll", true);
				if (flag)
				{
					RickRoller.CommenceRickroll(context, (ModuleDef)dnlibDef);
				}
				bool flag2 = nameService.CanRename(dnlibDef);
				RenameMode renameMode = nameService.GetRenameMode(dnlibDef);
				bool flag3 = dnlibDef is MethodDef;
				if (flag3)
				{
					MethodDef methodDef = (MethodDef)dnlibDef;
					bool flag4 = flag2 && parameters.GetParameter<bool>(context, dnlibDef, "renameArgs", true);
					if (flag4)
					{
						foreach (ParamDef paramDef in ((MethodDef)dnlibDef).ParamDefs)
						{
							paramDef.Name = null;
						}
					}
					bool flag5 = parameters.GetParameter<bool>(context, dnlibDef, "renPdb", true) && methodDef.HasBody;
					if (flag5)
					{
						foreach (Instruction instruction in methodDef.Body.Instructions)
						{
							bool flag6 = instruction.SequencePoint != null && !hashSet.Contains(instruction.SequencePoint.Document.Url);
							if (flag6)
							{
								instruction.SequencePoint.Document.Url = nameService.ObfuscateName(instruction.SequencePoint.Document.Url, renameMode);
								hashSet.Add(instruction.SequencePoint.Document.Url);
							}
						}
						foreach (Local local in methodDef.Body.Variables)
						{
							bool flag7 = !string.IsNullOrEmpty(local.Name);
							if (flag7)
							{
								local.Name = nameService.ObfuscateName(local.Name, renameMode);
							}
						}
						methodDef.Body.Scope = null;
					}
				}
				bool flag8 = flag2;
				if (flag8)
				{
					IList<INameReference> references = nameService.GetReferences(dnlibDef);
					bool flag9 = false;
					foreach (INameReference nameReference in references)
					{
						flag9 |= nameReference.ShouldCancelRename();
						bool flag10 = flag9;
						if (flag10)
						{
							break;
						}
					}
					bool flag11 = !flag9;
					if (flag11)
					{
						bool flag12 = dnlibDef is TypeDef;
						if (flag12)
						{
							TypeDef typeDef = (TypeDef)dnlibDef;
							bool parameter = parameters.GetParameter<bool>(context, dnlibDef, "flatten", true);
							if (parameter)
							{
								typeDef.Name = nameService.ObfuscateName(typeDef.FullName, renameMode);
								typeDef.Namespace = "ns27";
							}
							else
							{
								typeDef.Namespace = nameService.ObfuscateName(typeDef.Namespace, renameMode);
								typeDef.Name = nameService.ObfuscateName(typeDef.Name, renameMode);
							}
							using (IEnumerator<GenericParam> enumerator8 = typeDef.GenericParameters.GetEnumerator())
							{
								while (enumerator8.MoveNext())
								{
									GenericParam genericParam = enumerator8.Current;
									genericParam.Name = ((char)(genericParam.Number + 1)).ToString();
								}
								goto IL_43F;
							}
							goto IL_43D;
						}
						goto IL_43D;
						IL_43F:
						foreach (INameReference nameReference2 in references.ToList<INameReference>())
						{
							bool flag13 = !nameReference2.UpdateNameReference(context, nameService);
							if (flag13)
							{
								context.Logger.ErrorFormat("Failed to update name reference on '{0}'.", new object[]
								{
									dnlibDef
								});
								throw new ConfuserException(null);
							}
						}
						context.CheckCancellation();
						continue;
						IL_43D:
						bool flag14 = dnlibDef is MethodDef;
						if (flag14)
						{
							foreach (GenericParam genericParam2 in ((MethodDef)dnlibDef).GenericParameters)
							{
								genericParam2.Name = ((char)(genericParam2.Number + 1)).ToString();
							}
							dnlibDef.Name = nameService.ObfuscateName(dnlibDef.Name, renameMode);
							goto IL_43F;
						}
						dnlibDef.Name = nameService.ObfuscateName(dnlibDef.Name, renameMode);
						goto IL_43F;
					}
				}
			}
		}
	}
}
