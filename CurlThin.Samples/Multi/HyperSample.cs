using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CurlThin.Enums;
using CurlThin.HyperPipe;
using CurlThin.Native;
using CurlThin.SafeHandles;

namespace CurlThin.Samples.Multi
{
    internal class HyperSample : ISample
    {
        public void Run()
        {
            DllLoader.Init();
            if (CurlNative.Init() != CURLcode.OK)
            {
                throw new Exception("Could not init curl");
            }

            var reqProvider = new MyRequestProvider();
            var resConsumer = new MyResponseConsumer();

            using (var pipe = new HyperPipe<MyRequestContext>(6, reqProvider, resConsumer))
            {
                pipe.RunLoopWait();
            }
        }
    }

    /// <summary>
    ///     What exactly is request context? It can be any type (string, int, custom class, whatever) that will help pass some
    ///     data to method that will process response.
    /// </summary>
    public class MyRequestContext : IDisposable
    {
        public string Label { get; set; }
        public MemoryStream HeaderStream { get; } = new MemoryStream();
        public MemoryStream ContentStream { get; } = new MemoryStream();

        public CurlNative.Easy.DataHandler HeaderFunction { get; set; }
        public CurlNative.Easy.DataHandler ContentFunction { get; set; }

        public void Dispose()
        {
            HeaderStream?.Dispose();
            ContentStream?.Dispose();
        }
    }

    /// <summary>
    ///     Request provider generates requests that you want to send to cURL.
    ///     This example shows how to web scrape StackOverflow questions (https://stackoverflow.com/)
    ///     beginning with ID 4400000 until ID 4400100.
    /// </summary>
    public class MyRequestProvider : IRequestProvider<MyRequestContext>
    {
        private readonly int _maxQuestion = 4400100;
        private int _currentQuestion = 4400000;

        public bool TryNext(SafeEasyHandle easy, out MyRequestContext context)
        {
            // If question ID is higher than maximum, return false.
            if (_currentQuestion > _maxQuestion)
            {
                context = null;
                return false;
            }

            // Create request context. Assign it a label to easily recognize it later.
            var contextLocal = new MyRequestContext
            {
                Label = $"StackOverflow Question #{_currentQuestion}"
            };

            // Copy response header (it contains HTTP code and response headers, for example
            // "Content-Type") to MemoryStream in our RequestContext.
            context = contextLocal;
            context.HeaderFunction = (data, size, nmemb, userdata) =>
            {
                var length = (int) size * (int) nmemb;
                var buffer = new byte[length];
                Marshal.Copy(data, buffer, 0, length);
                contextLocal.HeaderStream.Write(buffer, 0, length);
                return (UIntPtr) length;
            };

            // Copy response body (it for example contains HTML source) to MemoryStream
            // in our RequestContext.
            context.ContentFunction = (data, size, nmemb, userdata) =>
            {
                var length = (int) size * (int) nmemb;
                var buffer = new byte[length];
                Marshal.Copy(data, buffer, 0, length);
                contextLocal.ContentStream.Write(buffer, 0, length);
                return (UIntPtr) length;
            };

            // Set request URL.
            CurlNative.Easy.SetOpt(easy, CURLoption.URL, $"http://stackoverflow.com/questions/{_currentQuestion}/");

            // Follow redirects.
            CurlNative.Easy.SetOpt(easy, CURLoption.FOLLOWLOCATION, 1);
            CurlNative.Easy.SetOpt(easy, CURLoption.HEADERFUNCTION, context.HeaderFunction);
            CurlNative.Easy.SetOpt(easy, CURLoption.WRITEFUNCTION, context.ContentFunction);

            _currentQuestion++;
            return true;
        }
    }

    /// <summary>
    ///     This class will process HTTP responses.
    /// </summary>
    public class MyResponseConsumer : IResponseConsumer<MyRequestContext>
    {
        public HandleCompletedAction OnComplete(SafeEasyHandle easy, MyRequestContext context, CURLcode errorCode)
        {
            Console.WriteLine($"Request label: {context.Label}.");
            if (errorCode != CURLcode.OK)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine($"cURL error code: {errorCode}");
                var pErrorMsg = CurlNative.Easy.StrError(errorCode);
                var errorMsg = Marshal.PtrToStringAnsi(pErrorMsg);
                Console.WriteLine($"cURL error message: {errorMsg}");

                Console.ResetColor();
                Console.WriteLine("--------");
                Console.WriteLine();

                context.Dispose();
                return HandleCompletedAction.ResetHandleAndNext;
            }

            // Get HTTP response code.
            CurlNative.Easy.GetInfo(easy, CURLINFO.RESPONSE_CODE, out int httpCode);
            if (httpCode != 200)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine($"Invalid HTTP response code: {httpCode}");

                Console.ResetColor();
                Console.WriteLine("--------");
                Console.WriteLine();

                context.Dispose();
                return HandleCompletedAction.ResetHandleAndNext;
            }
            Console.WriteLine($"Response code: {httpCode}");

            // Get effective URL.
            IntPtr pDoneUrl;
            CurlNative.Easy.GetInfo(easy, CURLINFO.EFFECTIVE_URL, out pDoneUrl);
            var doneUrl = Marshal.PtrToStringAnsi(pDoneUrl);
            Console.WriteLine($"Effective URL: {doneUrl}");

            // Get response body as string.
            string html;
            context.ContentStream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(context.ContentStream))
            {
                html = reader.ReadToEnd();
            }

            // Scrape question from HTML source.
            var match = Regex.Match(html, "<title>(.+?)<\\/");

            Console.WriteLine($"Question: {match.Groups[1].Value.Trim()}");
            Console.WriteLine("--------");
            Console.WriteLine();

            context.Dispose();
            return HandleCompletedAction.ResetHandleAndNext;
        }
    }
}