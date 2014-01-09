//
// Protocols.cs
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

namespace Curl
{
	public enum Protocols
	{
		Http   = 1 << 0,
		Https  = 1 << 1,
		Ftp    = 1 << 2,
		Ftps   = 1 << 3,
		Scp    = 1 << 4,
		Sftp   = 1 << 5,
		Telnet = 1 << 6,
		Ldap   = 1 << 7,
		Ldaps  = 1 << 8,
		Dict   = 1 << 9,
		File   = 1 << 10,
		Tftp   = 1 << 11,
		Imap   = 1 << 12,
		Imaps  = 1 << 13,
		Pop3   = 1 << 14,
		Pop3s  = 1 << 15,
		Smtp   = 1 << 16,
		Smtps  = 1 << 17,
		Rtsp   = 1 << 18,
		Rtmp   = 1 << 19,
		Rtmpt  = 1 << 20,
		Rtmpe  = 1 << 21,
		Rtmpte = 1 << 22,
		Rtmps  = 1 << 23,
		Rtmpts = 1 << 24,
		Gopher = 1 << 25,
		All    = ~0,
	}
}