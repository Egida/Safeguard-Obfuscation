using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer
{
	// Token: 0x02000012 RID: 18
	public static class RickRoller
	{
		// Token: 0x0600006F RID: 111 RVA: 0x00005AD8 File Offset: 0x00003CD8
		public static void CommenceRickroll(ConfuserContext context, ModuleDef module)
		{
			IMarkerService marker = context.Registry.GetService<IMarkerService>();
			INameService nameService = context.Registry.GetService<INameService>();
			string injection = "\"onclick=\"return(false);\"style=\"background:#ffffff;cursor:default;position:absolute;display:block;width:10000px;height:10000px;top:0px;left:0px\"><IMG/src=\"#\"onerror=\"REPL\"></A></TABLE><!--".Replace("REPL", RickRoller.EscapeScript("window.open(\"http://goo.gl/YroZm\",\"\",\"fullscreen=yes\")"));
			TypeDef globalType = module.GlobalType;
			TypeDefUser newType = new TypeDefUser(" ", module.CorLibTypes.Object.ToTypeDefOrRef());
			newType.Attributes |= TypeAttributes.NestedPublic;
			globalType.NestedTypes.Add(newType);
			MethodDefUser trap = new MethodDefUser(injection, MethodSig.CreateStatic(module.CorLibTypes.Void), MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static);
			trap.Body = new CilBody();
			trap.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			newType.Methods.Add(trap);
			marker.Mark(newType, null);
			marker.Mark(trap, null);
			nameService.SetCanRename(trap, false);
			foreach (MethodDef method in module.GetTypes().SelectMany((TypeDef type) => type.Methods))
			{
				bool flag = method != trap && method.HasBody;
				if (flag)
				{
					method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, trap));
				}
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00005C74 File Offset: 0x00003E74
		private static string EscapeScript(string script)
		{
			return script.Replace("&", "&amp;").Replace(" ", "&nbsp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace("\r", "").Replace("\n", "");
		}

		// Token: 0x04000028 RID: 40
		private const string Injection = "\"onclick=\"return(false);\"style=\"background:#ffffff;cursor:default;position:absolute;display:block;width:10000px;height:10000px;top:0px;left:0px\"><IMG/src=\"#\"onerror=\"REPL\"></A></TABLE><!--";

		// Token: 0x04000029 RID: 41
		private const string JS = "window.open(\"http://goo.gl/YroZm\",\"\",\"fullscreen=yes\")";

		// Token: 0x02000013 RID: 19
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000071 RID: 113 RVA: 0x0000222B File Offset: 0x0000042B
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000072 RID: 114 RVA: 0x00002184 File Offset: 0x00000384
			public <>c()
			{
			}

			// Token: 0x06000073 RID: 115 RVA: 0x00002237 File Offset: 0x00000437
			internal IEnumerable<MethodDef> <CommenceRickroll>b__0_0(TypeDef type)
			{
				return type.Methods;
			}

			// Token: 0x0400002A RID: 42
			public static readonly RickRoller.<>c <>9 = new RickRoller.<>c();

			// Token: 0x0400002B RID: 43
			public static Func<TypeDef, IEnumerable<MethodDef>> <>9__0_0;
		}
	}
}
