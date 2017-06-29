using System;
using System.Text;
using CurlThin.Enums;
using CurlThin.Helpers;

namespace CurlThin.Samples.Easy
{
    internal class PostSample : ISample
    {
        public void Run()
        {
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

                var dataCopier = new DataCallbackCopier();
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