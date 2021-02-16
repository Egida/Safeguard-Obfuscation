using System;
using dnlib.DotNet;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000076 RID: 118
	internal interface IKnownThings
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060002A5 RID: 677
		Func<KnownTypes, TypeDef> Types { get; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060002A6 RID: 678
		Func<KnownProperties, Tuple<KnownTypes, PropertyDef, TypeDef>> Properties { get; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060002A7 RID: 679
		AssemblyDef FrameworkAssembly { get; }
	}
}
