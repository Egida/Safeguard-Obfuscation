using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x02000077 RID: 119
	public interface ICompressionService
	{
		// Token: 0x060002C4 RID: 708
		byte[] Compress(byte[] data, Action<double> progressFunc = null);

		// Token: 0x060002C5 RID: 709
		MethodDef GetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init);

		// Token: 0x060002C6 RID: 710
		MethodDef TryGetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init);
	}
}
