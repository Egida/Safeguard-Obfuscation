using System;

namespace SevenZip
{
	// Token: 0x02000005 RID: 5
	internal interface ICodeProgress
	{
		/// <summary>
		///     Callback progress.
		/// </summary>
		/// <param name="inSize">
		///     input size. -1 if unknown.
		/// </param>
		/// <param name="outSize">
		///     output size. -1 if unknown.
		/// </param>
		// Token: 0x0600000B RID: 11
		void SetProgress(long inSize, long outSize);
	}
}
