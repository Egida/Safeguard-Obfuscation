using System;
using System.Windows.Markup;

namespace ConfuserEx
{
	// Token: 0x0200000D RID: 13
	public class EnumValuesExtension : MarkupExtension
	{
		// Token: 0x06000036 RID: 54 RVA: 0x000021A6 File Offset: 0x000003A6
		public EnumValuesExtension(Type enumType)
		{
			this.enumType = enumType;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003274 File Offset: 0x00001474
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Enum.GetValues(this.enumType);
		}

		// Token: 0x04000014 RID: 20
		private readonly Type enumType;
	}
}
