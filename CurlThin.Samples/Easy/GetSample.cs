using System;
using System.Text;
using CurlThin.Enums;
using CurlThin.Helpers;

namespace CurlThin.Samples.Easy
{
    internal class GetSample : ISample
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

                CurlNative.Easy.SetOpt(easy, CURLoption.URL, "http://httpbin.org/ip");
                CurlNative.Easy.SetOpt(easy, CURLoption.WRITEFUNCTION, dataCopier.DataHandler);

                var result = CurlNative.Easy.Perform(easy);

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