using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources2
{
	// Token: 0x0200004D RID: 77
	internal class InjectPhase : ProtectionPhase
	{
		// Token: 0x06000162 RID: 354 RVA: 0x00004A51 File Offset: 0x00002C51
		public InjectPhase(ResourceProtection2 parent) : base(parent)
		{
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000BCD0 File Offset: 0x00009ED0
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool flag = parameters.Targets.Any<IDnlibDef>();
			if (flag)
			{
				bool flag2 = !UTF8String.IsNullOrEmpty(context.CurrentModule.Assembly.Culture);
				if (flag2)
				{
					context.Logger.DebugFormat("Skipping resource encryption for satellite assembly '{0}'.", new object[]
					{
						context.CurrentModule.Assembly.FullName
					});
				}
				else
				{
					ICompressionService compression = context.Registry.GetService<ICompressionService>();
					INameService name = context.Registry.GetService<INameService>();
					IMarkerService marker = context.Registry.GetService<IMarkerService>();
					IRuntimeService rt = context.Registry.GetService<IRuntimeService>();
					REContext moduleCtx = new REContext
					{
						Random = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.Id),
						Context = context,
						Module = context.CurrentModule,
						Marker = marker,
						DynCipher = context.Registry.GetService<IDynCipherService>(),
						Name = name
					};
					moduleCtx.Mode = parameters.GetParameter<Mode>(context, context.CurrentModule, "mode", Mode.Dynamic);
					Mode mode = moduleCtx.Mode;
					if (mode != Mode.Normal)
					{
						if (mode != Mode.Dynamic)
						{
							throw new UnreachableException();
						}
						moduleCtx.ModeHandler = new DynamicMode();
					}
					else
					{
						moduleCtx.ModeHandler = new NormalMode();
					}
					MethodDef decomp = compression.GetRuntimeDecompressor(context.CurrentModule, delegate(IDnlibDef member)
					{
						name.MarkHelper(member, marker, (Protection)this.Parent);
						bool flag3 = member is MethodDef;
						if (flag3)
						{
							ProtectionParameters.GetParameters(context, member).Remove(this.Parent);
						}
					});
					this.InjectHelpers(context, compression, rt, moduleCtx);
					this.MutateInitializer(moduleCtx, decomp);
					MethodDef cctor = context.CurrentModule.GlobalType.FindStaticConstructor();
					cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, moduleCtx.InitMethod));
					new MDPhase(moduleCtx).Hook();
				}
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000BF68 File Offset: 0x0000A168
		private void InjectHelpers(ConfuserContext context, ICompressionService compression, IRuntimeService rt, REContext moduleCtx)
		{
			string fullName = (context.Packer != null) ? "Confuser.Runtime.Resource_Packer" : "Confuser.Runtime.Resource";
			IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(rt.GetRuntimeType(fullName), context.CurrentModule.GlobalType, context.CurrentModule);
			foreach (IDnlibDef dnlibDef in enumerable)
			{
				bool flag = dnlibDef.Name == "Initialize";
				if (flag)
				{
					moduleCtx.InitMethod = (MethodDef)dnlibDef;
				}
				moduleCtx.Name.MarkHelper(dnlibDef, moduleCtx.Marker, (Protection)base.Parent);
			}
			TypeDefUser typeDefUser = new TypeDefUser("", "SafeGuard", context.CurrentModule.CorLibTypes.GetTypeRef("System", "ValueType"));
			typeDefUser.Layout = TypeAttributes.ExplicitLayout;
			typeDefUser.Visibility = TypeAttributes.NestedPrivate;
			typeDefUser.IsSealed = true;
			typeDefUser.ClassLayout = new ClassLayoutUser(1, 0U);
			moduleCtx.DataType = typeDefUser;
			context.CurrentModule.GlobalType.NestedTypes.Add(typeDefUser);
			moduleCtx.Name.MarkHelper(typeDefUser, moduleCtx.Marker, (Protection)base.Parent);
			moduleCtx.DataField = new FieldDefUser("SafeGuard", new FieldSig(typeDefUser.ToTypeSig()))
			{
				IsStatic = true,
				HasFieldRVA = true,
				InitialValue = new byte[0],
				Access = FieldAttributes.PrivateScope
			};
			context.CurrentModule.GlobalType.Fields.Add(moduleCtx.DataField);
			moduleCtx.Name.MarkHelper(moduleCtx.DataField, moduleCtx.Marker, (Protection)base.Parent);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000C158 File Offset: 0x0000A358
		private void MutateInitializer(REContext moduleCtx, MethodDef decomp)
		{
			moduleCtx.InitMethod.Body.SimplifyMacros(moduleCtx.InitMethod.Parameters);
			List<Instruction> instrs = moduleCtx.InitMethod.Body.Instructions.ToList<Instruction>();
			for (int i = 0; i < instrs.Count; i++)
			{
				Instruction instr = instrs[i];
				IMethod method = instr.Operand as IMethod;
				bool flag = instr.OpCode == OpCodes.Call;
				if (flag)
				{
					bool flag2 = method.DeclaringType.Name == "Mutation" && method.Name == "Crypt";
					if (flag2)
					{
						Instruction ldBlock = instrs[i - 2];
						Instruction ldKey = instrs[i - 1];
						instrs.RemoveAt(i);
						instrs.RemoveAt(i - 1);
						instrs.RemoveAt(i - 2);
						instrs.InsertRange(i - 2, moduleCtx.ModeHandler.EmitDecrypt(moduleCtx.InitMethod, moduleCtx, (Local)ldBlock.Operand, (Local)ldKey.Operand));
					}
					else
					{
						bool flag3 = method.DeclaringType.Name == "Lzma" && method.Name == "Decompress";
						if (flag3)
						{
							instr.Operand = decomp;
						}
					}
				}
			}
			moduleCtx.InitMethod.Body.Instructions.Clear();
			foreach (Instruction instr2 in instrs)
			{
				moduleCtx.InitMethod.Body.Instructions.Add(instr2);
			}
			MutationHelper.ReplacePlaceholder(moduleCtx.InitMethod, delegate(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Dup));
				repl.Add(Instruction.Create(OpCodes.Ldtoken, moduleCtx.DataField));
				repl.Add(Instruction.Create(OpCodes.Call, moduleCtx.Module.Import(typeof(RuntimeHelpers).GetMethod("InitializeArray"))));
				return repl.ToArray();
			});
			moduleCtx.Context.Registry.GetService<IConstantService>().ExcludeMethod(moduleCtx.Context, moduleCtx.InitMethod);
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000166 RID: 358 RVA: 0x0000B304 File Offset: 0x00009504
		public override string Name
		{
			get
			{
				return "Resource encryption helpers injection";
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000167 RID: 359 RVA: 0x00009294 File Offset: 0x00007494
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x0200004E RID: 78
		[CompilerGenerated]
		private sealed class <>c__DisplayClass1_0
		{
			// Token: 0x06000168 RID: 360 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass1_0()
			{
			}

			// Token: 0x06000169 RID: 361 RVA: 0x0000C3AC File Offset: 0x0000A5AC
			internal void <Execute>b__0(IDnlibDef member)
			{
				this.name.MarkHelper(member, this.marker, (Protection)this.CS$<>8__locals1.<>4__this.Parent);
				bool flag = member is MethodDef;
				if (flag)
				{
					ProtectionParameters.GetParameters(this.CS$<>8__locals1.context, member).Remove(this.CS$<>8__locals1.<>4__this.Parent);
				}
			}

			// Token: 0x04000099 RID: 153
			public INameService name;

			// Token: 0x0400009A RID: 154
			public IMarkerService marker;

			// Token: 0x0400009B RID: 155
			public InjectPhase.<>c__DisplayClass1_1 CS$<>8__locals1;
		}

		// Token: 0x0200004F RID: 79
		[CompilerGenerated]
		private sealed class <>c__DisplayClass1_1
		{
			// Token: 0x0600016A RID: 362 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass1_1()
			{
			}

			// Token: 0x0400009C RID: 156
			public ConfuserContext context;

			// Token: 0x0400009D RID: 157
			public InjectPhase <>4__this;
		}

		// Token: 0x02000050 RID: 80
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_0
		{
			// Token: 0x0600016B RID: 363 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass3_0()
			{
			}

			// Token: 0x0600016C RID: 364 RVA: 0x0000C418 File Offset: 0x0000A618
			internal Instruction[] <MutateInitializer>b__0(Instruction[] arg)
			{
				List<Instruction> repl = new List<Instruction>();
				repl.AddRange(arg);
				repl.Add(Instruction.Create(OpCodes.Dup));
				repl.Add(Instruction.Create(OpCodes.Ldtoken, this.moduleCtx.DataField));
				repl.Add(Instruction.Create(OpCodes.Call, this.moduleCtx.Module.Import(typeof(RuntimeHelpers).GetMethod("InitializeArray"))));
				return repl.ToArray();
			}

			// Token: 0x0400009E RID: 158
			public REContext moduleCtx;
		}
	}
}
