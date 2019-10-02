using System;
using System.Threading;

namespace NKristek.Smaragd.Helpers
{
    /// <inheritdoc />
    /// <summary>
    /// A class which provides an easier way to implement the <see cref="IDisposable"/> interface.
    /// </summary>
    internal abstract class Disposable
        : IDisposable
    {
        private const int Undisposed = 0, Disposed = 1;

        private volatile int _disposeState;

        /// <summary>
        /// If this disposable instance is already disposed.
        /// </summary>
        protected bool IsDisposed => _disposeState != Undisposed;

        /// <inheritdoc />
        ~Disposable()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposeState, Disposed) != Undisposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        /// <param name="managed">If managed resources should be disposed. A managed resource is another managed type, which implements <see cref="IDisposable"/>.</param>
        protected abstract void Dispose(bool managed = true);

        /// <summary>
        /// Throw an <see cref="ObjectDisposedException"/> if this disposable instance is already disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType()?.Name ?? nameof(IDisposable));
        }
    }
}
