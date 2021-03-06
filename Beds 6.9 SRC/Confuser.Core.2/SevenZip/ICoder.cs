using System;
using System.IO;

namespace SevenZip
{
	// Token: 0x02000006 RID: 6
	internal interface ICoder
	{
		/// <summary>
		///     Codes streams.
		/// </summary>
		/// <param name="inStream">
		///     input Stream.
		/// </param>
		/// <param name="outStream">
		///     output Stream.
		/// </param>
		/// <param name="inSize">
		///     input Size. -1 if unknown.
		/// </param>
		/// <param name="outSize">
		///     output Size. -1 if unknown.
		/// </param>
		/// <param name="progress">
		///     callback progress reference.
		/// </param>
		/// <exception cref="T:SevenZip.DataErrorException">
		///     if input stream is not valid
		/// </exception>
		// Token: 0x0600000C RID: 12
		void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress);
	}
}
