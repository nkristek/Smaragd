using System;

namespace NKristek.Smaragd.Helpers
{
    /// <inheritdoc />
    /// <remarks>
    /// Dispose resources using an <see cref="Action"/>.
    /// </remarks>
    internal sealed class ActionDisposable
        : Disposable
    {
        private Action _disposeManagedResourcesAction;

        private Action _disposeNativeResourcesAction;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDisposable" /> class with actions to dispose resources.
        /// </summary>
        /// <param name="disposeManagedResourcesAction">Action to dispose managed resources. A managed resource is another managed type, which implements <see cref="IDisposable"/>.</param>
        /// <param name="disposeNativeResourcesAction">Action to dispose native resources. Native resources are anything outside the managed world such as native Windows handles etc.</param>
        public ActionDisposable(Action disposeManagedResourcesAction, Action disposeNativeResourcesAction = null)
        {
            _disposeManagedResourcesAction = disposeManagedResourcesAction;
            _disposeNativeResourcesAction = disposeNativeResourcesAction;
        }

        /// <inheritdoc />
        protected override void Dispose(bool managed = true)
        {
            _disposeNativeResourcesAction?.Invoke();
            if (managed)
                _disposeManagedResourcesAction?.Invoke();

            _disposeNativeResourcesAction = null;
            _disposeManagedResourcesAction = null;
        }
    }
}
