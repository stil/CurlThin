using System.Diagnostics.CodeAnalysis;

namespace CurlThin.Enums
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CURLMSG
    {
        /// <summary>
        ///     First, not used.
        /// </summary>
        NONE,

        /// <summary>
        ///     This easy handle has completed. 'result' contains the CURLcode of the transfer.
        /// </summary>
        DONE,

        /// <summary>
        ///     Last, not used.
        /// </summary>
        LAST
    }
}