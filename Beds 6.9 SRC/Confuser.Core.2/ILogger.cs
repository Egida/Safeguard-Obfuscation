using System;

namespace Confuser.Core
{
	/// <summary>
	///     Defines a logger used to log Confuser events
	/// </summary>
	// Token: 0x0200004A RID: 74
	public interface ILogger
	{
		/// <summary>
		///     Logs a message at DEBUG level.
		/// </summary>
		/// <param name="msg">The message.</param>
		// Token: 0x060001B9 RID: 441
		void Debug(string msg);

		/// <summary>
		///     Logs a message at DEBUG level with specified parameters.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		// Token: 0x060001BA RID: 442
		void DebugFormat(string format, params object[] args);

		/// <summary>
		///     Logs a message at INFO level.
		/// </summary>
		/// <param name="msg">The message.</param>
		// Token: 0x060001BB RID: 443
		void Info(string msg);

		/// <summary>
		///     Logs a message at INFO level with specified parameters.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		// Token: 0x060001BC RID: 444
		void InfoFormat(string format, params object[] args);

		/// <summary>
		///     Logs a message at WARN level.
		/// </summary>
		/// <param name="msg">The message.</param>
		// Token: 0x060001BD RID: 445
		void Warn(string msg);

		/// <summary>
		///     Logs a message at WARN level with specified parameters.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		// Token: 0x060001BE RID: 446
		void WarnFormat(string format, params object[] args);

		/// <summary>
		///     Logs a message at WARN level with specified exception.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="ex">The exception.</param>
		// Token: 0x060001BF RID: 447
		void WarnException(string msg, Exception ex);

		/// <summary>
		///     Logs a message at ERROR level.
		/// </summary>
		/// <param name="msg">The message.</param>
		// Token: 0x060001C0 RID: 448
		void Error(string msg);

		/// <summary>
		///     Logs a message at ERROR level with specified parameters.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		// Token: 0x060001C1 RID: 449
		void ErrorFormat(string format, params object[] args);

		/// <summary>
		///     Logs a message at ERROR level with specified exception.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="ex">The exception.</param>
		// Token: 0x060001C2 RID: 450
		void ErrorException(string msg, Exception ex);

		/// <summary>
		///     Logs the progress of protection.
		/// </summary>
		/// <remarks>
		///     This method is intended to be used with <see cref="M:Confuser.Core.ILogger.EndProgress" />.
		/// </remarks>
		/// <example>
		///     <code> 
		///         for (int i = 0; i &lt; defs.Length; i++) {
		///             logger.Progress(i + 1, defs.Length);
		///         }
		///         logger.EndProgress();
		///     </code>
		/// </example>
		/// <param name="overall">The total work amount .</param>
		/// <param name="progress">The amount of work done.</param>
		// Token: 0x060001C3 RID: 451
		void Progress(int progress, int overall);

		/// <summary>
		///     End the progress of protection.
		/// </summary>
		/// <seealso cref="M:Confuser.Core.ILogger.Progress(System.Int32,System.Int32)" />
		// Token: 0x060001C4 RID: 452
		void EndProgress();

		/// <summary>
		///     Logs the finish of protection.
		/// </summary>
		/// <param name="successful">Indicated whether the protection process is successful.</param>
		// Token: 0x060001C5 RID: 453
		void Finish(bool successful);
	}
}
