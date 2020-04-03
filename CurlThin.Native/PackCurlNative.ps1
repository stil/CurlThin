function GetLibcurlArch($arch, $version)
{
    Invoke-WebRequest `
        "https://bintray.com/artifact/download/vszakats/generic/curl-$version-$arch-mingw.zip" `
        -OutFile "curl-$arch-mingw.zip"

    Expand-Archive -Path "curl-$arch-mingw.zip" -DestinationPath "."
    Remove-Item "curl-$arch-mingw.zip"

    if ($arch -eq "win32")
    {
        $libcurlDll = "libcurl.dll"
    }
    else
    {
        $libcurlDll = "libcurl-x64.dll"
    }

    Copy-Item -Path "curl-$version-$arch-mingw\bin\$libcurlDll" -Destination "$arch\libcurl.dll"
    Copy-Item -Path "curl-$version-$arch-mingw\bin\*.crt" -Destination "$arch\"
    Remove-Item -Recurse "curl-$version-$arch-mingw"
}

function GetOpensslArch($arch, $version)
{
    Invoke-WebRequest `
        "https://bintray.com/artifact/download/vszakats/generic/openssl-$version-$arch-mingw.zip" `
        -OutFile "openssl-$arch-mingw.zip"

    Expand-Archive -Path "openssl-$arch-mingw.zip" -DestinationPath "."
    Remove-Item "openssl-$arch-mingw.zip"
    Copy-Item -Path "openssl-$version-$arch-mingw\*.dll" -Destination "$arch\"
    Remove-Item -Recurse "openssl-$version-$arch-mingw"
}

New-Item -Name win64 -ItemType "directory"
New-Item -Name win32 -ItemType "directory"

GetLibcurlArch win64 "7.69.1"
GetLibcurlArch win32 "7.69.1"

GetOpensslArch win64 "1.1.1f"
GetOpensslArch win32 "1.1.1f"

$compress = @{
    Path = "win64", "win32"
    CompressionLevel = "Optimal"
    DestinationPath = "Resources.zip"
}

Compress-Archive @compress

Remove-Item -Recurse win64
Remove-Item -Recurse win32