# <FUNCTIONS>
function DownloadBintray( $OutDir = '.' )
{
    $packageName = $args[0];
    $prefix = "https://bintray.com/vszakats/generic/download_file?file_path="
    $archiveName = $packageName + ".7z"

    Invoke-WebRequest $prefix$archiveName -OutFile $OutDir\$archiveName
    7z.exe x $OutDir\$archiveName "-o$OutDir"
    Remove-Item $OutDir\$archiveName
}

function ZipFiles( $zipfilename, $sourcedir )
{
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,
        $zipfilename, $compressionLevel, $false)
}
# </FUNCTIONS>

# <VARIABLES>
$downloadDir = "$PSScriptRoot/Download"
$resourcesDir = "$PSScriptRoot/Resources"
$curlVersion = "7.54.0"
$opensslVersion = "1.1.0e"
# </VARIABLES>

# <SCRIPT>
md -Force $downloadDir
md -Force $resourcesDir

foreach ($arch in @("win32","win64")) {
    md -Force $resourcesDir\$arch

    DownloadBintray "curl-$curlVersion-$arch-mingw" -OutDir $downloadDir
    cp "$downloadDir\curl-$curlVersion-$arch-mingw\bin\*.dll" $resourcesDir\$arch
    cp "$downloadDir\curl-$curlVersion-$arch-mingw\bin\*.crt" $resourcesDir

    DownloadBintray "openssl-$opensslVersion-$arch-mingw" -OutDir $downloadDir
    cp "$downloadDir\openssl-$opensslVersion-$arch-mingw\*.dll" $resourcesDir\$arch
}

ZipFiles -sourcedir $resourcesDir -zipfilename "$resourcesDir.zip"
# </SCRIPT>
