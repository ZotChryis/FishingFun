using System.Drawing;
using System.Threading;

namespace FishingFun
{
    /// <summary>
    /// Interface for bobber detection implementations.
    /// </summary>
    public interface IBobberFinder
    {
        /// <summary>
        /// Finds the bobber position in the captured screen area.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the search operation.</param>
        /// <returns>The screen coordinates of the bobber, or Point.Empty if not found.</returns>
        Point Find(CancellationToken cancellationToken = default);

        /// <summary>
        /// Resets the finder state.
        /// </summary>
        void Reset();
    }
}