using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using CurlThin.Enums;
using CurlThin.Native;

namespace CurlThin.Samples.Native
{
    internal class PostSample : ISample
    {
        public void Run()
        {
            DllLoader.Init();

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
                if (easy != IntPtr.Zero)
                {
                    CurlNative.Easy.Cleanup(easy);
                }

                if (global == CURLcode.OK)
                {
                    CurlNative.Cleanup();
                }
            }
        }
    }
}