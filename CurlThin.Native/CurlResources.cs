using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CurlThin.Native
{
    public static class CurlResources
    {
        private const string CaBundleFilename = "curl-ca-bundle.crt";

        private static readonly DirectoryInfo OutputDir;

        private static readonly FileInfo CaBundleFile;

        static CurlResources()
        {
            OutputDir = new DirectoryInfo(Assembly.GetEntryAssembly().Location).Parent;
            CaBundleFile = new FileInfo(Path.Combine(OutputDir.FullName, CaBundleFilename));
        }

        public static string CaBundlePath
        {
            get
            {
                if (!CaBundleFile.Exists)
                {
                    throw new FileNotFoundException($"{CaBundleFilename} is missing.");
                }
                return CaBundleFile.FullName;
            }
        }

        public static void Init()
        {
            ExtractResource(CaBundleFilename, OutputDir);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SetDllDirectory(OutputDir.FullName);
                ExtractResource(Environment.Is64BitProcess ? "win64/*.dll" : "win32/*.dll", OutputDir);
            }
        }

        private static void ExtractResource(string resourcePath, DirectoryInfo outputDir)
        {
            var assembly = typeof(CurlResources).GetTypeInfo().Assembly;

            // We keep our resources in ZIP file to cut assembly size.
            // Also, ZIP archive retains file modification timestamps.
            var zipResource = assembly.GetManifestResourceNames()
                .First(name => name.Like("Resources.zip"));

            using (var zipStream = assembly.GetManifestResourceStream(zipResource))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries.Where(e => e.FullName.Replace('\\', '/').Like(resourcePath)))
                {
                    var outputPath = new FileInfo(Path.Combine(outputDir.FullName, entry.Name));

                    // Extract if not exist or overwrite if filesizes mismatch.
                    if (!outputPath.Exists || outputPath.Length != entry.Length)
                    {
                        entry.ExtractToFile(outputPath.FullName, true);
                    }
                }
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);
    }
}