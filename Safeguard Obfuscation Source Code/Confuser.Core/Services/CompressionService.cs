using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core.Helpers;
using dnlib.DotNet;
using SevenZip;
using SevenZip.Compression.LZMA;

namespace Confuser.Core.Services
{
	// Token: 0x02000073 RID: 115
	internal class CompressionService : ICompressionService
	{
		// Token: 0x060002B8 RID: 696 RVA: 0x0000331B File Offset: 0x0000151B
		public CompressionService(ConfuserContext context)
		{
			this.context = context;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00012288 File Offset: 0x00010488
		public byte[] Compress(byte[] data, Action<double> progressFunc = null)
		{
			CoderPropID[] propIDs = new CoderPropID[]
			{
				CoderPropID.DictionarySize,
				CoderPropID.PosStateBits,
				CoderPropID.LitContextBits,
				CoderPropID.LitPosBits,
				CoderPropID.Algorithm,
				CoderPropID.NumFastBytes,
				CoderPropID.MatchFinder,
				CoderPropID.EndMarker
			};
			object[] properties = new object[]
			{
				8388608,
				2,
				3,
				0,
				2,
				128,
				"bt4",
				false
			};
			MemoryStream x = new MemoryStream();
			Encoder encoder = new Encoder();
			encoder.SetCoderProperties(propIDs, properties);
			encoder.WriteCoderProperties(x);
			long fileSize = (long)data.Length;
			for (int i = 0; i < 8; i++)
			{
				x.WriteByte((byte)(fileSize >> 8 * i));
			}
			ICodeProgress progress = null;
			bool flag = progressFunc != null;
			if (flag)
			{
				progress = new CompressionService.CompressionLogger(progressFunc, data.Length);
			}
			encoder.Code(new MemoryStream(data), x, -1L, -1L, progress);
			return x.ToArray();
		}

		// Token: 0x060002BA RID: 698 RVA: 0x000123A0 File Offset: 0x000105A0
		public MethodDef GetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init)
		{
			Tuple<MethodDef, List<IDnlibDef>> decompressor = this.context.Annotations.GetOrCreate<Tuple<MethodDef, List<IDnlibDef>>>(module, CompressionService.Decompressor, delegate(object m)
			{
				IRuntimeService rt = this.context.Registry.GetService<IRuntimeService>();
				List<IDnlibDef> members = InjectHelper.Inject(rt.GetRuntimeType("Confuser.Runtime.Lzma"), module.GlobalType, module).ToList<IDnlibDef>();
				MethodDef decomp = null;
				foreach (IDnlibDef member2 in members)
				{
					bool flag = member2 is MethodDef;
					if (flag)
					{
						MethodDef method = (MethodDef)member2;
						bool flag2 = method.Access == MethodAttributes.Public;
						if (flag2)
						{
							method.Access = MethodAttributes.Assembly;
						}
						bool flag3 = !method.IsConstructor;
						if (flag3)
						{
							method.IsSpecialName = false;
						}
						bool flag4 = method.Name == "Decompress";
						if (flag4)
						{
							decomp = method;
						}
					}
					else
					{
						bool flag5 = member2 is FieldDef;
						if (flag5)
						{
							FieldDef field = (FieldDef)member2;
							bool flag6 = field.Access == FieldAttributes.Public;
							if (flag6)
							{
								field.Access = FieldAttributes.Assembly;
							}
							bool isLiteral = field.IsLiteral;
							if (isLiteral)
							{
								field.DeclaringType.Fields.Remove(field);
							}
						}
					}
				}
				members.RemoveWhere((IDnlibDef def) => def is FieldDef && ((FieldDef)def).IsLiteral);
				return Tuple.Create<MethodDef, List<IDnlibDef>>(decomp, members);
			});
			foreach (IDnlibDef member in decompressor.Item2)
			{
				init(member);
			}
			return decompressor.Item1;
		}

		// Token: 0x060002BB RID: 699 RVA: 0x00012444 File Offset: 0x00010644
		public MethodDef TryGetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init)
		{
			Tuple<MethodDef, List<IDnlibDef>> decompressor = this.context.Annotations.Get<Tuple<MethodDef, List<IDnlibDef>>>(module, CompressionService.Decompressor, null);
			bool flag = decompressor == null;
			MethodDef result;
			if (flag)
			{
				result = null;
			}
			else
			{
				foreach (IDnlibDef member in decompressor.Item2)
				{
					init(member);
				}
				result = decompressor.Item1;
			}
			return result;
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000332C File Offset: 0x0000152C
		// Note: this type is marked as 'beforefieldinit'.
		static CompressionService()
		{
		}

		// Token: 0x04000219 RID: 537
		private readonly ConfuserContext context;

		// Token: 0x0400021A RID: 538
		private static readonly object Decompressor = new object();

		// Token: 0x02000074 RID: 116
		private class CompressionLogger : ICodeProgress
		{
			// Token: 0x060002BD RID: 701 RVA: 0x00003338 File Offset: 0x00001538
			public CompressionLogger(Action<double> progressFunc, int size)
			{
				this.progressFunc = progressFunc;
				this.size = size;
			}

			// Token: 0x060002BE RID: 702 RVA: 0x000124D0 File Offset: 0x000106D0
			public void SetProgress(long inSize, long outSize)
			{
				double precentage = (double)inSize / (double)this.size;
				this.progressFunc(precentage);
			}

			// Token: 0x0400021B RID: 539
			private readonly Action<double> progressFunc;

			// Token: 0x0400021C RID: 540
			private readonly int size;
		}

		// Token: 0x02000075 RID: 117
		[CompilerGenerated]
		private sealed class <>c__DisplayClass2_0
		{
			// Token: 0x060002BF RID: 703 RVA: 0x00002194 File Offset: 0x00000394
			public <>c__DisplayClass2_0()
			{
			}

			// Token: 0x060002C0 RID: 704 RVA: 0x000124F8 File Offset: 0x000106F8
			internal Tuple<MethodDef, List<IDnlibDef>> <GetRuntimeDecompressor>b__0(object m)
			{
				IRuntimeService rt = this.<>4__this.context.Registry.GetService<IRuntimeService>();
				List<IDnlibDef> members = InjectHelper.Inject(rt.GetRuntimeType("Confuser.Runtime.Lzma"), this.module.GlobalType, this.module).ToList<IDnlibDef>();
				MethodDef decomp = null;
				foreach (IDnlibDef member2 in members)
				{
					bool flag = member2 is MethodDef;
					if (flag)
					{
						MethodDef method = (MethodDef)member2;
						bool flag2 = method.Access == MethodAttributes.Public;
						if (flag2)
						{
							method.Access = MethodAttributes.Assembly;
						}
						bool flag3 = !method.IsConstructor;
						if (flag3)
						{
							method.IsSpecialName = false;
						}
						bool flag4 = method.Name == "Decompress";
						if (flag4)
						{
							decomp = method;
						}
					}
					else
					{
						bool flag5 = member2 is FieldDef;
						if (flag5)
						{
							FieldDef field = (FieldDef)member2;
							bool flag6 = field.Access == FieldAttributes.Public;
							if (flag6)
							{
								field.Access = FieldAttributes.Assembly;
							}
							bool isLiteral = field.IsLiteral;
							if (isLiteral)
							{
								field.DeclaringType.Fields.Remove(field);
							}
						}
					}
				}
				members.RemoveWhere(new Predicate<IDnlibDef>(CompressionService.<>c.<>9.<GetRuntimeDecompressor>b__2_1));
				return Tuple.Create<MethodDef, List<IDnlibDef>>(decomp, members);
			}

			// Token: 0x0400021D RID: 541
			public ModuleDef module;

			// Token: 0x0400021E RID: 542
			public CompressionService <>4__this;
		}

		// Token: 0x02000076 RID: 118
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060002C1 RID: 705 RVA: 0x00003350 File Offset: 0x00001550
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060002C2 RID: 706 RVA: 0x00002194 File Offset: 0x00000394
			public <>c()
			{
			}

			// Token: 0x060002C3 RID: 707 RVA: 0x0000335C File Offset: 0x0000155C
			internal bool <GetRuntimeDecompressor>b__2_1(IDnlibDef def)
			{
				return def is FieldDef && ((FieldDef)def).IsLiteral;
			}

			// Token: 0x0400021F RID: 543
			public static readonly CompressionService.<>c <>9 = new CompressionService.<>c();

			// Token: 0x04000220 RID: 544
			public static Predicate<IDnlibDef> <>9__2_1;
		}
	}
}
