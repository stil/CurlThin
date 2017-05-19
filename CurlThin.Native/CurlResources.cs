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

        static CurlResources()
        {
            var codeBase = typeof(CurlResources).GetTypeInfo().Assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            AssemblyDirectory = Path.GetDirectoryName(path);
        }

        private static string AssemblyDirectory { get; }

        public static string CaBundlePath
        {
            get
            {
                var path = Path.Combine(AssemblyDirectory, CaBundleFilename);
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"{CaBundleFilename} is missing.");
                }
                return path;
            }
        }

        public static void Init()
        {
            CopyResourceToOutput(CaBundleFilename);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                switch (RuntimeInformation.OSArchitecture)
                {
                    case Architecture.X64:
                        CopyResourceToOutput("win64/*.dll");
                        break;
                    case Architecture.X86:
                        CopyResourceToOutput("win32/*.dll");
                        break;
                }
            }
        }

        private static void CopyResourceToOutput(string resourcePath)
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
                    var outputPath = new FileInfo(Path.Combine(AssemblyDirectory, entry.Name));

                    // Extract if not exist or overwrite if filesizes mismatch.
                    if (!outputPath.Exists || outputPath.Length != entry.Length)
                    {
                        entry.ExtractToFile(outputPath.FullName, true);
                    }
                }
            }
        }
    }
}