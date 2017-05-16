using CurlThin.SafeHandles;

namespace CurlThin.HyperPipe
{
    /// <summary>
    ///     Provides requests to <see cref="HyperPipe{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRequestProvider<T>
    {
        bool TryNext(SafeEasyHandle easy, out T context);
    }
}