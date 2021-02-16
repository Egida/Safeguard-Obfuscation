using System;

namespace Confuser.Core
{
	// Token: 0x0200005F RID: 95
	internal class PackerLogger : ILogger
	{
		// Token: 0x06000232 RID: 562 RVA: 0x00002EB0 File Offset: 0x000010B0
		public PackerLogger(ILogger baseLogger)
		{
			this.baseLogger = baseLogger;
		}

		// Token: 0x06000233 RID: 563 RVA: 0x00002EC1 File Offset: 0x000010C1
		public void Debug(string msg)
		{
			this.baseLogger.Debug(msg);
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00002ED1 File Offset: 0x000010D1
		public void DebugFormat(string format, params object[] args)
		{
			this.baseLogger.DebugFormat(format, args);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00002EE2 File Offset: 0x000010E2
		public void Info(string msg)
		{
			this.baseLogger.Info(msg);
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00002EF2 File Offset: 0x000010F2
		public void InfoFormat(string format, params object[] args)
		{
			this.baseLogger.InfoFormat(format, args);
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00002F03 File Offset: 0x00001103
		public void Warn(string msg)
		{
			this.baseLogger.Warn(msg);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00002F13 File Offset: 0x00001113
		public void WarnFormat(string format, params object[] args)
		{
			this.baseLogger.WarnFormat(format, args);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00002F24 File Offset: 0x00001124
		public void WarnException(string msg, Exception ex)
		{
			this.baseLogger.WarnException(msg, ex);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00002F35 File Offset: 0x00001135
		public void Error(string msg)
		{
			this.baseLogger.Error(msg);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00002F45 File Offset: 0x00001145
		public void ErrorFormat(string format, params object[] args)
		{
			this.baseLogger.ErrorFormat(format, args);
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00002F56 File Offset: 0x00001156
		public void ErrorException(string msg, Exception ex)
		{
			this.baseLogger.ErrorException(msg, ex);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00002F67 File Offset: 0x00001167
		public void Progress(int progress, int overall)
		{
			this.baseLogger.Progress(progress, overall);
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00002F78 File Offset: 0x00001178
		public void EndProgress()
		{
			this.baseLogger.EndProgress();
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00011368 File Offset: 0x0000F568
		public void Finish(bool successful)
		{
			bool flag = !successful;
			if (flag)
			{
				throw new ConfuserException(null);
			}
			this.baseLogger.Info("Finish protecting packer stub.");
		}

		// Token: 0x040001C6 RID: 454
		private readonly ILogger baseLogger;
	}
}
