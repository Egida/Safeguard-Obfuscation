using System;

namespace Confuser.Core
{
	/// <summary>
	///     Various stages in <see cref="T:Confuser.Core.ProtectionPipeline" />.
	/// </summary>
	// Token: 0x0200006A RID: 106
	public enum PipelineStage
	{
		/// <summary>
		///     Confuser engine inspects the loaded modules and makes necessary changes.
		///     This stage occurs only once per pipeline run.
		/// </summary>
		// Token: 0x040001EF RID: 495
		Inspection,
		/// <summary>
		///     Confuser engine begins to process a module.
		///     This stage occurs once per module.
		/// </summary>
		// Token: 0x040001F0 RID: 496
		BeginModule,
		/// <summary>
		///     Confuser engine processes a module.
		///     This stage occurs once per module.
		/// </summary>
		// Token: 0x040001F1 RID: 497
		ProcessModule,
		/// <summary>
		///     Confuser engine optimizes opcodes of the method bodys.
		///     This stage occurs once per module.
		/// </summary>
		// Token: 0x040001F2 RID: 498
		OptimizeMethods,
		/// <summary>
		///     Confuser engine finishes processing a module.
		///     This stage occurs once per module.
		/// </summary>
		// Token: 0x040001F3 RID: 499
		EndModule,
		/// <summary>
		///     Confuser engine writes the module to byte array.
		///     This stage occurs once per module, after all processing of modules are completed.
		/// </summary>
		// Token: 0x040001F4 RID: 500
		WriteModule,
		/// <summary>
		///     Confuser engine generates debug symbols.
		///     This stage occurs only once per pipeline run.
		/// </summary>
		// Token: 0x040001F5 RID: 501
		Debug,
		/// <summary>
		///     Confuser engine packs up the output if packer is present.
		///     This stage occurs only once per pipeline run.
		/// </summary>
		// Token: 0x040001F6 RID: 502
		Pack,
		/// <summary>
		///     Confuser engine saves the output.
		///     This stage occurs only once per pipeline run.
		/// </summary>
		// Token: 0x040001F7 RID: 503
		SaveModules
	}
}
