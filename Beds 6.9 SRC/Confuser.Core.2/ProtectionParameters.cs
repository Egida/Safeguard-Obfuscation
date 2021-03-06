using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Parameters of <see cref="T:Confuser.Core.ConfuserComponent" />.
	/// </summary>
	// Token: 0x02000069 RID: 105
	public class ProtectionParameters
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionParameters" /> class.
		/// </summary>
		/// <param name="component">The component that this parameters applied to.</param>
		/// <param name="targets">The protection targets.</param>
		// Token: 0x06000281 RID: 641 RVA: 0x00003198 File Offset: 0x00001398
		internal ProtectionParameters(ConfuserComponent component, IList<IDnlibDef> targets)
		{
			this.comp = component;
			this.Targets = targets;
		}

		/// <summary>
		///     Gets the targets of protection.
		///     Possible targets are module, types, methods, fields, events, properties.
		/// </summary>
		/// <value>A list of protection targets.</value>
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000282 RID: 642 RVA: 0x000031B1 File Offset: 0x000013B1
		// (set) Token: 0x06000283 RID: 643 RVA: 0x000031B9 File Offset: 0x000013B9
		public IList<IDnlibDef> Targets
		{
			[CompilerGenerated]
			get
			{
				return this.<Targets>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Targets>k__BackingField = value;
			}
		}

		/// <summary>
		///     Obtains the value of a parameter of the specified target.
		/// </summary>
		/// <typeparam name="T">The type of the parameter value.</typeparam>
		/// <param name="context">The working context.</param>
		/// <param name="target">The protection target.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="defValue">Default value if the parameter does not exist.</param>
		/// <returns>The value of the parameter.</returns>
		// Token: 0x06000284 RID: 644 RVA: 0x00011668 File Offset: 0x0000F868
		public T GetParameter<T>(ConfuserContext context, IDnlibDef target, string name, T defValue = default(T))
		{
			bool flag = this.comp == null;
			T result;
			if (flag)
			{
				result = defValue;
			}
			else
			{
				bool flag2 = this.comp is Packer && target == null;
				if (flag2)
				{
					target = context.Modules[0];
				}
				ProtectionSettings objParams = context.Annotations.Get<ProtectionSettings>(target, ProtectionParameters.ParametersKey, null);
				bool flag3 = objParams == null;
				if (flag3)
				{
					result = defValue;
				}
				else
				{
					Dictionary<string, string> parameters;
					bool flag4 = !objParams.TryGetValue(this.comp, out parameters);
					if (flag4)
					{
						result = defValue;
					}
					else
					{
						string ret;
						bool flag5 = parameters.TryGetValue(name, out ret);
						if (flag5)
						{
							Type paramType = typeof(T);
							Type nullable = Nullable.GetUnderlyingType(paramType);
							bool flag6 = nullable != null;
							if (flag6)
							{
								paramType = nullable;
							}
							bool isEnum = paramType.IsEnum;
							if (isEnum)
							{
								result = (T)((object)Enum.Parse(paramType, ret, true));
							}
							else
							{
								result = (T)((object)Convert.ChangeType(ret, paramType));
							}
						}
						else
						{
							result = defValue;
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		///     Sets the protection parameters of the specified target.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The protection target.</param>
		/// <param name="parameters">The parameters.</param>
		// Token: 0x06000285 RID: 645 RVA: 0x000031C2 File Offset: 0x000013C2
		public static void SetParameters(ConfuserContext context, IDnlibDef target, ProtectionSettings parameters)
		{
			context.Annotations.Set<ProtectionSettings>(target, ProtectionParameters.ParametersKey, parameters);
		}

		/// <summary>
		///     Gets the protection parameters of the specified target.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The protection target.</param>
		/// <returns>The parameters.</returns>
		// Token: 0x06000286 RID: 646 RVA: 0x00011768 File Offset: 0x0000F968
		public static ProtectionSettings GetParameters(ConfuserContext context, IDnlibDef target)
		{
			return context.Annotations.Get<ProtectionSettings>(target, ProtectionParameters.ParametersKey, null);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x000031D8 File Offset: 0x000013D8
		// Note: this type is marked as 'beforefieldinit'.
		static ProtectionParameters()
		{
		}

		// Token: 0x040001EA RID: 490
		private static readonly object ParametersKey = new object();

		/// <summary>
		///     A empty instance of <see cref="T:Confuser.Core.ProtectionParameters" />.
		/// </summary>
		// Token: 0x040001EB RID: 491
		public static readonly ProtectionParameters Empty = new ProtectionParameters(null, new IDnlibDef[0]);

		// Token: 0x040001EC RID: 492
		private readonly ConfuserComponent comp;

		// Token: 0x040001ED RID: 493
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private IList<IDnlibDef> <Targets>k__BackingField;
	}
}
