# CurlThin #
[![Gitter](https://img.shields.io/gitter/room/CurlThin/Lobby.svg)](https://gitter.im/CurlThin/Lobby)


_CurlThin_ is a NET Standard compatible binding library against [libcurl](http://curl.haxx.se/libcurl).
It includes a modern wrapper for `curl_multi` interface which uses polling with [libuv](https://libuv.org/) library instead of using inefficient `select`.

_CurlThin_ has a very thin abstraction layer, which means that writing the code is as close as possible to writing purely in libcurl. libcurl has extensive documentation and relatively strong support of community and not having additional abstraction layer makes it easier to search solutions for your problems.

Using this library is very much like working with cURL's raw C API.

### License ###
Library is MIT licensed. NuGet icon made by [Freepik](http://www.freepik.com) and is licensed by [CC 3.0 BY](https://creativecommons.org/licenses/by/3.0/)

## Installation ##

| Package   | NuGet        | MyGet | Description  |
|-----------|--------------|-------|--------------|
| `CurlThin` | [![Nuget](https://img.shields.io/nuget/v/CurlThin.svg)](https://www.nuget.org/packages/CurlThin/) | ![MyGet](https://img.shields.io/myget/curlthin/vpre/CurlThin.svg) | The C# wrapper for libcurl.  |
| `CurlThin.Native` | [![Nuget](https://img.shields.io/nuget/v/CurlThin.Native.svg)](https://www.nuget.org/packages/CurlThin.Native/) | ![MyGet](https://img.shields.io/myget/curlthin/vpre/CurlThin.Native.svg) | Contains embedded libcurl native binaries for x86 and x64 Windows. |

If you have `libcurl` or `libcurl.dll` already in your PATH directory, you don't need to install `CurlThin.Native` package. Once you have installed `CurlThin.Native` NuGet package, call following method just once before you use cURL:

```csharp
CurlResources.Init();
```

It will extract following files to your application output directory

| Windows x86 | Windows x64 | Description |
|-------------|-------------|-------------|
| libcurl.dll | libcurl.dll | The multiprotocol file transfer library. |
| libssl-1_1.dll | libssl-1_1-x64.dll | Portion of OpenSSL which supports TLS ( SSL and TLS Protocols), and depends on libcrypto. |
| libcrypto-1_1.dll | libcrypto-1_1-x64.dll | Provides the fundamental cryptographic routines used by libssl. |
| curl-ca-bundle.crt | curl-ca-bundle.crt | Certificate Authority (CA) bundle. You can use it via [`CURLOPT_CAINFO`](https://curl.haxx.se/libcurl/c/CURLOPT_CAINFO.html). |

## Examples ##

### Easy interface ###

#### GET request ####
```csharp
// curl_global_init() with default flags.
var global = CurlNative.Init();

// curl_easy_init() to create easy handle.
var easy = CurlNative.Easy.Init();
try
{
    CurlNative.Easy.SetOpt(easy, CURLoption.URL, "http://httpbin.org/ip");

    var stream = new MemoryStream();
    CurlNative.Easy.SetOpt(easy, CURLoption.WRITEFUNCTION, (data, size, nmemb, user) =>
    {
        var length = (int) size * (int) nmemb;
        var buffer = new byte[length];
        Marshal.Copy(data, buffer, 0, length);
        stream.Write(buffer, 0, length);
        return (UIntPtr) length;
    });

    var result = CurlNative.Easy.Perform(easy);

    Console.WriteLine($"Result code: {result}.");
    Console.WriteLine();
    Console.WriteLine("Response body:");
    Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));
}
finally
{
    easy.Dispose();

    if (global == CURLcode.OK)
    {
        CurlNative.Cleanup();
    }
}
```


#### POST request ####
```csharp
// curl_global_init() with default flags.
var global = CurlNative.Init();

// curl_easy_init() to create easy handle.
var easy = CurlNative.Easy.Init();
try
{
    var postData = "fieldname1=fieldvalue1&fieldname2=fieldvalue2";

    CurlNative.Easy.SetOpt(easy, CURLoption.URL, "http://httpbin.org/post");

    // This one has to be called before setting COPYPOSTFIELDS.
    CurlNative.Easy.SetOpt(easy, CURLoption.POSTFIELDSIZE, Encoding.ASCII.GetByteCount(postData));
    CurlNative.Easy.SetOpt(easy, CURLoption.COPYPOSTFIELDS, postData);
    
    var stream = new MemoryStream();
    CurlNative.Easy.SetOpt(easy, CURLoption.WRITEFUNCTION, (data, size, nmemb, user) =>
    {
        var length = (int) size * (int) nmemb;
        var buffer = new byte[length];
        Marshal.Copy(data, buffer, 0, length);
        stream.Write(buffer, 0, length);
        return (UIntPtr) length;
    });

    var result = CurlNative.Easy.Perform(easy);

    Console.WriteLine($"Result code: {result}.");
    Console.WriteLine();
    Console.WriteLine("Response body:");
    Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));
}
finally
{
    easy.Dispose();

    if (global == CURLcode.OK)
    {
        CurlNative.Cleanup();
    }
}
```

### Multi interface ###

#### Web scrape StackOverflow questions ####
See [Multi/HyperSample.cs](CurlThin.Samples/Multi/HyperSample.cs).
