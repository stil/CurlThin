//
// Multi.cs
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
using System.Collections;
using System.Collections.Generic;
using Curl.Enums;

namespace Curl
{
	public class Multi : IDisposable, IEnumerable<Easy>
	{
		public enum PerformResult
		{
			Success,
			PerformAgainNow
		}

		IntPtr handle;
		List<Easy> children = new List<Easy> ();
		Native.Select select = new Native.Select ();

		int handlesRemaining;
		public int HandlesRemaining => handlesRemaining;

	    public TimeSpan Timeout { get; set; }

		public Multi ()
		{
			handle = Native.Multi.Init ();
			Timeout = TimeSpan.FromSeconds (60);
		}

		~Multi ()
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
			if (handle != IntPtr.Zero) {
				foreach (var child in children) {
					Native.Multi.RemoveHandle (handle, child.Handle);
					child.Dispose ();
				}

				children.Clear ();

				// FIXME: what if this returns !OK?
				Native.Multi.Cleanup (handle);
				handle = IntPtr.Zero;
			}

			if (select != null) {
				select.Dispose ();
				select = null;
			}
		}

		void CheckDisposed ()
		{
			if (handle == IntPtr.Zero || select == null)
				throw new ObjectDisposedException ("Curl.Easy");
		}

		void Invoke (Func<CURLMcode> call)
		{
			CheckDisposed ();
			var code = call ();
			if (code != CURLMcode.OK)
				throw new CurlException (code);
		}

		public void Add (Easy easy)
		{
			if (easy == null)
				throw new ArgumentNullException (nameof(easy));

			CheckDisposed ();

			var easyHandle = easy.Handle;

			if (easyHandle == IntPtr.Zero)
				throw new ObjectDisposedException ("Easy object already disposed");

			var code = Native.Multi.AddHandle (handle, easyHandle);
			if (code != CURLMcode.OK)
				throw new CurlException (code);

			children.Add (easy);
		}

		public void Remove (Easy easy)
		{
			if (easy == null)
				throw new ArgumentNullException (nameof(easy));

			CheckDisposed ();

			var easyHandle = easy.Handle;

			if (children.Remove (easy) && easyHandle != IntPtr.Zero) {
				var code = Native.Multi.RemoveHandle (handle, easyHandle);
				if (code != CURLMcode.OK)
					throw new CurlException (code);
			}
		}

		public void AutoPerformWithSelect ()
		{
			CheckDisposed ();

			select.Perform (
				requestedTimeout: () => {
					int curlTimeout = -1;
					var code = Native.Multi.Timeout (handle, ref curlTimeout);
					if (code != CURLMcode.OK)
						throw new CurlException (code);
				    if (curlTimeout >= 0)
				        return TimeSpan.FromMilliseconds (curlTimeout);
				    return Timeout;
				},

				setFds: (IntPtr readfds, IntPtr writefds, IntPtr errorfds, ref int maxfds) => {
					var code = Native.Multi.FdSet (handle, readfds, writefds, errorfds, ref maxfds);
					if (code != CURLMcode.OK)
						throw new CurlException (code);
					return true;
				}
			);

			while (Perform () == PerformResult.PerformAgainNow);
		}

		public PerformResult Perform ()
		{
			CheckDisposed ();

			var code = Native.Multi.Perform (handle, ref handlesRemaining);

			switch (code) {
			case CURLMcode.CALL_MULTI_PERFORM:
				return PerformResult.PerformAgainNow;
			case CURLMcode.OK:
				return PerformResult.Success;
			default:
				throw new CurlException (code);
			}
		}

		public IEnumerator<Easy> GetEnumerator ()
		{
			foreach (var child in children)
				yield return child;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}