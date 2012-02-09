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

        public static void EnsureDisposed(this Object value)
        {
            var disposable = value as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
