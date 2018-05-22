using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using nkristek.MVVMBase.Attributes;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// This <see cref="ViewModel"/> implements <see cref="IDataErrorInfo"/> and <see cref="INotifyDataErrorInfo"/>. To set validation errors use <see cref="SetValidationError"/> or <see cref="SetValidationErrors"/> in the property setter of the property which should be validated. Additionally, in <see cref="Validate"/> all validation logic should be executed.
    /// </summary>
    public abstract class ValidatingViewModel
        : ViewModel, IDataErrorInfo, INotifyDataErrorInfo
    {
        /// <summary>
        /// Classes implementing this method should execute all validation logic in this method.
        /// </summary>
        public abstract void Validate();

        /// <summary>
        /// If data in this <see cref="ViewModel"/> is valid
        /// </summary>
        [PropertySource(nameof(HasErrors))]
        public bool IsValid => !HasErrors;

        private readonly Dictionary<string, List<string>> _validationErrors = new Dictionary<string, List<string>>();

        /// <summary>
        /// Set the validation error of the property
        /// </summary>
        /// <param name="error">This is the validation error and has to be set to null if no validation error occured</param>
        /// <param name="propertyName">Name of the property which validates with this error</param>
        protected void SetValidationError(string error, [CallerMemberName] string propertyName = null)
        {
            SetValidationErrors(Enumerable.Repeat(error, 1), propertyName);
        }

        /// <summary>
        /// Set multiple validation errors of the property
        /// </summary>
        /// <param name="errors">These are the validation errors and has to be empty if no validation error occured</param>
        /// <param name="propertyName">Name of the property which validates with this error</param>
        protected void SetValidationErrors(IEnumerable<string> errors, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                return;

            var errorList = errors.ToList();
            if (errorList.Any(e => !String.IsNullOrEmpty(e)))
                _validationErrors[propertyName] = errorList;
            else
                _validationErrors.Remove(propertyName);

            RaisePropertyChanged(nameof(HasErrors));
            RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Raises an event on the ErrorsChanged event
        /// </summary>
        protected void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Concatenated validation errors
        /// </summary>
        public string Error
        {
            get { return String.Join(", ", _validationErrors.Select(kvp => $"{ kvp.Key }: { String.Join(", ", kvp.Value) }")); }
        }

        /// <summary>
        /// Get validation error by the name of the property
        /// </summary>
        /// <param name="columnName">Name of the property</param>
        /// <returns></returns>
        public string this[string columnName] => _validationErrors.ContainsKey(columnName) ? String.Join("\n", _validationErrors[columnName]) : String.Empty;

        /// <summary>
        /// Event that gets fired when the validation errors change
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets validation errors from a specified property
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>Validation errors from the property</returns>
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            return _validationErrors.ContainsKey(propertyName) ? _validationErrors[propertyName] : null;
        }

        /// <summary>
        /// Returns if there are any validation errors
        /// </summary>
        public bool HasErrors => _validationErrors.Any();

        protected override IEnumerable<string> GetIsDirtyIgnoredPropertyNames()
        {
            return base.GetIsDirtyIgnoredPropertyNames().Concat(new[] { nameof(HasErrors), nameof(IsValid) });
        }
    }
}
