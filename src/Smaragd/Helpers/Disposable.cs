using System;

namespace NKristek.Smaragd.Helpers
{
    /// <inheritdoc />
    /// <summary>
    /// A class which provides an easier way to implement the <see cref="IDisposable"/> interface.
    /// </summary>
    internal abstract class Disposable
        : IDisposable
    {
        /// <summary>
        /// Override to dispose managed resources.
        /// A managed resource is another managed type, which implements <see cref="IDisposable"/>.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
        }

        /// <summary>
        /// Override to dispose native resources.
        /// Native resources are anything outside the managed world such as native Windows handles etc.
        /// </summary>
        protected virtual void DisposeNativeResources()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposable()
        {
            Dispose(false);
        }

        private void Dispose(bool disposeManagedResources)
        {
            DisposeNativeResources();
            if (disposeManagedResources)
                DisposeManagedResources();
        }
    }
}
