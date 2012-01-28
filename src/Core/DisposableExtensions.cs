using System;

namespace InfinityMQ
{
    internal static class DisposableExtensions
    {
        public static void DisposeIfSet(this IDisposable disposable)
        {
            if(disposable != null)
                disposable.Dispose();
        }
    }
}
