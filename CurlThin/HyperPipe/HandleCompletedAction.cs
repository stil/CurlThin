namespace CurlThin.HyperPipe
{
    /// <summary>
    ///     What should be done after handle has completed its work?
    /// </summary>
    public enum HandleCompletedAction
    {
        /// <summary>
        ///     Reuse the handle with all its options unchanged and attach it to <see cref="HyperPipe" /> again.
        ///     Useful for example if your request has failed and you want to try again.
        ///     If you return this value, <see cref="CurlThin.HyperPipe" /> won't call <see cref="IRequestProvider{T}.TryNext" />.
        /// </summary>
        ReuseHandleAndRetry,

        /// <summary>
        ///     Reset the handle with <see cref="CurlNative.Easy.Reset" />. You'll have to set up the handle from scratch in your
        ///     implementation of <see cref="IRequestProvider{T}.TryNext" />.
        /// </summary>
        ResetHandleAndNext
    }
}