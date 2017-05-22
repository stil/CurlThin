using System.Threading.Tasks;
using CurlThin.SafeHandles;

namespace CurlThin.HyperPipe
{
    /// <summary>
    ///     Provides requests to <see cref="HyperPipe{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRequestProvider<out T>
    {
        /// <summary>
        ///     Gets the current element in the collection.
        /// </summary>
        T Current { get; }

        /// <summary>
        ///     Advances the enumerator to the next element of the collection asynchronously.
        /// </summary>
        /// <returns>
        ///     Returns a Task that does transition to the next element. The result of the task is True if the enumerator was
        ///     successfully advanced to the next element, or False if the enumerator has passed the end of the collection.
        /// </returns>
        ValueTask<bool> MoveNextAsync(SafeEasyHandle easy);
    }
}