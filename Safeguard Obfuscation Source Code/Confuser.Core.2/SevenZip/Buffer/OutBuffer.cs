using System;
using System.IO;

namespace SevenZip.Buffer
{
	// Token: 0x02000024 RID: 36
	internal class OutBuffer
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x000024E9 File Offset: 0x000006E9
		public OutBuffer(uint bufferSize)
		{
			this.m_Buffer = new byte[bufferSize];
			this.m_BufferSize = bufferSize;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00002506 File Offset: 0x00000706
		public void SetStream(Stream stream)
		{
			this.m_Stream = stream;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00002510 File Offset: 0x00000710
		public void FlushStream()
		{
			this.m_Stream.Flush();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000251F File Offset: 0x0000071F
		public void CloseStream()
		{
			this.m_Stream.Close();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000252E File Offset: 0x0000072E
		public void ReleaseStream()
		{
			this.m_Stream = null;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00002538 File Offset: 0x00000738
		public void Init()
		{
			this.m_ProcessedSize = 0UL;
			this.m_Pos = 0U;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000945C File Offset: 0x0000765C
		public void WriteByte(byte b)
		{
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			buffer[(int)pos] = b;
			bool flag = this.m_Pos >= this.m_BufferSize;
			if (flag)
			{
				this.FlushData();
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000094A0 File Offset: 0x000076A0
		public void FlushData()
		{
			bool flag = this.m_Pos == 0U;
			if (!flag)
			{
				this.m_Stream.Write(this.m_Buffer, 0, (int)this.m_Pos);
				this.m_Pos = 0U;
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000094E0 File Offset: 0x000076E0
		public ulong GetProcessedSize()
		{
			return this.m_ProcessedSize + (ulong)this.m_Pos;
		}

		// Token: 0x040000E7 RID: 231
		private readonly byte[] m_Buffer;

		// Token: 0x040000E8 RID: 232
		private readonly uint m_BufferSize;

		// Token: 0x040000E9 RID: 233
		private uint m_Pos;

		// Token: 0x040000EA RID: 234
		private ulong m_ProcessedSize;

		// Token: 0x040000EB RID: 235
		private Stream m_Stream;
	}
}
