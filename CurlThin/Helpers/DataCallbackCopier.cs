using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CurlThin.Helpers
{
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
        
        public void Reset()
        {
            Stream.Position = 0;
            Stream.SetLength(0);
        }

        public string ReadAsString()
        {
            Stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(Stream);
            return reader.ReadToEnd();
        }
    }
}