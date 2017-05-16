using System;
using System.Collections.Generic;

namespace CurlThin.HyperPipe
{
    /// <summary>
    ///     Equality comparer for <see cref="IntPtr" /> struct.
    /// </summary>
    internal class IntPtrEqualityComparer : IEqualityComparer<IntPtr>
    {
        public bool Equals(IntPtr x, IntPtr y)
        {
            return x == y;
        }

        public int GetHashCode(IntPtr obj)
        {
            return obj.GetHashCode();
        }
    }
}