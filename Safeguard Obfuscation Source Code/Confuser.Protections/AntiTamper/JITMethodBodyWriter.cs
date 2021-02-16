using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000AB RID: 171
	internal class JITMethodBodyWriter : MethodBodyWriterBase
	{
		// Token: 0x0600028F RID: 655 RVA: 0x000055A7 File Offset: 0x000037A7
		public JITMethodBodyWriter(MetaData md, CilBody body, JITMethodBody jitBody, uint mulSeed, bool keepMaxStack) : base(body.Instructions, body.ExceptionHandlers)
		{
			this.metadata = md;
			this.body = body;
			this.jitBody = jitBody;
			this.keepMaxStack = keepMaxStack;
			this.jitBody.MulSeed = mulSeed;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00015780 File Offset: 0x00013980
		public void Write()
		{
			uint codeSize = base.InitializeInstructionOffsets();
			this.jitBody.MaxStack = (this.keepMaxStack ? ((uint)this.body.MaxStack) : base.GetMaxStack());
			this.jitBody.Options = 0U;
			bool initLocals = this.body.InitLocals;
			if (initLocals)
			{
				this.jitBody.Options |= 16U;
			}
			bool flag = this.body.Variables.Count > 0;
			if (flag)
			{
				LocalSig local = new LocalSig((from var in this.body.Variables
				select var.Type).ToList<TypeSig>());
				this.jitBody.LocalVars = SignatureWriter.Write(this.metadata, local);
			}
			else
			{
				this.jitBody.LocalVars = new byte[0];
			}
			using (MemoryStream ms = new MemoryStream())
			{
				uint _codeSize = base.WriteInstructions(new BinaryWriter(ms));
				Debug.Assert(codeSize == _codeSize);
				this.jitBody.ILCode = ms.ToArray();
			}
			this.jitBody.EHs = new JITEHClause[this.exceptionHandlers.Count];
			bool flag2 = this.exceptionHandlers.Count > 0;
			if (flag2)
			{
				this.jitBody.Options |= 8U;
				for (int i = 0; i < this.exceptionHandlers.Count; i++)
				{
					ExceptionHandler eh = this.exceptionHandlers[i];
					this.jitBody.EHs[i].Flags = (uint)eh.HandlerType;
					uint tryStart = base.GetOffset(eh.TryStart);
					uint tryEnd = base.GetOffset(eh.TryEnd);
					this.jitBody.EHs[i].TryOffset = tryStart;
					this.jitBody.EHs[i].TryLength = tryEnd - tryStart;
					uint handlerStart = base.GetOffset(eh.HandlerStart);
					uint handlerEnd = base.GetOffset(eh.HandlerEnd);
					this.jitBody.EHs[i].HandlerOffset = handlerStart;
					this.jitBody.EHs[i].HandlerLength = handlerEnd - handlerStart;
					bool flag3 = eh.HandlerType == ExceptionHandlerType.Catch;
					if (flag3)
					{
						uint token = this.metadata.GetToken(eh.CatchType).Raw;
						bool flag4 = (token & 4278190080U) == 452984832U;
						if (flag4)
						{
							this.jitBody.Options |= 128U;
						}
						this.jitBody.EHs[i].ClassTokenOrFilterOffset = token;
					}
					else
					{
						bool flag5 = eh.HandlerType == ExceptionHandlerType.Filter;
						if (flag5)
						{
							this.jitBody.EHs[i].ClassTokenOrFilterOffset = base.GetOffset(eh.FilterStart);
						}
					}
				}
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00015AAC File Offset: 0x00013CAC
		protected override void WriteInlineField(BinaryWriter writer, Instruction instr)
		{
			writer.Write(this.metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00015AAC File Offset: 0x00013CAC
		protected override void WriteInlineMethod(BinaryWriter writer, Instruction instr)
		{
			writer.Write(this.metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00015AAC File Offset: 0x00013CAC
		protected override void WriteInlineSig(BinaryWriter writer, Instruction instr)
		{
			writer.Write(this.metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00015AAC File Offset: 0x00013CAC
		protected override void WriteInlineString(BinaryWriter writer, Instruction instr)
		{
			writer.Write(this.metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00015AAC File Offset: 0x00013CAC
		protected override void WriteInlineTok(BinaryWriter writer, Instruction instr)
		{
			writer.Write(this.metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00015AAC File Offset: 0x00013CAC
		protected override void WriteInlineType(BinaryWriter writer, Instruction instr)
		{
			writer.Write(this.metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x040001CF RID: 463
		private readonly CilBody body;

		// Token: 0x040001D0 RID: 464
		private readonly JITMethodBody jitBody;

		// Token: 0x040001D1 RID: 465
		private readonly bool keepMaxStack;

		// Token: 0x040001D2 RID: 466
		private readonly MetaData metadata;

		// Token: 0x020000AC RID: 172
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000297 RID: 663 RVA: 0x000055E7 File Offset: 0x000037E7
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000298 RID: 664 RVA: 0x00004A68 File Offset: 0x00002C68
			public <>c()
			{
			}

			// Token: 0x06000299 RID: 665 RVA: 0x000055F3 File Offset: 0x000037F3
			internal TypeSig <Write>b__5_0(Local var)
			{
				return var.Type;
			}

			// Token: 0x040001D3 RID: 467
			public static readonly JITMethodBodyWriter.<>c <>9 = new JITMethodBodyWriter.<>c();

			// Token: 0x040001D4 RID: 468
			public static Func<Local, TypeSig> <>9__5_0;
		}
	}
}
