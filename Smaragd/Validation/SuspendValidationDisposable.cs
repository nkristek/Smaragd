using System;
using NKristek.Smaragd.Helpers;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Validation
{
    /// <summary>
    /// This <see cref="IDisposable"/> suspends validations of the given <see cref="ValidatingViewModel"/> until disposed
    /// </summary>
    internal class SuspendValidationDisposable
        : Disposable
    {
        private WeakReference<ValidatingViewModel> _validatingViewModel;
        
        private ValidatingViewModel ValidatingViewModel
        {
            get
            {
                if (_validatingViewModel != null && _validatingViewModel.TryGetTarget(out var viewModel))
                    return viewModel;
                return null;
            }

            set
            {
                if (ValidatingViewModel == value) return;
                _validatingViewModel = value != null ? new WeakReference<ValidatingViewModel>(value) : null;
            }
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="validatingViewModel"><see cref="ValidatingViewModel"/> of which the validations should be suspended</param>
        internal SuspendValidationDisposable(ValidatingViewModel validatingViewModel)
        {
            if (validatingViewModel == null)
                throw new ArgumentNullException(nameof(validatingViewModel));

            validatingViewModel.ValidationSuspended = true;
            ValidatingViewModel = validatingViewModel;
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            var viewModel = ValidatingViewModel;
            if (viewModel == null)
                return;

            viewModel.ValidationSuspended = false;
            viewModel.Validate();
        }
    }
}
