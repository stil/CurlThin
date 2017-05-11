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

namespace Curl
{
	public static class Native
	{
		const string LIBCURL = "libcurl";

		public static class Easy
		{
			[DllImport (LIBCURL, EntryPoint = "curl_easy_init")]
			public static extern IntPtr Init ();

			[DllImport (LIBCURL, EntryPoint = "curl_easy_cleanup")]
			public static extern void Cleanup (IntPtr handle);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_perform")]
			public static extern Code Perform (IntPtr handle);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_setopt")]
			public static extern Code SetOpt (IntPtr handle, Option option, int value);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_setopt")]
			public static extern Code SetOpt (IntPtr handle, Option option, IntPtr value);

		    [DllImport(LIBCURL, EntryPoint = "curl_easy_setopt")]
            public static extern Code SetOpt (IntPtr handle, Option option, string value);

			public delegate UIntPtr DataHandler (IntPtr data, UIntPtr size, UIntPtr nmemb, IntPtr userdata);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_setopt")]
			public static extern Code SetOpt (IntPtr handle, Option option, DataHandler value);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_getinfo")]
			public static extern Code GetInfo (IntPtr handle, Info option, out int value);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_getinfo")]
			public static extern Code GetInfo (IntPtr handle, Info option, out IntPtr value);

			[DllImport (LIBCURL, EntryPoint = "curl_easy_getinfo")]
			public static extern Code GetInfo (IntPtr handle, Info option, out double value);
		}

		public static class Multi
		{
			[DllImport (LIBCURL, EntryPoint = "curl_multi_init")]
			public static extern IntPtr Init ();

			[DllImport (LIBCURL, EntryPoint = "curl_multi_cleanup")]
			public static extern MultiCode Cleanup (IntPtr multiHandle);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_add_handle")]
			public static extern MultiCode AddHandle (IntPtr multiHandle, IntPtr easyHandle);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_remove_handle")]
			public static extern MultiCode RemoveHandle (IntPtr multiHandle, IntPtr easyHandle);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_perform")]
			public static extern MultiCode Perform (IntPtr multiHandle, ref int runningHandles);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_fdset")]
			public static extern MultiCode FdSet (IntPtr multiHandle, IntPtr readfds, IntPtr writefds, IntPtr errorfds, ref int maxfds);

			[DllImport (LIBCURL, EntryPoint = "curl_multi_timeout")]
			public static extern MultiCode Timeout (IntPtr multiHandle, ref int timeout);
		}

		public class Select : IDisposable
		{
			const int FD_SETSIZE = 32; // __DARWIN_FD_SETSIZE

			struct timeval {
				public int tv_sec;
				public int tv_usec;
			}

			[DllImport ("libc")]
			static extern int select (int nfds, IntPtr readfds, IntPtr writefds, IntPtr errorfds, ref timeval timeout);

			[DllImport ("libc")]
			static extern IntPtr memset (IntPtr b, int c, IntPtr len);

			public delegate bool SetFdsHandler (IntPtr readfds, IntPtr writefds, IntPtr errorfds, ref int maxfds);

			IntPtr readfds = Marshal.AllocHGlobal (FD_SETSIZE);
			IntPtr writefds = Marshal.AllocHGlobal (FD_SETSIZE);
			IntPtr errorfds = Marshal.AllocHGlobal (FD_SETSIZE);

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

		public enum MultiCode
		{
			CALL_MULTI_PERFORM = -1,
			OK,
			BAD_HANDLE,
			BAD_EASY_HANDLE,
			OUT_OF_MEMORY,
			INTERNAL_ERROR,
			BAD_SOCKET,
			UNKNOWN_OPTION,
			LAST
		}

		public enum Code : uint
		{
			OK = 0,
			UNSUPPORTED_PROTOCOL,
			FAILED_INIT,
			URL_MALFORMAT,
			NOT_BUILT_IN,
			COULDNT_RESOLVE_PROXY,
			COULDNT_RESOLVE_HOST,
			COULDNT_CONNECT,
			FTP_WEIRD_SERVER_REPLY,
			REMOTE_ACCESS_DENIED,
			FTP_ACCEPT_FAILED,
			FTP_WEIRD_PASS_REPLY,
			FTP_ACCEPT_TIMEOUT,
			FTP_WEIRD_PASV_REPLY,
			FTP_WEIRD_227_FORMAT,
			FTP_CANT_GET_HOST,
			OBSOLETE16,
			FTP_COULDNT_SET_TYPE,
			PARTIAL_FILE,
			FTP_COULDNT_RETR_FILE,
			OBSOLETE20,
			QUOTE_ERROR,
			HTTP_RETURNED_ERROR,
			WRITE_ERROR,
			OBSOLETE24,
			UPLOAD_FAILED,
			READ_ERROR,
			OUT_OF_MEMORY,
			OPERATION_TIMEDOUT,
			OBSOLETE29,
			FTP_PORT_FAILED,
			FTP_COULDNT_USE_REST,
			OBSOLETE32,
			RANGE_ERROR,
			HTTP_POST_ERROR,
			SSL_CONNECT_ERROR,
			BAD_DOWNLOAD_RESUME,
			FILE_COULDNT_READ_FILE,
			LDAP_CANNOT_BIND,
			LDAP_SEARCH_FAILED,
			OBSOLETE40,
			FUNCTION_NOT_FOUND,
			ABORTED_BY_CALLBACK,
			BAD_FUNCTION_ARGUMENT,
			OBSOLETE44,
			INTERFACE_FAILED,
			OBSOLETE46,
			TOO_MANY_REDIRECTS,
			UNKNOWN_OPTION,
			TELNET_OPTION_SYNTAX,
			OBSOLETE50,
			PEER_FAILED_VERIFICATION,
			GOT_NOTHING,
			SSL_ENGINE_NOTFOUND,
			SSL_ENGINE_SETFAILED,
			SEND_ERROR,
			RECV_ERROR,
			OBSOLETE57,
			SSL_CERTPROBLEM,
			SSL_CIPHER,
			SSL_CACERT,
			BAD_CONTENT_ENCODING,
			LDAP_INVALID_URL,
			FILESIZE_EXCEEDED,
			USE_SSL_FAILED,
			SEND_FAIL_REWIND,
			SSL_ENGINE_INITFAILED,
			LOGIN_DENIED,
			TFTP_NOTFOUND,
			TFTP_PERM,
			REMOTE_DISK_FULL,
			TFTP_ILLEGAL,
			TFTP_UNKNOWNID,
			REMOTE_FILE_EXISTS,
			TFTP_NOSUCHUSER,
			CONV_FAILED,
			CONV_REQD,
			SSL_CACERT_BADFILE,
			REMOTE_FILE_NOT_FOUND,
			SSH,
			SSL_SHUTDOWN_FAILED,
			AGAIN,
			SSL_CRL_BADFILE,
			SSL_ISSUER_ERROR,
			FTP_PRET_FAILED,
			RTSP_CSEQ_ERROR,
			RTSP_SESSION_ERROR,
			FTP_BAD_FILE_LIST,
			CHUNK_FAILED,
			NO_CONNECTION_AVAILABLE,
			LAST
		}

		public enum Option : uint
		{
			CURLOPT_FILE = 10000 + 1,
			URL = 10000 + 2,
			PORT = 0 + 3,
			PROXY = 10000 + 4,
			USERPWD = 10000 + 5,
			PROXYUSERPWD = 10000 + 6,
			RANGE = 10000 + 7,
			INFILE = 10000 + 9,
			ERRORBUFFER = 10000 + 10,
			WRITEFUNCTION = 20000 + 11,
			READFUNCTION = 20000 + 12,
			TIMEOUT = 0 + 13,
			INFILESIZE = 0 + 14,
			POSTFIELDS = 10000 + 15,
			REFERER = 10000 + 16,
			FTPPORT = 10000 + 17,
			USERAGENT = 10000 + 18,
			LOW_SPEED_LIMIT = 0 + 19,
			LOW_SPEED_TIME = 0 + 20,
			RESUME_FROM = 0 + 21,
			COOKIE = 10000 + 22,
			HTTPHEADER = 10000 + 23,
			HTTPPOST = 10000 + 24,
			SSLCERT = 10000 + 25,
			KEYPASSWD = 10000 + 26,
			CRLF = 0 + 27,
			QUOTE = 10000 + 28,
			WRITEHEADER = 10000 + 29,
			COOKIEFILE = 10000 + 31,
			SSLVERSION = 0 + 32,
			TIMECONDITION = 0 + 33,
			TIMEVALUE = 0 + 34,
			CUSTOMREQUEST = 10000 + 36,
			STDERR = 10000 + 37,
			POSTQUOTE = 10000 + 39,
			WRITEINFO = 10000 + 40,
			VERBOSE = 0 + 41,
			HEADER = 0 + 42,
			NOPROGRESS = 0 + 43,
			NOBODY = 0 + 44,
			FAILONERROR = 0 + 45,
			UPLOAD = 0 + 46,
			POST = 0 + 47,
			DIRLISTONLY = 0 + 48,
			APPEND = 0 + 50,
			NETRC = 0 + 51,
			FOLLOWLOCATION = 0 + 52,
			TRANSFERTEXT = 0 + 53,
			PUT = 0 + 54,
			PROGRESSFUNCTION = 20000 + 56,
			PROGRESSDATA = 10000 + 57,
			AUTOREFERER = 0 + 58,
			PROXYPORT = 0 + 59,
			POSTFIELDSIZE = 0 + 60,
			HTTPPROXYTUNNEL = 0 + 61,
			INTERFACE = 10000 + 62,
			KRBLEVEL = 10000 + 63,
			SSL_VERIFYPEER = 0 + 64,
			CAINFO = 10000 + 65,
			MAXREDIRS = 0 + 68,
			FILETIME = 0 + 69,
			TELNETOPTIONS = 10000 + 70,
			MAXCONNECTS = 0 + 71,
			CLOSEPOLICY = 0 + 72,
			FRESH_CONNECT = 0 + 74,
			FORBID_REUSE = 0 + 75,
			RANDOM_FILE = 10000 + 76,
			EGDSOCKET = 10000 + 77,
			CONNECTTIMEOUT = 0 + 78,
			HEADERFUNCTION = 20000 + 79,
			HTTPGET = 0 + 80,
			SSL_VERIFYHOST = 0 + 81,
			COOKIEJAR = 10000 + 82,
			SSL_CIPHER_LIST = 10000 + 83,
			HTTP_VERSION = 0 + 84,
			FTP_USE_EPSV = 0 + 85,
			SSLCERTTYPE = 10000 + 86,
			SSLKEY = 10000 + 87,
			SSLKEYTYPE = 10000 + 88,
			SSLENGINE = 10000 + 89,
			SSLENGINE_DEFAULT = 0 + 90,
			DNS_USE_GLOBAL_CACHE = 0 + 91,
			DNS_CACHE_TIMEOUT = 0 + 92,
			PREQUOTE = 10000 + 93,
			DEBUGFUNCTION = 20000 + 94,
			DEBUGDATA = 10000 + 95,
			COOKIESESSION = 0 + 96,
			CAPATH = 10000 + 97,
			BUFFERSIZE = 0 + 98,
			NOSIGNAL = 0 + 99,
			SHARE = 10000 + 100,
			PROXYTYPE = 0 + 101,
			ACCEPT_ENCODING = 10000 + 102,
			PRIVATE = 10000 + 103,
			HTTP200ALIASES = 10000 + 104,
			UNRESTRICTED_AUTH = 0 + 105,
			FTP_USE_EPRT = 0 + 106,
			HTTPAUTH = 0 + 107,
			SSL_CTX_FUNCTION = 20000 + 108,
			SSL_CTX_DATA = 10000 + 109,
			FTP_CREATE_MISSING_DIRS = 0 + 110,
			PROXYAUTH = 0 + 111,
			FTP_RESPONSE_TIMEOUT = 0 + 112,
			IPRESOLVE = 0 + 113,
			MAXFILESIZE = 0 + 114,
			INFILESIZE_LARGE = 30000 + 115,
			RESUME_FROM_LARGE = 30000 + 116,
			MAXFILESIZE_LARGE = 30000 + 117,
			NETRC_FILE = 10000 + 118,
			USE_SSL = 0 + 119,
			POSTFIELDSIZE_LARGE = 30000 + 120,
			TCP_NODELAY = 0 + 121,
			FTPSSLAUTH = 0 + 129,
			IOCTLFUNCTION = 20000 + 130,
			IOCTLDATA = 10000 + 131,
			FTP_ACCOUNT = 10000 + 134,
			COOKIELIST = 10000 + 135,
			IGNORE_CONTENT_LENGTH = 0 + 136,
			FTP_SKIP_PASV_IP = 0 + 137,
			FTP_FILEMETHOD = 0 + 138,
			LOCALPORT = 0 + 139,
			LOCALPORTRANGE = 0 + 140,
			CONNECT_ONLY = 0 + 141,
			CONV_FROM_NETWORK_FUNCTION = 20000 + 142,
			CONV_TO_NETWORK_FUNCTION = 20000 + 143,
			CONV_FROM_UTF8_FUNCTION = 20000 + 144,
			MAX_SEND_SPEED_LARGE = 30000 + 145,
			MAX_RECV_SPEED_LARGE = 30000 + 146,
			FTP_ALTERNATIVE_TO_USER = 10000 + 147,
			SOCKOPTFUNCTION = 20000 + 148,
			SOCKOPTDATA = 10000 + 149,
			SSL_SESSIONID_CACHE = 0 + 150,
			SSH_AUTH_TYPES = 0 + 151,
			SSH_PUBLIC_KEYFILE = 10000 + 152,
			SSH_PRIVATE_KEYFILE = 10000 + 153,
			FTP_SSL_CCC = 0 + 154,
			TIMEOUT_MS = 0 + 155,
			CONNECTTIMEOUT_MS = 0 + 156,
			HTTP_TRANSFER_DECODING = 0 + 157,
			HTTP_CONTENT_DECODING = 0 + 158,
			NEW_FILE_PERMS = 0 + 159,
			NEW_DIRECTORY_PERMS = 0 + 160,
			POSTREDIR = 0 + 161,
			SSH_HOST_PUBLIC_KEY_MD5 = 10000 + 162,
			OPENSOCKETFUNCTION = 20000 + 163,
			OPENSOCKETDATA = 10000 + 164,
			COPYPOSTFIELDS = 10000 + 165,
			PROXY_TRANSFER_MODE = 0 + 166,
			SEEKFUNCTION = 20000 + 167,
			SEEKDATA = 10000 + 168,
			CRLFILE = 10000 + 169,
			ISSUERCERT = 10000 + 170,
			ADDRESS_SCOPE = 0 + 171,
			CERTINFO = 0 + 172,
			USERNAME = 10000 + 173,
			PASSWORD = 10000 + 174,
			PROXYUSERNAME = 10000 + 175,
			PROXYPASSWORD = 10000 + 176,
			NOPROXY = 10000 + 177,
			TFTP_BLKSIZE = 0 + 178,
			SOCKS5_GSSAPI_SERVICE = 10000 + 179,
			SOCKS5_GSSAPI_NEC = 0 + 180,
			PROTOCOLS = 0 + 181,
			REDIR_PROTOCOLS = 0 + 182,
			SSH_KNOWNHOSTS = 10000 + 183,
			SSH_KEYFUNCTION = 20000 + 184,
			SSH_KEYDATA = 10000 + 185,
			MAIL_FROM = 10000 + 186,
			MAIL_RCPT = 10000 + 187,
			FTP_USE_PRET = 0 + 188,
			RTSP_REQUEST = 0 + 189,
			RTSP_SESSION_ID = 10000 + 190,
			RTSP_STREAM_URI = 10000 + 191,
			RTSP_TRANSPORT = 10000 + 192,
			RTSP_CLIENT_CSEQ = 0 + 193,
			RTSP_SERVER_CSEQ = 0 + 194,
			INTERLEAVEDATA = 10000 + 195,
			INTERLEAVEFUNCTION = 20000 + 196,
			WILDCARDMATCH = 0 + 197,
			CHUNK_BGN_FUNCTION = 20000 + 198,
			CHUNK_END_FUNCTION = 20000 + 199,
			FNMATCH_FUNCTION = 20000 + 200,
			CHUNK_DATA = 10000 + 201,
			FNMATCH_DATA = 10000 + 202,
			RESOLVE = 10000 + 203,
			TLSAUTH_USERNAME = 10000 + 204,
			TLSAUTH_PASSWORD = 10000 + 205,
			TLSAUTH_TYPE = 10000 + 206,
			TRANSFER_ENCODING = 0 + 207,
			CLOSESOCKETFUNCTION = 20000 + 208,
			CLOSESOCKETDATA = 10000 + 209,
			GSSAPI_DELEGATION = 0 + 210,
			DNS_SERVERS = 10000 + 211,
			ACCEPTTIMEOUT_MS = 0 + 212,
			TCP_KEEPALIVE = 0 + 213,
			TCP_KEEPIDLE = 0 + 214,
			TCP_KEEPINTVL = 0 + 215,
			SSL_OPTIONS = 0 + 216,
			MAIL_AUTH = 10000 + 217,
			LASTENTRY
		}

		public enum Info : uint {
			NONE,
			EFFECTIVE_URL = 1048576 + 1,
			RESPONSE_CODE = 2097152 + 2,
			TOTAL_TIME = 3145728 + 3,
			NAMELOOKUP_TIME = 3145728 + 4,
			CONNECT_TIME = 3145728 + 5,
			PRETRANSFER_TIME = 3145728 + 6,
			SIZE_UPLOAD = 3145728 + 7,
			SIZE_DOWNLOAD = 3145728 + 8,
			SPEED_DOWNLOAD = 3145728 + 9,
			SPEED_UPLOAD = 3145728 + 10,
			HEADER_SIZE = 2097152 + 11,
			REQUEST_SIZE = 2097152 + 12,
			SSL_VERIFYRESULT = 2097152 + 13,
			FILETIME = 2097152 + 14,
			CONTENT_LENGTH_DOWNLOAD = 3145728 + 15,
			CONTENT_LENGTH_UPLOAD = 3145728 + 16,
			STARTTRANSFER_TIME = 3145728 + 17,
			CONTENT_TYPE = 1048576 + 18,
			REDIRECT_TIME = 3145728 + 19,
			REDIRECT_COUNT = 2097152 + 20,
			PRIVATE = 1048576 + 21,
			HTTP_CONNECTCODE = 2097152 + 22,
			HTTPAUTH_AVAIL = 2097152 + 23,
			PROXYAUTH_AVAIL = 2097152 + 24,
			OS_ERRNO = 2097152 + 25,
			NUM_CONNECTS = 2097152 + 26,
			SSL_ENGINES = 4194304 + 27,
			COOKIELIST = 4194304 + 28,
			LASTSOCKET = 2097152 + 29,
			FTP_ENTRY_PATH = 1048576 + 30,
			REDIRECT_URL = 1048576 + 31,
			PRIMARY_IP = 1048576 + 32,
			APPCONNECT_TIME = 3145728 + 33,
			CERTINFO = 4194304 + 34,
			CONDITION_UNMET = 2097152 + 35,
			RTSP_SESSION_ID = 1048576 + 36,
			RTSP_CLIENT_CSEQ = 2097152 + 37,
			RTSP_SERVER_CSEQ = 2097152 + 38,
			RTSP_CSEQ_RECV = 2097152 + 39,
			PRIMARY_PORT = 2097152 + 40,
			LOCAL_IP = 1048576 + 41,
			LOCAL_PORT = 2097152 + 42,
			LASTONE = 42
		}
	}
}