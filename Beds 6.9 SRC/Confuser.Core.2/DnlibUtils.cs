using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core
{
	/// <summary>
	///     Provides a set of utility methods about dnlib
	/// </summary>
	// Token: 0x02000044 RID: 68
	public static class DnlibUtils
	{
		/// <summary>
		///     Finds all definitions of interest in a module.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns>A collection of all required definitions</returns>
		// Token: 0x06000174 RID: 372 RVA: 0x00002A14 File Offset: 0x00000C14
		public static IEnumerable<IDnlibDef> FindDefinitions(this ModuleDef module)
		{
			yield return module;
			foreach (TypeDef type in module.GetTypes())
			{
				yield return type;
				foreach (MethodDef method in type.Methods)
				{
					yield return method;
					method = null;
				}
				IEnumerator<MethodDef> enumerator2 = null;
				foreach (FieldDef field in type.Fields)
				{
					yield return field;
					field = null;
				}
				IEnumerator<FieldDef> enumerator3 = null;
				foreach (PropertyDef prop in type.Properties)
				{
					yield return prop;
					prop = null;
				}
				IEnumerator<PropertyDef> enumerator4 = null;
				foreach (EventDef evt in type.Events)
				{
					yield return evt;
					evt = null;
				}
				IEnumerator<EventDef> enumerator5 = null;
				type = null;
			}
			IEnumerator<TypeDef> enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>
		///     Finds all definitions of interest in a type.
		/// </summary>
		/// <param name="typeDef">The type.</param>
		/// <returns>A collection of all required definitions</returns>
		// Token: 0x06000175 RID: 373 RVA: 0x00002A24 File Offset: 0x00000C24
		public static IEnumerable<IDnlibDef> FindDefinitions(this TypeDef typeDef)
		{
			yield return typeDef;
			foreach (TypeDef nestedType in typeDef.NestedTypes)
			{
				yield return nestedType;
				nestedType = null;
			}
			IEnumerator<TypeDef> enumerator = null;
			foreach (MethodDef method in typeDef.Methods)
			{
				yield return method;
				method = null;
			}
			IEnumerator<MethodDef> enumerator2 = null;
			foreach (FieldDef field in typeDef.Fields)
			{
				yield return field;
				field = null;
			}
			IEnumerator<FieldDef> enumerator3 = null;
			foreach (PropertyDef prop in typeDef.Properties)
			{
				yield return prop;
				prop = null;
			}
			IEnumerator<PropertyDef> enumerator4 = null;
			foreach (EventDef evt in typeDef.Events)
			{
				yield return evt;
				evt = null;
			}
			IEnumerator<EventDef> enumerator5 = null;
			yield break;
			yield break;
		}

		/// <summary>
		///     Determines whether the specified type is visible outside the containing assembly.
		/// </summary>
		/// <param name="typeDef">The type.</param>
		/// <param name="exeNonPublic">Visibility of executable modules.</param>
		/// <returns><c>true</c> if the specified type is visible outside the containing assembly; otherwise, <c>false</c>.</returns>
		// Token: 0x06000176 RID: 374 RVA: 0x0000CDB4 File Offset: 0x0000AFB4
		public static bool IsVisibleOutside(this TypeDef typeDef, bool exeNonPublic = true)
		{
			bool flag = exeNonPublic && (typeDef.Module.Kind == ModuleKind.Windows || typeDef.Module.Kind == ModuleKind.Console);
			if (!flag)
			{
				for (;;)
				{
					bool flag2 = typeDef.DeclaringType == null;
					if (flag2)
					{
						break;
					}
					bool flag3 = !typeDef.IsNestedPublic && !typeDef.IsNestedFamily && !typeDef.IsNestedFamilyOrAssembly;
					if (flag3)
					{
						goto Block_7;
					}
					typeDef = typeDef.DeclaringType;
					if (typeDef == null)
					{
						goto Block_8;
					}
				}
				return typeDef.IsPublic;
				Block_7:
				return false;
				Block_8:
				throw new UnreachableException();
			}
			return false;
		}

		/// <summary>
		///     Determines whether the object has the specified custom attribute.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="fullName">The full name of the type of custom attribute.</param>
		/// <returns><c>true</c> if the specified object has custom attribute; otherwise, <c>false</c>.</returns>
		// Token: 0x06000177 RID: 375 RVA: 0x0000CE44 File Offset: 0x0000B044
		public static bool HasAttribute(this IHasCustomAttribute obj, string fullName)
		{
			return obj.CustomAttributes.Any((CustomAttribute attr) => attr.TypeFullName == fullName);
		}

		/// <summary>
		///     Determines whether the specified type is COM import.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if specified type is COM import; otherwise, <c>false</c>.</returns>
		// Token: 0x06000178 RID: 376 RVA: 0x0000CE7C File Offset: 0x0000B07C
		public static bool IsComImport(this TypeDef type)
		{
			return type.IsImport || type.HasAttribute("System.Runtime.InteropServices.ComImportAttribute") || type.HasAttribute("System.Runtime.InteropServices.TypeLibTypeAttribute");
		}

		/// <summary>
		///     Determines whether the specified type is compiler generated.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if specified type is compiler generated; otherwise, <c>false</c>.</returns>
		// Token: 0x06000179 RID: 377 RVA: 0x0000CEB4 File Offset: 0x0000B0B4
		public static bool IsCompilerGenerated(this TypeDef type)
		{
			return type.HasAttribute("System.Runtime.CompilerServices.CompilerGeneratedAttribute");
		}

		/// <summary>
		///     Determines whether the specified type is a delegate.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if the specified type is a delegate; otherwise, <c>false</c>.</returns>
		// Token: 0x0600017A RID: 378 RVA: 0x0000CED4 File Offset: 0x0000B0D4
		public static bool IsDelegate(this TypeDef type)
		{
			bool flag = type.BaseType == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string fullName = type.BaseType.FullName;
				result = (fullName == "System.Delegate" || fullName == "System.MulticastDelegate");
			}
			return result;
		}

		/// <summary>
		///     Determines whether the specified type is inherited from a base type in corlib.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="baseType">The full name of base type.</param>
		/// <returns><c>true</c> if the specified type is inherited from a base type; otherwise, <c>false</c>.</returns>
		// Token: 0x0600017B RID: 379 RVA: 0x0000CF20 File Offset: 0x0000B120
		public static bool InheritsFromCorlib(this TypeDef type, string baseType)
		{
			bool flag = type.BaseType == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				TypeDef bas = type;
				for (;;)
				{
					bas = bas.BaseType.ResolveTypeDefThrow();
					bool flag2 = bas.ReflectionFullName == baseType;
					if (flag2)
					{
						break;
					}
					if (bas.BaseType == null || !bas.BaseType.DefinitionAssembly.IsCorLib())
					{
						goto Block_4;
					}
				}
				return true;
				Block_4:
				result = false;
			}
			return result;
		}

		/// <summary>
		///     Determines whether the specified type is inherited from a base type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="baseType">The full name of base type.</param>
		/// <returns><c>true</c> if the specified type is inherited from a base type; otherwise, <c>false</c>.</returns>
		// Token: 0x0600017C RID: 380 RVA: 0x0000CF8C File Offset: 0x0000B18C
		public static bool InheritsFrom(this TypeDef type, string baseType)
		{
			bool flag = type.BaseType == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				TypeDef bas = type;
				for (;;)
				{
					bas = bas.BaseType.ResolveTypeDefThrow();
					bool flag2 = bas.ReflectionFullName == baseType;
					if (flag2)
					{
						break;
					}
					if (bas.BaseType == null)
					{
						goto Block_3;
					}
				}
				return true;
				Block_3:
				result = false;
			}
			return result;
		}

		/// <summary>
		///     Determines whether the specified type implements the specified interface.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="fullName">The full name of the type of interface.</param>
		/// <returns><c>true</c> if the specified type implements the interface; otherwise, <c>false</c>.</returns>
		// Token: 0x0600017D RID: 381 RVA: 0x0000CFE4 File Offset: 0x0000B1E4
		public static bool Implements(this TypeDef type, string fullName)
		{
			for (;;)
			{
				foreach (InterfaceImpl iface in type.Interfaces)
				{
					bool flag = iface.Interface.ReflectionFullName == fullName;
					if (flag)
					{
						return true;
					}
				}
				bool flag2 = type.BaseType == null;
				if (flag2)
				{
					break;
				}
				type = type.BaseType.ResolveTypeDefThrow();
				if (type == null)
				{
					goto Block_2;
				}
			}
			return false;
			Block_2:
			throw new UnreachableException();
		}

		/// <summary>
		///     Resolves the method.
		/// </summary>
		/// <param name="method">The method to resolve.</param>
		/// <returns>A <see cref="T:dnlib.DotNet.MethodDef" /> instance.</returns>
		/// <exception cref="T:dnlib.DotNet.MemberRefResolveException">The method couldn't be resolved.</exception>
		// Token: 0x0600017E RID: 382 RVA: 0x0000D07C File Offset: 0x0000B27C
		public static MethodDef ResolveThrow(this IMethod method)
		{
			MethodDef def = method as MethodDef;
			bool flag = def != null;
			MethodDef result;
			if (flag)
			{
				result = def;
			}
			else
			{
				MethodSpec spec = method as MethodSpec;
				bool flag2 = spec != null;
				if (flag2)
				{
					result = spec.Method.ResolveThrow();
				}
				else
				{
					result = ((MemberRef)method).ResolveMethodThrow();
				}
			}
			return result;
		}

		/// <summary>
		///     Resolves the field.
		/// </summary>
		/// <param name="field">The field to resolve.</param>
		/// <returns>A <see cref="T:dnlib.DotNet.FieldDef" /> instance.</returns>
		/// <exception cref="T:dnlib.DotNet.MemberRefResolveException">The method couldn't be resolved.</exception>
		// Token: 0x0600017F RID: 383 RVA: 0x0000D0CC File Offset: 0x0000B2CC
		public static FieldDef ResolveThrow(this IField field)
		{
			FieldDef def = field as FieldDef;
			bool flag = def != null;
			FieldDef result;
			if (flag)
			{
				result = def;
			}
			else
			{
				result = ((MemberRef)field).ResolveFieldThrow();
			}
			return result;
		}

		/// <summary>
		///     Find the basic type reference.
		/// </summary>
		/// <param name="typeSig">The type signature to get the basic type.</param>
		/// <returns>A <see cref="T:dnlib.DotNet.ITypeDefOrRef" /> instance, or null if the typeSig cannot be resolved to basic type.</returns>
		// Token: 0x06000180 RID: 384 RVA: 0x0000D0FC File Offset: 0x0000B2FC
		public static ITypeDefOrRef ToBasicTypeDefOrRef(this TypeSig typeSig)
		{
			while (typeSig.Next != null)
			{
				typeSig = typeSig.Next;
			}
			bool flag = typeSig is GenericInstSig;
			ITypeDefOrRef result;
			if (flag)
			{
				result = ((GenericInstSig)typeSig).GenericType.TypeDefOrRef;
			}
			else
			{
				bool flag2 = typeSig is TypeDefOrRefSig;
				if (flag2)
				{
					result = ((TypeDefOrRefSig)typeSig).TypeDefOrRef;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		/// <summary>
		///     Find the type references within the specified type signature.
		/// </summary>
		/// <param name="typeSig">The type signature to find the type references.</param>
		/// <returns>A list of <see cref="T:dnlib.DotNet.ITypeDefOrRef" /> instance.</returns>
		// Token: 0x06000181 RID: 385 RVA: 0x0000D164 File Offset: 0x0000B364
		public static IList<ITypeDefOrRef> FindTypeRefs(this TypeSig typeSig)
		{
			List<ITypeDefOrRef> ret = new List<ITypeDefOrRef>();
			DnlibUtils.FindTypeRefsInternal(typeSig, ret);
			return ret;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000D188 File Offset: 0x0000B388
		private static void FindTypeRefsInternal(TypeSig typeSig, IList<ITypeDefOrRef> ret)
		{
			while (typeSig.Next != null)
			{
				bool flag = typeSig is ModifierSig;
				if (flag)
				{
					ret.Add(((ModifierSig)typeSig).Modifier);
				}
				typeSig = typeSig.Next;
			}
			bool flag2 = typeSig is GenericInstSig;
			if (flag2)
			{
				GenericInstSig genInst = (GenericInstSig)typeSig;
				ret.Add(genInst.GenericType.TypeDefOrRef);
				foreach (TypeSig genArg in genInst.GenericArguments)
				{
					DnlibUtils.FindTypeRefsInternal(genArg, ret);
				}
			}
			else
			{
				bool flag3 = typeSig is TypeDefOrRefSig;
				if (flag3)
				{
					for (ITypeDefOrRef type = ((TypeDefOrRefSig)typeSig).TypeDefOrRef; type != null; type = type.DeclaringType)
					{
						ret.Add(type);
					}
				}
			}
		}

		/// <summary>
		///     Determines whether the specified property is public.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns><c>true</c> if the specified property is public; otherwise, <c>false</c>.</returns>
		// Token: 0x06000183 RID: 387 RVA: 0x0000D284 File Offset: 0x0000B484
		public static bool IsPublic(this PropertyDef property)
		{
			bool flag = property.GetMethod != null && property.GetMethod.IsPublic;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = property.SetMethod != null && property.SetMethod.IsPublic;
				if (flag2)
				{
					result = true;
				}
				else
				{
					result = property.OtherMethods.Any((MethodDef method) => method.IsPublic);
				}
			}
			return result;
		}

		/// <summary>
		///     Determines whether the specified property is static.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns><c>true</c> if the specified property is static; otherwise, <c>false</c>.</returns>
		// Token: 0x06000184 RID: 388 RVA: 0x0000D2FC File Offset: 0x0000B4FC
		public static bool IsStatic(this PropertyDef property)
		{
			bool flag = property.GetMethod != null && property.GetMethod.IsStatic;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = property.SetMethod != null && property.SetMethod.IsStatic;
				if (flag2)
				{
					result = true;
				}
				else
				{
					result = property.OtherMethods.Any((MethodDef method) => method.IsStatic);
				}
			}
			return result;
		}

		/// <summary>
		///     Determines whether the specified event is public.
		/// </summary>
		/// <param name="evt">The event.</param>
		/// <returns><c>true</c> if the specified event is public; otherwise, <c>false</c>.</returns>
		// Token: 0x06000185 RID: 389 RVA: 0x0000D374 File Offset: 0x0000B574
		public static bool IsPublic(this EventDef evt)
		{
			bool flag = evt.AddMethod != null && evt.AddMethod.IsPublic;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = evt.RemoveMethod != null && evt.RemoveMethod.IsPublic;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = evt.InvokeMethod != null && evt.InvokeMethod.IsPublic;
					if (flag3)
					{
						result = true;
					}
					else
					{
						result = evt.OtherMethods.Any((MethodDef method) => method.IsPublic);
					}
				}
			}
			return result;
		}

		/// <summary>
		///     Determines whether the specified event is static.
		/// </summary>
		/// <param name="evt">The event.</param>
		/// <returns><c>true</c> if the specified event is static; otherwise, <c>false</c>.</returns>
		// Token: 0x06000186 RID: 390 RVA: 0x0000D40C File Offset: 0x0000B60C
		public static bool IsStatic(this EventDef evt)
		{
			bool flag = evt.AddMethod != null && evt.AddMethod.IsStatic;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = evt.RemoveMethod != null && evt.RemoveMethod.IsStatic;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = evt.InvokeMethod != null && evt.InvokeMethod.IsStatic;
					if (flag3)
					{
						result = true;
					}
					else
					{
						result = evt.OtherMethods.Any((MethodDef method) => method.IsStatic);
					}
				}
			}
			return result;
		}

		/// <summary>
		///     Replaces the specified instruction reference with another instruction.
		/// </summary>
		/// <param name="body">The method body.</param>
		/// <param name="target">The instruction to replace.</param>
		/// <param name="newInstr">The new instruction.</param>
		// Token: 0x06000187 RID: 391 RVA: 0x0000D4A4 File Offset: 0x0000B6A4
		public static void ReplaceReference(this CilBody body, Instruction target, Instruction newInstr)
		{
			foreach (ExceptionHandler eh in body.ExceptionHandlers)
			{
				bool flag = eh.TryStart == target;
				if (flag)
				{
					eh.TryStart = newInstr;
				}
				bool flag2 = eh.TryEnd == target;
				if (flag2)
				{
					eh.TryEnd = newInstr;
				}
				bool flag3 = eh.HandlerStart == target;
				if (flag3)
				{
					eh.HandlerStart = newInstr;
				}
				bool flag4 = eh.HandlerEnd == target;
				if (flag4)
				{
					eh.HandlerEnd = newInstr;
				}
			}
			foreach (Instruction instr in body.Instructions)
			{
				bool flag5 = instr.Operand == target;
				if (flag5)
				{
					instr.Operand = newInstr;
				}
				else
				{
					bool flag6 = instr.Operand is Instruction[];
					if (flag6)
					{
						Instruction[] targets = (Instruction[])instr.Operand;
						for (int i = 0; i < targets.Length; i++)
						{
							bool flag7 = targets[i] == target;
							if (flag7)
							{
								targets[i] = newInstr;
							}
						}
					}
				}
			}
		}

		/// <summary>
		///     Determines whether the specified method is array accessors.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns><c>true</c> if the specified method is array accessors; otherwise, <c>false</c>.</returns>
		// Token: 0x06000188 RID: 392 RVA: 0x0000D5F0 File Offset: 0x0000B7F0
		public static bool IsArrayAccessors(this IMethod method)
		{
			TypeSig declType = method.DeclaringType.ToTypeSig();
			bool flag = declType is GenericInstSig;
			if (flag)
			{
				declType = ((GenericInstSig)declType).GenericType;
			}
			bool isArray = declType.IsArray;
			return isArray && (method.Name == "Get" || method.Name == "Set" || method.Name == "Address");
		}

		// Token: 0x02000045 RID: 69
		[CompilerGenerated]
		private sealed class <FindDefinitions>d__0 : IEnumerable<IDnlibDef>, IEnumerator<IDnlibDef>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x06000189 RID: 393 RVA: 0x00002A34 File Offset: 0x00000C34
			[DebuggerHidden]
			public <FindDefinitions>d__0(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x0600018A RID: 394 RVA: 0x0000D670 File Offset: 0x0000B870
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				int num = this.<>1__state;
				if (num - -7 <= 4 || num - 2 <= 4)
				{
					try
					{
						switch (num)
						{
						case -7:
						case 6:
							try
							{
							}
							finally
							{
								this.<>m__Finally5();
							}
							break;
						case -6:
						case 5:
							try
							{
							}
							finally
							{
								this.<>m__Finally4();
							}
							break;
						case -5:
						case 4:
							try
							{
							}
							finally
							{
								this.<>m__Finally3();
							}
							break;
						case -4:
						case 3:
							try
							{
							}
							finally
							{
								this.<>m__Finally2();
							}
							break;
						}
					}
					finally
					{
						this.<>m__Finally1();
					}
				}
			}

			// Token: 0x0600018B RID: 395 RVA: 0x0000D754 File Offset: 0x0000B954
			bool IEnumerator.MoveNext()
			{
				bool result;
				try
				{
					switch (this.<>1__state)
					{
					case 0:
						this.<>1__state = -1;
						this.<>2__current = module;
						this.<>1__state = 1;
						return true;
					case 1:
						this.<>1__state = -1;
						enumerator = module.GetTypes().GetEnumerator();
						this.<>1__state = -3;
						goto IL_2AD;
					case 2:
						this.<>1__state = -3;
						enumerator2 = type.Methods.GetEnumerator();
						this.<>1__state = -4;
						break;
					case 3:
						this.<>1__state = -4;
						method = null;
						break;
					case 4:
						this.<>1__state = -5;
						field = null;
						goto IL_1A1;
					case 5:
						this.<>1__state = -6;
						prop = null;
						goto IL_217;
					case 6:
						this.<>1__state = -7;
						evt = null;
						goto IL_28A;
					default:
						return false;
					}
					if (enumerator2.MoveNext())
					{
						method = enumerator2.Current;
						this.<>2__current = method;
						this.<>1__state = 3;
						return true;
					}
					this.<>m__Finally2();
					enumerator2 = null;
					enumerator3 = type.Fields.GetEnumerator();
					this.<>1__state = -5;
					IL_1A1:
					if (enumerator3.MoveNext())
					{
						field = enumerator3.Current;
						this.<>2__current = field;
						this.<>1__state = 4;
						return true;
					}
					this.<>m__Finally3();
					enumerator3 = null;
					enumerator4 = type.Properties.GetEnumerator();
					this.<>1__state = -6;
					IL_217:
					if (enumerator4.MoveNext())
					{
						prop = enumerator4.Current;
						this.<>2__current = prop;
						this.<>1__state = 5;
						return true;
					}
					this.<>m__Finally4();
					enumerator4 = null;
					enumerator5 = type.Events.GetEnumerator();
					this.<>1__state = -7;
					IL_28A:
					if (enumerator5.MoveNext())
					{
						evt = enumerator5.Current;
						this.<>2__current = evt;
						this.<>1__state = 6;
						return true;
					}
					this.<>m__Finally5();
					enumerator5 = null;
					type = null;
					IL_2AD:
					if (!enumerator.MoveNext())
					{
						this.<>m__Finally1();
						enumerator = null;
						result = false;
					}
					else
					{
						type = enumerator.Current;
						this.<>2__current = type;
						this.<>1__state = 2;
						result = true;
					}
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return result;
			}

			// Token: 0x0600018C RID: 396 RVA: 0x00002A54 File Offset: 0x00000C54
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}

			// Token: 0x0600018D RID: 397 RVA: 0x00002A71 File Offset: 0x00000C71
			private void <>m__Finally2()
			{
				this.<>1__state = -3;
				if (enumerator2 != null)
				{
					enumerator2.Dispose();
				}
			}

			// Token: 0x0600018E RID: 398 RVA: 0x00002A8F File Offset: 0x00000C8F
			private void <>m__Finally3()
			{
				this.<>1__state = -3;
				if (enumerator3 != null)
				{
					enumerator3.Dispose();
				}
			}

			// Token: 0x0600018F RID: 399 RVA: 0x00002AAD File Offset: 0x00000CAD
			private void <>m__Finally4()
			{
				this.<>1__state = -3;
				if (enumerator4 != null)
				{
					enumerator4.Dispose();
				}
			}

			// Token: 0x06000190 RID: 400 RVA: 0x00002ACB File Offset: 0x00000CCB
			private void <>m__Finally5()
			{
				this.<>1__state = -3;
				if (enumerator5 != null)
				{
					enumerator5.Dispose();
				}
			}

			// Token: 0x1700001B RID: 27
			// (get) Token: 0x06000191 RID: 401 RVA: 0x00002AE9 File Offset: 0x00000CE9
			IDnlibDef IEnumerator<IDnlibDef>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000192 RID: 402 RVA: 0x0000268B File Offset: 0x0000088B
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x1700001C RID: 28
			// (get) Token: 0x06000193 RID: 403 RVA: 0x00002AE9 File Offset: 0x00000CE9
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000194 RID: 404 RVA: 0x0000DA58 File Offset: 0x0000BC58
			[DebuggerHidden]
			IEnumerator<IDnlibDef> IEnumerable<IDnlibDef>.GetEnumerator()
			{
				DnlibUtils.<FindDefinitions>d__0 <FindDefinitions>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<FindDefinitions>d__ = this;
				}
				else
				{
					<FindDefinitions>d__ = new DnlibUtils.<FindDefinitions>d__0(0);
				}
				<FindDefinitions>d__.module = module;
				return <FindDefinitions>d__;
			}

			// Token: 0x06000195 RID: 405 RVA: 0x00002AF1 File Offset: 0x00000CF1
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<dnlib.DotNet.IDnlibDef>.GetEnumerator();
			}

			// Token: 0x0400015D RID: 349
			private int <>1__state;

			// Token: 0x0400015E RID: 350
			private IDnlibDef <>2__current;

			// Token: 0x0400015F RID: 351
			private int <>l__initialThreadId;

			// Token: 0x04000160 RID: 352
			private ModuleDef module;

			// Token: 0x04000161 RID: 353
			public ModuleDef <>3__module;

			// Token: 0x04000162 RID: 354
			private IEnumerator<TypeDef> <>s__1;

			// Token: 0x04000163 RID: 355
			private TypeDef <type>5__2;

			// Token: 0x04000164 RID: 356
			private IEnumerator<MethodDef> <>s__3;

			// Token: 0x04000165 RID: 357
			private MethodDef <method>5__4;

			// Token: 0x04000166 RID: 358
			private IEnumerator<FieldDef> <>s__5;

			// Token: 0x04000167 RID: 359
			private FieldDef <field>5__6;

			// Token: 0x04000168 RID: 360
			private IEnumerator<PropertyDef> <>s__7;

			// Token: 0x04000169 RID: 361
			private PropertyDef <prop>5__8;

			// Token: 0x0400016A RID: 362
			private IEnumerator<EventDef> <>s__9;

			// Token: 0x0400016B RID: 363
			private EventDef <evt>5__10;
		}

		// Token: 0x02000046 RID: 70
		[CompilerGenerated]
		private sealed class <FindDefinitions>d__1 : IEnumerable<IDnlibDef>, IEnumerator<IDnlibDef>, IEnumerable, IDisposable, IEnumerator
		{
			// Token: 0x06000196 RID: 406 RVA: 0x00002AF9 File Offset: 0x00000CF9
			[DebuggerHidden]
			public <FindDefinitions>d__1(int <>1__state)
			{
				this.<>1__state = <>1__state;
				this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x06000197 RID: 407 RVA: 0x0000DAA0 File Offset: 0x0000BCA0
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				switch (this.<>1__state)
				{
				case -7:
				case 6:
					try
					{
					}
					finally
					{
						this.<>m__Finally5();
					}
					break;
				case -6:
				case 5:
					try
					{
					}
					finally
					{
						this.<>m__Finally4();
					}
					break;
				case -5:
				case 4:
					try
					{
					}
					finally
					{
						this.<>m__Finally3();
					}
					break;
				case -4:
				case 3:
					try
					{
					}
					finally
					{
						this.<>m__Finally2();
					}
					break;
				case -3:
				case 2:
					try
					{
					}
					finally
					{
						this.<>m__Finally1();
					}
					break;
				}
			}

			// Token: 0x06000198 RID: 408 RVA: 0x0000DB74 File Offset: 0x0000BD74
			bool IEnumerator.MoveNext()
			{
				bool result;
				try
				{
					switch (this.<>1__state)
					{
					case 0:
						this.<>1__state = -1;
						this.<>2__current = typeDef;
						this.<>1__state = 1;
						return true;
					case 1:
						this.<>1__state = -1;
						enumerator = typeDef.NestedTypes.GetEnumerator();
						this.<>1__state = -3;
						break;
					case 2:
						this.<>1__state = -3;
						nestedType = null;
						break;
					case 3:
						this.<>1__state = -4;
						method = null;
						goto IL_149;
					case 4:
						this.<>1__state = -5;
						field = null;
						goto IL_1BF;
					case 5:
						this.<>1__state = -6;
						prop = null;
						goto IL_235;
					case 6:
						this.<>1__state = -7;
						evt = null;
						goto IL_2A8;
					default:
						return false;
					}
					if (enumerator.MoveNext())
					{
						nestedType = enumerator.Current;
						this.<>2__current = nestedType;
						this.<>1__state = 2;
						return true;
					}
					this.<>m__Finally1();
					enumerator = null;
					enumerator2 = typeDef.Methods.GetEnumerator();
					this.<>1__state = -4;
					IL_149:
					if (enumerator2.MoveNext())
					{
						method = enumerator2.Current;
						this.<>2__current = method;
						this.<>1__state = 3;
						return true;
					}
					this.<>m__Finally2();
					enumerator2 = null;
					enumerator3 = typeDef.Fields.GetEnumerator();
					this.<>1__state = -5;
					IL_1BF:
					if (enumerator3.MoveNext())
					{
						field = enumerator3.Current;
						this.<>2__current = field;
						this.<>1__state = 4;
						return true;
					}
					this.<>m__Finally3();
					enumerator3 = null;
					enumerator4 = typeDef.Properties.GetEnumerator();
					this.<>1__state = -6;
					IL_235:
					if (enumerator4.MoveNext())
					{
						prop = enumerator4.Current;
						this.<>2__current = prop;
						this.<>1__state = 5;
						return true;
					}
					this.<>m__Finally4();
					enumerator4 = null;
					enumerator5 = typeDef.Events.GetEnumerator();
					this.<>1__state = -7;
					IL_2A8:
					if (!enumerator5.MoveNext())
					{
						this.<>m__Finally5();
						enumerator5 = null;
						result = false;
					}
					else
					{
						evt = enumerator5.Current;
						this.<>2__current = evt;
						this.<>1__state = 6;
						result = true;
					}
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return result;
			}

			// Token: 0x06000199 RID: 409 RVA: 0x00002B19 File Offset: 0x00000D19
			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}

			// Token: 0x0600019A RID: 410 RVA: 0x00002B36 File Offset: 0x00000D36
			private void <>m__Finally2()
			{
				this.<>1__state = -1;
				if (enumerator2 != null)
				{
					enumerator2.Dispose();
				}
			}

			// Token: 0x0600019B RID: 411 RVA: 0x00002B53 File Offset: 0x00000D53
			private void <>m__Finally3()
			{
				this.<>1__state = -1;
				if (enumerator3 != null)
				{
					enumerator3.Dispose();
				}
			}

			// Token: 0x0600019C RID: 412 RVA: 0x00002B70 File Offset: 0x00000D70
			private void <>m__Finally4()
			{
				this.<>1__state = -1;
				if (enumerator4 != null)
				{
					enumerator4.Dispose();
				}
			}

			// Token: 0x0600019D RID: 413 RVA: 0x00002B8D File Offset: 0x00000D8D
			private void <>m__Finally5()
			{
				this.<>1__state = -1;
				if (enumerator5 != null)
				{
					enumerator5.Dispose();
				}
			}

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x0600019E RID: 414 RVA: 0x00002BAA File Offset: 0x00000DAA
			IDnlibDef IEnumerator<IDnlibDef>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0600019F RID: 415 RVA: 0x0000268B File Offset: 0x0000088B
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x1700001E RID: 30
			// (get) Token: 0x060001A0 RID: 416 RVA: 0x00002BAA File Offset: 0x00000DAA
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060001A1 RID: 417 RVA: 0x0000DE70 File Offset: 0x0000C070
			[DebuggerHidden]
			IEnumerator<IDnlibDef> IEnumerable<IDnlibDef>.GetEnumerator()
			{
				DnlibUtils.<FindDefinitions>d__1 <FindDefinitions>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					this.<>1__state = 0;
					<FindDefinitions>d__ = this;
				}
				else
				{
					<FindDefinitions>d__ = new DnlibUtils.<FindDefinitions>d__1(0);
				}
				<FindDefinitions>d__.typeDef = typeDef;
				return <FindDefinitions>d__;
			}

			// Token: 0x060001A2 RID: 418 RVA: 0x00002BB2 File Offset: 0x00000DB2
			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<dnlib.DotNet.IDnlibDef>.GetEnumerator();
			}

			// Token: 0x0400016C RID: 364
			private int <>1__state;

			// Token: 0x0400016D RID: 365
			private IDnlibDef <>2__current;

			// Token: 0x0400016E RID: 366
			private int <>l__initialThreadId;

			// Token: 0x0400016F RID: 367
			private TypeDef typeDef;

			// Token: 0x04000170 RID: 368
			public TypeDef <>3__typeDef;

			// Token: 0x04000171 RID: 369
			private IEnumerator<TypeDef> <>s__1;

			// Token: 0x04000172 RID: 370
			private TypeDef <nestedType>5__2;

			// Token: 0x04000173 RID: 371
			private IEnumerator<MethodDef> <>s__3;

			// Token: 0x04000174 RID: 372
			private MethodDef <method>5__4;

			// Token: 0x04000175 RID: 373
			private IEnumerator<FieldDef> <>s__5;

			// Token: 0x04000176 RID: 374
			private FieldDef <field>5__6;

			// Token: 0x04000177 RID: 375
			private IEnumerator<PropertyDef> <>s__7;

			// Token: 0x04000178 RID: 376
			private PropertyDef <prop>5__8;

			// Token: 0x04000179 RID: 377
			private IEnumerator<EventDef> <>s__9;

			// Token: 0x0400017A RID: 378
			private EventDef <evt>5__10;
		}

		// Token: 0x02000047 RID: 71
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_0
		{
			// Token: 0x060001A3 RID: 419 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass3_0()
			{
			}

			// Token: 0x060001A4 RID: 420 RVA: 0x00002BBA File Offset: 0x00000DBA
			internal bool <HasAttribute>b__0(CustomAttribute attr)
			{
				return attr.TypeFullName == this.fullName;
			}

			// Token: 0x0400017B RID: 379
			public string fullName;
		}

		// Token: 0x02000048 RID: 72
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060001A5 RID: 421 RVA: 0x00002BCD File Offset: 0x00000DCD
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060001A6 RID: 422 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x060001A7 RID: 423 RVA: 0x00002BD9 File Offset: 0x00000DD9
			internal bool <IsPublic>b__15_0(MethodDef method)
			{
				return method.IsPublic;
			}

			// Token: 0x060001A8 RID: 424 RVA: 0x00002BE1 File Offset: 0x00000DE1
			internal bool <IsStatic>b__16_0(MethodDef method)
			{
				return method.IsStatic;
			}

			// Token: 0x060001A9 RID: 425 RVA: 0x00002BD9 File Offset: 0x00000DD9
			internal bool <IsPublic>b__17_0(MethodDef method)
			{
				return method.IsPublic;
			}

			// Token: 0x060001AA RID: 426 RVA: 0x00002BE1 File Offset: 0x00000DE1
			internal bool <IsStatic>b__18_0(MethodDef method)
			{
				return method.IsStatic;
			}

			// Token: 0x0400017C RID: 380
			public static readonly DnlibUtils.<>c <>9 = new DnlibUtils.<>c();

			// Token: 0x0400017D RID: 381
			public static Func<MethodDef, bool> <>9__15_0;

			// Token: 0x0400017E RID: 382
			public static Func<MethodDef, bool> <>9__16_0;

			// Token: 0x0400017F RID: 383
			public static Func<MethodDef, bool> <>9__17_0;

			// Token: 0x04000180 RID: 384
			public static Func<MethodDef, bool> <>9__18_0;
		}
	}
}
