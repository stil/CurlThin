using System;
using System.Runtime.InteropServices;
using System.Threading;
using CurlThin.Enums;
using CurlThin.SafeHandles;
using NetUV.Core.Handles;
using NLog;
using Timer = NetUV.Core.Handles.Timer;

namespace CurlThin.HyperPipe
{
    public class HyperPipe<T> : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly EasyPool<T> _easyPool;
        private readonly Loop _loop;
        private readonly SafeMultiHandle _multiHandle;
        private readonly SemaphoreSlim _oneRequestPullAtOnce = new SemaphoreSlim(1);
        private readonly IRequestProvider<T> _requestProvider;
        private readonly IResponseConsumer<T> _responseConsumer;
        private readonly CurlNative.Multi.SocketCallback _socketCallback;
        private readonly SocketPollMap _socketMap;
        private readonly Timer _timeout;
        private readonly CurlNative.Multi.TimerCallback _timerCallback;

        public HyperPipe(int poolSize,
            IRequestProvider<T> requestProvider,
            IResponseConsumer<T> responseConsumer)
        {
            _requestProvider = requestProvider;
            _responseConsumer = responseConsumer;

            _easyPool = new EasyPool<T>(poolSize);
            _multiHandle = CurlNative.Multi.Init();
            if (_multiHandle.IsInvalid)
            {
                throw new Exception("Could not init curl_multi handle.");
            }

            _socketMap = new SocketPollMap();
            _loop = new Loop();
            _timeout = _loop.CreateTimer();

            // Explicitly define callback functions to keep them from being GCed.
            _socketCallback = HandleSocket;
            _timerCallback = StartTimeout;

            Logger.Debug($"Set {CURLMoption.SOCKETFUNCTION}.");
            CurlNative.Multi.SetOpt(_multiHandle, CURLMoption.SOCKETFUNCTION, _socketCallback);
            Logger.Debug($"Set {CURLMoption.TIMERFUNCTION}.");
            CurlNative.Multi.SetOpt(_multiHandle, CURLMoption.TIMERFUNCTION, _timerCallback);
        }

        public void Dispose()
        {
            _multiHandle.Dispose();
            _loop.Dispose();
            _timeout.Dispose();
            _socketMap.Dispose();
            _oneRequestPullAtOnce.Dispose();
        }

        public void RunLoopWait()
        {
            Refill();

            // Kickstart.
            Logger.Debug("Kickstarting...");
            CurlNative.Multi.SocketAction(_multiHandle, SafeSocketHandle.Invalid, 0, out int _);

            Logger.Debug("Starting libuv loop...");
            _loop.RunDefault();
        }

        /// <summary>
        ///     SOCKETFUNCTION implementation.
        /// </summary>
        /// <param name="easy"></param>
        /// <param name="sockfd"></param>
        /// <param name="what"></param>
        /// <param name="userp"></param>
        /// <param name="socketp"></param>
        /// <returns></returns>
        private int HandleSocket(IntPtr easy, IntPtr sockfd, CURLpoll what, IntPtr userp, IntPtr socketp)
        {
            Logger.Trace(
                $"Called {nameof(CURLMoption.SOCKETFUNCTION)}. We need to poll for {what} on socket {sockfd}.");

            switch (what)
            {
                case CURLpoll.IN:
                case CURLpoll.OUT:
                case CURLpoll.INOUT:
                    PollMask events = 0;

                    if (what != CURLpoll.IN)
                    {
                        events |= PollMask.Writable;
                    }
                    if (what != CURLpoll.OUT)
                    {
                        events |= PollMask.Readable;
                    }

                    Logger.Trace($"Polling socket {sockfd} using libuv with mask {events}.");

                    _socketMap.GetOrCreatePoll(sockfd, _loop).Start(events, (poll, status) =>
                    {
                        CURLcselect flags = 0;

                        if (status.Mask.HasFlag(PollMask.Readable))
                        {
                            flags |= CURLcselect.IN;
                        }
                        if (status.Mask.HasFlag(PollMask.Writable))
                        {
                            flags |= CURLcselect.OUT;
                        }

                        Logger.Trace($"Finished polling socket {sockfd}.");
                        CurlNative.Multi.SocketAction(_multiHandle, sockfd, flags, out int _);
                        CheckMultiInfo();
                    });
                    break;
                case CURLpoll.REMOVE:
                    Logger.Trace($"Removing poll of socket {sockfd}.");
                    _socketMap.RemovePoll(sockfd);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(what));
            }

            return 0;
        }

        /// <summary>
        ///     TIMERFUNCTION implementation.
        /// </summary>
        /// <param name="multiHandle"></param>
        /// <param name="timeoutMs"></param>
        /// <param name="userp"></param>
        /// <returns></returns>
        private int StartTimeout(IntPtr multiHandle, int timeoutMs, IntPtr userp)
        {
            if (timeoutMs < 0)
            {
                Logger.Trace($"Called {nameof(CURLMoption.TIMERFUNCTION)} with timeout set to {timeoutMs}. "
                             + "Deleting our timer.");
                _timeout.Stop();
            }
            else if (timeoutMs == 0)
            {
                Logger.Trace($"Called {nameof(CURLMoption.TIMERFUNCTION)} with timeout set to {timeoutMs}. "
                             + "We should call curl_multi_socket_action or curl_multi_perform (once) as soon as possible.");

                CurlNative.Multi.SocketAction(_multiHandle, SafeSocketHandle.Invalid, 0, out int _);
                CheckMultiInfo();
            }
            else
            {
                Logger.Trace($"Called {nameof(CURLMoption.TIMERFUNCTION)} with timeout set to {timeoutMs} ms.");

                _timeout.Start(t =>
                {
                    CurlNative.Multi.SocketAction(_multiHandle, SafeSocketHandle.Invalid, 0, out int _);
                    CheckMultiInfo();
                }, timeoutMs, 0);
            }
            return 0;
        }

        private void CheckMultiInfo()
        {
            IntPtr pMessage;

            while ((pMessage = CurlNative.Multi.InfoRead(_multiHandle, out int _)) != IntPtr.Zero)
            {
                var message = Marshal.PtrToStructure<CurlNative.Multi.CURLMsg>(pMessage);
                if (message.msg != CURLMSG.DONE)
                {
                    throw new Exception($"Unexpected curl_multi_info_read result message: {message.msg}.");
                }

                var easy = _easyPool.GetSafeHandleFromPtr(message.easy_handle);

                /* Do not use message data after calling curl_multi_remove_handle() and
                           curl_easy_cleanup(). As per curl_multi_info_read() docs:
                           "WARNING: The data the returned pointer points to will not survive
                           calling curl_multi_cleanup, curl_multi_remove_handle or
                           curl_easy_cleanup." */

                _easyPool.GetAssignedContext(easy, out T requestContext);
                var action = _responseConsumer.OnComplete(easy, requestContext, message.data.result);

                if (action == HandleCompletedAction.ReuseHandleAndRetry)
                {
                    CurlNative.Multi.RemoveHandle(_multiHandle, easy);
                    CurlNative.Multi.AddHandle(_multiHandle, easy);
                }
                else if (action == HandleCompletedAction.ResetHandleAndNext)
                {
                    CurlNative.Multi.RemoveHandle(_multiHandle, easy);
                    CurlNative.Easy.Reset(easy);
                    _easyPool.UnassignContext(easy);
                    _easyPool.Free(easy);
                    Refill();
                }
            }
        }

        private void Refill()
        {
            if (!_easyPool.TryTakeFree(out SafeEasyHandle easy))
            {
                return; // All handles are in use.
            }

            _loop.CreateWorkRequest(work =>
                {
                    _oneRequestPullAtOnce.Wait();
                    var result = _requestProvider.MoveNextAsync(easy).Result;
                    work.UserToken = (result, _requestProvider.Current);
                    _oneRequestPullAtOnce.Release();
                },
                work =>
                {
                    var result = ((bool HasNext, T Next)) work.UserToken;
                    if (result.HasNext)
                    {
                        _easyPool.AssignContext(easy, result.Next);
                        CurlNative.Multi.AddHandle(_multiHandle, easy);
                        Refill();
                    }
                });
        }
    }
}