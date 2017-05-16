using System;
using System.Collections.Concurrent;
using NetUV.Core.Handles;

namespace CurlThin.HyperPipe
{
    internal class SocketPollMap : IDisposable
    {
        private readonly ConcurrentDictionary<IntPtr, Poll> _sockets
            = new ConcurrentDictionary<IntPtr, Poll>(new IntPtrEqualityComparer());

        public void Dispose()
        {
            foreach (var poll in _sockets.Values)
            {
                poll.Stop();
                poll.Dispose();
            }
        }

        public Poll GetOrCreatePoll(IntPtr sockfd, Loop loop)
        {
            if (!_sockets.TryGetValue(sockfd, out Poll poll))
            {
                poll = loop.CreatePoll(sockfd);
                _sockets.TryAdd(sockfd, poll);
            }

            return poll;
        }

        public void RemovePoll(IntPtr sockfd)
        {
            if (_sockets.TryRemove(sockfd, out Poll poll))
            {
                poll.Stop();
                poll.Dispose();
            }
        }
    }
}