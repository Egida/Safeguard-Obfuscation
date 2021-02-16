using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	/// <summary>
	///     Provides methods to do compression and inject decompression algorithm.
	/// </summary>
	// Token: 0x02000077 RID: 119
	public interface ICompressionService
	{
		/// <summary>
		///     Compresses the specified data.
		/// </summary>
		/// <param name="data">The buffer storing the data.</param>
		/// <param name="progressFunc">The function that receive the progress of compression.</param>
		/// <returns>The compressed data.</returns>
		// Token: 0x060002C4 RID: 708
		byte[] Compress(byte[] data, Action<double> progressFunc = null);

		/// <summary>
		///     Gets the runtime decompression method in the module and inject if it does not exists.
		/// </summary>
		/// <param name="module">The module which the decompression method resides in.</param>
		/// <param name="init">The initializing method for injected helper definitions.</param>
		/// <returns>The requested decompression method with signature 'static Byte[] (Byte[])'.</returns>
		// Token: 0x060002C5 RID: 709
		MethodDef GetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init);

		/// <summary>
		///     Gets the runtime decompression method in the module, or null if it's not yet injected.
		/// </summary>
		/// <param name="module">The module which the decompression method resides in.</param>
		/// <param name="init">The initializing method for compression helper definitions.</param>
		/// <returns>
		///     The requested decompression method with signature 'static Byte[] (Byte[])',
		///     or null if it hasn't been injected yet.
		/// </returns>
		// Token: 0x060002C6 RID: 710
		MethodDef TryGetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init);
	}
}
