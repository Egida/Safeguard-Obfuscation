using System;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     An <see cref="T:Confuser.Core.ILogger" /> implementation that doesn't actually do any logging.
	/// </summary>
	// Token: 0x0200005D RID: 93
	internal class NullLogger : ILogger
	{
		/// <summary>
		///     Prevents a default instance of the <see cref="T:Confuser.Core.NullLogger" /> class from being created.
		/// </summary>
		// Token: 0x0600021E RID: 542 RVA: 0x00002563 File Offset: 0x00000763
		private NullLogger()
		{
		}

		/// <inheritdoc />
		// Token: 0x0600021F RID: 543 RVA: 0x000025D6 File Offset: 0x000007D6
		public void Debug(string msg)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000220 RID: 544 RVA: 0x000025D6 File Offset: 0x000007D6
		public void DebugFormat(string format, params object[] args)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000221 RID: 545 RVA: 0x000025D6 File Offset: 0x000007D6
		public void Info(string msg)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000222 RID: 546 RVA: 0x000025D6 File Offset: 0x000007D6
		public void InfoFormat(string format, params object[] args)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000223 RID: 547 RVA: 0x000025D6 File Offset: 0x000007D6
		public void Warn(string msg)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000224 RID: 548 RVA: 0x000025D6 File Offset: 0x000007D6
		public void WarnFormat(string format, params object[] args)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000225 RID: 549 RVA: 0x000025D6 File Offset: 0x000007D6
		public void WarnException(string msg, Exception ex)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000226 RID: 550 RVA: 0x000025D6 File Offset: 0x000007D6
		public void Error(string msg)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000227 RID: 551 RVA: 0x000025D6 File Offset: 0x000007D6
		public void ErrorFormat(string format, params object[] args)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000228 RID: 552 RVA: 0x000025D6 File Offset: 0x000007D6
		public void ErrorException(string msg, Exception ex)
		{
		}

		/// <inheritdoc />
		// Token: 0x06000229 RID: 553 RVA: 0x000025D6 File Offset: 0x000007D6
		public void Progress(int overall, int progress)
		{
		}

		/// <inheritdoc />
		// Token: 0x0600022A RID: 554 RVA: 0x000025D6 File Offset: 0x000007D6
		public void EndProgress()
		{
		}

		/// <inheritdoc />
		// Token: 0x0600022B RID: 555 RVA: 0x000025D6 File Offset: 0x000007D6
		public void Finish(bool successful)
		{
		}

		/// <inheritdoc />
		// Token: 0x0600022C RID: 556 RVA: 0x000025D6 File Offset: 0x000007D6
		public void BeginModule(ModuleDef module)
		{
		}

		/// <inheritdoc />
		// Token: 0x0600022D RID: 557 RVA: 0x000025D6 File Offset: 0x000007D6
		public void EndModule(ModuleDef module)
		{
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00002E9B File Offset: 0x0000109B
		// Note: this type is marked as 'beforefieldinit'.
		static NullLogger()
		{
		}

		/// <summary>
		///     The singleton instance of <see cref="T:Confuser.Core.NullLogger" />.
		/// </summary>
		// Token: 0x040001C5 RID: 453
		public static readonly NullLogger Instance = new NullLogger();
	}
}
