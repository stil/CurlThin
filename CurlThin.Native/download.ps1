function DownloadBintray
{
    $prefix = "https://bintray.com/vszakats/generic/download_file?file_path="
    $withExt = $args[0]+".7z"
    Invoke-WebRequest $prefix$withExt -OutFile $withExt
    7z.exe x $withExt
    Remove-Item $withExt
}

foreach ($arch in @("win32","win64")) {
    md $arch

    DownloadBintray "curl-7.54.0-$arch-mingw"
    cp "curl-7.54.0-$arch-mingw\bin\*.dll" $arch
    cp "curl-7.54.0-$arch-mingw\bin\*.crt" $arch

    DownloadBintray "openssl-1.1.0e-$arch-mingw"
    cp "openssl-1.1.0e-$arch-mingw\*.dll" $arch
}
