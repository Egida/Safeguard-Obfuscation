using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x02000015 RID: 21
	internal class OutWindow
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00005398 File Offset: 0x00003598
		public void Create(uint windowSize)
		{
			bool flag = this._windowSize != windowSize;
			if (flag)
			{
				this._buffer = new byte[windowSize];
			}
			this._windowSize = windowSize;
			this._pos = 0U;
			this._streamPos = 0U;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000053DC File Offset: 0x000035DC
		public void Init(Stream stream, bool solid)
		{
			this.ReleaseStream();
			this._stream = stream;
			bool flag = !solid;
			if (flag)
			{
				this._streamPos = 0U;
				this._pos = 0U;
				this.TrainSize = 0U;
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005418 File Offset: 0x00003618
		public bool Train(Stream stream)
		{
			long len = stream.Length;
			uint size = (len < (long)((ulong)this._windowSize)) ? ((uint)len) : this._windowSize;
			this.TrainSize = size;
			stream.Position = len - (long)((ulong)size);
			this._streamPos = (this._pos = 0U);
			while (size > 0U)
			{
				uint curSize = this._windowSize - this._pos;
				bool flag = size < curSize;
				if (flag)
				{
					curSize = size;
				}
				int numReadBytes = stream.Read(this._buffer, (int)this._pos, (int)curSize);
				bool flag2 = numReadBytes == 0;
				if (flag2)
				{
					return false;
				}
				size -= (uint)numReadBytes;
				this._pos += (uint)numReadBytes;
				this._streamPos += (uint)numReadBytes;
				bool flag3 = this._pos == this._windowSize;
				if (flag3)
				{
					this._streamPos = (this._pos = 0U);
				}
			}
			return true;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00002310 File Offset: 0x00000510
		public void ReleaseStream()
		{
			this.Flush();
			this._stream = null;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00005508 File Offset: 0x00003708
		public void Flush()
		{
			uint size = this._pos - this._streamPos;
			bool flag = size == 0U;
			if (!flag)
			{
				this._stream.Write(this._buffer, (int)this._streamPos, (int)size);
				bool flag2 = this._pos >= this._windowSize;
				if (flag2)
				{
					this._pos = 0U;
				}
				this._streamPos = this._pos;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005570 File Offset: 0x00003770
		public void CopyBlock(uint distance, uint len)
		{
			uint pos = this._pos - distance - 1U;
			bool flag = pos >= this._windowSize;
			if (flag)
			{
				pos += this._windowSize;
			}
			while (len > 0U)
			{
				bool flag2 = pos >= this._windowSize;
				if (flag2)
				{
					pos = 0U;
				}
				byte[] buffer = this._buffer;
				uint pos2 = this._pos;
				this._pos = pos2 + 1U;
				buffer[(int)pos2] = this._buffer[(int)pos++];
				bool flag3 = this._pos >= this._windowSize;
				if (flag3)
				{
					this.Flush();
				}
				len -= 1U;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000560C File Offset: 0x0000380C
		public void PutByte(byte b)
		{
			byte[] buffer = this._buffer;
			uint pos = this._pos;
			this._pos = pos + 1U;
			buffer[(int)pos] = b;
			bool flag = this._pos >= this._windowSize;
			if (flag)
			{
				this.Flush();
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005650 File Offset: 0x00003850
		public byte GetByte(uint distance)
		{
			uint pos = this._pos - distance - 1U;
			bool flag = pos >= this._windowSize;
			if (flag)
			{
				pos += this._windowSize;
			}
			return this._buffer[(int)pos];
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002321 File Offset: 0x00000521
		public OutWindow()
		{
		}

		// Token: 0x0400004B RID: 75
		public uint TrainSize = 0U;

		// Token: 0x0400004C RID: 76
		private byte[] _buffer;

		// Token: 0x0400004D RID: 77
		private uint _pos;

		// Token: 0x0400004E RID: 78
		private Stream _stream;

		// Token: 0x0400004F RID: 79
		private uint _streamPos;

		// Token: 0x04000050 RID: 80
		private uint _windowSize;
	}
}
