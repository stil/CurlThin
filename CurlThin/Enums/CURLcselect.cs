using System;

// ReSharper disable InconsistentNaming

namespace CurlThin.Enums
{
    [Flags]
    public enum CURLcselect
    {
        IN = 0x01,
        OUT = 0x02,
        ERR = 0x04
    }
}