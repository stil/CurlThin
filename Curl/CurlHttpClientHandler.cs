//
// CurlHttpClientHandler.cs
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Curl
{
	public class CurlHttpClientHandler : HttpMessageHandler
	{
		protected override Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.Run (() => Send (request, cancellationToken), cancellationToken);
		}

		HttpResponseMessage Send (HttpRequestMessage request, CancellationToken cancellationToken)
		{
			HttpResponseMessage response = null;
			Curl.Easy easy = null;

			easy = new Curl.Easy {
				Protocols = Protocols.Http | Protocols.Https,
				Url = request.RequestUri.AbsoluteUri,
				AutoReferer = true,
				FollowLocation = true,
				HttpMethod = request.Method.Method,
				HttpVersion = request.Version,
				HeaderHandler = (kind, name, value) => {
					switch (kind) {
					case HeaderKind.Status:
						response.StatusCode = (HttpStatusCode)easy.ResponseCode;
						break;
					case HeaderKind.KeyValue:
						response.Headers.TryAddWithoutValidation (name, value);
						response.Content.Headers.TryAddWithoutValidation (name, value);
						break;
					}
				}
			};

			var responseStream = new CurlResponseStream (easy);
			cancellationToken.Register (responseStream.Dispose);

			response = new HttpResponseMessage {
				Content = new StreamContent (responseStream)
			};

			return response;
		}
	}
}