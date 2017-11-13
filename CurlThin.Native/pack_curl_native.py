import glob
import os
import shutil
import subprocess
import urllib.request
import argparse


def find_7z() -> str:
    """
    Searches for 7-Zip executable on system.
    :return: Path to 7z executable.
    """

    def is_exe(fpath):
        return os.path.isfile(fpath) and os.access(fpath, os.X_OK)

    if is_exe('7z'):
        return '7z'

    if is_exe('7z.exe'):
        return '7z.exe'

    if 'PROGRAMFILES' in os.environ:
        path = os.path.join(os.environ['PROGRAMFILES'], '7-Zip', '7z.exe')
        if is_exe(path):
            return path

    if 'PROGRAMFILES(X86)' in os.environ:
        path = os.path.join(os.environ['PROGRAMFILES(X86)'], '7-Zip', '7z.exe')
        if is_exe(path):
            return path

    raise Exception('Cannot find 7-Zip executable.')


def prepare_empty_dir(path: str):
    """
    Creates an empty dir or deletes its contents if already exists.
    :param path: Path to directory.
    """
    if os.path.exists(path):
        # Already exists, clear its whole contents.
        shutil.rmtree(path)

    os.makedirs(path)


def cp(src_glob: str, dst_folder: str):
    """
    Non-recursively copies files from one dir to another.
    :param src_glob: Source dir path, supports wildcard patterns.
    :param dst_folder: Destination directory.
    """
    for path in glob.iglob(src_glob):
        shutil.copy(path, os.path.join(dst_folder, os.path.basename(path)))


def download_bintray(package_name: str, out_dir: str, exe_7z: str):
    """
    Downloads and unpacks package from vszakats/generic bintray repo.
    :param package_name: Package name.
    :param out_dir: Output directory.
    :param exe_7z: Path to 7z executable
    """
    file_name = package_name + '.7z'
    url = 'https://bintray.com/vszakats/generic/download_file?file_path=' + file_name
    full_out_path = os.path.join(out_dir, file_name)

    print('Downloading ' + package_name + '... ', end='', flush=True)
    urllib.request.urlretrieve(url, full_out_path)
    print('OK')

    print('Unpacking ' + file_name + '... ', end='', flush=True)
    subprocess.run([exe_7z, 'x', full_out_path, '-o' + out_dir])
    print('OK')

    os.remove(full_out_path)
    print('Deleted ' + file_name)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--libcurl-version', required=True, help='Specify libcurl version')
    parser.add_argument('--openssl-version', required=True, help='Specify openssl version')
    parser.add_argument('--clean', action='store_true', help='Do cleanup afterwards')
    args = parser.parse_args()

    exe_7z = find_7z()
    current_dir = os.path.dirname(os.path.abspath(__file__))
    build_dir = os.path.join(current_dir, 'build')
    download_dir = os.path.join(build_dir, 'Download')
    resources_dir = os.path.join(build_dir, 'Resources')
    resources_zip = os.path.join(current_dir, 'Resources.zip')

    prepare_empty_dir(build_dir)
    if os.path.isfile(resources_zip):
        os.remove(resources_zip)
    os.makedirs(download_dir)
    os.makedirs(resources_dir)

    for arch in ('win32', 'win64'):
        cur_res_dir = os.path.join(resources_dir, arch)
        os.makedirs(cur_res_dir)

        cur_pkg = f'curl-{args.libcurl_version}-{arch}-mingw'
        download_bintray(cur_pkg, download_dir, exe_7z)
        cp(os.path.join(download_dir, cur_pkg, 'bin', '*.dll'), cur_res_dir)
        cp(os.path.join(download_dir, cur_pkg, 'bin', '*.crt'), cur_res_dir)

        if arch == 'win64':
            libcurlx64 = os.path.join(cur_res_dir, 'libcurl-x64.dll')
            if os.path.isfile(libcurlx64):
                os.rename(libcurlx64, os.path.join(cur_res_dir, 'libcurl.dll'))

        cur_pkg = f'openssl-{args.openssl_version}-{arch}-mingw'
        download_bintray(cur_pkg, download_dir, exe_7z)
        cp(os.path.join(download_dir, cur_pkg, '*.dll'), cur_res_dir)

    shutil.make_archive(os.path.splitext(resources_zip)[0], 'zip', resources_dir)

    if args.clean:
        shutil.rmtree(build_dir)


if __name__ == '__main__':
    main()
