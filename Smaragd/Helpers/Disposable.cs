using System;

namespace NKristek.Smaragd.Helpers
{
    /// <inheritdoc />
    /// <summary>
    /// This class provides a basic implementation of <see cref="T:System.IDisposable" />.
    /// </summary>
    internal abstract class Disposable
        : IDisposable
    {
        /// <summary>
        /// <para>
        /// Dispose managed resources.
        /// </para>
        /// <para>
        /// A managed resource is another managed type, which implements <see cref="IDisposable"/>.
        /// </para>
        /// </summary>
        protected virtual void DisposeManagedResources() { }

        /// <summary>
        /// <para>
        /// Dispose native resources.
        /// </para>
        /// <para>
        /// Native resources are anything outside the managed world such as native Windows handles etc.
        /// </para>
        /// </summary>
        protected virtual void DisposeNativeResources() { }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        ~Disposable()
        {
            Dispose(false);
        }
        
        private void Dispose(bool managed)
        {
            DisposeNativeResources();
            if (managed)
                DisposeManagedResources();
        }
    }
}
