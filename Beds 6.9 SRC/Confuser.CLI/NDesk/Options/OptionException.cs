using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NDesk.Options
{
	// Token: 0x02000007 RID: 7
	[Serializable]
	public class OptionException : Exception
	{
		// Token: 0x06000042 RID: 66 RVA: 0x0000309F File Offset: 0x0000129F
		public OptionException()
		{
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000030A9 File Offset: 0x000012A9
		public OptionException(string message, string optionName) : base(message)
		{
			this.option = optionName;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000030BB File Offset: 0x000012BB
		public OptionException(string message, string optionName, Exception innerException) : base(message, innerException)
		{
			this.option = optionName;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000030CE File Offset: 0x000012CE
		protected OptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.option = info.GetString("OptionName");
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000030EC File Offset: 0x000012EC
		public string OptionName
		{
			get
			{
				return this.option;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003104 File Offset: 0x00001304
		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("OptionName", this.option);
		}

		// Token: 0x04000013 RID: 19
		private string option;
	}
}
