using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000B4 RID: 180
	internal class NormalMode : IModeHandler
	{
		// Token: 0x060002C7 RID: 711 RVA: 0x00016FCC File Offset: 0x000151CC
		public void HandleInject(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters)
		{
			this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator(parent.FullId);
			this.kk = this.random.NextUInt32();
			this.bb = this.random.NextUInt32();
			this.aa = this.random.NextUInt32();
			this.pp = this.random.NextUInt32();
			this.name1 = (this.random.NextUInt32() & 2139062143U);
			this.name2 = (this.random.NextUInt32() & 2139062143U);
			Mode parameter = parameters.GetParameter<Mode>(context, context.CurrentModule, "key", Mode.Dynamic);
			if (parameter != Mode.Normal)
			{
				if (parameter != Mode.Dynamic)
				{
					throw new UnreachableException();
				}
				this.deriver = new DynamicDeriver();
			}
			else
			{
				this.deriver = new NormalDeriver();
			}
			this.deriver.Init(context, this.random);
			IRuntimeService rt = context.Registry.GetService<IRuntimeService>();
			TypeDef initType = rt.GetRuntimeType("Confuser.Runtime.AntiTamperNormal");
			IEnumerable<IDnlibDef> members = InjectHelper.Inject(initType, context.CurrentModule.GlobalType, context.CurrentModule);
			MethodDef initMethod = (MethodDef)members.Single((IDnlibDef m) => m.Name == "Initialize");
			initMethod.Body.SimplifyMacros(initMethod.Parameters);
			List<Instruction> instrs = initMethod.Body.Instructions.ToList<Instruction>();
			for (int i = 0; i < instrs.Count; i++)
			{
				Instruction instr = instrs[i];
				bool flag = instr.OpCode == OpCodes.Ldtoken;
				if (flag)
				{
					instr.Operand = context.CurrentModule.GlobalType;
				}
				else
				{
					bool flag2 = instr.OpCode == OpCodes.Call;
					if (flag2)
					{
						IMethod method = (IMethod)instr.Operand;
						bool flag3 = method.DeclaringType.Name == "Mutation" && method.Name == "Crypt";
						if (flag3)
						{
							Instruction ldDst = instrs[i - 2];
							Instruction ldSrc = instrs[i - 1];
							Debug.Assert(ldDst.OpCode == OpCodes.Ldloc && ldSrc.OpCode == OpCodes.Ldloc);
							instrs.RemoveAt(i);
							instrs.RemoveAt(i - 1);
							instrs.RemoveAt(i - 2);
							instrs.InsertRange(i - 2, this.deriver.EmitDerivation(initMethod, context, (Local)ldDst.Operand, (Local)ldSrc.Operand));
						}
					}
				}
			}
			initMethod.Body.Instructions.Clear();
			foreach (Instruction instr2 in instrs)
			{
				initMethod.Body.Instructions.Add(instr2);
			}
			MutationHelper.InjectKeys(initMethod, new int[]
			{
				0,
				1,
				2,
				3,
				4
			}, new int[]
			{
				(int)(this.name1 * this.name2),
				(int)this.kk,
				(int)this.bb,
				(int)this.aa,
				(int)this.pp
			});
			INameService name = context.Registry.GetService<INameService>();
			INameService name2 = context.Registry.GetService<INameService>();
			IMarkerService marker = context.Registry.GetService<IMarkerService>();
			foreach (IDnlibDef member2 in members)
			{
				marker.Mark(member2, parent);
				name.Analyze(member2);
				bool flag4 = member2 is MethodDef;
				if (flag4)
				{
					MethodDef method2 = (MethodDef)member2;
				}
				foreach (IDnlibDef def in members)
				{
					name.MarkHelper(def, marker, parent);
					bool flag5 = def is MethodDef;
					if (flag5)
					{
						parent.ExcludeMethod(context, (MethodDef)def);
					}
				}
				MethodDef cctor = context.CurrentModule.GlobalType.FindStaticConstructor();
				cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, initMethod));
				parent.ExcludeMethod(context, cctor);
				member2.Name = name.ObfuscateName(member2.Name, RenameMode.Sequential);
				name.SetCanRename(member2, true);
			}
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x00005733 File Offset: 0x00003933
		public void HandleMD(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters)
		{
			this.methods = parameters.Targets.OfType<MethodDef>().ToList<MethodDef>();
			context.CurrentModuleWriterListener.OnWriterEvent += this.OnWriterEvent;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x000174C8 File Offset: 0x000156C8
		private void OnWriterEvent(object sender, ModuleWriterListenerEventArgs e)
		{
			ModuleWriterBase writer = (ModuleWriterBase)sender;
			bool flag = e.WriterEvent == ModuleWriterEvent.MDEndCreateTables;
			if (flag)
			{
				this.CreateSections(writer);
			}
			else
			{
				bool flag2 = e.WriterEvent == ModuleWriterEvent.BeginStrongNameSign;
				if (flag2)
				{
					this.EncryptSection(writer);
				}
			}
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00017510 File Offset: 0x00015710
		private void CreateSections(ModuleWriterBase writer)
		{
			byte[] nameBuffer = new byte[]
			{
				(byte)this.name1,
				(byte)(this.name1 >> 8),
				(byte)(this.name1 >> 16),
				(byte)(this.name1 >> 24),
				(byte)this.name2,
				(byte)(this.name2 >> 8),
				(byte)(this.name2 >> 16),
				(byte)(this.name2 >> 24)
			};
			PESection newSection = new PESection(Encoding.ASCII.GetString(nameBuffer), 3758096448U);
			writer.Sections.Insert(0, newSection);
			uint alignment = writer.TextSection.Remove(writer.MetaData).Value;
			writer.TextSection.Add(writer.MetaData, alignment);
			alignment = writer.TextSection.Remove(writer.NetResources).Value;
			writer.TextSection.Add(writer.NetResources, alignment);
			alignment = writer.TextSection.Remove(writer.Constants).Value;
			newSection.Add(writer.Constants, alignment);
			PESection peSection = new PESection("", 1610612768U);
			bool moved = false;
			bool flag = writer.StrongNameSignature != null;
			if (flag)
			{
				alignment = writer.TextSection.Remove(writer.StrongNameSignature).Value;
				peSection.Add(writer.StrongNameSignature, alignment);
				moved = true;
			}
			ModuleWriter managedWriter = writer as ModuleWriter;
			bool flag2 = managedWriter != null;
			if (flag2)
			{
				bool flag3 = managedWriter.ImportAddressTable != null;
				if (flag3)
				{
					alignment = writer.TextSection.Remove(managedWriter.ImportAddressTable).Value;
					peSection.Add(managedWriter.ImportAddressTable, alignment);
					moved = true;
				}
				bool flag4 = managedWriter.StartupStub != null;
				if (flag4)
				{
					alignment = writer.TextSection.Remove(managedWriter.StartupStub).Value;
					peSection.Add(managedWriter.StartupStub, alignment);
					moved = true;
				}
			}
			bool flag5 = moved;
			if (flag5)
			{
				writer.Sections.Add(peSection);
			}
			MethodBodyChunks encryptedChunk = new MethodBodyChunks(writer.TheOptions.ShareMethodBodies);
			newSection.Add(encryptedChunk, 4U);
			foreach (MethodDef method in this.methods)
			{
				bool flag6 = !method.HasBody;
				if (!flag6)
				{
					dnlib.DotNet.Writer.MethodBody body = writer.MetaData.GetMethodBody(method);
					bool ok = writer.MethodBodies.Remove(body);
					encryptedChunk.Add(body);
				}
			}
			newSection.Add(new ByteArrayChunk(new byte[4]), 4U);
		}

		// Token: 0x060002CB RID: 715 RVA: 0x000177DC File Offset: 0x000159DC
		private void EncryptSection(ModuleWriterBase writer)
		{
			Stream stream = writer.DestinationStream;
			BinaryReader reader = new BinaryReader(writer.DestinationStream);
			stream.Position = 60L;
			stream.Position = (long)((ulong)reader.ReadUInt32());
			stream.Position += 6L;
			ushort sections = reader.ReadUInt16();
			stream.Position += 12L;
			ushort optSize = reader.ReadUInt16();
			stream.Position += (long)(2 + optSize);
			uint encLoc = 0U;
			uint encSize = 0U;
			int origSects = -1;
			bool flag = writer is NativeModuleWriter && writer.Module is ModuleDefMD;
			if (flag)
			{
				origSects = ((ModuleDefMD)writer.Module).MetaData.PEImage.ImageSectionHeaders.Count;
			}
			for (int i = 0; i < (int)sections; i++)
			{
				bool flag2 = origSects > 0;
				uint nameHash;
				if (flag2)
				{
					origSects--;
					stream.Write(new byte[8], 0, 8);
					nameHash = 0U;
				}
				else
				{
					nameHash = reader.ReadUInt32() * reader.ReadUInt32();
				}
				stream.Position += 8L;
				bool flag3 = nameHash == this.name1 * this.name2;
				if (flag3)
				{
					encSize = reader.ReadUInt32();
					encLoc = reader.ReadUInt32();
				}
				else
				{
					bool flag4 = nameHash > 0U;
					if (flag4)
					{
						uint sectSize = reader.ReadUInt32();
						uint sectLoc = reader.ReadUInt32();
						this.Hash(stream, reader, sectLoc, sectSize);
					}
					else
					{
						stream.Position += 8L;
					}
				}
				stream.Position += 16L;
			}
			uint[] key = this.DeriveKey();
			encSize >>= 2;
			stream.Position = (long)((ulong)encLoc);
			uint[] result = new uint[encSize];
			for (uint j = 0U; j < encSize; j += 1U)
			{
				uint data = reader.ReadUInt32();
				result[(int)j] = (data ^ key[(int)(j & 15U)]);
				key[(int)(j & 15U)] = (key[(int)(j & 15U)] ^ data) + 1035675673U;
			}
			byte[] byteResult = new byte[encSize << 2];
			Buffer.BlockCopy(result, 0, byteResult, 0, byteResult.Length);
			stream.Position = (long)((ulong)encLoc);
			stream.Write(byteResult, 0, byteResult.Length);
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00017A1C File Offset: 0x00015C1C
		private void Hash(Stream stream, BinaryReader reader, uint offset, uint size)
		{
			long original = stream.Position;
			stream.Position = (long)((ulong)offset);
			size >>= 2;
			for (uint i = 0U; i < size; i += 1U)
			{
				uint data = reader.ReadUInt32();
				uint tmp = (this.kk ^ data) + this.bb + this.aa * this.pp;
				this.kk = this.bb;
				this.bb = this.aa;
				this.bb = this.pp;
				this.pp = tmp;
			}
			stream.Position = original;
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00017AB0 File Offset: 0x00015CB0
		private uint[] DeriveKey()
		{
			uint[] dst = new uint[16];
			uint[] src = new uint[16];
			for (int i = 0; i < 16; i++)
			{
				dst[i] = this.pp;
				src[i] = this.bb;
				this.kk = (this.bb >> 5 | this.bb << 27);
				this.bb = (this.aa >> 3 | this.aa << 29);
				this.aa = (this.pp >> 7 | this.pp << 25);
				this.pp = (this.kk >> 11 | this.kk << 21);
			}
			return this.deriver.DeriveKey(dst, src);
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00004A68 File Offset: 0x00002C68
		public NormalMode()
		{
		}

		// Token: 0x040001FF RID: 511
		private uint aa;

		// Token: 0x04000200 RID: 512
		private IKeyDeriver deriver;

		// Token: 0x04000201 RID: 513
		private List<MethodDef> methods;

		// Token: 0x04000202 RID: 514
		private uint name1;

		// Token: 0x04000203 RID: 515
		private uint name2;

		// Token: 0x04000204 RID: 516
		private RandomGenerator random;

		// Token: 0x04000205 RID: 517
		private uint pp;

		// Token: 0x04000206 RID: 518
		private uint bb;

		// Token: 0x04000207 RID: 519
		private uint kk;

		// Token: 0x020000B5 RID: 181
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060002CF RID: 719 RVA: 0x00005764 File Offset: 0x00003964
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060002D0 RID: 720 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x060002D1 RID: 721 RVA: 0x00004A71 File Offset: 0x00002C71
			internal bool <HandleInject>b__9_0(IDnlibDef m)
			{
				return m.Name == "Initialize";
			}

			// Token: 0x04000208 RID: 520
			public static readonly NormalMode.<>c <>9 = new NormalMode.<>c();

			// Token: 0x04000209 RID: 521
			public static Func<IDnlibDef, bool> <>9__9_0;
		}
	}
}
