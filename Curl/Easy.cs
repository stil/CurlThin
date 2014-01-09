//
// Easy.cs
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
using System.Text;
using System.Runtime.InteropServices;

namespace Curl
{
	public enum HeaderKind
	{
		Status,
		KeyValue,
		BodyDelimiter
	}

	public delegate void DataHandler (byte [] data);
	public delegate void HeaderHandler (HeaderKind kind, string name, string value);

	public class Easy : IDisposable
	{
		IntPtr handle;
		internal IntPtr Handle {
			get { return handle; }
		}

		public Easy ()
		{
			handle = Native.Easy.Init ();
		}

		~Easy ()
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
				Native.Easy.Cleanup (handle);
				handle = IntPtr.Zero;
			}
		}

		void CheckDisposed ()
		{
			if (handle == IntPtr.Zero)
				throw new ObjectDisposedException ("Curl.Easy");
		}

		public void Perform ()
		{
			CheckDisposed ();
			var code = Native.Easy.Perform (handle);
			if (code != Native.Code.OK)
				throw new CurlException (code);
		}

		internal void SetOpt (Native.Option option, int value)
		{
			CheckDisposed ();
			var code = Native.Easy.SetOpt (handle, option, value);
			if (code != Native.Code.OK)
				throw new CurlException (code);
		}

		internal void SetOpt (Native.Option option, string value)
		{
			CheckDisposed ();
			var code = Native.Easy.SetOpt (handle, option, value);
			if (code != Native.Code.OK)
				throw new CurlException (code);
		}

		internal void SetOpt (Native.Option option, IntPtr value)
		{
			CheckDisposed ();
			var code = Native.Easy.SetOpt (handle, option, value);
			if (code != Native.Code.OK)
				throw new CurlException (code);
		}

		internal void SetOpt (Native.Option option, Native.Easy.DataHandler value)
		{
			CheckDisposed ();
			var code = Native.Easy.SetOpt (handle, option, value);
			if (code != Native.Code.OK)
				throw new CurlException (code);
		}

		internal int GetInfoInt32 (Native.Info info)
		{
			CheckDisposed ();
			int value;
			var code = Native.Easy.GetInfo (handle, info, out value);
			if (code != Native.Code.OK)
				throw new CurlException (code);
			return value;
		}

		internal IntPtr GetInfoIntPtr (Native.Info info)
		{
			CheckDisposed ();
			IntPtr value;
			var code = Native.Easy.GetInfo (handle, info, out value);
			if (code != Native.Code.OK)
				throw new CurlException (code);
			return value;
		}

		internal double GetInfoDouble (Native.Info info)
		{
			CheckDisposed ();
			double value;
			var code = Native.Easy.GetInfo (handle, info, out value);
			if (code != Native.Code.OK)
				throw new CurlException (code);
			return value;
		}

		IntPtr NativeWriteHandler (IntPtr data, IntPtr size, IntPtr nmemb, IntPtr userdata)
		{
			var length = (int)size * (int)nmemb;

			if (dataHandler != null) {
				var buffer = new byte [length];
				Marshal.Copy (data, buffer, 0, length);
				dataHandler (buffer);
			}

			return (IntPtr)length;
		}

		IntPtr NativeHeaderHandler (IntPtr data, IntPtr size, IntPtr nmemb, IntPtr userdata)
		{
			var length = (int)size * (int)nmemb;

			if (headerHandler != null) {
				var buffer = new byte [length];
				Marshal.Copy (data, buffer, 0, length);
				ParseHeader (Encoding.UTF8.GetString (buffer), headerHandler);
			}

			return (IntPtr)length;
		}

		void ParseHeader (string header, HeaderHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException ("handler");

			if (header == null) {
				return;
			} else if (header == "\r\n") {
				handler (HeaderKind.BodyDelimiter, null, null);
				return;
			}

			header = header.TrimEnd ('\r', '\n');
			var colonOffset = header.IndexOf (':');

			if (header.StartsWith ("HTTP/") && colonOffset < 0) {
				handler (HeaderKind.Status, header, null);
				return;
			} else if (colonOffset > 0) {
				handler (HeaderKind.KeyValue,
					header.Substring (0, colonOffset).Trim (),
					header.Substring (colonOffset + 1).Trim ()
				);
			}
		}

		string url;
		public string Url {
			get { return url; }
			set { SetOpt (Native.Option.URL, value); url = value; }
		}

		DataHandler dataHandler;
		public DataHandler WriteHandler {
			get { return dataHandler; }
			set {
				dataHandler = value;
				if (value == null)
					SetOpt (Native.Option.WRITEFUNCTION, IntPtr.Zero);
				else
					SetOpt (Native.Option.WRITEFUNCTION, NativeWriteHandler);
			}
		}

		HeaderHandler headerHandler;
		public HeaderHandler HeaderHandler {
			get { return headerHandler; }
			set {
				headerHandler = value;
				if (value == null)
					SetOpt (Native.Option.HEADERFUNCTION, IntPtr.Zero);
				else
					SetOpt (Native.Option.HEADERFUNCTION, NativeHeaderHandler);
			}
		}

		Protocols protocols;
		public Protocols Protocols {
			get { return protocols; }
			set {
				SetOpt (Native.Option.PROTOCOLS, (int)value);
				protocols = value;
			}
		}

		bool followLocation;
		public bool FollowLocation {
			get { return followLocation; }
			set {
				SetOpt (Native.Option.FOLLOWLOCATION, value ? 1 : 0);
				followLocation = value;
			}
		}

		bool autoReferer;
		public bool AutoReferer {
			get { return autoReferer; }
			set {
				SetOpt (Native.Option.AUTOREFERER, value ? 1 : 0);
				autoReferer = value;
			}
		}

		Version httpVersion;
		public Version HttpVersion {
			get { return httpVersion; }
			set {
				if (value.Major == 1 && (value.Minor == 0 || value.Minor == 1)) {
					SetOpt (Native.Option.HTTP_VERSION, value.Minor + 1);
					httpVersion = value;
				} else {
					SetOpt (Native.Option.HTTP_VERSION, 0);
					httpVersion = default (Version);
				}
			}
		}

		string httpMethod;
		public string HttpMethod {
			get { return httpMethod; }
			set {
				var m = (value ?? "GET").ToUpper ();

				switch (m) {
				case "GET":
					SetOpt (Native.Option.HTTPGET, 1);
					break;
				case "POST":
					SetOpt (Native.Option.HTTPPOST, 1);
					break;
				case "PUT":
					SetOpt (Native.Option.PUT, 1);
					break;
				default:
					throw new CurlException ("Unsupported HTTP method: " + m);
				}

				httpMethod = m;
			}
		}

		public int ResponseCode {
			get { return GetInfoInt32 (Native.Info.RESPONSE_CODE); }
		}
	}
}