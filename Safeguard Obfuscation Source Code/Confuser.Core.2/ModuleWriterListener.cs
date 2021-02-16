using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace Confuser.Core
{
	/// <summary>
	///     The listener of module writer event.
	/// </summary>
	// Token: 0x0200004E RID: 78
	public class ModuleWriterListener : IModuleWriterListener
	{
		/// <inheritdoc />
		// Token: 0x060001DD RID: 477 RVA: 0x0000E7B8 File Offset: 0x0000C9B8
		void IModuleWriterListener.OnWriterEvent(ModuleWriterBase writer, ModuleWriterEvent evt)
		{
			bool flag = evt == ModuleWriterEvent.PESectionsCreated;
			if (flag)
			{
				NativeEraser.Erase(writer as NativeModuleWriter, writer.Module as ModuleDefMD);
			}
			bool flag2 = this.OnWriterEvent != null;
			if (flag2)
			{
				this.OnWriterEvent(writer, new ModuleWriterListenerEventArgs(evt));
			}
		}

		/// <summary>
		///     Occurs when a module writer event is triggered.
		/// </summary>
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060001DE RID: 478 RVA: 0x0000E808 File Offset: 0x0000CA08
		// (remove) Token: 0x060001DF RID: 479 RVA: 0x0000E840 File Offset: 0x0000CA40
		public event EventHandler<ModuleWriterListenerEventArgs> OnWriterEvent
		{
			[CompilerGenerated]
			add
			{
				EventHandler<ModuleWriterListenerEventArgs> eventHandler = this.OnWriterEvent;
				EventHandler<ModuleWriterListenerEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler<ModuleWriterListenerEventArgs> value2 = (EventHandler<ModuleWriterListenerEventArgs>)Delegate.Combine(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange<EventHandler<ModuleWriterListenerEventArgs>>(ref this.OnWriterEvent, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<ModuleWriterListenerEventArgs> eventHandler = this.OnWriterEvent;
				EventHandler<ModuleWriterListenerEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler<ModuleWriterListenerEventArgs> value2 = (EventHandler<ModuleWriterListenerEventArgs>)Delegate.Remove(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange<EventHandler<ModuleWriterListenerEventArgs>>(ref this.OnWriterEvent, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00002194 File Offset: 0x00000394
		public ModuleWriterListener()
		{
		}

		// Token: 0x0400018F RID: 399
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<ModuleWriterListenerEventArgs> OnWriterEvent;
	}
}
