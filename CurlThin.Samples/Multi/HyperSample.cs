using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CurlThin.Enums;
using CurlThin.HyperPipe;
using CurlThin.SafeHandles;

namespace CurlThin.Samples.Multi
{
    internal class HyperSample : ISample
    {
        public void Run()
        {
            if (CurlNative.Init() != CURLcode.OK)
            {
                throw new Exception("Could not init curl");
            }

            var reqProvider = new MyRequestProvider();
            var resConsumer = new MyResponseConsumer();

            using (var pipe = new HyperPipe<MyRequestContext>(4, reqProvider, resConsumer))
            {
                pipe.RunLoopWait();
            }
        }
    }

    public class DataCallbackCopier : IDisposable
    {
        public DataCallbackCopier()
        {
            DataHandler = (data, size, nmemb, userdata) =>
            {
                var length = (int) size * (int) nmemb;
                var buffer = new byte[length];
                Marshal.Copy(data, buffer, 0, length);
                Stream.Write(buffer, 0, length);
                return (UIntPtr) length;
            };
        }

        public MemoryStream Stream { get; } = new MemoryStream();

        public CurlNative.Easy.DataHandler DataHandler { get; }

        public void Dispose()
        {
            Stream?.Dispose();
        }

        public string ReadAsString()
        {
            Stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(Stream);
            return reader.ReadToEnd();
        }
    }

    /// <summary>
    ///     What exactly is request context? It can be any type (string, int, custom class, whatever) that will help pass some
    ///     data to method that will process response.
    /// </summary>
    public class MyRequestContext : IDisposable
    {
        public MyRequestContext(string label)
        {
            Label = label;
        }

        public string Label { get; }
        public DataCallbackCopier HeaderData { get; } = new DataCallbackCopier();
        public DataCallbackCopier ContentData { get; } = new DataCallbackCopier();

        public void Dispose()
        {
            HeaderData?.Dispose();
            ContentData?.Dispose();
        }
    }

    /// <summary>
    ///     Request provider generates requests that you want to send to cURL.
    ///     This example shows how to web scrape StackOverflow questions (https://stackoverflow.com/)
    ///     beginning with ID 4400000 until ID 4400050.
    /// </summary>
    public class MyRequestProvider : IRequestProvider<MyRequestContext>
    {
        private readonly int _maxQuestion = 4400050;
        private int _currentQuestion = 4400000;

        public MyRequestContext Current { get; private set; }

        public ValueTask<bool> MoveNextAsync(SafeEasyHandle easy)
        {
            // If question ID is higher than maximum, return false.
            if (_currentQuestion > _maxQuestion)
            {
                Current = null;
                return new ValueTask<bool>(false);
            }

            // Create request context. Assign it a label to easily recognize it later.
            var context = new MyRequestContext($"StackOverflow Question #{_currentQuestion}");

            // Set request URL.
            CurlNative.Easy.SetOpt(easy, CURLoption.URL, $"http://stackoverflow.com/questions/{_currentQuestion}/");

            // Follow redirects.
            CurlNative.Easy.SetOpt(easy, CURLoption.FOLLOWLOCATION, 1);

            // Set request timeout.
            CurlNative.Easy.SetOpt(easy, CURLoption.TIMEOUT_MS, 3000);

            // Copy response header (it contains HTTP code and response headers, for example
            // "Content-Type") to MemoryStream in our RequestContext.
            CurlNative.Easy.SetOpt(easy, CURLoption.HEADERFUNCTION, context.HeaderData.DataHandler);

            // Copy response body (it for example contains HTML source) to MemoryStream
            // in our RequestContext.
            CurlNative.Easy.SetOpt(easy, CURLoption.WRITEFUNCTION, context.ContentData.DataHandler);

            _currentQuestion++;
            Current = context;
            return new ValueTask<bool>(true);
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
            var html = context.ContentData.ReadAsString();

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