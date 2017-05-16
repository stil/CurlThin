using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CurlThin.Native
{
    public class DllLoader
    {
        public static void Init()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                switch (RuntimeInformation.OSArchitecture)
                {
                    case Architecture.X64:
                        SetDllDirectory(Path.Combine(AssemblyDirectory, "win64"));
                        break;
                    case Architecture.X86:
                        SetDllDirectory(Path.Combine(AssemblyDirectory, "win32"));
                        break;
                }
            }
        }

        private static string AssemblyDirectory
        {
            get
            {
                var codeBase = typeof(DllLoader).GetTypeInfo().Assembly.CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);
    }
}