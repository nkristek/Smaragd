using System;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase
{
    internal class SuspendValidationDisposable
        : Disposable
    {
        private WeakReference<ValidatingViewModel> _validatingViewModel;
        /// <summary>
        /// The <see cref="ValidatingViewModel"/> of which the validation should be suspended.
        /// </summary>
        internal ValidatingViewModel ValidatingViewModel
        {
            get
            {
                if (_validatingViewModel != null && _validatingViewModel.TryGetTarget(out var viewModel))
                    return viewModel;
                return null;
            }

            private set
            {
                if (ValidatingViewModel == value) return;
                _validatingViewModel = value != null ? new WeakReference<ValidatingViewModel>(value) : null;
            }
        }

        internal SuspendValidationDisposable(ValidatingViewModel validatingViewModel)
        {
            validatingViewModel.ValidationSuspended = true;
            ValidatingViewModel = validatingViewModel;
        }

        protected override void DisposeManagedResources()
        {
            ValidatingViewModel.ValidationSuspended = false;
            ValidatingViewModel.Validate();
        }
    }
}
