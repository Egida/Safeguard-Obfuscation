using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Writer;

namespace Confuser.Core
{
	// Token: 0x0200004F RID: 79
	public class ModuleWriterListenerEventArgs : EventArgs
	{
		// Token: 0x060001E1 RID: 481 RVA: 0x00002CAB File Offset: 0x00000EAB
		public ModuleWriterListenerEventArgs(ModuleWriterEvent evt)
		{
			this.WriterEvent = evt;
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x00002CBD File Offset: 0x00000EBD
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x00002CC5 File Offset: 0x00000EC5
		public ModuleWriterEvent WriterEvent
		{
			[CompilerGenerated]
			get
			{
				return this.<WriterEvent>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<WriterEvent>k__BackingField = value;
			}
		}

		// Token: 0x04000190 RID: 400
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ModuleWriterEvent <WriterEvent>k__BackingField;
	}
}
