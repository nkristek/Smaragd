using System;
using NKristek.Smaragd.Helpers;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Validation
{
    /// <inheritdoc />
    /// <summary>
    /// This <see cref="T:System.IDisposable" /> suspends validations of the given <see cref="P:NKristek.Smaragd.Validation.SuspendValidationDisposable.ValidatingViewModel" /> until disposed.
    /// </summary>
    internal class SuspendValidationDisposable
        : Disposable
    {
        private readonly ValidatingViewModel _validatingViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuspendValidationDisposable"/> class with the <paramref name="validatingViewModel"/> on which the validations should be suspended.
        /// </summary>
        /// <param name="validatingViewModel">A <see cref="ValidatingViewModel"/> of which the validations should be suspended.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="validatingViewModel"/> is null.</exception>
        internal SuspendValidationDisposable(ValidatingViewModel validatingViewModel)
        {
            if (validatingViewModel == null)
                throw new ArgumentNullException(nameof(validatingViewModel));

            validatingViewModel.ValidationSuspended = true;
            _validatingViewModel = validatingViewModel;
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            _validatingViewModel.ValidationSuspended = false;
        }
    }
}
