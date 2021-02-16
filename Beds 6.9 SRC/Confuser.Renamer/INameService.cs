using System;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000005 RID: 5
	public interface INameService
	{
		// Token: 0x06000016 RID: 22
		void AddReference<T>(T obj, INameReference<T> reference);

		// Token: 0x06000017 RID: 23
		void Analyze(IDnlibDef def);

		// Token: 0x06000018 RID: 24
		bool CanRename(object obj);

		// Token: 0x06000019 RID: 25
		T FindRenamer<T>();

		// Token: 0x0600001A RID: 26
		RenameMode GetRenameMode(object obj);

		// Token: 0x0600001B RID: 27
		VTableStorage GetVTables();

		// Token: 0x0600001C RID: 28
		void MarkHelper(IDnlibDef def, IMarkerService marker, ConfuserComponent parentComp);

		// Token: 0x0600001D RID: 29
		string ObfuscateName(string name, RenameMode mode);

		// Token: 0x0600001E RID: 30
		string RandomName();

		// Token: 0x0600001F RID: 31
		string RandomName(RenameMode mode);

		// Token: 0x06000020 RID: 32
		void ReduceRenameMode(object obj, RenameMode val);

		// Token: 0x06000021 RID: 33
		void RegisterRenamer(IRenamer renamer);

		// Token: 0x06000022 RID: 34
		void SetCanRename(object obj, bool val);

		// Token: 0x06000023 RID: 35
		void SetOriginalName(object obj, string name);

		// Token: 0x06000024 RID: 36
		void SetOriginalNamespace(object obj, string ns);

		// Token: 0x06000025 RID: 37
		void SetRenameMode(object obj, RenameMode val);
	}
}
