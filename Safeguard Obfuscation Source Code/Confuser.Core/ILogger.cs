using System;

namespace Confuser.Core
{
	// Token: 0x0200004A RID: 74
	public interface ILogger
	{
		// Token: 0x060001B9 RID: 441
		void Debug(string msg);

		// Token: 0x060001BA RID: 442
		void DebugFormat(string format, params object[] args);

		// Token: 0x060001BB RID: 443
		void Info(string msg);

		// Token: 0x060001BC RID: 444
		void InfoFormat(string format, params object[] args);

		// Token: 0x060001BD RID: 445
		void Warn(string msg);

		// Token: 0x060001BE RID: 446
		void WarnFormat(string format, params object[] args);

		// Token: 0x060001BF RID: 447
		void WarnException(string msg, Exception ex);

		// Token: 0x060001C0 RID: 448
		void Error(string msg);

		// Token: 0x060001C1 RID: 449
		void ErrorFormat(string format, params object[] args);

		// Token: 0x060001C2 RID: 450
		void ErrorException(string msg, Exception ex);

		// Token: 0x060001C3 RID: 451
		void Progress(int progress, int overall);

		// Token: 0x060001C4 RID: 452
		void EndProgress();

		// Token: 0x060001C5 RID: 453
		void Finish(bool successful);
	}
}
