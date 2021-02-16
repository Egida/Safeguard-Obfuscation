using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000058 RID: 88
	internal class DefAttributeKeyTypeRecord : ElementStartRecord, IBamlDeferRecord
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000225 RID: 549 RVA: 0x00009E08 File Offset: 0x00008008
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DefAttributeKeyType;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000226 RID: 550 RVA: 0x00003134 File Offset: 0x00001334
		// (set) Token: 0x06000227 RID: 551 RVA: 0x0000313C File Offset: 0x0000133C
		public bool Shared
		{
			[CompilerGenerated]
			get
			{
				return this.<Shared>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Shared>k__BackingField = value;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000228 RID: 552 RVA: 0x00003145 File Offset: 0x00001345
		// (set) Token: 0x06000229 RID: 553 RVA: 0x0000314D File Offset: 0x0000134D
		public bool SharedSet
		{
			[CompilerGenerated]
			get
			{
				return this.<SharedSet>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<SharedSet>k__BackingField = value;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600022A RID: 554 RVA: 0x00003156 File Offset: 0x00001356
		// (set) Token: 0x0600022B RID: 555 RVA: 0x0000315E File Offset: 0x0000135E
		public BamlRecord Record
		{
			[CompilerGenerated]
			get
			{
				return this.<Record>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Record>k__BackingField = value;
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x00009E1C File Offset: 0x0000801C
		public void ReadDefer(BamlDocument doc, int index, Func<long, BamlRecord> resolve)
		{
			for (;;)
			{
				BamlRecordType type = doc[index].Type;
				bool keys;
				if (type <= BamlRecordType.KeyElementStart)
				{
					if (type - BamlRecordType.DefAttributeKeyString <= 1)
					{
						goto IL_34;
					}
					if (type != BamlRecordType.KeyElementStart)
					{
						goto IL_5A;
					}
					DefAttributeKeyTypeRecord.NavigateTree(doc, BamlRecordType.KeyElementStart, BamlRecordType.KeyElementEnd, ref index);
					keys = true;
				}
				else if (type != BamlRecordType.StaticResourceStart)
				{
					if (type != BamlRecordType.OptimizedStaticResource)
					{
						goto IL_5A;
					}
					goto IL_34;
				}
				else
				{
					DefAttributeKeyTypeRecord.NavigateTree(doc, BamlRecordType.StaticResourceStart, BamlRecordType.StaticResourceEnd, ref index);
					keys = true;
				}
				IL_63:
				index++;
				if (!keys)
				{
					break;
				}
				continue;
				IL_34:
				keys = true;
				goto IL_63;
				IL_5A:
				keys = false;
				index--;
				goto IL_63;
			}
			this.Record = resolve(doc[index].Position + (long)((ulong)this.pos));
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00009EB8 File Offset: 0x000080B8
		public void WriteDefer(BamlDocument doc, int index, BinaryWriter wtr)
		{
			for (;;)
			{
				BamlRecordType type = doc[index].Type;
				bool keys;
				if (type <= BamlRecordType.KeyElementStart)
				{
					if (type - BamlRecordType.DefAttributeKeyString <= 1)
					{
						goto IL_34;
					}
					if (type != BamlRecordType.KeyElementStart)
					{
						goto IL_5A;
					}
					DefAttributeKeyTypeRecord.NavigateTree(doc, BamlRecordType.KeyElementStart, BamlRecordType.KeyElementEnd, ref index);
					keys = true;
				}
				else if (type != BamlRecordType.StaticResourceStart)
				{
					if (type != BamlRecordType.OptimizedStaticResource)
					{
						goto IL_5A;
					}
					goto IL_34;
				}
				else
				{
					DefAttributeKeyTypeRecord.NavigateTree(doc, BamlRecordType.StaticResourceStart, BamlRecordType.StaticResourceEnd, ref index);
					keys = true;
				}
				IL_63:
				index++;
				if (!keys)
				{
					break;
				}
				continue;
				IL_34:
				keys = true;
				goto IL_63;
				IL_5A:
				keys = false;
				index--;
				goto IL_63;
			}
			wtr.BaseStream.Seek((long)((ulong)this.pos), SeekOrigin.Begin);
			wtr.Write((uint)(this.Record.Position - doc[index].Position));
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00003167 File Offset: 0x00001367
		public override void Read(BamlBinaryReader reader)
		{
			base.Read(reader);
			this.pos = reader.ReadUInt32();
			this.Shared = reader.ReadBoolean();
			this.SharedSet = reader.ReadBoolean();
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00003198 File Offset: 0x00001398
		public override void Write(BamlBinaryWriter writer)
		{
			base.Write(writer);
			this.pos = (uint)writer.BaseStream.Position;
			writer.Write(0U);
			writer.Write(this.Shared);
			writer.Write(this.SharedSet);
		}

		// Token: 0x06000230 RID: 560 RVA: 0x00009F68 File Offset: 0x00008168
		private static void NavigateTree(BamlDocument doc, BamlRecordType start, BamlRecordType end, ref int index)
		{
			index++;
			for (;;)
			{
				bool flag = doc[index].Type == start;
				if (flag)
				{
					DefAttributeKeyTypeRecord.NavigateTree(doc, start, end, ref index);
				}
				else
				{
					bool flag2 = doc[index].Type == end;
					if (flag2)
					{
						break;
					}
				}
				index++;
			}
		}

		// Token: 0x06000231 RID: 561 RVA: 0x000031D7 File Offset: 0x000013D7
		public DefAttributeKeyTypeRecord()
		{
		}

		// Token: 0x0400010F RID: 271
		internal uint pos = uint.MaxValue;

		// Token: 0x04000110 RID: 272
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool <Shared>k__BackingField;

		// Token: 0x04000111 RID: 273
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool <SharedSet>k__BackingField;

		// Token: 0x04000112 RID: 274
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private BamlRecord <Record>k__BackingField;
	}
}
