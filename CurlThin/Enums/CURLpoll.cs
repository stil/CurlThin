using System.Diagnostics.CodeAnalysis;

namespace CurlThin.Enums
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CURLpoll
    {
        /// <summary>
        ///     Register, not interested in readiness (yet).
        /// </summary>
        NONE = 0,

        /// <summary>
        ///     Register, interested in read readiness.
        /// </summary>
        IN = 1,

        /// <summary>
        ///     Register, interested in write readiness.
        /// </summary>
        OUT = 2,

        /// <summary>
        ///     Register, interested in both read and write readiness.
        /// </summary>
        INOUT = 3,

        /// <summary>
        ///     Unregister.
        /// </summary>
        REMOVE = 4
    }
}