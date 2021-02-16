using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000AD RID: 173
	internal class JITBodyIndex : IChunk
	{
		// Token: 0x0600029A RID: 666 RVA: 0x00015ADC File Offset: 0x00013CDC
		public JITBodyIndex(IEnumerable<uint> tokens)
		{
			this.bodies = tokens.ToDictionary((uint token) => token, (uint token) => null);
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600029B RID: 667 RVA: 0x000055FB File Offset: 0x000037FB
		// (set) Token: 0x0600029C RID: 668 RVA: 0x00005603 File Offset: 0x00003803
		public FileOffset FileOffset
		{
			[CompilerGenerated]
			get
			{
				return this.<FileOffset>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<FileOffset>k__BackingField = value;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600029D RID: 669 RVA: 0x0000560C File Offset: 0x0000380C
		// (set) Token: 0x0600029E RID: 670 RVA: 0x00005614 File Offset: 0x00003814
		public RVA RVA
		{
			[CompilerGenerated]
			get
			{
				return this.<RVA>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<RVA>k__BackingField = value;
			}
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000561D File Offset: 0x0000381D
		public void SetOffset(FileOffset offset, RVA rva)
		{
			this.FileOffset = offset;
			this.RVA = rva;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00015B3C File Offset: 0x00013D3C
		public uint GetFileLength()
		{
			return (uint)(this.bodies.Count * 8 + 4);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00015B60 File Offset: 0x00013D60
		public uint GetVirtualSize()
		{
			return this.GetFileLength();
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00015B78 File Offset: 0x00013D78
		public void WriteTo(BinaryWriter writer)
		{
			uint length = this.GetFileLength() - 4U;
			writer.Write((uint)this.bodies.Count);
			foreach (KeyValuePair<uint, JITMethodBody> entry2 in from entry in this.bodies
			orderby entry.Key
			select entry)
			{
				writer.Write(entry2.Key);
				Debug.Assert(entry2.Value != null);
				Debug.Assert((length + entry2.Value.Offset) % 4U == 0U);
				writer.Write(length + entry2.Value.Offset >> 2);
			}
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00005630 File Offset: 0x00003830
		public void Add(uint token, JITMethodBody body)
		{
			Debug.Assert(this.bodies.ContainsKey(token));
			this.bodies[token] = body;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00015C50 File Offset: 0x00013E50
		public void PopulateSection(PESection section)
		{
			uint offset = 0U;
			foreach (KeyValuePair<uint, JITMethodBody> entry2 in from entry in this.bodies
			orderby entry.Key
			select entry)
			{
				Debug.Assert(entry2.Value != null);
				section.Add(entry2.Value, 4U);
				entry2.Value.Offset = offset;
				Debug.Assert(entry2.Value.GetFileLength() % 4U == 0U);
				offset += entry2.Value.GetFileLength();
			}
		}

		// Token: 0x040001D5 RID: 469
		private readonly Dictionary<uint, JITMethodBody> bodies;

		// Token: 0x040001D6 RID: 470
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private FileOffset <FileOffset>k__BackingField;

		// Token: 0x040001D7 RID: 471
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private RVA <RVA>k__BackingField;

		// Token: 0x020000AE RID: 174
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060002A5 RID: 677 RVA: 0x00005653 File Offset: 0x00003853
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060002A6 RID: 678 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x060002A7 RID: 679 RVA: 0x0000565F File Offset: 0x0000385F
			internal uint <.ctor>b__1_0(uint token)
			{
				return token;
			}

			// Token: 0x060002A8 RID: 680 RVA: 0x00005662 File Offset: 0x00003862
			internal JITMethodBody <.ctor>b__1_1(uint token)
			{
				return null;
			}

			// Token: 0x060002A9 RID: 681 RVA: 0x00005665 File Offset: 0x00003865
			internal uint <WriteTo>b__13_0(KeyValuePair<uint, JITMethodBody> entry)
			{
				return entry.Key;
			}

			// Token: 0x060002AA RID: 682 RVA: 0x00005665 File Offset: 0x00003865
			internal uint <PopulateSection>b__15_0(KeyValuePair<uint, JITMethodBody> entry)
			{
				return entry.Key;
			}

			// Token: 0x040001D8 RID: 472
			public static readonly JITBodyIndex.<>c <>9 = new JITBodyIndex.<>c();

			// Token: 0x040001D9 RID: 473
			public static Func<uint, uint> <>9__1_0;

			// Token: 0x040001DA RID: 474
			public static Func<uint, JITMethodBody> <>9__1_1;

			// Token: 0x040001DB RID: 475
			public static Func<KeyValuePair<uint, JITMethodBody>, uint> <>9__13_0;

			// Token: 0x040001DC RID: 476
			public static Func<KeyValuePair<uint, JITMethodBody>, uint> <>9__15_0;
		}
	}
}
