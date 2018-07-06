using System;
using System.Runtime.InteropServices;
using CurlThin.Enums;
using CurlThin.SafeHandles;

namespace CurlThin
{
    /// <remarks>
    ///     Type mappings (C -> C#):
    ///     - size_t -> UIntPtr
    ///     - int    -> int
    ///     - long   -> int
    /// </remarks>
    public static class CurlNative
    {
        private const string LIBCURL = "libcurl";

        [DllImport(LIBCURL, EntryPoint = "curl_global_init")]
        public static extern CURLcode Init(CURLglobal flags = CURLglobal.DEFAULT);

        [DllImport(LIBCURL, EntryPoint = "curl_global_cleanup")]
        public static extern void Cleanup();

        public static class Easy
        {
            public delegate UIntPtr DataHandler(IntPtr data, UIntPtr size, UIntPtr nmemb, IntPtr userdata);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_init")]
            public static extern SafeEasyHandle Init();

            [DllImport(LIBCURL, EntryPoint = "curl_easy_cleanup")]
            public static extern void Cleanup(IntPtr handle);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_perform")]
            public static extern CURLcode Perform(SafeEasyHandle handle);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_reset")]
            public static extern void Reset(SafeEasyHandle handle);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_setopt")]
            public static extern CURLcode SetOpt(SafeEasyHandle handle, CURLoption option, int value);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_setopt")]
            public static extern CURLcode SetOpt(SafeEasyHandle handle, CURLoption option, IntPtr value);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_setopt", CharSet = CharSet.Ansi)]
            public static extern CURLcode SetOpt(SafeEasyHandle handle, CURLoption option, string value);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_setopt")]
            public static extern CURLcode SetOpt(SafeEasyHandle handle, CURLoption option, DataHandler value);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_getinfo")]
            public static extern CURLcode GetInfo(SafeEasyHandle handle, CURLINFO option, out int value);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_getinfo")]
            public static extern CURLcode GetInfo(SafeEasyHandle handle, CURLINFO option, out IntPtr value);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_getinfo")]
            public static extern CURLcode GetInfo(SafeEasyHandle handle, CURLINFO option, out double value);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_getinfo", CharSet = CharSet.Ansi)]
            public static extern CURLcode GetInfo(SafeEasyHandle handle, CURLINFO option, IntPtr value);

            [DllImport(LIBCURL, EntryPoint = "curl_easy_strerror")]
            public static extern IntPtr StrError(CURLcode errornum);
        }

        public static class Multi
        {
            [DllImport(LIBCURL, EntryPoint = "curl_multi_init")]
            public static extern SafeMultiHandle Init();

            [DllImport(LIBCURL, EntryPoint = "curl_multi_cleanup")]
            public static extern CURLMcode Cleanup(IntPtr multiHandle);

            [DllImport(LIBCURL, EntryPoint = "curl_multi_add_handle")]
            public static extern CURLMcode AddHandle(SafeMultiHandle multiHandle, SafeEasyHandle easyHandle);

            [DllImport(LIBCURL, EntryPoint = "curl_multi_remove_handle")]
            public static extern CURLMcode RemoveHandle(SafeMultiHandle multiHandle, SafeEasyHandle easyHandle);

            [DllImport(LIBCURL, EntryPoint = "curl_multi_setopt")]
            public static extern CURLMcode SetOpt(SafeMultiHandle multiHandle, CURLMoption option, int value);

            [DllImport(LIBCURL, EntryPoint = "curl_multi_info_read")]
            public static extern IntPtr InfoRead(SafeMultiHandle multiHandle, out int msgsInQueue);

            [DllImport(LIBCURL, EntryPoint = "curl_multi_socket_action")]
            public static extern CURLMcode SocketAction(SafeMultiHandle multiHandle, SafeSocketHandle sockfd,
                CURLcselect evBitmask,
                out int runningHandles);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct CURLMsg
            {
                public CURLMSG msg; /* what this message means */
                public IntPtr easy_handle; /* the handle it concerns */
                public CURLMsgData data;

                [StructLayout(LayoutKind.Explicit)]
                public struct CURLMsgData
                {
                    [FieldOffset(0)] public IntPtr whatever; /* (void*) message-specific data */
                    [FieldOffset(0)] public CURLcode result; /* return code for transfer */
                }
            }

            #region curl_multi_setopt for CURLMOPT_TIMERFUNCTION

            public delegate int TimerCallback(IntPtr multiHandle, int timeoutMs, IntPtr userp);

            [DllImport(LIBCURL, EntryPoint = "curl_multi_setopt")]
            public static extern CURLMcode SetOpt(SafeMultiHandle multiHandle, CURLMoption option, TimerCallback value);

            #endregion

            #region curl_multi_setopt for CURLMOPT_SOCKETFUNCTION

            public delegate int SocketCallback(IntPtr easy, IntPtr s, CURLpoll what, IntPtr userp, IntPtr socketp);

            [DllImport(LIBCURL, EntryPoint = "curl_multi_setopt")]
            public static extern CURLMcode SetOpt(SafeMultiHandle multiHandle, CURLMoption option,
                SocketCallback value);

            #endregion
        }

        public static class Slist
        {
            [DllImport(LIBCURL, EntryPoint = "curl_slist_append")]
            public static extern SafeSlistHandle Append(SafeSlistHandle slist, string data);
            
            [DllImport(LIBCURL, EntryPoint = "curl_slist_free_all")]
            public static extern void FreeAll(SafeSlistHandle pList);
        }
    }
}