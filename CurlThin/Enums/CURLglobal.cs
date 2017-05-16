using System;
using System.Diagnostics.CodeAnalysis;

namespace CurlThin.Enums
{
    /// <summary>
    ///     Contains values used to initialize libcurl internally. One of
    ///     these is passed in the call to <see cref="CurlNative.Init" />.
    /// </summary>
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CURLglobal
    {
        /// <summary>
        ///     Initialise nothing extra. This sets no bit.
        /// </summary>
        NOTHING = 0,

        /// <summary>
        ///     Initialize SSL.
        ///     The implication here is that if this bit is not set, the initialization of the SSL layer needs to be done by the
        ///     application or at least outside of libcurl. The exact procedure how to do SSL initializtion depends on the
        ///     TLS backend libcurl uses.
        ///     Doing TLS based transfers without having the TLS layer initialized may lead to unexpected behaviors.
        /// </summary>
        SSL = 1 << 0,

        /// <summary>
        ///     Initialize the Win32 socket libraries.
        ///     The implication here is that if this bit is not set, the initialization of winsock has to be done by the
        ///     application or you risk getting undefined behaviors. This option exists for when the initialization is
        ///     handled outside of libcurl so there's no need for libcurl to do it again.
        /// </summary>
        WIN32 = 1 << 1,

        /// <summary>
        ///     When this flag is set, curl will acknowledge EINTR condition when connecting or when waiting for data.
        ///     Otherwise, curl waits until full timeout elapses. (Added in 7.30.0)
        /// </summary>
        ACK_EINTR = 1 << 2,

        /// <summary>
        ///     Initialize everything possible. This sets all known bits except <see cref="ACK_EINTR" />.
        /// </summary>
        ALL = SSL | WIN32,

        /// <summary>
        ///     A sensible default. It will init both SSL and Win32. Right now, this equals the functionality of the
        ///     <see cref="ALL" /> mask.
        /// </summary>
        DEFAULT = ALL
    }
}