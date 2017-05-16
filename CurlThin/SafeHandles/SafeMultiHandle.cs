using System;
using System.Runtime.InteropServices;

namespace CurlThin.SafeHandles
{
    public sealed class SafeMultiHandle : SafeHandle
    {
        private SafeMultiHandle() : base(IntPtr.Zero, false)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            CurlNative.Multi.Cleanup(handle);
            return true;
        }
    }
}