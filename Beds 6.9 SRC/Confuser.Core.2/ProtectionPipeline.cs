using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Protection processing pipeline.
	/// </summary>
	// Token: 0x0200006B RID: 107
	public class ProtectionPipeline
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionPipeline" /> class.
		/// </summary>
		// Token: 0x06000288 RID: 648 RVA: 0x0001178C File Offset: 0x0000F98C
		public ProtectionPipeline()
		{
			PipelineStage[] stages = (PipelineStage[])Enum.GetValues(typeof(PipelineStage));
			this.preStage = stages.ToDictionary((PipelineStage stage) => stage, (PipelineStage stage) => new List<ProtectionPhase>());
			this.postStage = stages.ToDictionary((PipelineStage stage) => stage, (PipelineStage stage) => new List<ProtectionPhase>());
		}

		/// <summary>
		///     Inserts the phase into pre-processing pipeline of the specified stage.
		/// </summary>
		/// <param name="stage">The pipeline stage.</param>
		/// <param name="phase">The protection phase.</param>
		// Token: 0x06000289 RID: 649 RVA: 0x000031F5 File Offset: 0x000013F5
		public void InsertPreStage(PipelineStage stage, ProtectionPhase phase)
		{
			this.preStage[stage].Add(phase);
		}

		/// <summary>
		///     Inserts the phase into post-processing pipeline of the specified stage.
		/// </summary>
		/// <param name="stage">The pipeline stage.</param>
		/// <param name="phase">The protection phase.</param>
		// Token: 0x0600028A RID: 650 RVA: 0x0000320B File Offset: 0x0000140B
		public void InsertPostStage(PipelineStage stage, ProtectionPhase phase)
		{
			this.postStage[stage].Add(phase);
		}

		/// <summary>
		///     Finds the phase with the specified type in the pipeline.
		/// </summary>
		/// <typeparam name="T">The type of the phase.</typeparam>
		/// <returns>The phase with specified type in the pipeline.</returns>
		// Token: 0x0600028B RID: 651 RVA: 0x0001184C File Offset: 0x0000FA4C
		public T FindPhase<T>() where T : ProtectionPhase
		{
			foreach (List<ProtectionPhase> phases in this.preStage.Values)
			{
				foreach (ProtectionPhase phase in phases)
				{
					bool flag = phase is T;
					if (flag)
					{
						return (T)((object)phase);
					}
				}
			}
			foreach (List<ProtectionPhase> phases2 in this.postStage.Values)
			{
				foreach (ProtectionPhase phase2 in phases2)
				{
					bool flag2 = phase2 is T;
					if (flag2)
					{
						return (T)((object)phase2);
					}
				}
			}
			return default(T);
		}

		/// <summary>
		///     Execute the specified pipeline stage with pre-processing and post-processing.
		/// </summary>
		/// <param name="stage">The pipeline stage.</param>
		/// <param name="func">The stage function.</param>
		/// <param name="targets">The target list of the stage.</param>
		/// <param name="context">The working context.</param>
		// Token: 0x0600028C RID: 652 RVA: 0x000119A4 File Offset: 0x0000FBA4
		internal void ExecuteStage(PipelineStage stage, Action<ConfuserContext> func, Func<IList<IDnlibDef>> targets, ConfuserContext context)
		{
			foreach (ProtectionPhase pre in this.preStage[stage])
			{
				context.CheckCancellation();
				context.Logger.DebugFormat("Executing '{0}' phase...", new object[]
				{
					pre.Name
				});
				pre.Execute(context, new ProtectionParameters(pre.Parent, ProtectionPipeline.Filter(context, targets(), pre)));
			}
			context.CheckCancellation();
			func(context);
			context.CheckCancellation();
			foreach (ProtectionPhase post in this.postStage[stage])
			{
				context.Logger.DebugFormat("Executing '{0}' phase...", new object[]
				{
					post.Name
				});
				post.Execute(context, new ProtectionParameters(post.Parent, ProtectionPipeline.Filter(context, targets(), post)));
				context.CheckCancellation();
			}
		}

		/// <summary>
		///     Returns only the targets with the specified type and used by specified component.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="targets">List of targets.</param>
		/// <param name="phase">The component phase.</param>
		/// <returns>Filtered targets.</returns>
		// Token: 0x0600028D RID: 653 RVA: 0x00011AF0 File Offset: 0x0000FCF0
		private static IList<IDnlibDef> Filter(ConfuserContext context, IList<IDnlibDef> targets, ProtectionPhase phase)
		{
			ProtectionTargets targetType = phase.Targets;
			IEnumerable<IDnlibDef> filter = targets;
			bool flag = (targetType & ProtectionTargets.Modules) == (ProtectionTargets)0;
			if (flag)
			{
				filter = from def in filter
				where !(def is ModuleDef)
				select def;
			}
			bool flag2 = (targetType & ProtectionTargets.Types) == (ProtectionTargets)0;
			if (flag2)
			{
				filter = from def in filter
				where !(def is TypeDef)
				select def;
			}
			bool flag3 = (targetType & ProtectionTargets.Methods) == (ProtectionTargets)0;
			if (flag3)
			{
				filter = from def in filter
				where !(def is MethodDef)
				select def;
			}
			bool flag4 = (targetType & ProtectionTargets.Fields) == (ProtectionTargets)0;
			if (flag4)
			{
				filter = from def in filter
				where !(def is FieldDef)
				select def;
			}
			bool flag5 = (targetType & ProtectionTargets.Properties) == (ProtectionTargets)0;
			if (flag5)
			{
				filter = from def in filter
				where !(def is PropertyDef)
				select def;
			}
			bool flag6 = (targetType & ProtectionTargets.Events) == (ProtectionTargets)0;
			if (flag6)
			{
				filter = from def in filter
				where !(def is EventDef)
				select def;
			}
			bool processAll = phase.ProcessAll;
			IList<IDnlibDef> result;
			if (processAll)
			{
				result = filter.ToList<IDnlibDef>();
			}
			else
			{
				result = filter.Where(delegate(IDnlibDef def)
				{
					ProtectionSettings parameters = ProtectionParameters.GetParameters(context, def);
					Debug.Assert(parameters != null);
					bool flag7 = parameters == null;
					if (flag7)
					{
						context.Logger.ErrorFormat("'{0}' not marked for obfuscation, possibly a bug.", new object[]
						{
							def
						});
						throw new ConfuserException(null);
					}
					return parameters.ContainsKey(phase.Parent);
				}).ToList<IDnlibDef>();
			}
			return result;
		}

		// Token: 0x040001F8 RID: 504
		private readonly Dictionary<PipelineStage, List<ProtectionPhase>> postStage;

		// Token: 0x040001F9 RID: 505
		private readonly Dictionary<PipelineStage, List<ProtectionPhase>> preStage;

		// Token: 0x0200006C RID: 108
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600028E RID: 654 RVA: 0x00003221 File Offset: 0x00001421
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600028F RID: 655 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x06000290 RID: 656 RVA: 0x00002942 File Offset: 0x00000B42
			internal PipelineStage <.ctor>b__2_0(PipelineStage stage)
			{
				return stage;
			}

			// Token: 0x06000291 RID: 657 RVA: 0x0000322D File Offset: 0x0000142D
			internal List<ProtectionPhase> <.ctor>b__2_1(PipelineStage stage)
			{
				return new List<ProtectionPhase>();
			}

			// Token: 0x06000292 RID: 658 RVA: 0x00002942 File Offset: 0x00000B42
			internal PipelineStage <.ctor>b__2_2(PipelineStage stage)
			{
				return stage;
			}

			// Token: 0x06000293 RID: 659 RVA: 0x0000322D File Offset: 0x0000142D
			internal List<ProtectionPhase> <.ctor>b__2_3(PipelineStage stage)
			{
				return new List<ProtectionPhase>();
			}

			// Token: 0x06000294 RID: 660 RVA: 0x00003234 File Offset: 0x00001434
			internal bool <Filter>b__7_0(IDnlibDef def)
			{
				return !(def is ModuleDef);
			}

			// Token: 0x06000295 RID: 661 RVA: 0x00003242 File Offset: 0x00001442
			internal bool <Filter>b__7_1(IDnlibDef def)
			{
				return !(def is TypeDef);
			}

			// Token: 0x06000296 RID: 662 RVA: 0x00003250 File Offset: 0x00001450
			internal bool <Filter>b__7_2(IDnlibDef def)
			{
				return !(def is MethodDef);
			}

			// Token: 0x06000297 RID: 663 RVA: 0x0000325E File Offset: 0x0000145E
			internal bool <Filter>b__7_3(IDnlibDef def)
			{
				return !(def is FieldDef);
			}

			// Token: 0x06000298 RID: 664 RVA: 0x0000326C File Offset: 0x0000146C
			internal bool <Filter>b__7_4(IDnlibDef def)
			{
				return !(def is PropertyDef);
			}

			// Token: 0x06000299 RID: 665 RVA: 0x0000327A File Offset: 0x0000147A
			internal bool <Filter>b__7_5(IDnlibDef def)
			{
				return !(def is EventDef);
			}

			// Token: 0x040001FA RID: 506
			public static readonly ProtectionPipeline.<>c <>9 = new ProtectionPipeline.<>c();

			// Token: 0x040001FB RID: 507
			public static Func<PipelineStage, PipelineStage> <>9__2_0;

			// Token: 0x040001FC RID: 508
			public static Func<PipelineStage, List<ProtectionPhase>> <>9__2_1;

			// Token: 0x040001FD RID: 509
			public static Func<PipelineStage, PipelineStage> <>9__2_2;

			// Token: 0x040001FE RID: 510
			public static Func<PipelineStage, List<ProtectionPhase>> <>9__2_3;

			// Token: 0x040001FF RID: 511
			public static Func<IDnlibDef, bool> <>9__7_0;

			// Token: 0x04000200 RID: 512
			public static Func<IDnlibDef, bool> <>9__7_1;

			// Token: 0x04000201 RID: 513
			public static Func<IDnlibDef, bool> <>9__7_2;

			// Token: 0x04000202 RID: 514
			public static Func<IDnlibDef, bool> <>9__7_3;

			// Token: 0x04000203 RID: 515
			public static Func<IDnlibDef, bool> <>9__7_4;

			// Token: 0x04000204 RID: 516
			public static Func<IDnlibDef, bool> <>9__7_5;
		}

		// Token: 0x0200006D RID: 109
		[CompilerGenerated]
		private sealed class <>c__DisplayClass7_0
		{
			// Token: 0x0600029A RID: 666 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass7_0()
			{
			}

			// Token: 0x0600029B RID: 667 RVA: 0x00011C84 File Offset: 0x0000FE84
			internal bool <Filter>b__6(IDnlibDef def)
			{
				ProtectionSettings parameters = ProtectionParameters.GetParameters(this.context, def);
				Debug.Assert(parameters != null);
				bool flag = parameters == null;
				if (flag)
				{
					this.context.Logger.ErrorFormat("'{0}' not marked for obfuscation, possibly a bug.", new object[]
					{
						def
					});
					throw new ConfuserException(null);
				}
				return parameters.ContainsKey(this.phase.Parent);
			}

			// Token: 0x04000205 RID: 517
			public ConfuserContext context;

			// Token: 0x04000206 RID: 518
			public ProtectionPhase phase;
		}
	}
}
