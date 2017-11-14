using System.Diagnostics.CodeAnalysis;

namespace CurlThin.Enums
{
    /// <summary>
    ///     Reference: https://github.com/curl/curl/blob/master/include/curl/curl.h
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class CURLINFOTYPE
    {
        public const uint STRING   = 0x100000;
        public const uint LONG     = 0x200000;
        public const uint DOUBLE   = 0x300000;
        public const uint SLIST    = 0x400000;
        public const uint PTR      = 0x400000; // same as SLIST
        public const uint SOCKET   = 0x500000;
        public const uint OFF_T    = 0x600000;
        public const uint MASK     = 0x0fffff;
        public const uint TYPEMASK = 0xf00000;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CURLINFO : uint
    {
        NONE, // First, never use this.
        EFFECTIVE_URL    = CURLINFOTYPE.STRING + 1,
        RESPONSE_CODE    = CURLINFOTYPE.LONG   + 2,
        TOTAL_TIME       = CURLINFOTYPE.DOUBLE + 3,
        NAMELOOKUP_TIME  = CURLINFOTYPE.DOUBLE + 4,
        CONNECT_TIME     = CURLINFOTYPE.DOUBLE + 5,
        PRETRANSFER_TIME = CURLINFOTYPE.DOUBLE + 6,
        SIZE_UPLOAD      = CURLINFOTYPE.DOUBLE + 7,
        SIZE_UPLOAD_T    = CURLINFOTYPE.OFF_T  + 7,
        SIZE_DOWNLOAD    = CURLINFOTYPE.DOUBLE + 8,
        SIZE_DOWNLOAD_T  = CURLINFOTYPE.OFF_T  + 8,
        SPEED_DOWNLOAD   = CURLINFOTYPE.DOUBLE + 9,
        SPEED_DOWNLOAD_T = CURLINFOTYPE.OFF_T  + 9,
        SPEED_UPLOAD     = CURLINFOTYPE.DOUBLE + 10,
        SPEED_UPLOAD_T   = CURLINFOTYPE.OFF_T  + 10,
        HEADER_SIZE      = CURLINFOTYPE.LONG   + 11,
        REQUEST_SIZE     = CURLINFOTYPE.LONG   + 12,
        SSL_VERIFYRESULT = CURLINFOTYPE.LONG   + 13,
        FILETIME         = CURLINFOTYPE.LONG   + 14,
        CONTENT_LENGTH_DOWNLOAD   = CURLINFOTYPE.DOUBLE + 15,
        CONTENT_LENGTH_DOWNLOAD_T = CURLINFOTYPE.OFF_T  + 15,
        CONTENT_LENGTH_UPLOAD     = CURLINFOTYPE.DOUBLE + 16,
        CONTENT_LENGTH_UPLOAD_T   = CURLINFOTYPE.OFF_T  + 16,
        STARTTRANSFER_TIME = CURLINFOTYPE.DOUBLE + 17,
        CONTENT_TYPE     = CURLINFOTYPE.STRING + 18,
        REDIRECT_TIME    = CURLINFOTYPE.DOUBLE + 19,
        REDIRECT_COUNT   = CURLINFOTYPE.LONG   + 20,
        PRIVATE          = CURLINFOTYPE.STRING + 21,
        HTTP_CONNECTCODE = CURLINFOTYPE.LONG   + 22,
        HTTPAUTH_AVAIL   = CURLINFOTYPE.LONG   + 23,
        PROXYAUTH_AVAIL  = CURLINFOTYPE.LONG   + 24,
        OS_ERRNO         = CURLINFOTYPE.LONG   + 25,
        NUM_CONNECTS     = CURLINFOTYPE.LONG   + 26,
        SSL_ENGINES      = CURLINFOTYPE.SLIST  + 27,
        COOKIELIST       = CURLINFOTYPE.SLIST  + 28,
        LASTSOCKET       = CURLINFOTYPE.LONG   + 29,
        FTP_ENTRY_PATH   = CURLINFOTYPE.STRING + 30,
        REDIRECT_URL     = CURLINFOTYPE.STRING + 31,
        PRIMARY_IP       = CURLINFOTYPE.STRING + 32,
        APPCONNECT_TIME  = CURLINFOTYPE.DOUBLE + 33,
        CERTINFO         = CURLINFOTYPE.PTR    + 34,
        CONDITION_UNMET  = CURLINFOTYPE.LONG   + 35,
        RTSP_SESSION_ID  = CURLINFOTYPE.STRING + 36,
        RTSP_CLIENT_CSEQ = CURLINFOTYPE.LONG   + 37,
        RTSP_SERVER_CSEQ = CURLINFOTYPE.LONG   + 38,
        RTSP_CSEQ_RECV   = CURLINFOTYPE.LONG   + 39,
        PRIMARY_PORT     = CURLINFOTYPE.LONG   + 40,
        LOCAL_IP         = CURLINFOTYPE.STRING + 41,
        LOCAL_PORT       = CURLINFOTYPE.LONG   + 42,
        TLS_SESSION      = CURLINFOTYPE.PTR    + 43,
        ACTIVESOCKET     = CURLINFOTYPE.SOCKET + 44,
        TLS_SSL_PTR      = CURLINFOTYPE.PTR    + 45,
        HTTP_VERSION     = CURLINFOTYPE.LONG   + 46,
        PROXY_SSL_VERIFYRESULT = CURLINFOTYPE.LONG + 47,
        PROTOCOL         = CURLINFOTYPE.LONG   + 48,
        SCHEME           = CURLINFOTYPE.STRING + 49,
        // Fill in new entries below here!

        LASTONE = 49
    }
}