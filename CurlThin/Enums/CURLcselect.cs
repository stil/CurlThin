using System;
using System.Diagnostics.CodeAnalysis;

namespace CurlThin.Enums
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CURLcselect
    {
        IN = 0x01,
        OUT = 0x02,
        ERR = 0x04
    }
}