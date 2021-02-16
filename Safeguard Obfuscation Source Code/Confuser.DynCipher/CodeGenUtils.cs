using System;
using System.IO;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher
{
	// Token: 0x02000006 RID: 6
	public static class CodeGenUtils
	{
		// Token: 0x06000010 RID: 16 RVA: 0x000021C4 File Offset: 0x000003C4
		public static byte[] AssembleCode(x86CodeGen codeGen, x86Register reg)
		{
			MemoryStream stream = new MemoryStream();
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				writer.Write(new byte[]
				{
					137,
					224
				});
				writer.Write(new byte[]
				{
					83
				});
				writer.Write(new byte[]
				{
					87
				});
				writer.Write(new byte[]
				{
					86
				});
				writer.Write(new byte[]
				{
					41,
					224
				});
				writer.Write(new byte[]
				{
					131,
					248,
					24
				});
				writer.Write(new byte[]
				{
					116,
					7
				});
				writer.Write(new byte[]
				{
					139,
					68,
					36,
					16
				});
				writer.Write(new byte[]
				{
					80
				});
				writer.Write(new byte[]
				{
					235,
					1
				});
				writer.Write(new byte[]
				{
					81
				});
				foreach (x86Instruction i in codeGen.Instructions)
				{
					writer.Write(i.Assemble());
				}
				bool flag = reg > x86Register.EAX;
				if (flag)
				{
					writer.Write(x86Instruction.Create(x86OpCode.MOV, new Ix86Operand[]
					{
						new x86RegisterOperand(x86Register.EAX),
						new x86RegisterOperand(reg)
					}).Assemble());
				}
				writer.Write(new byte[]
				{
					94
				});
				writer.Write(new byte[]
				{
					95
				});
				writer.Write(new byte[]
				{
					91
				});
				writer.Write(new byte[]
				{
					195
				});
			}
			return stream.ToArray();
		}
	}
}
