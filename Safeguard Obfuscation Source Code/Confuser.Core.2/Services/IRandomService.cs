using System;

namespace Confuser.Core.Services
{
	/// <summary>
	///     Provides methods to obtain a unique stable PRNG for any given ID.
	/// </summary>
	// Token: 0x02000079 RID: 121
	public interface IRandomService
	{
		/// <summary>
		///     Gets a RNG with the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>The requested RNG.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="id" /> is <c>null</c>.</exception>
		// Token: 0x060002CA RID: 714
		RandomGenerator GetRandomGenerator(string id);
	}
}
