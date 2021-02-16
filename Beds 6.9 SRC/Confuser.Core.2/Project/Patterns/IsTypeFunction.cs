using System;
using System.Text;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	/// <summary>
	///     A function that indicate the type of type(?).
	/// </summary>
	// Token: 0x02000091 RID: 145
	public class IsTypeFunction : PatternFunction
	{
		/// <inheritdoc />
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000366 RID: 870 RVA: 0x00015298 File Offset: 0x00013498
		public override string Name
		{
			get
			{
				return "is-type";
			}
		}

		/// <inheritdoc />
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000367 RID: 871 RVA: 0x00015248 File Offset: 0x00013448
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		/// <inheritdoc />
		// Token: 0x06000368 RID: 872 RVA: 0x000152B0 File Offset: 0x000134B0
		public override object Evaluate(IDnlibDef definition)
		{
			TypeDef type = definition as TypeDef;
			bool flag = type == null && definition is IMemberDef;
			if (flag)
			{
				type = ((IMemberDef)definition).DeclaringType;
			}
			bool flag2 = type == null;
			object result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				string typeRegex = base.Arguments[0].Evaluate(definition).ToString();
				StringBuilder typeType = new StringBuilder();
				bool isEnum = type.IsEnum;
				if (isEnum)
				{
					typeType.Append("enum ");
				}
				bool isInterface = type.IsInterface;
				if (isInterface)
				{
					typeType.Append("interface ");
				}
				bool isValueType = type.IsValueType;
				if (isValueType)
				{
					typeType.Append("valuetype ");
				}
				bool flag3 = type.IsDelegate();
				if (flag3)
				{
					typeType.Append("delegate ");
				}
				bool isAbstract = type.IsAbstract;
				if (isAbstract)
				{
					typeType.Append("abstract ");
				}
				bool isNested = type.IsNested;
				if (isNested)
				{
					typeType.Append("nested ");
				}
				bool isSerializable = type.IsSerializable;
				if (isSerializable)
				{
					typeType.Append("serializable ");
				}
				result = Regex.IsMatch(typeType.ToString(), typeRegex);
			}
			return result;
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0000385A File Offset: 0x00001A5A
		public IsTypeFunction()
		{
		}

		// Token: 0x0400025E RID: 606
		internal const string FnName = "is-type";
	}
}
