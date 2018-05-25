using System;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase
{
    internal class SuspendNotificationsDisposable
        : Disposable
    {
        private WeakReference<BindableBase> _bindable;
        /// <summary>
        /// The <see cref="BindableBase"/> of which the <see cref="BindableBase.PropertyChanged"/> notifications should be suspended.
        /// </summary>
        internal BindableBase Bindable
        {
            get
            {
                if (_bindable != null && _bindable.TryGetTarget(out var bindable))
                    return bindable;
                return null;
            }

            private set
            {
                if (Bindable == value) return;
                _bindable = value != null ? new WeakReference<BindableBase>(value) : null;
            }
        }

        internal SuspendNotificationsDisposable(BindableBase bindable)
        {
            bindable.PropertyChangedNotificationsSuspended = true;
            Bindable = bindable;
        }

        protected override void DisposeManagedResources()
        {
            Bindable.PropertyChangedNotificationsSuspended = false;
        }
    }
}
