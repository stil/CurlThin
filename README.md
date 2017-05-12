# CurlSharp #

_CurlSharp_ is a modern C# (5.0/async) binding against
[libcurl](http://curl.haxx.se/libcurl) with a
`System.Net.Http.HttpMessageHandler` implementation for
use with .NET 4.5's `System.Net.Http.HttpClient`.

## Incomplete, Barely Tested, Proof of Concept ##

_CurlSharp_ is a brand new project hacked on in spare time (started Jan 7,
2014). It's sort of an itch scratcher that may prove genuinely useful at
some point (e.g. to avoid using the default `System.Net.HttpWebRequest`
backend when using `System.Net.Http.HttpClient`, which has its own set
of issues).

As such, do not use it in production. You will probably find that much
is missing. For example, `POST` isn't supported. That's useful.

Please do try it out and file issues on Github, however!

## Classes ##

### `CurlHttpClientHandler` ###

This class derives from `System.Net.Http.HttpMessageHandler` and can be
used with `System.Net.Http.HttpClient` to provide a cURL-powered HTTP
experience in .NET. This is the easiest and recommended way to use this
library.

```csharp
var client = new HttpClient(new CurlHttpClientHandler());
var cats = client.GetStringAsync("http://catoverflow.com/api/query").Result;
Console.WriteLine(cats);
```

### `CurlEasy` ###

This class implements a wrist and code completion friendly binding over
the cURL `easy` API, which is the core API for interacting with cURL. It
is a blocking and single-threaded API.

```csharp
using (var stdout = Console.OpenStandardOutput())
{
    using (var easy = new CurlEasy())
    {
        easy.Url = "http://catoverflow.com/api/query";
        easy.WriteHandler = buffer => stdout.Write(buffer, 0, buffer.Length);
        easy.Perform();
    }
    stdout.Flush();
}
```

### `Multi` ###

This class implements a wrist and code completion friendly binding over
the cURL `multi` API, which provides facilities for non-blocking IO and
sits atop the cURL `easy` API. It supports `select` operations on `fd_set`
handles provided by cURL for efficient IO, though using this API is optional.

```csharp
using (var stdout = Console.OpenStandardOutput ()) {
	using (var multi = new Multi {
		new Easy {
			Url = "http://catoverflow.com/api/query",
			WriteHandler = buffer => stdout.Write (buffer, 0, buffer.Length)
		}
	}) {
		do {
			multi.AutoPerformWithSelect ();
		} while (multi.HandlesRemaining > 0);
	}
	stdout.Flush ();
}
```

### `CurlNative` ###

This class provides an extremely thin P/Invoke layer over the actual
libcurl API. Using this API is very much like working with cURL's raw
C API.

```csharp
using (var stdout = Console.OpenStandardOutput())
{
    var easy = CurlNative.Easy.Init();
    try
    {
        CurlNative.Easy.SetOpt(easy, CURLoption.URL, "http://catoverflow.com/api/query");
        CurlNative.Easy.SetOpt(easy, CURLoption.WRITEFUNCTION, (data, size, nmemb, user) =>
        {
            var length = (int) size * (int) nmemb;
            var buffer = new byte[length];
            Marshal.Copy(data, buffer, 0, length);
            stdout.Write(buffer, 0, length);
            return (UIntPtr) length;
        });
        CurlNative.Easy.Perform(easy);
        stdout.Flush();
    }
    finally
    {
        if (easy != IntPtr.Zero)
        {
            CurlNative.Easy.Cleanup(easy);
        }
    }
}
```

### `CurlResponseStream` ###

This class provides the most .NET-familiar wrapper that does not integrate
into any real framework (e.g. like `CurlHttpClientHandler`). It derives
from `System.IO.Stream` and takes either a `Curl.Multi` and a `Curl.Easy`
or just a `Curl.Easy`.

It then consumes these objects to provide a standard .NET read-only stream
with efficient `select`-based IO. This class is used by `CurlHttpClientHandler`
to perform the bulk of its work.

```csharp
var easy = new Easy { Url = "http://catoverflow.com/api/query" };
using (var stream = new CurlResponseStream (easy))
	Console.WriteLine (new StreamReader (stream).ReadToEnd ());
```
