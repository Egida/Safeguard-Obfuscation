using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000042 RID: 66
	internal class DefAttributeKeyStringRecord : SizedBamlRecord, IBamlDeferRecord
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000184 RID: 388 RVA: 0x00009A5C File Offset: 0x00007C5C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DefAttributeKeyString;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00002A7C File Offset: 0x00000C7C
		// (set) Token: 0x06000186 RID: 390 RVA: 0x00002A84 File Offset: 0x00000C84
		public ushort ValueId
		{
			[CompilerGenerated]
			get
			{
				return this.<ValueId>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<ValueId>k__BackingField = value;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000187 RID: 391 RVA: 0x00002A8D File Offset: 0x00000C8D
		// (set) Token: 0x06000188 RID: 392 RVA: 0x00002A95 File Offset: 0x00000C95
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

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00002A9E File Offset: 0x00000C9E
		// (set) Token: 0x0600018A RID: 394 RVA: 0x00002AA6 File Offset: 0x00000CA6
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

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00002AAF File Offset: 0x00000CAF
		// (set) Token: 0x0600018C RID: 396 RVA: 0x00002AB7 File Offset: 0x00000CB7
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

		// Token: 0x0600018D RID: 397 RVA: 0x00009A70 File Offset: 0x00007C70
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
					DefAttributeKeyStringRecord.NavigateTree(doc, BamlRecordType.KeyElementStart, BamlRecordType.KeyElementEnd, ref index);
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
					DefAttributeKeyStringRecord.NavigateTree(doc, BamlRecordType.StaticResourceStart, BamlRecordType.StaticResourceEnd, ref index);
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

		// Token: 0x0600018E RID: 398 RVA: 0x00009B0C File Offset: 0x00007D0C
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
					DefAttributeKeyStringRecord.NavigateTree(doc, BamlRecordType.KeyElementStart, BamlRecordType.KeyElementEnd, ref index);
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
					DefAttributeKeyStringRecord.NavigateTree(doc, BamlRecordType.StaticResourceStart, BamlRecordType.StaticResourceEnd, ref index);
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

		// Token: 0x0600018F RID: 399 RVA: 0x00002AC0 File Offset: 0x00000CC0
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.ValueId = reader.ReadUInt16();
			this.pos = reader.ReadUInt32();
			this.Shared = reader.ReadBoolean();
			this.SharedSet = reader.ReadBoolean();
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00009BBC File Offset: 0x00007DBC
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.ValueId);
			this.pos = (uint)writer.BaseStream.Position;
			writer.Write(0U);
			writer.Write(this.Shared);
			writer.Write(this.SharedSet);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00009C0C File Offset: 0x00007E0C
		private static void NavigateTree(BamlDocument doc, BamlRecordType start, BamlRecordType end, ref int index)
		{
			index++;
			for (;;)
			{
				bool flag = doc[index].Type == start;
				if (flag)
				{
					DefAttributeKeyStringRecord.NavigateTree(doc, start, end, ref index);
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

		// Token: 0x06000192 RID: 402 RVA: 0x00002AF6 File Offset: 0x00000CF6
		public DefAttributeKeyStringRecord()
		{
		}

		// Token: 0x040000EA RID: 234
		internal uint pos = uint.MaxValue;

		// Token: 0x040000EB RID: 235
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort <ValueId>k__BackingField;

		// Token: 0x040000EC RID: 236
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool <Shared>k__BackingField;

		// Token: 0x040000ED RID: 237
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool <SharedSet>k__BackingField;

		// Token: 0x040000EE RID: 238
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BamlRecord <Record>k__BackingField;
	}
}
