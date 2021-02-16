using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000AA RID: 170
	internal class JITMethodBody : IChunk
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000285 RID: 645 RVA: 0x00005551 File Offset: 0x00003751
		// (set) Token: 0x06000286 RID: 646 RVA: 0x00005559 File Offset: 0x00003759
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

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000287 RID: 647 RVA: 0x00005562 File Offset: 0x00003762
		// (set) Token: 0x06000288 RID: 648 RVA: 0x0000556A File Offset: 0x0000376A
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

		// Token: 0x06000289 RID: 649 RVA: 0x00005573 File Offset: 0x00003773
		public void SetOffset(FileOffset offset, RVA rva)
		{
			this.FileOffset = offset;
			this.RVA = rva;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x000154C0 File Offset: 0x000136C0
		public uint GetFileLength()
		{
			return (uint)(this.Body.Length + 4);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x000154DC File Offset: 0x000136DC
		public uint GetVirtualSize()
		{
			return this.GetFileLength();
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00005586 File Offset: 0x00003786
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write((uint)(this.Body.Length >> 2));
			writer.Write(this.Body);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000154F4 File Offset: 0x000136F4
		public void Serialize(uint token, uint key, byte[] fieldLayout)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryWriter writer = new BinaryWriter(ms);
				for (int j = 0; j < fieldLayout.Length; j++)
				{
					switch (fieldLayout[j])
					{
					case 0:
						writer.Write((uint)this.ILCode.Length);
						break;
					case 1:
						writer.Write(this.MaxStack);
						break;
					case 2:
						writer.Write((uint)this.EHs.Length);
						break;
					case 3:
						writer.Write((uint)this.LocalVars.Length);
						break;
					case 4:
						writer.Write(this.Options);
						break;
					case 5:
						writer.Write(this.MulSeed);
						break;
					}
				}
				writer.Write(this.ILCode);
				writer.Write(this.LocalVars);
				foreach (JITEHClause clause in this.EHs)
				{
					writer.Write(clause.Flags);
					writer.Write(clause.TryOffset);
					writer.Write(clause.TryLength);
					writer.Write(clause.HandlerOffset);
					writer.Write(clause.HandlerLength);
					writer.Write(clause.ClassTokenOrFilterOffset);
				}
				writer.WriteZeros(4 - ((int)ms.Length & 3));
				this.Body = ms.ToArray();
			}
			Debug.Assert(this.Body.Length % 4 == 0);
			uint state = token * key;
			uint counter = state;
			uint i = 0U;
			while ((ulong)i < (ulong)((long)this.Body.Length))
			{
				uint data = (uint)((int)this.Body[(int)i] | (int)this.Body[(int)(i + 1U)] << 8 | (int)this.Body[(int)(i + 2U)] << 16 | (int)this.Body[(int)(i + 3U)] << 24);
				byte[] body = this.Body;
				uint num = i;
				body[(int)num] = (body[(int)num] ^ (byte)state);
				byte[] body2 = this.Body;
				uint num2 = i + 1U;
				body2[(int)num2] = (body2[(int)num2] ^ (byte)(state >> 8));
				byte[] body3 = this.Body;
				uint num3 = i + 2U;
				body3[(int)num3] = (body3[(int)num3] ^ (byte)(state >> 16));
				byte[] body4 = this.Body;
				uint num4 = i + 3U;
				body4[(int)num4] = (body4[(int)num4] ^ (byte)(state >> 24));
				state += (data ^ counter);
				counter ^= (state >> 5 | state << 27);
				i += 4U;
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00004A68 File Offset: 0x00002C68
		public JITMethodBody()
		{
		}

		// Token: 0x040001C5 RID: 453
		public byte[] Body;

		// Token: 0x040001C6 RID: 454
		public JITEHClause[] EHs;

		// Token: 0x040001C7 RID: 455
		public byte[] ILCode;

		// Token: 0x040001C8 RID: 456
		public byte[] LocalVars;

		// Token: 0x040001C9 RID: 457
		public uint MaxStack;

		// Token: 0x040001CA RID: 458
		public uint MulSeed;

		// Token: 0x040001CB RID: 459
		public uint Offset;

		// Token: 0x040001CC RID: 460
		public uint Options;

		// Token: 0x040001CD RID: 461
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private FileOffset <FileOffset>k__BackingField;

		// Token: 0x040001CE RID: 462
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private RVA <RVA>k__BackingField;
	}
}
