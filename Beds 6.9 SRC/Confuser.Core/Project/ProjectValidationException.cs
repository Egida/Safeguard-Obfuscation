﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Schema;

namespace Confuser.Core.Project
{
	// Token: 0x0200008B RID: 139
	public class ProjectValidationException : Exception
	{
		// Token: 0x06000349 RID: 841 RVA: 0x00003791 File Offset: 0x00001991
		internal ProjectValidationException(List<XmlSchemaException> exceptions) : base(exceptions[0].Message)
		{
			this.Errors = exceptions;
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600034A RID: 842 RVA: 0x000037AF File Offset: 0x000019AF
		// (set) Token: 0x0600034B RID: 843 RVA: 0x000037B7 File Offset: 0x000019B7
		public IList<XmlSchemaException> Errors
		{
			[CompilerGenerated]
			get
			{
				return this.<Errors>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Errors>k__BackingField = value;
			}
		}

		// Token: 0x04000253 RID: 595
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<XmlSchemaException> <Errors>k__BackingField;
	}
}
