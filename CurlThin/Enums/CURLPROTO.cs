using System;
using System.Diagnostics.CodeAnalysis;

namespace CurlThin.Enums
{
    /// <summary>
    ///     Reference: https://github.com/curl/curl/blob/master/include/curl/curl.h
    /// </summary>
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CURLPROTO
    {
        HTTP   = 1<<0,
        HTTPS  = 1<<1,
        FTP    = 1<<2,
        FTPS   = 1<<3,
        SCP    = 1<<4,
        SFTP   = 1<<5,
        TELNET = 1<<6,
        LDAP   = 1<<7,
        LDAPS  = 1<<8,
        DICT   = 1<<9,
        FILE   = 1<<10,
        TFTP   = 1<<11,
        IMAP   = 1<<12,
        IMAPS  = 1<<13,
        POP3   = 1<<14,
        POP3S  = 1<<15,
        SMTP   = 1<<16,
        SMTPS  = 1<<17,
        RTSP   = 1<<18,
        RTMP   = 1<<19,
        RTMPT  = 1<<20,
        RTMPE  = 1<<21,
        RTMPTE = 1<<22,
        RTMPS  = 1<<23,
        RTMPTS = 1<<24,
        GOPHER = 1<<25,
        SMB    = 1<<26,
        SMBS   = 1<<27,

        /// <summary>
        ///     Enable everything.
        /// </summary>
        ALL    = ~0
	}
}