using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using dnlib.IO;

namespace Confuser.Core
{
	// Token: 0x02000049 RID: 73
	public class ImageStream : Stream
	{
		// Token: 0x060001AB RID: 427 RVA: 0x00002BE9 File Offset: 0x00000DE9
		public ImageStream(IImageStream baseStream)
		{
			this.BaseStream = baseStream;
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060001AC RID: 428 RVA: 0x00002BFB File Offset: 0x00000DFB
		// (set) Token: 0x060001AD RID: 429 RVA: 0x00002C03 File Offset: 0x00000E03
		public IImageStream BaseStream
		{
			[CompilerGenerated]
			get
			{
				return this.<BaseStream>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<BaseStream>k__BackingField = value;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060001AE RID: 430 RVA: 0x0000DEB8 File Offset: 0x0000C0B8
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000DEB8 File Offset: 0x0000C0B8
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000DECC File Offset: 0x0000C0CC
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000DEE0 File Offset: 0x0000C0E0
		public override long Length
		{
			get
			{
				return this.BaseStream.Length;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x0000DF00 File Offset: 0x0000C100
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x00002C0C File Offset: 0x00000E0C
		public override long Position
		{
			get
			{
				return this.BaseStream.Position;
			}
			set
			{
				this.BaseStream.Position = value;
			}
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x000025D6 File Offset: 0x000007D6
		public override void Flush()
		{
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000DF20 File Offset: 0x0000C120
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.BaseStream.Read(buffer, offset, count);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000DF40 File Offset: 0x0000C140
		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.BaseStream.Position = offset;
				break;
			case SeekOrigin.Current:
				this.BaseStream.Position += offset;
				break;
			case SeekOrigin.End:
				this.BaseStream.Position = this.BaseStream.Length + offset;
				break;
			}
			return this.BaseStream.Position;
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00002C1C File Offset: 0x00000E1C
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00002C1C File Offset: 0x00000E1C
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		// Token: 0x04000181 RID: 385
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IImageStream <BaseStream>k__BackingField;
	}
}
