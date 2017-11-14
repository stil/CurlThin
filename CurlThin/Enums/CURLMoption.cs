using System.Diagnostics.CodeAnalysis;

namespace CurlThin.Enums
{
    /// <summary>
    ///     Reference: https://github.com/curl/curl/blob/master/include/curl/multi.h
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CURLMoption : uint
    {
        /// <summary>
        ///     This is the socket callback function pointer.
        /// </summary>
        SOCKETFUNCTION = CURLOPTTYPE.FUNCTIONPOINT + 1,

        /// <summary>
        ///     This is the argument passed to the socket callback.
        /// </summary>
        SOCKETDATA = CURLOPTTYPE.OBJECTPOINT + 2,

        /// <summary>
        ///     Set to 1 to enable pipelining for this multi handle.
        /// </summary>
        PIPELINING = CURLOPTTYPE.LONG + 3,

        /// <summary>
        ///     This is the timer callback function pointer.
        /// </summary>
        TIMERFUNCTION = CURLOPTTYPE.FUNCTIONPOINT + 4,

        /// <summary>
        ///     This is the argument passed to the timer callback.
        /// </summary>
        TIMERDATA = CURLOPTTYPE.OBJECTPOINT + 5,

        /// <summary>
        ///     Maximum number of entries in the connection cache.
        /// </summary>
        MAXCONNECTS = CURLOPTTYPE.LONG + 6,

        /// <summary>
        ///     Maximum number of (pipelining) connections to one host.
        /// </summary>
        MAX_HOST_CONNECTIONS = CURLOPTTYPE.LONG + 7,

        /* maximum number of requests in a pipeline */
        MAX_PIPELINE_LENGTH = CURLOPTTYPE.LONG + 8,

        /// <summary>
        ///     A connection with a content-length longer than this will not be considered for pipelining.
        /// </summary>
        CONTENT_LENGTH_PENALTY_SIZE = CURLOPTTYPE.OFF_T + 9,

        /// <summary>
        ///     A connection with a chunk length longer than this will not be considered for pipelining.
        /// </summary>
        CHUNK_LENGTH_PENALTY_SIZE = CURLOPTTYPE.OFF_T + 10,

        /// <summary>
        ///     A list of site names(+port) that are blacklisted from pipelining.
        /// </summary>
        PIPELINING_SITE_BL = CURLOPTTYPE.OBJECTPOINT + 11,

        /// <summary>
        ///     A list of server types that are blacklisted from pipelining.
        /// </summary>
        PIPELINING_SERVER_BL = CURLOPTTYPE.OBJECTPOINT + 12,

        /// <summary>
        ///     Maximum number of open connections in total.
        /// </summary>
        MAX_TOTAL_CONNECTIONS = CURLOPTTYPE.LONG + 13,

        /// <summary>
        ///     This is the server push callback function pointer.
        /// </summary>
        PUSHFUNCTION = CURLOPTTYPE.FUNCTIONPOINT + 14,

        /// <summary>
        ///     This is the argument passed to the server push callback.
        /// </summary>
        PUSHDATA = CURLOPTTYPE.OBJECTPOINT + 15,

        /// <summary>
        ///     The last unused.
        /// </summary>
        CURLMOPT_LASTENTRY
    }
}