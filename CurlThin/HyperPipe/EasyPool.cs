using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CurlThin.SafeHandles;

namespace CurlThin.HyperPipe
{
    internal class EasyHandleContext<T>
    {
        public EasyHandleContext(SafeEasyHandle easy)
        {
            Easy = easy;
        }

        public SafeEasyHandle Easy { get; }

        public bool IsUsed { get; set; } = false;

        public T CurrentRequest { get; set; } = default(T);
    }

    internal class EasyPool<T> : IDisposable
    {
        private readonly ImmutableDictionary<IntPtr, EasyHandleContext<T>> _pool;

        public EasyPool(int size)
        {
            Size = size;
            var poolBuilder = ImmutableDictionary.CreateBuilder<IntPtr, EasyHandleContext<T>>(
                new IntPtrEqualityComparer());

            for (var i = 0; i < size; i++)
            {
                var handle = CurlNative.Easy.Init();
                poolBuilder.Add(handle.DangerousGetHandle(), new EasyHandleContext<T>(handle));
            }

            _pool = poolBuilder.ToImmutable();
        }

        public int Size { get; }

        public EasyHandleContext<T> this[IntPtr handle] => _pool[handle];

        public IEnumerable<EasyHandleContext<T>> NotInUse => _pool.Values.Where(c => !c.IsUsed);

        public void Dispose()
        {
            foreach (var easyHandle in _pool)
            {
                CurlNative.Easy.Cleanup(easyHandle.Key);
            }
        }
    }
}