using System;
using System.Text;
using CurlThin.Enums;
using CurlThin.Helpers;
using CurlThin.SafeHandles;

namespace CurlThin.Samples.Easy
{
    internal class HttpHeadersSample : ISample
    {
        public void Run()
        {
            // curl_global_init() with default flags.
            var global = CurlNative.Init();

            // curl_easy_init() to create easy handle.
            var easy = CurlNative.Easy.Init();
            try
            {
                var dataCopier = new DataCallbackCopier();

                CurlNative.Easy.SetOpt(easy, CURLoption.URL, "http://httpbin.org/headers");
                CurlNative.Easy.SetOpt(easy, CURLoption.WRITEFUNCTION, dataCopier.DataHandler);

                // Initialize HTTP header list with first value.
                var headers = CurlNative.Slist.Append(SafeSlistHandle.Null, "X-Foo: Bar");
                // Add one more value to existing HTTP header list.
                CurlNative.Slist.Append(headers, "X-Qwerty: Asdfgh");

                // Configure libcurl easy handle to send HTTP headers we configured.
                CurlNative.Easy.SetOpt(easy, CURLoption.HTTPHEADER, headers.DangerousGetHandle());

                var result = CurlNative.Easy.Perform(easy);

                // Cleanup HTTP header list after request has complete.
                CurlNative.Slist.FreeAll(headers);

                Console.WriteLine($"Result code: {result}.");
                Console.WriteLine();
                Console.WriteLine("Response body:");
                Console.WriteLine(Encoding.UTF8.GetString(dataCopier.Stream.ToArray()));
            }
            finally
            {
                easy.Dispose();

                if (global == CURLcode.OK)
                {
                    CurlNative.Cleanup();
                }
            }
        }
    }
}