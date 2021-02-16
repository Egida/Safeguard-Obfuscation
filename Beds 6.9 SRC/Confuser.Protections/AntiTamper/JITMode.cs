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
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000AF RID: 175
	internal class JITMode : IModeHandler
	{
		// Token: 0x060002AB RID: 683 RVA: 0x00015D14 File Offset: 0x00013F14
		public void HandleInject(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters)
		{
			this.context = context;
			this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator(parent.FullId);
			this.z = this.random.NextUInt32();
			this.x = this.random.NextUInt32();
			this.c = this.random.NextUInt32();
			this.v = this.random.NextUInt32();
			this.name1 = (this.random.NextUInt32() & 2139062143U);
			this.name2 = (this.random.NextUInt32() & 2139062143U);
			this.key = this.random.NextUInt32();
			this.fieldLayout = new byte[6];
			for (int i = 0; i < 6; i++)
			{
				int index = this.random.NextInt32(0, 6);
				while (this.fieldLayout[index] > 0)
				{
					index = this.random.NextInt32(0, 6);
				}
				this.fieldLayout[index] = (byte)i;
			}
			Mode parameter = parameters.GetParameter<Mode>(context, context.CurrentModule, "key", Mode.Normal);
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
			TypeDef initType = rt.GetRuntimeType("Confuser.Runtime.AntiTamperJIT");
			IEnumerable<IDnlibDef> defs = InjectHelper.Inject(initType, context.CurrentModule.GlobalType, context.CurrentModule);
			this.initMethod = defs.OfType<MethodDef>().Single((MethodDef method) => method.Name == "Initialize");
			this.initMethod.Body.SimplifyMacros(this.initMethod.Parameters);
			List<Instruction> instrs = this.initMethod.Body.Instructions.ToList<Instruction>();
			for (int j = 0; j < instrs.Count; j++)
			{
				Instruction instr = instrs[j];
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
						IMethod method2 = (IMethod)instr.Operand;
						bool flag3 = method2.DeclaringType.Name == "Mutation" && method2.Name == "Crypt";
						if (flag3)
						{
							Instruction ldDst = instrs[j - 2];
							Instruction ldSrc = instrs[j - 1];
							Debug.Assert(ldDst.OpCode == OpCodes.Ldloc && ldSrc.OpCode == OpCodes.Ldloc);
							instrs.RemoveAt(j);
							instrs.RemoveAt(j - 1);
							instrs.RemoveAt(j - 2);
							instrs.InsertRange(j - 2, this.deriver.EmitDerivation(this.initMethod, context, (Local)ldDst.Operand, (Local)ldSrc.Operand));
						}
					}
				}
			}
			this.initMethod.Body.Instructions.Clear();
			foreach (Instruction instr2 in instrs)
			{
				this.initMethod.Body.Instructions.Add(instr2);
			}
			MutationHelper.InjectKeys(this.initMethod, new int[]
			{
				0,
				1,
				2,
				3,
				4
			}, new int[]
			{
				(int)(this.name1 * this.name2),
				(int)this.z,
				(int)this.x,
				(int)this.c,
				(int)this.v
			});
			INameService name = context.Registry.GetService<INameService>();
			IMarkerService marker = context.Registry.GetService<IMarkerService>();
			this.cctor = context.CurrentModule.GlobalType.FindStaticConstructor();
			this.cctorRepl = new MethodDefUser(name.RandomName(), MethodSig.CreateStatic(context.CurrentModule.CorLibTypes.Void));
			this.cctorRepl.IsStatic = true;
			this.cctorRepl.Access = MethodAttributes.PrivateScope;
			this.cctorRepl.Body = new CilBody();
			this.cctorRepl.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			context.CurrentModule.GlobalType.Methods.Add(this.cctorRepl);
			name.MarkHelper(this.cctorRepl, marker, parent);
			MutationHelper.InjectKeys(defs.OfType<MethodDef>().Single((MethodDef method) => method.Name == "HookHandler"), new int[1], new int[]
			{
				(int)this.key
			});
			foreach (IDnlibDef def in defs)
			{
				bool flag4 = def.Name == "MethodData";
				if (flag4)
				{
					TypeDef dataType = (TypeDef)def;
					FieldDef[] fields = dataType.Fields.ToArray<FieldDef>();
					byte[] layout = this.fieldLayout.Clone() as byte[];
					Array.Sort<byte, FieldDef>(layout, fields);
					for (byte k = 0; k < 6; k += 1)
					{
						layout[(int)k] = k;
					}
					Array.Sort<byte, byte>(this.fieldLayout, layout);
					this.fieldLayout = layout;
					dataType.Fields.Clear();
					foreach (FieldDef f in fields)
					{
						dataType.Fields.Add(f);
					}
				}
				name.MarkHelper(def, marker, parent);
				bool flag5 = def is MethodDef;
				if (flag5)
				{
					parent.ExcludeMethod(context, (MethodDef)def);
				}
			}
			parent.ExcludeMethod(context, this.cctor);
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00016388 File Offset: 0x00014588
		public void HandleMD(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters)
		{
			this.cctorRepl.Body = this.cctor.Body;
			this.cctor.Body = new CilBody();
			this.cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, this.initMethod));
			this.cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, this.cctorRepl));
			this.cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			this.methods = (from method in parameters.Targets.OfType<MethodDef>()
			where method.HasBody
			select method).ToList<MethodDef>();
			context.CurrentModuleWriterListener.OnWriterEvent += this.OnWriterEvent;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0001647C File Offset: 0x0001467C
		private void OnWriterEvent(object sender, ModuleWriterListenerEventArgs e)
		{
			ModuleWriterBase writer = (ModuleWriterBase)sender;
			bool flag = e.WriterEvent == ModuleWriterEvent.MDBeginWriteMethodBodies;
			if (flag)
			{
				this.context.Logger.Debug("Extracting method bodies...");
				this.CreateSection(writer);
			}
			else
			{
				bool flag2 = e.WriterEvent == ModuleWriterEvent.BeginStrongNameSign;
				if (flag2)
				{
					this.context.Logger.Debug("Encrypting method section...");
					this.EncryptSection(writer);
				}
			}
		}

		// Token: 0x060002AE RID: 686 RVA: 0x000164F0 File Offset: 0x000146F0
		private void CreateSection(ModuleWriterBase writer)
		{
			PESection peSection = new PESection("", 1610612768U);
			bool moved = false;
			bool flag = writer.StrongNameSignature != null;
			if (flag)
			{
				uint alignment = writer.TextSection.Remove(writer.StrongNameSignature).Value;
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
					uint alignment = writer.TextSection.Remove(managedWriter.ImportAddressTable).Value;
					peSection.Add(managedWriter.ImportAddressTable, alignment);
					moved = true;
				}
				bool flag4 = managedWriter.StartupStub != null;
				if (flag4)
				{
					uint alignment = writer.TextSection.Remove(managedWriter.StartupStub).Value;
					peSection.Add(managedWriter.StartupStub, alignment);
					moved = true;
				}
			}
			bool flag5 = moved;
			if (flag5)
			{
				writer.Sections.Add(peSection);
			}
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
			writer.Sections.Insert(this.random.NextInt32(writer.Sections.Count), newSection);
			newSection.Add(new ByteArrayChunk(this.random.NextBytes(16)), 16U);
			JITBodyIndex bodyIndex = new JITBodyIndex(from method in this.methods
			select writer.MetaData.GetToken(method).Raw);
			newSection.Add(bodyIndex, 16U);
			foreach (MethodDef method2 in this.methods.WithProgress(this.context.Logger))
			{
				bool flag6 = !method2.HasBody;
				if (!flag6)
				{
					MDToken token = writer.MetaData.GetToken(method2);
					JITMethodBody jitBody = new JITMethodBody();
					JITMethodBodyWriter bodyWriter = new JITMethodBodyWriter(writer.MetaData, method2.Body, jitBody, this.random.NextUInt32(), writer.MetaData.KeepOldMaxStack || method2.Body.KeepOldMaxStack);
					bodyWriter.Write();
					jitBody.Serialize(token.Raw, this.key, this.fieldLayout);
					bodyIndex.Add(token.Raw, jitBody);
					method2.Body = JITMode.NopBody;
					RawMethodRow rawMethodRow = writer.MetaData.TablesHeap.MethodTable[token.Rid];
					rawMethodRow.ImplFlags |= 8;
					this.context.CheckCancellation();
				}
			}
			bodyIndex.PopulateSection(newSection);
			newSection.Add(new ByteArrayChunk(new byte[4]), 4U);
		}

		// Token: 0x060002AF RID: 687 RVA: 0x000168A0 File Offset: 0x00014AA0
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

		// Token: 0x060002B0 RID: 688 RVA: 0x00016AE0 File Offset: 0x00014CE0
		private void Hash(Stream stream, BinaryReader reader, uint offset, uint size)
		{
			long original = stream.Position;
			stream.Position = (long)((ulong)offset);
			size >>= 2;
			for (uint i = 0U; i < size; i += 1U)
			{
				uint data = reader.ReadUInt32();
				uint tmp = (this.z ^ data) + this.x + this.c * this.v;
				this.z = this.x;
				this.x = this.c;
				this.x = this.v;
				this.v = tmp;
			}
			stream.Position = original;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00016B74 File Offset: 0x00014D74
		private uint[] DeriveKey()
		{
			uint[] dst = new uint[16];
			uint[] src = new uint[16];
			for (int i = 0; i < 16; i++)
			{
				dst[i] = this.v;
				src[i] = this.x;
				this.z = (this.x >> 5 | this.x << 27);
				this.x = (this.c >> 3 | this.c << 29);
				this.c = (this.v >> 7 | this.v << 25);
				this.v = (this.z >> 11 | this.z << 21);
			}
			return this.deriver.DeriveKey(dst, src);
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00004A68 File Offset: 0x00002C68
		public JITMode()
		{
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0000566E File Offset: 0x0000386E
		// Note: this type is marked as 'beforefieldinit'.
		static JITMode()
		{
		}

		// Token: 0x040001DD RID: 477
		private static readonly CilBody NopBody = new CilBody
		{
			Instructions = 
			{
				Instruction.Create(OpCodes.Ldnull),
				Instruction.Create(OpCodes.Throw)
			}
		};

		// Token: 0x040001DE RID: 478
		private uint c;

		// Token: 0x040001DF RID: 479
		private MethodDef cctor;

		// Token: 0x040001E0 RID: 480
		private MethodDef cctorRepl;

		// Token: 0x040001E1 RID: 481
		private ConfuserContext context;

		// Token: 0x040001E2 RID: 482
		private IKeyDeriver deriver;

		// Token: 0x040001E3 RID: 483
		private byte[] fieldLayout;

		// Token: 0x040001E4 RID: 484
		private MethodDef initMethod;

		// Token: 0x040001E5 RID: 485
		private uint key;

		// Token: 0x040001E6 RID: 486
		private List<MethodDef> methods;

		// Token: 0x040001E7 RID: 487
		private uint name1;

		// Token: 0x040001E8 RID: 488
		private uint name2;

		// Token: 0x040001E9 RID: 489
		private RandomGenerator random;

		// Token: 0x040001EA RID: 490
		private uint v;

		// Token: 0x040001EB RID: 491
		private uint x;

		// Token: 0x040001EC RID: 492
		private uint z;

		// Token: 0x020000B0 RID: 176
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060002B4 RID: 692 RVA: 0x000056A6 File Offset: 0x000038A6
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060002B5 RID: 693 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x060002B6 RID: 694 RVA: 0x000056B2 File Offset: 0x000038B2
			internal bool <HandleInject>b__16_0(MethodDef method)
			{
				return method.Name == "Initialize";
			}

			// Token: 0x060002B7 RID: 695 RVA: 0x000056C4 File Offset: 0x000038C4
			internal bool <HandleInject>b__16_1(MethodDef method)
			{
				return method.Name == "HookHandler";
			}

			// Token: 0x060002B8 RID: 696 RVA: 0x000054B4 File Offset: 0x000036B4
			internal bool <HandleMD>b__17_0(MethodDef method)
			{
				return method.HasBody;
			}

			// Token: 0x040001ED RID: 493
			public static readonly JITMode.<>c <>9 = new JITMode.<>c();

			// Token: 0x040001EE RID: 494
			public static Func<MethodDef, bool> <>9__16_0;

			// Token: 0x040001EF RID: 495
			public static Func<MethodDef, bool> <>9__16_1;

			// Token: 0x040001F0 RID: 496
			public static Func<MethodDef, bool> <>9__17_0;
		}

		// Token: 0x020000B1 RID: 177
		[CompilerGenerated]
		private sealed class <>c__DisplayClass19_0
		{
			// Token: 0x060002B9 RID: 697 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c__DisplayClass19_0()
			{
			}

			// Token: 0x060002BA RID: 698 RVA: 0x00016C30 File Offset: 0x00014E30
			internal uint <CreateSection>b__0(MethodDef method)
			{
				return this.writer.MetaData.GetToken(method).Raw;
			}

			// Token: 0x040001F1 RID: 497
			public ModuleWriterBase writer;
		}
	}
}
