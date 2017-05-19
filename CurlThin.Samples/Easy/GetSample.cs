using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using CurlThin.Enums;

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
        }
    }
}