import os
import argparse
import subprocess


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--version-prefix', required=True)
    parser.add_argument('--version-suffix')
    parser.add_argument('--native-version-prefix', required=True)
    parser.add_argument('--native-version-suffix')
    args = parser.parse_args()

    current_dir = os.path.dirname(os.path.abspath(__file__))

    build_csproj(
        os.path.join(current_dir, 'CurlThin', 'CurlThin.csproj'),
        args.version_prefix,
        args.version_suffix,
    )

    build_csproj(
        os.path.join(current_dir, 'CurlThin.Native', 'CurlThin.Native.csproj'),
        args.native_version_prefix,
        args.native_version_suffix,
    )


def build_csproj(csproj, version_prefix, version_suffix):
    build_args = [
        '/p:Configuration=Release',
        '/p:VersionPrefix=' + version_prefix
    ]

    if version_suffix is not None:
        build_args.append('/p:VersionSuffix=' + version_suffix)

    subprocess.run(['dotnet', 'clean', csproj])
    subprocess.run(['dotnet', 'restore', csproj])
    subprocess.run(['dotnet', 'build', csproj] + build_args)
    subprocess.run(['dotnet', 'pack', csproj] + build_args)


if __name__ == '__main__':
    main()
