//
// Native.cs
//
// Author:
//   Aaron Bockover <aaron@abock.org>
//
// Copyright 2014 Aaron Bockover
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Runtime.InteropServices;
using CurlThin.Enums;
using CurlThin.SafeHandles;

namespace CurlThin
{
	public static class CurlNative
	{
	    private const string LIBCURL = "libcurl";

	    [DllImport(LIBCURL, EntryPoint = "curl_global_init")]
	    public static extern CURLcode Init(CURLglobal flags = CURLglobal.DEFAULT);

	    [DllImport(LIBCURL, EntryPoint = "curl_global_cleanup")]
	    public static extern void Cleanup();

        public static class Easy
		{
			[DllImport (LIBCURL, EntryPoint = "curl_easy_init")]
			public static extern SafeEasyHandle Init ();

			[DllImport (LIBCURL, EntryPoint = "curl_easy_cleanup")]
			public static extern void Cleanup (IntPtr handle);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_perform")]
			public static extern CURLcode Perform (SafeEasyHandle handle);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_setopt")]
			public static extern CURLcode SetOpt (SafeEasyHandle handle, CURLoption option, long value);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_setopt")]
			public static extern CURLcode SetOpt (SafeEasyHandle handle, CURLoption option, IntPtr value);

		    [DllImport(LIBCURL, EntryPoint = "curl_easy_setopt", CharSet = CharSet.Ansi)]
            public static extern CURLcode SetOpt (SafeEasyHandle handle, CURLoption option, string value);

			public delegate UIntPtr DataHandler (IntPtr data, UIntPtr size, UIntPtr nmemb, IntPtr userdata);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_setopt")]
			public static extern CURLcode SetOpt (SafeEasyHandle handle, CURLoption option, DataHandler value);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_getinfo")]
			public static extern CURLcode GetInfo (SafeEasyHandle handle, CURLINFO option, out int value);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_getinfo")]
			public static extern CURLcode GetInfo (SafeEasyHandle handle, CURLINFO option, out IntPtr value);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_getinfo")]
			public static extern CURLcode GetInfo (SafeEasyHandle handle, CURLINFO option, out double value);
		}

		public static class Multi
		{
			[DllImport (LIBCURL, EntryPoint = "curl_multi_init")]
			public static extern IntPtr Init ();

			[DllImport (LIBCURL, EntryPoint = "curl_multi_cleanup")]
			public static extern CURLMcode Cleanup (IntPtr multiHandle);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_add_handle")]
			public static extern CURLMcode AddHandle (IntPtr multiHandle, SafeEasyHandle easyHandle);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_remove_handle")]
			public static extern CURLMcode RemoveHandle (IntPtr multiHandle, SafeEasyHandle easyHandle);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_perform")]
			public static extern CURLMcode Perform (IntPtr multiHandle, ref int runningHandles);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_fdset")]
			public static extern CURLMcode FdSet (IntPtr multiHandle, IntPtr readfds, IntPtr writefds, IntPtr errorfds, ref int maxfds);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_timeout")]
			public static extern CURLMcode Timeout (IntPtr multiHandle, ref int timeout);
		}

		public class Select : IDisposable
		{
		    private const int FD_SETSIZE = 32; // __DARWIN_FD_SETSIZE

		    private struct timeval {
				public int tv_sec;
				public int tv_usec;
			}

			[DllImport ("libc")]
			private static extern int select (int nfds, IntPtr readfds, IntPtr writefds, IntPtr errorfds, ref timeval timeout);

			[DllImport ("libc")]
			private static extern IntPtr memset (IntPtr b, int c, IntPtr len);

			public delegate bool SetFdsHandler (IntPtr readfds, IntPtr writefds, IntPtr errorfds, ref int maxfds);

		    private IntPtr readfds = Marshal.AllocHGlobal (FD_SETSIZE);
		    private IntPtr writefds = Marshal.AllocHGlobal (FD_SETSIZE);
		    private IntPtr errorfds = Marshal.AllocHGlobal (FD_SETSIZE);

			~Select ()
			{
				Dispose (false);
			}

			public void Dispose ()
			{
				Dispose (true);
				GC.SuppressFinalize (this);
			}

			protected virtual void Dispose (bool disposing)
			{
				if (readfds != IntPtr.Zero) {
					Marshal.FreeHGlobal (readfds);
					readfds = IntPtr.Zero;
				}

				if (writefds != IntPtr.Zero) {
					Marshal.FreeHGlobal (writefds);
					writefds = IntPtr.Zero;
				}

				if (errorfds != IntPtr.Zero) {
					Marshal.FreeHGlobal (errorfds);
					errorfds = IntPtr.Zero;
				}
			}

			public bool Perform (Func<TimeSpan> requestedTimeout = null, SetFdsHandler setFds = null)
			{
				if (readfds == IntPtr.Zero || writefds == IntPtr.Zero || errorfds == IntPtr.Zero)
					throw new ObjectDisposedException ("Select");

				int maxfds = -1;

				memset (readfds, 0, (IntPtr)FD_SETSIZE);
				memset (writefds, 0, (IntPtr)FD_SETSIZE);
				memset (errorfds, 0, (IntPtr)FD_SETSIZE);

				var timeout = new timeval ();

				if (requestedTimeout != null) {
					var timespan = requestedTimeout ();
					if (timespan == TimeSpan.Zero)
						return true;

					timeout.tv_sec = (int)(timespan.Ticks / TimeSpan.TicksPerSecond);
					timeout.tv_usec = (int)(timespan.Ticks % TimeSpan.TicksPerSecond / TimeSpan.TicksPerMillisecond);
				}

				if (setFds != null) {
					if (!setFds (readfds, writefds, errorfds, ref maxfds))
						return false;
				}

				return select (maxfds + 1, readfds, writefds, errorfds, ref timeout) == 0;
			}
		}
	}
}