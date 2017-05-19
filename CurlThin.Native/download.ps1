function DownloadBintray
{
    $prefix = "https://bintray.com/vszakats/generic/download_file?file_path="
    $withExt = $args[0]+".7z"
    Invoke-WebRequest $prefix$withExt -OutFile $withExt
    7z.exe x $withExt
    Remove-Item $withExt
}

function ZipFiles( $zipfilename, $sourcedir )
{
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,
        $zipfilename, $compressionLevel, $false)
}

md -Force "Resources"
cd "Resources"

foreach ($arch in @("win32","win64")) {
    md $arch

    DownloadBintray "curl-7.54.0-$arch-mingw"
    cp "curl-7.54.0-$arch-mingw\bin\*.dll" $arch
    cp "curl-7.54.0-$arch-mingw\bin\*.crt" "."

    DownloadBintray "openssl-1.1.0e-$arch-mingw"
    cp "openssl-1.1.0e-$arch-mingw\*.dll" $arch
}

cd ..
ZipFiles -sourcedir "Resources" -zipfilename "Resources.zip"
