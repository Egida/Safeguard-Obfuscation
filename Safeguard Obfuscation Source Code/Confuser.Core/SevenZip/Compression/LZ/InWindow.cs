using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x02000014 RID: 20
	internal class InWindow
	{
		// Token: 0x06000055 RID: 85 RVA: 0x0000507C File Offset: 0x0000327C
		public void MoveBlock()
		{
			uint offset = this._bufferOffset + this._pos - this._keepSizeBefore;
			bool flag = offset > 0U;
			if (flag)
			{
				offset -= 1U;
			}
			uint numBytes = this._bufferOffset + this._streamPos - offset;
			for (uint i = 0U; i < numBytes; i += 1U)
			{
				this._bufferBase[(int)i] = this._bufferBase[(int)(offset + i)];
			}
			this._bufferOffset -= offset;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000050F0 File Offset: 0x000032F0
		public virtual void ReadBlock()
		{
			bool streamEndWasReached = this._streamEndWasReached;
			if (!streamEndWasReached)
			{
				for (;;)
				{
					int size = (int)(0U - this._bufferOffset + this._blockSize - this._streamPos);
					bool flag = size == 0;
					if (flag)
					{
						break;
					}
					int numReadBytes = this._stream.Read(this._bufferBase, (int)(this._bufferOffset + this._streamPos), size);
					bool flag2 = numReadBytes == 0;
					if (flag2)
					{
						goto Block_3;
					}
					this._streamPos += (uint)numReadBytes;
					bool flag3 = this._streamPos >= this._pos + this._keepSizeAfter;
					if (flag3)
					{
						this._posLimit = this._streamPos - this._keepSizeAfter;
					}
				}
				return;
				Block_3:
				this._posLimit = this._streamPos;
				uint pointerToPostion = this._bufferOffset + this._posLimit;
				bool flag4 = pointerToPostion > this._pointerToLastSafePosition;
				if (flag4)
				{
					this._posLimit = this._pointerToLastSafePosition - this._bufferOffset;
				}
				this._streamEndWasReached = true;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002281 File Offset: 0x00000481
		private void Free()
		{
			this._bufferBase = null;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000051F4 File Offset: 0x000033F4
		public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
		{
			this._keepSizeBefore = keepSizeBefore;
			this._keepSizeAfter = keepSizeAfter;
			uint blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
			bool flag = this._bufferBase == null || this._blockSize != blockSize;
			if (flag)
			{
				this.Free();
				this._blockSize = blockSize;
				this._bufferBase = new byte[this._blockSize];
			}
			this._pointerToLastSafePosition = this._blockSize - keepSizeAfter;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000228B File Offset: 0x0000048B
		public void SetStream(Stream stream)
		{
			this._stream = stream;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002295 File Offset: 0x00000495
		public void ReleaseStream()
		{
			this._stream = null;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000229F File Offset: 0x0000049F
		public void Init()
		{
			this._bufferOffset = 0U;
			this._pos = 0U;
			this._streamPos = 0U;
			this._streamEndWasReached = false;
			this.ReadBlock();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00005260 File Offset: 0x00003460
		public void MovePos()
		{
			this._pos += 1U;
			bool flag = this._pos > this._posLimit;
			if (flag)
			{
				uint pointerToPostion = this._bufferOffset + this._pos;
				bool flag2 = pointerToPostion > this._pointerToLastSafePosition;
				if (flag2)
				{
					this.MoveBlock();
				}
				this.ReadBlock();
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000052BC File Offset: 0x000034BC
		public byte GetIndexByte(int index)
		{
			return this._bufferBase[(int)(checked((IntPtr)(unchecked((ulong)(this._bufferOffset + this._pos) + (ulong)((long)index)))))];
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000052E8 File Offset: 0x000034E8
		public uint GetMatchLen(int index, uint distance, uint limit)
		{
			bool streamEndWasReached = this._streamEndWasReached;
			if (streamEndWasReached)
			{
				bool flag = (ulong)this._pos + (ulong)((long)index) + (ulong)limit > (ulong)this._streamPos;
				if (flag)
				{
					limit = this._streamPos - (uint)((ulong)this._pos + (ulong)((long)index));
				}
			}
			distance += 1U;
			uint pby = this._bufferOffset + this._pos + (uint)index;
			uint i = 0U;
			while (i < limit && this._bufferBase[(int)(pby + i)] == this._bufferBase[(int)(pby + i - distance)])
			{
				i += 1U;
			}
			return i;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00005378 File Offset: 0x00003578
		public uint GetNumAvailableBytes()
		{
			return this._streamPos - this._pos;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000022C5 File Offset: 0x000004C5
		public void ReduceOffsets(int subValue)
		{
			this._bufferOffset += (uint)subValue;
			this._posLimit -= (uint)subValue;
			this._pos -= (uint)subValue;
			this._streamPos -= (uint)subValue;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002300 File Offset: 0x00000500
		public InWindow()
		{
		}

		// Token: 0x04000040 RID: 64
		public uint _blockSize;

		// Token: 0x04000041 RID: 65
		public byte[] _bufferBase = null;

		// Token: 0x04000042 RID: 66
		public uint _bufferOffset;

		// Token: 0x04000043 RID: 67
		private uint _keepSizeAfter;

		// Token: 0x04000044 RID: 68
		private uint _keepSizeBefore;

		// Token: 0x04000045 RID: 69
		private uint _pointerToLastSafePosition;

		// Token: 0x04000046 RID: 70
		public uint _pos;

		// Token: 0x04000047 RID: 71
		private uint _posLimit;

		// Token: 0x04000048 RID: 72
		private Stream _stream;

		// Token: 0x04000049 RID: 73
		private bool _streamEndWasReached;

		// Token: 0x0400004A RID: 74
		public uint _streamPos;
	}
}
