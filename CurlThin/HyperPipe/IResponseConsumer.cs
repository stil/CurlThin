using CurlThin.Enums;
using CurlThin.SafeHandles;

namespace CurlThin.HyperPipe
{
    /// <summary>
    ///     This interface defines class that should consume completed cURL requests.
    /// </summary>
    /// <typeparam name="T">Your custom request context type.</typeparam>
    public interface IResponseConsumer<in T>
    {
        /// <summary>
        ///     Implement this method to process completed cURL requests. For example you can call
        ///     curl_easy_getinfo() to extract information from cURL handle such as HTTP response code
        ///     or total request time.
        /// </summary>
        /// <param name="easy">
        ///     cURL easy handle. You MUST NOT dispose it. Use this handle to for example pass it to
        ///     curl_easy_getinfo().
        /// </param>
        /// <param name="context">Request context. You should use it to recognize "which one request it was".</param>
        /// <param name="errorCode">Error code. On success it's <see cref="CURLcode.OK" /></param>
        /// <returns>This method returns one of actions that will be taken after this request is processed.</returns>
        HandleCompletedAction OnComplete(SafeEasyHandle easy, T context, CURLcode errorCode);
    }
}