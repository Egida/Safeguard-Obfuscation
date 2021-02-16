using System;
using System.IO;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;

namespace Confuser.Protections
{
	// Token: 0x02000036 RID: 54
	internal class MyHeap : IHeap, IChunk
	{
		// Token: 0x06000101 RID: 257 RVA: 0x00004ED6 File Offset: 0x000030D6
		public MyHeap(string name)
		{
			this.name = name;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000A72C File Offset: 0x0000892C
		public uint GetFileLength()
		{
			return (uint)this.heapData.Length;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000A748 File Offset: 0x00008948
		public uint GetVirtualSize()
		{
			return this.GetFileLength();
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00004EF4 File Offset: 0x000030F4
		public void SetOffset(FileOffset offset, RVA rva)
		{
			this.offset = offset;
			this.rva = rva;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00004A34 File Offset: 0x00002C34
		public void SetReadOnly()
		{
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00004F05 File Offset: 0x00003105
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(this.heapData);
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000107 RID: 263 RVA: 0x0000A760 File Offset: 0x00008960
		public FileOffset FileOffset
		{
			get
			{
				return this.offset;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000108 RID: 264 RVA: 0x0000A778 File Offset: 0x00008978
		public bool IsEmpty
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000109 RID: 265 RVA: 0x0000A78C File Offset: 0x0000898C
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600010A RID: 266 RVA: 0x0000A7A4 File Offset: 0x000089A4
		public RVA RVA
		{
			get
			{
				return this.rva;
			}
		}

		// Token: 0x0400005C RID: 92
		private byte[] heapData = new byte[10];

		// Token: 0x0400005D RID: 93
		private string name;

		// Token: 0x0400005E RID: 94
		private FileOffset offset;

		// Token: 0x0400005F RID: 95
		private RVA rva;
	}
}
