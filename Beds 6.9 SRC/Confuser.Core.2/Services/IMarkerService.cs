using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	/// <summary>
	///     Provides methods to access the obfuscation marker.
	/// </summary>
	// Token: 0x02000078 RID: 120
	public interface IMarkerService
	{
		/// <summary>
		///     Gets the parent component of the specified helper.
		/// </summary>
		/// <param name="def">The helper definition.</param>
		/// <returns>The parent component of the helper, or <c>null</c> if the specified definition is not a helper.</returns>
		// Token: 0x060002C7 RID: 711
		ConfuserComponent GetHelperParent(IDnlibDef def);

		/// <summary>
		///     Determines whether the specified definition is marked.
		/// </summary>
		/// <param name="def">The definition.</param>
		/// <returns><c>true</c> if the specified definition is marked; otherwise, <c>false</c>.</returns>
		// Token: 0x060002C8 RID: 712
		bool IsMarked(IDnlibDef def);

		/// <summary>
		///     Marks the helper member.
		/// </summary>
		/// <param name="member">The helper member.</param>
		/// <param name="parentComp">The parent component.</param>
		/// <exception cref="T:System.ArgumentException"><paramref name="member" /> is a <see cref="T:dnlib.DotNet.ModuleDef" />.</exception>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="member" /> is <c>null</c>.</exception>
		// Token: 0x060002C9 RID: 713
		void Mark(IDnlibDef member, ConfuserComponent parentComp);
	}
}
