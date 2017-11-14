using System.Diagnostics.CodeAnalysis;

namespace CurlThin.Enums
{
    /// <summary>
    ///     Reference: https://github.com/curl/curl/blob/master/include/curl/multi.h
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CURLMcode
    {
        /// <summary>
        ///     Please call curl_multi_perform() or curl_multi_socket*() soon.
        /// </summary>
        CALL_MULTI_PERFORM = -1,

        OK,

        /// <summary>
        ///     The passed-in handle is not a valid CURLM handle.
        /// </summary>
        BAD_HANDLE,

        /// <summary>
        ///     An easy handle was not good/valid.
        /// </summary>
        BAD_EASY_HANDLE,

        /// <summary>
        ///     If you ever get this, you're in deep sh*t.
        /// </summary>
        OUT_OF_MEMORY,

        /// <summary>
        ///     This is a libcurl bug.
        /// </summary>
        INTERNAL_ERROR,

        /// <summary>
        ///     The passed in socket argument did not match.
        /// </summary>
        BAD_SOCKET,

        /// <summary>
        ///     curl_multi_setopt() with unsupported option.
        /// </summary>
        UNKNOWN_OPTION,

        /// <summary>
        ///     An easy handle already added to a multi handle was attempted to get added - again.
        /// </summary>
        ADDED_ALREADY,

        LAST
    }
}