using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Resources
{
	// Token: 0x02000044 RID: 68
	internal class MDPhase
	{
		// Token: 0x06000148 RID: 328 RVA: 0x00004FE2 File Offset: 0x000031E2
		public MDPhase(REContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00004FF3 File Offset: 0x000031F3
		public void Hook()
		{
			this.ctx.Context.CurrentModuleWriterListener.OnWriterEvent += this.OnWriterEvent;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000B410 File Offset: 0x00009610
		private void OnWriterEvent(object sender, ModuleWriterListenerEventArgs e)
		{
			ModuleWriterBase writer = (ModuleWriterBase)sender;
			bool flag = e.WriterEvent == ModuleWriterEvent.MDBeginAddResources;
			if (flag)
			{
				this.ctx.Context.CheckCancellation();
				this.ctx.Context.Logger.Debug("Encrypting resources...");
				bool hasPacker = this.ctx.Context.Packer != null;
				List<EmbeddedResource> resources = this.ctx.Module.Resources.OfType<EmbeddedResource>().ToList<EmbeddedResource>();
				bool flag2 = !hasPacker;
				if (flag2)
				{
					this.ctx.Module.Resources.RemoveWhere((Resource res) => res is EmbeddedResource);
				}
				string asmName = this.ctx.Name.RandomName(RenameMode.Letters);
				PublicKey pubKey = null;
				bool flag3 = writer.TheOptions.StrongNameKey != null;
				if (flag3)
				{
					pubKey = PublicKeyBase.CreatePublicKey(writer.TheOptions.StrongNameKey.PublicKey);
				}
				AssemblyDefUser assembly = new AssemblyDefUser(asmName, new Version(0, 0), pubKey);
				assembly.Modules.Add(new ModuleDefUser(asmName + ".dll"));
				ModuleDef module = assembly.ManifestModule;
				assembly.ManifestModule.Kind = ModuleKind.Dll;
				AssemblyRefUser asmRef = new AssemblyRefUser(module.Assembly);
				bool flag4 = !hasPacker;
				if (flag4)
				{
					foreach (EmbeddedResource res2 in resources)
					{
						res2.Attributes = ManifestResourceAttributes.Public;
						module.Resources.Add(res2);
						this.ctx.Module.Resources.Add(new AssemblyLinkedResource(res2.Name, asmRef, res2.Attributes));
					}
				}
				byte[] moduleBuff;
				using (MemoryStream ms = new MemoryStream())
				{
					module.Write(ms, new ModuleWriterOptions
					{
						StrongNameKey = writer.TheOptions.StrongNameKey
					});
					moduleBuff = ms.ToArray();
				}
				moduleBuff = this.ctx.Context.Registry.GetService<ICompressionService>().Compress(moduleBuff, delegate(double progress)
				{
					this.ctx.Context.Logger.Progress((int)(progress * 10000.0), 10000);
				});
				this.ctx.Context.Logger.EndProgress();
				this.ctx.Context.CheckCancellation();
				uint compressedLen = (uint)((moduleBuff.Length + 3) / 4);
				compressedLen = (compressedLen + 15U & 4294967280U);
				uint[] compressedBuff = new uint[compressedLen];
				Buffer.BlockCopy(moduleBuff, 0, compressedBuff, 0, moduleBuff.Length);
				uint keySeed = this.ctx.Random.NextUInt32() | 16U;
				uint[] key = new uint[16];
				uint state = keySeed;
				for (int i = 0; i < 16; i++)
				{
					state ^= state >> 13;
					state ^= state << 25;
					state ^= state >> 27;
					key[i] = state;
				}
				byte[] encryptedBuffer = new byte[compressedBuff.Length * 4];
				for (int buffIndex = 0; buffIndex < compressedBuff.Length; buffIndex += 16)
				{
					uint[] enc = this.ctx.ModeHandler.Encrypt(compressedBuff, buffIndex, key);
					for (int j = 0; j < 16; j++)
					{
						key[j] ^= compressedBuff[buffIndex + j];
					}
					Buffer.BlockCopy(enc, 0, encryptedBuffer, buffIndex * 4, 64);
				}
				uint size = (uint)encryptedBuffer.Length;
				TablesHeap tblHeap = writer.MetaData.TablesHeap;
				tblHeap.ClassLayoutTable[writer.MetaData.GetClassLayoutRid(this.ctx.DataType)].ClassSize = size;
				RawFieldRow expr_39F = tblHeap.FieldTable[writer.MetaData.GetRid(this.ctx.DataField)];
				RawFieldRow rawFieldRow = expr_39F;
				rawFieldRow.Flags |= 256;
				this.encryptedResource = writer.Constants.Add(new ByteArrayChunk(encryptedBuffer), 8U);
				MutationHelper.InjectKeys(this.ctx.InitMethod, new int[]
				{
					0,
					1
				}, new int[]
				{
					(int)(size / 4U),
					(int)keySeed
				});
			}
			else
			{
				bool flag5 = e.WriterEvent == ModuleWriterEvent.EndCalculateRvasAndFileOffsets;
				if (flag5)
				{
					TablesHeap tblHeap2 = writer.MetaData.TablesHeap;
					tblHeap2.FieldRVATable[writer.MetaData.GetFieldRVARid(this.ctx.DataField)].RVA = (uint)this.encryptedResource.RVA;
				}
			}
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00005018 File Offset: 0x00003218
		[CompilerGenerated]
		private void <OnWriterEvent>b__2_1(double progress)
		{
			this.ctx.Context.Logger.Progress((int)(progress * 10000.0), 10000);
		}

		// Token: 0x04000076 RID: 118
		private readonly REContext ctx;

		// Token: 0x04000077 RID: 119
		private ByteArrayChunk encryptedResource;

		// Token: 0x02000045 RID: 69
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600014C RID: 332 RVA: 0x00005042 File Offset: 0x00003242
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600014D RID: 333 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x0600014E RID: 334 RVA: 0x0000504E File Offset: 0x0000324E
			internal bool <OnWriterEvent>b__2_0(Resource res)
			{
				return res is EmbeddedResource;
			}

			// Token: 0x04000078 RID: 120
			public static readonly MDPhase.<>c <>9 = new MDPhase.<>c();

			// Token: 0x04000079 RID: 121
			public static Predicate<Resource> <>9__2_0;
		}
	}
}
